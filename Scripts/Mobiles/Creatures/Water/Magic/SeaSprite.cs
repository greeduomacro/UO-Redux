using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
    [CorpseName("a sea sprite corpse")]
    public class SeaSprite : BaseCreature, IIlluminatingObject
    {
        public override InhumanSpeech SpeechType { get { return InhumanSpeech.Wisp; } }

        public bool IsIlluminating { get { return this.Alive; } }

        [Constructable]
        public SeaSprite()
            : base(AIType.AI_Mage, FightMode.Evil, 10, 1, 0.2, 0.4)
        {
            Name = "a sea sprite";
            Body = 58;
            BaseSoundID = 466;
            Hue = Utility.RandomBlueHue();
            CanSwim = true;

            SetStr(196, 225);
            SetDex(196, 225);
            SetInt(196, 225);

            SetHits(200, 300);

            SetDamage(17, 18);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 20, 40);
            SetResistance(ResistanceType.Cold, 10, 30);
            SetResistance(ResistanceType.Poison, 5, 10);
            SetResistance(ResistanceType.Energy, 50, 70);

            SetSkill(SkillName.EvalInt, 80.0, 95);
            SetSkill(SkillName.Magery, 80.0, 95);
            SetSkill(SkillName.MagicResist, 90, 110);
            SetSkill(SkillName.Tactics, 80.0);
            SetSkill(SkillName.Wrestling, 80.0);

            Fame = 6000;
            Karma = 6000;

            VirtualArmor = 40;

            AddItem(new LightSource());

            if( Utility.Random(4) == 1 )
                PackItem(new Rope());

            switch( Utility.Random(5) )
            {
                case 0:
                    {
                        BodyValue = 58;
                        break;
                    }
                case 1:
                    {
                        BodyValue = 128;
                        break;
                    }
                case 2:
                    {
                        BodyValue = 150;
                        break;
                    }
                case 3:
                    {
                        BodyValue = 13;
                        break;
                    }
                case 4:
                    {
                        BodyValue = 300;
                        break;
                    }
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);
        }

        public SeaSprite( Serial serial )
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
