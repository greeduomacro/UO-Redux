using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Server.Utilities
{
    public class StartupSettings
    {
        public static readonly bool LightingChanged = false;

        private static readonly DayOfWeek TreasureRandomizationDay = DayOfWeek.Sunday;

        [CallPriority(1000)]
        public static void Initialize()
        {
            EventSink.Logout += new LogoutEventHandler(HandleOnLogout);

            Console.Write("Startup: Performing maintenance routines...");
            Stopwatch sw = new Stopwatch();
            sw.Start();

            CloseDoors();
            ProcessMobiles();

            CleanBackups();

            sw.Stop();
            Console.WriteLine("done ({0:F2} second{1})", sw.Elapsed.TotalSeconds, ((int)sw.Elapsed.TotalSeconds == 1 ? "" : "s"));

            if(DateTime.Now.DayOfWeek == TreasureRandomizationDay)
                RandomizeTreasureLocations();
        }

        #region -static void CloseDoors()
        private static void CloseDoors()
        {
            foreach(Item i in World.Items.Values)
            {
                if(i is BaseDoor)
                    ((BaseDoor)i).Open = false;
            }
        }
        #endregion

        #region -static void HandleOnLogout( LogoutEventArgs )
        private static void HandleOnLogout(LogoutEventArgs args)
        {
            Mobile m = args.Mobile;

            try
            {
                StaticTile[] tiles = m.Map.Tiles.GetStaticTiles(m.X, m.Y, true);
                NetState state = (NetState)m.NetState;
                int tileID = 0;
                bool safe = false;

                int[] bedID = new int[]
				{
					2651,	2653,	2654,	2656,
					2659,	2660,	2662,	2663,
					2665,	2666,	2682,	2684,
					2688,	2690,	2692,	2696,
					2702,	2704
				};

                foreach(Item item in m.GetItemsInRange(6))
                {
                    for(int i = 0; !safe && i < bedID.Length; i++)
                    {
                        for(int j = 0; !safe && j < tiles.Length; j++)
                        {
                            tileID = tiles[j].ID;

                            if(tiles[j].Z == m.Z)
                                safe = (tileID == bedID[i]) || (item.ItemID == bedID[i]);
                        }
                    }
                }

                if(safe)
                {
                    ((PlayerMobile)m).BedrollLogout = true;

                    if(state != null)
                        state.Dispose();
                }
            }
            catch(Exception e)
            {
                ExceptionManager.LogException("StartupSettings.cs", e);
            }
        }
        #endregion

        private static void ProcessMobiles()
        {
            foreach(Mobile m in World.Mobiles.Values)
            {
                if(m.Player)
                {
                    Player pl = m as Player;

                    m.Hunger = 20;
                    m.Thirst = 20;

                    if(pl.EoCLedger == null)
                        pl.EoCLedger = new EssenceOfCharacter.EoCLedger(pl);

                    if(pl.RespawnLocation == Point3D.Zero)
                        pl.RespawnLocation = CharacterCreation.StartingCity.Location;

                    if(pl.RespawnMap == null || pl.RespawnMap == Map.Internal)
                        pl.RespawnMap = CharacterCreation.StartingCity.Map;
                }
            }
        }

        public static void CleanBackups()
        {
            try
            {
                if(Directory.Exists("Backups"))
                    DeleteOldBackups("Backups");
            }
            catch
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.Write("[failed to clean Backups directory!] ");
                Utility.PopColor();
            }
        }

        #region +static void DeleteOldBackups( string )
        public static void DeleteOldBackups(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            string[] subDirs = Directory.GetDirectories(path);

            foreach(string dir in subDirs)
                DeleteOldBackups(dir);

            if(info.Name != "Backups" && info.CreationTime < (DateTime.Now.Subtract(TimeSpan.FromDays(5.0))))
            {
                Directory.Delete(path, true);
            }
        }
        #endregion

        private static void RandomizeTreasureLocations()
        {
            Map map = Map.Felucca;
            Rectangle2D[] TreasureBounds = new Rectangle2D[]
				{
					new Rectangle2D( 4312, 1148, 428, 920 ), new Rectangle2D( 3204, 1066, 716, 396 ),
					new Rectangle2D( 2376, 462, 300, 656 ), new Rectangle2D( 2378, 1738, 206, 76 ),
					new Rectangle2D( 2659, 2458, 940, 221 ), new Rectangle2D( 3576, 2409, 942, 233 ),
					new Rectangle2D( 1889, 2525, 517, 189 ), new Rectangle2D( 1555, 1757, 619, 419 ),
					new Rectangle2D( 554, 50, 1222, 1406 ), new Rectangle2D( 16, 2432, 732, 578 )
				};

            Console.Write("StartupSettings.RandomizeTreasureLocations(): Randomizing locations...");
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                using(StreamWriter writer = new StreamWriter("Data/treasure.cfg", false))
                {
                    List<IPoint2D> createdPoints = new List<IPoint2D>();
                    IPoint2D currPoint = Point2D.Zero;
                    Rectangle2D rect;
                    Region currReg;

                    for(int i = 0, x = 0, y = 0; i < 400; i++)
                    {
                        rect = TreasureBounds[Utility.Random(TreasureBounds.Length)];
                        x = Utility.Random(rect.X, rect.Width);
                        y = Utility.Random(rect.Y, rect.Height);
                        currPoint = new Point2D(x, y);
                        currReg = Region.Find(new Point3D(currPoint, map.GetAverageZ(x, y)), map);

                        if(Server.Multis.BaseHouse.FindHouseAt(new Point3D(x, y, map.GetAverageZ(x, y)), map, 16) != null)
                            continue;
                        else if(currReg != null && (!currReg.AllowSpawn() || currReg is Server.Regions.GuardedRegion))
                            continue;
                        else if(createdPoints.Contains(currPoint) || !map.CanSpawnMobile((Point2D)currPoint, map.GetAverageZ(x, y)))
                            continue;

                        createdPoints.Add(currPoint);

                        writer.WriteLine("{0} {1}", x, y);
                        writer.Flush();
                    }

                    createdPoints.Clear();
                    createdPoints = null;

                    writer.Close();
                }

                Console.Write("done");
            }
            catch(Exception e)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.Write("failed!");
                Utility.PopColor();

                Server.Utilities.ExceptionManager.LogException("StartupSettings.cs", e);
            }
            finally
            {
                sw.Stop();
                Console.WriteLine(" ({0:F2} second{1})", (sw.ElapsedMilliseconds / 1000), ((sw.ElapsedMilliseconds / 1000) == 1 ? "" : "s"));
            }
        }
    }
}