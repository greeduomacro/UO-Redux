using System;
using Server;
using Server.Mobiles;

namespace Server.Mobiles
{
	public class FrostSerpent : BaseCreature
	{
		[Constructable]
		public FrostSerpent()
			: base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a frost serpent";
			Hue = 1150;
			BaseSoundID = 219;

			if( Utility.Random( 5 ) == 1 )
			{
				BodyValue = 0x15;

				SetStr( 185, 200 );
				SetDex( 50, 85 );
				SetInt( 25, 35 );

				SetHits( 170, 200 );

				SetDamage( 5, 12 );
			}
			else
			{
				BodyValue = 0x34;

				SetStr( 25, 40 );
				SetDex( 15, 25 );
				SetInt( 5, 10 );

				SetHits( 30, 40 );

				SetDamage( 2, 6 );
			}

			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Cold, 90 );
			SetDamageType( ResistanceType.Poison, 10 );

			SetResistance( ResistanceType.Physical, 15, 30 );
			SetResistance( ResistanceType.Fire, 0, 5 );
			SetResistance( ResistanceType.Cold, 20, 35 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 0, 10 );

			SetSkill( SkillName.Wrestling, 15, 30 );
			SetSkill( SkillName.Tactics, 30, 45 );

			Fame = 250;
			Karma = -250;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
		}

		public override Poison HitPoison { get { return Poison.Regular; } }
		public override double HitPoisonChance { get { return 0.10; } }

		public FrostSerpent( Serial serial )
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
