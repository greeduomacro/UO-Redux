using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a hijnoral corpse")]
    public class Hijnoral : BaseCreature
    {
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.MortalStrike;
        }

        [Constructable]
        public Hijnoral()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a hijnoral";
            BodyValue = 9;
            Hue = Utility.RandomOrangeHue();
            BaseSoundID = 357;

            SetStr(890, 1000);
            SetDex(250, 300);
            SetInt(100, 150);

            SetHits(600, 680);

            SetDamage(12, 16);
            SetDamageType(ResistanceType.Physical, 0);
            SetDamageType(ResistanceType.Fire, 100);

            SetResistance(ResistanceType.Physical, 65, 75);
            SetResistance(ResistanceType.Fire, 100);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.Tactics, 100, 110);
            SetSkill(SkillName.Wrestling, 85, 95);
            SetSkill(SkillName.Anatomy, 80, 95);
            SetSkill(SkillName.MagicResist, 115, 125);

            Fame = 25000;
            Karma = -25000;

            VirtualArmor = 85;

            if( Utility.Random(10) == 1 )
                PackItem(new DaemonHelm());
        }

        public override int TreasureMapLevel { get { return 6; } }
        public override bool BardImmune { get { return true; } }
        public override int Meat { get { return 22; } }
        public override double WeaponAbilityChance { get { return 0.65; } }

        public override bool HasBreath { get { return true; } }
        public override int BreathEffectHue { get { return 1360; } }
        public override bool BreathEffectExplodes { get { return true; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.MedScrolls, 3);
            AddLoot(LootPack.HighScrolls, 3);
        }

        public Hijnoral( Serial serial )
            : base(serial)
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}