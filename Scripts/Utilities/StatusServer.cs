using Server.Guilds;
using Server.Misc;
using Server.Network;
using Server.Regions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Server.Utilities
{
    public class StatusServer
    {
        public static readonly bool Enabled = false;

        private static string RegionColorDescriptor = @"Data\RegionColors.xml";
        private static string DescriptorRootElementName = "regionColors";
        private static string DescriptorChildTagName = "region";

        public static void Initialize()
        {
            if(Enabled)
                PacketHandlers.Register(0x52, 2, false, new OnPacketReceive(statusServer_packetReceived));
        }

        private static void statusServer_packetReceived(NetState state, PacketReader reader)
        {
            string status = BuildStatus();

            int currBufferSize = SendQueue.CoalesceBufferSize;
            SendQueue.CoalesceBufferSize = Math.Max((status.Length + 2), currBufferSize);

            state.Send(new StatusInfo(status));

            SendQueue.CoalesceBufferSize = currBufferSize;
        }

        #region +static string BuildStatus()
        public static string BuildStatus()
        {
            StringBuilder resultsBuilder = new StringBuilder("<status>");
            List<NetState> userList = new List<NetState>(NetState.Instances);
            userList.Sort(NetStateComparer.Instance);

            Mobile m;
            int serial;
            string name, guild, accessLevel, mapName, fullName, profile;

            for(int i = 0; i < userList.Count; i++)
            {
                m = userList[i].Mobile;
                name = guild = accessLevel = mapName = fullName = profile = "";

                if(m == null)
                    continue;

                serial = m.Serial;
                name = Encode(m.RawName);

                if(((Guild)m.Guild) != null)
                    guild = Encode(((Guild)m.Guild).Abbreviation);

                if(m.AccessLevel > AccessLevel.Player)
                    accessLevel = Encode(m.AccessLevel.ToString());

                if(IsHidden(m))
                {
                    mapName = "unknown";
                }
                else
                {
                    Region reg = Region.Find(m.Location, m.Map);

                    if(reg != null)
                    {
                        if(reg.Name != null && reg.Name != "world")
                            mapName = Encode(Util.SplitString(reg.Name));
                        else if(reg is HouseRegion && Server.Multis.BaseHouse.FindHouseAt(m) != null)
                            mapName = "inside a house";
                    }
                }

                if(String.IsNullOrEmpty(mapName))
                    mapName = Encode(String.Format("somewhere in {0}", m.Map.Name));

                fullName = Encode(String.Format("{0}\n{1}", Titles.GetNameTitle(m, m), Titles.GetSkillTitle(m)));
                profile = Encode(m.Profile);

                resultsBuilder.AppendFormat("<mobile name='{0}' guild='{1}' access='{2}' map='{3}' fullName='{4}' profile='{5}' serial='{6}'/>",
                    name, guild, accessLevel, mapName, fullName, profile, serial);
            }

            userList.Clear();

            Dictionary<string, uint> colorList = LoadRegionColors();
            List<RegionWatcherObject> regionWatchList = RegionPopularityWatcher.Load(false);
            RegionWatcherObject obj;

            regionWatchList.Sort((x, y) => GetRegionColor(x, colorList).CompareTo(GetRegionColor(y, colorList)));

            for(int i = 0; i < regionWatchList.Count; i++)
            {
                obj = regionWatchList[i];

                resultsBuilder.AppendFormat("<region name='{0}' map='{1}' popularity='{2}' color='{3}'/>",
                    Encode(Util.SplitString(obj.Name)), Encode(obj.Map.Name), obj.PopularityPercent, GetRegionColor(obj, colorList));
            }

            string uptime = Util.FormatLongTimeSpan(DateTime.Now - Server.Items.Clock.ServerStart);
            string ramUsage = Util.FormatByteAmount(System.Diagnostics.Process.GetCurrentProcess().WorkingSet64);
            string cycles = Core.AverageCPS.ToString("N2");

            resultsBuilder.AppendFormat("<stats uptime='{0}' ram='{1}' cycles='{2}'/>", uptime, ramUsage, cycles);
            resultsBuilder.Append("</status>");

            return resultsBuilder.ToString();
        }
        #endregion

        #region -static string Encode( string )
        private static string Encode(string input)
        {
            StringBuilder sb = new StringBuilder(input);

            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace("\"", "&quot;");
            sb.Replace("'", "&apos;");

            return sb.ToString();
        }
        #endregion

        #region -static bool IsHidden( Mobile )
        private static bool IsHidden(Mobile m)
        {
            return false;
        }
        #endregion

        #region +static Dictionary<string, uint> LoadRegionColors()
        private static Dictionary<string, uint> LoadRegionColors()
        {
            Dictionary<string, uint> table = new Dictionary<string, uint>();

            if(File.Exists(RegionColorDescriptor))
            {
                XmlDocument doc = new XmlDocument();
                XmlElement root;

                try
                {
                    doc.Load(RegionColorDescriptor);
                    root = doc[DescriptorRootElementName];

                    table.Add("default", UInt32.Parse(root.GetAttribute("defaultColor"), NumberStyles.HexNumber));

                    foreach(XmlElement node in root.GetElementsByTagName(DescriptorChildTagName))
                    {
                        table.Add(node.GetAttribute("name"), UInt32.Parse(node.GetAttribute("color"), NumberStyles.HexNumber));
                    }
                }
                catch(Exception e)
                {
                    ExceptionManager.LogException("StatusServer", e);
                }
            }

            return table;
        }
        #endregion

        #region static uint GetRegionColor( RegionWatcherObject, Dictionary<string, uint> )
        private static uint GetRegionColor(RegionWatcherObject obj, Dictionary<string, uint> colorList)
        {
            string name = obj.Name.Replace(" ", "");
            uint color = colorList["default"];

            if(colorList.ContainsKey(name))
                color = colorList[name];

            return color;
        }
        #endregion
    }

    public sealed class StatusInfo : Packet
    {
        public StatusInfo(string xmlResults)
            : base(0x52, 4 + xmlResults.Length + 2)
        {
            m_Stream.Write(4 + (int)(xmlResults.Length + 2));
            m_Stream.WriteAsciiNull(xmlResults);
        }
    }
}