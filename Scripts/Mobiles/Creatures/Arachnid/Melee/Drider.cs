using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a drider's corpse" )]
	public class Drider : BaseCreature
	{
		[Constructable]
		public Drider() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a drider";
			Body = 71;
			Hue = 1109;
			BaseSoundID = 0x252;

			SetStr( 150, 175 );
			SetDex( 77, 95 );
			SetInt( 126, 150 );

			SetHits( 100, 150 );
			SetMana( 46, 70 );

			SetDamage( 10, 12 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Poison, 50 );

			SetResistance( ResistanceType.Physical, 40, 50 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 35, 45 );
			SetResistance( ResistanceType.Poison, 80, 85 );
			SetResistance( ResistanceType.Energy, 35, 45 );

			SetSkill( SkillName.Poisoning, 70, 80 );
			SetSkill( SkillName.MagicResist, 55, 65 );
			SetSkill( SkillName.Tactics, 95, 100 );
			SetSkill( SkillName.Wrestling, 100, 110 );
			SetSkill( SkillName.Anatomy, 80, 90 );

			Fame = 15000;
			Karma = -15000;

			VirtualArmor = 50;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 2 );
		}

		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		public override Poison HitPoison{ get{ return Poison.Lesser; } }
		public override int TreasureMapLevel{ get{ return 3; } }
		public override int Meat{ get{ return 2; } }

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.TerathansAndOphidians; }
		}

		public Drider( Serial serial ) : base( serial )
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
