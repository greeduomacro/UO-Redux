using Server.Items;
using Server.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Mobiles.Creatures.Reptiles
{
    [CorpseName("a reptilian corpse")]
    public class BaseReptile : BaseCreature
    {
        static int[] m_BodyValueSet0 = new int[] { 52, 92, 0xCA, 0xCE, 62, 61, 59, 46 };
        static int[] m_BodyValueSet1 = new int[] { 52, 92, 0xCA, 0xCE, 62, 61, 59, 46 };

        internal static double Pi = 3.14159265359;
        internal static double Phi = 1.618;
        internal static double Modus = ((1024 * 128) / Pi) / Phi;

        public static int[] GetBodyValues(BaseReptile m)
        {
            if (m.m_DomRecessive)
                return m_BodyValueSet0;
            else return m_BodyValueSet1;
        }

        internal static string[] m_StageNames = new string[] 
        { 
            "reptilian hatchling", // 1
            "giant serpent",       // 2
            "large reptile",       // 3
            "massive lizard",      // 4
            "young wyrm",          // 5
            "adolesecent dragon",  // 6
            "adult dragon",
            "elder wyrm"
        };

        static int[] m_StageSounds = new int[]
        {
            0xDB, 219, 660, 0x5A, 362, 362, 362, 362
        };

        internal bool m_DomRecessive = false;

        ReptileType m_Type;
        internal ReptileType rType
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        private static bool m_LaysEggs;
        internal static bool LaysEggs
        {
            get { return m_LaysEggs; }
            set { m_LaysEggs = value; }
        }

        private static bool m_CanEvolve = false;
        internal static bool CanEvolve
        {
            get { return m_CanEvolve; }
            set { m_CanEvolve = value; }
        }

        private int m_Experience = 0;
                
        [CommandProperty(AccessLevel.GameMaster)]
        public int Experience
        {
            get { return m_Experience; }
            set { m_Experience = value; }
        }

        private int m_MaxLevel = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxLevel
        {
            get { return m_MaxLevel; }
            set { m_MaxLevel = value; }
        }

        private int m_Level = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Level
        {
            get { return m_Level; }
            set { m_Level = value; Evolve(m_Level); }
        }

        public BaseReptile
            (AIType ai, FightMode mode, int iRangePerception, int iRangeFight, double activeSpeed, double passiveSpeed) 
            :   base(ai, mode, iRangePerception, iRangeFight, activeSpeed, passiveSpeed)
        {
            Hue = Utility.RandomSnakeHue();
            m_DomRecessive = Utility.RandomBool();
        }

        public override bool BardImmune
        {
	        get 
	        { 
                return base.BardImmune;
	        }
        }

        public Poison m_PoisonType;

        public override bool CanAngerOnTame { get { return true; } }
        public override bool HasBreath{ get{ return false; } } // fire breath enabled
		public override bool AutoDispel{ get{ return false; } }
		public override HideType HideType{ get{ return HideType.Regular; } }
		public override int Hides{ get{ return 1; } }
		public override int Meat{ get{ return 1; } }
		public override int Scales{ get{ return 1; } }
		public override ScaleType ScaleType{ get{ return (ScaleType)Utility.Random( 4 ); } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override int TreasureMapLevel{ get{ return 5; } }

        public BaseReptile(Serial serial) : base(serial)
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            IncreaseExperience(this, Utility.RandomMinMax(1,3));
            base.OnDamage(amount, from, willKill);
        }

        public override void DoHarmful(Mobile target)
        {
            IncreaseExperience(this, Utility.RandomMinMax(1, 3));
            base.DoHarmful(target);
        }

        public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );
		}

        public override void OnThink()
        {
            IncreaseExperience(this, 1);
            base.OnThink();
        }

        public static void IncreaseExperience(BaseReptile m, int val)
        {
            try
            {
                m.Experience += val;
                QueryEvolutionStatus(m);
            }

            catch (Exception e) { eqUtility.HandleMobileException(e, this); }
        }

        private static void QueryEvolutionStatus(BaseReptile m)
        {
            try
            {
                int _temp = (int)(Modus * (m.Level + 1)) / 2;
                if (m.Experience >= _temp)
                {
                    m.Evolve(m.Level + 1);
                }
            }

            catch (Exception e) { eqUtility.HandleMobileException(e, this); }
        }

        private void Evolve(int level)
        {
            try 
            {
                UpdateAppearances();
                Level = level;
                Experience = 0;
                GenerateEffects(this);
            }

            catch (Exception e) { eqUtility.HandleMobileException(e, this); }

        }

        private void UpdateAppearances()
        {
            try
            {
                Body = GetBodyValues(this)[Level + 1];
                BaseSoundID = m_StageSounds[Level + 1];
                Name = "a " + m_StageNames[Level + 1];
            }

            catch (Exception e) { eqUtility.HandleMobileException(e, this); }
        }

        private void UpgradeStat(BaseReptile from)
        {
            try
            {
                from.RawStr += (int)(RawStr * 1.618);
                from.RawDex += (int)(RawDex * 1.618);
                from.RawInt += (int)(RawInt * 1.618);

                foreach (Skill s in from.Skills)
                {
                    s.Base += s.Base * 0.1618;
                }

                for (int i = 0; i <= from.Resistances.Length; i++)
                {
                    from.Resistances[i] += Utility.RandomMinMax(2, 4);
                }

                from.VirtualArmor += Utility.RandomMinMax(8, 16);
            }

            catch (Exception e) { eqUtility.HandleMobileException(e, this); }
        }

        internal static void GenerateEffects(BaseReptile from)
        {
            try
            {
                Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0, 0, 0, 0, 0, 5060, 0);
                Effects.PlaySound(from.Location, from.Map, 0x243);

                Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 6, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 4, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 6, from.Y - 4, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

                Effects.SendTargetParticles(from, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);
            }

            catch (Exception e) { eqUtility.HandleMobileException(e, from); }
        }

        public override void OnCarve(Mobile from, Items.Corpse corpse, Item with)
        {
            base.OnCarve(from, corpse, with);

            if (m_PoisonType.Level >= Poison.Lesser.Level)
                corpse.AddItem(new ReptileVenomSack(m_PoisonType));

            if (Utility.RandomBool() && Utility.RandomBool())
            {
                corpse.AddItem(new ReptileEgg(m_Type));
                from.SendMessage("Carving into the dragon you notice something pearl white.");
            }
        }
    }

    public class EvolutionCreature : BaseReptile
    {
        [Constructable]
        public EvolutionCreature()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a" + " " + BaseReptile.m_StageNames[0];
            Body = 52;
            Hue = Utility.RandomSnakeHue();
            BaseSoundID = 0xDB;

            SetStr(33, 100);
            SetDex(33, 100);
            SetInt(33, 100);

            SetHits(55, 55);
            SetMana(50, 50);

            SetDamage(1, 4);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 20);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Fire, 15, 20);
            SetResistance(ResistanceType.Energy, 20, 30);
            SetResistance(ResistanceType.Cold, 15, 20);

            SetSkill(SkillName.Poisoning, 50.1, 70.0);
            SetSkill(SkillName.MagicResist, 15.1, 20.0);
            SetSkill(SkillName.Tactics, 19.3, 34.0);
            SetSkill(SkillName.Wrestling, 19.3, 34.0);

            Fame = 300;

            VirtualArmor = 16;

            Tamable = true;
            ControlSlots = 1;
        }

        public EvolutionCreature(Serial serial) : base(serial)
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }
}
