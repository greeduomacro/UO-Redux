using System;
using Server;

namespace Server.Mobiles
{
	[CorpseName( "an alimarchus corpse" )]
	public class Alimarchus : BaseMount
	{
		[Constructable]
		public Alimarchus()
			: this( "an alimarchus" )
		{
		}

		[Constructable]
		public Alimarchus( string name )
			: base( name, 0xE4, 0x3EA1, AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.1, 0.2 )
		{
			BaseSoundID = 0xA8;
			Hue = Utility.RandomList( 447, 842, 1058 );

			SetStr( 220, 255 );
			SetDex( 650, 675 );
			SetInt( 65, 80 );

			SetHits( 350, 410 );

			SetDamage( 6, 9 );
			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Energy, 100 );

			SetResistance( ResistanceType.Physical, 40, 50 );
			SetResistance( ResistanceType.Energy, 75, 80 );
			SetResistance( ResistanceType.Cold, 30, 35 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Fire, 35, 40 );

			SetSkill( SkillName.Anatomy, 65, 75 );
			SetSkill( SkillName.Tactics, 80, 85 );
			SetSkill( SkillName.Wrestling, 80, 90 );
			SetSkill( SkillName.MagicResist, 75, 85 );

			Fame = 3500;
			Karma = 0;

			Tamable = true;
			ControlSlots = 2;
			MinTameSkill = 98.7;
		}

		public override int Meat { get { return 6; } }
		public override int Hides { get { return 10; } }
		public override FoodType FavoriteFood { get { return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public Alimarchus( Serial serial )
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
