using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a giant beetle corpse" )]
	public class BoneBeetle : BaseMount
	{
		[Constructable]
		public BoneBeetle()
			: this( "a large, boney beetle" )
		{
		}

		[Constructable]
		public BoneBeetle( string name )
			: base( name, 0x317, 0x3E97, AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Hue = 942;

			SetStr( 300, 320 );
			SetDex( 80, 105 );
			SetInt( 35, 60 );

			SetHits( 150, 175 );

			SetDamage( 9, 12 );
			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Cold, 100 );

			SetResistance( ResistanceType.Physical, 45, 60 );
			SetResistance( ResistanceType.Fire, 25, 35 );
			SetResistance( ResistanceType.Cold, 30, 40 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 30, 35 );

			SetSkill( SkillName.Wrestling, 85, 95 );
			SetSkill( SkillName.Tactics, 85, 95 );
			SetSkill( SkillName.Anatomy, 50, 65 );
			SetSkill( SkillName.MagicResist, 70, 85 );

			Fame = 5000;
			Karma = -5000;

			VirtualArmor = 20;

			PackItem( new Bone() );
			PackItem( new Spine() );

			Tamable = true;
			ControlSlots = 2;
			MinTameSkill = 95.6;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Gems, 3 );
		}

		public BoneBeetle( Serial serial )
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
