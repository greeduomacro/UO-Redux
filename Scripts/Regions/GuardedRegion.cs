using Server.Commands;
using Server.Mobiles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

//using Ulmeta.Alliances.Guards;

namespace Server.Regions
{
    public class GuardedRegion : BaseRegion
    {
        private static object[] m_GuardParams = new object[1];
        private Type m_GuardType;
        private bool m_Disabled;

        public bool Disabled { get { return m_Disabled; } set { m_Disabled = value; } }

        public virtual bool IsDisabled()
        {
            return m_Disabled;
        }

        public static void Initialize()
        {
            CommandSystem.Register("CheckGuarded", AccessLevel.GameMaster, new CommandEventHandler(CheckGuarded_OnCommand));
            CommandSystem.Register("SetGuarded", AccessLevel.Administrator, new CommandEventHandler(SetGuarded_OnCommand));
            CommandSystem.Register("ToggleGuarded", AccessLevel.Administrator, new CommandEventHandler(ToggleGuarded_OnCommand));
        }

        [Usage("CheckGuarded")]
        [Description("Returns a value indicating if the current region is guarded or not.")]
        private static void CheckGuarded_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            GuardedRegion reg = (GuardedRegion)from.Region.GetRegion(typeof(GuardedRegion));

            if(reg == null)
                from.SendMessage("You are not in a guardable region.");
            else if(reg.Disabled)
                from.SendMessage("The guards in this region have been disabled.");
            else
                from.SendMessage("This region is actively guarded.");
        }

        [Usage("SetGuarded <true|false>")]
        [Description("Enables or disables guards for the current region.")]
        private static void SetGuarded_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if(e.Length == 1)
            {
                GuardedRegion reg = (GuardedRegion)from.Region.GetRegion(typeof(GuardedRegion));

                if(reg == null)
                {
                    from.SendMessage("You are not in a guardable region.");
                }
                else
                {
                    reg.Disabled = !e.GetBoolean(0);

                    if(reg.Disabled)
                        from.SendMessage("The guards in this region have been disabled.");
                    else
                        from.SendMessage("The guards in this region have been enabled.");
                }
            }
            else
            {
                from.SendMessage("Format: SetGuarded <true|false>");
            }
        }

        [Usage("ToggleGuarded")]
        [Description("Toggles the state of guards for the current region.")]
        private static void ToggleGuarded_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            GuardedRegion reg = (GuardedRegion)from.Region.GetRegion(typeof(GuardedRegion));

            if(reg == null)
            {
                from.SendMessage("You are not in a guardable region.");
            }
            else
            {
                reg.Disabled = !reg.Disabled;

                if(reg.Disabled)
                    from.SendMessage("The guards in this region have been disabled.");
                else
                    from.SendMessage("The guards in this region have been enabled.");
            }
        }

        public static GuardedRegion Disable(GuardedRegion reg)
        {
            reg.Disabled = true;
            return reg;
        }

        public virtual bool AllowReds { get { return true; } }

        public virtual bool CheckVendorAccess(BaseVendor vendor, Mobile from)
        {
            if(from.AccessLevel >= AccessLevel.GameMaster || IsDisabled())
                return true;

            return (from.Kills < 5 || AllowReds);
        }

        public virtual Type DefaultGuardType
        {
            get
            {
                return typeof(MilitiaWarrior);
            }
        }

        public GuardedRegion(string name, Map map, int priority, params Rectangle3D[] area)
            : base(name, map, priority, area)
        {
            m_GuardType = DefaultGuardType;
        }

        public GuardedRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
            XmlElement el = xml["guards"];

            if(ReadType(el, "type", ref m_GuardType, false))
            {
                if(!typeof(Mobile).IsAssignableFrom(m_GuardType))
                {
                    Console.WriteLine("Invalid guard type for region '{0}'", this);
                    m_GuardType = DefaultGuardType;
                }
            }
            else
            {
                m_GuardType = DefaultGuardType;
            }

            bool disabled = false;
            if(ReadBoolean(el, "disabled", ref disabled, false))
                this.Disabled = disabled;
        }

        public override bool OnBeginSpellCast(Mobile m, ISpell s)
        {
            if(!IsDisabled() && !s.OnCastInTown(this))
            {
                m.SendLocalizedMessage(500946); // You cannot cast this in town!
                return false;
            }

            return base.OnBeginSpellCast(m, s);
        }

        public override bool AllowHousing(Mobile from, Point3D p)
        {
            if(this.Name.Contains("Palace"))
                return true;

            return false;
        }

        public override void MakeGuard(Mobile focus)
        {
            SummonGuard(null, focus);
        }

        public virtual void SummonGuard(Mobile caller, Mobile aggressor)
        {
            IPooledEnumerable eable;

            if(caller != null)
                eable = caller.GetMobilesInRange(14);
            else
                eable = aggressor.GetMobilesInRange(14);

            foreach(Mobile m in eable)
            {
                if(m is BaseGuard)
                {
                    if(((BaseGuard)m).Focus == null)
                        ((BaseGuard)m).Focus = aggressor;
                }
            }

            eable.Free();
        }

        public override void OnEnter(Mobile m)
        {
            if(IsDisabled())
                return;

            if(!AllowReds && m.Kills >= 5)
                MakeGuard(m);
        }

        public override void OnExit(Mobile m)
        {
            if(IsDisabled())
                return;
        }

        public override void OnSpeech(SpeechEventArgs args)
        {
            if(IsDisabled())
                return;

            if(args.Mobile.Player && args.Mobile.Alive && args.HasKeyword(0x0007))
                CallGuards(args.Mobile);

            if(args.Mobile.Alive && args.HasKeyword(0x0007)) // *guards*
                CallGuards(args.Mobile.Location);
        }

        public override void OnAggressed(Mobile aggressor, Mobile aggressed, bool criminal)
        {
            base.OnAggressed(aggressor, aggressed, criminal);

            if(!IsDisabled() && aggressor != aggressed && criminal)
                CheckGuardCandidate(aggressor);
        }

        public override void OnGotBeneficialAction(Mobile helper, Mobile helped)
        {
            base.OnGotBeneficialAction(helper, helped);

            if(IsDisabled())
                return;

            int noto = Notoriety.Compute(helper, helped);

            if(helper != helped && (noto == Notoriety.Criminal || noto == Notoriety.Murderer))
                CheckGuardCandidate(helper);
        }

        public override void OnCriminalAction(Mobile m, bool message)
        {
            base.OnCriminalAction(m, message);

            if(!IsDisabled())
                CheckGuardCandidate(m);
        }

        private Dictionary<Mobile, GuardTimer> m_GuardCandidates = new Dictionary<Mobile, GuardTimer>();

        public void CheckGuardCandidate(Mobile m)
        {
            if(IsDisabled())
                return;

            if(IsGuardCandidate(m))
            {
                GuardTimer timer = null;
                m_GuardCandidates.TryGetValue(m, out timer);

                if(timer == null)
                {
                    timer = new GuardTimer(m, m_GuardCandidates);
                    timer.Start();

                    m_GuardCandidates[m] = timer;

                    Map map = m.Map;

                    if(map != null)
                    {
                        Mobile fakeCall = null;
                        double prio = 0.0;

                        foreach(Mobile v in m.GetMobilesInRange(8))
                        {
                            if(!v.Player && v.Body.IsHuman && v != m && !IsGuardCandidate(v))
                            {
                                double dist = m.GetDistanceToSqrt(v);

                                if(fakeCall == null || dist < prio)
                                {
                                    fakeCall = v;
                                    prio = dist;
                                }
                            }
                        }

                        if(fakeCall != null)
                        {
                            if(fakeCall is BaseVendor && !(fakeCall is BaseGuard))
                                fakeCall.Say(Utility.RandomList(1007037, 501603, 1013037, 1013038, 1013039, 1013041, 1013042, 1013043, 1013052));

                            MakeGuard(m);
                            timer.Stop();
                            m_GuardCandidates.Remove(m);
                        }
                    }
                }
                else
                {
                    timer.Stop();
                    timer.Start();
                }
            }
        }

        public void CallGuards(Mobile m)
        {
            if(IsDisabled())
                return;

            ArrayList aggressors = new ArrayList(m.Aggressors);

            foreach(AggressorInfo ai in aggressors)
            {
                SummonGuard(m, ai.Attacker);
                m_GuardCandidates.Remove(ai.Attacker);
            }
        }

        public void CallGuards(Point3D p)
        {
            if(IsDisabled())
                return;

            IPooledEnumerable eable = Map.GetMobilesInRange(p, 14);

            foreach(Mobile m in eable)
            {
                if(IsGuardCandidate(m) && ((!AllowReds && m.Kills >= 5) || m_GuardCandidates.ContainsKey(m)))
                {
                    MakeGuard(m);
                    m_GuardCandidates.Remove(m);
                    m.SendLocalizedMessage(502276); // Guards can no longer be called on you.
                    break;
                }
            }

            eable.Free();
        }

        public bool IsGuardCandidate(Mobile m)
        {
            if(m is BaseGuard || !m.Alive || m.AccessLevel > AccessLevel.Player || m.Blessed || IsDisabled() || ((m.GuildFealty != null) && m.GuildFealty.Serial == 0x7AF))
                return false;

            return (!AllowReds && m.Kills >= 5) || m.Criminal;
        }

        private class GuardTimer : Timer
        {
            private Mobile m_Mobile;
            private Dictionary<Mobile, GuardTimer> m_Table;

            public GuardTimer(Mobile m, Dictionary<Mobile, GuardTimer> table)
                : base(TimeSpan.FromSeconds(15.0))
            {
                Priority = TimerPriority.TwoFiftyMS;

                m_Mobile = m;
                m_Table = table;
            }

            protected override void OnTick()
            {
                if(m_Table.ContainsKey(m_Mobile))
                {
                    m_Table.Remove(m_Mobile);
                    m_Mobile.SendLocalizedMessage(502276); // Guards can no longer be called on you.
                }
            }
        }
    }
}