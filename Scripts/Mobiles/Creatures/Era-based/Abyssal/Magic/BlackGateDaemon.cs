using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a charred daemon corpse" )]
	public class BlackGateDaemon : BaseCreature
	{
		[Constructable]
		public BlackGateDaemon()
			: base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a black gate daemon";
			Body = 38;
			BaseSoundID = 357;

			SetStr( 986, 1185 );
			SetDex( 177, 255 );
			SetInt( 251, 350 );

			SetHits( 1892, 2111 );

			SetDamage( 32, 45 );

			SetDamageType( ResistanceType.Physical, 75 );
			SetDamageType( ResistanceType.Fire, 65 );
			SetDamageType( ResistanceType.Energy, 65 );

			SetResistance( ResistanceType.Physical, 80, 90 );
			SetResistance( ResistanceType.Fire, 80, 90 );
			SetResistance( ResistanceType.Cold, 70, 80 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 75, 95 );

			SetSkill( SkillName.Anatomy, 90.1, 96.0 );
			SetSkill( SkillName.EvalInt, 95.1, 100.0 );
			SetSkill( SkillName.Magery, 95.5, 120.0 );
			SetSkill( SkillName.Meditation, 65.1, 85.0 );
			SetSkill( SkillName.MagicResist, 100.5, 150.0 );
			SetSkill( SkillName.Tactics, 95.1, 100.0 );
			SetSkill( SkillName.Wrestling, 95.1, 120.0 );

			Fame = 24000;
			Karma = -24000;

			VirtualArmor = 90;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.SuperBoss );
			AddLoot( LootPack.UltraRich );
			AddLoot( LootPack.Gems, 8 );
			AddLoot( LootPack.MedScrolls, 3 );
			AddLoot( LootPack.HighScrolls, 3 );
		}


		public override bool CanRummageCorpses { get { return true; } }
		public override bool BardImmune { get { return true; } }
		public override bool AutoDispel { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Deadly; } }
		public override int TreasureMapLevel { get { return 5; } }
		public override int Meat { get { return 1; } }

		public BlackGateDaemon( Serial serial )
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