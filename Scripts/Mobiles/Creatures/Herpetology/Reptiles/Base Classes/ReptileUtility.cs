using Server.Items;
using Server.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles.Creatures.Reptiles
{
    public enum ReptileType
    {
        Generic,
        Dragon,
        Ophidian,
        Serpent,
        Ancient //I think we can now argue that a creature's life impacts its DNA.
    }

    public enum EggType
    {
        Small,
        Medium,
        Large,
        Gigantic
    }

    public class ReptileUtility
    {
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
    }

    public class ReptileVenomSack : Item
    {
        public Poison m_PosionType;

        public ReptileVenomSack(Poison p)
        {
            m_PosionType = p;
        }

        public ReptileVenomSack( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
            Poison.Serialize(m_PosionType, writer);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
            Poison m_PoisonType = Poison.Deserialize(reader);
		}
    }

    public class ReptileEgg : Item
    {
        ReptileType m_Type;

        DateTime m_Creation;
        DateTime m_ToHatch;

        EggType m_EggType = EggType.Small;

        internal static int[] m_ItemIdBySize = new int[] { 0, 0, 0, 0 };

        public EggType Type
        {
            get { return m_EggType; }
        }

        public override double DefaultWeight
        {
            get { return 2.0; }
        }

        public override int Hue
        {
            get
            {
                return 1150;
            }
            set
            {
                base.Hue = value;
            }
        }

        public override string DefaultName
        {
            get
            {
                return "a large, pearl-like egg.";
            }
        } 
       
        [Constructable]
        public ReptileEgg() : base (3164)
        {
            m_Creation = DateTime.Now;
            m_ToHatch = DateTime.Now;
            m_Type = ReptileType.Generic;
        }

        [Constructable]
        public ReptileEgg(ReptileType rType) : base (3164)
        {
            m_Type = rType;
            m_Creation = DateTime.Now;
            m_ToHatch = m_Creation + TimeSpan.FromDays(3.0);
        }

        public ReptileEgg( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
            writer.Write((DateTime)m_ToHatch);
            writer.Write((DateTime)m_Creation);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
            m_ToHatch = reader.ReadDateTime();
            m_Creation = reader.ReadDateTime();
		}

        public override void OnDoubleClick(Mobile from)
        {
            if (DateTime.Now > m_Creation || from.AccessLevel == AccessLevel.Administrator)
            {
                if (from.Followers + 3 > from.FollowersMax)
                {
                    from.SendMessage("You have too many followers to hatch this.");
                    return;
                }

                EvolutionCreature ec = new EvolutionCreature();

                ec.ControlMaster = from;
                ec.Controlled = true;
                ec.IsBonded = true;
                ec.ControlOrder = OrderType.Follow;

                ec.MoveToWorld(from.Location, from.Map);

                BaseReptile.GenerateEffects(ec);

                this.Delete();
            }

            else
            {
                switch (Utility.RandomMinMax(0, 3))
                {
                    case 0: { from.SendMessage("The warmth of your touch seems to spur activity!"); break; }
                    case 1: { from.SendMessage("You touch the egg and it begins to move.."); break; }
                    case 2: { from.SendMessage("You place your hand on the egg and it begins to glow!"); break; }
                    case 3: { from.SendMessage("Something inside the egg seems to be stirring.."); break; }
                    default: break;
                }
            }
        }
    }
}