using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class CannonBall : Item
	{
		[Constructable]
		public CannonBall()
			: base( 0xE73 )
		{
			Name = "a cannon ball";
			Weight = 10.0;
		}

		public CannonBall( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
