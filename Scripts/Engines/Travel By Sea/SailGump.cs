using System;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Currency;

namespace Khazman.TravelBySea
{
	public class SailDestinationGump : Gump
	{
		private const int fieldsPerPage = 14;
		
		private Mobile m_From;
		private Point3D m_StartPoint;
		private Point3D m_SailTo;
        private Point3D m_TravelBoat;
		
		public SailDestinationGump
            ( Mobile from, Point3D startPoint, Point3D ferryLoc, Point3D endPoint, string locName ) : base( 10, 10 )
		{
			m_From = from;
			m_StartPoint = startPoint;
            m_TravelBoat = ferryLoc;
            m_SailTo = endPoint;
			
			AddPage( 1 );
			AddBackground( 10, 10, 350, 125, 9250 );
			AddLabel( 35, 30, 0, "Would you like to sail to " + locName + "?" );
			
			AddButton( 30, 70, 5601, 5605, 1, GumpButtonType.Reply, 1 );
			AddLabel( 55, 68, 0, "Yes" );
			AddButton( 30, 95, 5601, 5605, 2, GumpButtonType.Reply, 1 );
			AddLabel( 55, 93, 0, "No" );
		}
		
		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			
			int xPoint;
			int yPoint;
			int totalDistance;
			
			Container pack = from.Backpack;

            if (pack == null) return;
			
			switch( info.ButtonID )
			{
				default:
					{
						break;
					}
                case 1:
                    {
                        xPoint = 1466 - m_StartPoint.X;
                        yPoint = 1765 - m_StartPoint.Y;
                        totalDistance = (int)Math.Sqrt((xPoint * xPoint) + (yPoint * yPoint));

                        from.SendGump(new SailConfirmGump(from, totalDistance, m_TravelBoat, m_SailTo, from.Map));

                        break;
                    }
			}
		}
	}
	
	public class SailConfirmGump : Gump
	{
		int m_Cost;
		Point3D m_SendTo;
		Point3D m_SailTo;
		Map m_SailMap;
		
		public SailConfirmGump( Mobile from, int cost, Point3D sendTo, Point3D sailTo, Map map ) : base( 10, 10 )
		{
			m_Cost = CalcCost( cost );
			m_SendTo = sendTo;
			m_SailTo = sailTo;
			m_SailMap = map;
			
			AddPage( 0 );
			AddBackground( 10, 10, 335, 255, 9250 );
			AddImageTiled( 25, 25, 305, 25, 3004 );
			AddLabel( 55, 27, 0, String.Format( "Sailing there will require {0} copper coins.", m_Cost ) );
			
			AddButton( 30, 85, 5601, 5605, 1, GumpButtonType.Reply, 1 );
			AddLabel( 55, 83, 0, "Pay the fee" );
			AddButton( 30, 120, 5601, 5605, 2, GumpButtonType.Reply, 1 );
			AddLabel( 55, 118, 0, "Present a membership card" );
			AddButton( 30, 155, 5601, 5605, 3, GumpButtonType.Reply, 1 );
			AddLabel( 55, 153, 0, "Cancel trip" );
		}

		private static int CalcCost( int startCost )
		{
            startCost = (int)( startCost / 10 );
				return startCost;
		}
		
		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			Container pack = from.Backpack;

            if (pack == null) return;
			
			SailTimer waitTime;
			int travelTime = 5;
			
			switch( info.ButtonID )
			{
				default:
					{
						from.SendMessage( "Another time, perhaps." );
						break;
					}
				case 1:
					{
						if( pack.ConsumeTotal( typeof( Gold ), m_Cost ) )
						{
							if( from.Map != m_SailMap )
								travelTime += 30;

							travelTime += (int)(m_Cost / Math.PI);

							if( travelTime > 90 && from.Map == m_SailMap )
								travelTime = Utility.RandomMinMax( 75, 90 );
							else if( travelTime > 120 && from.Map != m_SailMap )
								travelTime = Utility.RandomMinMax( 110, 120 );



                            foreach (Mobile m in from.GetMobilesInRange(3))
                            {
                                if (m is BaseCreature)
                                {
                                    BaseCreature pet = (BaseCreature)m;

                                    if (pet.Controlled && pet.ControlMaster == from)
                                    {
                                        pet.Blessed = true;
                                    }
                                }
                            }

							BaseCreature.TeleportPets( from, m_SendTo, from.Map );
                            from.Location = m_SendTo;
                            from.Blessed = true;

							waitTime = new SailTimer( from, m_SailTo, m_SailMap, TimeSpan.FromSeconds( travelTime ) );
							waitTime.Start();

							from.AddToBackpack( new SailTimerCheck() );
						}
						else
						{
							from.SendMessage( "Please come back with the fare price in copper." );
						}
						
						break;
					}
				case 2:
					{
						if( pack.ConsumeTotal( typeof( SailingMembershipCard ), 0 ) )
						{
							if( from.Map != m_SailMap )
								travelTime += 30;

							travelTime += (m_Cost / 15);

							if( travelTime > 90 && from.Map == m_SailMap )
								travelTime = Utility.RandomMinMax( 75, 90 );
							else if( travelTime > 120 && from.Map != m_SailMap )
								travelTime = Utility.RandomMinMax( 110, 120 );

                            foreach (Mobile m in from.GetMobilesInRange(3))
                            {
                                if (m is BaseCreature)
                                {
                                    BaseCreature pet = (BaseCreature)m;

                                    if (pet.Controlled && pet.ControlMaster == from)
                                    {
                                        pet.Blessed = true;
                                    }
                                }
                            }

							BaseCreature.TeleportPets( from, m_SendTo, from.Map );

                            from.Location = m_SendTo;
                            from.Blessed = true;
							
							waitTime = new SailTimer( from, m_SailTo, m_SailMap, TimeSpan.FromSeconds( travelTime ) );
							waitTime.Start();

							from.AddToBackpack( new SailTimerCheck() );
						}
						else
						{
							from.SendMessage( "That's not a membership card!" );
						}
						
						break;
					}
				case 3:
					{
						from.SendMessage( "Another time, perhaps." );
						break;
					}
			}
		}
	}
	
	public class SailTimer : Timer
	{
		Mobile m_From;
		Point3D m_Destination;
		Map m_DestMap;
		
		public SailTimer( Mobile from, Point3D destination, Map destMap, TimeSpan duration ) : base( duration )
		{
			m_From = from;
			m_Destination = destination;
			m_DestMap = destMap;
			
			Priority = TimerPriority.OneSecond;
		}
		
		protected override void OnTick()
		{
			if( m_From.Backpack.ConsumeTotal( typeof( SailTimerCheck ), 1 ) )
			{
                foreach (Mobile m in m_From.GetMobilesInRange(3))
                {
                    if (m is BaseCreature)
                    {
                        BaseCreature pet = (BaseCreature)m;

                        if (pet.Controlled && pet.ControlMaster == m_From)
                        {
                            pet.Blessed = false;
                        }
                    }
                }

				BaseCreature.TeleportPets( m_From, m_Destination, m_DestMap );

				m_From.Location = m_Destination;
				m_From.Map = m_DestMap;

                m_From.Blessed = false;

                if (m_From.Mount != null && m_From.Mount is BaseCreature)
                    ((BaseCreature)m_From.Mount).Blessed = false;
			
				m_From.SendMessage( "I hope you enjoyed the trip!" );
			}

			Stop();
		}
	}

	public class SailTimerCheck : Item
	{
		[Constructable]
		public SailTimerCheck() : base( 0x14F4 )
		{
			Name = "Do not delete!";
			Visible = false;
			Movable = false;
			Weight = 0.0;
		}

		public SailTimerCheck( Serial serial ) : base( serial )
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