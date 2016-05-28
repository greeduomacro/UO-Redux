using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using System.Linq;
using System.Text;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.ContextMenus;

namespace Redux.ContextMenus
{
    public class RaceAbilityEntry : ContextMenuEntry
    {
        private Player pm;
        List<Mobile> mobList;

        public RaceAbilityEntry(Player from)
            : base(1063491, -1)
        {
            pm = from;
        }

        public override void OnClick()
        {
            Race race = ((Player)pm).Race;

            switch (race)
            {
                case Race.Ogre:
                    {
                        pm.Target = new OgreTarget();
                        pm.SendMessage("Target a creature to bash!");
                        break;
                    }

                case Race.Terathan:
                    {
                        mobList = new List<Mobile>();

                        foreach (Mobile mobile in pm.GetMobilesInRange(8))
                        {
                            if (mobile.AccessLevel > pm.AccessLevel)
                                continue;
                            else if (mobile.Blessed || !mobile.Alive)
                                continue;
                            else if (mobile == pm)
                                continue;
                            else
                                mobList.Add(mobile);
                        }

                        if (mobList != null)
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds(8.0), new TimerCallback(FinishWebs));

                            pm.PublicOverheadMessage(MessageType.Regular, pm.EmoteHue, false, "*Begins hurling webs at nearby creatures*");

                            for (int i = 0; i < mobList.Count; i++)
                            {
                                Mobile m = mobList[i];

                                if (m is PlayerMobile && 0.51 > Utility.RandomDouble())
                                {
                                    m.SendMessage("You've been struck by a terathan web!");
                                }

                                m.CantWalk = true;

                                Effects.SendMovingEffect(pm, m, 4308, 0, 10, false, false);
                            }

                            Effects.PlaySound(pm.Location, pm.Map, 0x027);

                            pm.NextRaceAbility = TimeSpan.FromMinutes(1);

                        }

                        break;
                    }
                case Race.Lich:
                    {
                        if (pm.Followers >= pm.FollowersMax)
                        {
                            pm.SendMessage("You already have too many followers to summon more.");
                        }

                        else
                        {
                            do
                            {
                                BaseCreature bc = null;

                                switch (Utility.Random(3))
                                {
                                    case 0: bc = new Skeleton(); break;
                                    case 1: bc = new Zombie(); break;
                                    case 2: bc = new Wraith(); break;
                                }

                                if (bc != null && BaseCreature.Summon(bc, true, pm, Point3D.Zero, 0x1E2, TimeSpan.FromMinutes(1.5)))
                                {
                                    bc.MoveToWorld(new Point3D(pm.X + Utility.RandomMinMax(1, 3), pm.Y - Utility.RandomMinMax(0, 3), pm.Z), pm.Map);

                                    Effects.SendLocationEffect(pm.Location, pm.Map, 0x3709, 15, 945, 0);
                                }

                            } 
                            
                            while (pm.Followers < pm.FollowersMax);

                            pm.NextRaceAbility = TimeSpan.FromMinutes(2.0);
                        }
                        break;
                    }

                case Race.HalfDaemon:
                    {
                        if (pm.AbilityActive)
                        {
                            pm.RaceBody = 0;
                            pm.BodyDamageBonus = 0;
                            pm.AdjustBody();

                            pm.Str -= 40;
                            pm.Dex -= 20;
                            pm.Int += 60;

                            pm.AbilityActive = false;

                            pm.PublicOverheadMessage(MessageType.Regular, pm.EmoteHue, false, String.Format("*{0} begins to shrink, taking the form of a human*", pm.Name));

                        }

                        else
                        {
                            for (int x = 1; x <= 2; x++)
                            {
                                Item toDisarm = pm.FindItemOnLayer(Layer.OneHanded);

                                if (toDisarm == null || !toDisarm.Movable)
                                    toDisarm = pm.FindItemOnLayer(Layer.TwoHanded);

                                Container pack = pm.Backpack;
                                pack.DropItem(toDisarm);
                            }

                            pm.RaceBody = 792;
                            pm.AdjustBody();
                            pm.BodyDamageBonus = 36;

                            pm.Str += 40;
                            pm.Dex += 20;
                            pm.Int -= 60;

                            pm.AbilityActive = true;

                            pm.PublicOverheadMessage(MessageType.Regular, pm.EmoteHue, false, String.Format("*{0}'s flesh begins to buldge and tear as something emerges from within*", pm.Name));

                        }

                        break;
                    }

                case Race.Shapeshifter:
                    {
                        pm.Target = new ShapeshifterTarget();
                        pm.SendMessage("Select a creature to assume their form.");
                        break;
                    }

                case Race.Marid:
                    {
                        if (pm.AbilityActive)
                        {
                            pm.AbilityActive = false;
                            pm.CanSwim = false;
                            Effects.SendLocationParticles(EffectItem.Create(pm.Location, pm.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);

                            try
                            {
                                IMount mount = pm.Mount;
                                mount.Rider = null;
                                ((EtherealMount)mount).Delete();
                            }

                            catch { }
                        }

                        else
                        {
                            for (int x = 1; x <= 2; x++)
                            {
                                Item toDisarm = pm.FindItemOnLayer(Layer.OneHanded);

                                if (toDisarm == null || !toDisarm.Movable)
                                    toDisarm = pm.FindItemOnLayer(Layer.TwoHanded);

                                Container pack = pm.Backpack;
                                pack.DropItem(toDisarm);
                            }

                            pm.AbilityActive = true;
                            pm.PublicOverheadMessage(MessageType.Regular, pm.EmoteHue, false, String.Format("*A mass of vapors condenses under {0}, forming a steed*", pm.Name));
                            pm.CanSwim = true;

                            bool isWater = false;

                            Map map = pm.Map;
                            LandTile land = map.Tiles.GetLandTile(pm.X, pm.Y);
                            StaticTile[] tiles = map.Tiles.GetStaticTiles(pm.X, pm.Y);

                            isWater = (land.Z == pm.Z && ((land.ID >= 168 && land.ID <= 171) || (land.ID >= 310 && land.ID <= 311)));

                            for (int i = 0; i < tiles.Length; ++i)
                            {
                                StaticTile tile = tiles[i];
                                isWater = (tile.ID >= 0x1796 && tile.ID <= 0x17B2);
                            }

                            try
                            {
                                if (isWater)
                                {
                                    EtherealMount seaHorse = new EtherealSeaHorse();
                                    seaHorse.MoveToWorld(pm.Location);
                                    seaHorse.Rider = pm;
                                }

                                else
                                {
                                    EtherealMount horse = new EtherealHorse();
                                    horse.MoveToWorld(pm.Location);
                                    horse.Rider = pm;
                                }
                            }

                            catch { }
                        }

                        break;
                    }

                default: break;
            }
        }

        public void FinishWebs()
        {
            if (mobList != null)
            {
                for (int i = 0; i < mobList.Count; i++)
                {
                    mobList[i].CantWalk = false;
                }

                mobList.Clear();
            }
        }
    }

    public class ShapeshifterTarget : Target
    {
        private bool hasChanged = false;

        public ShapeshifterTarget()
            : base(12, false, TargetFlags.None)
        {
        }

        protected override void OnTarget(Mobile from, object target)
        {
            if (!from.CanSee(target))
            {
                from.SendMessage("Target cannot be seen.");
            }
            else if (!from.Alive)
            {
                from.SendMessage("You cannot transform out of your current state.");
            }
            else if (target is Mobile && ((Mobile)target).Alive)
            {
                Mobile t = (Mobile)target;

                if (t == from)
                {
                    from.HueMod = -1;

                    ((Player)from).RawStr -= ((Player)from).StrMod;
                    ((Player)from).RawDex -= ((Player)from).DexMod;
                    ((Player)from).RawInt -= ((Player)from).IntMod;

                    ((Player)from).RaceBody = 58;
                    ((Player)from).AdjustBody();

                    ((Player)from).StrMod = 0;
                    ((Player)from).DexMod = 0;
                    ((Player)from).IntMod = 0;

                    ((Player)from).BodyDamageBonus = 0;

                }

                else if (from.Mounted && !t.Body.IsHuman)
                {
                    from.SendMessage("You cannot transform while mounted.");
                }

                else
                {
                    from.FixedParticles(14089, 0, 30, 0, 1155, 7, EffectLayer.CenterFeet);

                    ((Player)from).RaceBody = t.Body;
                    ((Player)from).AdjustBody();

                    from.HueMod = t.Hue;

                    ((Player)from).RawStr -= ((Player)from).StrMod;
                    ((Player)from).RawDex -= ((Player)from).DexMod;
                    ((Player)from).RawInt -= ((Player)from).IntMod;

                    ((Player)from).StrMod = (int)(t.RawStr / 10);
                    ((Player)from).DexMod = (int)(t.RawDex / 10);
                    ((Player)from).IntMod = (int)(t.RawInt / 10);

                    from.RawStr += ((Player)from).StrMod;
                    from.RawDex += ((Player)from).DexMod;
                    from.RawInt += ((Player)from).IntMod;

                    if (t.Body.IsHuman == false)
                    {
                        ((Player)from).BodyDamageBonus = (int)(t.RawStr / 15);
                    }

                    if (t.Body.IsHuman)
                    {
                        from.HairItemID = t.HairItemID;
                        from.FacialHairItemID = t.FacialHairItemID;
                        from.HairHue = t.HairHue;
                        from.FacialHairHue = t.FacialHairHue;
                    }

                    from.PublicOverheadMessage(MessageType.Regular, from.EmoteHue, false, String.Format("*transforms into the shape of {0}*", t.RawName));
                    
                    hasChanged = true;

                    for (int x = 1; x <= 2; x++)
                    {
                        Item toDisarm = from.FindItemOnLayer(Layer.OneHanded);

                        if (toDisarm == null || !toDisarm.Movable)
                            toDisarm = from.FindItemOnLayer(Layer.TwoHanded);

                        Container pack = from.Backpack;
                        pack.DropItem(toDisarm);
                    }
                }
            }

            else
            {
                from.SendMessage("You cannot take that form.");
            }
        }

        protected override void OnTargetFinish(Mobile from)
        {
            if (hasChanged)
            {
                ((Player)from).NextRaceAbility = TimeSpan.FromSeconds(5);
                from.FixedParticles(14089, 0, 30, 0, 1155, 7, EffectLayer.CenterFeet);
            }
        }
    }

    public class OgreTarget : Target
    {
        public OgreTarget(): base( 1, false, TargetFlags.Harmful )
        {
        }

        protected override void OnTarget(Mobile from, object target)
        {
            if (target is Mobile && target != from)
            {
                Mobile defender = target as Mobile;
                Mobile attacker = from;

                attacker.SendLocalizedMessage(1060165);
                defender.SendLocalizedMessage(1060166); 

                defender.PlaySound(0x213);
                defender.FixedParticles(0x377A, 1, 32, 9949, 1153, 0, EffectLayer.Head);

                Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(defender.X, defender.Y, defender.Z + 10), defender.Map), new Entity(Serial.Zero, new Point3D(defender.X, defender.Y, defender.Z + 20), defender.Map), 0x36FE, 1, 0, false, false, 1133, 3, 9501, 1, 0, EffectLayer.Waist, 0x100);

                int damage = 10; // Base

                if (defender.HitsMax > 0)
                {
                    double hitsPercent = ((double)defender.Hits / (double)defender.HitsMax) * 100.0;

                    double manaPercent = 0;

                    if (defender.ManaMax > 0)
                        manaPercent = ((double)defender.Mana / (double)defender.ManaMax) * 100.0;

                    damage += Math.Min((int)(Math.Abs(hitsPercent - manaPercent) / 4), 20);
                }

                defender.Damage(damage + Utility.RandomMinMax(4, 8), attacker);
                attacker.DoHarmful(defender);

                StatMod intLoss = new StatMod(StatType.Int, "intloss", defender.Int / 3, TimeSpan.FromSeconds(30));
                defender.AddStatMod(intLoss);

                ((Player)from).NextRaceAbility = TimeSpan.FromMinutes(0.5);
            }

            else
            {
                from.SendMessage("You can not attack that.");
            }
        }
    }
}
