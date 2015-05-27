using System;
using Server;

namespace Server.Items
{
	public class WellAddon : BaseAddon, IWaterSource
	{
		public int Quantity
		{
			get { return 500; }
			set { }
		}

		[Constructable]
		public WellAddon()
		{
			// roof
			AddComponent( new AddonComponent( 1442 ), 0, 0, 12 );
			AddComponent( new AddonComponent( 1442 ), -1, 0, 12 );
			AddComponent( new AddonComponent( 1443 ), 0, -1, 12 );
			AddComponent( new AddonComponent( 1443 ), -1, -1, 12 );
			// post
			AddComponent( new AddonComponent( 19 ), -1, 0, 8 );
			// bucket
			//AddComponent( new AddonComponent( 4090 ), 0, 0, 6 );
			AddComponent( new WellBucket(), 0, 0, 6 );
			// well walls
			AddComponent( new AddonComponent( 36 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 38 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 37 ), 0, -2, 0 );
			AddComponent( new AddonComponent( 37 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 37 ), -1, -2, 0 );
			AddComponent( new AddonComponent( 38 ), -2, 0, 0 );
			AddComponent( new AddonComponent( 38 ), -2, -1, 0 );
			AddComponent( new AddonComponent( 39 ), -2, -2, 0 );
			// water
			AddComponent( new AddonComponent( 6039 ), -1, -1, 0 );
		}

		public WellAddon( Serial serial )
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

	public class WellBucket : AddonComponent
	{
		[Constructable]
		public WellBucket()
			: this( 4090 )
		{
		}

		[Constructable]
		public WellBucket( int itemid )
			: base( itemid )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if( from.InRange( this.GetWorldLocation(), 2 ) )
			{
				if( from.Thirst >= 20 )
				{
					from.SendMessage( "You are not thirsty at all." );
				}
				else
				{
					string msg = null;

					switch( Utility.RandomMinMax( 1, 5 ) )
					{
						case 1: msg = "The cool well water is refreshing."; break;
						case 2: msg = "The warm well water is refreshing."; break;
						case 3: msg = "You drink deeply from the clean water."; break;
						case 4: msg = "The calm well water is refreshing."; break;
						case 5: msg = "You drink your fill from the well."; break;
					}

					from.SendMessage( msg );

					from.Thirst += 5;
				}
			}
			else
			{
				from.SendMessage( "Get closer." );
			}
		}

		public WellBucket( Serial serial )
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
