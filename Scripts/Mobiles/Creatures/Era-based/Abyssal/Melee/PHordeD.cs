using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a psychedelic horde daemons corpse" )]
	public class PHordeD : BaseCreature
	{
		[Constructable]
		public PHordeD() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 999;
			Name = "a psychedelic horde daemon";
			Kills = 20;

			SetStr( 300, 400 );
			SetDex( 100, 150 );
			SetInt( 50, 75 );
			SetSkill( SkillName.Wrestling, 80, 90 );
			SetSkill( SkillName.Tactics, 76, 90 );
			SetSkill( SkillName.MagicResist, 76, 90 );

			VirtualArmor = 70;
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

			PackScroll( 2, 8 );
		}

		public PHordeD( Serial serial ) : base( serial )
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
