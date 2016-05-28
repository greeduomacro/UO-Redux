using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("the corpse of an eryon")]
    public class Eryon : BaseCreature
    {
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.ArmorIgnore;
        }

        [Constructable]
        public Eryon()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.1, 0.3)
        {
            Name = "an eryon";
            BodyValue = 248;

            SetStr(650, 750);
            SetDex(70, 80);
            SetInt(60, 90);

            SetHits(650, 770);

            SetDamage(5, 10);
            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 20, 35);
            SetResistance(ResistanceType.Energy, 45, 60);

            SetSkill(SkillName.Wrestling, 90, 100);
            SetSkill(SkillName.Tactics, 90, 100);
            SetSkill(SkillName.Anatomy, 85, 95);
            SetSkill(SkillName.MagicResist, 90, 110);

            Fame = 10000;
            Karma = -10000;

            VirtualArmor = 55;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 99.1;
        }

        public override int TreasureMapLevel { get { return 4; } }
        public override double WeaponAbilityChance { get { return 0.50; } }
        public override bool SubdueBeforeTame
        {
            get
            {
                return true;
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Gems, 5);
        }

        public override void OnDamagedBySpell( Mobile from )
        {
            base.OnDamagedBySpell(from);

            if( 0.10 >= Utility.RandomDouble() )
            {
                ExplodeHorn(from);
            }
        }

        private void ExplodeHorn( Mobile target )
        {
            target.SendMessage("One of the eryon's horns seems to glow, erupting violently in a flash!");

            this.FixedEffect(0x36B0, 10, 1);
            target.BoltEffect(0);
            target.BoltEffect(0);

            target.Freeze(TimeSpan.FromSeconds(5));

            this.Direction = this.GetDirectionTo(target.Location);
            this.DoHarmful(target, true);
            target.Damage(Utility.RandomMinMax(15, 45), this);

            Item hornShard = new Item(0x9D1); //grape bunch itemID
            hornShard.Hue = 1109;
            hornShard.Name = "a piece of eryon horn";
            hornShard.MoveToWorld(target.Location, target.Map);
        }

        public override int GetAngerSound()
        {
            return 1274;
        }

        public override int GetAttackSound()
        {
            return 1273;
        }

        public override int GetIdleSound()
        {
            return 1271;
        }

        public override int GetHurtSound()
        {
            return 1270;
        }

        public override int GetDeathSound()
        {
            return 1269;
        }

        public Eryon( Serial serial )
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
