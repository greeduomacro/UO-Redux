using System;
using Server.Commands;
using Server.EssenceOfCharacter;
using Server.Events;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Multis;
using Server.Network;
using Server.Perks;
using Server.Engines.PartySystem;
using Server.Language;
using Server.ContextMenus;
using System.Collections;
using System.Collections.Generic;

using Redux.ContextMenus;
using Ulmeta.Guards;

namespace Server.Mobiles
{
    public enum PlayerFlag
    {
        None = 0x00000000,
        FameLocked = 0x00000001
    }

    public enum RaceBodies
    {
        None = 0,
        Human = 0,
        Ogre = 1,
        Terathan = 70,
        Lich = 24,
        HalfDaemon = 0,
        Shapeshifter = 58,
        Marid = 0,
        Planewalker = 0
    }

    public enum Race
    {
        None,
        Human,
        Ogre,
        Terathan,
        Lich,
        HalfDaemon,
        Shapeshifter,
        Marid,
        Planewalker
    }

    public class Player : PlayerMobile
    {        
        public static string[] AccessLevelLabels = new string[]
		{
			"Player", "Counselor", "GameMaster", "Seer", "Administrator"
		};

        #region Race

        private Race playerRace;
        private DateTime abilityTime;
        private bool abilityActive = false;
        private int raceBody = 0;

        private int strMod;
        private int dexMod;
        private int intMod;

        private int bodyDamageBonus;

        [CommandProperty(AccessLevel.Administrator)]
        public int BodyDamageBonus
        {
            get { return bodyDamageBonus; }
            set { bodyDamageBonus = value; }
        }

        public void AdjustBody()
        {
            if (Race == Race.None)
            {
                SendGump(new RaceSelectionGump(this));
            }

            this.BodyMod = raceBody;
        }

        [CommandProperty(AccessLevel.Administrator)]
        public int RaceBody
        {
            get { return raceBody; }
            set
            {
                raceBody = value;
            }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public int StrMod { get { return strMod; } set { strMod = value; } }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public int DexMod { get { return dexMod; } set { dexMod = value; } }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public int IntMod { get { return intMod; } set { intMod = value; } }

        [CommandProperty(AccessLevel.Administrator)]
        public Race Race 
        { 
            get { return playerRace; } 
            set 
            {
                playerRace = value; 
                RaceBody = (int)(Enum.Parse(typeof(RaceBodies), playerRace.ToString(), true)); 
                AdjustBody();
            } 
        }

        [CommandProperty(AccessLevel.Administrator)]
        public bool AbilityActive { get { return abilityActive; } set { abilityActive = value; } }

        [CommandProperty(AccessLevel.Administrator)]
        public TimeSpan NextRaceAbility
        {
            get
            {
                TimeSpan time = abilityTime - DateTime.Now;

                if (time < TimeSpan.Zero)
                    time = TimeSpan.Zero;

                return time;
            }
            set
            {
                try { abilityTime = DateTime.Now + value; }
                catch { }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this == from && Race == Race.Marid && AbilityActive)
                return;
            
            else base.OnDoubleClick(from);
        }

        #endregion

        #region Context Menu

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from == this)
            {
                Player pm = (Player)from;

                if (!pm.Mounted || (Race == Race.Marid && AbilityActive))
                    list.Add(new JumpEntry(pm));

                if (NextRaceAbility <= TimeSpan.Zero && pm.Race != Race.Human)
                    list.Add(new RaceAbilityEntry(pm));

                list.Add(new COVEntry(pm));
            }
        }

        #endregion

        #region KnockOut

        private int KOs = 0;

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public int KOCount { get { return KOs; } set { KOs = value; } }

        private class KOProtectionTimer : Timer
        {
            Mobile from;

            public KOProtectionTimer(Mobile m)
                : base(TimeSpan.FromSeconds(5))
            {
                Priority = TimerPriority.OneSecond;
                from = m;
                from.Blessed = true;
            }

            protected override void OnTick()
            {
                from.Blessed = false;
                from.SendMessage("You are again vulnerable to attack!");
                Stop();
            }
        }

        /// <summary>
        /// Overridden to also freeze the player and send the <code>Respawn Gump</code>
        /// </summary>
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            KOCount++;
            CantWalk = true;

            double koTime = KOCount * 2;

            if (LastKiller is Guard && Criminal)
                Criminal = false;

            SendGump(new KOGump(this, TimeSpan.FromSeconds(koTime)));
        }

        #endregion

        #region Language

        public enum PlayerLanguage
        {  
           Invalid,
           Common,
           Ancient,
           Tribal,
           Pagan,
           Glyph,
           Translate // Vii add
        }

        public int[] LevelofUnderstanding = new int[]
        {
          0, 100, 0, 0, 0, 0
        };

        private string m_culture;

        // Vii add
        [CommandProperty(AccessLevel.GameMaster)]
        public string Culture
        {
            get { return m_culture; }
            set { m_culture = value; }
        }
        // end Vii add

        private PlayerLanguage speakingLanguage = PlayerLanguage.Common;

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerLanguage CurrentLanguage
        {
            get { return speakingLanguage; }
            set { speakingLanguage = value; }
        }

        #endregion

        # region EoC

        private int m_EoC;
        private EoCLedger m_Ledger;

        /// <summary>
        /// Player's point value of Essence of Character
        /// </summary>
        public int EssenceOfCharacter
        {
            get { return m_EoC; }
            set { m_EoC = value; }
        }

        /// <summary>
        /// Shortcut to <code>EssenceOfCharacter</code>
        /// </summary>
        [CommandProperty(AccessLevel.GameMaster)]
        public int EoC
        {
            get { return EssenceOfCharacter; }
            set { EssenceOfCharacter = value; }
        }

        public EoCLedger EoCLedger
        {
            get { return m_Ledger; }
            set { m_Ledger = value; }
        }

        #endregion

        #region Jump

        /// <summary>
        /// Player's current vertical jump limit
        /// </summary>
        [CommandProperty(AccessLevel.Counselor)]
        public int JumpHeight
        {
            get
            {
                return this.Z + Math.Max(0, ((Str + Dex) / 10));
                   
            }
        }

        /// <summary>
        /// Player's current horizontal jump limit
        /// </summary>
        [CommandProperty(AccessLevel.Counselor)]
        public int JumpRange
        {
            get
            {
                int value = Math.Max(0, ((Str + Dex) / 50));

                Acrobat acr = Perk.GetByType<Acrobat>(this);

                if( acr != null )
                    value += acr.GetJumpRangeBonus();

                if ( this.Race == Race.Terathan )
                    value += 2;

                if ( this.Race == Race.Ogre )
                    value -= 1;

                if (value > 1)
                    return value;

                else return 2;
            }
        }

        #endregion

        #region Reputation

        /// <summary>
        /// Negative staff reputation
        /// </summary>
        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public int RepBad { get; set; }

        /// <summary>
        /// Positive staff reputation
        /// </summary>
        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public int RepGood { get; set; }

        #endregion

        private bool _produceCorpses = true;

        [CommandProperty(AccessLevel.Administrator)]
        public bool ProduceCorpses
        {
            get { return _produceCorpses; }
            set { _produceCorpses = value; }
        }

        #region Respawn

        private Point3D m_RespawnLoc;
        private Map m_RespawnMap;

        /// <summary>
        /// Timestamp for when the player last respawned
        /// </summary>
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastRespawn { get; set; }

        /// <summary>
        /// Expiration time of the <code>LastRespawn</code>
        /// </summary>
        public DateTime RespawnExpiry { get { return (LastRespawn + TimeSpan.FromMinutes(1.5)); } }

        /// <summary>
        /// Recorded <code>Point3D</code> for the player's respawn location
        /// </summary>
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D RespawnLocation { get; set; }

        /// <summary>
        /// Recorded <code>Map</code> for the player's respawn location
        /// </summary>
        [CommandProperty(AccessLevel.GameMaster)]
        public Map RespawnMap { get; set; }

        #endregion

        #region Flags

        private PlayerFlag m_Flags;

        /// <summary>
        /// Indicates if the player has locked their Fame from rising
        /// </summary>
        [CommandProperty(AccessLevel.GameMaster)]
        public bool FameLocked
        {
            get { return GetFlag(PlayerFlag.FameLocked); }
            set { SetFlag(PlayerFlag.FameLocked, value); }
        }

        /// <summary>
        /// Gets the state of a <code>PlayerFlag</code>
        /// </summary>
        public bool GetFlag( PlayerFlag flag )
        {
            return ((m_Flags & flag) != 0);
        }

        /// <summary>
        /// Sets the state of a <code>PlayerFlag</code>
        /// </summary>
        public void SetFlag( PlayerFlag flag, bool value )
        {
            if( value )
                m_Flags |= flag;
            else
                m_Flags &= flag;
        }

        #endregion

        public override bool OnEquip(Item item)
        {   
            if(item.Layer == Layer.OneHanded || item.Layer == Layer.TwoHanded)
            {
                if (Race == Race.Lich && (item is BaseStaff || item is BaseWand || item is Spellbook))
                    return base.OnEquip(item);

                else if (Race == Race.Lich && (!(item is BaseStaff) && !(item is BaseWand) && !(item is Spellbook)))
                {
                    SendMessage("As a Liche you may only wield spellbooks, staves and wand-type weapons.");
                    return false;
                }

                else if (!( RaceBody == 0 || RaceBody == 400 || RaceBody == 401 ))
                {
                    SendMessage("You may not equip this object in your current state.");
                    return false;
                }

                else return base.OnEquip(item);
            }

            else return base.OnEquip(item);
        }

        public override int StamMax
        {
            get
            {
                Acrobat acr = Perk.GetByType<Acrobat>(this);

                if (acr != null && acr.Olympian())
                {
                    return base.StamMax + 40;
                }

                else return base.StamMax;
            }
        }

        public override void OnStamChange(int oldValue)
        {
            if (oldValue > Stam)
            {
                if (Utility.RandomDouble() <= 0.01)
                {
                    bool coin = Utility.RandomBool();

                    if (coin)
                        Hunger--;

                    else Thirst--;

                    //Idk, some days I want it, some days I don't
                    SendMessage("You begin to feel your exertion depleting your body.");
                }
            }
        }

        public override int MaxWeight
        {
            get
            {
                Craftsman cm = Perk.GetByType<Craftsman>(this);

                int x = 50;

                if (cm != null && cm.WorkHorse())
                    x += 60;

                return (x + (int)(3.5 * this.Str)) + 40;
            }
        }

        public Player()
            : base()
        {
            EoCLedger = new EoCLedger(this);
        }

        /// <summary>
        /// serialization ctor
        /// </summary>
        public Player( Serial s ) : base(s) { }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int FollowersMax
        {
            get 
            {
                int loreBonus = (int)(this.Skills.AnimalLore.Value / 50);
                int shepherdBonus = (int)(this.Skills.Herding.Value / 50);

                Beastmaster bmr = Perk.GetByType<Beastmaster>(this);
                Summoner sum = Perk.GetByType<Summoner>(this);

                int totalBonus = loreBonus + shepherdBonus;

                if (sum != null)
                {
                    if (sum.Puppeteer())
                    {
                        if(bmr != null && bmr.FollowerBonus())
                            return (base.FollowersMax + totalBonus + 5);
                        else
                            return (base.FollowersMax + totalBonus + 2);
                    }
                }

                if(bmr != null)
                {
                    if(bmr.FollowerBonus())
                    {
                        if (sum != null && sum.Puppeteer())
                            return (base.FollowersMax + totalBonus + 5);

                        else
                            return (base.FollowersMax + totalBonus + 3);
                    }
                }

                return base.FollowersMax + totalBonus;
            }

            set 
            {
                base.FollowersMax = value;
            }
        }

        public override bool CanSee(Mobile m)
        {
            Scout scout = Perk.GetByType<Scout>(this);

            if (scout != null && scout.CanSeeHidden())
            {
                if (m is Mobile && m.InRange(this.Location, 4) && m.AccessLevel == AccessLevel.Player)
                {
                    return true;
                }

                else 
                    return base.CanSee(m);
            }

            else 
                return base.CanSee(m);
        }


        public virtual void ApplyKOPenalty( )
        {
            if( AccessLevel > AccessLevel.Player || DateTime.Now < RespawnExpiry )
                return;

            if( 0.33 >= Utility.RandomDouble() )
            {
                Item item;

                for( int i = 0; i < Items.Count; i++ )
                {
                    item = Items[i];

                    if( item is BaseArmor )
                        ((BaseArmor)item).HitPoints -= (int)(((BaseArmor)item).MaxHitPoints * 0.05);
                    else if( item is BaseWeapon )
                        ((BaseWeapon)item).HitPoints -= (int)(((BaseWeapon)item).MaxHitPoints * 0.05);
                }

                //if( Backpack != null )
                //{
                //    for( int i = 0; i < Backpack.Items.Count; i++ )
                //    {
                //        item = Backpack.Items[i];

                //        if( item is BaseArmor )
                //            ((BaseArmor)item).HitPoints -= (int)(((BaseArmor)item).MaxHitPoints * 0.05);
                //        else if( item is BaseWeapon )
                //            ((BaseWeapon)item).HitPoints -= (int)(((BaseWeapon)item).MaxHitPoints * 0.05);
                //    }
                //}
            }

            LastRespawn = DateTime.Now;
        }
         
        public virtual void AutoRespawn()
        {
            if( Alive )
                return;

            Resurrect();

            if (Corpse != null)
            {
                Corpse.MoveToWorld(this.Location, this.Map);
                Corpse.OnDoubleClick(this);
            }

            if (Corpse != null)
                Corpse.Delete();
        }

        /// <summary>
        /// Tries to consume an amount of <code>EssenceOfCharacter</code>
        /// </summary>
        /// <param name="value">amount of EoC to consume</param>
        /// <returns>true unless the player doesn't have enough EoC to consume</returns>
        public bool ConsumeEoC( int value )
        {
            if( (EssenceOfCharacter - value) <= 0 )
                return false;

            EssenceOfCharacter -= value;
            return true;
        }

        /// <summary>
        /// Jumps the player to a new location
        /// </summary>
        public virtual void JumpTo( Point3D loc )
        {
            Point3D old = Location;
            Location = loc;

                int zDrop = (old.Z - loc.Z);

                if( zDrop > 20 )
                {
                    int toDmg = ((zDrop / 20) * 30) + 15;

                    Acrobat acr = Perk.GetByType<Acrobat>(this);

                    if( acr != null )
                        toDmg -= acr.GetFallingDamageBonus(toDmg);

                    Damage(toDmg);
                }

            OnJump(old, loc);
            ProcessDelta();
        }

        /// <summary>
        /// Invoked when the player has jumped to a new point
        /// </summary>
        public virtual void OnJump( Point3D oldLocation, Point3D newLocation )
        {
        }

        public override ApplyPoisonResult ApplyPoison( Mobile from, Poison poison )
        {
            Adventurer adv = Perk.GetByType<Adventurer>(this);

            if( adv != null && poison != null && poison.Level > Poison.Lesser.Level )
                poison = adv.GetLowerPoison(poison);

            return base.ApplyPoison(from, poison);
        }

        /// <summary>
        /// Overridden to prevent beneficial action if the player has been recently resurrected
        /// </summary>
        public override bool CanBeBeneficial( Mobile target, bool message, bool allowDead )
        {
            //if( target != this && DateTime.Now < RespawnExpiry )
            //{
            //    TimeSpan timeLeft = RespawnExpiry.Subtract(DateTime.Now);

            //    SendMessage("You cannot take a beneficial action for another {0}.", (timeLeft.Minutes > 0 ? (timeLeft.Minutes + (timeLeft.Minutes > 1 ? " minutes" : " minute")) : (timeLeft.Seconds + (timeLeft.Seconds > 1 ? " seconds" : " second"))));
            //    return false;
            //}

            return base.CanBeBeneficial(target, message, allowDead);
        }

        /// <summary>
        /// Overridden to prevent harmful action if the player has been recently resurrected
        /// </summary>
        public override bool CanBeHarmful( Mobile target, bool message, bool ignoreOurBlessedness )
        {
            //if( target.Combatant != this && DateTime.Now < RespawnExpiry )
            //{
            //    TimeSpan timeLeft = RespawnExpiry.Subtract(DateTime.Now);


            //    SendMessage("You cannot take an aggressive action for another {0}.", (timeLeft.Minutes > 0 ? (timeLeft.Minutes + (timeLeft.Minutes > 1 ? " minutes" : " minute")) : (timeLeft.Seconds + (timeLeft.Seconds > 1 ? " seconds" : " second"))));
            //    return false;
            //}

            return base.CanBeHarmful(target, message, ignoreOurBlessedness);
        }

        /// <summary>
        /// Overridden to allow the paperdoll to be opened even while the player is transformed
        /// </summary>
        public override bool CanPaperdollBeOpenedBy( Mobile from )
        {
            return true;
        }

        /// <summary>
        /// Overridden to append a staff level below the name of staff characters
        /// </summary>
        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties(list);
        }

        /// <summary>
        /// Overridden to reset the player state and apply penalties on resurrection
        /// </summary>
        public override void OnAfterResurrect()
        {
            base.OnAfterResurrect();

            CantWalk = false;
            HueMod = -1;

            Body = Female ? 0x191 : 0x190;

            Hits = (int)(HitsMax * 0.33);
            Stam = (int)(StamMax * 0.90);
            Mana = (int)(ManaMax * 0.33);

            if( Fame > 0 )
                Titles.AwardFame(this, -(Fame / 20), true);

            CloseGump(typeof(KOGump));
            AdjustBody();

            KOProtectionTimer protectionTimer = new KOProtectionTimer(this);
            protectionTimer.Start();

        }

        /// <summary>
        /// Overridden to disable <code>SpeedControl</code> on death
        /// </summary>
        public override bool OnBeforeDeath()
        {
            Send(SpeedControl.Disable);

            return base.OnBeforeDeath();
        }

        /// <summary>
        /// Overridden to wrap the player's movement at map edge
        /// (The Elemental Race can walk on water)
        /// </summary>
        //public override void OnAfterMove( Point3D oldLocation )
        //{
        //    if (Map != Map.Backtrol)
        //        return;

        //    Rectangle2D[] wrap = BaseBoat.GetWrapFor(Map);

        //    int newX = Location.X;
        //    int newY = Location.Y;

        //    for( int i = 0; i < wrap.Length; i++ )
        //    {
        //        Rectangle2D rect = wrap[i];

        //        if( rect.Contains(oldLocation) && !rect.Contains(new Point2D(newX, newY)) )
        //        {
        //            if( newX < rect.X )
        //                newX = rect.X + rect.Width - 1;
        //            else if( newX >= rect.X + rect.Width )
        //                newX = rect.X;

        //            if( newY < rect.Y )
        //                newY = rect.Y + rect.Height + 1;
        //            else if( newY >= rect.Y + rect.Height )
        //                newY = rect.Y;

        //            MoveToWorld(new Point3D(newX, newY, Location.Z), Map);
        //        }
        //    }
        //}

        /// <summary>
        /// Overridden to dispatch the PlayerDamaged event
        /// </summary>
        public override void OnDamage( int amount, Mobile from, bool willKill )
        {
            EventDispatcher.InvokePlayerDamaged(new PlayerDamagedEventArgs(this, amount, from, willKill));

            base.OnDamage(amount, from, willKill);
        }

        /// <summary>
        /// Overridden to dispatch the PlayerKarmaChange event
        /// </summary>
        public override void OnKarmaChange( int oldValue )
        {
            base.OnKarmaChange(oldValue);

            EventDispatcher.InvokePlayerKarmaChange(new PlayerKarmaChangeEventArgs(this, oldValue, Karma));
        }

        /// <summary>
        /// Overridden to redesign hidden movement handler
        /// Overriden to Consume Stamina when running.
        /// </summary>

        public double stamLost;
        public DateTime lastMove;
        public bool isRunning;
        int lastZ = 0, currentZ = 0;

        protected override bool OnMove(Direction d)
        {
            lastZ = currentZ; 
            base.OnMove(d); 
            currentZ = Z;

            EventDispatcher.InvokePlayerMove(new PlayerMoveEventArgs(this, d));
            isRunning = ((d & Direction.Running) != 0);

            lastMove = DateTime.Now;

            if (this is Player && this.Mounted && isRunning && this.Mount is BaseMount)
            {
                if (AccessLevel > AccessLevel.Player)
                    return true;

                BaseMount mount = this.Mount as BaseMount;
                double mountCapacity = (double)(mount.Str * 12);
                stamLost += ((double)((TotalWeight * 0.618) / mountCapacity));

                if (mount.IsInjured)
                    return false;

                if (lastZ < currentZ)
                    stamLost = stamLost * 1.618;

                if (stamLost > 1.0)
                {
                    mount.Stam -= (int)stamLost;
                    stamLost = 0;
                }

                if (mount.Stam < StamMax * 0.10)
                {
                    if (Utility.RandomDouble() <= 0.08)
                    {
                        mount.Rider = null;
                        SendMessage("Your steed appears exhuasted!");
                    }
                }

                if (mount.Stam <= 0)
                {
                    if (stamLost > 1.0)
                    {
                        mount.Damage((int)stamLost);
                        if (Utility.RandomDouble() <= 0.08)
                            mount.Injure();
                    }
                }
            }

            if (this != null && this is Player && isRunning && this.Mounted == false)
            {
                if (AccessLevel > AccessLevel.Player)
                    return true;

                if (Blessed) 
                    return true;

                if (Hunger < 1 || Thirst < 1)
                    Damage(Utility.RandomMinMax(1, 3));

                double htRatio;
                double weightRatio = ((double)(TotalWeight / MaxWeight));

                if (Thirst > 0)
                    htRatio = (double)((20 / Thirst) / 8);

                else htRatio = 10.0;

                if (Hunger > 0)
                    htRatio += (double)((20 / Hunger) / 8);

                else htRatio += 10.0;

                stamLost += (weightRatio + htRatio) * 0.8;

                if (FindItemOnLayer(Layer.Pants) != null)
                    stamLost += FindItemOnLayer(Layer.Pants).Weight * 0.08;

                if (lastZ < currentZ)
                    stamLost = stamLost * 1.618;

                if (stamLost > 1.0)
                {
                    Stam -= (int)stamLost;
                    stamLost = 0;

                    if (Utility.RandomDouble() == Utility.RandomDouble())
                    {
                        if (Utility.RandomBool())
                        {
                            int toggle = Utility.RandomMinMax(1, 2);

                            switch (toggle)
                            {
                                case 1:
                                    Hunger--;
                                    Thirst--;
                                    break;

                                case 2:
                                    Thirst--;
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            Item shoes = this.FindItemOnLayer(Layer.Shoes);
            Item legs = this.FindItemOnLayer(Layer.Pants);

            if (shoes == null && this is Player && !(legs is PlateLegs) && !Mounted)
            {
                if (Utility.RandomDouble() <= 0.01)
                {
                    if (Utility.RandomBool())
                    {
                        this.SendMessage("You've stumbled, and your feet are bare!");
                        Damage(Utility.RandomMinMax(4, 8));

                        if (Utility.RandomDouble() <= 0.15 && Utility.RandomBool())
                            BleedAttack.BeginBleed(this, this);
                    }
                }
            }

            if (Hidden && Server.Multis.DesignContext.Find(this) == null)	//Hidden & NOT customizing a house
            {
                if (AccessLevel > AccessLevel.Player)
                    return true;

                if (!Server.SkillHandlers.Hiding.CheckLighting(this))
                {
                    SendMessage("You could not hope to stay hidden in so much light.");
                    RevealingAction();
                }
                else if (!Mounted)
                {
                    if ((int)this.ArmorRating > 25 && Skills.Stealth.Value >= 25.0)
                    {
                        RevealingAction();
                        SendLocalizedMessage(502727); //You could not hope to move quietly wearing this much armor.
                    }
                    else
                    {
                        Rogue rge = Perk.GetByType<Rogue>((Player)this);

                        if (isRunning)
                        {
                            if (rge != null)
                            {
                                if (!rge.CanRunHidden())
                                    RevealingAction();
                            }
                            else
                                RevealingAction();

                        }

                        else if (AllowedStealthSteps-- <= 0)
                        {
                            Server.SkillHandlers.Stealth.OnUse(this);
                        }
                    }
                }
                else
                {
                    RevealingAction();
                }
            }

            return true;
        }
    

        /// <summary>
        /// Overridden to dispatch the RawStatChange event
        /// </summary>
        public override void OnRawStatChange( StatType stat, int oldValue )
        {
            base.OnRawStatChange(stat, oldValue);

            EventDispatcher.InvokePlayerRawStatChange(new PlayerRawStatChangeEventArgs(this, stat, oldValue));
        }

        /// <summary>
        /// Overridden to add handlers for fame locking
        /// </summary>
        public override void OnSaid( SpeechEventArgs args )
        {
            string speech = args.Speech.ToLower();

            if( speech.IndexOf("i wish to lock my fame") > -1 )
            {
                FameLocked = true;
                SendMessage("Your fame has been locked.");
            }
            else if( speech.IndexOf("i wish to unlock my fame") > -1 )
            {
                FameLocked = false;
                SendMessage("Your fame has been unlocked.");
            }
            else
            {
                string[] jumpWords = new string[] { "jump", "jumps" };

                for( int i = 0; i < jumpWords.Length; i++ )
                {
                    string word = jumpWords[i];

                    if( speech == "*" + word + "*" || (args.Type == MessageType.Emote && speech == word) )
                    {
                        JumpCommand.BeginJump(this);
                        break;
                    }
                }
            }

            base.OnSaid(args);
        }

        /// <summary>
        /// Overridden to dispatch the PlayerSkillChanged event
        /// </summary>
        public override void OnSkillChange( SkillName skill, double oldBase )
        {
            base.OnSkillChange(skill, oldBase);

            EventDispatcher.InvokePlayerSkillChanged(new PlayerSkillChangedEventArgs(this, Skills[skill], oldBase));
        }

        /// <summary>
        /// Overridden to apply the death penalty on resurrect
        /// </summary>
        public override void Resurrect()
        {
            base.Resurrect();
            ApplyKOPenalty();
        }

        /// <summary>
        /// Overridden to apply falling damage
        /// </summary>
        public override void SetLocation( Point3D loc, bool isTeleport )
        {
            if( !isTeleport && AccessLevel == AccessLevel.Player )
            {
                int zDrop = (this.Location.Z - loc.Z);
                
                if( zDrop > 15) // We fell more than half a story
                {
                    int toDmg = ((zDrop / 20) * 5) + 5;

                    Acrobat acr = Perk.GetByType<Acrobat>(this);

                    if (acr != null)
                        toDmg -= acr.GetFallingDamageBonus(toDmg);

                    Damage(toDmg);
                }

                if( zDrop > 25 ) // we fell more than one story
                {
                    int toDmg = ((zDrop / 20) * 30) + 15;

                    Acrobat acr = Perk.GetByType<Acrobat>(this);

                    if( acr != null )
                        toDmg -= acr.GetFallingDamageBonus(toDmg);

                    Damage(toDmg);
                }
            }

            base.SetLocation(loc, isTeleport);
        }

        /// <summary>
        /// Overridden to include class serialization
        /// </summary>
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize(writer);

            // Vii edit
            writer.Write((int)6);// Version

            if (m_culture == null)
                m_culture = "en";

            writer.Write((string)m_culture);
            // end Vii edit
            
            writer.Write((int)EssenceOfCharacter);

            writer.Write((int)m_Flags);

            writer.Write((int)RepGood);
            writer.Write((int)RepBad);

            writer.Write((Point3D)m_RespawnLoc);
            writer.Write((Map)m_RespawnMap);

            EoCLedger.Serialize(writer);

            writer.Write((int)speakingLanguage);
            writer.Write(LevelofUnderstanding.Length);

            for (int i = 0; i < LevelofUnderstanding.Length; i++)
            {
                writer.Write(LevelofUnderstanding[i]);
            }

            writer.Write((int)KOs);
            writer.Write((int)playerRace);
            writer.Write((int)raceBody);

            writer.Write((int)strMod);
            writer.Write((int)dexMod);
            writer.Write((int)IntMod);

            writer.Write((int)BodyDamageBonus);
        }

        /// <summary>
        /// Overridden to include class deserialization
        /// </summary>
        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            // Vii added case 6
            if (version == 6)
                m_culture = reader.ReadString();
            else
                m_culture = "en";

            // case 5
            EssenceOfCharacter = reader.ReadInt();

            m_Flags = (PlayerFlag)reader.ReadInt();

            RepGood = reader.ReadInt();
            RepBad = reader.ReadInt();

            m_RespawnLoc = reader.ReadPoint3D();
            m_RespawnMap = reader.ReadMap();

            if( version >= 1 )
            {
                EoCLedger = new EoCLedger(reader);
            }

            if (version >= 2)
            {
                speakingLanguage = (PlayerLanguage)reader.ReadInt();

                int count = reader.ReadInt();

                LevelofUnderstanding = new int[count];

                for (int i = 0; i < count; i++)
                {
                    LevelofUnderstanding[i] = reader.ReadInt();
                }
            }

            if (version >= 3)
            {
                KOs = reader.ReadInt();
                playerRace = (Race)reader.ReadInt();
            }

            if (version >= 4)
            {
                raceBody = reader.ReadInt();
            }

            if (version >= 5)
            {
                strMod = reader.ReadInt();
                dexMod = reader.ReadInt();
                intMod = reader.ReadInt();

                bodyDamageBonus = reader.ReadInt();
            }
        }
    }
}
