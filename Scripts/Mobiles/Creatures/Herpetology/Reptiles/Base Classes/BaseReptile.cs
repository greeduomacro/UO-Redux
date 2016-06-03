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
        static int[] m_BodyValueSet1 = new int[] { 52, 92, 0xCA, 0xCE, 62, 60, 12, 46 };

        internal static double Pi = 3.14159265359;
        internal static double Phi = 1.618;
        internal static double Modus = ((1024 * 128) / Pi) / Phi;

        internal int m_LastLevel;

        public static int[] GetBodyValues(BaseReptile m)
        {
            if (m.m_DomRecessive)
                return m_BodyValueSet0;
            else return m_BodyValueSet1;
        }

        internal static string[] m_StageNames = new string[] 
        { 
            "reptilian hatchling", // 0
            "giant serpent",       // 1
            "large reptile",       // 2
            "massive lizard",      // 3
            "young wyrm",          // 4
            "adolesecent dragon",  // 5
            "adult dragon",
            "elder wyrm"
        };

        internal static string GetArticle(int level)
        {
            if (level < 4) 
                return "a";
            else 
                return "an";
        }

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
            set { m_Level = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsEvolving
        {
            get { return false; }
            set { if (value) { Evolve(); } }
        }

        public BaseReptile
            (AIType ai, FightMode mode, int iRangePerception, int iRangeFight, double activeSpeed, double passiveSpeed) 
            :   base(ai, mode, iRangePerception, iRangeFight, activeSpeed, passiveSpeed)
        {            
            m_DomRecessive = Utility.RandomBool();
            QueryLevel();
            switch (Utility.RandomMinMax(1, 5))
            {
                case 1: { Hue = Utility.RandomSnakeHue(); break; }
                case 2: { Hue = Utility.RandomSkinHue(); break; }
                case 3: { Hue = Utility.RandomBirdHue(); break; }
                case 4: { Hue = Utility.RandomMetalHue(); break; }
                case 5: { Hue = Utility.RandomHairHue(); break; }
                default: break;
            }
        }

        internal void QueryLevel()
        {
            if (m_Level == -1)
            {
                int temp = QueryLevelFromForm();
                if (temp != -1) m_Level = temp;
            }
        }

        internal int QueryLevelFromForm()
        {
            try
            {
                int lvl = 0;
                for (int i = 0; i < m_BodyValueSet0.Length; i++)
                {
                    if (m_BodyValueSet0[i] == BodyValue)
                    {
                        lvl = i;
                    }
                    else if (m_BodyValueSet1[i] == BodyValue)
                    {
                        lvl = i;
                    }
                }
                return lvl;
            }

            catch (Exception e) { eqUtility.HandleGenericException(e); return -1; }
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
            writer.Write(0); //Version
            writer.Write(m_LastLevel);
            writer.Write(m_Level);
            writer.Write(m_LaysEggs);
            writer.Write(m_MaxLevel);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            if (reader.ReadInt() >= 0)
            {
                m_LastLevel = reader.ReadInt();
                m_Level = reader.ReadInt();
                m_LaysEggs = reader.ReadBool();
                m_MaxLevel = reader.ReadInt();
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            IncreaseExperience(this, Utility.RandomMinMax(1,3));
            base.OnDamage(amount, from, willKill);
        }

        public override void DoHarmful(Mobile target)
        {
            if(Utility.RandomBool())
                IncreaseExperience(this, Utility.RandomMinMax(1, 3));
            base.DoHarmful(target);
        }

        public override void OnGotMeleeAttack( Mobile attacker )
		{
            IncreaseExperience(this, 1);
			base.OnGotMeleeAttack( attacker );
		}

        public override void OnThink()
        {
            if (eqUtility.HardBool())
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

            catch (Exception e) { eqUtility.HandleMobileException(e, m); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int NextEvolution
        {
            get { return (int)(Modus * (this.Level + 1)) / 2; }
        }

        private static void QueryEvolutionStatus(BaseReptile m)
        {
            try
            {
                int _temp = (int)(Modus * (m.Level + 1)) / 2;
                if (m.Experience >= _temp)
                {
                    m.Evolve();
                }
            }

            catch (Exception e) { eqUtility.HandleMobileException(e, m); }
        }

        private void Evolve()
        {
            try 
            {
                m_LastLevel = Level;
                Level++;

                UpdateAppearances();
                Experience = 0;
                GenerateEffects(this);

                for (int i = m_LastLevel; i <= Level; i++)
                {
                    UpgradeStat(this);
                }
            }

            catch (Exception e) { eqUtility.HandleMobileException(e, this); }

        }

        private void UpdateAppearances()
        {
            try
            {
                Body = GetBodyValues(this)[Level];
                BaseSoundID = m_StageSounds[Level];
                Name = GetArticle(Level) + " " + m_StageNames[Level];
            }

            catch (Exception e) { eqUtility.HandleMobileException(e, this); }
        }

        private void UpgradeStat(BaseReptile from)
        {
            try
            {
                from.RawStr += (int)(from.RawStr * 0.1618);
                from.RawDex += (int)(from.RawStr * 0.1618);
                from.RawInt += (int)(from.RawStr * 0.1618);

                from.HitsMaxSeed += (int)(from.HitsMaxSeed * 0.1618);
                from.ManaMaxSeed += (int)(from.ManaMaxSeed * 0.1618);

                int newMax = DamageMax + Utility.RandomMinMax(3, 9);
                int newMin = DamageMin + Utility.RandomMinMax(2, 8);

                SetDamage(newMin, newMax);

                for(int i = 0; i < from.Skills.Length; ++i)
                {
                    Skill s = from.Skills[i];
                    if (s.Base > 0.0 && s.Base < 120.0)
                    {
                        s.Base += (int)
                            (s.Base * 0.01618 + Utility.RandomMinMax(1, 2));

                        if (s.Base > 120.0) 
                            s.Base = 120.0;
                    }
                }

                for (int i = 0; i < from.Resistances.Length; i++)
                {
                    from.Resistances[i] += Utility.RandomMinMax(4,6);
                }

                from.VirtualArmor += Utility.RandomMinMax(5, 12);
            }

            catch (Exception e) { eqUtility.HandleMobileException(e, this); }
        }

        internal static void GenerateEffects(BaseReptile from)
        {
            ReptileUtility.GenerateEffects(from);
        }

        public override void OnCarve(Mobile from, Items.Corpse corpse, Item with)
        {
            base.OnCarve(from, corpse, with);

            if (m_PoisonType.Level >= Poison.Lesser.Level)
                corpse.AddItem(new ReptileVenomSack(m_PoisonType));

            if (Utility.RandomDouble() <= 0.015)
            {
                corpse.AddItem(new ReptileEgg(m_Type));
                from.SendMessage("Carving into the creature you notice something pearl white.");
            }
        }
    }

    public class EvolutionCreature : BaseReptile
    {
        [Constructable]
        public EvolutionCreature()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = GetArticle(this.Level) 
                + " " + BaseReptile.m_StageNames[0];
            Body = 52;
            Hue = Utility.RandomSnakeHue();
            BaseSoundID = 0xDB;

            SetStr(33, 100);
            SetDex(33, 100);
            SetInt(33, 100);

            SetHits(55, 100);
            SetMana(50, 95);

            SetDamage(3, 5);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 20);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Fire, 15, 20);
            SetResistance(ResistanceType.Energy, 20, 30);
            SetResistance(ResistanceType.Cold, 15, 20);

            SetSkill(SkillName.Anatomy, 15.1, 36.0);
            SetSkill(SkillName.MagicResist, 15.1, 20.0);
            SetSkill(SkillName.Tactics, 19.3, 34.0);
            SetSkill(SkillName.Wrestling, 19.3, 34.0);
            SetSkill(SkillName.Magery, 19.3, 34.0);
            SetSkill(SkillName.EvalInt, 19.3, 34.0);
            SetSkill(SkillName.Meditation, 19.3, 34.0);
            Fame = 300;

            VirtualArmor = 16;
            Tamable = false;
            ControlSlots = 3;
        }

        public virtual FoodType FavoriteFood { get { return FoodType.Gold; } }

        public EvolutionCreature(Serial serial) : base(serial)
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); //Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            if (reader.ReadInt() >= 0)
            {
            }
        }
    }
}
