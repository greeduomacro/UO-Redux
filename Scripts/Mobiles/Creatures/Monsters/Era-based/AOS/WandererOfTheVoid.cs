using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a wanderer of the void corpse" )]
	public class WandererOfTheVoid : BaseCreature
	{
		[Constructable]
		public WandererOfTheVoid()
			: base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a wanderer of the void";
			Body = 316;
			BaseSoundID = 377;

			SetStr( 111, 200 );
			SetDex( 101, 125 );
			SetInt( 1301, 1390 );

			SetHits( 351, 400 );

			SetDamage( 11, 13 );

			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Cold, 15 );
			SetDamageType( ResistanceType.Energy, 85 );

			SetResistance( ResistanceType.Physical, 70, 80 );
			SetResistance( ResistanceType.Fire, 25, 35 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 70, 75 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.EvalInt, 260.1, 370.0 );
			SetSkill( SkillName.Magery, 160.1, 170.0 );
			SetSkill( SkillName.Meditation, 160.1, 170.0 );
			SetSkill( SkillName.MagicResist, 150.1, 175.0 );
			SetSkill( SkillName.Tactics, 60.1, 70.0 );
			SetSkill( SkillName.Wrestling, 160.1, 170.0 );

			Fame = 20000;
			Karma = -20000;

			VirtualArmor = 44;

            Tamable = true;
            MinTameSkill = 119.9;
            ControlSlots = 4;
		}

		public override bool BleedImmune { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }
		public override int TreasureMapLevel { get { return Core.AOS ? 4 : 1; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
		}

		public WandererOfTheVoid( Serial serial )
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