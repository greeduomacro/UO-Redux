using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "the corpse of a d'haryop bruiser" )]
	public class DharyopBruiser : BaseCreature
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.CrushingBlow;
		}
		
		[Constructable]
		public DharyopBruiser() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a d'haryop bruiser";
			BodyValue = 769;
			Hue = 2219;
			
			SetStr( 350, 425 );
			SetDex( 280, 315 );
			SetInt( 55, 80 );
			
			SetHits( 575, 690 );
			
			SetDamage( 12, 17 );
			SetDamageType( ResistanceType.Physical, 30 );
			SetDamageType( ResistanceType.Fire, 25 );
			SetDamageType( ResistanceType.Energy, 45 );
			
			SetResistance( ResistanceType.Physical, 55, 70 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 30, 45 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 55, 65 );
			
			SetSkill( SkillName.Wrestling, 115, 125 );
			SetSkill( SkillName.Tactics, 105, 115 );
			SetSkill( SkillName.Anatomy, 95, 110 );
			SetSkill( SkillName.MagicResist, 100, 120 );
			
			Fame = 13000;
			Karma = 0;
			
			VirtualArmor = 70;
			
			Tamable = true;
			ControlSlots = 3;
			MinTameSkill = 99.7;
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Gems, Utility.RandomMinMax( 2, 8 ) );
		}
		
		public override bool SubdueBeforeTame{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 3; } }
		public override int Meat{ get{ return 9; } }
		public override double WeaponAbilityChance{ get{ return 0.50; } }
		
		public override int GetAngerSound()
		{
			return 699;
		}
		
		public override int GetAttackSound()
		{
			return 706;
		}
		
		public override int GetIdleSound()
		{
			return 1240;
		}
		
		public override int GetHurtSound()
		{
			return 606;
		}
		
		public override int GetDeathSound()
		{
			return 767;
		}
		
		public DharyopBruiser( Serial serial ) : base( serial )
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
