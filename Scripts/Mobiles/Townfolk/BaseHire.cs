using System;
using Server;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Currency;

namespace Server.Mobiles
{
	public class BaseHire : BaseCreature
	{
		private bool m_IsHired = false;
		private int m_PayRate = 0;
		private int m_RemainingPay = int.MinValue;
		private Timer m_PayTimer;

		public static void Initialize()
		{
			foreach( object o in World.Mobiles.Values )
			{
				if( o is BaseHire )
				{
					BaseHire bh = (BaseHire)o;

					Mobile Owner = bh.ControlMaster;

					if( Owner != null )
						HireTable[Owner] = bh;
				}
			}
		}

		public BaseHire( AIType AI )
			: base( AI, FightMode.Aggressor, 10, 1, 0.1, 4.0 )
		{
			InitializeHire();
		}

		public BaseHire()
			: base( AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.1, 4.0 )
		{
			InitializeHire();
		}

		private void InitializeHire()
		{
			m_PayTimer = new PayTimer( this );
			UpdateHireCost();
		}

		public BaseHire( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)1 ); // version

			// version 1
			writer.Write( (int)m_PayRate );

			// version 0
			writer.Write( (bool)m_IsHired );
			writer.Write( (int)m_RemainingPay );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch( version )
			{
				case 1:
					m_PayRate = reader.ReadInt();
					goto case 0;

				case 0:
					m_IsHired = reader.ReadBool();
					m_RemainingPay = reader.ReadInt();

					m_PayTimer = new PayTimer( this );
					if( m_RemainingPay > int.MinValue )
						m_PayTimer.Start();
					UpdateHireCost();
					break;
			}
		}

		private static Hashtable m_HireTable = Hashtable.Synchronized( new Hashtable() );
		private static Hashtable HireTable { get { return m_HireTable; } }

		public override bool KeepsItemsOnDeath { get { return true; } }
		private int m_GoldOnDeath = 0;

		public override bool OnBeforeDeath()
		{
			// Stop the pay timer if its running
			if( m_PayTimer != null )
				m_PayTimer.Stop();

			m_PayTimer = null;

			// Get all of the gold on the hireling and add up the total amount
			if( this.Backpack != null )
			{
				Item[] AllGold = this.Backpack.FindItemsByType( typeof( Valorite ), true );
				if( AllGold != null )
				{
					foreach( Valorite g in AllGold )
						this.m_GoldOnDeath += g.Amount;
				}
			}

			return base.OnBeforeDeath();
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );
		}

		public override void OnAfterDelete()
		{
			if( m_PayTimer != null )
				m_PayTimer.Stop();

			m_PayTimer = null;

			base.OnAfterDelete();
		}

		[CommandProperty( AccessLevel.Administrator )]
		public bool IsHired { get { return m_IsHired; } }

		[CommandProperty( AccessLevel.Administrator )]
		public int PayRate { get { return m_PayRate; } }

		[CommandProperty( AccessLevel.Administrator )]
		public int RemainingPay { get { return m_RemainingPay; } }

		#region [ GetOwner ]
		public virtual Mobile GetOwner()
		{
			m_IsHired = false;
			Mobile Owner = ControlMaster;

			if( !Controlled || Owner == null || Owner.Deleted || Owner.Map != this.Map || !Owner.InRange( Location, 30 ) )
			{
				if( Owner != null )
				{
					HireTable.Remove( Owner );
					Say( 1005653 ); // Hmmm.  I seem to have lost my master.
				}

				m_RemainingPay = 0;
				SetControlMaster( null );
				return null;
			}

			m_IsHired = true;
			return Owner;
		}
		#endregion

		#region [ AddHire ]
		public virtual bool AddHire( Mobile m, Valorite pay )
		{
			Mobile owner = GetOwner();

			if( owner != null )
			{
				Console.Write( "BaseHire.AddHire() {0} Owner.Name = ", this.Name );
				Console.WriteLine( owner.Name );
				m.SendLocalizedMessage( 1043283, owner.Name ); // I am following ~1_NAME~.
				return false;
			}

			if( SetControlMaster( m ) )
			{
				m_IsHired = true;
				HireTable[m] = this;
				m_RemainingPay += pay.Amount;
				m_PayTimer.Start();
				SayTo( m, 1043258, string.Format( "{0}", (pay.Amount / m_PayRate) ) );//"I thank thee for paying me. I will work for thee for ~1_NUMBER~ days.", (int)item.Amount / m_PayRate );
				return true;
			}

			return false;
		}
		#endregion

		#region [ UpdateHireCost ]
		public virtual bool UpdateHireCost()
		{
			m_PayRate = (int)Skills[SkillName.Anatomy].Value + (int)Skills[SkillName.Tactics].Value;
			m_PayRate += (int)Skills[SkillName.Macing].Value + (int)Skills[SkillName.Swords].Value;
			m_PayRate += (int)Skills[SkillName.Fencing].Value + (int)Skills[SkillName.Archery].Value;
			m_PayRate += (int)Skills[SkillName.MagicResist].Value + (int)Skills[SkillName.Healing].Value;
			m_PayRate /= 38;
			return true;
		}
		#endregion

		#region [ OnDragDrop ]
		public override bool OnDragDrop( Mobile from, Item item )
		{
			if( m_PayRate > 0 )
			{
				// Is the creature already hired
				if( Controlled == false )
				{
					// Is the item the payment in gold
					if( item is Valorite )
					{
						Valorite payment = (Valorite)item;

						// Is the payment in gold sufficient
						if( payment.Amount >= m_PayRate )
						{

							// Check if this mobile already has a hire
							BaseHire hire = (BaseHire)HireTable[from];

							if( (hire != null) &&
							   (hire.Deleted == false) &&
							   (hire.Alive == true) &&
							   (hire.GetOwner() == from) )
							{
								SayTo( from, 500896 ); // I see you already have an escort.
								return false;
							}

							// Try to add the hireling as a follower
							return AddHire( from, payment );
						}
						else
						{
							this.SayHireCost();
						}
					}
					else
					{
						SayTo( from, 1043268 ); // Tis crass of me, but I want gold
					}
				}
				else
				{
					Say( 1042495 );// I have already been hired.
				}
			}
			else
			{
				SayTo( from, 500200 ); // I have no need for that.
			}

			return base.OnDragDrop( from, item );
		}
		#endregion


		#region [ OnSpeech ]
		public bool SayHireCost()
		{
			UpdateHireCost();
			Say( 1043256, string.Format( "{0}", m_PayRate ) ); // "I am available for hire for ~1_AMOUNT~ gold coins a day. If thou dost give me gold, I will work for thee."
			return true;
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			if( !e.Handled && e.Mobile.InRange( this, 6 ) )
			{
				int[] keywords = e.Keywords;
				string speech = e.Speech;

				// Check for a greeting or 'Hire'
				if( (e.HasKeyword( 0x003B ) == true) || (e.HasKeyword( 0x0162 ) == true) )
					e.Handled = this.SayHireCost();
			}

			base.OnSpeech( e );
		}
		#endregion

		#region [ GetContextMenuEntries ]
		public override void GetContextMenuEntries( Mobile from, System.Collections.Generic.List<ContextMenuEntry> list )
		{
			Mobile Owner = GetOwner();

			if( Owner == null )
			{
				base.GetContextMenuEntries( from, list );
				list.Add( new HireEntry( from, this ) );
			}
			else
				base.GetContextMenuEntries( from, list );
		}
		#endregion

		#region [ Class PayTimer ]
		private class PayTimer : Timer
		{
			private BaseHire m_Hire;

			public PayTimer( BaseHire hire )
				: base( TimeSpan.FromMinutes( 30.0 ), TimeSpan.FromMinutes( 30.0 ) )
			{
				m_Hire = hire;
				Priority = TimerPriority.OneMinute;
			}

			protected override void OnTick()
			{
				if( m_Hire != null && !m_Hire.Deleted && m_Hire.Alive )
				{
					// Get the current owner, if any (updates HireTable)
					Mobile owner = m_Hire.GetOwner();

					if( (owner == null) || (m_Hire.m_RemainingPay <= m_Hire.m_PayRate) )
					{
						if( owner != null && !owner.Deleted && owner.Alive && owner.InRange( m_Hire.Location, 30 ) )
							m_Hire.SendLocalizedMessage( 1060139, owner.Name ); // You have made my work easy for me, ~1_NAME~.  My task here is done.

						Stop();
						m_Hire.Delete();
					}
					else
					{
						m_Hire.m_RemainingPay -= m_Hire.m_PayRate;
					}
				}
				else
				{
					Stop();
				}
			}
		}
		#endregion

		#region [ Class HireEntry ]
		public class HireEntry : ContextMenuEntry
		{
			private Mobile m_Mobile;
			private BaseHire m_Hire;

			public HireEntry( Mobile from, BaseHire hire )
				: base( 6120, 3 )
			{
				m_Hire = hire;
				m_Mobile = from;
			}

			public override void OnClick()
			{
				m_Hire.SayHireCost();
			}
		}
		#endregion
	}
}
