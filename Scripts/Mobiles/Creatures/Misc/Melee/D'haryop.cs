using System;
using Server;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "the corpse of a d'haryop" )]
	public class Dharyop : BaseCreature
	{
		[Constructable]
		public Dharyop() : base( AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a d'haryop";
			BodyValue = 771;
			Hue = 1146;
			
			SetStr( 300, 395 );
			SetDex( 200, 250 );
			SetInt( 140, 180 );
			
			SetHits( 550, 625 );
			
			SetDamage( 9, 14 );
			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Fire, 45 );
			SetDamageType( ResistanceType.Energy, 55 );
			
			SetResistance( ResistanceType.Physical, 35, 40 );
			SetResistance( ResistanceType.Fire, 45, 55 );
			SetResistance( ResistanceType.Cold, 25, 35 );
			SetResistance( ResistanceType.Poison, 25, 35 );
			SetResistance( ResistanceType.Energy, 50, 65 );
			
			SetSkill( SkillName.Wrestling, 110, 120 );
			SetSkill( SkillName.Tactics, 100, 115 );
			SetSkill( SkillName.Anatomy, 65, 80 );
			SetSkill( SkillName.MagicResist, 85, 105 );
			
			Fame = 13000;
			Karma = 0;
			
			VirtualArmor = 55;
			
			Tamable = true;
			ControlSlots = 3;
			MinTameSkill = 99.1;
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Gems, Utility.RandomMinMax( 9, 14 ) );
		}
		
		public override int TreasureMapLevel{ get{ return 6; } }
		public override int Meat{ get{ return 9; } }
		
		public override int GetAngerSound()
		{
			return 1241;
		}
		
		public override int GetAttackSound()
		{
			return 1242;
		}
		
		public override int GetIdleSound()
		{
			return 1240;
		}
		
		public override int GetHurtSound()
		{
			return 1239;
		}
		
		public override int GetDeathSound()
		{
			return 1238;
		}
		
		public Dharyop( Serial serial ) : base( serial )
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
