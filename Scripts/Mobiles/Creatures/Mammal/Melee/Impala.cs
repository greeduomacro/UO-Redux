using System;
using Server;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "an impala corpse" )]
	public class Impala : BaseCreature
	{
		[Constructable]
		public Impala() : base( AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.1, 0.2 )
		{
			Name = "an impala";
			Body = 0xEA;

			SetStr( 20, 30 );
			SetDex( 40, 60 );
			SetInt( 35, 55 );

			SetHits( 40, 60 );

			SetDamage( 2, 4 );

			SetResistance( ResistanceType.Physical, 10, 20 );
	
			VirtualArmor = Utility.RandomMinMax( 5, 15 );
		}

		public Impala( Serial serial ) : base( serial )
		{
		}

		public override HideType HideType { get{ return HideType.Regular; } }
		public override int Hides{ get{ return 12; } }
		public override int Meat{ get{ return Utility.RandomMinMax( 3, 5 ); } }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}