using System;
using Server;

namespace Server.Currency
{
	public class Valorite : BaseCoin
	{
		public override CurrencyType CurrencyType { get { return CurrencyType.Valorite; } }

		[Constructable]
		public Valorite()
			: this( 1 )
		{
		}

		[Constructable]
		public Valorite( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable]
		public Valorite( int amount )
			: base( 3824 )
		{
            Hue = 2219;
			Amount = amount;
            Name = "valorite";
		}

		public Valorite( Serial serial )
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