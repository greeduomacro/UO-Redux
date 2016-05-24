using System;
using System.Collections;
using System.Collections.Generic;
using Server.Accounting;
using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Engines.Help;
using Server.Engines.PartySystem;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Multis;
using Server.Network;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Seventh;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells.Bushido;

using Ulmeta.Factions;
using EqPvP;

namespace Server.Mobiles
{
    #region Enums
    [Flags]
    public enum PMFlag // First 16 bits are reserved for default-distro use, start custom flags at 0x00010000
    {
        None = 0x00000000,
        Glassblowing = 0x00000001,
        Masonry = 0x00000002,
        SandMining = 0x00000004,
        StoneMining = 0x00000008,
        ToggleMiningStone = 0x00000010,
        KarmaLocked = 0x00000020,
        //AutoRenewInsurance = 0x00000040,
        UseOwnFilter = 0x00000080,
        //PublicMyRunUO = 0x00000100,
        PagingSquelched = 0x00000200,
        //Young = 0x00000400,
        AcceptGuildInvites = 0x00000800,
        //DisplayChampionTitle = 0x00001000,
        HasStatReward = 0x00002000
    }

    public enum NpcGuild
    {
        None,
        MagesGuild,
        WarriorsGuild,
        ThievesGuild,
        RangersGuild,
        HealersGuild,
        MinersGuild,
        MerchantsGuild,
        TinkersGuild,
        TailorsGuild,
        FishermensGuild,
        BardsGuild,
        BlacksmithsGuild
    }
    #endregion

    public partial class PlayerMobile : Mobile, IHonorTarget, IPlayerCombatant
    {
        CombatantProfile m_combatProfile { get; set; }
        public CombatantProfile CombatProfile
        {
            get { return m_combatProfile; }
            set { m_combatProfile = value; }
        }

        public InteractionState iState;

        private PlayerFaction currentFaction;

        public PlayerFaction CurrentFaction
        {
            set { currentFaction = value; }

            get { return currentFaction; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public String FactionName
        {
            get
            {
                if (currentFaction != null)
                    return currentFaction.FactionName;

                else return null;
            }
        }

        private class CountAndTimeStamp
        {
            private int m_Count;
            private DateTime m_Stamp;

            public CountAndTimeStamp()
            {
            }

            public DateTime TimeStamp { get { return m_Stamp; } }
            public int Count
            {
                get { return m_Count; }
                set { m_Count = value; m_Stamp = DateTime.Now; }
            }
        }

        private DesignContext m_DesignContext;

        private NpcGuild m_NpcGuild;
        private DateTime m_NpcGuildJoinTime;
        private DateTime m_NextBODTurnInTime;
        private TimeSpan m_NpcGuildGameTime;
        private PMFlag m_Flags;
        private int m_StepsTaken;
        private int m_Profession;
        private bool m_IsStealthing; // IsStealthing should be moved to Server.Mobiles
        private bool m_IgnoreMobiles; // IgnoreMobiles should be moved to Server.Mobiles

        private bool m_NinjaWepCooldown;
        private int m_ExecutesLightningStrike;

        private DateTime m_LastOnline;
        private Server.Guilds.RankDefinition m_GuildRank;

        private int m_GuildMessageHue, m_AllianceMessageHue;

        private List<Mobile> m_AllFollowers;
        private List<Mobile> m_RecentlyReported;

        #region Getters & Setters

        public List<Mobile> RecentlyReported
        {
            get
            {
                return m_RecentlyReported;
            }
            set
            {
                m_RecentlyReported = value;
            }
        }
		public bool NinjaWepCooldown
		{
			get
			{
				return m_NinjaWepCooldown;
			}
			set
			{
				m_NinjaWepCooldown = value;
			}
		}

        public List<Mobile> AllFollowers
        {
            get
            {
                if( m_AllFollowers == null )
                    m_AllFollowers = new List<Mobile>();
                return m_AllFollowers;
            }
        }

        public Server.Guilds.RankDefinition GuildRank
        {
            get
            {
                if( this.AccessLevel >= AccessLevel.GameMaster )
                    return Server.Guilds.RankDefinition.Leader;
                else
                    return m_GuildRank;
            }
            set { m_GuildRank = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int GuildMessageHue
        {
            get { return m_GuildMessageHue; }
            set { m_GuildMessageHue = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int AllianceMessageHue
        {
            get { return m_AllianceMessageHue; }
            set { m_AllianceMessageHue = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Profession
        {
            get { return m_Profession; }
            set { m_Profession = value; }
        }

        public int StepsTaken
        {
            get { return m_StepsTaken; }
            set { m_StepsTaken = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsStealthing // IsStealthing should be moved to Server.Mobiles
        {
            get { return m_IsStealthing; }
            set { m_IsStealthing = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IgnoreMobiles // IgnoreMobiles should be moved to Server.Mobiles
        {
            get
            {
                return m_IgnoreMobiles;
            }
            set
            {
                if( m_IgnoreMobiles != value )
                {
                    m_IgnoreMobiles = value;
                    Delta(MobileDelta.Flags);
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public NpcGuild NpcGuild
        {
            get { return m_NpcGuild; }
            set { m_NpcGuild = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NpcGuildJoinTime
        {
            get { return m_NpcGuildJoinTime; }
            set { m_NpcGuildJoinTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextBODTurnInTime
        {
            get { return m_NextBODTurnInTime; }
            set { m_NextBODTurnInTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastOnline
        {
            get { return m_LastOnline; }
            set { m_LastOnline = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastMoved
        {
            get { return LastMoveTime; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan NpcGuildGameTime
        {
            get { return m_NpcGuildGameTime; }
            set { m_NpcGuildGameTime = value; }
        }

		public int ExecutesLightningStrike
		{
			get { return m_ExecutesLightningStrike; }
			set { m_ExecutesLightningStrike = value; }
		}

        #endregion

        #region PlayerFlags
        public PMFlag Flags
        {
            get { return m_Flags; }
            set { m_Flags = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PagingSquelched
        {
            get { return GetFlag(PMFlag.PagingSquelched); }
            set { SetFlag(PMFlag.PagingSquelched, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Glassblowing
        {
            get { return GetFlag(PMFlag.Glassblowing); }
            set { SetFlag(PMFlag.Glassblowing, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Masonry
        {
            get { return GetFlag(PMFlag.Masonry); }
            set { SetFlag(PMFlag.Masonry, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool SandMining
        {
            get { return GetFlag(PMFlag.SandMining); }
            set { SetFlag(PMFlag.SandMining, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool StoneMining
        {
            get { return GetFlag(PMFlag.StoneMining); }
            set { SetFlag(PMFlag.StoneMining, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ToggleMiningStone
        {
            get { return GetFlag(PMFlag.ToggleMiningStone); }
            set { SetFlag(PMFlag.ToggleMiningStone, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool KarmaLocked
        {
            get { return GetFlag(PMFlag.KarmaLocked); }
            set { SetFlag(PMFlag.KarmaLocked, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool UseOwnFilter
        {
            get { return GetFlag(PMFlag.UseOwnFilter); }
            set { SetFlag(PMFlag.UseOwnFilter, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool AcceptGuildInvites
        {
            get { return GetFlag(PMFlag.AcceptGuildInvites); }
            set { SetFlag(PMFlag.AcceptGuildInvites, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasStatReward
        {
            get { return GetFlag(PMFlag.HasStatReward); }
            set { SetFlag(PMFlag.HasStatReward, value); }
        }
        #endregion

        #region Auto Arrow Recovery
        private Dictionary<Type, int> m_RecoverableAmmo = new Dictionary<Type, int>();

        public Dictionary<Type, int> RecoverableAmmo
        {
            get { return m_RecoverableAmmo; }
        }

        public void RecoverAmmo()
        {
            if( Core.AOS && Alive )
            {
                foreach( KeyValuePair<Type, int> kvp in m_RecoverableAmmo )
                {
                    if( kvp.Value > 0 )
                    {
                        Item ammo = null;

                        try
                        {
                            ammo = Activator.CreateInstance(kvp.Key) as Item;
                        }
                        catch
                        {
                        }

                        if( ammo != null )
                        {
                            string name = ammo.Name;
                            ammo.Amount = kvp.Value;

                            if( name == null )
                            {
                                if( ammo is Arrow )
                                    name = "arrow";
                                else if( ammo is Bolt )
                                    name = "bolt";
                            }

                            if( name != null && ammo.Amount > 1 )
                                name = String.Format("{0}s", name);

                            if( name == null )
                                name = String.Format("#{0}", ammo.LabelNumber);

                            PlaceInBackpack(ammo);
                            SendLocalizedMessage(1073504, String.Format("{0}\t{1}", ammo.Amount, name)); // You recover ~1_NUM~ ~2_AMMO~.
                        }
                    }
                }

                m_RecoverableAmmo.Clear();
            }
        }

        #endregion

        private DateTime m_AnkhNextUse;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime AnkhNextUse
        {
            get { return m_AnkhNextUse; }
            set { m_AnkhNextUse = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan DisguiseTimeLeft
        {
            get { return DisguiseTimers.TimeRemaining(this); }
        }

        private DateTime m_PeacedUntil;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime PeacedUntil
        {
            get { return m_PeacedUntil; }
            set { m_PeacedUntil = value; }
        }

        public static Direction GetDirection4( Point3D from, Point3D to )
        {
            int dx = from.X - to.X;
            int dy = from.Y - to.Y;

            int rx = dx - dy;
            int ry = dx + dy;

            Direction ret;

            if( rx >= 0 && ry >= 0 )
                ret = Direction.West;
            else if( rx >= 0 && ry < 0 )
                ret = Direction.South;
            else if( rx < 0 && ry < 0 )
                ret = Direction.East;
            else
                ret = Direction.North;

            return ret;
        }

        public override bool OnDroppedItemToWorld( Item item, Point3D location )
        {
            if( !base.OnDroppedItemToWorld(item, location) )
                return false;

            if( Core.AOS )
            {
                IPooledEnumerable mobiles = Map.GetMobilesInRange(location, 0);

                foreach( Mobile m in mobiles )
                {
                    if( m.Z >= location.Z && m.Z < location.Z + 16 )
                    {
                        mobiles.Free();
                        return false;
                    }
                }

                mobiles.Free();
            }

            BounceInfo bi = item.GetBounce();

            if( bi != null )
            {
                Type type = item.GetType();

                if( type.IsDefined(typeof(FurnitureAttribute), true) || type.IsDefined(typeof(DynamicFlipingAttribute), true) )
                {
                    object[] objs = type.GetCustomAttributes(typeof(FlipableAttribute), true);

                    if( objs != null && objs.Length > 0 )
                    {
                        FlipableAttribute fp = objs[0] as FlipableAttribute;

                        if( fp != null )
                        {
                            int[] itemIDs = fp.ItemIDs;

                            Point3D oldWorldLoc = bi.m_WorldLoc;
                            Point3D newWorldLoc = location;

                            if( oldWorldLoc.X != newWorldLoc.X || oldWorldLoc.Y != newWorldLoc.Y )
                            {
                                Direction dir = GetDirection4(oldWorldLoc, newWorldLoc);

                                if( itemIDs.Length == 2 )
                                {
                                    switch( dir )
                                    {
                                        case Direction.North:
                                        case Direction.South: item.ItemID = itemIDs[0]; break;
                                        case Direction.East:
                                        case Direction.West: item.ItemID = itemIDs[1]; break;
                                    }
                                }
                                else if( itemIDs.Length == 4 )
                                {
                                    switch( dir )
                                    {
                                        case Direction.South: item.ItemID = itemIDs[0]; break;
                                        case Direction.East: item.ItemID = itemIDs[1]; break;
                                        case Direction.North: item.ItemID = itemIDs[2]; break;
                                        case Direction.West: item.ItemID = itemIDs[3]; break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        public override int GetPacketFlags()
        {
            int flags = base.GetPacketFlags();

            if( m_IgnoreMobiles )
                flags |= 0x10;

            return flags;
        }

        public override int GetOldPacketFlags()
        {
            int flags = base.GetOldPacketFlags();

            if( m_IgnoreMobiles )
                flags |= 0x10;

            return flags;
        }

        public bool GetFlag( PMFlag flag )
        {
            return ((m_Flags & flag) != 0);
        }

        public void SetFlag( PMFlag flag, bool value )
        {
            if( value )
                m_Flags |= flag;
            else
                m_Flags &= ~flag;
        }

        public DesignContext DesignContext
        {
            get { return m_DesignContext; }
            set { m_DesignContext = value; }
        }

        public static void Initialize()
        {
            if( FastwalkPrevention )
                PacketHandlers.RegisterThrottler(0x02, new ThrottlePacketCallback(MovementThrottle_Callback));

            EventSink.Login += new LoginEventHandler(OnLogin);
            EventSink.Connected += new ConnectedEventHandler(EventSink_Connected);
            EventSink.Disconnected += new DisconnectedEventHandler(EventSink_Disconnected);
        }

		private MountBlock m_MountBlock;

		public BlockMountType MountBlockReason
		{
			get
			{
				return ( CheckBlock( m_MountBlock ) ) ? m_MountBlock.m_Type : BlockMountType.None;
			}
		}

		private static bool CheckBlock( MountBlock block )
		{
			return ( ( block is MountBlock ) && block.m_Timer.Running );
		}

		private class MountBlock
		{
			public BlockMountType m_Type;
			public Timer m_Timer;

			public MountBlock( TimeSpan duration, BlockMountType type, Mobile mobile )
			{
				m_Type = type;

				m_Timer = Timer.DelayCall( duration, new TimerStateCallback<Mobile>( RemoveBlock ), mobile );
			}

			private void RemoveBlock( Mobile mobile )
			{
				( mobile as PlayerMobile ).m_MountBlock = null;
			}
		}

		public void SetMountBlock( BlockMountType type, TimeSpan duration, bool dismount )
		{
			if( dismount )
			{
				if ( this.Mount != null )
				{
					this.Mount.Rider = null;
				}
				else if( AnimalForm.UnderTransformation( this ) )
				{
					AnimalForm.RemoveContext(this, true);
				}
			}

			if( ( m_MountBlock == null ) || !m_MountBlock.m_Timer.Running || ( m_MountBlock.m_Timer.Next < ( DateTime.UtcNow + duration ) ) )
			{
				m_MountBlock = new MountBlock( duration, type, this );
			}
		}

        /*public override void OnSkillInvalidated( Skill skill )
        {
            if( Core.AOS && skill.SkillName == SkillName.MagicResist )
                UpdateResistances();
        }*/

        public override int GetMaxResistance( ResistanceType type )
        {
            if (AccessLevel > AccessLevel.Player)
                return 75;

            int max = base.GetMaxResistance(type);

            if( type != ResistanceType.Physical && 40 < max && Spells.Fourth.CurseSpell.UnderEffect(this) )
                max = 50;

            return max;
        }

        public override int MaxWeight { get { return 40 + (int)(3.5 * this.Str); } }

        private int m_LastGlobalLight = -1, m_LastPersonalLight = -1;

        public override void OnNetStateChanged()
        {
            m_LastGlobalLight = -1;
            m_LastPersonalLight = -1;
        }

        public override void ComputeBaseLightLevels( out int global, out int personal )
        {
            global = LightCycle.ComputeLevelFor(this);

            if( this.LightLevel < 21 && AosAttributes.GetValue(this, AosAttribute.NightSight) > 0 )
                personal = 21;
            else
                personal = this.LightLevel;
        }

        public override void CheckLightLevels( bool forceResend )
        {
            NetState ns = this.NetState;

            if( ns == null )
                return;

            int global, personal;

            ComputeLightLevels(out global, out personal);

            if( !forceResend )
                forceResend = (global != m_LastGlobalLight || personal != m_LastPersonalLight);

            if( !forceResend )
                return;

            m_LastGlobalLight = global;
            m_LastPersonalLight = personal;

            ns.Send(GlobalLightLevel.Instantiate(global));
            ns.Send(new PersonalLightLevel(this, personal));
        }


        /*public override int GetMinResistance( ResistanceType type )
        {
            int magicResist = (int)(Skills[SkillName.MagicResist].Value * 10);
            int min = int.MinValue;

            if( magicResist >= 1000 )
                min = 40 + ((magicResist - 1000) / 50);
            else if( magicResist >= 400 )
                min = (magicResist - 400) / 15;

            if( min > MaxPlayerResistance )
                min = MaxPlayerResistance;

            int baseMin = base.GetMinResistance(type);

            if( min < baseMin )
                min = baseMin;

            return min;
        }*/

        private static void OnLogin( LoginEventArgs e )
        {
            Mobile from = e.Mobile;

            CheckAtrophies(from);

            if( AccountHandler.LockdownLevel > AccessLevel.Player )
            {
                string notice;

                Accounting.Account acct = from.Account as Accounting.Account;

                if( acct == null || !acct.HasAccess(from.NetState) )
                {
                    if( from.AccessLevel == AccessLevel.Player )
                        notice = "The server is currently under lockdown. No players are allowed to log in at this time.";
                    else
                        notice = "The server is currently under lockdown. You do not have sufficient access level to connect.";

                    Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(Disconnect), from);
                }
                else if( from.AccessLevel >= AccessLevel.Administrator )
                {
                    notice = "The server is currently under lockdown. As you are an administrator, you may change this from the [Admin gump.";
                }
                else
                {
                    notice = "The server is currently under lockdown. You have sufficient access level to connect.";
                }

                from.SendGump(new NoticeGump(1060637, 30720, notice, 0xFFC000, 300, 140, null, null));
                return;
            }
        }

        private bool m_NoDeltaRecursion;

        public void ValidateEquipment()
        {
            if( m_NoDeltaRecursion || Map == null || Map == Map.Internal )
                return;

            if( this.Items == null )
                return;

            m_NoDeltaRecursion = true;
            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(ValidateEquipment_Sandbox));
        }

        private void ValidateEquipment_Sandbox()
        {
            try
            {
                if( Map == null || Map == Map.Internal )
                    return;

                List<Item> items = this.Items;

                if( items == null )
                    return;

                bool moved = false;

                int str = this.Str;
                int dex = this.Dex;
                int intel = this.Int;

                Mobile from = this;

                for( int i = items.Count - 1; i >= 0; --i )
                {
                    if( i >= items.Count )
                        continue;

                    Item item = items[i];

                    if( item is BaseWeapon )
                    {
                        BaseWeapon weapon = (BaseWeapon)item;

                        bool drop = false;

                        if( dex < weapon.DexRequirement )
                            drop = true;
                        else if( intel < weapon.IntRequirement )
                            drop = true;

                        if( drop )
                        {
                            string name = weapon.Name;

                            if( name == null )
                                name = String.Format("#{0}", weapon.LabelNumber);

                            from.SendLocalizedMessage(1062001, name); // You can no longer wield your ~1_WEAPON~
                            from.AddToBackpack(weapon);
                            moved = true;
                        }
                    }
                    else if( item is BaseArmor )
                    {
                        BaseArmor armor = (BaseArmor)item;

                        bool drop = false;

                        if( !armor.AllowMaleWearer && !from.Female && from.AccessLevel < AccessLevel.GameMaster )
                        {
                            drop = true;
                        }
                        else if( !armor.AllowFemaleWearer && from.Female && from.AccessLevel < AccessLevel.GameMaster )
                        {
                            drop = true;
                        }
                        else
                        {
                            int strBonus = armor.ComputeStatBonus(StatType.Str), strReq = armor.ComputeStatReq(StatType.Str);
                            int dexBonus = armor.ComputeStatBonus(StatType.Dex), dexReq = armor.ComputeStatReq(StatType.Dex);
                            int intBonus = armor.ComputeStatBonus(StatType.Int), intReq = armor.ComputeStatReq(StatType.Int);

                            if( dex < dexReq || (dex + dexBonus) < 1 )
                                drop = true;
                            else if( str < strReq || (str + strBonus) < 1 )
                                drop = true;
                            else if( intel < intReq || (intel + intBonus) < 1 )
                                drop = true;
                        }

                        if( drop )
                        {
                            string name = armor.Name;

                            if( name == null )
                                name = String.Format("#{0}", armor.LabelNumber);

                            if( armor is BaseShield )
                                from.SendLocalizedMessage(1062003, name); // You can no longer equip your ~1_SHIELD~
                            else
                                from.SendLocalizedMessage(1062002, name); // You can no longer wear your ~1_ARMOR~

                            from.AddToBackpack(armor);
                            moved = true;
                        }
                    }
                    else if( item is BaseClothing )
                    {
                        BaseClothing clothing = (BaseClothing)item;

                        bool drop = false;

                        if( !clothing.AllowMaleWearer && !from.Female && from.AccessLevel < AccessLevel.GameMaster )
                        {
                            drop = true;
                        }
                        else if( !clothing.AllowFemaleWearer && from.Female && from.AccessLevel < AccessLevel.GameMaster )
                        {
                            drop = true;
                        }
                        else
                        {
                            int strBonus = clothing.ComputeStatBonus(StatType.Str);
                            int strReq = clothing.ComputeStatReq(StatType.Str);

                            if( str < strReq || (str + strBonus) < 1 )
                                drop = true;
                        }

                        if( drop )
                        {
                            string name = clothing.Name;

                            if( name == null )
                                name = String.Format("#{0}", clothing.LabelNumber);

                            from.SendLocalizedMessage(1062002, name); // You can no longer wear your ~1_ARMOR~

                            from.AddToBackpack(clothing);
                            moved = true;
                        }
                    }
                }

                if( moved )
                    from.SendLocalizedMessage(500647); // Some equipment has been moved to your backpack.
            }
            catch( Exception e )
            {
                Console.WriteLine(e);
            }
            finally
            {
                m_NoDeltaRecursion = false;
            }
        }

        public override void Delta( MobileDelta flag )
        {
            base.Delta(flag);

            if( (flag & MobileDelta.Stat) != 0 )
                ValidateEquipment();
        }

        private static void Disconnect( object state )
        {
            NetState ns = ((Mobile)state).NetState;

            if( ns != null )
                ns.Dispose();
        }

        private static void EventSink_Connected( ConnectedEventArgs e )
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if( pm != null )
            {
                pm.m_SessionStart = DateTime.Now;

                pm.BedrollLogout = false;
                pm.LastOnline = DateTime.Now;
            }

            DisguiseTimers.StartTimer(e.Mobile);

            Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(ClearSpecialMovesCallback), e.Mobile);
        }

        private static void ClearSpecialMovesCallback( object state )
        {
            Mobile from = (Mobile)state;

            SpecialMove.ClearAllMoves(from);
        }

        private static void EventSink_Disconnected( DisconnectedEventArgs e )
        {
            Mobile from = e.Mobile;
            DesignContext context = DesignContext.Find(from);

            if( context != null )
            {
                /* Client disconnected
                 *  - Remove design context
                 *  - Eject all from house
                 *  - Restore relocated entities
                 */

                // Remove design context
                DesignContext.Remove(from);

                // Eject all from house
                from.RevealingAction();

                foreach( Item item in context.Foundation.GetItems() )
                    item.Location = context.Foundation.BanLocation;

                foreach( Mobile mobile in context.Foundation.GetMobiles() )
                    mobile.Location = context.Foundation.BanLocation;

                // Restore relocated entities
                context.Foundation.RestoreRelocatedEntities();
            }

            PlayerMobile pm = e.Mobile as PlayerMobile;

            if( pm != null )
            {
                pm.m_GameTime += (DateTime.Now - pm.m_SessionStart);

                pm.m_SpeechLog = null;
                pm.LastOnline = DateTime.Now;
            }

            DisguiseTimers.StopTimer(from);
        }

        public override void RevealingAction()
        {
            if( m_DesignContext != null )
                return;

            Spells.Sixth.InvisibilitySpell.RemoveTimer(this);

            base.RevealingAction();

            m_IsStealthing = false; // IsStealthing should be moved to Server.Mobiles
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override bool Hidden
        {
            get
            {
                return base.Hidden;
            }
            set
            {
                base.Hidden = value;

                RemoveBuff(BuffIcon.Invisibility);	//Always remove, default to the hiding icon EXCEPT in the invis spell where it's explicitly set

                if( !Hidden )
                {
                    RemoveBuff(BuffIcon.HidingAndOrStealth);
                }
                else// if( !InvisibilitySpell.HasTimer( this ) )
                {
                    BuffInfo.AddBuff(this, new BuffInfo(BuffIcon.HidingAndOrStealth, 1075655));	//Hidden/Stealthing & You Are Hidden
                }
            }
        }

        public override void OnSubItemAdded( Item item )
        {
            if( AccessLevel < AccessLevel.GameMaster && item.IsChildOf(this.Backpack) )
            {
                int maxWeight = WeightOverloading.GetMaxWeight(this);
                int curWeight = TotalWeight;

                if( curWeight > maxWeight )
                    this.SendLocalizedMessage(1019035, true, String.Format(" : {0} / {1}", curWeight, maxWeight));
            }
        }

        public override bool CanBeHarmful( Mobile target, bool message, bool ignoreOurBlessedness )
        {
            if( m_DesignContext != null || (target is PlayerMobile && ((PlayerMobile)target).m_DesignContext != null) )
                return false;

            if( (target is BaseVendor && ((BaseVendor)target).IsInvulnerable) || target is PlayerVendor || target is TownCrier )
            {
                if( message )
                {
                    if( target.Title == null )
                        SendMessage("{0} the vendor cannot be harmed.", target.Name);
                    else
                        SendMessage("{0} {1} cannot be harmed.", target.Name, target.Title);
                }

                return false;
            }

            return base.CanBeHarmful(target, message, ignoreOurBlessedness);
        }

        public override bool CanBeBeneficial( Mobile target, bool message, bool allowDead )
        {
            if( m_DesignContext != null || (target is PlayerMobile && ((PlayerMobile)target).m_DesignContext != null) )
                return false;

            return base.CanBeBeneficial(target, message, allowDead);
        }

        public override bool CheckContextMenuDisplay( IEntity target )
        {
            return (m_DesignContext == null);
        }

        public override void OnItemAdded( Item item )
        {
            base.OnItemAdded(item);

            if( item is BaseArmor || item is BaseWeapon )
            {
                Hits = Hits; Stam = Stam; Mana = Mana;
            }

            if( this.NetState != null )
                CheckLightLevels(false);
        }

        public override void OnItemRemoved( Item item )
        {
            base.OnItemRemoved(item);

            if( item is BaseArmor || item is BaseWeapon )
            {
                Hits = Hits; Stam = Stam; Mana = Mana;
            }

            if( this.NetState != null )
                CheckLightLevels(false);
        }

        public override double ArmorRating
        {
            get
            {
                //BaseArmor ar;
                double rating = 0.0;

                AddArmorRating(ref rating, NeckArmor);
                AddArmorRating(ref rating, HandArmor);
                AddArmorRating(ref rating, HeadArmor);
                AddArmorRating(ref rating, ArmsArmor);
                AddArmorRating(ref rating, LegsArmor);
                AddArmorRating(ref rating, ChestArmor);
                AddArmorRating(ref rating, ShieldArmor);

                return VirtualArmor + VirtualArmorMod + rating;
            }
        }

        private void AddArmorRating( ref double rating, Item armor )
        {
            BaseArmor ar = armor as BaseArmor;

            if( ar != null && (!Core.AOS || ar.ArmorAttributes.MageArmor == 0) )
                rating += ar.ArmorRatingScaled;
        }

        #region [Stats]Max
        [CommandProperty(AccessLevel.GameMaster)]
        public override int HitsMax
        {
            get
            {
                int strBase;
                int strOffs = GetStatOffset(StatType.Str);

                if( Core.AOS )
                {
                    strBase = this.Str;	//this.Str already includes GetStatOffset/str
                    strOffs = AosAttributes.GetValue(this, AosAttribute.BonusHits);

                    if( Core.ML && strOffs > 25 && AccessLevel <= AccessLevel.Player )
                        strOffs = 25;
					if ( AnimalForm.UnderTransformation( this, typeof( BakeKitsune ) ) || AnimalForm.UnderTransformation( this, typeof( GreyWolf ) ) )
						strOffs += 20;
                }
                else
                {
                    strBase = this.RawStr;
                }

                return base.HitsMax + strOffs;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int StamMax
        {
            get { return base.StamMax + AosAttributes.GetValue(this, AosAttribute.BonusStam); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int ManaMax
        {
            get { return base.ManaMax + AosAttributes.GetValue(this, AosAttribute.BonusMana); }
        }
        #endregion

        #region Stat Getters/Setters

        [CommandProperty(AccessLevel.GameMaster)]
        public override int Str
        {
            get
            {
                if( Core.ML && this.AccessLevel == AccessLevel.Player )
                    return Math.Min(base.Str, 400);

                return base.Str;
            }
            set
            {
                base.Str = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int Int
        {
            get
            {
                if( Core.ML && this.AccessLevel == AccessLevel.Player )
                    return Math.Min(base.Int, 400);

                return base.Int;
            }
            set
            {
                base.Int = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int Dex
        {
            get
            {
                if( Core.ML && this.AccessLevel == AccessLevel.Player )
                    return Math.Min(base.Dex, 400);

                return base.Dex;
            }
            set
            {
                base.Dex = value;
            }
        }

        #endregion

        public override bool Move( Direction d )
        {
            NetState ns = this.NetState;

            if( ns != null )
            {
                if( HasGump(typeof(ResurrectGump)) )
                {
                    if( Alive )
                    {
                        CloseGump(typeof(ResurrectGump));
                    }
                    else
                    {
                        SendLocalizedMessage(500111); // You are frozen and cannot move.
                        return false;
                    }
                }
            }

            TimeSpan speed = ComputeMovementSpeed(d);

            bool res;

            if( !Alive )
                Server.Movement.MovementImpl.IgnoreMovableImpassables = true;

            res = base.Move(d);

            Server.Movement.MovementImpl.IgnoreMovableImpassables = false;

            if( !res )
                return false;

            m_NextMovementTime += speed;

            return true;
        }

        public override bool CheckMovement( Direction d, out int newZ )
        {
            DesignContext context = m_DesignContext;

            if( context == null )
                return base.CheckMovement(d, out newZ);

            HouseFoundation foundation = context.Foundation;

            newZ = foundation.Z + HouseFoundation.GetLevelZ(context.Level, context.Foundation);

            int newX = this.X, newY = this.Y;
            Movement.Movement.Offset(d, ref newX, ref newY);

            int startX = foundation.X + foundation.Components.Min.X + 1;
            int startY = foundation.Y + foundation.Components.Min.Y + 1;
            int endX = startX + foundation.Components.Width - 1;
            int endY = startY + foundation.Components.Height - 2;

            return (newX >= startX && newY >= startY && newX < endX && newY < endY && Map == foundation.Map);
        }

        public override bool AllowItemUse( Item item )
        {
            return DesignContext.Check(this);
        }

        public override bool AllowSkillUse( SkillName skill )
        {
            return DesignContext.Check(this);
        }

        private bool m_LastProtectedMessage;
        private int m_NextProtectionCheck = 10;

        public virtual void RecheckTownProtection()
        {
            m_NextProtectionCheck = 10;

            Regions.GuardedRegion reg = (Regions.GuardedRegion)this.Region.GetRegion(typeof(Regions.GuardedRegion));
            bool isProtected = (reg != null && !reg.IsDisabled());

            if( isProtected != m_LastProtectedMessage )
            {
                if( isProtected )
                    SendLocalizedMessage(500112); // You are now under the protection of the town guards.
                else
                    SendLocalizedMessage(500113); // You have left the protection of the town guards.

                m_LastProtectedMessage = isProtected;
            }
        }

        public override void MoveToWorld( Point3D loc, Map map )
        {
            base.MoveToWorld(loc, map);

            RecheckTownProtection();
        }

        public override void SetLocation( Point3D loc, bool isTeleport )
        {
            base.SetLocation(loc, isTeleport);

            if( isTeleport || --m_NextProtectionCheck == 0 )
                RecheckTownProtection();
        }

        public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
        {
            base.GetContextMenuEntries(from, list);

            if( from == this )
            {
                BaseHouse house = BaseHouse.FindHouseAt(this);

                if( house != null )
                {
                    if( Alive && house.InternalizedVendors.Count > 0 && house.IsOwner(this) )
                        list.Add(new CallbackEntry(6204, new ContextCallback(GetVendor)));
                }

                if( m_JusticeProtectors.Count > 0 )
                    list.Add(new CallbackEntry(6157, new ContextCallback(CancelProtection)));
            }

            if( from != this )
            {
                if( Alive && Core.Expansion >= Expansion.AOS )
                {
                    Party theirParty = from.Party as Party;
                    Party ourParty = this.Party as Party;

                    if( theirParty == null && ourParty == null )
                    {
                        list.Add(new AddToPartyEntry(from, this));
                    }
                    else if( theirParty != null && theirParty.Leader == from )
                    {
                        if( ourParty == null )
                        {
                            list.Add(new AddToPartyEntry(from, this));
                        }
                        else if( ourParty == theirParty )
                        {
                            list.Add(new RemoveFromPartyEntry(from, this));
                        }
                    }
                }
            }
        }

        private void CancelProtection()
        {
            for( int i = 0; i < m_JusticeProtectors.Count; ++i )
            {
                Mobile prot = m_JusticeProtectors[i];

                string args = String.Format("{0}\t{1}", this.Name, prot.Name);

                prot.SendLocalizedMessage(1049371, args); // The protective relationship between ~1_PLAYER1~ and ~2_PLAYER2~ has been ended.
                this.SendLocalizedMessage(1049371, args); // The protective relationship between ~1_PLAYER1~ and ~2_PLAYER2~ has been ended.
            }

            m_JusticeProtectors.Clear();
        }

        private void GetVendor()
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if( CheckAlive() && house != null && house.IsOwner(this) && house.InternalizedVendors.Count > 0 )
            {
                CloseGump(typeof(ReclaimVendorGump));
                SendGump(new ReclaimVendorGump(house));
            }
        }

        private delegate void ContextCallback();

        private class CallbackEntry : ContextMenuEntry
        {
            private ContextCallback m_Callback;

            public CallbackEntry( int number, ContextCallback callback )
                : this(number, -1, callback)
            {
            }

            public CallbackEntry( int number, int range, ContextCallback callback )
                : base(number, range)
            {
                m_Callback = callback;
            }

            public override void OnClick()
            {
                if( m_Callback != null )
                    m_Callback();
            }
        }

        public override void DisruptiveAction()
        {
            if( Meditating )
            {
                RemoveBuff(BuffIcon.ActiveMeditation);
            }

            base.DisruptiveAction();
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( this == from && !Warmode )
            {
                IMount mount = Mount;

                if( mount != null && !DesignContext.Check(this) )
                    return;
            }

            base.OnDoubleClick(from);
        }

        public override void DisplayPaperdollTo( Mobile to )
        {
            if( DesignContext.Check(this) )
                base.DisplayPaperdollTo(to);
        }

        private static bool m_NoRecursion;

        public override bool CheckEquip( Item item )
        {
            if( !base.CheckEquip(item) )
                return false;

            if( this.AccessLevel < AccessLevel.GameMaster && item.Layer != Layer.Mount && this.HasTrade )
            {
                BounceInfo bounce = item.GetBounce();

                if( bounce != null )
                {
                    if( bounce.m_Parent is Item )
                    {
                        Item parent = (Item)bounce.m_Parent;

                        if( parent == this.Backpack || parent.IsChildOf(this.Backpack) )
                            return true;
                    }
                    else if( bounce.m_Parent == this )
                    {
                        return true;
                    }
                }

                SendLocalizedMessage(1004042); // You can only equip what you are already carrying while you have a trade pending.
                return false;
            }

            return true;
        }

        public override bool CheckTrade( Mobile to, Item item, SecureTradeContainer cont, bool message, bool checkItems, int plusItems, int plusWeight )
        {
            int msgNum = 0;

            if( cont == null )
            {
                if( to.Holding != null )
                    msgNum = 1062727; // You cannot trade with someone who is dragging something.
                else if( this.HasTrade )
                    msgNum = 1062781; // You are already trading with someone else!
                else if( to.HasTrade )
                    msgNum = 1062779; // That person is already involved in a trade
            }

            if( msgNum == 0 )
            {
                if( cont != null )
                {
                    plusItems += cont.TotalItems;
                    plusWeight += cont.TotalWeight;
                }

                if( this.Backpack == null || !this.Backpack.CheckHold(this, item, false, checkItems, plusItems, plusWeight) )
                    msgNum = 1004040; // You would not be able to hold this if the trade failed.
                else if( to.Backpack == null || !to.Backpack.CheckHold(to, item, false, checkItems, plusItems, plusWeight) )
                    msgNum = 1004039; // The recipient of this trade would not be able to carry this.
                else
                    msgNum = CheckContentForTrade(item);
            }

            if( msgNum != 0 )
            {
                if( message )
                    this.SendLocalizedMessage(msgNum);

                return false;
            }

            return true;
        }

        private static int CheckContentForTrade( Item item )
        {
            if( item is TrapableContainer && ((TrapableContainer)item).TrapType != TrapType.None )
                return 1004044; // You may not trade trapped items.

            if( SkillHandlers.StolenItem.IsStolen(item) )
                return 1004043; // You may not trade recently stolen items.

            if( item is Container )
            {
                foreach( Item subItem in item.Items )
                {
                    int msg = CheckContentForTrade(subItem);

                    if( msg != 0 )
                        return msg;
                }
            }

            return 0;
        }

        public override bool CheckNonlocalDrop( Mobile from, Item item, Item target )
        {
            if( !base.CheckNonlocalDrop(from, item, target) )
                return false;

            if( from.AccessLevel >= AccessLevel.GameMaster )
                return true;

            Container pack = this.Backpack;
            if( from == this && this.HasTrade && (target == pack || target.IsChildOf(pack)) )
            {
                BounceInfo bounce = item.GetBounce();

                if( bounce != null && bounce.m_Parent is Item )
                {
                    Item parent = (Item)bounce.m_Parent;

                    if( parent == pack || parent.IsChildOf(pack) )
                        return true;
                }

                SendLocalizedMessage(1004041); // You can't do that while you have a trade pending.
                return false;
            }

            return true;
        }

        protected override void OnLocationChange( Point3D oldLocation )
        {
            CheckLightLevels(false);

            DesignContext context = m_DesignContext;

            if( context == null || m_NoRecursion )
                return;

            m_NoRecursion = true;

            HouseFoundation foundation = context.Foundation;

            int newX = this.X, newY = this.Y;
            int newZ = foundation.Z + HouseFoundation.GetLevelZ(context.Level, context.Foundation);

            int startX = foundation.X + foundation.Components.Min.X + 1;
            int startY = foundation.Y + foundation.Components.Min.Y + 1;
            int endX = startX + foundation.Components.Width - 1;
            int endY = startY + foundation.Components.Height - 2;

            if( newX >= startX && newY >= startY && newX < endX && newY < endY && Map == foundation.Map )
            {
                if( Z != newZ )
                    Location = new Point3D(X, Y, newZ);

                m_NoRecursion = false;
                return;
            }

            Location = new Point3D(foundation.X, foundation.Y, newZ);
            Map = foundation.Map;

            m_NoRecursion = false;
        }

        public override bool OnMoveOver( Mobile m )
        {
            if( m is BaseCreature && !((BaseCreature)m).Controlled )
                return (!Alive || !m.Alive || IsDeadBondedPet || m.IsDeadBondedPet) || (Hidden && AccessLevel > AccessLevel.Player);

            return base.OnMoveOver(m);
        }

        public override bool CheckShove( Mobile shoved )
        {
            if( m_IgnoreMobiles )
                return true;
            else
                return base.CheckShove(shoved);
        }

        protected override void OnMapChange( Map oldMap )
        {
            DesignContext context = m_DesignContext;

            if( context == null || m_NoRecursion )
                return;

            m_NoRecursion = true;

            HouseFoundation foundation = context.Foundation;

            if( Map != foundation.Map )
                Map = foundation.Map;

            m_NoRecursion = false;
        }

        public override void OnBeneficialAction( Mobile target, bool isCriminal )
        {
            if( m_SentHonorContext != null )
                m_SentHonorContext.OnSourceBeneficialAction(target);

            base.OnBeneficialAction(target, isCriminal);
        }

        public override void OnDamage( int amount, Mobile from, bool willKill )
        {
            int disruptThreshold;

            if( !Core.AOS )
                disruptThreshold = 0;
            else if( from != null && from.Player )
                disruptThreshold = 18;
            else
                disruptThreshold = 25;

            if( amount > disruptThreshold )
            {
                BandageContext c = BandageContext.GetContext(this);

                if( c != null )
                    c.Slip();
            }

            WeightOverloading.FatigueOnDamage(this, amount);

            if( m_ReceivedHonorContext != null )
                m_ReceivedHonorContext.OnTargetDamaged(from, amount);
            if( m_SentHonorContext != null )
                m_SentHonorContext.OnSourceDamaged(from, amount);

            if( willKill && from is PlayerMobile )
                Timer.DelayCall(TimeSpan.FromSeconds(10), new TimerCallback(((PlayerMobile)from).RecoverAmmo));

            base.OnDamage(amount, from, willKill);
        }

        public override void OnWarmodeChanged()
        {
            if( !Warmode )
                Timer.DelayCall(TimeSpan.FromSeconds(10), new TimerCallback(RecoverAmmo));
        }

        private List<Item> m_EquipSnapshot;

        public List<Item> EquipSnapshot
        {
            get { return m_EquipSnapshot; }
        }

        private bool FindItems_Callback( Item item )
        {
            if( !item.Deleted && (item.LootType == LootType.Blessed || item.Insured) )
            {
                if( this.Backpack != item.ParentEntity )
                {
                    return true;
                }
            }
            return false;
        }

        public override bool OnBeforeDeath()
        {
            NetState state = NetState;

            if( state != null )
                state.CancelAllTrades();

            DropHolding();

            if( Core.AOS && Backpack != null && !Backpack.Deleted )
            {
                List<Item> ilist = Backpack.FindItemsByType<Item>(FindItems_Callback);

                for( int i = 0; i < ilist.Count; i++ )
                {
                    Backpack.AddItem(ilist[i]);
                }
            }

            m_EquipSnapshot = new List<Item>(this.Items);

            if( m_ReceivedHonorContext != null )
                m_ReceivedHonorContext.OnTargetKilled();
            if( m_SentHonorContext != null )
                m_SentHonorContext.OnSourceKilled();

            RecoverAmmo();

            return base.OnBeforeDeath();
        }

        public override void OnDeath( Container c )
        {
            base.OnDeath(c);

            m_EquipSnapshot = null;

            HueMod = -1;
            NameMod = null;
            SavagePaintExpiration = TimeSpan.Zero;

            SetHairMods(-1, -1);

            PolymorphSpell.StopTimer(this);
            IncognitoSpell.StopTimer(this);
            DisguiseTimers.RemoveTimer(this);

            EndAction(typeof(PolymorphSpell));
            EndAction(typeof(IncognitoSpell));

            SkillHandlers.StolenItem.ReturnOnDeath(this, c);

            if( m_PermaFlags.Count > 0 )
            {
                m_PermaFlags.Clear();

                if( c is Corpse )
                    ((Corpse)c).Criminal = true;

                if( SkillHandlers.Stealing.ClassicMode )
                    Criminal = true;
            }

            if( this.Kills >= 5 && DateTime.Now >= m_NextJustAward )
            {
                Mobile m = FindMostRecentDamager(false);

                if( m is BaseCreature )
                    m = ((BaseCreature)m).GetMaster();

                if( m != null && m is PlayerMobile && m != this )
                {
                    bool gainedPath = false;

                    int pointsToGain = 0;

                    pointsToGain += (int)Math.Sqrt(this.GameTime.TotalSeconds * 4);
                    pointsToGain *= 5;
                    pointsToGain += (int)Math.Pow(this.Skills.Total / 250, 2);

                    if( VirtueHelper.Award(m, VirtueName.Justice, pointsToGain, ref gainedPath) )
                    {
                        if( gainedPath )
                            m.SendLocalizedMessage(1049367); // You have gained a path in Justice!
                        else
                            m.SendLocalizedMessage(1049363); // You have gained in Justice.

                        m.FixedParticles(0x375A, 9, 20, 5027, EffectLayer.Waist);
                        m.PlaySound(0x1F7);

                        m_NextJustAward = DateTime.Now + TimeSpan.FromMinutes(pointsToGain / 3);
                    }
                }
            }

            Mobile killer = this.FindMostRecentDamager(true);

            if( killer is BaseCreature )
            {
                BaseCreature bc = (BaseCreature)killer;

                Mobile master = bc.GetMaster();
                if( master != null )
                    killer = master;
            }

            Server.Guilds.Guild.HandleDeath(this, killer);

            if( m_BuffTable != null )
            {
                List<BuffInfo> list = new List<BuffInfo>();

                foreach( BuffInfo buff in m_BuffTable.Values )
                {
                    if( !buff.RetainThroughDeath )
                    {
                        list.Add(buff);
                    }
                }

                for( int i = 0; i < list.Count; i++ )
                {
                    RemoveBuff(list[i]);
                }
            }
        }

        private List<Mobile> m_PermaFlags;
        private List<Mobile> m_VisList;
        private Hashtable m_AntiMacroTable;
        private TimeSpan m_GameTime;
        private TimeSpan m_ShortTermElapse;
        private TimeSpan m_LongTermElapse;
        private DateTime m_SessionStart;
        private DateTime m_LastEscortTime;
        private DateTime m_LastPetBallTime;
        private DateTime m_NextSmithBulkOrder;
        private DateTime m_NextTailorBulkOrder;
        private DateTime m_SavagePaintExpiration;
        private SkillName m_Learning = (SkillName)(-1);

        public SkillName Learning
        {
            get { return m_Learning; }
            set { m_Learning = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan SavagePaintExpiration
        {
            get
            {
                TimeSpan ts = m_SavagePaintExpiration - DateTime.Now;

                if( ts < TimeSpan.Zero )
                    ts = TimeSpan.Zero;

                return ts;
            }
            set
            {
                m_SavagePaintExpiration = DateTime.Now + value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan NextSmithBulkOrder
        {
            get
            {
                TimeSpan ts = m_NextSmithBulkOrder - DateTime.Now;

                if( ts < TimeSpan.Zero )
                    ts = TimeSpan.Zero;

                return ts;
            }
            set
            {
                try { m_NextSmithBulkOrder = DateTime.Now + value; }
                catch { }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan NextTailorBulkOrder
        {
            get
            {
                TimeSpan ts = m_NextTailorBulkOrder - DateTime.Now;

                if( ts < TimeSpan.Zero )
                    ts = TimeSpan.Zero;

                return ts;
            }
            set
            {
                try { m_NextTailorBulkOrder = DateTime.Now + value; }
                catch { }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastEscortTime
        {
            get { return m_LastEscortTime; }
            set { m_LastEscortTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastPetBallTime
        {
            get { return m_LastPetBallTime; }
            set { m_LastPetBallTime = value; }
        }

        public PlayerMobile()
        {
            m_VisList = new List<Mobile>();
            m_PermaFlags = new List<Mobile>();
            m_AntiMacroTable = new Hashtable();
            m_RecentlyReported = new List<Mobile>();

            m_BOBFilter = new Engines.BulkOrders.BOBFilter();

            m_GameTime = TimeSpan.Zero;
            m_ShortTermElapse = TimeSpan.FromHours(8.0);
            m_LongTermElapse = TimeSpan.FromHours(40.0);

            m_JusticeProtectors = new List<Mobile>();
            m_GuildRank = Guilds.RankDefinition.Lowest;
        }

        public override bool MutateSpeech( List<Mobile> hears, ref string text, ref object context )
        {
            if( Alive )
                return false;

            if( Core.ML && Skills[SkillName.SpiritSpeak].Value >= 100.0 )
                return false;

            if( Core.AOS )
            {
                for( int i = 0; i < hears.Count; ++i )
                {
                    Mobile m = hears[i];

                    if( m != this && m.Skills[SkillName.SpiritSpeak].Value >= 100.0 )
                        return false;
                }
            }

            return base.MutateSpeech(hears, ref text, ref context);
        }

        public override void DoSpeech( string text, int[] keywords, MessageType type, int hue )
        {
            if( Guilds.Guild.NewGuildSystem && (type == MessageType.Guild || type == MessageType.Alliance) )
            {
                Guilds.Guild g = this.Guild as Guilds.Guild;
                if( g == null )
                {
                    SendLocalizedMessage(1063142); // You are not in a guild!
                }
                else if( type == MessageType.Alliance )
                {
                    if( g.Alliance != null && g.Alliance.IsMember(g) )
                    {
                        //g.Alliance.AllianceTextMessage( hue, "[Alliance][{0}]: {1}", this.Name, text );
                        g.Alliance.AllianceChat(this, text);
                        SendToStaffMessage(this, "[Alliance]: {0}", text);

                        m_AllianceMessageHue = hue;
                    }
                    else
                    {
                        SendLocalizedMessage(1071020); // You are not in an alliance!
                    }
                }
                else	//Type == MessageType.Guild
                {
                    m_GuildMessageHue = hue;

                    g.GuildChat(this, text);
                    SendToStaffMessage(this, "[Guild]: {0}", text);
                }
            }
            else
            {
                base.DoSpeech(text, keywords, type, hue);
            }
        }

        private static void SendToStaffMessage( Mobile from, string text )
        {
            Packet p = null;

            foreach( NetState ns in from.GetClientsInRange(8) )
            {
                Mobile mob = ns.Mobile;

                if( mob != null && mob.AccessLevel >= AccessLevel.GameMaster && mob.AccessLevel > from.AccessLevel )
                {
                    if( p == null )
                        p = Packet.Acquire(new UnicodeMessage(from.Serial, from.Body, MessageType.Regular, from.SpeechHue, 3, from.Language, from.Name, text));

                    ns.Send(p);
                }
            }

            Packet.Release(p);
        }

        private static void SendToStaffMessage( Mobile from, string format, params object[] args )
        {
            SendToStaffMessage(from, String.Format(format, args));
        }

		public override void Damage( int amount, Mobile from )
		{
			if ( Spells.Necromancy.EvilOmenSpell.TryEndEffect( this ) )
				amount = (int)(amount * 1.25);

			Mobile oath = Spells.Necromancy.BloodOathSpell.GetBloodOath( from );

				/* Per EA's UO Herald Pub48 (ML):
				 * ((resist spellsx10)/20 + 10=percentage of damage resisted)
				 */

			if ( oath == this )
			{
				amount = (int)(amount * 1.1);

				if( amount > 35 && from is PlayerMobile )  /* capped @ 35, seems no expansion */
				{
					amount = 35;
				}

				if( Core.ML )
				{
					from.Damage( (int)(amount * ( 1 - ((( from.Skills.MagicResist.Value * .5 ) + 10) / 100 ))), this );
				}
				else
				{
					from.Damage( amount, this );
				}
			}

			base.Damage( amount, from );
		}

        #region Poison

        public override ApplyPoisonResult ApplyPoison( Mobile from, Poison poison )
        {
            if( !Alive )
                return ApplyPoisonResult.Immune;

			if ( Spells.Necromancy.EvilOmenSpell.TryEndEffect( this ) )
				poison = PoisonImpl.IncreaseLevel( poison );

            ApplyPoisonResult result = base.ApplyPoison(from, poison);

            if( from != null && result == ApplyPoisonResult.Poisoned && PoisonTimer is PoisonImpl.PoisonTimer )
                (PoisonTimer as PoisonImpl.PoisonTimer).From = from;

            return result;
        }

        #endregion

        public PlayerMobile( Serial s )
            : base(s)
        {
            m_VisList = new List<Mobile>();
            m_AntiMacroTable = new Hashtable();
        }

        public List<Mobile> VisibilityList
        {
            get { return m_VisList; }
        }

        public List<Mobile> PermaFlags
        {
            get { return m_PermaFlags; }
        }

        public override int Luck { get { return AosAttributes.GetValue(this, AosAttribute.Luck); } }

        public override bool IsHarmfulCriminal( Mobile target )
        {
            if( SkillHandlers.Stealing.ClassicMode && target is PlayerMobile && ((PlayerMobile)target).m_PermaFlags.Count > 0 )
            {
                int noto = Notoriety.Compute(this, target);

                if( noto == Notoriety.Innocent )
                    target.Delta(MobileDelta.Noto);

                return false;
            }

            if( target is BaseCreature && ((BaseCreature)target).InitialInnocent && !((BaseCreature)target).Controlled )
                return false;

            if( Core.ML && target is BaseCreature && ((BaseCreature)target).Controlled && this == ((BaseCreature)target).ControlMaster )
                return false;

            return base.IsHarmfulCriminal(target);
        }

        public bool AntiMacroCheck( Skill skill, object obj )
        {
            if( obj == null || m_AntiMacroTable == null || this.AccessLevel != AccessLevel.Player )
                return true;

            Hashtable tbl = (Hashtable)m_AntiMacroTable[skill];
            if( tbl == null )
                m_AntiMacroTable[skill] = tbl = new Hashtable();

            CountAndTimeStamp count = (CountAndTimeStamp)tbl[obj];
            if( count != null )
            {
                if( count.TimeStamp + SkillCheck.AntiMacroExpire <= DateTime.Now )
                {
                    count.Count = 1;
                    return true;
                }
                else
                {
                    ++count.Count;
                    if( count.Count <= SkillCheck.Allowance )
                        return true;
                    else
                        return false;
                }
            }
            else
            {
                tbl[obj] = count = new CountAndTimeStamp();
                count.Count = 1;

                return true;
            }
        }

        private void RevertHair()
        {
            SetHairMods(-1, -1);
        }

        private Engines.BulkOrders.BOBFilter m_BOBFilter;

        public Engines.BulkOrders.BOBFilter BOBFilter
        {
            get { return m_BOBFilter; }
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch( version )
            {

                case 29:
                    {
                        goto case 28;
                    }
                case 28:
                    {
                        m_PeacedUntil = reader.ReadDateTime();

                        goto case 27;
                    }
                case 27:
                    {
                        m_AnkhNextUse = reader.ReadDateTime();

                        goto case 26;
                    }
                case 26:
                    {
                        goto case 25;
                    }
                case 25:
                    {
                        int recipeCount = reader.ReadInt();

                        if( recipeCount > 0 )
                        {
                            m_AcquiredRecipes = new Dictionary<int, bool>();

                            for( int i = 0; i < recipeCount; i++ )
                            {
                                int r = reader.ReadInt();
                                if( reader.ReadBool() )	//Don't add in recipies which we haven't gotten or have been removed
                                    m_AcquiredRecipes.Add(r, true);
                            }
                        }
                        goto case 24;
                    }
                case 24:
                    {
                        m_LastHonorLoss = reader.ReadDeltaTime();
                        goto case 23;
                    }
                case 23:
                    {
                        goto case 22;
                    }
                case 22:
                    {
                        m_LastValorLoss = reader.ReadDateTime();
                        goto case 21;
                    }
                case 21:
                    {
                        goto case 20;
                    }
                case 20:
                    {
                        m_AllianceMessageHue = reader.ReadEncodedInt();
                        m_GuildMessageHue = reader.ReadEncodedInt();

                        goto case 19;
                    }
                case 19:
                    {
                        int rank = reader.ReadEncodedInt();
                        int maxRank = Guilds.RankDefinition.Ranks.Length - 1;
                        if( rank > maxRank )
                            rank = maxRank;

                        m_GuildRank = Guilds.RankDefinition.Ranks[rank];
                        m_LastOnline = reader.ReadDateTime();
                        goto case 18;
                    }
                case 18:
                    {
                        goto case 17;
                    }
                case 17: // changed how DoneQuests is serialized
                case 16:
                    {
                        m_Profession = reader.ReadEncodedInt();
                        goto case 15;
                    }
                case 15:
                    {
                        m_LastCompassionLoss = reader.ReadDeltaTime();
                        goto case 14;
                    }
                case 14:
                    {
                        m_CompassionGains = reader.ReadEncodedInt();

                        if( m_CompassionGains > 0 )
                            m_NextCompassionDay = reader.ReadDeltaTime();

                        goto case 13;
                    }
                case 13: // just removed m_PayedInsurance list
                case 12:
                    {
                        m_BOBFilter = new Engines.BulkOrders.BOBFilter(reader);
                        goto case 11;
                    }
                case 11:
                    {
                        if( version < 13 )
                        {
                            List<Item> payed = reader.ReadStrongItemList();

                            for( int i = 0; i < payed.Count; ++i )
                                payed[i].PayedInsurance = true;
                        }

                        goto case 10;
                    }
                case 10:
                    {
                        if( reader.ReadBool() )
                        {
                            m_HairModID = reader.ReadInt();
                            m_HairModHue = reader.ReadInt();
                            m_BeardModID = reader.ReadInt();
                            m_BeardModHue = reader.ReadInt();
                        }

                        goto case 9;
                    }
                case 9:
                    {
                        SavagePaintExpiration = reader.ReadTimeSpan();

                        if( SavagePaintExpiration > TimeSpan.Zero )
                        {
                            BodyMod = (Female ? 184 : 183);
                            HueMod = 0;
                        }

                        goto case 8;
                    }
                case 8:
                    {
                        m_NpcGuild = (NpcGuild)reader.ReadInt();
                        m_NpcGuildJoinTime = reader.ReadDateTime();
                        m_NpcGuildGameTime = reader.ReadTimeSpan();
                        goto case 7;
                    }
                case 7:
                    {
                        m_PermaFlags = reader.ReadStrongMobileList();
                        goto case 6;
                    }
                case 6:
                    {
                        NextTailorBulkOrder = reader.ReadTimeSpan();
                        goto case 5;
                    }
                case 5:
                    {
                        NextSmithBulkOrder = reader.ReadTimeSpan();
                        goto case 4;
                    }
                case 4:
                    {
                        m_LastJusticeLoss = reader.ReadDeltaTime();
                        m_JusticeProtectors = reader.ReadStrongMobileList();
                        goto case 3;
                    }
                case 3:
                    {
                        m_LastSacrificeGain = reader.ReadDeltaTime();
                        m_LastSacrificeLoss = reader.ReadDeltaTime();
                        m_AvailableResurrects = reader.ReadInt();
                        goto case 2;
                    }
                case 2:
                    {
                        m_Flags = (PMFlag)reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        m_LongTermElapse = reader.ReadTimeSpan();
                        m_ShortTermElapse = reader.ReadTimeSpan();
                        m_GameTime = reader.ReadTimeSpan();
                        goto case 0;
                    }
                case 0:
                    {
                        break;
                    }
            }

            if( m_RecentlyReported == null )
                m_RecentlyReported = new List<Mobile>();

            // Professions weren't verified on 1.0 RC0
            if( !CharacterCreation.VerifyProfession(m_Profession) )
                m_Profession = 0;

            if( m_PermaFlags == null )
                m_PermaFlags = new List<Mobile>();

            if( m_JusticeProtectors == null )
                m_JusticeProtectors = new List<Mobile>();

            if( m_BOBFilter == null )
                m_BOBFilter = new Engines.BulkOrders.BOBFilter();

            if( m_GuildRank == null )
                m_GuildRank = Guilds.RankDefinition.Member;	//Default to member if going from older version to new version (only time it should be null)

            if( m_LastOnline == DateTime.MinValue && Account != null )
                m_LastOnline = ((Account)Account).LastLogin;

            if( AccessLevel > AccessLevel.Player )
                m_IgnoreMobiles = true;

            List<Mobile> list = this.Stabled;

            for( int i = 0; i < list.Count; ++i )
            {
                BaseCreature bc = list[i] as BaseCreature;

                if( bc != null )
                {
                    bc.IsStabled = true;
                    bc.StabledBy = this;
                }
            }

            CheckAtrophies(this);

            if( Hidden )	//Hiding is the only buff where it has an effect that's serialized.
                AddBuff(new BuffInfo(BuffIcon.HidingAndOrStealth, 1075655));
        }

        public override void Serialize( GenericWriter writer )
        {
            //cleanup our anti-macro table
            foreach( Hashtable t in m_AntiMacroTable.Values )
            {
                ArrayList remove = new ArrayList();
                foreach( CountAndTimeStamp time in t.Values )
                {
                    if( time.TimeStamp + SkillCheck.AntiMacroExpire <= DateTime.Now )
                        remove.Add(time);
                }

                for( int i = 0; i < remove.Count; ++i )
                    t.Remove(remove[i]);
            }

            CheckKillDecay();

            CheckAtrophies(this);

            base.Serialize(writer);

            writer.Write((int)29); // version

            writer.Write((DateTime)m_PeacedUntil);
            writer.Write((DateTime)m_AnkhNextUse);

            if( m_AcquiredRecipes == null )
            {
                writer.Write((int)0);
            }
            else
            {
                writer.Write(m_AcquiredRecipes.Count);

                foreach( KeyValuePair<int, bool> kvp in m_AcquiredRecipes )
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value);
                }
            }

            writer.WriteDeltaTime(m_LastHonorLoss);

            writer.Write(m_LastValorLoss);

            writer.WriteEncodedInt(m_AllianceMessageHue);
            writer.WriteEncodedInt(m_GuildMessageHue);

            writer.WriteEncodedInt(m_GuildRank.Rank);
            writer.Write(m_LastOnline);

            writer.WriteEncodedInt((int)m_Profession);

            writer.WriteDeltaTime(m_LastCompassionLoss);

            writer.WriteEncodedInt(m_CompassionGains);

            if( m_CompassionGains > 0 )
                writer.WriteDeltaTime(m_NextCompassionDay);

            m_BOBFilter.Serialize(writer);

            bool useMods = (m_HairModID != -1 || m_BeardModID != -1);

            writer.Write(useMods);

            if( useMods )
            {
                writer.Write((int)m_HairModID);
                writer.Write((int)m_HairModHue);
                writer.Write((int)m_BeardModID);
                writer.Write((int)m_BeardModHue);
            }

            writer.Write(SavagePaintExpiration);

            writer.Write((int)m_NpcGuild);
            writer.Write((DateTime)m_NpcGuildJoinTime);
            writer.Write((TimeSpan)m_NpcGuildGameTime);

            writer.Write(m_PermaFlags, true);

            writer.Write(NextTailorBulkOrder);

            writer.Write(NextSmithBulkOrder);

            writer.WriteDeltaTime(m_LastJusticeLoss);
            writer.Write(m_JusticeProtectors, true);

            writer.WriteDeltaTime(m_LastSacrificeGain);
            writer.WriteDeltaTime(m_LastSacrificeLoss);
            writer.Write(m_AvailableResurrects);

            writer.Write((int)m_Flags);

            writer.Write(m_LongTermElapse);
            writer.Write(m_ShortTermElapse);
            writer.Write(this.GameTime);
        }

        public static void CheckAtrophies( Mobile m )
        {
            SacrificeVirtue.CheckAtrophy(m);
            JusticeVirtue.CheckAtrophy(m);
            CompassionVirtue.CheckAtrophy(m);
            ValorVirtue.CheckAtrophy(m);
        }

        public void CheckKillDecay()
        {
            if( m_ShortTermElapse < this.GameTime )
            {
                m_ShortTermElapse += TimeSpan.FromHours(8);
                if( ShortTermMurders > 0 )
                    --ShortTermMurders;
            }

            if( m_LongTermElapse < this.GameTime )
            {
                m_LongTermElapse += TimeSpan.FromHours(40);
                if( Kills > 0 )
                    --Kills;
            }
        }

        public void ResetKillTime()
        {
            m_ShortTermElapse = this.GameTime + TimeSpan.FromHours(8);
            m_LongTermElapse = this.GameTime + TimeSpan.FromHours(40);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime SessionStart
        {
            get { return m_SessionStart; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan GameTime
        {
            get
            {
                if( NetState != null )
                    return m_GameTime + (DateTime.Now - m_SessionStart);
                else
                    return m_GameTime;
            }
        }

        public override bool CanSee( Mobile m )
        {
            if( m is PlayerMobile && ((PlayerMobile)m).m_VisList.Contains(this) )
                return true;

            return base.CanSee(m);
        }

        public override bool CanSee( Item item )
        {
            if( m_DesignContext != null && m_DesignContext.Foundation.IsHiddenToCustomizer(item) )
                return false;

            return base.CanSee(item);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            BaseHouse.HandleDeletion(this);

            DisguiseTimers.RemoveTimer(this);
        }

        public override bool NewGuildDisplay { get { return Server.Guilds.Guild.NewGuildSystem; } }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties(list);      

            if( Core.ML )
            {
                for( int i = AllFollowers.Count - 1; i >= 0; i-- )
                {
                    BaseCreature c = AllFollowers[i] as BaseCreature;

                    if( c != null && c.ControlOrder == OrderType.Guard )
                    {
                        list.Add(501129); // guarded
                        break;
                    }
                }
            }

            if (((IPlayerCombatant)this).CombatProfile != null)
            {
                    string color = "";
                    int rating = ((IPlayerCombatant)this).CombatProfile.CombatRating;
                    CombatantProfile profile = ((IPlayerCombatant)this).CombatProfile;

                    switch (profile.Rank)
                    {
                        case EqPvP.CombatantProfile.CombatRank.Pawn: color = "#5A5ABA"; break;
                        case EqPvP.CombatantProfile.CombatRank.Bishop: color = "#5A5ABA"; break;
                        case EqPvP.CombatantProfile.CombatRank.Knight: color = "#C10000"; break;
                        case EqPvP.CombatantProfile.CombatRank.Rook: color = "#C10000"; break;
                        case EqPvP.CombatantProfile.CombatRank.King: color = "#006A00"; break;
                        case EqPvP.CombatantProfile.CombatRank.Queen: color = "#006A00"; break;
                        case EqPvP.CombatantProfile.CombatRank.Master: color = "#010198"; break;
                    }

                    list.Add(1060658, "{0}\t{1}", "Combat Rating", String.Format("<BASEFONT COLOR={0}>{1}", color, profile.CombatRating));
            }
        }

        protected override bool OnMove( Direction d )
        {
            if( !Core.SE )
                return base.OnMove(d);

            if( AccessLevel != AccessLevel.Player )
                return true;

            if( Hidden && DesignContext.Find(this) == null )	//Hidden & NOT customizing a house
            {
                if( !Mounted && Skills.Stealth.Value >= 25.0 )
                {
                    bool running = (d & Direction.Running) != 0;

                    if( running )
                    {
                        if( (AllowedStealthSteps -= 2) <= 0 )
                            RevealingAction();
                    }
                    else if( AllowedStealthSteps-- <= 0 )
                    {
                        Server.SkillHandlers.Stealth.OnUse(this);
                    }
                }
                else
                {
                    RevealingAction();
                }
            }

            return true;
        }

        private bool m_BedrollLogout;

        public bool BedrollLogout
        {
            get { return m_BedrollLogout; }
            set { m_BedrollLogout = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override bool Paralyzed
        {
            get
            {
                return base.Paralyzed;
            }
            set
            {
                base.Paralyzed = value;

                if( value )
                    AddBuff(new BuffInfo(BuffIcon.Paralyze, 1075827));	//Paralyze/You are frozen and can not move
                else
                    RemoveBuff(BuffIcon.Paralyze);
            }
        }

        public override void OnAccessLevelChanged( AccessLevel oldLevel )
        {
            if( AccessLevel == AccessLevel.Player )
                IgnoreMobiles = false;
            else
                IgnoreMobiles = true;
        }

        public override void OnDelete()
        {
            if( m_ReceivedHonorContext != null )
                m_ReceivedHonorContext.Cancel();
            if( m_SentHonorContext != null )
                m_SentHonorContext.Cancel();
        }

        #region Fastwalk Prevention
        private static bool FastwalkPrevention = true; // Is fastwalk prevention enabled?
        private static TimeSpan FastwalkThreshold = TimeSpan.FromSeconds(0.4); // Fastwalk prevention will become active after 0.4 seconds

        public DateTime NextMovementTime { get{ return m_NextMovementTime; } }
        private DateTime m_NextMovementTime;

        public virtual bool UsesFastwalkPrevention { get { return (AccessLevel < AccessLevel.Counselor); } }

        public override TimeSpan ComputeMovementSpeed( Direction dir, bool checkTurning )
        {
            if( checkTurning && (dir & Direction.Mask) != (this.Direction & Direction.Mask) )
                return Mobile.RunMount;	// We are NOT actually moving (just a direction change)

            bool running = ((dir & Direction.Running) != 0);
            bool onHorse = (this.Mount != null);

	    AnimalFormContext animalContext = AnimalForm.GetContext( this );

	    if( onHorse || (animalContext != null && animalContext.SpeedBoost) )
                return (running ? Mobile.RunMount : Mobile.WalkMount);

            return (running ? Mobile.RunFoot : Mobile.WalkFoot);
        }

        public static bool MovementThrottle_Callback( NetState ns )
        {
            PlayerMobile pm = ns.Mobile as PlayerMobile;

            if( pm == null || !pm.UsesFastwalkPrevention )
                return true;

            if( pm.m_NextMovementTime == DateTime.MinValue )
            {
                // has not yet moved
                pm.m_NextMovementTime = DateTime.Now;
                return true;
            }

            TimeSpan ts = pm.m_NextMovementTime - DateTime.Now;

            if( ts < TimeSpan.Zero )
            {
                // been a while since we've last moved
                pm.m_NextMovementTime = DateTime.Now;
                return true;
            }

            return (ts < FastwalkThreshold);
        }

        #endregion

	#region Enemy of One
		private Type m_EnemyOfOneType;
		private bool m_WaitingForEnemy;

		public Type EnemyOfOneType
		{
			get{ return m_EnemyOfOneType; }
			set
			{
				Type oldType = m_EnemyOfOneType;
				Type newType = value;

				if ( oldType == newType )
					return;

				m_EnemyOfOneType = value;

				DeltaEnemies( oldType, newType );
			}
		}

		public bool WaitingForEnemy
		{
			get{ return m_WaitingForEnemy; }
			set{ m_WaitingForEnemy = value; }
		}

		private void DeltaEnemies( Type oldType, Type newType )
		{
			foreach ( Mobile m in this.GetMobilesInRange( 18 ) )
			{
				Type t = m.GetType();

				if ( t == oldType || t == newType ) {
					NetState ns = this.NetState;

					if ( ns != null ) {
						if ( ns.StygianAbyss ) {
							ns.Send( new MobileMoving( m, Notoriety.Compute( this, m ) ) );
						} else {
							ns.Send( new MobileMovingOld( m, Notoriety.Compute( this, m ) ) );
						}
					}
				}
			}
		}

		#endregion

        #region Hair and beard mods
        private int m_HairModID = -1, m_HairModHue;
        private int m_BeardModID = -1, m_BeardModHue;

        public void SetHairMods( int hairID, int beardID )
        {
            if( hairID == -1 )
                InternalRestoreHair(true, ref m_HairModID, ref m_HairModHue);
            else if( hairID != -2 )
                InternalChangeHair(true, hairID, ref m_HairModID, ref m_HairModHue);

            if( beardID == -1 )
                InternalRestoreHair(false, ref m_BeardModID, ref m_BeardModHue);
            else if( beardID != -2 )
                InternalChangeHair(false, beardID, ref m_BeardModID, ref m_BeardModHue);
        }

        private void CreateHair( bool hair, int id, int hue )
        {
            if( hair )
            {
                //TODO Verification?
                HairItemID = id;
                HairHue = hue;
            }
            else
            {
                FacialHairItemID = id;
                FacialHairHue = hue;
            }
        }

        private void InternalRestoreHair( bool hair, ref int id, ref int hue )
        {
            if( id == -1 )
                return;

            if( hair )
                HairItemID = 0;
            else
                FacialHairItemID = 0;

            //if( id != 0 )
            CreateHair(hair, id, hue);

            id = -1;
            hue = 0;
        }

        private void InternalChangeHair( bool hair, int id, ref int storeID, ref int storeHue )
        {
            if( storeID == -1 )
            {
                storeID = hair ? HairItemID : FacialHairItemID;
                storeHue = hair ? HairHue : FacialHairHue;
            }
            CreateHair(hair, id, 0);
        }

        #endregion

        #region Virtues
        private DateTime m_LastSacrificeGain;
        private DateTime m_LastSacrificeLoss;
        private int m_AvailableResurrects;

        public DateTime LastSacrificeGain { get { return m_LastSacrificeGain; } set { m_LastSacrificeGain = value; } }
        public DateTime LastSacrificeLoss { get { return m_LastSacrificeLoss; } set { m_LastSacrificeLoss = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int AvailableResurrects { get { return m_AvailableResurrects; } set { m_AvailableResurrects = value; } }

        private DateTime m_NextJustAward;
        private DateTime m_LastJusticeLoss;
        private List<Mobile> m_JusticeProtectors;

        public DateTime LastJusticeLoss { get { return m_LastJusticeLoss; } set { m_LastJusticeLoss = value; } }
        public List<Mobile> JusticeProtectors { get { return m_JusticeProtectors; } set { m_JusticeProtectors = value; } }

        private DateTime m_LastCompassionLoss;
        private DateTime m_NextCompassionDay;
        private int m_CompassionGains;

        public DateTime LastCompassionLoss { get { return m_LastCompassionLoss; } set { m_LastCompassionLoss = value; } }
        public DateTime NextCompassionDay { get { return m_NextCompassionDay; } set { m_NextCompassionDay = value; } }
        public int CompassionGains { get { return m_CompassionGains; } set { m_CompassionGains = value; } }

        private DateTime m_LastValorLoss;

        public DateTime LastValorLoss { get { return m_LastValorLoss; } set { m_LastValorLoss = value; } }

        private DateTime m_LastHonorLoss;
        private DateTime m_LastHonorUse;
        private bool m_HonorActive;
        private HonorContext m_ReceivedHonorContext;
        private HonorContext m_SentHonorContext;
        public DateTime m_hontime;

        public DateTime LastHonorLoss { get { return m_LastHonorLoss; } set { m_LastHonorLoss = value; } }
        public DateTime LastHonorUse { get { return m_LastHonorUse; } set { m_LastHonorUse = value; } }
        public bool HonorActive { get { return m_HonorActive; } set { m_HonorActive = value; } }
        public HonorContext ReceivedHonorContext { get { return m_ReceivedHonorContext; } set { m_ReceivedHonorContext = value; } }
        public HonorContext SentHonorContext { get { return m_SentHonorContext; } set { m_SentHonorContext = value; } }
        #endregion

        public override TimeSpan GetLogoutDelay()
        {
            if( BedrollLogout )
                return TimeSpan.Zero;

            return base.GetLogoutDelay();
        }

        #region Speech log
        private SpeechLog m_SpeechLog;

        public SpeechLog SpeechLog { get { return m_SpeechLog; } }

        public override void OnSpeech( SpeechEventArgs e )
        {
            if( SpeechLog.Enabled && this.NetState != null )
            {
                if( m_SpeechLog == null )
                    m_SpeechLog = new SpeechLog();

                m_SpeechLog.Add(e.Mobile, e.Speech);
            }
        }

        #endregion

        #region Recipes

        private Dictionary<int, bool> m_AcquiredRecipes;

        public virtual bool HasRecipe( Recipe r )
        {
            if( r == null )
                return false;

            return HasRecipe(r.ID);
        }

        public virtual bool HasRecipe( int recipeID )
        {
            if( m_AcquiredRecipes != null && m_AcquiredRecipes.ContainsKey(recipeID) )
                return m_AcquiredRecipes[recipeID];

            return false;
        }

        public virtual void AcquireRecipe( Recipe r )
        {
            if( r != null )
                AcquireRecipe(r.ID);
        }

        public virtual void AcquireRecipe( int recipeID )
        {
            if( m_AcquiredRecipes == null )
                m_AcquiredRecipes = new Dictionary<int, bool>();

            m_AcquiredRecipes[recipeID] = true;
        }

        public virtual void ResetRecipes()
        {
            m_AcquiredRecipes = null;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int KnownRecipes
        {
            get
            {
                if( m_AcquiredRecipes == null )
                    return 0;

                return m_AcquiredRecipes.Count;
            }
        }

        #endregion

        #region Buff Icons

        public void ResendBuffs()
        {
            if( !BuffInfo.Enabled || m_BuffTable == null )
                return;

            NetState state = this.NetState;

            if( state != null && state.BuffIcon )
            {
                foreach( BuffInfo info in m_BuffTable.Values )
                {
                    state.Send(new AddBuffPacket(this, info));
                }
            }
        }

        private Dictionary<BuffIcon, BuffInfo> m_BuffTable;

        public void AddBuff( BuffInfo b )
        {
            if( !BuffInfo.Enabled || b == null )
                return;

            RemoveBuff(b);	//Check & subsequently remove the old one.

            if( m_BuffTable == null )
                m_BuffTable = new Dictionary<BuffIcon, BuffInfo>();

            m_BuffTable.Add(b.ID, b);

            NetState state = this.NetState;

            if( state != null && state.BuffIcon )
            {
                state.Send(new AddBuffPacket(this, b));
            }
        }

        public void RemoveBuff( BuffInfo b )
        {
            if( b == null )
                return;

            RemoveBuff(b.ID);
        }

        public void RemoveBuff( BuffIcon b )
        {
            if( m_BuffTable == null || !m_BuffTable.ContainsKey(b) )
                return;

            BuffInfo info = m_BuffTable[b];

            if( info.Timer != null && info.Timer.Running )
                info.Timer.Stop();

            m_BuffTable.Remove(b);

            NetState state = this.NetState;

            if( state != null && state.BuffIcon )
            {
                state.Send(new RemoveBuffPacket(this, b));
            }

            if( m_BuffTable.Count <= 0 )
                m_BuffTable = null;
        }

        #endregion
    }
}