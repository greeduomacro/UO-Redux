using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a white wyrm corpse" )]
	public class WhiteWyrm : BaseCreature
	{
		[Constructable]
		public WhiteWyrm()
			: base( AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
            Name = NameList.RandomName("ancient lich") +",";
			Title = "the white wyrm";
			Body = 49;
			BaseSoundID = 362;

			SetStr( 721, 760 );
			SetDex( 101, 130 );
			SetInt( 386, 425 );

			SetHits( 1433, 1456 );

			SetDamage( 17, 25 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Cold, 50 );

			SetResistance( ResistanceType.Physical, 55, 70 );
			SetResistance( ResistanceType.Fire, 15, 25 );
			SetResistance( ResistanceType.Cold, 80, 90 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.EvalInt, 99.1, 100.0 );
			SetSkill( SkillName.Magery, 99.1, 100.0 );
			SetSkill( SkillName.MagicResist, 99.1, 100.0 );
			SetSkill( SkillName.Tactics, 97.6, 100.0 );
			SetSkill( SkillName.Wrestling, 90.1, 100.0 );

			Fame = 18000;
			Karma = 18000;

			VirtualArmor = 64;

			if( Utility.Random( 20 ) == 1 )
			{
				BodyValue = 59;
				Hue = 1153;
			}

            Tamable = true;
            ControlSlots = 4;
            MinTameSkill = 119.1;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Gems, Utility.Random( 1, 5 ) );
		}

		public override int TreasureMapLevel { get { return 4; } }
		public override int Meat { get { return 19; } }
		public override int Hides { get { return 20; } }
		public override HideType HideType { get { return HideType.Barbed; } }
		public override int Scales { get { return 9; } }
		public override ScaleType ScaleType { get { return ScaleType.White; } }
		public override FoodType FavoriteFood { get { return FoodType.Gold; } }
		public override bool CanAngerOnTame { get { return true; } }

		public WhiteWyrm( Serial serial )
			: base( serial )
		{
		}

		public virtual void BonusAction( PlayerMobile player, Mobile attacker )
		{
			if( attacker != null )
			{
				Effects.SendMovingEffect( this, attacker, 0x36D4, 5, 10, false, true, 1152, 0 );

				attacker.SendMessage( "You have been hit by a blast of ice!" );
				AOS.Damage( attacker, this, Utility.RandomMinMax( 20, 35 ), 0, 0, 100, 0, 0, false );
			}
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
