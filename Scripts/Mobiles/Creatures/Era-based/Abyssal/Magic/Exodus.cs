using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "Exodus corpse" )]
	public class Exodus : BaseCreature
	{
		[Constructable]
		public Exodus () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 102;
			Name = "Exodus";
			BaseSoundID = 357;

			SetStr( 700, 805 );
			SetDex( 100, 200 );
			SetInt( 305, 325 );
			SetSkill( SkillName.Wrestling, 80, 100 );
			SetSkill( SkillName.Tactics, 70, 90 );
			SetSkill( SkillName.MagicResist, 86, 95 );
			SetSkill( SkillName.Magery, 75, 120 );

			VirtualArmor = Utility.RandomMinMax( 52, 58 );
			SetFameLevel( 5 );
			SetKarmaLevel( 5 );

			switch ( Utility.Random( 8 ))
			{
				case 0: PackItem( new SpidersSilk( 3 ) ); break;
				case 1: PackItem( new BlackPearl( 3 ) ); break;
				case 2: PackItem( new Bloodmoss( 3 ) ); break;
				case 3: PackItem( new Garlic( 3 ) ); break;
				case 4: PackItem( new MandrakeRoot( 3 ) ); break;
				case 5: PackItem( new Nightshade( 3 ) ); break;
				case 6: PackItem( new SulfurousAsh( 3 ) ); break;
				case 7: PackItem( new Ginseng( 3 ) ); break;
			}
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 2 );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override int TreasureMapLevel{ get{ return 4; } }
		public override int Meat{ get{ return 1; } }
		public override bool AlwaysMurderer{ get{ return true; } }

		public Exodus( Serial serial ) : base( serial )
		{
		}

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
