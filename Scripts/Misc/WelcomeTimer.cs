using System;
using Server.Network;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Misc
{
	/// <summary>
	/// This timer spouts some welcome messages to a user at a set interval. It is used on character creation and login.
	/// </summary>
	public class WelcomeTimer : Timer
	{
		private Mobile m_Mobile;
		private int m_State, m_Count;

		private static string[] m_Messages = new string[]
				{
                    "Welcome to Ultima Online : Redux.",
                    "If you find yourself in need of assistance, please use the 'help' button on your character's papperdoll.",
                    "Thank you for chosing UO Aberration, we hope you enjoy your stay."
				};

		public WelcomeTimer( Mobile m ) : this( m, m_Messages.Length )
		{
		}

		public WelcomeTimer( Mobile m, int count ) : base( TimeSpan.FromSeconds( 10.0 ), TimeSpan.FromSeconds( 15.0 ) )
		{
			m_Mobile = m;
			m_Count = count;

            
		}

		protected override void OnTick()
		{
			if ( m_State < m_Count )
				m_Mobile.SendMessage( 1151, m_Messages[m_State++] );

            if (m_State == m_Count)
            {
                Stop();
            }
		}
	}
}