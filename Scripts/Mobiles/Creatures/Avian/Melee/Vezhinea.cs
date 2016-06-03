using System;
using Server;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "the feathered corpse of a vezhinea" )]
	public class Vezhinea : BaseMount
	{
		[Constructable]
		public Vezhinea() : this( "a vezhinea" )
		{
		}
		
		[Constructable]
		public Vezhinea( string name ) : base( name, 243, 16020, AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a vezhinea";
			
			SetStr( 1000, 1150 );
			SetDex( 170, 270 );
			SetInt( 300, 325 );
			
			SetHits( 900, 1100 );
			
			SetDamage( 23, 26 );
			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Energy, 100 );
			
			SetResistance( ResistanceType.Physical, 55, 70 );
			SetResistance( ResistanceType.Fire, 60, 75 );
			SetResistance( ResistanceType.Cold, 55, 70 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 90, 100 );
			
			SetSkill( SkillName.Wrestling, 100, 120 );
			SetSkill( SkillName.Tactics, 100, 110 );
			SetSkill( SkillName.Anatomy, 75, 85 );
			SetSkill( SkillName.MagicResist, 85, 100 );
			
			Fame = 15000;
			Karma = -15000;
			
			VirtualArmor = 90;
			
			Tamable = true;
			ControlSlots = 4;
			MinTameSkill = 99.8;
		}
		
		public override bool SubdueBeforeTame{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 6; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		public override bool BardImmune { get { return true; } }
		public override bool CanAngerOnTame { get { return true; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich );
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Gems, 4 );
		}

		public override void AlterMeleeDamageFrom( Mobile from, ref int damage )
		{
			if( from is BaseCreature && ((BaseCreature)from).Controlled )
				damage = 0;
		}

		
		public override int GetAngerSound()
		{
			return 1279;
		}
		
		public override int GetAttackSound()
		{
			return 1278;
		}
		
		public override int GetHurtSound()
		{
			return 1276;
		}
		
		public override int GetIdleSound()
		{
			return 1277;
		}
		
		public override int GetDeathSound()
		{
			return 1275;
		}
		
		public Vezhinea( Serial serial ) : base( serial )
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
