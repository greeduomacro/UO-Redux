using System;
using System.Collections.Generic;
using Server;
using Server.Engines.Plants;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "the corpse of a plant demon" )]
	public class PlantDemon : BaseCreature
	{
		private Timer m_Timer;

		[Constructable]
		public PlantDemon()
			: base( AIType.AI_Archer, FightMode.Weakest, 14, 5, 0.2, 0.3 )
		{
			Body = 0x315;
			Hue = 2101;
			Name = "a plant demon";
			BaseSoundID = 352;

			SetDex( 125, 180 );
			SetInt( 10, 40 );
			SetStr( 150, 190 );

			SetHits( 550, 825 );

			SetDamage( 8, 12 );

			SetResistance( ResistanceType.Cold, 45, 75 );
			SetResistance( ResistanceType.Energy, 65, 85 );
			SetResistance( ResistanceType.Fire, 10, 35 );
			SetResistance( ResistanceType.Physical, 30, 50 );
			SetResistance( ResistanceType.Poison, 60, 90 );

			SetSkill( SkillName.Archery, 90.0, 105.0 );
			SetSkill( SkillName.MagicResist, 55.0, 65.0 );
			SetSkill( SkillName.Tactics, 85.0, 95.0 );
			SetSkill( SkillName.Wrestling, 45.0, 55.0 );

			Fame = 9500;
			Karma = -10500;

			int seeds = Utility.RandomMinMax( 0, 8 );

			for( int i = 0; i < seeds; i++ )
				AddToBackpack( new Seed() );

			if( 0.25 > Utility.RandomDouble() )
			{
				Seed s = Seed.RandomBonsaiSeed();

				AddToBackpack( s );
			}
		}

		public PlantDemon( Serial serial )
			: base( serial )
		{
		}

		public override HideType HideType{ get{ return HideType.Barbed; } }
		public override int Hides{ get{ return 6; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 2 );
		}

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			base.OnDamage( amount, from, willKill );

			if( from == null || !from.Alive || from.Map != this.Map )
				return;

			Item i = from.FindItemOnLayer( Layer.FirstValid );
			Item ii = from.FindItemOnLayer( Layer.TwoHanded );

			if( i != null && i is BaseWeapon )
			{
				if( i is BaseRanged )
				{
					if( 0.40 > Utility.RandomDouble() )
						FireBullets( from );
				}
			}
			else if( ii != null && ii is BaseWeapon )
			{
				if( ii is BaseRanged )
				{
					if( 0.45 > Utility.RandomDouble() )
						FireBullets( from );
				}
			}
		}

		public override void AlterSpellDamageFrom( Mobile from, ref int damage )
		{
			base.AlterSpellDamageFrom( from, ref damage );

			double chance = 0.33;

			if( damage > 50 )
				chance = 0.5;
			else if( damage > 85 )
				chance = 0.85;

			if( chance > Utility.RandomDouble() )
				FireBullets( from );
		}

		public void FireBullets( Mobile target )
		{
			List<Mobile> toDmg = new List<Mobile>();
			IPooledEnumerable eable = target.GetMobilesInRange( 6 );

			foreach( Mobile m in eable )
			{
				if( m.Z < (target.Z + 5) && m.Z > (target.Z - 5) && m != this && !m.Hidden && m.Alive && this.CanSee( m ) )
					toDmg.Add( m );
			}

			eable.Free();

			if( toDmg.Count == 0 )
				return;

			if( m_Timer == null || !m_Timer.Running )
				this.Say( true, "*begins pelting the area with giant plant seeds*" );
			else if( m_Timer != null )
			{
				m_Timer.Stop();
				m_Timer = null;
			}

			int ammo = Utility.RandomMinMax( 12, 32 );

			m_Timer = new InternalTimer( this, toDmg, ammo );
			m_Timer.Start();
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

			if( m_Timer != null )
			{
				m_Timer.Stop();
				m_Timer = null;
			}
		}

		private class InternalTimer : Timer
		{
			private PlantDemon m_Demon;
			private List<Mobile> m_List;
			private int ammoAmt;

			public InternalTimer( PlantDemon demon, List<Mobile> list, int ammo )
				: base( TimeSpan.FromSeconds( 0.25 ), TimeSpan.FromSeconds( 0.35 ) )
			{
				m_Demon = demon;
				m_List = list;
				ammoAmt = ammo;

				Priority = TimerPriority.TenMS;
			}

			protected override void OnTick()
			{
				if( !m_Demon.Alive || m_Demon.Deleted )
				{
					this.Stop();
					return;
				}

				if( ammoAmt-- > 0 )
				{
					Mobile m = m_List[Utility.Random( m_List.Count )];

					if( m.Alive && m.InRange( m_Demon, 14 ) && m_Demon.CanSee( m ) && m_Demon.InLOS( m ) )
					{
						Effects.PlaySound( m.Location, m.Map, 0x2B1 );
						Effects.SendMovingEffect( m_Demon, m, 0xE73, 5, 10, false, false );

						if( m_List.Count < 5 )
							m.Damage( Utility.RandomMinMax( 3, 6 ), m_Demon );
						else
							m.Damage( Utility.RandomMinMax( 12, 16 ), m_Demon );

						if( m is PlayerMobile && 0.10 > Utility.RandomDouble() )
						{
							m.SendMessage( "The giant plant seed slams into you, but falls mostly undamaged to the ground." );

							Seed s = null;

							if( 0.33 > Utility.RandomDouble() )
								s = Seed.RandomBonsaiSeed();
							else
								s = new Seed();

							if( s != null )
								s.MoveToWorld( m.Location, m.Map );
						}
					}
					else
						ammoAmt++;
				}
				else
				{
					m_List.Clear();
					m_List = null;

					this.Stop();
				}
			}
		}
	}
}