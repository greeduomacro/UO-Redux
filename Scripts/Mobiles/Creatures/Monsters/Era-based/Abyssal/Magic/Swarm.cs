using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a corpse" )]
	public class Swarm : BaseCreature
	{
		[Constructable]
		public Swarm () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 778;
			Name = "a swarm";
			BaseSoundID = 377;
			Kills = 20;

			SetStr( 298, 325 );
			SetDex( 86, 101 );
			SetInt( 301, 364 );
			SetSkill( SkillName.Wrestling, 85, 97 );
			SetSkill( SkillName.Tactics, 82, 97 );
			SetSkill( SkillName.MagicResist, 118, 128 );
			SetSkill( SkillName.Magery, 92, 99 );
			SetSkill( SkillName.EvalInt, 25, 33 );

			VirtualArmor = 50;
			SetFameLevel( 5 );
			SetKarmaLevel( 5 );

			PackGem();
			PackGem();

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
			PackArmor( 2, 5 );
			PackWeapon( 3, 5 );
			PackWeapon( 5, 5 );
		}

		public Swarm( Serial serial ) : base( serial )
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