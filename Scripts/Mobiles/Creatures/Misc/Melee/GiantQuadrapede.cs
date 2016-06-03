using System;
using Server;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a quadrapedal corpse" )]
	public class GiantQuadrapede : BaseCreature
	{
		[Constructable]
		public GiantQuadrapede() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.1, 0.2 )
		{
			Name = "a giant quadrapede";
			BodyValue = 242;
			BaseSoundID = 0x1A1;

			SetStr( 20, 30 );
			SetDex( 30, 45 );
			SetInt( 10, 15 );

			SetHits( 30, 45 );

			SetDamage( 4, 7 );
			SetDamageType( ResistanceType.Physical, 10 );
			SetDamageType( ResistanceType.Poison, 90 );

			SetResistance( ResistanceType.Physical, 10, 20 );

			VirtualArmor = Utility.RandomMinMax( 5, 15 );
		}

		public GiantQuadrapede( Serial serial ) : base( serial )
		{
		}

		public override Poison HitPoison{ get{ return Poison.Regular; } }
		public override double HitPoisonChance{ get{ return 0.25; } }

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