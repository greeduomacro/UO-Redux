using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a slimey slug body" )]
	public class TormentedSlug : BaseCreature
	{
		[Constructable]
		public TormentedSlug()
			: base( AIType.AI_Melee, FightMode.Closest, 8, 1, 0.4, 0.5 )
		{
			Body = 0x100;
			Hue = Utility.RandomSlimeHue();
			Name = "a tormented slug";

			SetDex( 30, 50 );
			SetInt( 20, 30 );
			SetStr( 400, 600 );

			SetHits( 350, 650 );

			SetDamage( 7, 12 );
			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Cold, 30, 45 );
			SetResistance( ResistanceType.Energy, 20, 25 );
			SetResistance( ResistanceType.Fire, 20, 25 );
			SetResistance( ResistanceType.Physical, 35, 55 );
			SetResistance( ResistanceType.Poison, 40, 50 );

			SetSkill( SkillName.MagicResist, 40.0, 55.0 );
			SetSkill( SkillName.Tactics, 50.0, 75.0 );
			SetSkill( SkillName.Wrestling, 50.0, 65.0 );

			Fame = 4500;
			Karma = -4000;

			VirtualArmor = 25;

			PackItem( new Bone( Utility.RandomMinMax( 1, 9 ) ) );
			PackItem( new DecoItem() );
			PackItem( new DecoItem() );
			PackItem( new DecoItem() );
		}

		public TormentedSlug( Serial serial )
			: base( serial )
		{
		}

		public override int GetAngerSound() { return 1394; }
		public override int GetDeathSound() { return 1391; }
		public override int GetHurtSound() { return 1396; }
		public override int GetIdleSound() { return 1393; }

		public override bool AlwaysMurderer { get { return true; } }
		public override int TreasureMapLevel { get { return 4; } }

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