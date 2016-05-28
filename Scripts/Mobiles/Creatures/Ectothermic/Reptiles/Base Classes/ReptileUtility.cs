using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles.Creatures.Reptiles
{
    static enum ReptileType
    {
        Generic,
        Dragon,
        Ophidian,
        Serpent,
    }

    static enum EggType
    {
        Small,
        Medium,
        Large,
        Gigantic
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

        public override double DefaultWeight
        {
            get { return 2.0; }
        }

        public ReptileEgg(ReptileType rType)
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
            if (DateTime.Now > m_Creation)
            {
                ReptileEvolutionCreature rec = new ReptileEvolutionCreature();

                rec.ControlMaster = from;
                rec.Controlled = true;
                rec.IsBonded = true;

                ((BaseReptile)rec).GenerateEffects(rec);

                from.SendMessage("The warmth of your touch seems to spur activity!");
            }

            else from.SendMessage("This egg is not yet ready to hatch..");
        }
    }
}