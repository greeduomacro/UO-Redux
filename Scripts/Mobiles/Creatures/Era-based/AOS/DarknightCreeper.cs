using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a darknight creeper corpse" )]
	public class DarknightCreeper : Champion
	{
		[Constructable]
		public DarknightCreeper()
			: base( AIType.AI_Mage, FightMode.Closest)
		{
			Name = NameList.RandomName( "darknight creeper" );
			Body = 313;
			BaseSoundID = 0xE0;

			SetStr( 301, 330 );
			SetDex( 101, 110 );
			SetInt( 3010, 3300 );

			SetHits( 40000 );

			SetDamage( 22, 26 );

			SetDamageType( ResistanceType.Physical, 85, 95 );
			SetDamageType( ResistanceType.Poison, 5, 15 );

			SetResistance( ResistanceType.Physical, 60, 78 );
			SetResistance( ResistanceType.Fire, 60, 86 );
			SetResistance( ResistanceType.Cold, 100, 120 );
			SetResistance( ResistanceType.Poison, 90, 111 );
			SetResistance( ResistanceType.Energy, 75, 80 );

			SetSkill( SkillName.DetectHidden, 80.0, 120 );
			SetSkill( SkillName.EvalInt, 118.1, 180.0 );
			SetSkill( SkillName.Magery, 112.6, 160.0 );
			SetSkill( SkillName.Meditation, 150.0, 225.0 );
			SetSkill( SkillName.Poisoning, 120.0, 135.0 );
			SetSkill( SkillName.MagicResist, 90.1, 190.9 );
			SetSkill( SkillName.Tactics, 100.0, 110.0 );
			SetSkill( SkillName.Wrestling, 90.1, 190.9 );

			Fame = 22000;
			Karma = -22000;

			VirtualArmor = 34;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 2 );
		}

		public override bool BardImmune { get { return !Core.SE; } }
		public override bool Unprovokable { get { return Core.SE; } }
		public override bool AreaPeaceImmune { get { return Core.SE; } }
		public override bool BleedImmune { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }
		public override Poison HitPoison { get { return Poison.Lethal; } }

		public override int TreasureMapLevel { get { return 1; } }

		public DarknightCreeper( Serial serial ) : base( serial )
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

			if( BaseSoundID == 471 )
				BaseSoundID = 0xE0;
		}
	}
}