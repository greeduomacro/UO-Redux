using System;
using Server;
using Server.Items;
using Server.Targeting;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Commands
{
    public class WayPointHelper
    {
        WayPoint _currentWayPoint = null;
        WayPoint _previousWayPoint = null;

        public WayPoint Current
        {
            get { return _currentWayPoint; }
            set { _currentWayPoint = value; }
        }

        public WayPoint Previous
        {
            get { return _previousWayPoint; }
            set { _previousWayPoint = value; }
        }
    }

    public class WaypointGenerator
    {
        static Dictionary<PlayerMobile, WayPointHelper>
            WayPoints = new Dictionary<PlayerMobile, WayPointHelper>();

        static Dictionary<Direction, int> 
            WayPointIds = new Dictionary<Direction, int>();

        public static void Initialize()
        {   
            WayPointIds.Add(Direction.North, 7956);
            WayPointIds.Add(Direction.South, 7957);
            WayPointIds.Add(Direction.East,  7958);
            WayPointIds.Add(Direction.West,  7959);
        }

        [CommandAttribute("genwp", AccessLevel.GameMaster)]
        public static void GenerateWaypoint_OnCommand(CommandEventArgs args)
        {
            ProcessArguments(args);
        }

        static void ProcessArguments(CommandEventArgs args)
        {
            string text = args.ArgString.Trim().ToLower();
            Mobile from = args.Mobile;

            switch (text)
            {
                case "north":
                    {
                        GenerateWaypoint(Direction.North, from);
                        break;
                    }
                case "south":
                    {
                        GenerateWaypoint(Direction.South, from);
                        break;
                    }

                case "east":
                    {
                        GenerateWaypoint(Direction.East, from);
                        break;
                    }
                case "west":
                    {
                        GenerateWaypoint(Direction.West, from);
                        break;
                    }

                case "clear":
                    {
                        ClearCache(from);
                        break;
                    }

                default: { from.SendMessage("Usage: [genwp (direction) || clear"); break; }
            }
        }

        /// <summary>
        /// Removes current WayPoints stored, allowing a new series to be started.
        /// </summary>
        static void ClearCache(Mobile from)
        {
            if (from is PlayerMobile)
            {
                PlayerMobile p = from as PlayerMobile;

                if (WayPoints.ContainsKey(p))
                {
                    WayPoints[p] = null;              
                    WayPoints.Remove(p);

                    p.SendMessage
                        ("Your way point cache has been cleared. You may now begin another series.");
                }
            }
        }

        /// <summary>
        /// Generates a way point whose ItemID is based off direction and then links the previous way point cached.
        /// </summary>
        static void GenerateWaypoint(Direction direction, Mobile m)
        {
            int waypointID = -1;
            if (m is PlayerMobile)
            {
                if (!WayPoints.ContainsKey(m as PlayerMobile))
                    WayPoints.Add(m as PlayerMobile, new WayPointHelper());

                if (WayPointIds.ContainsKey(direction))
                    waypointID = WayPointIds[direction];

                if ( WayPoints[m as PlayerMobile].Current == null)
                {
                    WayPoint temp = new WayPoint();
                    temp.ItemID = waypointID;
                    temp.MoveToWorld(m.Location, m.Map);
                    WayPoints[m as PlayerMobile].Current = temp;

                    m.SendMessage("You begin a new series of way points.");
                }

                else
                {
                    WayPoint temp = new WayPoint();
                    temp.ItemID = waypointID;
                    temp.MoveToWorld(m.Location, m.Map);

                    WayPoints[m as PlayerMobile].Previous = WayPoints[m as PlayerMobile].Current;
                    WayPoints[m as PlayerMobile].Previous.NextPoint = temp;
                    WayPoints[m as PlayerMobile].Current = temp;

                    m.SendMessage("You add a new way point to the series.");
                }
            }
        }
    }
}