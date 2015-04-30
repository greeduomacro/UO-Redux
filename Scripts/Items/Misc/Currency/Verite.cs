using System;
using Server;

namespace Server.Currency
{
	public class Verite : BaseCoin
	{
		public override CurrencyType CurrencyType { get { return CurrencyType.Verite; } }

		[Constructable]
		public Verite()
			: this( 1 )
		{
		}

		[Constructable]
		public Verite( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable]
		public Verite( int amount )
			: base( 0xEF0 )
		{
			Amount = amount;
            Hue = 2207;
            Name = "Verite";
		}

		public Verite( Serial serial )
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