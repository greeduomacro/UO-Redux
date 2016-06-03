using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a ghostly form" )]
	public class GhoulLord : BaseCreature
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.ParalyzingBlow;
		}
		
		[Constructable]
		public GhoulLord() : base( AIType.AI_Mage, FightMode.Good, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "impaler" ) + ", the ghoul lord";
			BodyValue = 26;
			BaseSoundID = 0x47E;
			
			SetStr( 475, 500 );
			SetDex( 75, 95 );
			SetInt( 300, 325 );
			
			SetHits( 285, 300 );
			
			SetDamage( 10, 13 );
			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Energy, 50 );
			
			SetResistance( ResistanceType.Physical, 45, 60 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Cold, 30, 40 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 40 );
			
			SetSkill( SkillName.Wrestling, 60, 80 );
			SetSkill( SkillName.Tactics, 70, 80 );
			SetSkill( SkillName.MagicResist, 85, 95 );
			SetSkill( SkillName.Magery, 70, 80 );
			SetSkill( SkillName.EvalInt, 75, 85 );
			
			Fame = 10000;
			Karma = -12500;
			
			VirtualArmor = 30;
		}
		
		public override int TreasureMapLevel{ get{ return 4; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.MedScrolls, 2 );
		}
		
		public void SpawnGhouls( Mobile target )
		{
			Map map = this.Map;
			
			if( map == null )
				return;
			
			int newGhoul = Utility.RandomMinMax( 2, 5 );
			
			for( int i = 0; i < newGhoul; ++i )
			{
				Ghoul ghoul = new Ghoul();
				
				ghoul.Team = this.Team;
				ghoul.FightMode = FightMode.Closest;
				
				bool validLocation = false;
				Point3D loc = this.Location;
				
				for( int j = 0; !validLocation && j < 10; ++j )
				{
					int x = X + Utility.Random( 3 ) - 1;
					int y = Y + Utility.Random( 3 ) - 1;
					int z = map.GetAverageZ( x, y );
					
					if( validLocation = map.CanFit( x, y, this.Z, 16, false, false ) )
						loc = new Point3D( x, y, Z );
					else if( validLocation = map.CanFit( x, y, z, 16, false, false ) )
						loc = new Point3D( x, y, z );
				}
				
				ghoul.MoveToWorld( loc, map );
				ghoul.Combatant = target;
			}
		}
		
		public override bool OnBeforeDeath()
		{
			Mobile m = this;
			
			SpawnGhouls( m.Combatant );
			
			return base.OnBeforeDeath();
		}
		
		public GhoulLord( Serial serial ) : base( serial )
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
	
