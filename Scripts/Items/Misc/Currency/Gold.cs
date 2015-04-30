using System;
using Server;

namespace Server.Currency
{
	public class Gold : BaseCoin
	{
		public override CurrencyType CurrencyType { get { return CurrencyType.Gold; } }

		[Constructable]
		public Gold()
			: this( 1 )
		{
		}

		[Constructable]
		public Gold( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable]
		public Gold( int amount )
			: base( 3824 )
		{
			Amount = amount;
			Hue = 2213;
            Name = "gold";
		}

		public Gold( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}