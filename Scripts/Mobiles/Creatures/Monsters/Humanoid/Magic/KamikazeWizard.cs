using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class KamikazeWizard : BaseCreature
	{
		[Constructable]
        public KamikazeWizard()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 5, 0.2, 0.4)
		{
			Name = NameList.RandomName( "male" ) + ",";
			Title = "the wizard";
			BodyValue = 0x190;
			Hue = Utility.RandomSkinHue();
			
			SetStr( 200, 300 );
			SetInt( 400, 500 );
			SetDex( 50, 125 );
			
			SetHits( 350, 450 );
			
			SetDamage( 10, 14 );
			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Energy, 50 );
			
			SetResistance( ResistanceType.Physical, 20, 30 );
			SetResistance( ResistanceType.Fire, 35, 50 );
			SetResistance( ResistanceType.Cold, 30, 40 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 40, 50 );
			
			SetSkill( SkillName.Magery, 100, 110 );
			SetSkill( SkillName.EvalInt, 80, 90 );
			SetSkill( SkillName.Meditation, 50, 60 );
			SetSkill( SkillName.Tactics, 40, 50 );
			SetSkill( SkillName.Wrestling, 40, 50 );
			
			Fame = 10000;
			Karma = 0;
			
			VirtualArmor = 35;

			AddItem( new Robe( Utility.RandomAnimalHue() ) );
			AddItem( new WizardsHat( Utility.RandomYellowHue() ) );
			AddItem( new Sandals( 1 ) );
			EquipItem( new FullSpellBook() );
		}
		
        public KamikazeWizard( Serial serial )
            : base(serial)
		{
		}

		public override bool InitialInnocent{ get{ return true; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average, 2 );
		}

		#region Self-explode
		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			new InternalTimer( this ).Start();
			
			base.OnDamage( amount, from, willKill );
		}
		
		public void Explode()
		{
			if( this != null && this.Alive )
			{
				List<Mobile> list = new List<Mobile>();

				foreach( Mobile mobile in this.GetMobilesInRange( 10 ) )
				{
					list.Add( mobile );
				}
				
				for( int i = 0; i < list.Count; i++ )
					AOS.Damage( list[i], this, Utility.RandomMinMax( 20, 30 ), 0, 100, 0, 0, 0, false );

				list.Clear();

				int[] parts = new int[] { 7389, 7390, 7394, 7395, 7396, 7397, 7405, 7406, 7407, 7408, 7409 };

				for( int i = 0; i < parts.Length; i++ )
				{
					int itemID = parts[i];

					Point3D end = new Point3D( this.X + Utility.RandomMinMax( -8, 8 ), this.Y + Utility.RandomMinMax( -8, 8 ), this.Z );
					Effects.SendMovingEffect( this, new Entity( Serial.Zero, end, this.Map ), itemID, 5, 10, false, false );

					Timer.DelayCall( TimeSpan.FromSeconds( 0.5 ), new TimerStateCallback( FinishExplosion ), new object[] { this.Map, end, itemID } );
				}

				this.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Waist );
				this.PlaySound( 0x307 );
				
				this.Kill();

				if( this.Corpse != null )
					this.Corpse.Delete();
			}
		}

		public static void FinishExplosion( object info )
		{
			object[] args = (object[])info;

			if( args == null || args.Length <= 0 )
				return;

			Map map = (Map)args[0];
			Point3D p = (Point3D)args[1];
			int itemID = (int)args[2];

			new Server.Items.TimedItem( 120.0, itemID ).MoveToWorld( p, map );
		}
		#endregion

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
		
		private class InternalTimer : Timer
		{
			private KamikazeWizard m_Wizard;
			private int ticker;
			
            public InternalTimer( KamikazeWizard wizard )
                : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0))
			{
				m_Wizard = wizard;
				
				ticker = 0;
				
				Priority = TimerPriority.TwoFiftyMS;
			}
			
			protected override void OnTick()
			{
				if( m_Wizard != null && m_Wizard.Alive )
				{
					switch( Utility.Random( 4 ) )
					{
						default:
							case 0: m_Wizard.Say( "*squeek*" ); break;
							case 1: m_Wizard.Say( "*fizzle*" ); break;
							case 2: m_Wizard.Say( "*boomie!*" ); break;
							case 3: m_Wizard.Say( "*blerp*" ); break;
					}
					
					if( ++ticker == 4 )
					{
						m_Wizard.Say( "uh oh!" );
						m_Wizard.Explode();

						Stop();
					}
				}
				else
					Stop();
			}
		}
	}
}
