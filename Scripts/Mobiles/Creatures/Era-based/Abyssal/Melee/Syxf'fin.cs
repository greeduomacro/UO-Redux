using System;
using Server;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a spinly corpse" )]
	public class Syxffin : BaseCreature
	{
		[Constructable]
		public Syxffin() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a syxf'fin";
			BodyValue = 315;
			Hue = 828;
			
			SetStr( 200, 280 );
			SetDex( 150, 175 );
			SetInt( 30, 45 );
			
			SetHits( 3100, 3850 );
			
			SetDamage( 13, 19 );
			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Cold, 100 );
			
			SetResistance( ResistanceType.Physical, 65, 70 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 95, 100 );
			SetResistance( ResistanceType.Poison, 20, 25 );
			SetResistance( ResistanceType.Energy, 30, 35 );
			
			SetSkill( SkillName.Anatomy, 50, 60 );
			SetSkill( SkillName.Wrestling, 75, 80 );
			SetSkill( SkillName.Tactics, 75, 85 );
			SetSkill( SkillName.MagicResist, 95, 110 );
			
			Fame = 8500;
			Karma = -10000;
			
			VirtualArmor = 55;
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.UltraRich );
		}
		
		public override bool BardImmune{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 6; } }
		public override int Hides{ get{ return 14; } }
		public override HideType HideType{ get{ return HideType.Spined; } }

		public override int GetAttackSound()
		{
			return 0x34C;
		}

		public override int GetHurtSound()
		{
			return 0x354;
		}

		public override int GetAngerSound()
		{
			return 0x34C;
		}

		public override int GetIdleSound()
		{
			return 0x34C;
		}

		public override int GetDeathSound()
		{
			return 0x354;
		}
		
		public override void OnGaveMeleeAttack( Mobile defender )
		{
			if( 0.05 >= Utility.RandomDouble() )
				DismountDamage( defender );
		}
		
		private void DismountDamage( Mobile target )
		{
			IMount mount = target.Mount;

			if( mount == null )
				return;
			
			if( target.Mounted )
				target.SendLocalizedMessage( 1060083 ); //You fall off your mount and take damage!
			
			target.PlaySound( 1238 );
			target.FixedParticles( 0x3709, 0, 25, 9934, 1151, 4, EffectLayer.CenterFeet );
			target.Damage( Utility.RandomMinMax( 25, 40 ), this );

			mount.Rider = null;
		}
		
		public Syxffin( Serial serial ) : base( serial )
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
