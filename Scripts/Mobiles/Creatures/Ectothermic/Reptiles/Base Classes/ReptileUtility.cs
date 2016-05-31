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

        EggType m_EggType = EggType.Small;
        ReptileType m_ParentType = ReptileType.Generic;

        internal static int[] m_ItemIdBySize = new int[] { 0, 0, 0, 0 };

        public EggType Type
        {
            get { return m_EggType; }
        }

        public override double DefaultWeight
        {
            get { return 2.0; }
        }

       
        public ReptileEgg()
        {
            m_Creation = DateTime.Now;
            m_ToHatch = DateTime.Now;
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

                switch (Utility.RandomMinMax(0, 3))
                {
                    case 0: { from.SendMessage("The warmth of your touch seems to spur activity!"); break; }
                    case 1: { from.SendMessage("You touch the egg and it begins to move.."); break; }
                    case 2: { from.SendMessage("You place your hand on the egg and it begins to glow!"); break; }
                    case 3: { from.SendMessage("Something inside the egg seems to be stirring.."); break; }
                    default: break;
                }
                
            }

            else from.SendMessage("This egg is not yet ready to hatch..");
        }
    }
}