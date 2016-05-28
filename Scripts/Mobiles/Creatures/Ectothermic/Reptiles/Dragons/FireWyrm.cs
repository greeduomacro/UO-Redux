using System;
using System.Collections;
using Server;
using Server.Mobiles;

namespace Server.Mobiles.Creatures.Reptiles
{
	[CorpseName( "a charred dragon corpse" )]
	public class FireWyrm : BaseDragon
	{
		[Constructable]
		public FireWyrm()
			: base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.1, 0.3 )
		{
			Name = "a fire wyrm";
			BodyValue = 59;
			BaseSoundID = 362;

			SetStr( 800, 850 );
			SetDex( 100, 150 );
			SetInt( 500, 575 );

			SetHits( 1650, 1750 );

			SetDamage( 20, 25 );
			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Fire, 60 );

			SetResistance( ResistanceType.Physical, 60, 70 );
			SetResistance( ResistanceType.Fire, 75, 85 );
			SetResistance( ResistanceType.Cold, 40, 45 );
			SetResistance( ResistanceType.Poison, 50, 55 );
			SetResistance( ResistanceType.Energy, 50, 55 );

			SetSkill( SkillName.EvalInt, 110, 120 );
			SetSkill( SkillName.Magery, 110, 115 );
			SetSkill( SkillName.Meditation, 95, 105 );
			SetSkill( SkillName.MagicResist, 100, 110 );
			SetSkill( SkillName.Tactics, 100, 110 );
			SetSkill( SkillName.Wrestling, 100, 110 );
			SetSkill( SkillName.Anatomy, 105, 110 );

			Fame = 15000;
			Karma = 8000;

			VirtualArmor = 70;
			RangePerception = 20;

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 2 );
			AddLoot( LootPack.Gems, 10 );
			AddLoot( LootPack.MedScrolls, 3 );
			AddLoot( LootPack.HighScrolls, 2 );
		}

		public override bool HasBreath { get { return true; } }
		public override int TreasureMapLevel { get { return 5; } }
		public override int Meat { get { return 25; } }
		public override int Hides { get { return 50; } }
		public override HideType HideType { get { return HideType.Barbed; } }
		public override int Scales { get { return 15; } }
		public override ScaleType ScaleType { get { return ScaleType.Red; } }
		public override FoodType FavoriteFood { get { return FoodType.Meat; } }
		public override bool CanDestroyObstacles { get { return true; } }
		public override bool IsScaryToPets { get { return true; } }
		public override bool IsScaredOfScaryThings { get { return false; } }

		public override void GetContextMenuEntries( Mobile from, System.Collections.Generic.List<Server.ContextMenus.ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if( ControlMaster != null && ControlMaster.InRange( this, 10 ) && from == this.ControlMaster )
				list.Add( new InternalContextMenu( this ) );
		}

		public override bool OnBeforeDeath()
		{
			Mobile m = this;

			SpawnDrakes( m.Combatant );
			DeathStrike();

			return base.OnBeforeDeath();
		}

		public void SpawnDrakes( Mobile target )
		{
			Map map = this.Map;

			if( map == null )
				return;

			int newDrakes = 2;

			for( int i = 0; i < newDrakes; ++i )
			{
				Drake drake = new Drake();

				drake.Team = this.Team;
				drake.FightMode = FightMode.Closest;

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

				drake.MoveToWorld( loc, map );
				drake.Combatant = target;
			}
		}

		public void DeathStrike()
		{
			ArrayList list = new ArrayList();

			foreach( Mobile m in this.GetMobilesInRange( 10 ) )
			{
				if( m == this || !CanBeHarmful( m ) )
					continue;

				if( m is BaseCreature /*&& (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team)*/ )
					list.Add( m );
				else if( m.Player && m.Combatant == this )
					list.Add( m );
			}

			foreach( Mobile m in list )
			{
				if( m.Combatant == this )
				{
					DoHarmful( m );

					m.FixedParticles( 0x3709, 10, 30, 5052, EffectLayer.LeftFoot );
					m.PlaySound( 0x208 );

					int toStrike = Utility.RandomMinMax( 50, 65 );

					m.Damage( toStrike, this );
				}
			}
		}

		public void FlameStrike()
		{
			ArrayList list = new ArrayList();

			foreach( Mobile m in this.GetMobilesInRange( 5 ) )
			{
				if( m == this || !CanBeHarmful( m ) )
					continue;

				if( m is BaseCreature && m.Combatant == this )
					list.Add( m );
				else if( m.Player && m.Combatant == this && (m != this.ControlMaster) )
					list.Add( m );
			}

			foreach( Mobile m in list )
			{
				if( m.Combatant == this )
				{
					DoHarmful( m );

					m.FixedParticles( 0x3709, 10, 30, 5052, EffectLayer.LeftFoot );
					m.PlaySound( 0x208 );

					if( m.Player )
					{
						m.SendMessage( "Your skin blisters as the fire burns you!" );
					}

					int toStrike = (int)(Hits * 0.10);

					Hits += toStrike;
					m.Damage( toStrike, this );
				}
			}
		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			double chance = 0.15;

			if( !Controlled )
				chance = 0.30;
			else if( Controlled && (ControlMaster != null && ControlMaster.InRange( this, 8 )) )
				chance = 0.40;

			if( chance >= Utility.RandomDouble() )
				FlameStrike();
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			double chance = 0.10;

			if( !Controlled )
				chance = 0.25;
			else if( Controlled && (ControlMaster != null && ControlMaster.InRange( this, 8 )) )
				chance = 0.30;

			if( chance >= Utility.RandomDouble() )
				FlameStrike();
		}

		public override void AlterMeleeDamageTo( Mobile to, ref int damage )
		{
			int fixedDamage = (damage / 2);

			if( this.Controlled )
				damage = fixedDamage;
		}

		public FireWyrm( Serial serial )
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

		private class InternalContextMenu : Server.ContextMenus.ContextMenuEntry
		{
			private FireWyrm m_Wyrm;

			public InternalContextMenu( FireWyrm wyrm )
				: base( 2061, 10 )
			{
				m_Wyrm = wyrm;
			}

			public override void OnClick()
			{
				m_Wyrm.FlameStrike();
			}
		}
	}
}
