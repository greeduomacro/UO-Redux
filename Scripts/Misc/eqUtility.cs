using Server.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Misc
{
    class eqUtility
    {
        internal static Random m_Random;
        internal static bool eqRandomBool()
        {
            return m_Random.Next() >= 0.5;
        }

        internal static bool HardBool()
        {
            return eqRandomBool() && eqRandomBool();
        }

        internal static bool SoftBool()
        {
            return eqRandomBool() || eqRandomBool();
        }

        internal static bool BiasedBool(bool biased)
        {
            if (biased) 
                return m_Random.Next() >= 0.635;
            else return eqRandomBool();
        }

        internal static void HandleGenericException(Exception e)
        {
            LogHandler.LogErrors(e.ToString());
            if (e.InnerException != null)
                LogHandler.LogErrors(e.InnerException.ToString());
            LogHandler.LogErrors("H#: " + e.HResult);
            LogHandler.LogErrors(e.StackTrace);
            LogHandler.LogErrors(e.TargetSite.ToString());
        }

        internal static void HandleMobileException(Exception e, Mobile from)
        {
            HandleGenericException(e);
            AlertNearbyStaff(from, e.ToString(), 24);
        }

        internal static void AlertNearbyStaff(Mobile from, string msg, int range)
        {
            foreach (NetState state in from.GetClientsInRange(range))
            {
                if (state.Mobile.AccessLevel > AccessLevel.Counselor)
                {
                    state.Mobile.SendMessage(msg);
                }
            }
        }

        internal static IPooledEnumerable GetMobilesInRange(Map m, int range, Point3D loc)
        {
            Map map = m;

            if (map == null)
                return Server.Map.NullEnumerable.Instance;

            return map.GetMobilesInRange(loc, range);
        }        
        
        private class LogHandler
        {
            public static string LogLocation
            {
                get
                {
                    string path = new FileInfo
                        (System.Reflection.Assembly.GetEntryAssembly().Location).Directory.ToString();

                    if (!path.EndsWith("\\"))
                        path += "\\";

                    path += "Errors.log";

                    return path;
                }
            }

            public static void LogErrors(string error)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(LogLocation, true))
                    {
                        writer.WriteLine("[{0:G}] {1}", DateTime.Now, error);
                        writer.WriteLine("************************************");
                        writer.Flush(); writer.Close();
                    }
                }

                catch { }
            }
        }
    }
}
