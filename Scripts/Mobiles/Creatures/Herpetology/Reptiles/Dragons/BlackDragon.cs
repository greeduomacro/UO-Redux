using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles.Creatures.Reptiles
{
	[CorpseName( "the remains of a black dragon" )]
	public class BlackDragon : BaseDragon
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.BleedAttack;
		}

		[Constructable]
		public BlackDragon()
			: base( AIType.AI_Mage, FightMode.Weakest, 12, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName("ancient lich") + ",";
			Title = "the black dragon";
			Hue = Utility.RandomList( 1175, 1109 );
			BodyValue = 12;
			BaseSoundID = 0x16A;

			SetStr( 900, 1000 );
			SetDex( 400, 500 );
			SetInt( 350, 500 );

			SetHits( 1700, 1750 );

			SetDamage( 18, 22 );
			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Cold, 50 );

			SetResistance( ResistanceType.Physical, 60, 70 );
			SetResistance( ResistanceType.Fire, 70, 85 );
			SetResistance( ResistanceType.Cold, 80, 100 );
			SetResistance( ResistanceType.Poison, 50, 65 );
			SetResistance( ResistanceType.Energy, 50, 60 );

			SetSkill( SkillName.Magery, 50, 65 );
			SetSkill( SkillName.EvalInt, 50, 65 );
			SetSkill( SkillName.MagicResist, 55, 65 );
			SetSkill( SkillName.Tactics, 75, 80 );
			SetSkill( SkillName.Wrestling, 60, 70 );

			Fame = 10000;
			Karma = -10000;

			VirtualArmor = 50;
		}

		public override int TreasureMapLevel { get { return 5; } }
		public override double WeaponAbilityChance { get { return 0.6; } }
		public override int Meat { get { return Utility.RandomMinMax( 10, 20 ); } }
		public override int Hides { get { return Utility.RandomMinMax( 5, 15 ); } }
		public override HideType HideType { get { return Utility.RandomBool() ? HideType.Barbed : HideType.Horned; } }
		public override int Scales { get { return Utility.RandomMinMax( 10, 20 ); } }
		public override ScaleType ScaleType { get { return ScaleType.Black; } }

		public override bool HasBreath { get { return true; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 2 );
			AddLoot( LootPack.Gems, 5 );
		}

		public BlackDragon( Serial serial )
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
