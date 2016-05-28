using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a bone demon corpse" )]
	public class BoneDemon : BaseCreature
	{
		[Constructable]
		public BoneDemon() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a bone demon";
			Body = 308;
			BaseSoundID = 0x48D;

			SetStr( 1000 );
			SetDex( 151, 175 );
			SetInt( 171, 220 );

			SetHits( 3600 );

			SetDamage( 34, 36 );

			SetDamageType( ResistanceType.Physical, 40, 65 );
			SetDamageType( ResistanceType.Cold, 40, 65 );

			SetResistance( ResistanceType.Physical, 60, 90 );
			SetResistance( ResistanceType.Fire, 55, 70 );
			SetResistance( ResistanceType.Cold, 80, 120 );
			SetResistance( ResistanceType.Poison, 80, 120 );
			SetResistance( ResistanceType.Energy, 60 );

			SetSkill( SkillName.DetectHidden, 80.0, 120 );
			SetSkill( SkillName.EvalInt, 77.6, 120.5 );
			SetSkill( SkillName.Magery, 77.6, 107.5 );
			SetSkill( SkillName.Meditation, 100.0, 120.0 );
			SetSkill( SkillName.MagicResist, 50.1, 95.0 );
			SetSkill( SkillName.Tactics, 120.0 );
			SetSkill( SkillName.Wrestling, 120.0 );

			Fame = 20000;
			Karma = -20000;

			VirtualArmor = 44;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 8 );
		}

		public override bool Unprovokable{ get{ return true; } }
		public override bool AreaPeaceImmune { get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 1; } }

		public BoneDemon( Serial serial ) : base( serial )
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