using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Server;
using Server.Misc;
using Server.Network;

namespace Server
{
	public class SocketOptions
	{
		private const bool NagleEnabled = false; // Should the Nagle algorithm be enabled? This may reduce performance
		private const int CoalesceBufferSize = 512; // MSS that the core will use when buffering packets

		private static IPEndPoint[] m_ListenerEndPoints = new IPEndPoint[]
			{
				new IPEndPoint( IPAddress.Any, 2600 ),
                new IPEndPoint( IPAddress.Any, Utility.RandomMinMax(2591,2599)) // Faster Testing On Same Machine
			};

		public static void Initialize()
		{
			SendQueue.CoalesceBufferSize = CoalesceBufferSize;

			EventSink.SocketConnect += new SocketConnectEventHandler( EventSink_SocketConnect );

			Listener.EndPoints = m_ListenerEndPoints;
		}

		private static void EventSink_SocketConnect( SocketConnectEventArgs e )
		{
			if( !e.AllowConnection )
				return;

			if( !NagleEnabled )
				e.Socket.SetSocketOption( SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1 ); // RunUO uses its own algorithm
		}
	}
}