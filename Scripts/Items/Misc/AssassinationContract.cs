using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Gumps;
using Server.Currency;

namespace Server.Items
{
	public class AContract : Item
	{
		private Mobile		m_Owner;		// Whos Contracted?
		private Mobile		m_Issuer;		// Who Issued the Contract?
		private string		m_Target;		// what is the Name of Target?
		private int		m_Expiration;		// How many days to complete this Contract?
		private int		m_TimeLeft;		// How many Hours are left?
		private bool		m_Completed;		// Is this contract Completed?
		private bool		m_Expired;		// Has this Contract Expired? 
        private int         m_Copper;			// How much is this Contract for?
		private DateTime	m_Created;		// When was this Contract created??
		private bool		m_Debug;		// Debugging Timer Speedup
		public static void Initialize()
		{
			// Register our speech handler
			EventSink.Speech += new SpeechEventHandler( AContractSpeech );
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool Expired
		{
			get{ return m_Expired; }
			set{ m_Expired = value; }
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool Completed
		{
			get{ return m_Completed; }
			set{ m_Completed = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int DaystoComplete
		{
			get{ return m_Expiration; }
			set{ m_Expiration = value; }
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime Created
		{
			get{ return m_Created; }
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public int HoursLeft
		{
			get{ return m_TimeLeft; }
			set{ m_TimeLeft = value; }
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public int ContractValue
		{
			get{ return m_Copper; }
            set { m_Copper = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Contractor
		{
			get{ return m_Issuer; }
			set{ m_Issuer = value; }
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Contractee
		{
			get{ return m_Owner; }
			set{ m_Owner = value; } 

		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string Target
		{
			get{ return m_Target; }
			set{ m_Target = value; }
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool DebugMode
		{
			get{ return m_Debug; }
			set{ m_Debug = value; }
		}

		public AContract( Serial serial ) : base( serial )
		{
		}
		private Timer m_ContractTimer;
		public void BeginContractTimer( )
		{
			if ( m_TimeLeft > 0 )
			{
				if ( m_Debug == true ) m_ContractTimer = new ExpireContractDebugTimer( m_TimeLeft, this );
				else m_ContractTimer = new ExpireContractTimer( m_TimeLeft, this );
				m_ContractTimer.Start();
			}
			else m_Expired = true;
		}

		private class ExpireContractTimer : Timer
		{
			private int m_TimeLeft;
			private Timer m_ContractTimer;
			private AContract m_AContract;

			public ExpireContractTimer( int timeleft, AContract acontract ) : base( TimeSpan.FromHours( 1 ) )
			{
				m_TimeLeft = timeleft;
				m_AContract = acontract;
				Priority = TimerPriority.OneMinute;
			}

			protected override void OnTick()
			{
				m_AContract.m_TimeLeft = m_AContract.m_TimeLeft - 1;

				if ( m_AContract.m_TimeLeft > 0 )
				{
					m_ContractTimer = new ExpireContractTimer(m_AContract.m_TimeLeft, m_AContract);
					m_ContractTimer.Start();
					
				}
				else
				{
					if (m_AContract.m_Issuer != null ) 
					{
						BankBox box = m_AContract.m_Issuer.BankBox;
						if ( box != null )
						{
							m_AContract.Delete();

							int deposited = 0;

                            int toAdd = m_AContract.m_Copper;

							Gold copper;

							while ( toAdd > 10)
							{
								copper = new Gold( 10 );

								if ( box.TryDropItem( m_AContract.m_Issuer, copper, false ) )
								{
									toAdd -= 10;
									deposited += 10;
								}
								else
								{
									copper.Delete();

									m_AContract.m_Issuer.AddToBackpack( new BankCheck( toAdd ) );
									toAdd = 0;

									break;
								}
							}

							if ( toAdd > 0 )
							{
								copper = new Gold( toAdd );

								if ( box.TryDropItem( m_AContract.m_Issuer, copper, false ) )
								{
									deposited += toAdd;
								}
								else
								{
									copper.Delete();

									m_AContract.m_Issuer.AddToBackpack( new BankCheck( toAdd ) );
								}
							}

							// Gold was deposited in your account:
							m_AContract.m_Issuer.SendLocalizedMessage( 1042672, true, " " + deposited.ToString() );

						}
						/*else
						{
							//from.SendLocalizedMessage( 1047026 ); // That must be in your bank box to use it.
						}*/
		

					}

					m_AContract.m_Expired = true;
				}
			}
		}

		private class ExpireContractDebugTimer : Timer
		{
			private int m_TimeLeft;
			private Timer m_ContractTimer;
			private AContract m_AContract;

			public ExpireContractDebugTimer( int timeleft, AContract acontract ) : base( TimeSpan.FromMinutes( 1 ) )
			{
				m_TimeLeft = timeleft;
				m_AContract = acontract;
				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				m_AContract.m_TimeLeft = m_AContract.m_TimeLeft - 1;

				if ( m_AContract.m_TimeLeft > 0 )
				{
					if ( m_AContract.m_Debug == true ) m_ContractTimer = new ExpireContractDebugTimer(m_AContract.m_TimeLeft, m_AContract);
					else  m_ContractTimer = new ExpireContractTimer(m_AContract.m_TimeLeft, m_AContract);
					m_ContractTimer.Start();
					
				}
				else
				{
					if (m_AContract.m_Issuer != null ) 
					{
						BankBox box = m_AContract.m_Issuer.BankBox;
						if ( box != null )
						{

							int deposited = 0;

							int toAdd = m_AContract.m_Copper;

							Gold copper;

							while ( toAdd > 60000 )
							{
								copper = new Gold( 60000 );

								if ( box.TryDropItem( m_AContract.m_Issuer, copper, false ) )
								{
									toAdd -= 60000;
									deposited += 60000;
								}
								else
								{
									copper.Delete();

									m_AContract.m_Issuer.AddToBackpack( new BankCheck( toAdd) );
									toAdd = 0;

									break;
								}
							}

							if ( toAdd > 0 )
							{
								copper = new Gold( toAdd );

								if ( box.TryDropItem( m_AContract.m_Issuer, copper, false ) )
								{
									deposited += toAdd;
								}
								else
								{
									copper.Delete();

									m_AContract.m_Issuer.AddToBackpack( new BankCheck( toAdd ) );
								}
							}

							// Gold was deposited in your account:
							m_AContract.m_Issuer.SendLocalizedMessage( 1042672, true, " " + deposited.ToString() );

						}
						/*else
						{
							//from.SendLocalizedMessage( 1047026 ); // That must be in your bank box to use it.
						}*/
		

					}

					m_AContract.m_Expired = true;
				}
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version
			writer.Write( (bool) m_Debug );
			writer.Write( (DateTime) m_Created );
			writer.Write( (int) m_Copper );
			writer.Write( (bool) m_Expired );
			writer.Write( (bool) m_Completed );
			writer.Write( (int) m_TimeLeft );
			writer.Write( (int) m_Expiration );
			writer.Write( (string) m_Target );
			writer.Write( m_Issuer );
			writer.Write( m_Owner );

		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 2: //added Debug
				case 1:
				{
					if ( version < 2 ) m_Debug = false;
					else m_Debug = reader.ReadBool();

					m_Created = reader.ReadDateTime();
					goto case 0;
				}
				case 0:				
				{
					m_Copper = reader.ReadInt();
					m_Expired = reader.ReadBool();
					m_Completed = reader.ReadBool();
					m_TimeLeft = reader.ReadInt();
					m_Expiration = reader.ReadInt();
					m_Target = reader.ReadString();
					m_Issuer = reader.ReadMobile();
					m_Owner = reader.ReadMobile();
					if ( m_Expired == false ) BeginContractTimer( );
					break;
				}
			}
		}

		[Constructable]
		public AContract( int expiration ) : base( 0x14F0 )
		{
			Name = "A contract of employment";
			Weight = 1.0;
			LootType = LootType.Blessed;
			m_Expiration = expiration;
			m_TimeLeft = ( m_Expiration * 24 );
			m_Expired = false;
			BeginContractTimer();
			m_Created = DateTime.Now;
			m_Debug = false;
			m_Completed = false;
		}

		public override bool DisplayLootType{ get{ return false; } }

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties( list );

			list.Add( 1060738, m_Copper.ToString() ); // value: ~1_val~
		}

		public override void OnSingleClick( Mobile from )
		{
			from.Send( new MessageLocalizedAffix( Serial, ItemID, MessageType.Label, 0x3B2, 3, 1041243, "", AffixType.Append, String.Concat( " ", m_Copper.ToString() ), "" ) ); // A bank check:
		}

		public override void OnDoubleClick( Mobile from )
		{
			from.SendGump( new AContractGump( m_Target, m_Issuer, m_Owner, m_Copper, m_Expiration, from, this  ) );
		}

		public class AContractGump : Server.Gumps.Gump
		{
			private Mobile m_From;
			private AContract m_AContract;

			public AContractGump( string target, Mobile issuer, Mobile owner, int copper, int expiration, Mobile from, AContract acontract ) : base(0, 0)
			{
				m_From = from;
				m_AContract = acontract;
				if ( target == null ) target = "Nobody";
				if ( issuer == null ) issuer = from;
				if ( owner == null ) owner = from;


				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;
				this.AddPage(0);
				this.AddBackground(7, 6, 285, 270, 9380);
				this.AddLabel(67, 47, 0, @"Contract of Assasination");
				this.AddImage(33, 50, 59);
				this.AddImage(228, 50, 57);
				this.AddImage(98, 181, 52);
				this.AddLabel(43, 70, 0, @"I hereby employ " + owner.Name + " to");
				this.AddLabel(151, 189, 0, @issuer.Name);
				this.AddLabel(43, 85, 0, @"assassinate " + target + " within the");
				this.AddLabel(43, 150, 0, @"this contract and I will provide");
				this.AddLabel(43, 100, 0, @"next " + expiration + " day(s) for the amount of");
				this.AddLabel(43, 115, 0, @copper + " copper pieces.");
				this.AddLabel(43, 135, 0, @"Upon completion, return to me with");
				this.AddLabel(43, 165, 0, @"your reward.");
				if ( acontract.m_Owner == from )
				{
					if ( acontract.m_Expired == false ) 
					{
						this.AddButton(137, 220, 1210, 1209, 1, GumpButtonType.Reply, 1);
						if ( acontract.m_Completed != true ) this.AddLabel(156, 218, 0, @"Verify Kill");
						else this.AddLabel(156, 218, 0, @"Claim Reward");
					}
					else this.AddLabel(137, 218, 0, @"Contract Expired");
				}	
				else this.AddLabel(137, 218, 0, @"Not your Contract");
					
				
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{
				if ( info.ButtonID == 1 ) 
				{
					if ( m_AContract.m_Issuer.NetState == null  &&  m_AContract.m_Completed == true  )
					{
						m_AContract.m_Owner.SendMessage( "The employer is not available at this moment. Your contract will not expire until you have been paid." );
						if ( m_AContract.HoursLeft < 24 ) m_AContract.HoursLeft = 24;
					}
					else
						m_From.Target = new M_Target( m_AContract );
				}
			}

		
		}
		public class CreateAContractGump : Server.Gumps.Gump
		{
			private Mobile m_Issuer;
			private Mobile m_Owner ;
			private string m_Target;	
			private int	   m_Expiration;
			private int	   m_Copper;

			public CreateAContractGump( Mobile issuer, Mobile owner ) : base(0, 0)
			{
				m_Issuer = issuer;
				m_Owner = owner;


				this.Closable=false;
				this.Disposable=false;
				this.Dragable=true;
				this.Resizable=false;
				this.AddPage(0);
				this.AddBackground(6, 4, 322, 293, 2520);
				this.AddLabel(38, 45, 0, @"Application for a Contract of Employment");
				this.AddImage(38, 40, 93);
				this.AddImage(176, 40, 93);
				this.AddImage(107, 40, 93);
				this.AddTextEntry(65, 89, 200, 20, 0, 3, "" );
				this.AddTextEntry(65, 189, 200, 20, 0, 4, String.Format( "{0}", 1 ) );
				this.AddTextEntry(65, 140, 200, 20, 0, 5, String.Format( "{0}", 60000 ) );
				this.AddLabel(65, 68, 0, @"Target Name");
				this.AddLabel(65, 118, 0, @"Reward for Completion");
				this.AddLabel(65, 167, 0, @"Time to Complete (In Days)");
				this.AddButton(227, 227, 242, 241, 1, GumpButtonType.Reply, 0);
				this.AddButton(39, 227, 247, 248, 2, GumpButtonType.Reply, 0);
					
				
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{
				Mobile from = state.Mobile;
				if ( info.ButtonID == 2 )
				{
					try
					{
						if ( info.GetTextEntry( 3 ) != null && info.GetTextEntry( 4 ) != null && info.GetTextEntry( 5 ) != null ) 
						{
							if (info.GetTextEntry( 3 ).Text == "")
							{
								m_Issuer.SendMessage( "Please fill in all blanks." );
								from.SendGump( new CreateAContractGump( m_Issuer, m_Owner ) );
								return;
							}
							m_Target = info.GetTextEntry( 3 ).Text as string;
							m_Expiration = Convert.ToInt32(info.GetTextEntry( 4 ).Text, 10);
							if ( m_Expiration < 1 )
							{
								m_Issuer.SendMessage( "This contract must last at least 1 day." );
								from.SendGump( new CreateAContractGump( m_Issuer, m_Owner ) );
								return;
							}
							m_Copper = Convert.ToInt32(info.GetTextEntry( 5 ).Text, 10);
                            if (!Banker.Withdraw(from, m_Copper))
							{
                                from.SendMessage("You cannot afford a reward of {0}!", m_Copper);
								from.SendGump( new CreateAContractGump( m_Issuer, m_Owner ) );
								return;
							}

							AContract x = ( new AContract( m_Expiration ) );
							x.m_Owner = m_Owner;
							x.m_Issuer = m_Issuer;
							x.m_Target = m_Target;
                            x.m_Copper = m_Copper;
						m_Owner.SendGump( new ConfirmAContractGump( m_Issuer, m_Owner, x, m_Target, m_Copper, m_Expiration ) );
						}
						else
						{
							m_Issuer.SendMessage( "Please fill in all blanks." );
							m_Issuer.SendGump( new CreateAContractGump( m_Issuer, m_Owner ) );
						}
					}	
					catch
					{
						from.SendMessage( "Bad format. #### expected." );
						m_Issuer.SendGump( new CreateAContractGump( m_Issuer, m_Owner ) );

					}
					//else if ( info.ButtonID == 2 ) Close;
				}

		
			}
		}

			public static void AContractSpeech( SpeechEventArgs e )
			{
				Mobile from = e.Mobile;
				if ( e.Speech.ToLower().IndexOf( "i wish to form a contract" ) >= 0 )
				{
					from.Target = new CreateContract_Target();
				}
			}

			public void ExpireContract()
			{
				BankBox box = m_Issuer.BankBox;

				if ( box != null )
				{
					Delete();

					int deposited = 0;

					int toAdd = m_Copper;

					Gold copper;

					while ( toAdd > 60000 )
					{
						copper = new Gold( 60000 );

						if ( box.TryDropItem( m_Issuer, copper, false ) )
						{
							toAdd -= 60000;
							deposited += 60000;
						}
						else
						{
							copper.Delete();

							m_Issuer.AddToBackpack( new BankCheck( toAdd ) );
							toAdd = 0;

							break;
						}
					}

					if ( toAdd > 0 )
					{
						copper = new Gold( toAdd );

						if ( box.TryDropItem( m_Issuer, copper, false ) )
						{
							deposited += toAdd;
						}
						else
						{
							copper.Delete();

							m_Issuer.AddToBackpack( new BankCheck( toAdd ) );
						}
					}

					// Gold was deposited in your account:
					m_Issuer.SendLocalizedMessage( 1042672, true, " " + deposited.ToString() );

				}
				/*else
				{
					//from.SendLocalizedMessage( 1047026 ); // That must be in your bank box to use it.
				}*/
			}
			private class M_Target : Target
			{
				private AContract	m_AContract;
				private PlayerMobile m;

				public M_Target( AContract acontract ) : base( 12, false, TargetFlags.None )
				{
					m_AContract = acontract;
				}

				protected override void OnTarget( Mobile from, object targeted )
				{
					if ( m_AContract.Completed == false )
					{
						if ( targeted is Corpse )
						{

							if (((Corpse)targeted).Owner is PlayerMobile )
							{
								if (((Corpse)targeted).Killer == m_AContract.m_Owner )
								{

									m = ((Corpse)targeted).Owner as PlayerMobile;
									if ( m.RawName == m_AContract.m_Target )
									{
										if( m.GameTime - TimeSpan.FromHours( ( m_AContract.m_Expiration * 24 ) - m_AContract.m_TimeLeft  ) > TimeSpan.FromSeconds(1))
										{
											m_AContract.m_Completed = true; 
										}
										else 
										{
											from.SendMessage("Date is younger");
						
										}
									}
									else from.SendMessage("That does not seem to be the target." );
								}
								else from.SendMessage("You did not kill that being.");
							}
							else from.SendMessage("That corpse does not belong to a player.");
						}
						else from.SendMessage("That is not a corpse.");

						from.SendGump( new AContractGump( m_AContract.m_Target, m_AContract.m_Issuer, m_AContract.m_Owner, m_AContract.m_Copper, m_AContract.m_Expiration, from, m_AContract  ) );

					}
					else
					{
					
						if (targeted == m_AContract.m_Issuer) 
						{
							from.Say( m_AContract.m_Owner.Name + " , your services have been completed." );
							from.SendMessage("Your reward has been placed in your pack.");
							m_AContract.Delete();
							from.AddToBackpack( new BankCheck( m_AContract.m_Copper ) );

						}
						else 
						{
							from.SendMessage("This is not the person who signed the contract.");
							from.SendGump( new AContractGump( m_AContract.m_Target, m_AContract.m_Issuer, m_AContract.m_Owner, m_AContract.m_Copper, m_AContract.m_Expiration, from, m_AContract  ) );

						}

					}
				}

			}
			private class CreateContract_Target : Target
			{
				private Mobile		m_Owner;				// Whos Contracted?
				private Mobile		m_Issuer;				// Who Issued the Contract?


				public CreateContract_Target() : base( 12, false, TargetFlags.None )
				{

				}

				protected override void OnTarget( Mobile from, object targeted )
				{
					if ( targeted is PlayerMobile )
					{
						m_Owner = targeted as PlayerMobile;
						m_Issuer = from;
						if ( from.AccessLevel > AccessLevel.Player ) from.SendGump( new CreateAContractGump( m_Issuer, m_Owner ) );
						else
						{
							if	( m_Owner == m_Issuer ) from.SendMessage("You can't contract yourself!");
							else from.SendGump( new CreateAContractGump( m_Issuer, m_Owner ) );
						}
					
					}
					else from.SendMessage("This is not a Player");
				
				}

			}
		}
	public class ConfirmAContractGump : Gump
	{
		private Mobile m_From;
		private Mobile m_ToPlayer;
		private AContract m_AContract;
		private string m_Target;
		private int m_Expiration;
		private int m_Copper;

		public ConfirmAContractGump( Mobile from, Mobile toplayer, AContract acontract, string target, int copper, int expiration ) : base( 0, 0 )
		{
			m_From = from;
			m_ToPlayer = toplayer;
			m_AContract = acontract;
			m_Copper = copper;
			m_Expiration = expiration;
			m_Target = target;

			Closable = false;
			Resizable = false;
			
			this.Closable=false;
			this.Disposable=false;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);
			this.AddBackground(-1, 0, 341, 369, 9200);
			this.AddBackground(22, 40, 285, 270, 9380);
			this.AddLabel(82, 81, 0, @"Contract of Assasination");
			this.AddImage(48, 84, 59);
			this.AddImage(243, 84, 57);
			this.AddImage(113, 215, 52);
			this.AddLabel(58, 99, 0, @"I hereby request " + m_ToPlayer.Name + " to");
			this.AddLabel(166, 223, 0, m_From.Name);
			this.AddLabel(58, 114, 0, @"assasinate " + m_Target + " within the");
			this.AddLabel(58, 179, 0, @"this contract and I will provide");
			this.AddLabel(58, 194, 0, @"your reward.");
			this.AddLabel(58, 129, 0, @"next " + m_Expiration + " day(s) for the amount of");
			this.AddLabel(58, 164, 0, @"Upon completion, return to me with");
			this.AddLabel(58, 144, 0, @m_Copper + " copper pieces.");
			this.AddLabel(13, 13, 0, @"You have been offered the following contract:");
			this.AddLabel(13, 328, 0, @"Do you choose to accept it?");
			this.AddButton(228, 328, 4005, 4006, 1, GumpButtonType.Reply, 0);
			this.AddLabel(200, 329, 0, @"Yes");
			this.AddLabel(271, 329, 0, @"No");
			this.AddButton(292, 328, 4005, 4006, 2, GumpButtonType.Reply, 0);

		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			
			if ( info.ButtonID == 1 )
			{
				m_ToPlayer.AddToBackpack( m_AContract );
				m_ToPlayer.SendMessage( "The contract of service has been completed. A copy has been placed in your backpack." );
			}
			if ( info.ButtonID == 2 )
			{
				m_AContract.Delete();
				m_From.SendMessage( m_ToPlayer.Name + " has denied your contract." );
			}
		}
	}
}
