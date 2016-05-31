using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles.Creatures.Reptiles
{
	[CorpseName( "the remains of an iron dragon" )]
	public class IronDragon : BaseDragon
	{
		[Constructable]
		public IronDragon()
			: base( AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName("ancient lich") + ",";
			Title = "the iron dragon";
			BodyValue = 12;
			Hue = 945;
			BaseSoundID = 0x16A;

			SetStr( 750, 850 );
			SetDex( 100, 125 );
			SetInt( 350, 450 );

			SetHits( 650, 700 );

			SetDamage( 18, 22 );
			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Cold, 50 );

			SetResistance( ResistanceType.Physical, 95, 100 );
			SetResistance( ResistanceType.Fire, 65, 75 );
			SetResistance( ResistanceType.Cold, 65, 75 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 50, 60 );

			SetSkill( SkillName.Tactics, 90, 105 );
			SetSkill( SkillName.Wrestling, 100, 110 );
			SetSkill( SkillName.Anatomy, 50, 60 );
			SetSkill( SkillName.MagicResist, 75, 85 );

			Fame = 15000;
			Karma = 6500;

			VirtualArmor = 50;

			PackItem( new IronOre( Utility.RandomMinMax( 40, 60 ) ) );
			PackItem( new IronIngot( Utility.RandomMinMax( 35, 50 ) ) );
		}

		public IronDragon( Serial serial )
			: base( serial )
		{
		}

		public override Poison PoisonImmune { get { return Poison.Lethal; } }
		public override bool Uncalmable { get { return true; } }
		public override bool IsScaryToPets { get { return true; } }
		public override int Meat { get { return Utility.RandomMinMax( 10, 15 ); } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Gems, 10 );
		}

		public virtual void BonusAction( PlayerMobile player, Mobile attacker )
		{
			if( attacker != null )
			{
				this.Animate( 9, 1, 1, true, false, 0 );

				this.Say( "*its tail whips the ground hard, sending a boulder through the air*" );
				Effects.SendMovingEffect( this, attacker, 0x11B6, 8, 0, false, false, 0, 0 );

				int dmg = (int)(attacker.Hits * 0.33);

				attacker.Damage( dmg, this );
				attacker.SendMessage( "You have been hit by a boulder!" );
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
