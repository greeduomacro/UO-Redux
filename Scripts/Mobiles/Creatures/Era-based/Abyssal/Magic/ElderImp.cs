using System;
using Server;

namespace Server.Mobiles
{
	[CorpseName( "an imp corpse" )]
	public class ElderImp : BaseCreature
	{
		[Constructable]
		public ElderImp()
			: base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an elder imp";
			Body = 74;
			BaseSoundID = 422;
			Hue = 1209;

			SetStr( 191, 315 );
			SetDex( 61, 80 );
			SetInt( 186, 305 );

			SetHits( 155, 270 );

			SetDamage( 14, 18 );

			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Fire, 50 );
			SetDamageType( ResistanceType.Poison, 50 );

			SetResistance( ResistanceType.Physical, 25, 35 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.EvalInt, 70.1, 75.0 );
			SetSkill( SkillName.Magery, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 90.1, 120.0 );
			SetSkill( SkillName.Tactics, 42.1, 50.0 );
			SetSkill( SkillName.Wrestling, 90.1, 94.0 );

			Fame = 2500;
			Karma = -2500;

			VirtualArmor = 80;

			Tamable = true;
			ControlSlots = 2;
			MinTameSkill = 93.1;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average, 2 );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override int Meat { get { return 1; } }
		public override int Hides { get { return 6; } }
		public override HideType HideType { get { return HideType.Spined; } }
		public override FoodType FavoriteFood { get { return FoodType.Meat; } }
		public override PackInstinct PackInstinct { get { return PackInstinct.Daemon; } }

		public ElderImp( Serial serial )
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