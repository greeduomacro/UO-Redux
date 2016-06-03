using System;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "the corpse of a d'haryop wizard" )]
	public class DharyopWizard : BaseCreature
	{
		[Constructable]
		public DharyopWizard()
			: base( AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a d'haryop wizard";
			BodyValue = 784;
			Hue = 1130;

			SetStr( 350, 400 );
			SetDex( 180, 210 );
			SetInt( 540, 575 );

			SetHits( 700, 765 );

			SetDamage( 8, 12 );
			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Fire, 35 );
			SetDamageType( ResistanceType.Energy, 65 );

			SetResistance( ResistanceType.Physical, 45, 60 );
			SetResistance( ResistanceType.Fire, 50, 65 );
			SetResistance( ResistanceType.Cold, 40, 45 );
			SetResistance( ResistanceType.Poison, 40, 55 );
			SetResistance( ResistanceType.Energy, 70, 85 );

			SetSkill( SkillName.Wrestling, 75, 90 );
			SetSkill( SkillName.Tactics, 85, 100 );
			SetSkill( SkillName.Anatomy, 25, 45 );
			SetSkill( SkillName.MagicResist, 115, 130 );
			SetSkill( SkillName.Meditation, 95, 105 );
			SetSkill( SkillName.Magery, 100, 110 );
			SetSkill( SkillName.EvalInt, 105, 115 );

			Fame = 13000;
			Karma = 0;

			VirtualArmor = 65;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Average );
			AddLoot( LootPack.LowScrolls, 4 );
			AddLoot( LootPack.MedScrolls, 3 );
			AddLoot( LootPack.HighScrolls, 2 );
			AddLoot( LootPack.Gems, Utility.RandomMinMax( 6, 10 ) );
		}

		public override int TreasureMapLevel { get { return 6; } }
		public override int Meat { get { return 11; } }

		public override int GetAngerSound()
		{
			return 1278;
		}

		public override int GetAttackSound()
		{
			return 1295;
		}

		public override int GetIdleSound()
		{
			return 1240;
		}

		public override int GetHurtSound()
		{
			return 761;
		}

		public override int GetDeathSound()
		{
			return 843;
		}

		public DharyopWizard( Serial serial )
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
