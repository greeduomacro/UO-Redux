//By using this software you are agreeing to leaving this header in-tact.
//Implicit Copyright © 2014 - Under no circumstances may you redistribute this software.

using System;
using Server;
using Server.Guilds;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Misc;
using Server.Commands;
using Server.Targeting;
using System.IO;
using System.Xml;
using System.Text;
using Server.Accounting;
using System.Globalization;
using Server.Regions;
using Server.Items;
using Server.Spells;
using Server.ContextMenus;
using Server.Currency;
using Ulmeta.Guards;
using Ulmeta.Factions.Vendors;

#region readme
//Add to PlayerMobile.cs
//using Ulmeta.Factions

//        public InteractionState iState;

//        private PlayerFaction currentFaction;

 //       public PlayerFaction CurrentFaction
 //       {
 //           set { currentFaction = value; }

 //           get { return currentFaction; }
 //       }

 //       [CommandProperty(AccessLevel.GameMaster)]
 //       public String FactionName
 //       {      
 //           get 
 //           {
 //               if (currentFaction != null)
 //                   return currentFaction.FactionName;

 //               else return null;
 //           }
 //       }

// Add to PlayerMobile public override void GetProperties(ObjectPropertyList list)

            //if (iState != null)
            //{
            //    string color = "";

            //    switch (iState.playerRank)
            //    {
            //        case Rank.Private: color = "#5A5ABA"; break;
            //        case Rank.Corporal: color = "#5A5ABA"; break;
            //        case Rank.Seargent: color = "#C10000"; break;
            //        case Rank.Captain: color = "#C10000"; break;
            //        case Rank.Major: color = "#006A00"; break;
            //        case Rank.Colonel: color = "#006A00"; break;
            //        case Rank.General: color = "#010198"; break;
            //    }

            //    list.Add(1060658, "{0}\t{1}", "Rank", String.Format("<BASEFONT COLOR={0}>{1}", color, iState.playerRank.ToString()));
            //}

//  Add to Notoriety.cs MobileNotoriety

//using Ulmeta.Factions;
//using Ulmeta.Factions.Guards;
//Add to Notoriety.MobileNotoriety( Mobile source, Mobile target )

//            if((source is PlayerMobile || source is Guard) && (target is PlayerMobile || target is Guard))
//            {
                   
//                BaseFaction sourceFaction = (source is PlayerMobile ? ((PlayerMobile)source).CurrentFaction : ((Guard)source).Faction);
//                BaseFaction targetFaction = (source is PlayerMobile ? ((PlayerMobile)source).CurrentFaction : ((Guard)source).Faction);

//                if(sourceFaction != null && targetFaction != null)
//                {
//                    if (sourceFaction == targetFaction)
//                    {
//                        return Notoriety.Ally;
//                    }

//                    if (sourceFaction != targetFaction || targetFaction != sourceFaction)
//                    {
//                        return Notoriety.Enemy;
//                    }
//                }
//            }

// Add to Guilds.cs
// Add to Guilds.AddMember(Mobile m) at the bottom of the if statement.
// FactionHandler.AddMember(((PlayerMobile)Leader).CurrentFaction, m);

// Add to Guilds.RemoveMember(Mobile m)

// FactionHandler.RemoveMember(m);

#endregion

namespace Ulmeta.Factions
{
    public enum Rank
    {
        Private,
        Corporal,
        Seargent,
        Captain,
        Major,
        Colonel,
        General
    }

    public interface  IFactionEntity
    {
        PlayerFaction Faction { get; set; }

        String FactionName { get; set; }
    }

    public class       PlayerFaction
    {
        string serialString;

        Serial serial;

        public List<Guild> ChildGuilds = new List<Guild>();
        public List<Mobile> MembersOf = new List<Mobile>();

        public List<InteractionState> iStates = new List<InteractionState>();
        public List<Sigillum> SigilsControlled = new List<Sigillum>();

        public List<string> MemberSerials = new List<string>();

        string factionName;

        PlayerMobile factionLeader;
        PlayerMobile factionSheriff;
        PlayerMobile factionMinister;

        public string leaderSerial;
        public string sheriffSerial;
        public string ministerSerial;

        public int treasuryBalance = 0;
        public int ministerBalance = 0;
        public int sheriffBalance = 0;

        public int MilitaryLevel = 1;
        public int EconomyLevel = 1;
        public int guardCount = 0;
        public int vendorCount = 0;

        public int primaryHue = 0;
        public int secondaryHue = 0;
        public int mountBody = 0;
        public int mountID = 0;
        public int emblemID = 0;

        public int SigilCount
        {
            get { return SigilsControlled.Count; }
            set { }
        }

        public int GuardLimit 
        {
            get { return  SigilCount * (8 * MilitaryLevel) + 1; }
        }

        public int VendorLimit
        {
            get { return 4 * (EconomyLevel + 1) * SigilCount; }
        }

        public int TotalCreditsEarned
        {
            get
            {
                int total = 0;

                for (int x = 0; x < MembersOf.Count; x++)
                {
                    if (MembersOf[x] is PlayerMobile)
                    {
                        PlayerMobile p = MembersOf[x] as PlayerMobile;
                        total += p.iState.Kills;
                    }
                }

                return total;
            }
        }

        public int CollectivePoints
        {
            get
            {
                int total = 0;

                for (int i = 0; i < MembersOf.Count; i++ )
                {
                    if (MembersOf[i] is PlayerMobile)
                    {
                        PlayerMobile p = MembersOf[i] as PlayerMobile;
                        total += p.iState.Kills;
                    }
                }

                return total;
            }
        }

        public PlayerMobile FactionSheriff
        {
            get { return factionSheriff; }
            set { factionSheriff = value; }
        }

        public PlayerMobile FactionMinister
        {
            get { return factionMinister; }
            set { factionMinister = value; }
        }

        public PlayerMobile FactionLeader
        {
            get { return factionLeader; }
            set { factionLeader = value; }
        }

        public String FactionName
        {
            get { return factionName; }
            set { factionName = value; }
        }

        public Serial Serial
        {
            get { return serial; }
            set { serial = value; }
        }

        public string SerialString
        {
            get { return serialString; }
            set { serialString = value; }
        }

        //Oh yeah?
        //~PlayerFaction()
        //{
        //    Console.WriteLine("The Faction '{0}' has been disbanded.", factionName);
        //}

        public void Serialize(BinaryFileWriter writer)
        {
            writer.Write((int)0); //Version

            writer.Write((string)serialString.ToString());
            writer.WriteMobile(factionLeader);

            writer.Write((int)MembersOf.Count);

            foreach (Mobile m in MembersOf)
            {
                writer.WriteMobile(m);
            }

            writer.Write((int)ChildGuilds.Count);

            foreach (Guild g in ChildGuilds)
            {
                writer.WriteGuild(g);
            }

            writer.Write((string)factionName);
            writer.Write((int)primaryHue);
            writer.Write((int)secondaryHue);
            writer.Write((int)mountBody);
            writer.Write((int)mountID);
            writer.Write((int)EconomyLevel);
            writer.Write((int)MilitaryLevel);
            writer.Write((int)treasuryBalance);
            writer.Write((int)sheriffBalance);
            writer.Write((int)ministerBalance);

            writer.Write((int)iStates.Count);

            foreach (InteractionState a in iStates)
            {
                a.Serialize(writer);
            }
        }

        public void Deserialize(BinaryFileReader reader)
        {
            int version = reader.ReadInt();

            if (version >= 0)
            {
                serialString = reader.ReadString();

                factionLeader = reader.ReadMobile() as PlayerMobile;

                int count = reader.ReadInt();

                for (int n = 1; n <= count; n++)
                {
                    MembersOf.Add(reader.ReadMobile());
                }

                int guildCount = reader.ReadInt();

                for (int x = 1; x <= guildCount; x++)
                {
                    ChildGuilds.Add(reader.ReadGuild() as Guild);
                }

                factionName = reader.ReadString();
                primaryHue = reader.ReadInt();
                secondaryHue = reader.ReadInt();
                mountBody = reader.ReadInt();
                mountID = reader.ReadInt();
                EconomyLevel = reader.ReadInt();
                MilitaryLevel = reader.ReadInt();
                treasuryBalance = reader.ReadInt();
                sheriffBalance = reader.ReadInt();
                ministerBalance = reader.ReadInt();

                foreach (Mobile m in MembersOf)
                {
                    if (m is PlayerMobile)
                    {
                        PlayerMobile p = m as PlayerMobile;
                        p.CurrentFaction = this;
                    }
                }

                int stateCount = reader.ReadInt();

                for (int z = 0; z < stateCount; z++)
                {
                    InteractionState iState = new InteractionState();
                    iState.Deserialize(reader);
                    iStates.Add(iState);
                }
            }
        }

        public void Add(Mobile m)
        {
            if (m is PlayerMobile && ((PlayerMobile)m).CurrentFaction == null)
            {
                ((PlayerMobile)m).CurrentFaction = this;
                MembersOf.Add(m);
                ((PlayerMobile)m).ValidateEquipment();
            }
        }

        public void AssimilateGuild(Mobile m)
        {
            if (m.Guild is Guild)
            {
                Guild g = m.Guild as Guild;

                ChildGuilds.Add(g);

                foreach (Mobile mob in g.Members)
                {
                    if (m != mob && m is PlayerMobile && ((PlayerMobile)m).CurrentFaction == null)
                    {
                        MembersOf.Add(m);

                        InteractionState aState = new InteractionState();

                        if (m is PlayerMobile)
                        {
                            ((PlayerMobile)m).CurrentFaction = this;
                            ((PlayerMobile)m).iState = aState;
                            ((PlayerMobile)m).iState.Player = m as PlayerMobile;
                            ((PlayerMobile)m).CurrentFaction.iStates.Add(aState);
                        }

                        m.SendMessage("Your guild has joined a faction, {0}.", this.FactionName);
                    }
                }
            }
        }

        public void QueryMember(Mobile m)
        {
            if (MemberSerials.Count > 0)
            {
                foreach (string s in MemberSerials)
                {
                    if (m.Serial.ToString() == s && m is PlayerMobile && ((PlayerMobile)m).CurrentFaction == null)
                    {
                        MembersOf.Add(m);

                        ((PlayerMobile)m).CurrentFaction = this;
                    }
                }

                if (m.Serial.ToString() == leaderSerial && m is PlayerMobile)
                    FactionLeader = m as PlayerMobile;

                if (m.Serial.ToString() == ministerSerial && m is PlayerMobile)
                    FactionMinister = m as PlayerMobile;

                if (m.Serial.ToString() == sheriffSerial && m is PlayerMobile)
                    factionSheriff = m as PlayerMobile;

                AssimilateGuild(m);
            }
        }

        public void Sanitize()
        {
            foreach (PlayerMobile p in MembersOf)
            {
                if (p is PlayerMobile)
                {
                    ((PlayerMobile)p).CurrentFaction = null;
                    ((PlayerMobile)p).iState = null;
                    p.SendMessage("The faction you claim fealty to has been dissolved.");
                }
            }

            ChildGuilds.Clear();
            MembersOf.Clear();
            iStates.Clear();
            SigilsControlled.Clear();

            FactionDefinition.Factions.Remove(this);
        }
    }    

    public class   FactionDefinition
    {
        public static List<PlayerFaction> Factions = new List<PlayerFaction>();

        private static readonly string dataPath = "Data";
        private static readonly string factionXML = Path.Combine(dataPath, "factions.xml");

        private static readonly string SavePath = "Saves\\Factions";
        private static readonly string SaveFile = Path.Combine(SavePath, "factions.bin");

        public static bool useXML = false; //Development Purposes
        public static int FactionLimit = 11; //Not recommended for more than 100 factions.

        public static readonly int FactionCost = 1000000;

        public static void Configure()
        {
            EventSink.WorldLoad += new WorldLoadEventHandler(Event_WorldLoad);
        }

        public static void Initialize()
        {
            if (useXML)
            {
                Console.Write("Factions: Constructing from {0}... ", factionXML);
                ConstructFactions();
            }

            EventSink.WorldSave += new WorldSaveEventHandler(Event_WorldSave);
        }

        private static void ConstructXML()
        {
            if (Factions.Count > 0)
            {
                if (!Directory.Exists(dataPath))
                {
                    try { Directory.CreateDirectory(dataPath); }

                    catch { Console.WriteLine("Error: Unable to create new directory."); }
                }

                try
                {
                    using (StreamWriter writer = new StreamWriter(factionXML))
                    {
                        XmlTextWriter xml = new XmlTextWriter(writer);

                        xml.Formatting = Formatting.Indented;
                        xml.IndentChar = '\t';
                        xml.Indentation = 1;

                        xml.WriteStartDocument(true);
                        xml.WriteStartElement("Factions");
                        xml.WriteAttributeString("limit", FactionLimit.ToString());

                        foreach (PlayerFaction a in Factions)
                        {
                            xml.WriteStartElement("faction");

                            xml.WriteAttributeString("serial", a.Serial.ToString());
                            xml.WriteAttributeString("name", a.FactionName.ToString());
                            xml.WriteAttributeString("leader", a.FactionLeader.Serial.ToString());

                            if (a.FactionSheriff != null)
                                xml.WriteAttributeString("general", a.FactionSheriff.Serial.ToString());

                            if (a.FactionMinister != null)
                                xml.WriteAttributeString("magistrate", a.FactionMinister.Serial.ToString());

                            if (a.primaryHue != 0)
                                xml.WriteAttributeString("hue", a.primaryHue.ToString());

                            if (a.mountID != 0)
                                xml.WriteAttributeString("mountID", a.mountID.ToString());

                            if (a.MembersOf.Count >= 1)
                            {
                                StringBuilder builder = new StringBuilder();

                                foreach (Mobile m in a.MembersOf)
                                {
                                    builder.Append(m.Serial.ToString()).Append(", ");
                                }

                                string memberList = builder.ToString();
                                memberList = memberList.Remove(memberList.Length - 2);

                                builder.Clear();

                                xml.WriteString(memberList);
                            }

                            xml.WriteEndElement();
                        }

                        xml.WriteEndElement();
                        xml.WriteEndDocument();
                        xml.Close();
                    }
                }

                catch
                {             
                    Console.WriteLine("Error: Unable to create new xml file.");
                }
            }
        }  

        private static void Event_WorldSave(WorldSaveEventArgs args)
        {
            try
            {
                if (!Directory.Exists(SavePath))
                    Directory.CreateDirectory(SavePath);

                BinaryFileWriter writer = new BinaryFileWriter(SaveFile, true);

                Serialize(writer);

                ConstructXML();

                writer.Close();
            }

            catch(ArgumentException e) 
            { 
                Console.WriteLine("Error: Event_WorldSave Failed in Faction Definition...");
                Console.WriteLine(e.Message);
            }
        }

        private static void Event_WorldLoad()
        {
            if (!File.Exists(SaveFile) || useXML)
                return;

            try
            {
                using (FileStream stream = new FileStream(SaveFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryFileReader reader = new BinaryFileReader(new BinaryReader(stream));
                    Deserialize(reader);
                    reader.Close();
                }

                FactionHandler.OnWorldLoad();
            }

            catch (ArgumentException e)
            { 
                Console.WriteLine("Error: Event_WorldLoad Failed in Faction Definition.");
                Console.WriteLine(e.Message);       
            }
        }

        private static void ConstructFactions()
        {
            if (!File.Exists(factionXML))
                return;

            XmlDocument doc = new XmlDocument();
            XmlElement root;

            try
            {
                doc.Load(factionXML);
                root = doc["Factions"];

                FactionLimit = Convert.ToInt32(root.GetAttribute("limit"));

                foreach (XmlElement ele in root.GetElementsByTagName("faction"))
                {
                    PlayerFaction faction = new PlayerFaction();

                    faction.FactionName = ele.Attributes["name"].Value;
                    faction.SerialString = ele.Attributes["serial"].Value;
                    faction.leaderSerial = ele.Attributes["leader"].Value;

                    string serials = ele.InnerText.ToString();

                    string[] serialStrings = serials.Split(',');

                    foreach (string s in serialStrings)
                    {
                        string serial = s.Trim();

                        faction.MemberSerials.Add(serial);
                    }

                    Factions.Add(faction);
                }
            }

            catch { }
        }

        public static void Serialize(BinaryFileWriter writer)
        {
            writer.Write((int)0); //Version

            writer.Write((int)FactionLimit);
            writer.Write((bool)useXML);
            writer.Write((int)Factions.Count);

            foreach (PlayerFaction a in Factions)
            {
                a.Serialize(writer);
            }
        }

        public static void Deserialize(BinaryFileReader reader)
        {
            int version = reader.ReadInt();

            if (version >= 0)
            {
                FactionLimit = reader.ReadInt();
                useXML = reader.ReadBool();
                int count = reader.ReadInt();

                for (int i = 1; i <= count; i++)
                {
                    PlayerFaction faction = new PlayerFaction();
                    faction.Deserialize(reader);
                    Factions.Add(faction);
                }
            }
        }

 

    }

    public class      FactionHandler
    {
        private static readonly string dataPath = "Data";
        private static readonly string RevenueOutput = Path.Combine(dataPath, "revenue.txt");

        public static void Initialize()
        {
            EventSink.Speech += new SpeechEventHandler(HookSpeech);
            EventSink.PlayerDeath += new PlayerDeathEventHandler(OnAllyDeath);

            if(FactionDefinition.useXML)
                EventSink.Login  += new LoginEventHandler(EventSink_Login);

            RevenueTimer timer = new RevenueTimer(TimeSpan.FromHours(6)); timer.Start();
        }

        public static void OnWorldLoad()
        {
            List<IFactionEntity> entities = new List<IFactionEntity>();

            foreach (KeyValuePair<Serial, Mobile> kvp in World.Mobiles)
            {
                if (kvp.Value is IFactionEntity)
                    entities.Add((IFactionEntity)kvp.Value);
            }

            foreach (KeyValuePair<Serial, Item> kvp in World.Items)
            {
                if (kvp.Value is IFactionEntity)
                    entities.Add((IFactionEntity)kvp.Value);
            }

            for (int i = 0; i < entities.Count; i++)
            {
                for (int j = 0; j < FactionDefinition.Factions.Count; j++)
                {
                    if (entities[i].FactionName == FactionDefinition.Factions[j].FactionName)
                    {
                        entities[i].Faction = FactionDefinition.Factions[j];

                        if (entities[i] is Sigillum)
                            FactionDefinition.Factions[j].SigilsControlled.Add(entities[i] as Sigillum);

                        break;
                    }
                }
            }
        }

        private class RevenueTimer : Timer
        {
            public static TextWriter writer;

            public RevenueTimer(TimeSpan delay)
                : base(delay)
            {
                Priority = TimerPriority.OneMinute;
                Console.WriteLine("[Factions]: Revenue Timer Initialized.");
            }

            protected override void OnTick()
            {
                Start(); try { writer = new StreamWriter(RevenueOutput); }
                catch { }
                Console.WriteLine("[Factions]: Distributing Tax Revenues.. ({0})", DateTime.Now.ToString());

                if (FactionDefinition.Factions != null && FactionDefinition.Factions.Count > 0)
                {
                    foreach (PlayerFaction a in FactionDefinition.Factions)
                    {
                        int sigils = 0;
                        int totalRev = 0;
                        int totalArea = 0;

                        const double pi = 3.14159265359;

                        foreach (Sigillum sigil in a.SigilsControlled)
                        {
                            sigils++;

                            if (sigil.RegionArea != null)
                            {
                                foreach (Rectangle3D rects in sigil.RegionArea)
                                {
                                    int area = rects.Width * rects.Height;
                                    double ratio = (Math.Sqrt(area) / pi);
                                    int tax = Convert.ToInt32(ratio) * sigil.Priority;

                                    long revenueUpdate = Math.Min(Int32.MaxValue, (long)a.treasuryBalance + (long)tax);
                                    a.treasuryBalance = (int)revenueUpdate;

                                    totalRev += tax;
                                    totalArea += area;
                                }
                            }
                        }

                        writer.WriteLine("[{0}] - Sigils ({1}), Total Revenue ({2}), Total Area ({3})",
                        a.FactionName, sigils, totalRev, totalArea);
                    }
                }

                Console.WriteLine("[Factions]: Distribution Statistics Logged @ ({0})", RevenueOutput);
                try { writer.Close(); }
                catch { }
            }
        }

        private static void EventSink_Login(LoginEventArgs args)
        {
            if (FactionDefinition.useXML)
            {
                foreach (PlayerFaction a in FactionDefinition.Factions)
                {
                    a.QueryMember(args.Mobile);
                }
            }
        }

        public static void HookSpeech(SpeechEventArgs e)
        {
            if (e.Speech.ToLower().IndexOf("i wish to form a faction") >= 0)
                AttemptInitialization(e);

            if (e.Speech.ToLower().IndexOf("i wish to disband my faction") >= 0)
                CheckLeader(e);

            if (e.Speech.ToLower().IndexOf("i wish to form a pact") >= 0)
                AttemptPact(e);

            if (e.Speech.ToLower().IndexOf("i wish to manage my post") >= 0)
                ManagePost(e);

            if (e.Speech.ToLower().IndexOf("i wish to cut ties with my faction") >= 0)
                LeaveFaction(e.Mobile);

            if (e.Speech.ToLower().IndexOf("akos") >= 0)
                DisplayPoints(e);
        }

        public static void DisplayPoints(SpeechEventArgs e)
        {
            if (e.Mobile is PlayerMobile && ((PlayerMobile)e.Mobile).iState != null)
            {
                InteractionState aState = ((PlayerMobile)e.Mobile).iState;
                PlayerFaction faction = aState.Faction;
                e.Mobile.PublicOverheadMessage(MessageType.Spell, faction.primaryHue, true, "" + aState.Kills);
            }
        }

        public static void ManagePost(SpeechEventArgs e)
        {
            PlayerMobile p;
            if (e.Mobile is PlayerMobile)
            {
                p = e.Mobile as PlayerMobile;

                if (p.CurrentFaction == null)
                    return;

                if (p.Region is SigilRegion)
                {
                    SigilRegion sRegion = p.Region as SigilRegion;
                    PlayerFaction faction = sRegion.Sigil.Controlling_Faction;

                    if (faction != null)
                    {
                        p.CloseGump(typeof(LeaderInterface));
                        p.CloseGump(typeof(VendorInterface));
                        p.CloseGump(typeof(GuardInterface));

                        if (p.AccessLevel == AccessLevel.Administrator)
                        {
                            p.SendGump(new LeaderInterface(faction));
                            p.SendGump(new VendorInterface(faction));
                            p.SendGump(new GuardInterface(faction));
                        }

                        else if (p.CurrentFaction == faction)
                        {
                            if (faction.FactionLeader == p)
                                p.SendGump(new LeaderInterface(faction));

                            if (faction.FactionMinister == p)
                                p.SendGump(new VendorInterface(faction));

                            if (faction.FactionSheriff == p)
                                p.SendGump(new GuardInterface(faction));
                        }

                        else p.SendMessage("You do not control this region!");
                    }
                }

                else p.SendMessage("You must be within a controlled region to manage your post.");
            }
        }

        public static void AttemptPact(SpeechEventArgs e)
        {
            if (e.Mobile is PlayerMobile && ((PlayerMobile)e.Mobile).CurrentFaction == null)
            {
                e.Mobile.SendMessage
                    ("You must be the leader of an faction to form pacts with other guilds.");
                return;
            }

            if (e.Mobile is PlayerMobile && ((PlayerMobile)e.Mobile).CurrentFaction.FactionLeader == e.Mobile)
                e.Mobile.Target = new PactTarget(e.Mobile);

            else e.Mobile.SendMessage("You must be the leader of an faction to form pacts with other guilds.");
        }

        public static void AttemptInitialization(SpeechEventArgs e)
        {
            PlayerMobile caller = e.Mobile as PlayerMobile;
            Container c = caller.FindBankNoCreate();

            if (c == null)
            {
                caller.SendMessage("You must have a bank account to form an faction..");
                return;
            }

            if (caller.CurrentFaction == null)
                QueryGuild(caller);

            else caller.SendMessage("You are already in an faction!");
        }

        public static void CheckLeader(SpeechEventArgs e)
        {
            PlayerMobile caller = e.Mobile as PlayerMobile;

            if (caller.CurrentFaction == null)
            {
                caller.SendMessage("You are not in an faction.");
                return;
            }

            if (caller.CurrentFaction.FactionLeader == caller)
            {
                caller.CloseGump(typeof(ConfirmSanitize));
                caller.SendGump(new ConfirmSanitize(caller));
            }

            else caller.SendMessage("You must be the leader of your faction to disband it.");
        }

        public static void LeaveFaction( Mobile m )
        {
            if (m is PlayerMobile)
            {
                PlayerMobile pm = m as PlayerMobile;

                if (pm.CurrentFaction == null)
                    return;

                else pm.SendGump(new ConfirmDetach(pm));
            }
        }

        public static void QueryGuild(PlayerMobile m)
        {
            PlayerMobile p = m;

            if (p.Guild == null && p.AccessLevel == AccessLevel.Player)
            {
                m.SendMessage("You must be a guild leader to form an faction.");
                return;
            }

            if (((p.Guild != null && p.Guild is Guild) && ((Guild)p.Guild).Leader == p)
                || p.AccessLevel > AccessLevel.Counselor)
            {
                if (FactionDefinition.Factions.Count >= FactionDefinition.FactionLimit)
                {
                    m.SendMessage("There are currently too many factions to form another. Please try again later.");
                    return;
                }

                if (FactionDefinition.Factions.Count < FactionDefinition.FactionLimit)
                    p.SendGump(new FactionNameQuery());
            }
        }

        public static void OnAllyDeath(PlayerDeathEventArgs e)
        {
            PlayerMobile victim; PlayerMobile killer;
            PlayerFaction faction;
            InteractionState allyState;

            if (e.Mobile is PlayerMobile && e.Mobile.LastKiller is PlayerMobile)
            {
                victim = e.Mobile as PlayerMobile;              
                killer = victim.LastKiller as PlayerMobile;

                if (victim.CurrentFaction != null && victim.iState != null 
                    && killer.iState != null && killer.CurrentFaction != null
                    && killer.CurrentFaction != victim.CurrentFaction)
                {
                    faction = victim.CurrentFaction;
                    allyState = victim.iState;

                    allyState.Kills--;

                    if (allyState.Kills < 0)
                    {
                        allyState.Kills = 0;
                        allyState.pointDeficit++;

                        if (allyState.pointDeficit > 8)
                            allyState.pointDeficit = 8;
                    }

                    killer.iState.Kills++;

                    int pts = killer.iState.Kills;
                    InteractionState state = killer.iState;

                    int rank = (int)(pts / 50);
                    state.playerRank = (Rank)rank;

                    if (killer.iState.pointDeficit > 0)
                        killer.iState.pointDeficit--;

                    if (victim.iState.pointDeficit <= 3)
                    {
                        int credits = (int)((victim.iState.Kills * 25) / 2) + Utility.RandomMinMax(2, 10);

                        Valorite gold = new Valorite(credits);
                        killer.AddToBackpack(gold);
                        killer.iState.totalCredits += credits;

                        killer.CurrentFaction.treasuryBalance += credits;

                        killer.SendMessage("You have been awarded ({0}) credits for your honorable victory.", credits);
                    }
                }               
            }
        }

        public static void AddMember(PlayerFaction a, Mobile m)
        {
            if (m is PlayerMobile)
            {
                PlayerMobile p = m as PlayerMobile;

                InteractionState state = new InteractionState();
                state.Player = p;
                p.CurrentFaction = a;
            }
        }

        public static void RemoveMember(Mobile m)
        {
            if (m is PlayerMobile)
            {
                PlayerMobile p = m as PlayerMobile;
                p.iState = null;
                p.CurrentFaction = null;

                if (p.CurrentFaction.FactionLeader == p)
                    p.CurrentFaction.Sanitize();
            }
        }

        private class PactTarget : Target
        {
            public PactTarget(Mobile m)
                : base(12, true, TargetFlags.None)
            {
                m.SendMessage("Select the guild leader you wish to invite into your faction.");
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is PlayerMobile && ((PlayerMobile)o).Guild is Guild)
                {
                    PlayerMobile target = o as PlayerMobile;

                    if (((Guild)target.Guild).Leader == target)
                    {
                        if (from is PlayerMobile)
                        {
                            target.SendGump(new JoinFactionQuery(from));
                        }
                    }

                    else from.SendMessage("You must target a guild leader.");
                }

                else from.SendMessage("You must target a guild leader.");
            }
        }
    }

    public class    InteractionState
    {
        public Rank playerRank;
        PlayerFaction faction;

        public PlayerMobile Player;

        public InteractionState()
        {
        }

        public int Kills;
        public int totalCredits;
        public int pointDeficit;

        public PlayerFaction Faction
        {
            get {   return faction;    }

            set {   faction = value;   }
        }

        public void Serialize(BinaryFileWriter writer)
        {
            try
            {
                writer.Write((int)1); //Version;

                writer.Write((int)playerRank);

                writer.WriteMobile((Mobile)Player);

                writer.Write(Kills);
                writer.Write(totalCredits);
                writer.Write(pointDeficit);
            }

            catch { Console.WriteLine("Error: Serialization failed in Faction State."); }
        }

        public void Deserialize(BinaryFileReader reader)
        {
            try
            {
                int version = reader.ReadInt();

                if (version > 0)
                {
                    playerRank = (Rank)reader.ReadInt();
                    Mobile m = reader.ReadMobile();

                    if (m is PlayerMobile)
                        Player = m as PlayerMobile;

                    Kills = reader.ReadInt();
                    totalCredits = reader.ReadInt();
                    pointDeficit = reader.ReadInt();

                    if (Player != null && Player.CurrentFaction != null)
                    {
                        faction = Player.CurrentFaction;
                        Player.iState = this;
                    }
                }
            }

            catch { Console.WriteLine("Error: Deserialization Failed in Faction State."); }
        }
    }
    

    public class Sigillum     : Item, IFactionEntity
    {     
        int home_X, home_Y, home_Z;

        [CommandProperty(AccessLevel.GameMaster)]
        public int HomeX
        {
            get { return home_X; }
            set { home_X = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HomeY
        {
            get { return home_Y; }
            set { home_Y = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HomeZ
        {
            get { return home_Z; }
            set { home_Z = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Priority
        {
            get { return sigilPriority; }

            set 
            {
                if (value < 256)
                    sigilPriority = value;

                else sigilPriority = 255;
            }
        }

        int resistance;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Resistance
        {
            get { return resistance; }

            set
            {
                if (value < 256)
                    resistance = value;

                else resistance = 255;
            }
        }

        int min_z = -120, max_z = 120;
        int sigilPriority = 1;

        public String FactionName { get; set; }

        public PlayerFaction Faction { get; set;}

        public PlayerFaction Controlling_Faction
        {
            get { return Faction; }
            set 
            {
                Faction = value;            
            }
        }

        SigilRegion region;
        Rectangle3D[] region_Area;

        string name = "an Unnamed Region";

        PlayerMobile captor;
        public PlayerMobile Captor
        {
            get { return captor; }
            set { captor = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string ControllingFaction
        {
            get 
            {
                if (Controlling_Faction != null)
                    return Controlling_Faction.FactionName;

                else
                    return "uncaptured";
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string RegionName
        {
            get { return name; }
            set { name = value; UpdateRegion(); } 
        }

        public Rectangle3D[] RegionArea
        {
            get { return region_Area; }
            set { region_Area = value; }
        }

        [Constructable]
        public Sigillum()
        {
            ItemID = 6249;
            Movable = false;
            Name = "a sigillum";          
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(RegionName);
        }

        public Sigillum( Serial serial ) : base( serial )
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            bool empty = false;
            base.Serialize(writer);

            writer.Write((int)1); //Version

            writer.Write((Controlling_Faction == null ? "null" : Controlling_Faction.FactionName));

            writer.Write(HomeX);
            writer.Write(HomeY);
            writer.Write(HomeZ);

            writer.Write(Priority);
            writer.Write(Resistance);

            writer.Write(RegionName);

            if (RegionArea == null)
                empty = true;

            writer.Write(empty);

            if (empty == false)
            {
                writer.Write(RegionArea.Length);

                for (int i = 0; i < RegionArea.Length; i++)
                {
                    Point3D start = RegionArea[i].Start;
                    Point3D end = RegionArea[i].End;

                    writer.Write(start);
                    writer.Write(end);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version > 0)
            {
                FactionName = reader.ReadString();

                HomeX = reader.ReadInt();
                HomeY = reader.ReadInt();
                HomeZ = reader.ReadInt();

                Priority = reader.ReadInt();
                Resistance = reader.ReadInt();

                RegionName = reader.ReadString();

                bool empty = reader.ReadBool();

                if (!empty)
                {
                    int count = reader.ReadInt();

                    List<Rectangle3D> areas = new List<Rectangle3D>();

                    for (int i = 0; i < count; i++)
                    {
                        Point3D start = reader.ReadPoint3D();
                        Point3D end = reader.ReadPoint3D();

                        areas.Add(new Rectangle3D(start, end));
                    }

                    if (areas.Count > 0)
                        RegionArea = areas.ToArray();
                }
            }

            UpdateRegion();
        }

        public void TargetArea(Mobile m)
        {
            BoundingBoxPicker.Begin(m, new BoundingBoxCallback(DefineArea), this);
        }

        public void DefineArea(Mobile from, Map map, Point3D start, Point3D end, object control)
        {
            if (this != null)
            {
                List<Rectangle3D> areas = new List<Rectangle3D>();

                if (RegionArea != null)
                {
                    foreach (Rectangle3D rect in RegionArea)
                        areas.Add(rect);
                }

                Rectangle3D newrect = new Rectangle3D(new Point3D(start.X, start.Y, min_z), new Point3D(end.X, end.Y, max_z));
                areas.Add(newrect);

                RegionArea = areas.ToArray();

                UpdateRegion();

                from.SendMessage("Region Added: (" + start.X + ", " + start.Y + ") - (" + end.X + ", " + end.Y + ")");
            }
        }

        public void UpdateRegion()
        {
            if (region != null && RegionArea != null)
                region.Unregister();

            if ( Map != null )
            {
                if (this != null && RegionArea != null && RegionArea.Length > 0)
                {
                    region = new SigilRegion(RegionName, Map, 255, RegionArea, this);
                    region.Register();
                }
            }
        }

        public bool Home()
        {
            bool home = true;

            if (Location.X != home_X)
                home = false;
            if (Location.Y != home_Y)
                home = false;
            if (Location.Z != home_Z)
                home = false;

            return home;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.GameMaster)
            {
                if (Home())
                    TargetArea(from);

                else
                {
                    HomeX = X;
                    HomeY = Y;
                    HomeZ = Z;

                    from.SendMessage("The Sigillum's current location is now its home location.");
                }
            }

            if (from.AccessLevel == AccessLevel.Player && RegionArea != null)
            {
                if (Home() && from.InRange((IPoint2D)this.Location, 1))
                {
                    if (from.Skills.Stealing.Value < 100)
                    {
                        from.SendMessage("You are not skilled enough in Stealing to take this object.");
                        return;
                    }

                    else if (from is PlayerMobile && ((PlayerMobile)from).CurrentFaction == null)
                    {
                        from.SendMessage("You must be in an faction to capture this sigillum");
                        return;
                    }

                    else if (Controlling_Faction == null && from is PlayerMobile && ((PlayerMobile)from).CurrentFaction != null)
                    {
                        from.SendMessage("You capture the uncontested sigillum and gain control of " + RegionName);
                        Controlling_Faction = ((PlayerMobile)from).CurrentFaction;
                        Controlling_Faction.SigilsControlled.Add(this);

                        AssimilateRegion();
                        ReturnHome();

                        return;
                    }

                    else if (from is PlayerMobile && from.Skills.Stealing.Value >= 100 && Controlling_Faction != null 
                        && Controlling_Faction != ((PlayerMobile)from).CurrentFaction)
                    {
                        from.Hidden = false;
                        from.AddToBackpack(this);
                        from.SendMessage(1410, "You have successfully captured the sigillum. You must now hold it for {0} minutes to capture the the region.", Resistance);

                        if (from is PlayerMobile)
                            Captor = from as PlayerMobile;

                        SigilHoldTimer timer = new SigilHoldTimer(this, from, TimeSpan.FromMinutes(Resistance));
                        timer.Start();

                        foreach (Mobile m in Controlling_Faction.MembersOf)
                        {
                            m.SendMessage(1209, RegionName + "'s sigillum has been captured. Return it at all costs.");
                        }
                    }
                }

                else if (from is PlayerMobile && ((PlayerMobile)from).CurrentFaction == Controlling_Faction 
                    && ((PlayerMobile)from).CurrentFaction != null && !Home())
                {
                    ReturnHome();
                    from.SendMessage("You return the sigillum to its home location.");
                }
            }
        }

        public void ReturnHome()
        {
            if (!Deleted)
            {
                Captor = null;
                MoveToWorld(new Point3D(home_X, home_Y, home_Z));
            }
        }

        public void SanitizeVendors(PlayerFaction faction)
        {
            foreach (Mobile m in region.GetMobiles())
            {
                if (m is IFactionEntity)
                {
                    IFactionEntity entity = m as IFactionEntity;

                    if (entity.Faction == faction && entity is FactionVendor)
                        ((Mobile)entity).Delete();
                }
            }
        }

        public void AssimilateRegion()
        {
            foreach (FactionDecor ao in region.GetItems())
            {
                if (ao.Tier == FactionDecor.HueTier.Primary)
                    ao.Hue = Controlling_Faction.primaryHue;

                if (ao.Tier == FactionDecor.HueTier.Secondary)
                    ao.Hue = Controlling_Faction.secondaryHue;
            }
        }

        private class SigilHoldTimer : Timer
        {
            Sigillum beheldSigil;
            PlayerMobile pm;

            public SigilHoldTimer( Sigillum sigil, Mobile from, TimeSpan delay ) : base( delay )
            {
                Priority = TimerPriority.OneSecond;

                beheldSigil = sigil;   
       
                if(from is PlayerMobile)
                pm = from as PlayerMobile;
            }

            protected override void OnTick()
            {
                if (beheldSigil.Captor != null)
                {
                    if (beheldSigil.Controlling_Faction != null)
                    {
                        beheldSigil.SanitizeVendors(beheldSigil.Controlling_Faction);
                        beheldSigil.Controlling_Faction.SigilsControlled.Remove(beheldSigil);

                        foreach (Mobile m in beheldSigil.Controlling_Faction.MembersOf)
                        {
                            if (m != null)
                                m.SendMessage(beheldSigil.Controlling_Faction.FactionName + " has taken control of " + beheldSigil.RegionName + ".");
                        }
                    }

                    if (pm != null && pm is PlayerMobile && ((PlayerMobile)pm).CurrentFaction != null)
                    {
                        beheldSigil.ReturnHome();
                        beheldSigil.Controlling_Faction = ((PlayerMobile)pm).CurrentFaction;
                        beheldSigil.AssimilateRegion();
                        beheldSigil.Controlling_Faction.SigilsControlled.Add(beheldSigil);
                    }

                    if (beheldSigil.Controlling_Faction != null)
                    {
                        foreach (Mobile m in beheldSigil.Controlling_Faction.MembersOf)
                        {
                            if (m != null)
                                m.SendMessage("Your faction has taken control of " + beheldSigil.RegionName + ".");
                        }
                    }
                }

                Stop();
            }
        }
    }

    public class SigilRegion  : Region
    {
        string region_Name = "";
        Rectangle3D[] region_Area;
        Map region_map;

        public Sigillum Sigil;

        public SigilRegion( string name, Map map, int priority, Rectangle3D[] area, Sigillum s) 
            : base (name, map, priority, area)
        {
            Sigil = s;
            region_Name = name;
            region_Area = area;
            region_map = map;
        }

        public override void OnExit(Mobile m)
        {
            if (m != null && m is PlayerMobile && m.Backpack != null)
            {
                //Using foreach with AcquireItems() produces failure.
                for (int i = 0; i < m.Backpack.AcquireItems().Count; i++)
                {
                    Item item = m.Backpack.AcquireItems()[i];

                    if (item is Sigillum)
                    {
                        ((Sigillum)item).ReturnHome();
                        m.SendMessage("By leaving " + region_Name + ", you return the sigil to it's home location.");
                    }
                }

                base.OnExit(m);
            }
        }

        public override void OnEnter(Mobile m)
        {
            if (m is PlayerMobile)
            {
                m.SendMessage("You have entered " + region_Name + ".");
                base.OnEnter(m);
            }      
        }

        public List<Item> GetItems()
        {
            List<Item> list = new List<Item>();

            if (Sectors != null)
            {
                for (int i = 0; i < Sectors.Length; i++)
                {
                    Sector sector = Sectors[i];

                    foreach (Item item in sector.Items)
                    {
                        if(this.Contains(item.Location) && item is FactionDecor)
                            list.Add(item);
                    }
                }
            }

            return list;
        }
    }

    public class FactionDecor : Item
    {
        public enum HueTier
        {
            None,
            Primary,
            Secondary
        }

        HueTier tier = HueTier.Primary;

        [CommandProperty(AccessLevel.GameMaster)]
        public HueTier Tier
        {
            get { return tier; }
            set { tier= value; }
        }

        [Constructable]
        public FactionDecor()
            : base()
        {
            ItemID = 1801;
            Movable = false;
        }

        [Constructable]
        public FactionDecor(int Id) : base()
        {
            ItemID = Id;
            Movable = false;
        }

        public FactionDecor( Serial serial ) : base( serial )		{}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);             
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }


    public class  InitiateInterface
    {
        public static void Initialize()
        {
            CommandSystem.Register("Factions", AccessLevel.Administrator, new CommandEventHandler(AllyInterface_Command));
        }

        public static void AllyInterface_Command(CommandEventArgs args)
        {
            InterfaceKey key = new InterfaceKey();

            args.Mobile.CloseGump(typeof(AdminInterface));
            args.Mobile.SendGump(new AdminInterface(args.Mobile, key));
        }
    }

    public class       InterfaceKey
    {
        int val;

        public Int32 Value
        {
            get { return val; }
            set { val = value; }
        }
    }


    public class ConfirmDetach     : Gump
    {
        PlayerMobile caller;

        public ConfirmDetach(PlayerMobile p)
            : base(160, 160)
        {
            caller = p;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(139, 46, 119, 107, 9200);
            this.AddLabel(154, 60, 1149, @"Are You Sure?");
            this.AddButton(168, 121, 247, 248, (int)Buttons.Okay, GumpButtonType.Reply, 0);
            this.AddButton(168, 89, 241, 242, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
        }

        public enum Buttons
        {
            Invalid,
            Okay,
            Cancel,
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == (int)Buttons.Okay)
            {
                if (sender.Mobile is PlayerMobile)
                {
                    PlayerMobile pm = sender.Mobile as PlayerMobile;

                    if (pm.Guild == null)
                        return;

                    Mobile[] members = ((Guild)pm.Guild).Members.ToArray();
                    for (int i = 0; i < members.Length; i++)
                    {
                        FactionHandler.RemoveMember(members[i]);
                        members[i].SendMessage("Your guild has severed ties with your faction.");
                    }

                    Array.Clear(members, 0, members.Length);
                }
            }
        }
    }

    public class ConfirmSanitize   : Gump
    {
        PlayerMobile caller;

        public ConfirmSanitize(PlayerMobile p)
            : base(160, 160)
        {
            caller = p;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(139, 46, 119, 107, 9200);
            this.AddLabel(154, 60, 1149, @"Are You Sure?");
            this.AddButton(168, 121, 247, 248, (int)Buttons.Okay, GumpButtonType.Reply, 0);
            this.AddButton(168, 89, 241, 242, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
        }

        public enum Buttons
        {
            Invalid,
            Okay,
            Cancel,
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == (int)Buttons.Okay)
            {
                if(caller.AccessLevel > AccessLevel.GameMaster && caller.CurrentFaction.FactionLeader != caller )
                    sender.Mobile.SendMessage("Selected faction has been purged.");

                caller.CurrentFaction.Sanitize();
            }
        }
    }

    public class FactionNameQuery  : Gump
    {
        public FactionNameQuery()
            : base(100, 100)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            this.AddPage(0);
            this.AddBackground(65, 62, 344, 104, 9260);
            this.AddLabel(85, 77, 1149, @"Name Your Faction");
            this.AddButton(315, 107, 247, 248, (int)Buttons.Okay, GumpButtonType.Reply, 0);
            this.AddTextEntry(123, 109, 200, 20, 0, (int)Buttons.TextEntry, @"");
            this.AddImage(69, 116, 52);

        }

        public enum Buttons
        {
            Invalid,
            Okay,
            TextEntry,
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == (int)Buttons.Okay && sender.Mobile is PlayerMobile &&
                !String.IsNullOrEmpty(info.GetTextEntry((int)Buttons.TextEntry).Text))
            {
                String nameRequested = info.GetTextEntry((int)Buttons.TextEntry).Text;

                Dictionary<string, PlayerFaction> _factionDictionary = new Dictionary<string, PlayerFaction>();

                if (FactionDefinition.Factions.Count > 0)
                {
                    foreach(PlayerFaction a in FactionDefinition.Factions)
                        _factionDictionary.Add(a.FactionName, a);
                }

                if(_factionDictionary.ContainsKey(nameRequested))
                {
                    sender.Mobile.SendMessage("The name you have requested already exists. Please try again.");
                    return;
                }

                PlayerMobile caller = sender.Mobile as PlayerMobile;
                Container c = caller.FindBankNoCreate();

                if (!(c.ConsumeTotal(typeof(Valorite), FactionDefinition.FactionCost)
                    || caller.AccessLevel > AccessLevel.Player))
                {
                    string cost = FactionDefinition.FactionCost.ToString("N0");
                    sender.Mobile.SendMessage("Your bank account must contain {0} gold to form an faction.", cost );
                    return;
                }

                if (NameVerification.Validate(nameRequested, 2, 16, true, true, true, 1, NameVerification.SpaceDashPeriodQuote)
                    && FactionDefinition.Factions.Count <= FactionDefinition.FactionLimit)
                {

                    PlayerFaction faction = new PlayerFaction();
                    faction.FactionName = nameRequested;

                    PlayerMobile pm = sender.Mobile as PlayerMobile;

                    InteractionState aState = new InteractionState();
                    ((PlayerMobile)pm).iState = aState;
                    ((PlayerMobile)pm).iState.Player = pm as PlayerMobile;
                    ((PlayerMobile)pm).iState.Faction = faction;

                    faction.iStates.Add(aState);
                    faction.MembersOf.Add(pm);
                    faction.FactionLeader = pm as PlayerMobile;
                    faction.Serial = pm.Serial;
                    faction.SerialString = pm.Serial.ToString();

                    if ((Guild)pm.Guild != null)
                        faction.ChildGuilds.Add((Guild)pm.Guild);

                    FactionDefinition.Factions.Add(faction);

                    ((PlayerMobile)pm).CurrentFaction = faction;

                    if (pm.Guild != null)
                    {
                        foreach (Mobile m in ((Guild)pm.Guild).Members)
                        {
                            if (m is PlayerMobile && ((PlayerMobile)m).CurrentFaction == null)
                            {
                                PlayerMobile p = m as PlayerMobile;

                                p.CurrentFaction = faction;
                                faction.MembersOf.Add(p);

                                aState = new InteractionState();
                                p.iState = aState;
                                p.iState.Faction = faction;
                                p.iState.Player = p;

                                faction.iStates.Add(aState);

                                p.SendMessage("Your guild has joined a new faction.");
                            }
                        }
                    }

                    sender.Mobile.SendGump(new SteedSelection());
                }

                else sender.Mobile.SendMessage("This is not an acceptable faction name.");
            }
        }
    }

    public class JoinFactionQuery  : Gump
    {
        PlayerFaction faction;
        Mobile from;

        public JoinFactionQuery(Mobile m)
            : base(80, 80)
        {
            if (m is PlayerMobile)
                faction = ((PlayerMobile)m).CurrentFaction;

            from = m;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(87, 53, 323, 164, 9350);
            this.AddHtml(104, 85, 284, 45, m.Name + @" Is inviting you to join an faction. Do you accept?", true, true);
            this.AddButton(214, 162, 247, 248, (int)Buttons.Okay, GumpButtonType.Reply, 0);     
            this.AddButton(303, 162, 241, 242, (int)Buttons.Cancel, GumpButtonType.Reply, 0);

        }

        public enum Buttons
        {
            Invalid,
            Okay,
            Cancel,
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == (int)Buttons.Okay)
            {
                PlayerMobile target = sender.Mobile as PlayerMobile;

                target.CurrentFaction = faction;
                Guild g = target.Guild as Guild;

                faction.ChildGuilds.Add(g);
                faction.MembersOf.Add(target);

                InteractionState alState = new InteractionState();
                target.iState = alState;

                faction.AssimilateGuild(target);

                target.SendMessage("You enter into a pact with the faction '{0}'.", faction.FactionName);
                from.SendMessage("{0} has accepted your faction invitation.", target.Name);
            }
        }
    }

    public class AdminInterface    : Gump
    {
        PlayerFaction currentFactionParsed;

        List<PlayerFaction> factionsToParse = new List<PlayerFaction>();

        InterfaceKey key;

        public AdminInterface(Mobile from, InterfaceKey interfaceKey) : base(150, 150)
        {
             key = interfaceKey;

            if (FactionDefinition.Factions.Count > 0)
            {
                foreach (PlayerFaction a in FactionDefinition.Factions)
                    factionsToParse.Add(a);

                currentFactionParsed = factionsToParse[key.Value];

                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                this.AddPage(0);
                this.AddBackground(23, 15, 459, 278, 9260);
                this.AddAlphaRegion(39, 32, 194, 54);
                this.AddLabel(111, 48, currentFactionParsed.secondaryHue -1, currentFactionParsed.FactionName);
                this.AddButton(230, 258, 2468, 2466, (int)Buttons.Previous, GumpButtonType.Reply, 0);
                this.AddButton(406, 258, 2469, 2470, (int)Buttons.Next, GumpButtonType.Reply, 0);
                this.AddItem(41, 36, 7940, currentFactionParsed.primaryHue);
                this.AddLabel(47, 257, 0, @"Faction Limit: ");
                this.AddTextEntry(135, 257, 29, 20, 0, (int)Buttons.FactionLimitEntry, FactionDefinition.FactionLimit.ToString());
                this.AddLabel(86, 98, 0, @"Member Count: " + currentFactionParsed.MembersOf.Count);
                this.AddButton(325, 258, 2463, 2464, (int)Buttons.Delete, GumpButtonType.Reply, 0);
                this.AddLabel(76, 120, 0, @"Sigils Controlled: " + currentFactionParsed.SigilCount);
                this.AddLabel(73, 141, 0, @"Collective Points: " + currentFactionParsed.CollectivePoints.ToString("N0"));
                this.AddLabel(71, 162, 0, @"Treasury Balance: " + currentFactionParsed.treasuryBalance.ToString("N0"));
                this.AddLabel(291, 37, 0, @"Primary Hue:");
                this.AddTextEntry(382, 36, 52, 20, 0, (int)Buttons.PrimeHueEntry, currentFactionParsed.primaryHue.ToString());
                this.AddLabel(278, 63, 0, @"Secondary Hue:");
                this.AddTextEntry(382, 63, 52, 20, 0, (int)Buttons.SecondHueEntry, currentFactionParsed.secondaryHue.ToString());
                this.AddButton(444, 37, 1209, 1210, (int)Buttons.PrimeHueButton, GumpButtonType.Reply, 0);
                this.AddButton(444, 67, 1209, 1210, (int)Buttons.SecondHueButton, GumpButtonType.Reply, 0);
                this.AddButton(173, 261, 1209, 1210, (int)Buttons.FactionLimitButton, GumpButtonType.Reply, 0);
                this.AddLabel(300, 220, 0, @"Total Factions: " + factionsToParse.Count);
                this.AddLabel(250, 190, 1411, @"Add Funds:");
                this.AddTextEntry(320, 190, 65, 20, 0, (int)Buttons.AddFundsEntry, "" );
                this.AddButton(400, 192, 1209, 1210, (int)Buttons.AddFundsButton, GumpButtonType.Reply, 0);
                this.AddImage(432, 188, 9005);
            }

            else from.SendMessage("There are currently no factions with which to interface.");
        }

        public enum Buttons      
        {
            Invalid,
            Previous,
            Next,
            FactionLimitEntry,
            Delete,
            PrimeHueEntry,
            SecondHueEntry,
            PrimeHueButton,
            SecondHueButton,
            FactionLimitButton,
            AddFundsEntry,
            AddFundsButton,
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == (int)Buttons.AddFundsButton)
            {
                int toAdd = 0;
                try { toAdd = Int32.Parse(info.GetTextEntry((int)Buttons.AddFundsEntry).Text); }
                catch { sender.Mobile.SendMessage("Invalid entry. You must enter a number value."); }

                if (toAdd > 0)
                {
                    long l = Math.Min(Int32.MaxValue, (long)currentFactionParsed.treasuryBalance + (long)toAdd);
                    currentFactionParsed.treasuryBalance = (int)l;
                }

                sender.Mobile.SendGump(new AdminInterface(sender.Mobile, key));  
            }

            if (info.ButtonID == (int)Buttons.Delete)
            {
                if(sender.Mobile is PlayerMobile)
                    sender.Mobile.SendGump(new ConfirmSanitize((PlayerMobile)sender.Mobile));

                //if (FactionDefinition.Factions.Count > 0)
                //{
                //    foreach (BaseFaction a in FactionDefinition.Factions)
                //        factionsToParse.Add(a);

                //    currentFactionParsed = factionsToParse[key.Value];
                //    key.Value = 0;
                //    sender.Mobile.SendGump(new AdminInterface(sender.Mobile, key));             
                //}
            }

            if (info.ButtonID == (int)Buttons.Next)
            {
                if (key.Value < factionsToParse.Count -1)
                {
                    sender.Mobile.CloseGump(typeof(AdminInterface));
                    key.Value++;
                    sender.Mobile.SendGump(new AdminInterface(sender.Mobile, key));
                }

                else sender.Mobile.SendGump(new AdminInterface(sender.Mobile, key));
            }

            if (info.ButtonID == (int)Buttons.Previous)
            {            
                if (key.Value > 0)
                {
                    sender.Mobile.CloseGump(typeof(AdminInterface));
                    key.Value--;
                    sender.Mobile.SendGump(new AdminInterface(sender.Mobile, key));
                }

                else sender.Mobile.SendGump(new AdminInterface(sender.Mobile, key));
            }

            if (info.ButtonID == (int)Buttons.PrimeHueButton)
            {
                if(!String.IsNullOrEmpty(info.GetTextEntry((int)Buttons.PrimeHueEntry).Text))
                {
                    int newHue = 0;

                    if (!String.IsNullOrEmpty(info.GetTextEntry((int)Buttons.PrimeHueEntry).Text))
                    {
                        try { newHue = Int32.Parse(info.GetTextEntry((int)Buttons.PrimeHueEntry).Text); }

                        catch
                        {
                            sender.Mobile.SendMessage("Invalid hue entry, please try again.");
                            sender.Mobile.SendGump(new AdminInterface(sender.Mobile, key));
                            return;
                        }

                        if (newHue <= 0 || newHue > 3000)
                        {
                            sender.Mobile.SendGump(new AdminInterface(sender.Mobile, key));
                            sender.Mobile.SendMessage("Hue value out of range (1-3000)");
                            return;
                        }

                        if (newHue > 0 && newHue < 3000)
                            currentFactionParsed.primaryHue = newHue;

                        sender.Mobile.SendGump(new AdminInterface(sender.Mobile, key));
                    }
                }
            }

            if (info.ButtonID == (int)Buttons.SecondHueButton)
            {
                if (!String.IsNullOrEmpty(info.GetTextEntry((int)Buttons.SecondHueEntry).Text))
                {
                    int newHue = 0;

                    if (!String.IsNullOrEmpty(info.GetTextEntry((int)Buttons.SecondHueEntry).Text))
                    {
                        try { newHue = Int32.Parse(info.GetTextEntry((int)Buttons.SecondHueEntry).Text); }

                        catch
                        {
                            sender.Mobile.SendMessage("Invalid hue entry, please try again.");
                            sender.Mobile.SendGump(new AdminInterface(sender.Mobile, key));
                            return;
                        }

                        if (newHue <= 0 || newHue > 3000)
                        {
                            sender.Mobile.SendGump(new AdminInterface(sender.Mobile, key));
                            sender.Mobile.SendMessage("Hue value out of range (1-3000)");
                            return;
                        }

                        if (newHue > 0 && newHue < 3000)
                            currentFactionParsed.secondaryHue = newHue;

                        sender.Mobile.SendGump(new AdminInterface(sender.Mobile, key));
                    }
                }
            }

            if (info.ButtonID == (int)Buttons.FactionLimitButton)
            {
                int newLimit = 0;

                if (!String.IsNullOrEmpty(info.GetTextEntry((int)Buttons.FactionLimitEntry).Text))
                {
                    try
                    {
                        newLimit = Int32.Parse(info.GetTextEntry((int)Buttons.FactionLimitEntry).Text);
                    }

                    catch
                    {
                        sender.Mobile.SendMessage("You have entered invalid characters.");
                        sender.Mobile.SendGump(new AdminInterface(sender.Mobile, key));
                        return;
                    }

                    if (newLimit <= 0)
                    {
                        sender.Mobile.SendMessage("The faction limit must be a number larger than 0");
                        sender.Mobile.SendGump(new AdminInterface(sender.Mobile, key));
                        return;
                    }

                    if (newLimit < FactionDefinition.Factions.Count)
                    {
                        sender.Mobile.SendMessage("The faction limit can not be less than the number of active factions.");
                        sender.Mobile.SendGump(new AdminInterface(sender.Mobile, key));
                        return;
                    }

                    if (newLimit > 0 && newLimit >= FactionDefinition.Factions.Count)
                    {
                        FactionDefinition.FactionLimit = newLimit;
                        sender.Mobile.SendMessage("The maximum number of factions has been changed.");
                        sender.Mobile.SendGump(new AdminInterface(sender.Mobile, key));
                    }
                }
            }
        }
    }

    public class SteedSelection    : Gump
    {
        public SteedSelection() : base(100, 100)
        {
            this.Closable = false;
            this.Disposable = false;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(9, 11, 411, 177, 9270);
            this.AddAlphaRegion(22, 56, 82, 74);
            this.AddItem(40, 75, 8479);
            this.AddAlphaRegion(123, 56, 82, 74);
            this.AddItem(141, 74, 8480);
            this.AddAlphaRegion(224, 56, 82, 74);
            this.AddItem(242, 74, 8481);
            this.AddAlphaRegion(324, 56, 82, 74);
            this.AddItem(342, 74, 8484);
            this.AddButton( 29, 143, 247, 248, (int)Buttons.HorseOne, GumpButtonType.Reply, 0);
            this.AddButton(132, 143, 247, 248, (int)Buttons.HorseTwo, GumpButtonType.Reply, 0);
            this.AddButton(235, 143, 247, 248, (int)Buttons.HorseThree, GumpButtonType.Reply, 0);
            this.AddButton(336, 143, 247, 248, (int)Buttons.HorseFour, GumpButtonType.Reply, 0);
            this.AddLabel(156, 27, 1149, @"Select Thine Steed");
        }

        public enum Buttons
        {
            Ivalid,
            HorseOne,
            HorseTwo,
            HorseThree,
            HorseFour,
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender.Mobile is PlayerMobile && ((PlayerMobile)sender.Mobile).CurrentFaction != null
                && ((PlayerMobile)sender.Mobile).CurrentFaction.FactionLeader == sender.Mobile)
            {
                PlayerMobile p = sender.Mobile as PlayerMobile;
                PlayerFaction a = p.CurrentFaction;

                if (p.AccessLevel > AccessLevel.Counselor)
                    p.SendMessage("Selection: Horse {0}.", info.ButtonID);

                switch (info.ButtonID)
                {
                    case (int)Buttons.HorseOne:
                        {
                            a.mountBody = 226;
                            a.mountID = 16032;
                        } 
                        break;

                    case (int)Buttons.HorseTwo:
                        {
                            a.mountBody = 228;
                            a.mountID = 16034;
                        }
                        break;

                    case (int)Buttons.HorseThree:
                        {
                            a.mountBody = 204;
                            a.mountID = 16034;
                        }
                        break;

                    case (int)Buttons.HorseFour:
                        {
                            a.mountBody = 200;
                            a.mountID = 16031;
                        }
                        break;

                    default: break;
                }

                sender.Mobile.SendGump(new HueSelection(sender.Mobile));
            }
        }
    }

    public class HueSelection      : Gump
    {
        public enum Tier
        {
            None,
            Primary,
            Secondary
        }

        Tier currentTier;
        int currentHue;

        public HueSelection(Mobile m)
            : base(80, 80)
        {
            m.SendGump(new HueSelection(0, Tier.Primary));
        }

        public HueSelection(int hue, Tier tier)
            : base(80, 80)
        {
            currentHue = hue;
            currentTier = (Tier)tier;

            this.Closable = false;
            this.Disposable = false;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(110, 56, 326, 170, 9380);
            this.AddAlphaRegion(136, 92, 100, 100);
            this.AddItem(161, 121, 7940, currentHue);
            this.AddTextEntry(245, 128, 94, 20, 0, (int)Buttons.TextEntry, @"" + currentHue);
            
            this.AddButton(348, 126, 247, 248, (int)Buttons.OkayButton, GumpButtonType.Reply, 0);
            this.AddButton(250, 165, 1209, 1210, (int)Buttons.Preview, GumpButtonType.Reply, 0);
            this.AddLabel(275, 162, 0, @"Preview Hue");

            if(currentTier == Tier.Primary)
                AddLabel(242, 96, 0, @"Select A Primary Hue");

            if (currentTier == Tier.Secondary)
                AddLabel(242, 96, 0, @"Select A Secondary Hue");

        }

        public enum Buttons
        {
            Invalid,
            TextEntry,
            OkayButton,
            Preview
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender.Mobile is PlayerMobile && ((PlayerMobile)sender.Mobile).CurrentFaction != null
                 && ((PlayerMobile)sender.Mobile).CurrentFaction.FactionLeader == sender.Mobile)
            {
                PlayerMobile p = sender.Mobile as PlayerMobile;
                PlayerFaction a = p.CurrentFaction;

                int hueRequest = 0;

                switch (info.ButtonID)
                {
                    case (int)Buttons.OkayButton:
                        {
                            try
                            {
                                hueRequest = Int32.Parse(info.GetTextEntry((int)Buttons.TextEntry).Text);
                            }

                            catch
                            {
                                sender.Mobile.CloseGump(typeof(HueSelection));
                                sender.Mobile.SendMessage("You have entered invalid characters, only numbers may be used.");
                                sender.Mobile.SendGump(new HueSelection(currentHue, currentTier));
                            }

                            if (hueRequest <= 0 || hueRequest > 3000)
                            {
                                sender.Mobile.CloseGump(typeof(HueSelection));
                                sender.Mobile.SendGump(new HueSelection(currentHue, currentTier));
                                sender.Mobile.SendMessage("Hue value out of range (1-3000)");
                                return;
                            }

                            else if 
                                (hueRequest > 0 && hueRequest < 3000)
                            {
                                if (currentTier == Tier.Primary)
                                {
                                    a.primaryHue = hueRequest;
                                    sender.Mobile.CloseGump(typeof(HueSelection));
                                    sender.Mobile.SendGump(new HueSelection(0, Tier.Secondary));
                                }

                                if (currentTier == Tier.Secondary)
                                {
                                    a.secondaryHue = hueRequest;
                                    sender.Mobile.CloseGump(typeof(HueSelection));
                                    sender.Mobile.SendMessage("You have successfully completed the faction formation process.");
                                }
                            }

                            break;
                        }

                    case (int)Buttons.Preview:
                        {
                             try
                            {
                                hueRequest = Int32.Parse(info.GetTextEntry((int)Buttons.TextEntry).Text);
                            }

                            catch
                            {
                                sender.Mobile.CloseGump(typeof(HueSelection));
                                sender.Mobile.SendMessage("You have entered invalid characters, only numbers may be used.");
                                sender.Mobile.SendGump(new HueSelection(currentHue, currentTier));
                            }

                            if (hueRequest <= 0 || hueRequest > 3000)
                            {
                                sender.Mobile.CloseGump(typeof(HueSelection));
                                sender.Mobile.SendGump(new HueSelection(currentHue, currentTier));
                                sender.Mobile.SendMessage("Hue value out of range (1-3000)");
                                return;
                            }

                            else if
                                (hueRequest > 0 && hueRequest < 3000)
                            {
                                sender.Mobile.CloseGump(typeof(HueSelection));
                                sender.Mobile.SendGump(new HueSelection(hueRequest, currentTier));
                            }

                            break;
                        }

                    default: break;
                }
            }
        }
    }

    public class LeaderInterface   : Gump
    {
        PlayerFaction faction;
        public LeaderInterface(PlayerFaction a)
            : base(60, 60)
        {
            faction = a;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            this.AddPage(0);
            this.AddBackground(39, 30, 420, 153, 9270);
            this.AddLabel(315, 56, 1149, @"Assign Minister");
            this.AddButton(419, 61, 2103, 2104, (int)Buttons.MinisterButton, GumpButtonType.Reply, 0);
            this.AddLabel(315, 79, 1149, @"Assign Sheriff");
            this.AddButton(419, 85, 2103, 2104, (int)Buttons.SheriffButton, GumpButtonType.Reply, 0);
            this.AddLabel(56, 45, 1149, @"Faction Leader Interface");
            this.AddLabel(58, 146, 1311, @"Allocate To Sheriff");
            this.AddLabel(58, 124, 1311, @"Allocate To Minister");
            this.AddTextEntry(197, 124, 95, 20, 1149, (int)Buttons.ministerEntry, @"0");
            this.AddTextEntry(197, 146, 95, 20, 1149, (int)Buttons.sheriffEntry, @"0");
            this.AddButton(298, 127, 2103, 2104, (int)Buttons.ministerAllocateButton, GumpButtonType.Reply, 0);
            this.AddButton(298, 151, 2103, 2104, (int)Buttons.sheriffAllocateButton, GumpButtonType.Reply, 0);
            this.AddLabel(56, 64, 1411, @"Treasury Balance:");
            this.AddLabel(57, 85, 1149, @"" + faction.treasuryBalance.ToString("N0"));
            this.AddButton(414, 144, 1153, 1155, (int)Buttons.upgradeButton, GumpButtonType.Reply, 0);
            this.AddLabel(356, 143, 1149, @"Upgrade");

        }

        public enum Buttons
        {
            mouseRightClick,
            MinisterButton,
            SheriffButton,
            ministerEntry,
            sheriffEntry,
            ministerAllocateButton,
            sheriffAllocateButton,
            upgradeButton,
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Int32 id = info.ButtonID;

            switch (id)
            {
                case (int)Buttons.upgradeButton:
                    {
                        sender.Mobile.CloseGump(typeof(UpgradeSelection));
                        sender.Mobile.SendGump(new UpgradeSelection(faction));
                        sender.Mobile.SendGump(new LeaderInterface(faction));
                        break;
                    }

                case (int)Buttons.sheriffAllocateButton:
                    {
                        if (faction != null)
                        {
                            int shift = 0;
                            try { shift = Int32.Parse(info.GetTextEntry((int)Buttons.sheriffEntry).Text); }
                            catch
                            {
                                sender.Mobile.SendGump(new LeaderInterface(faction));
                                sender.Mobile.SendMessage("Entry Invalid: You may only use numbers (0-9).");
                                return;
                            }

                            if (shift > faction.treasuryBalance)
                            {
                                sender.Mobile.SendGump(new LeaderInterface(faction));
                                sender.Mobile.SendMessage("Your treasury does not contain enough funds.");
                                return;
                            }

                            if (shift > 0 && faction.treasuryBalance > shift)
                            {
                                long lShift = Math.Min(Int32.MaxValue, (long)faction.sheriffBalance + (long)shift);

                                faction.sheriffBalance += (int)lShift;
                                faction.treasuryBalance -= (int)lShift;

                                sender.Mobile.SendGump(new LeaderInterface(faction));
                            }
                        } break;
                    }

                case (int)Buttons.ministerAllocateButton:
                    {
                        if (faction != null)
                        {
                            int shift = 0;
                            try { shift = Int32.Parse(info.GetTextEntry((int)Buttons.ministerEntry).Text); }
                            catch
                            {
                                sender.Mobile.SendGump(new LeaderInterface(faction));
                                sender.Mobile.SendMessage("Entry Invalid: You may only use numbers (0-9).");
                                return;
                            }

                            if (shift > faction.treasuryBalance)
                            {
                                sender.Mobile.SendGump(new LeaderInterface(faction));
                                sender.Mobile.SendMessage("Your treasury does not contain enough funds.");
                                return;
                            }

                            if (shift > 0 && faction.treasuryBalance > shift)
                            {
                                long lShift = Math.Min(Int32.MaxValue, (long)faction.ministerBalance + (long)shift);

                                faction.ministerBalance += (int)lShift;
                                faction.treasuryBalance -= (int)lShift;

                                sender.Mobile.SendGump(new LeaderInterface(faction));
                            }
                        } break;
                    }

                case (int)Buttons.SheriffButton:
                    {
                        if (faction != null)
                        {
                            sender.Mobile.SendGump(new LeaderInterface(faction));
                            sender.Mobile.Target = new AssignTarget(sender.Mobile, Buttons.SheriffButton);
                        }

                    } break;

                case (int)Buttons.MinisterButton:
                    {
                        if (faction != null)
                        {
                            sender.Mobile.SendGump(new LeaderInterface(faction));
                            sender.Mobile.Target = new AssignTarget(sender.Mobile, Buttons.MinisterButton);
                        }

                    } break;

                default: break;
            }
        }

        private class AssignTarget : Target
        {
            int buttonID;
            public AssignTarget(Mobile m, Buttons id)
                : base(12, true, TargetFlags.None)
            {
                buttonID = (int)id;

                if (id == Buttons.SheriffButton)
                    m.SendMessage("Select the player you wish to make Sheriff.");

                if (id == Buttons.MinisterButton)
                    m.SendMessage("Select the player you wish to make Minister.");
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is PlayerMobile &&  from is PlayerMobile && o != from &&
                    ((PlayerMobile)o).CurrentFaction == ((PlayerMobile)from).CurrentFaction)
                {
                    if (buttonID == (int)Buttons.SheriffButton)
                    {
                        PlayerMobile target = o as PlayerMobile;
                        target.CurrentFaction.FactionSheriff = target;
                        from.SendMessage("You assign {0} as sheriff.", target.Name);
                    }

                    if (buttonID == (int)Buttons.MinisterButton)
                    {
                        PlayerMobile target = o as PlayerMobile;
                        target.CurrentFaction.FactionMinister = target;
                        from.SendMessage("You assign {0} as finance minister.", target.Name);
                    }
                }

                else from.SendMessage("You must target a fellow faction member.");
            }
        }
    }
    
    public class VendorInterface   : Gump
    {
        PlayerFaction faction;
        public VendorInterface(PlayerFaction a)
            : base(50, 50)
        {
            faction = a;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(49, 14, 321, 343, 9390);
            this.AddItem(71, 88, 4021);
            this.AddLabel(120, 88, 0, @"Armsdealer");
            this.AddButton(222, 89, 55, 55, (int)Buttons.armsdealerButton, GumpButtonType.Reply, 0);
            this.AddItem(67, 124, 3997);
            this.AddLabel(120, 125, 0, @"Tailor");
            this.AddButton(222, 126, 55, 55, (int)Buttons.tailorButton, GumpButtonType.Reply, 0);
            this.AddLabel(110, 53, 1111, @"Vendor Type");
            this.AddItem(74, 159, 4022);
            this.AddLabel(119, 161, 0, @"Horse Breeder");
            this.AddButton(222, 162, 55, 55, (int)Buttons.breederButton, GumpButtonType.Reply, 0);
            this.AddItem(74, 196, 7141);
            this.AddLabel(120, 199, 0, @"Merchant");
            this.AddButton(222, 199, 55, 55, (int)Buttons.merchantButton, GumpButtonType.Reply, 0);
            this.AddItem(80, 232, 7867);
            this.AddLabel(120, 236, 0, @"Engineer");
            this.AddButton(222, 238, 55, 55, (int)Buttons.engiButton, GumpButtonType.Reply, 0);
            this.AddLabel(285, 54, 1211, @"Cost");
            this.AddLabel(282,  88, 0, @""+ faction.EconomyLevel * 2000);
            this.AddLabel(282, 125, 0, @""+ faction.EconomyLevel * 2000);
            this.AddLabel(282, 161, 0, @""+ faction.EconomyLevel * 2000);
            this.AddLabel(282, 199, 0, @""+ faction.EconomyLevel * 2000);
            this.AddLabel(282, 236, 0, @""+ faction.EconomyLevel * 2000);
            this.AddButton(317, 277, 2224, 2224, (int)Buttons.dismissButton, GumpButtonType.Reply, 0);
            this.AddLabel(216, 273, 1311, @"Dismiss Vendor");
            this.AddLabel(91, 302, 1411, @"Funds Available:");
            this.AddLabel(197, 302, 0, @"" + faction.ministerBalance.ToString("N0"));

        }

        public enum Buttons
        {
            Invalid,
            mouseRightClick,
            armsdealerButton,
            tailorButton,
            breederButton,
            merchantButton,
            engiButton,
            dismissButton,
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID != (int)Buttons.Invalid)
                sender.Mobile.SendGump(new VendorInterface(faction));

            if (!(sender.Mobile is PlayerMobile) || !(sender.Mobile.Region is SigilRegion) ||
                ((SigilRegion)sender.Mobile.Region).Sigil.Faction != ((PlayerMobile)sender.Mobile).CurrentFaction) return;

            if (faction.vendorCount >= faction.VendorLimit)
            {
                sender.Mobile.SendMessage("You have reached your economy's vendor capacity.");
                return;
            }

            if (info.ButtonID == (int)Buttons.dismissButton)
                sender.Mobile.Target = new DismissalTarget(sender.Mobile);

            if (info.ButtonID == (int)Buttons.armsdealerButton)
            {
                if (faction.EconomyLevel * 2000 > faction.ministerBalance)
                {
                    sender.Mobile.SendMessage("You do not have the funds required."); return;
                }

                ArmsDealer dealer = new ArmsDealer(faction);
                dealer.MoveToWorld(sender.Mobile.Location, sender.Mobile.Map);
                dealer.Direction = sender.Mobile.Direction;
                dealer.Home = sender.Mobile.Location;
                dealer.RangeHome = 4;

                faction.ministerBalance -= faction.EconomyLevel * 2000;
            }

            if (info.ButtonID == (int)Buttons.tailorButton)
            {
                if (faction.EconomyLevel * 2000 > faction.ministerBalance)
                {
                    sender.Mobile.SendMessage("You do not have the funds required."); return;
                }

                Outfitter fitter = new Outfitter(faction);
                fitter.MoveToWorld(sender.Mobile.Location, sender.Mobile.Map);
                fitter.Direction = sender.Mobile.Direction;
                fitter.Home = sender.Mobile.Location;
                fitter.RangeHome = 4;

                faction.ministerBalance -= faction.EconomyLevel * 2000;
            }

            if (info.ButtonID == (int)Buttons.breederButton)
            {
                if (faction.EconomyLevel * 2000 > faction.ministerBalance)
                {
                    sender.Mobile.SendMessage("You do not have the funds required."); return;
                }

                HorseBreeder breeder = new HorseBreeder(faction);
                breeder.MoveToWorld(sender.Mobile.Location, sender.Mobile.Map);
                breeder.Direction = sender.Mobile.Direction;
                breeder.Home = sender.Mobile.Location;
                breeder.RangeHome = 4;

                faction.ministerBalance -= faction.EconomyLevel * 2000;
            }

            if (info.ButtonID == (int)Buttons.merchantButton)
            {
                if (faction.EconomyLevel * 2000 > faction.ministerBalance)
                {
                    sender.Mobile.SendMessage("You do not have the funds required."); return;
                }

                FactionMerchant merchant = new FactionMerchant(faction);
                merchant.MoveToWorld(sender.Mobile.Location, sender.Mobile.Map);
                merchant.Direction = sender.Mobile.Direction;
                merchant.Home = sender.Mobile.Location;
                merchant.RangeHome = 4;

                faction.ministerBalance -= faction.EconomyLevel * 2000;
            }

            if (info.ButtonID == (int)Buttons.engiButton)
            {
                if (faction.EconomyLevel * 2000 > faction.ministerBalance)
                {
                    sender.Mobile.SendMessage("You do not have the funds required."); return;
                }

                Engineer engi = new Engineer(faction);
                engi.MoveToWorld(sender.Mobile.Location, sender.Mobile.Map);
                engi.Direction = sender.Mobile.Direction;
                engi.Home = sender.Mobile.Location;
                engi.RangeHome = 4;

                faction.ministerBalance -= faction.EconomyLevel * 2000;
            }
        }

        private class DismissalTarget : Target
        {
            public DismissalTarget(Mobile m)
                : base(12, true, TargetFlags.None)
            {
                m.SendMessage("Select the unit you wish to dismiss from service.");
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (from is PlayerMobile && o is FactionVendor && ((PlayerMobile)from).CurrentFaction == ((FactionVendor)o).Faction)
                {
                    ((Mobile)o).Delete();
                    ((PlayerMobile)from).CurrentFaction.vendorCount--;
                    from.SendMessage("You remove the vendor from service.");
                }

                else from.SendMessage("You must target a vendor under your employ.");
            }
        }

    }

    public class GuardInterface    : Gump
    {
        PlayerFaction faction;
        public GuardInterface(PlayerFaction a)
            : base(50, 50)
        {
            faction = a;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            this.AddPage(0);
            this.AddBackground(49, 14, 321, 374, 9380);
            this.AddItem(73, 81, 8481);
            this.AddLabel(120, 88, 0, @"Cavalry");
            this.AddButton(222, 89, 55, 55, (int)Buttons.cavalryButton, GumpButtonType.Reply, 0);
            this.AddItem(62, 119, 5123);
            this.AddLabel(120, 125, 0, @"Pikeman");
            this.AddButton(222, 126, 55, 55, (int)Buttons.pikemanButton, GumpButtonType.Reply, 0);
            this.AddLabel(110, 53, 1111, @"Guard Type");
            this.AddItem(66, 159, 5049);
            this.AddLabel(119, 161, 0, @"Swordsman");
            this.AddButton(222, 162, 55, 55, (int)Buttons.swordsmanButton, GumpButtonType.Reply, 0);
            this.AddItem(65, 193, 5042);
            this.AddLabel(120, 199, 0, @"Archer");
            this.AddButton(222, 199, 55, 55, (int)Buttons.archerButton, GumpButtonType.Reply, 0);
            this.AddItem(71, 237, 5912);
            this.AddLabel(120, 236, 0, @"Wizard");
            this.AddButton(222, 238, 55, 55, (int)Buttons.wizardButton, GumpButtonType.Reply, 0);
            this.AddLabel(270, 88, 0, @"" + (4000 + (faction.MilitaryLevel * 1000)));
            this.AddLabel(272, 54, 1211, @"Cost");
            this.AddLabel(270, 125, 0, @"" + (4000 + (faction.MilitaryLevel * 1000)));
            this.AddLabel(270, 161, 0, @"" + (4000 + (faction.MilitaryLevel * 1000)));
            this.AddLabel(270, 199, 0, @"" + (4000 + (faction.MilitaryLevel * 1000)));
            this.AddLabel(270, 236, 0, @"" + (4000 + (faction.MilitaryLevel * 1000)));
            this.AddLabel(270, 274, 0, @"" + (4000 + (faction.MilitaryLevel * 1000)));
            this.AddButton(312, 314, 2224, 2224, (int)Buttons.dismissButton, GumpButtonType.Reply, 0);
            this.AddLabel(215, 311, 1311, @"Dismiss Guard");
            this.AddLabel(77, 335, 1411, @"Funds Available:");
            this.AddLabel(183, 335, 0, faction.sheriffBalance.ToString("N0"));
            this.AddItem(74, 273, 3834);
            this.AddLabel(120, 275, 0, @"Medic");
            this.AddButton(222, 276, 55, 55, (int)Buttons.medicButton, GumpButtonType.Reply, 0);
            

        }

        public enum Buttons
        {
            Invalid,
            cavalryButton,
            pikemanButton,
            swordsmanButton,
            archerButton,
            wizardButton,
            dismissButton,
            medicButton,
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if(info.ButtonID != (int)Buttons.Invalid)
            sender.Mobile.SendGump(new GuardInterface(faction));

            if (!(sender.Mobile is PlayerMobile) || !(sender.Mobile.Region is SigilRegion) ||
                ((SigilRegion)sender.Mobile.Region).Sigil.Faction != ((PlayerMobile)sender.Mobile).CurrentFaction) return;

            if (faction.guardCount >= faction.GuardLimit)
            {
                sender.Mobile.SendMessage("You have reached your military's capacity");
                return;
            }

            if (info.ButtonID == (int)Buttons.dismissButton)
            {
                sender.Mobile.Target = new DismissalTarget(sender.Mobile);
            }

            if (info.ButtonID == (int)Buttons.archerButton)
            {
                if((4000 + (faction.MilitaryLevel * 1000) > faction.sheriffBalance))
                {
                    sender.Mobile.SendMessage("You do not have the funds required."); return;
                }

                Guard guard = new Guard(GuardType.Archer, faction);
                guard.MoveToWorld(sender.Mobile.Location, sender.Mobile.Map);
                guard.Direction = sender.Mobile.Direction;
                guard.Home = sender.Mobile.Location;

                faction.sheriffBalance -= (4000 + (faction.MilitaryLevel * 1000));
            }

            if (info.ButtonID == (int)Buttons.cavalryButton)
            {
                if ((4000 + (faction.MilitaryLevel * 1000) > faction.sheriffBalance))
                {
                    sender.Mobile.SendMessage("You do not have the funds required."); return;
                }

                Guard guard = new Guard(GuardType.Cavalry, faction);
                guard.MoveToWorld(sender.Mobile.Location, sender.Mobile.Map);
                guard.Direction = sender.Mobile.Direction;
                guard.Home = sender.Mobile.Location;

                faction.sheriffBalance -= (4000 + (faction.MilitaryLevel * 1000));
            }

            if (info.ButtonID == (int)Buttons.medicButton)
            {
                if ((4000 + (faction.MilitaryLevel * 1000) > faction.sheriffBalance))
                {
                    sender.Mobile.SendMessage("You do not have the funds required."); return;
                }

                Guard guard = new Guard(GuardType.Medic, faction);
                guard.MoveToWorld(sender.Mobile.Location, sender.Mobile.Map);
                guard.Direction = sender.Mobile.Direction;
                guard.Home = sender.Mobile.Location;

                faction.sheriffBalance -= (4000 + (faction.MilitaryLevel * 1000));
            }

            if (info.ButtonID == (int)Buttons.pikemanButton)
            {
                if ((4000 + (faction.MilitaryLevel * 1000) > faction.sheriffBalance))
                {
                    sender.Mobile.SendMessage("You do not have the funds required."); return;
                }

                Guard guard = new Guard(GuardType.Pikeman, faction);
                guard.MoveToWorld(sender.Mobile.Location, sender.Mobile.Map);
                guard.Direction = sender.Mobile.Direction;
                guard.Home = sender.Mobile.Location;

                faction.sheriffBalance -= (4000 + (faction.MilitaryLevel * 1000));
            }

            if (info.ButtonID == (int)Buttons.wizardButton)
            {
                if ((4000 + (faction.MilitaryLevel * 1000) > faction.sheriffBalance))
                {
                    sender.Mobile.SendMessage("You do not have the funds required."); return;
                }

                Guard guard = new Guard(GuardType.Wizard, faction);
                guard.MoveToWorld(sender.Mobile.Location, sender.Mobile.Map);
                guard.Direction = sender.Mobile.Direction;
                guard.Home = sender.Mobile.Location;

                faction.sheriffBalance -= (4000 + (faction.MilitaryLevel * 1000));
            }

            if (info.ButtonID == (int)Buttons.swordsmanButton)
            {
                if ((4000 + (faction.MilitaryLevel * 1000) > faction.sheriffBalance))
                {
                    sender.Mobile.SendMessage("You do not have the funds required."); return;
                }

                Guard guard = new Guard(GuardType.Swordsman, faction);
                guard.MoveToWorld(sender.Mobile.Location, sender.Mobile.Map);
                guard.Direction = sender.Mobile.Direction;
                guard.Home = sender.Mobile.Location;

                faction.sheriffBalance -= (4000 + (faction.MilitaryLevel * 1000));
            }
            
        }

        private class DismissalTarget : Target
        {
            public DismissalTarget(Mobile m)
                : base(12, true, TargetFlags.None)
            {
                m.SendMessage("Select the unit you wish to dismiss from service.");
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (from is PlayerMobile && o is Guard && ((PlayerMobile)from).CurrentFaction == ((Guard)o).Faction)
                {
                    ((Mobile)o).Delete();
                    ((PlayerMobile)from).CurrentFaction.guardCount--;
                }

                else from.SendMessage("You must target a military unit under your control.");
            }
        }

    }

    public class UpgradeSelection  : Gump
    {
        PlayerFaction faction;

        public UpgradeSelection(PlayerFaction a)
            : base(55, 55)
        {
            faction = a;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            this.AddPage(0);
            this.AddBackground(66, 48, 290, 221, 9380);
            this.AddItem(83, 115, 2472);
            this.AddItem(94, 187, 7109);
            this.AddItem(93, 131, 7144);
            this.AddLabel(140, 125, 0, @"Economy");
            this.AddLabel(140, 188, 0, @"Military");
            this.AddLabel(125, 86, 1311, @"Upgrade Type");
            this.AddLabel(236, 87, 1211, @"Cost");

            if (faction.EconomyLevel >= 5) AddLabel(235, 125, 0, @"N/A");
            else AddLabel(232, 125, 0, @"" + (faction.EconomyLevel * 60000).ToString("N0"));

            if (faction.MilitaryLevel >= 5) AddLabel(235, 189, 0, @"N/A");
            else AddLabel(232, 189, 0, @"" + (faction.MilitaryLevel * 60000).ToString("N0"));

            this.AddButton(291, 127, 1209, 1210, (int)Buttons.economyButton, GumpButtonType.Reply, 0);
            this.AddButton(291, 192, 1209, 1210, (int)Buttons.militaryButton, GumpButtonType.Reply, 0);
            this.AddItem(97, 170, 5050);

        }

        public enum Buttons
        {
            Invalid,
            economyButton,
            militaryButton,
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile m = sender.Mobile;

            if (info.ButtonID != (int)Buttons.Invalid)
            {
                sender.Mobile.CloseGump(typeof(LeaderInterface));
                sender.Mobile.SendGump(new LeaderInterface(faction));
            }

            if (info.ButtonID == (int)Buttons.economyButton && faction != null)
            {
                if (faction.EconomyLevel >= 5)
                {
                    m.SendGump(new UpgradeSelection(faction));
                    m.SendMessage("You can not upgrade your economy any further.");
                    return;
                }

                else if (faction.treasuryBalance > faction.EconomyLevel * 60000)
                {
                    faction.treasuryBalance -= faction.EconomyLevel * 60000;
                    faction.EconomyLevel++;

                    m.SendMessage("You have successfully upgraded your economic capabilities.");
                    m.SendGump(new UpgradeSelection(faction));
                }
            }

            if (info.ButtonID == (int)Buttons.militaryButton && faction != null)
            {
                if (faction.MilitaryLevel >= 5)
                {
                    m.SendGump(new UpgradeSelection(faction));
                    m.SendMessage("You can not upgrade your economy any further.");
                    return;
                }

                else if (faction.treasuryBalance > faction.MilitaryLevel * 60000)
                {
                    faction.treasuryBalance -= faction.MilitaryLevel * 60000;
                    faction.MilitaryLevel++;

                    m.SendMessage("You have successfully upgraded your military capabilities.");
                    m.SendGump(new UpgradeSelection(faction));
                }
            }
        }
    }

    public class TrapSelection     : Gump
    {
        private class SelectPotion : Target
        {
            Buttons buttonId; Container container; PlayerFaction faction;
            public SelectPotion(PlayerFaction a, Buttons b, Container c, Mobile m)
                : base(1, true, TargetFlags.None)
            {
                faction = a;
                buttonId = b;
                container = c;

                if (b == Buttons.pouchButton)
                    m.SendMessage("Select a poison potion to use in your trap.");

                if (b == Buttons.bombButton)
                    m.SendMessage("Select an explosion potion to use in your trap.");
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                #region Bomb
                if (buttonId == Buttons.bombButton)
                {
                    if (targeted is BaseExplosionPotion)
                    {
                        Type potion = null;
                        int damage = 0;

                        if (targeted is GreaterExplosionPotion)
                        {
                            potion = typeof(GreaterExplosionPotion);
                            damage = 40;
                        }

                        if (targeted is ExplosionPotion)
                        {
                            potion = typeof(ExplosionPotion);
                            damage = 30;
                        }

                        if (targeted is LesserExplosionPotion)
                        {        
                            potion = typeof(LesserExplosionPotion);
                            damage = 20;
                        }

                        if (potion == null)
                            return;

                        int validate = container.ConsumeTotal
                        (new Type[] {
					       typeof( SpoolOfThread ),
					       typeof( Springs )
                        },
                        new int[] {
					        1,
					        1
                        });

                        switch (validate)
                        {

                            case 0:
                                {
                                    from.SendMessage("You must have a spool of thread to construct this trap.");
                                    break;
                                }

                            case 1:
                                {
                                    from.SendMessage("You must have a spring to construct this trap.");
                                    break;
                                }

                            default:
                                {
                                    from.AddToBackpack(new FactionExplosionTrap(damage, faction));
                                    from.SendMessage("You successfully create the trap.");
                                    ((Item)targeted).Consume(1); break;
                                }
                        }
                    }

                    else from.SendMessage("You must target an explosion potion.");
                }
                #endregion

                #region Poison
                if (buttonId == Buttons.pouchButton)
                {
                    if (targeted is BasePoisonPotion)
                    {
                        Type potion = null;
                        Poison poison = null;

                        if (targeted is LesserPoisonPotion)
                        {
                            potion = typeof(LesserPoisonPotion);
                            poison = Poison.Lesser;
                        }

                        if (targeted is PoisonPotion)
                        {
                            potion = typeof(PoisonPotion);
                            poison = Poison.Regular;
                        }

                        if (targeted is GreaterPoisonPotion)
                        {
                            potion = typeof(GreaterPoisonPotion);
                            poison = Poison.Greater;
                        }

                        if (targeted is DeadlyPoisonPotion)
                        {
                            potion = typeof(DeadlyPoisonPotion);
                            poison = Poison.Deadly;
                        }

                        if (potion == null || poison == null)
                            return;

                        Type[] types = new Type[]
                        {
                            typeof(SpoolOfThread),
                            typeof(Springs)
                        };

                        int[] amounts = new int[] { 1, 1, };

                        int validate = container.ConsumeTotal(types, amounts);

                        switch (validate)
                        {
                            case 0:
                                {
                                    from.SendMessage("You must have a spool of thread to construct this trap.");
                                    break;
                                }

                            case 1:
                                {
                                    from.SendMessage("You must have a spring to construct this trap.");
                                    break;
                                }

                            default:
                                {
                                    from.AddToBackpack(new FactionGasTrap(poison, faction));
                                    from.SendMessage("You successfully create the trap.");
                                    ((Item)targeted).Consume(1);  break;
                                }
                        }
                    }

                    else from.SendMessage("You must target a poison potion.");
                }
                #endregion
            }
        }

        void AttemptCreation(PlayerFaction a, Buttons b, Mobile m)
        {
            if (m is PlayerMobile && ((PlayerMobile)m).CurrentFaction != null)
            {
                PlayerMobile p = m as PlayerMobile;
                int skill = (int)(p.Skills.Tinkering.Value);
                Container c = p.Backpack;

                if (b == Buttons.bombButton || b == Buttons.pouchButton)
                {
                    p.Target = new SelectPotion(faction, b, c, m);
                }

                if (b == Buttons.snareButton)
                {
                    Type[] types = new Type[]
                        {
                            typeof(Hinge),
                            typeof(Springs),
                            typeof(BaseIngot)
                        };

                    int[] amounts = new int[] { 1, 1, 1 };

                    int validate = c.ConsumeTotal(types, amounts);

                    switch (validate)
                    {
                        case 0:
                            {
                                m.SendMessage("You must have a hinge to craft this.");
                                break;
                            }

                        case 1:
                            {
                                m.SendMessage("You must have a set of springs to craft this.");
                                break;
                            }

                        case 3:
                            {
                                m.SendMessage("You must have an ingot to craft this.");
                                break;
                            }

                        default:
                            {
                                m.AddToBackpack(new FactionSnareTrap(faction));
                                m.SendMessage("You successfully create the trap.");
                                break;
                            }
                    }
                }
            }
        }

        PlayerFaction faction;
        public TrapSelection(PlayerFaction a)
            : base(80, 80)
        {
            faction = a; 

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(61, 39, 263, 113, 9250);
            this.AddAlphaRegion(76, 52, 232, 86);
            this.AddItem(90, 84, 2480);
            this.AddButton(84, 110, 247, 248, (int)Buttons.pouchButton, GumpButtonType.Reply, 0);
            this.AddItem(169, 85, 7192);
            this.AddButton(161, 110, 247, 248, (int)Buttons.bombButton, GumpButtonType.Reply, 0);
            this.AddItem(247, 84, 3741, 1175);
            this.AddButton(239, 110, 247, 248, (int)Buttons.snareButton, GumpButtonType.Reply, 0);
            this.AddLabel(148, 56, 1149, @"Select A Trap");

        }

        public enum Buttons
        {
            Invalid,
            pouchButton,
            bombButton,
            snareButton,
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if(info.ButtonID != (int)Buttons.Invalid)
            AttemptCreation(faction, (Buttons)info.ButtonID, sender.Mobile);
        }

    }


    public abstract class FactionTrap  : BaseTrap, IFactionEntity
    {
        public PlayerFaction Faction { get; set; }

        public String FactionName { get; set; }

        public bool Active;

        public FactionTrap( int itemID ) : base( itemID )
		{
            Movable = true;
            Weight = 1.0;
            Stackable = false;
		}

		public FactionTrap( Serial serial ) : base( serial )
		{
		}

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Region is SigilRegion && from is PlayerMobile)
            {
                SigilRegion region = from.Region as SigilRegion;
                PlayerMobile p = from as PlayerMobile;

                if (region.Sigil.Faction == p.CurrentFaction)
                    AttemptPlacement(from);

                else from.SendMessage("Your faction must control this region in order to place this trap.");
            }

            if (from is PlayerMobile && Faction != ((PlayerMobile)from).CurrentFaction)
            {
                PlayerMobile p = from as PlayerMobile;

                if (p.Skills.RemoveTrap.Value < 100)
                {
                    if (Utility.RandomDouble() <= 0.30)
                    {
                        OnTrigger(p);
                        p.SendMessage("You trigger the device!");
                    }

                    else p.SendMessage("You fail to dismantle the trap.");
                }

                else
                {
                    p.SendMessage("You successfully dismantle the trap."); 
                    Delete();
                }
            }
        }

        void AttemptPlacement(Mobile from)
        {
            List<Item> traps = new List<Item>();
            IPooledEnumerable eable = from.GetItemsInRange(1);

            foreach (Item i in eable)
            {
                if (i is FactionTrap && i != this)
                    traps.Add(i);
            } 
            
            eable.Free();

            if (!IsChildOf(from.Backpack))
                return;

            if (traps.Count > 0)
                from.SendMessage("You are too close to another trap.");

            if (from != null && traps.Count == 0)
            {
                MoveToWorld(from.Location, from.Map);
               
                Active = true;
                Movable = false;

                from.SendMessage("You sucessfully place the trap at your feet.");
            }

            traps.Clear();
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version
         
            writer.Write((Faction == null ? "null" : Faction.FactionName));

            writer.Write(Active);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            if (version > 0)
            {
                FactionName = reader.ReadString();
            }

            if (version > 1)
               Active = reader.ReadBool();
		}
	}

    public class FactionGasTrap        : FactionTrap
    {
        private Poison m_Poison;

        [CommandProperty(AccessLevel.GameMaster)]
        public Poison Poison
        {
            get { return m_Poison; }
            set { m_Poison = value; }
        }

        public FactionGasTrap(Poison p, PlayerFaction a)
            : base(2480)
        {
            Poison = p;
            Faction = a;
            Name = "a pouch";
        }

        public override bool PassivelyTriggered{ get{ return true; } }
		public override TimeSpan PassiveTriggerDelay{ get{ return TimeSpan.Zero; } }
		public override int PassiveTriggerRange{ get{ return 1; } }
		public override TimeSpan ResetDelay{ get{ return TimeSpan.FromSeconds( 1.0 ); } }

		public override void OnTrigger( Mobile from )
		{
            if (from is PlayerMobile && Active)
            {
                PlayerMobile p = from as PlayerMobile;

                if (p.CurrentFaction != Faction)
                {
                    if (m_Poison == null || !from.Player || !from.Alive || from.AccessLevel > AccessLevel.Player)
                        return;

                    Effects.SendLocationEffect(Location, Map, 4410, 16, 3, GetEffectHue(), 0);
                    Effects.PlaySound(Location, Map, 0x231);

                    from.ApplyPoison(from, m_Poison);

                    from.LocalOverheadMessage(MessageType.Regular, 0x22, 500855); // You are enveloped by a noxious gas cloud!

                    Delete();
                }
            }
		}

		public FactionGasTrap( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			Poison.Serialize( m_Poison, writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Poison = Poison.Deserialize( reader );
					break;
				}
			}
		}
    }

    public class FactionExplosionTrap  : FactionTrap
    {
        int damage; public bool hasTriggered = false;

        public FactionExplosionTrap(int dmg, PlayerFaction a)
            : base(7192)
        {
            damage = dmg;
            Faction = a;
            Name = "an explosive flask";
        }

        public override bool PassivelyTriggered{ get{ return true; } }
		public override TimeSpan PassiveTriggerDelay{ get{ return TimeSpan.Zero; } }
		public override int PassiveTriggerRange{ get{ return 1; } }
		public override TimeSpan ResetDelay{ get{ return TimeSpan.FromSeconds( 1.0 ); } }

		public override void OnTrigger( Mobile from )
		{
            List<Item> items = new List<Item>();
            IPooledEnumerable eable = GetItemsInRange(2);
            foreach (Item i in eable)
            {   if (i is FactionExplosionTrap && i != this)
                    items.Add(i); 
            }           Item[] flasks = items.ToArray();

            for (int j = 0; j < flasks.Length; j++)
            {
                if (!((FactionExplosionTrap)flasks[j]).hasTriggered)
                {
                    ((FactionExplosionTrap)flasks[j]).hasTriggered = true;
                    ((FactionExplosionTrap)flasks[j]).OnTrigger(from);
                }
            }

            if (from is PlayerMobile && Active)
            {
                PlayerMobile p = from as PlayerMobile;

                if (p.CurrentFaction != Faction && p.CurrentFaction != null)
                {
                    if ( !from.Player || !from.Alive || from.AccessLevel > AccessLevel.Player)
                        return;

                    Effects.SendLocationEffect(Location, Map, 14000, 16, 3, GetEffectHue(), 0);
                    Effects.PlaySound(Location, Map, 0x208);

                    IPooledEnumerable mobs = GetMobilesInRange(2);
                    foreach (Mobile m in mobs)
                    {
                        int trueDamage = damage;

                        if (m.InRange(this, 2))
                            trueDamage = (int)(damage * 0.66);

                        else if (m.InRange(this, 1))
                            trueDamage = (int)(damage * 0.88);

                        m.Damage(trueDamage + Utility.RandomMinMax(3, 9));
                        m.LocalOverheadMessage(MessageType.Regular, 0x22, true, "The explosion scorches your flesh!");
                    }   mobs.Free();

                    Delete();
                }
            }
		}

		public FactionExplosionTrap( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

            writer.Write(damage);
            writer.Write(hasTriggered);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            if (version > 0)
            {
                damage = reader.ReadInt();
                hasTriggered = reader.ReadBool();
            }
		}
    }

    public class FactionSnareTrap      : FactionTrap
    {
        public FactionSnareTrap(PlayerFaction a)
            : base(3741)
        {
            Hue = 1175;
            Faction = a;
            Name = "a snare";
        }

        public override bool PassivelyTriggered{ get{ return false; } }
		public override TimeSpan PassiveTriggerDelay{ get{ return TimeSpan.Zero; } }
		public override int PassiveTriggerRange{ get{ return 0; } }
		public override TimeSpan ResetDelay{ get{ return TimeSpan.FromSeconds( 1.0 ); } }

		public override void OnTrigger( Mobile from )
		{
            if (from is PlayerMobile && Active)
            {
                PlayerMobile p = from as PlayerMobile;

                if (p.CurrentFaction != Faction && p.CurrentFaction != null)
                {
                    if ( !from.Player || !from.Alive || from.AccessLevel > AccessLevel.Player)
                        return;

                    Effects.PlaySound(Location, Map, 0x143);

                    from.Damage( Utility.RandomMinMax(14, 22));
                    BleedAttack.BeginBleed(from, from);

                    from.LocalOverheadMessage(MessageType.Regular, 0x22, true, "*The snare snaps close around your ankle!*");

                    Delete();
                }
            }
		}

		public FactionSnareTrap( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
    }

    public class FactionTrapKit : Item
    {
        [Constructable]
        public FactionTrapKit()
            : base(7867)
        {
        }

        public FactionTrapKit(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Skills.Tinkering.Value < 80)
            {
                from.SendMessage("You must have atleast 80.0 Tinkering to use this.");
            }

            if (from is PlayerMobile && ((PlayerMobile)from).CurrentFaction != null)
            {
                from.CloseGump(typeof(TrapSelection));
                from.SendGump(new TrapSelection(((PlayerMobile)from).CurrentFaction));
            }

            else from.SendMessage("You must be in an faction to use this item.");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}

namespace Ulmeta.Factions.Vendors
{
    public abstract class FactionVendor : BaseVendor, IFactionEntity
    {
        public PlayerFaction Faction { get; set; }

        public String FactionName { get; set; }

        protected abstract override List<SBInfo> SBInfos { get; }
        public abstract override void InitSBInfo();

        public FactionVendor(string title)
            : base(title)
        {
            Faction = new PlayerFaction();
            Faction.primaryHue = 1408;
            Faction.secondaryHue = 1308;
            Faction.FactionName = "United Backt'rol";
            Faction.mountID = 16033;
            Faction.mountBody = 0xE4;
            Faction.EconomyLevel = 5;

            Robe robe = new Robe();
            robe.Hue = (Utility.RandomBool() ? Faction.primaryHue : Faction.secondaryHue);
            AddItem(robe);
        }

        public FactionVendor(PlayerFaction a, string title)
            : base(title)
        {
            Faction = a;
            Faction.vendorCount++;
        }

        public FactionVendor(Serial serial)
            : base(serial)
        {
        }

        public override void InitOutfit()
        {
            base.InitOutfit();
        }

        private void ProcessSinglePurchase(BuyItemResponse buy, IBuyItemInfo bii, List<BuyItemResponse> validBuy, ref int controlSlots, ref bool fullPurchase, ref int totalCost)
        {
            int amount = buy.Amount;

            if (amount > bii.Amount)
                amount = bii.Amount;

            if (amount <= 0)
                return;

            int slots = bii.ControlSlots * amount;

            if (controlSlots >= slots)
            {
                controlSlots -= slots;
            }
            else
            {
                fullPurchase = false;
                return;
            }

            totalCost += bii.Price * amount;
            validBuy.Add(buy);
        }

        private void ProcessValidPurchase(int amount, IBuyItemInfo bii, Mobile buyer, Container cont)
        {
            if (amount > bii.Amount)
                amount = bii.Amount;

            if (amount < 1)
                return;

            bii.Amount -= amount;

            IEntity o = bii.GetEntity();
            int hue = bii.Hue; //Oh, that was hard

            if (o is Item)
            {
                Item item = (Item)o;

                if (item.Stackable)
                {
                    item.Amount = amount;

                    if (cont == null || !cont.TryDropItem(buyer, item, false))
                        item.MoveToWorld(buyer.Location, buyer.Map);
                }
                else
                {
                    item.Amount = 1;

                    if (item is BaseWeapon || item is BaseShield)
                        item.Hue = Faction.primaryHue;

                    if (item is BaseArmor || item is BaseHat)
                        item.Hue = Faction.secondaryHue;

                    if (item is BaseClothing || item is BaseJewel)
                        item.Hue = (Utility.RandomBool() ? Faction.secondaryHue : Faction.primaryHue);

                    if (item is DyeTub)
                    {
                        item.Hue = hue;
                        ((DyeTub)item).DyedHue = item.Hue;
                    }

                    if (cont == null || !cont.TryDropItem(buyer, item, false))
                        item.MoveToWorld(buyer.Location, buyer.Map);

                    for (int i = 1; i < amount; i++)
                    {
                        item = bii.GetEntity() as Item;

                        if (item != null)
                        {
                            if (item is BaseWeapon || item is BaseShield)
                                item.Hue = Faction.primaryHue;

                            if (item is BaseArmor || item is BaseHat)
                                item.Hue = Faction.primaryHue;

                            if (item is BaseClothing || item is BaseJewel)
                                item.Hue = (Utility.RandomBool() ? Faction.secondaryHue : Faction.primaryHue);

                            if (item is DyeTub)
                            {
                                item.Hue = hue;
                                ((DyeTub)item).DyedHue = item.Hue;
                            }

                            item.Amount = 1;

                            if (cont == null || !cont.TryDropItem(buyer, item, false))
                                item.MoveToWorld(buyer.Location, buyer.Map);
                        }
                    }
                }
            }
            else if (o is Mobile)
            {
                Mobile m = (Mobile)o;

                m.Direction = (Direction)Utility.Random(8);
                m.MoveToWorld(buyer.Location, buyer.Map);
                m.PlaySound(m.GetIdleSound());

                if (m is BaseCreature)
                {
                    ((BaseCreature)m).SetControlMaster(buyer);

                    if (m is WarHorse)
                    {
                        ((WarHorse)m).Body = ((IFactionEntity)this).Faction.mountBody;
                        ((WarHorse)m).ItemID = ((IFactionEntity)this).Faction.mountID;
                        ((WarHorse)m).SetHits(HitsMax + (((IFactionEntity)this).Faction.EconomyLevel * Utility.RandomMinMax(80, 100)));

                        m.Hue = ((IFactionEntity)this).Faction.primaryHue;
                    }
                }

                for (int i = 1; i < amount; ++i)
                {
                    m = bii.GetEntity() as Mobile;

                    if (m != null)
                    {
                        m.Direction = (Direction)Utility.Random(8);
                        m.MoveToWorld(buyer.Location, buyer.Map);

                        if (m is BaseCreature)
                        {
                            ((BaseCreature)m).SetControlMaster(buyer);

                            if (m is WarHorse)
                            {
                                ((WarHorse)m).Body = ((IFactionEntity)this).Faction.mountBody;
                                ((Horse)m).ItemID = ((IFactionEntity)this).Faction.mountID;
                                ((WarHorse)m).SetHits(HitsMax + (((IFactionEntity)this).Faction.EconomyLevel * Utility.RandomMinMax(80, 100)));

                                m.Hue = ((IFactionEntity)this).Faction.primaryHue;
                            }
                        }
                    }
                }
            }
        }

        public override bool OnBuyItems(Mobile buyer, List<BuyItemResponse> list)
        {
            if (!IsActiveSeller)
                return false;

            if (!buyer.CheckAlive())
                return false;

            if (!CheckVendorAccess(buyer))
            {
                Say(501522); // I shall not treat with scum like thee!
                return false;
            }

            if (((IFactionEntity)this).Faction != ((PlayerMobile)buyer).CurrentFaction && buyer.AccessLevel == AccessLevel.Player)
            {
                Say("I can not do business with thee. It is matter of allegiance.");
                return false;
            }

            UpdateBuyInfo();

            IBuyItemInfo[] buyInfo = this.GetBuyInfo();
            IShopSellInfo[] info = GetSellInfo();
            int totalCost = 0;
            List<BuyItemResponse> validBuy = new List<BuyItemResponse>(list.Count);
            Container cont;
            bool bought = false;
            bool fromBank = false;
            bool fullPurchase = true;
            int controlSlots = buyer.FollowersMax - buyer.Followers;

            foreach (BuyItemResponse buy in list)
            {
                Serial ser = buy.Serial;
                int amount = buy.Amount;

                if (ser.IsItem)
                {
                    Item item = World.FindItem(ser);

                    if (item == null)
                        continue;

                    GenericBuyInfo gbi = LookupDisplayObject(item);

                    if (gbi != null)
                    {
                        ProcessSinglePurchase(buy, gbi, validBuy, ref controlSlots, ref fullPurchase, ref totalCost);
                    }
                    else if (item != this.BuyPack && item.IsChildOf(this.BuyPack))
                    {
                        if (amount > item.Amount)
                            amount = item.Amount;

                        if (amount <= 0)
                            continue;

                        foreach (IShopSellInfo ssi in info)
                        {
                            if (ssi.IsSellable(item))
                            {
                                if (ssi.IsResellable(item))
                                {
                                    totalCost += ssi.GetBuyPriceFor(item) * amount;
                                    validBuy.Add(buy);
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (ser.IsMobile)
                {
                    Mobile mob = World.FindMobile(ser);

                    if (mob == null)
                        continue;

                    GenericBuyInfo gbi = LookupDisplayObject(mob);

                    if (gbi != null)
                        ProcessSinglePurchase(buy, gbi, validBuy, ref controlSlots, ref fullPurchase, ref totalCost);
                }
            }//foreach

            if (fullPurchase && validBuy.Count == 0)
                SayTo(buyer, 500190); // Thou hast bought nothing!
            else if (validBuy.Count == 0)
                SayTo(buyer, 500187); // Your order cannot be fulfilled, please try again.

            if (validBuy.Count == 0)
                return false;

            bought = (buyer.AccessLevel >= AccessLevel.GameMaster);

            if (buyer is PlayerMobile && ((PlayerMobile)buyer).CurrentFaction == Faction)
            {
                double discount = Faction.EconomyLevel * 0.05;
                totalCost = totalCost - (int)(totalCost * discount);
                buyer.SendMessage(1410, "You receive a faction discount of {0} coins.", (int)(totalCost * discount));
            }

            cont = buyer.Backpack;
            if (!bought && cont != null)
            {
                if (cont.ConsumeTotal(typeof(Valorite), totalCost))
                    bought = true;
                else if (totalCost < 2000)
                    SayTo(buyer, 500192); // Begging thy pardon, but thou canst not afford that.
            }

            if (!bought && totalCost >= 2000)
            {
                cont = buyer.FindBankNoCreate();
                if (cont != null && cont.ConsumeTotal(typeof(Valorite), totalCost))
                {
                    bought = true;
                    fromBank = true;
                }
                else
                {
                    SayTo(buyer, 500191); //Begging thy pardon, but thy bank account lacks these funds.
                }
            }

            if (!bought)
                return false;
            else
                buyer.PlaySound(0x32);

            cont = buyer.Backpack;
            if (cont == null)
                cont = buyer.BankBox;

            foreach (BuyItemResponse buy in validBuy)
            {
                Serial ser = buy.Serial;
                int amount = buy.Amount;

                if (amount < 1)
                    continue;

                if (ser.IsItem)
                {
                    Item item = World.FindItem(ser);

                    if (item == null)
                        continue;

                    GenericBuyInfo gbi = LookupDisplayObject(item);

                    if (gbi != null)
                    {
                        ProcessValidPurchase(amount, gbi, buyer, cont);
                    }
                    else
                    {
                        if (amount > item.Amount)
                            amount = item.Amount;

                        foreach (IShopSellInfo ssi in info)
                        {
                            if (ssi.IsSellable(item))
                            {
                                if (ssi.IsResellable(item))
                                {
                                    Item buyItem;
                                    if (amount >= item.Amount)
                                    {
                                        buyItem = item;
                                    }
                                    else
                                    {
                                        buyItem = Mobile.LiftItemDupe(item, item.Amount - amount);

                                        if (buyItem == null)
                                            buyItem = item;
                                    }

                                    if (cont == null || !cont.TryDropItem(buyer, buyItem, false))
                                        buyItem.MoveToWorld(buyer.Location, buyer.Map);

                                    break;
                                }
                            }
                        }
                    }
                }
                else if (ser.IsMobile)
                {
                    Mobile mob = World.FindMobile(ser);

                    if (mob == null)
                        continue;

                    GenericBuyInfo gbi = LookupDisplayObject(mob);

                    if (gbi != null)
                        ProcessValidPurchase(amount, gbi, buyer, cont);
                }
            }//foreach

            if (fullPurchase)
            {
                if (buyer.AccessLevel >= AccessLevel.GameMaster)
                    SayTo(buyer, true, "I would not presume to charge thee anything.  Here are the goods you requested.");
                else if (fromBank)
                    SayTo(buyer, true, "The total of thy purchase is {0} gold, which has been withdrawn from your bank account.  My thanks for the patronage.", totalCost);
                else
                    SayTo(buyer, true, "The total of thy purchase is {0} gold.  My thanks for the patronage.", totalCost);
            }
            else
            {
                if (buyer.AccessLevel >= AccessLevel.GameMaster)
                    SayTo(buyer, true, "I would not presume to charge thee anything. Unfortunately, I could not sell you all the goods you requested.");
                else if (fromBank)
                    SayTo(buyer, true, "The total of thy purchase is {0} gold, which has been withdrawn from your bank account.  My thanks for the patronage.  Unfortunately, I could not sell you all the goods you requested.", totalCost);
                else
                    SayTo(buyer, true, "The total of thy purchase is {0} gold.  My thanks for the patronage.  Unfortunately, I could not sell you all the goods you requested.", totalCost);
            }

            return true;
        }

        private GenericBuyInfo LookupDisplayObject(object obj)
        {
            IBuyItemInfo[] buyInfo = this.GetBuyInfo();

            for (int i = 0; i < buyInfo.Length; ++i)
            {
                GenericBuyInfo gbi = (GenericBuyInfo)buyInfo[i];

                if (gbi.GetDisplayEntity() == obj)
                    return gbi;
            }

            return null;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((string)(Faction == null ? "null" : Faction.FactionName));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version > 0)
            {
                FactionName = reader.ReadString();
            }
        }
    }

    public class ArmsDealerSB    : SBInfo
    {
        public PlayerFaction faction;

        private List<GenericBuyInfo> m_BuyInfo;
        private IShopSellInfo m_SellInfo;

        public ArmsDealerSB(PlayerFaction a)
        {
            faction = a;

            if (faction == null)
            {
                faction = new PlayerFaction();
                faction.primaryHue = 1408;
                faction.secondaryHue = 1308;
            }

            m_SellInfo = new InternalSellInfo();
            m_BuyInfo = new InternalBuyInfo(faction);
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo(PlayerFaction a)
            {
                Add(new GenericBuyInfo(typeof(IronIngot), 5, 16, 0x1BF2, 0));
                Add(new GenericBuyInfo(typeof(Tongs), 13, 14, 0xFBB, 0));

                Add(new GenericBuyInfo(typeof(BronzeShield), 66, 20, 0x1B72, a.primaryHue));
                Add(new GenericBuyInfo(typeof(Buckler), 50, 20, 0x1B73, a.primaryHue));
                Add(new GenericBuyInfo(typeof(MetalKiteShield), 123, 20, 0x1B74, a.primaryHue));
                Add(new GenericBuyInfo(typeof(HeaterShield), 231, 20, 0x1B76, a.primaryHue));
                Add(new GenericBuyInfo(typeof(WoodenKiteShield), 70, 20, 0x1B78, a.primaryHue));
                Add(new GenericBuyInfo(typeof(MetalShield), 121, 20, 0x1B7B, a.primaryHue));

                Add(new GenericBuyInfo(typeof(WoodenShield), 30, 20, 0x1B7A, a.primaryHue));

                Add(new GenericBuyInfo(typeof(PlateGorget), 104, 20, 0x1413, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(PlateChest), 243, 20, 0x1415, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(PlateLegs), 218, 20, 0x1411, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(PlateArms), 188, 20, 0x1410, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(PlateGloves), 155, 20, 0x1414, a.secondaryHue));

                Add(new GenericBuyInfo(typeof(PlateHelm), 21, 20, 0x1412, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(CloseHelm), 18, 20, 0x1408, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(CloseHelm), 18, 20, 0x1409, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(Helmet), 31, 20, 0x140A, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(Helmet), 18, 20, 0x140B, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(NorseHelm), 18, 20, 0x140E, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(NorseHelm), 18, 20, 0x140F, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(Bascinet), 18, 20, 0x140C, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(PlateHelm), 21, 20, 0x1419, a.secondaryHue));

                Add(new GenericBuyInfo(typeof(ChainCoif), 17, 20, 0x13BB, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(ChainChest), 143, 20, 0x13BF, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(ChainLegs), 149, 20, 0x13BE, a.secondaryHue));

                Add(new GenericBuyInfo(typeof(RingmailChest), 121, 20, 0x13ec, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(RingmailLegs), 90, 20, 0x13F0, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(RingmailArms), 85, 20, 0x13EE, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(RingmailGloves), 93, 20, 0x13eb, a.secondaryHue));

                Add(new GenericBuyInfo(typeof(ExecutionersAxe), 30, 20, 0xF45, a.primaryHue));
                Add(new GenericBuyInfo(typeof(Bardiche), 60, 20, 0xF4D, a.primaryHue));
                Add(new GenericBuyInfo(typeof(BattleAxe), 26, 20, 0xF47, a.primaryHue));
                Add(new GenericBuyInfo(typeof(TwoHandedAxe), 32, 20, 0x1443, a.primaryHue));
                Add(new GenericBuyInfo(typeof(Bow), 35, 20, 0x13B2, a.primaryHue));
                Add(new GenericBuyInfo(typeof(ButcherKnife), 14, 20, 0x13F6, a.primaryHue));
                Add(new GenericBuyInfo(typeof(Crossbow), 46, 20, 0xF50, a.primaryHue));
                Add(new GenericBuyInfo(typeof(HeavyCrossbow), 55, 20, 0x13FD, a.primaryHue));
                Add(new GenericBuyInfo(typeof(Cutlass), 24, 20, 0x1441, a.primaryHue));
                Add(new GenericBuyInfo(typeof(Dagger), 21, 20, 0xF52, a.primaryHue));
                Add(new GenericBuyInfo(typeof(Halberd), 42, 20, 0x143E, a.primaryHue));
                Add(new GenericBuyInfo(typeof(HammerPick), 26, 20, 0x143D, a.primaryHue));
                Add(new GenericBuyInfo(typeof(Katana), 33, 20, 0x13FF, a.primaryHue));
                Add(new GenericBuyInfo(typeof(Kryss), 32, 20, 0x1401, a.primaryHue));
                Add(new GenericBuyInfo(typeof(Broadsword), 35, 20, 0xF5E, a.primaryHue));
                Add(new GenericBuyInfo(typeof(Longsword), 55, 20, 0xF61, a.primaryHue));
                Add(new GenericBuyInfo(typeof(ThinLongsword), 27, 20, 0x13B8, a.primaryHue));
                Add(new GenericBuyInfo(typeof(VikingSword), 55, 20, 0x13B9, a.primaryHue));
                Add(new GenericBuyInfo(typeof(Cleaver), 15, 20, 0xEC3, a.primaryHue));
                Add(new GenericBuyInfo(typeof(Axe), 40, 20, 0xF49, a.primaryHue));
                Add(new GenericBuyInfo(typeof(DoubleAxe), 52, 20, 0xF4B, a.primaryHue));
                Add(new GenericBuyInfo(typeof(Pickaxe), 22, 20, 0xE86, a.primaryHue));
                Add(new GenericBuyInfo(typeof(Pitchfork), 19, 20, 0xE87, a.primaryHue));
                Add(new GenericBuyInfo(typeof(Scimitar), 36, 20, 0x13B6, a.primaryHue));
                Add(new GenericBuyInfo(typeof(SkinningKnife), 14, 20, 0xEC4, a.primaryHue));
                Add(new GenericBuyInfo(typeof(LargeBattleAxe), 33, 20, 0x13FB, a.primaryHue));
                Add(new GenericBuyInfo(typeof(WarAxe), 29, 20, 0x13B0, a.primaryHue));

                if (Core.SE)
                {
                    Add(new GenericBuyInfo(typeof(BoneHarvester), 35, 20, 0x26BB, a.primaryHue));
                    Add(new GenericBuyInfo(typeof(CrescentBlade), 37, 20, 0x26C1, a.primaryHue));
                    Add(new GenericBuyInfo(typeof(DoubleBladedStaff), 35, 20, 0x26BF, a.primaryHue));
                    Add(new GenericBuyInfo(typeof(Lance), 34, 20, 0x26C0, a.primaryHue));
                    Add(new GenericBuyInfo(typeof(Pike), 39, 20, 0x26BE, a.primaryHue));
                    Add(new GenericBuyInfo(typeof(Scythe), 39, 20, 0x26BA, a.primaryHue));
                    Add(new GenericBuyInfo(typeof(CompositeBow), 50, 20, 0x26C2, a.primaryHue));
                    Add(new GenericBuyInfo(typeof(RepeatingCrossbow), 57, 20, 0x26C3, a.primaryHue));
                }

                Add(new GenericBuyInfo(typeof(BlackStaff), 22, 20, 0xDF1, a.primaryHue));
                Add(new GenericBuyInfo(typeof(Club), 16, 20, 0x13B4, a.primaryHue));
                Add(new GenericBuyInfo(typeof(GnarledStaff), 16, 20, 0x13F8, a.primaryHue));
                Add(new GenericBuyInfo(typeof(Mace), 28, 20, 0xF5C, a.primaryHue));
                Add(new GenericBuyInfo(typeof(Maul), 21, 20, 0x143B, a.primaryHue));
                Add(new GenericBuyInfo(typeof(QuarterStaff), 19, 20, 0xE89, a.primaryHue));
                Add(new GenericBuyInfo(typeof(ShepherdsCrook), 20, 20, 0xE81, a.primaryHue));
                Add(new GenericBuyInfo(typeof(SmithHammer), 21, 20, 0x13E3, a.primaryHue));
                Add(new GenericBuyInfo(typeof(ShortSpear), 23, 20, 0x1403, a.primaryHue));
                Add(new GenericBuyInfo(typeof(Spear), 31, 20, 0xF62, a.primaryHue));
                Add(new GenericBuyInfo(typeof(WarHammer), 25, 20, 0x1439, a.primaryHue));
                Add(new GenericBuyInfo(typeof(WarMace), 31, 20, 0x1407, a.primaryHue));

                if (Core.SE)
                {
                    Add(new GenericBuyInfo(typeof(Scepter), 39, 20, 0x26BC, a.primaryHue));
                    Add(new GenericBuyInfo(typeof(BladedStaff), 40, 20, 0x26BD, a.primaryHue));
                }

                Add(new GenericBuyInfo(typeof(StuddedArms), 87, 20, 0x13DC, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(StuddedChest), 128, 20, 0x13DB, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(StuddedGloves), 79, 20, 0x13D5, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(StuddedGorget), 73, 20, 0x13D6, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(StuddedLegs), 103, 20, 0x13DA, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(FemaleStuddedChest), 142, 20, 0x1C02, a.secondaryHue));
                Add(new GenericBuyInfo(typeof(StuddedBustierArms), 120, 20, 0x1c0c, a.secondaryHue));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(Tongs), 7);
                Add(typeof(IronIngot), 4);

                Add(typeof(Buckler), 25);
                Add(typeof(BronzeShield), 33);
                Add(typeof(MetalShield), 60);
                Add(typeof(MetalKiteShield), 62);
                Add(typeof(HeaterShield), 115);
                Add(typeof(WoodenKiteShield), 35);

                Add(typeof(WoodenShield), 15);

                Add(typeof(PlateArms), 94);
                Add(typeof(PlateChest), 121);
                Add(typeof(PlateGloves), 72);
                Add(typeof(PlateGorget), 52);
                Add(typeof(PlateLegs), 109);

                Add(typeof(FemalePlateChest), 113);
                Add(typeof(FemaleLeatherChest), 18);
                Add(typeof(FemaleStuddedChest), 25);
                Add(typeof(LeatherShorts), 14);
                Add(typeof(LeatherSkirt), 11);
                Add(typeof(LeatherBustierArms), 11);
                Add(typeof(StuddedBustierArms), 27);

                Add(typeof(Bascinet), 9);
                Add(typeof(CloseHelm), 9);
                Add(typeof(Helmet), 9);
                Add(typeof(NorseHelm), 9);
                Add(typeof(PlateHelm), 10);

                Add(typeof(ChainCoif), 6);
                Add(typeof(ChainChest), 71);
                Add(typeof(ChainLegs), 74);

                Add(typeof(RingmailArms), 42);
                Add(typeof(RingmailChest), 60);
                Add(typeof(RingmailGloves), 26);
                Add(typeof(RingmailLegs), 45);

                Add(typeof(BattleAxe), 13);
                Add(typeof(DoubleAxe), 26);
                Add(typeof(ExecutionersAxe), 15);
                Add(typeof(LargeBattleAxe), 16);
                Add(typeof(Pickaxe), 11);
                Add(typeof(TwoHandedAxe), 16);
                Add(typeof(WarAxe), 14);
                Add(typeof(Axe), 20);

                Add(typeof(Bardiche), 30);
                Add(typeof(Halberd), 21);

                Add(typeof(ButcherKnife), 7);
                Add(typeof(Cleaver), 7);
                Add(typeof(Dagger), 10);
                Add(typeof(SkinningKnife), 7);

                Add(typeof(Club), 8);
                Add(typeof(HammerPick), 13);
                Add(typeof(Mace), 14);
                Add(typeof(Maul), 10);
                Add(typeof(WarHammer), 12);
                Add(typeof(WarMace), 15);

                Add(typeof(HeavyCrossbow), 27);
                Add(typeof(Bow), 17);
                Add(typeof(Crossbow), 23);

                if (Core.SE)
                {
                    Add(typeof(CompositeBow), 25);
                    Add(typeof(RepeatingCrossbow), 28);
                    Add(typeof(Scepter), 20);
                    Add(typeof(BladedStaff), 20);
                    Add(typeof(Scythe), 19);
                    Add(typeof(BoneHarvester), 17);
                    Add(typeof(Scepter), 18);
                    Add(typeof(BladedStaff), 16);
                    Add(typeof(Pike), 19);
                    Add(typeof(DoubleBladedStaff), 17);
                    Add(typeof(Lance), 17);
                    Add(typeof(CrescentBlade), 18);
                }

                Add(typeof(Spear), 15);
                Add(typeof(Pitchfork), 9);
                Add(typeof(ShortSpear), 11);

                Add(typeof(BlackStaff), 11);
                Add(typeof(GnarledStaff), 8);
                Add(typeof(QuarterStaff), 9);
                Add(typeof(ShepherdsCrook), 10);

                Add(typeof(SmithHammer), 10);

                Add(typeof(Broadsword), 17);
                Add(typeof(Cutlass), 12);
                Add(typeof(Katana), 16);
                Add(typeof(Kryss), 16);
                Add(typeof(Longsword), 27);
                Add(typeof(Scimitar), 18);
                Add(typeof(ThinLongsword), 13);
                Add(typeof(VikingSword), 27);


            }
        }
    }

    public class OutfitterSB     : SBInfo
    {
        public PlayerFaction faction;

        private List<GenericBuyInfo> m_BuyInfo;
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public OutfitterSB(PlayerFaction a)
        {
            faction = a;

            if (faction == null)
            {
                faction = new PlayerFaction();
                faction.primaryHue = 1408;
                faction.secondaryHue = 1308;
                faction.FactionName = "United Backt'rol";
                faction.mountID = 16033;
                faction.mountBody = 0xE4;
            }

            m_BuyInfo = new InternalBuyInfo(faction);
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo(PlayerFaction a)
            {
                Add(new GenericBuyInfo(typeof(SewingKit), 3, 20, 0xF9D, 0));
                Add(new GenericBuyInfo(typeof(Scissors), 11, 20, 0xF9F, 0));
                Add(new GenericBuyInfo(typeof(DyeTub), 8, 20, 0xFAB, a.primaryHue));
                Add(new GenericBuyInfo(typeof(DyeTub), 8, 20, 0xFAB, a.secondaryHue));

                Add(new GenericBuyInfo(typeof(Shirt), 12, 20, 0x1517, 0));
                Add(new GenericBuyInfo(typeof(ShortPants), 7, 20, 0x152E, 0));
                Add(new GenericBuyInfo(typeof(FancyShirt), 21, 20, 0x1EFD, 0));
                Add(new GenericBuyInfo(typeof(LongPants), 10, 20, 0x1539, 0));
                Add(new GenericBuyInfo(typeof(FancyDress), 26, 20, 0x1EFF, 0));
                Add(new GenericBuyInfo(typeof(PlainDress), 13, 20, 0x1F01, 0));
                Add(new GenericBuyInfo(typeof(Kilt), 11, 20, 0x1537, 0));
                Add(new GenericBuyInfo(typeof(Kilt), 11, 20, 0x1537, 0));
                Add(new GenericBuyInfo(typeof(HalfApron), 10, 20, 0x153b, 0));
                Add(new GenericBuyInfo(typeof(Robe), 18, 20, 0x1F03, 0));
                Add(new GenericBuyInfo(typeof(Cloak), 8, 20, 0x1515, 0));
                Add(new GenericBuyInfo(typeof(Cloak), 8, 20, 0x1515, 0));
                Add(new GenericBuyInfo(typeof(Doublet), 13, 20, 0x1F7B, 0));
                Add(new GenericBuyInfo(typeof(Tunic), 18, 20, 0x1FA1, 0));
                Add(new GenericBuyInfo(typeof(JesterSuit), 26, 20, 0x1F9F, 0));

                Add(new GenericBuyInfo(typeof(JesterHat), 12, 20, 0x171C, 0));
                Add(new GenericBuyInfo(typeof(FloppyHat), 7, 20, 0x1713, 0));
                Add(new GenericBuyInfo(typeof(WideBrimHat), 8, 20, 0x1714, 0));
                Add(new GenericBuyInfo(typeof(Cap), 10, 20, 0x1715, 0));
                Add(new GenericBuyInfo(typeof(TallStrawHat), 8, 20, 0x1716, 0));
                Add(new GenericBuyInfo(typeof(StrawHat), 7, 20, 0x1717, 0));
                Add(new GenericBuyInfo(typeof(WizardsHat), 11, 20, 0x1718, 0));
                Add(new GenericBuyInfo(typeof(LeatherCap), 10, 20, 0x1DB9, 0));
                Add(new GenericBuyInfo(typeof(FeatheredHat), 10, 20, 0x171A, 0));
                Add(new GenericBuyInfo(typeof(TricorneHat), 8, 20, 0x171B, 0));
                Add(new GenericBuyInfo(typeof(Bandana), 6, 20, 0x1540, 0));
                Add(new GenericBuyInfo(typeof(SkullCap), 7, 20, 0x1544, 0));

                Add(new GenericBuyInfo(typeof(BoltOfCloth), 100, 20, 0xf95, 0));

                Add(new GenericBuyInfo(typeof(Cloth), 2, 20, 0x1766, 0));
                Add(new GenericBuyInfo(typeof(UncutCloth), 2, 20, 0x1767, 0));

                Add(new GenericBuyInfo(typeof(Cotton), 102, 20, 0xDF9, 0));
                Add(new GenericBuyInfo(typeof(Wool), 62, 20, 0xDF8, 0));
                Add(new GenericBuyInfo(typeof(Flax), 102, 20, 0x1A9C, 0));
                Add(new GenericBuyInfo(typeof(SpoolOfThread), 18, 20, 0xFA0, 0));

                Add(new GenericBuyInfo(typeof(LeatherArms), 80, 20, 0x13CD, 0));
                Add(new GenericBuyInfo(typeof(LeatherChest), 101, 20, 0x13CC, 0));
                Add(new GenericBuyInfo(typeof(LeatherGloves), 60, 20, 0x13C6, 0));
                Add(new GenericBuyInfo(typeof(LeatherGorget), 74, 20, 0x13C7, 0));
                Add(new GenericBuyInfo(typeof(LeatherLegs), 80, 20, 0x13cb, 0));
                Add(new GenericBuyInfo(typeof(LeatherCap), 10, 20, 0x1DB9, 0));
                Add(new GenericBuyInfo(typeof(FemaleLeatherChest), 116, 20, 0x1C06, 0));
                Add(new GenericBuyInfo(typeof(LeatherBustierArms), 97, 20, 0x1C0A, 0));
                Add(new GenericBuyInfo(typeof(LeatherShorts), 86, 20, 0x1C00, 0));
                Add(new GenericBuyInfo(typeof(LeatherSkirt), 87, 20, 0x1C08, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(Scissors), 6);
                Add(typeof(SewingKit), 1);
                Add(typeof(Dyes), 4);
                Add(typeof(DyeTub), 4);

                Add(typeof(BoltOfCloth), 50);

                Add(typeof(FancyShirt), 10);
                Add(typeof(Shirt), 6);

                Add(typeof(ShortPants), 3);
                Add(typeof(LongPants), 5);

                Add(typeof(Cloak), 4);
                Add(typeof(FancyDress), 12);
                Add(typeof(Robe), 9);
                Add(typeof(PlainDress), 7);

                Add(typeof(Skirt), 5);
                Add(typeof(Kilt), 5);

                Add(typeof(Doublet), 7);
                Add(typeof(Tunic), 9);
                Add(typeof(JesterSuit), 13);

                Add(typeof(FullApron), 5);
                Add(typeof(HalfApron), 5);

                Add(typeof(JesterHat), 6);
                Add(typeof(FloppyHat), 3);
                Add(typeof(WideBrimHat), 4);
                Add(typeof(Cap), 5);
                Add(typeof(SkullCap), 3);
                Add(typeof(Bandana), 3);
                Add(typeof(TallStrawHat), 4);
                Add(typeof(StrawHat), 4);
                Add(typeof(WizardsHat), 5);
                Add(typeof(Bonnet), 4);
                Add(typeof(FeatheredHat), 5);
                Add(typeof(TricorneHat), 4);

                Add(typeof(SpoolOfThread), 9);

                Add(typeof(Flax), 51);
                Add(typeof(Cotton), 51);
                Add(typeof(Wool), 31);
            }
        }
    }

    public class HorseBreederSB  : SBInfo
    {
        PlayerFaction faction;

        private List<GenericBuyInfo> m_BuyInfo;
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public HorseBreederSB(PlayerFaction a)
        {
            faction = a;
            m_BuyInfo = new InternalBuyInfo(a);
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo(PlayerFaction a)
            {
                Add(new AnimalBuyInfo(1, typeof(WarHorse), 5000, 10, 204, a.primaryHue));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }

    public class EngineerSB      : SBInfo
    {
        public PlayerFaction faction;

        private List<GenericBuyInfo> m_BuyInfo;
        private IShopSellInfo m_SellInfo;

        public EngineerSB(PlayerFaction a)
        {
            faction = a;

            if (faction == null)
            {
                faction = new PlayerFaction();
                faction.primaryHue = 1408;
                faction.secondaryHue = 1308;
            }

            m_SellInfo = new InternalSellInfo();
            m_BuyInfo = new InternalBuyInfo(faction);
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo(PlayerFaction a)
            {
                Add(new GenericBuyInfo(typeof(IronIngot), 5, 16, 0x1BF2, 0));
                Add(new GenericBuyInfo(typeof(Tongs), 13, 14, 0xFBB, 0));
                Add(new GenericBuyInfo(typeof(Axle), 2, 20, 0x105B, 0));
                Add(new GenericBuyInfo(typeof(Springs), 3, 20, 0x105D, 0));
                Add(new GenericBuyInfo(typeof(Gears), 2, 20, 0x1053, 0));
                Add(new GenericBuyInfo(typeof(Hinge), 2, 20, 0x1055, 0));

                Add(new GenericBuyInfo(typeof(FactionTrapKit), 6, 20, 7867, 0));

                Add(new GenericBuyInfo(typeof(GreaterPoisonPotion), 15, 10, 0xF0A, 0));
                Add(new GenericBuyInfo(typeof(GreaterExplosionPotion), 21, 10, 0xF0D, 0));

            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }

    public class MerchantSB      : SBInfo
    {
        public PlayerFaction faction;

        private List<GenericBuyInfo> m_BuyInfo;
        private IShopSellInfo m_SellInfo;

        public MerchantSB(PlayerFaction a)
        {
            faction = a;

            if (faction == null)
            {
                faction = new PlayerFaction();
                faction.primaryHue = 1408;
                faction.secondaryHue = 1308;
            }

            m_SellInfo = new InternalSellInfo();
            m_BuyInfo = new InternalBuyInfo(faction);
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo(PlayerFaction a)
            {
                int count = a.EconomyLevel * 100;
                Add(new GenericBuyInfo(typeof(BlackPearl), 5, count, 0xF7A, 0));
                Add(new GenericBuyInfo(typeof(Bloodmoss), 5, count, 0xF7B, 0));
                Add(new GenericBuyInfo(typeof(Garlic), 3, count, 0xF84, 0));
                Add(new GenericBuyInfo(typeof(Ginseng), 3, count, 0xF85, 0));
                Add(new GenericBuyInfo(typeof(MandrakeRoot), 3, count, 0xF86, 0));
                Add(new GenericBuyInfo(typeof(Nightshade), 3, count, 0xF88, 0));
                Add(new GenericBuyInfo(typeof(SpidersSilk), 3, count, 0xF8D, 0));
                Add(new GenericBuyInfo(typeof(SulfurousAsh), 3, count, 0xF8C, 0));

                Add(new GenericBuyInfo(typeof(IronIngot), 5, count, 0x1BF2, 0));
                Add(new GenericBuyInfo(typeof(BlankScroll), 5, count, 0x0E34, 0));
                Add(new GenericBuyInfo(typeof(Board), 3, 20, 0x1BD7, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }

    public class HorseBreeder    : FactionVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public HorseBreeder()
            : base("the horse breeder")
        {
            SetSkill(SkillName.AnimalLore, 94.0, 100.0);
            SetSkill(SkillName.AnimalTaming, 90.0, 100.0);
            SetSkill(SkillName.Veterinary, 95.0, 88.0);

            m_SBInfos.Add(new HorseBreederSB(Faction));
        }

        public HorseBreeder(PlayerFaction a)
            : base(a, "the horse breeder")
        {
            Faction = a;

            SetSkill(SkillName.AnimalLore, 94.0, 100.0);
            SetSkill(SkillName.AnimalTaming, 90.0, 100.0);
            SetSkill(SkillName.Veterinary, 95.0, 88.0);

            m_SBInfos.Add(new HorseBreederSB(Faction));
        }

        private class StableEntry : ContextMenuEntry
        {
            private HorseBreeder m_Trainer;
            private Mobile m_From;

            public StableEntry(HorseBreeder trainer, Mobile from)
                : base(6126, 12)
            {
                m_Trainer = trainer;
                m_From = from;
            }

            public override void OnClick()
            {
                m_Trainer.BeginStable(m_From);
            }
        }

        private class ClaimListGump : Gump
        {
            private HorseBreeder m_Trainer;
            private Mobile m_From;
            private List<BaseCreature> m_List;

            public ClaimListGump(HorseBreeder trainer, Mobile from, List<BaseCreature> list)
                : base(50, 50)
            {
                m_Trainer = trainer;
                m_From = from;
                m_List = list;

                from.CloseGump(typeof(ClaimListGump));

                AddPage(0);

                AddBackground(0, 0, 325, 50 + (list.Count * 20), 9250);
                AddAlphaRegion(5, 5, 315, 40 + (list.Count * 20));

                AddHtml(15, 15, 275, 20, "<BASEFONT COLOR=#FFFFFF>Select a pet to retrieve from the stables:</BASEFONT>", false, false);

                for (int i = 0; i < list.Count; ++i)
                {
                    BaseCreature pet = list[i];

                    if (pet == null || pet.Deleted)
                        continue;

                    AddButton(15, 39 + (i * 20), 10006, 10006, i + 1, GumpButtonType.Reply, 0);
                    AddHtml(32, 35 + (i * 20), 275, 18, String.Format("<BASEFONT COLOR=#C0C0EE>{0}</BASEFONT>", pet.Name), false, false);
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                int index = info.ButtonID - 1;

                if (index >= 0 && index < m_List.Count)
                    m_Trainer.EndClaimList(m_From, m_List[index]);
            }
        }

        private class ClaimAllEntry : ContextMenuEntry
        {
            private HorseBreeder m_Trainer;
            private Mobile m_From;

            public ClaimAllEntry(HorseBreeder trainer, Mobile from)
                : base(6127, 12)
            {
                m_Trainer = trainer;
                m_From = from;
            }

            public override void OnClick()
            {
                m_Trainer.Claim(m_From);
            }
        }

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive)
            {
                list.Add(new StableEntry(this, from));

                if (from.Stabled.Count > 0)
                    list.Add(new ClaimAllEntry(this, from));
            }

            base.AddCustomContextEntries(from, list);
        }

        public static int GetMaxStabled(Mobile from)
        {
            double taming = from.Skills[SkillName.AnimalTaming].Value;
            double anlore = from.Skills[SkillName.AnimalLore].Value;
            double vetern = from.Skills[SkillName.Veterinary].Value;
            double sklsum = taming + anlore + vetern;

            int max;

            if (sklsum >= 240.0)
                max = 5;
            else if (sklsum >= 200.0)
                max = 4;
            else if (sklsum >= 160.0)
                max = 3;
            else
                max = 2;

            if (taming >= 100.0)
                max += (int)((taming - 90.0) / 10);

            if (anlore >= 100.0)
                max += (int)((anlore - 90.0) / 10);

            if (vetern >= 100.0)
                max += (int)((vetern - 90.0) / 10);

            return max;
        }

        private class StableTarget : Target
        {
            private HorseBreeder m_Trainer;

            public StableTarget(HorseBreeder trainer)
                : base(12, false, TargetFlags.None)
            {
                m_Trainer = trainer;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BaseCreature)
                    m_Trainer.EndStable(from, (BaseCreature)targeted);
                else if (targeted == from)
                    m_Trainer.SayTo(from, 502672); // HA HA HA! Sorry, I am not an inn.
                else
                    m_Trainer.SayTo(from, 1048053); // You can't stable that!
            }
        }

        private void CloseClaimList(Mobile from)
        {
            from.CloseGump(typeof(ClaimListGump));
        }

        public void BeginClaimList(Mobile from)
        {
            if (Deleted || !from.CheckAlive())
                return;

            List<BaseCreature> list = new List<BaseCreature>();

            for (int i = 0; i < from.Stabled.Count; ++i)
            {
                BaseCreature pet = from.Stabled[i] as BaseCreature;

                if (pet == null || pet.Deleted)
                {
                    pet.IsStabled = false;
                    from.Stabled.RemoveAt(i);
                    --i;
                    continue;
                }

                list.Add(pet);
            }

            if (list.Count > 0)
                from.SendGump(new ClaimListGump(this, from, list));
            else
                SayTo(from, 502671); // But I have no animals stabled with me at the moment!
        }

        public void EndClaimList(Mobile from, BaseCreature pet)
        {
            if (pet == null || pet.Deleted || from.Map != this.Map || !from.InRange(this, 14) || !from.Stabled.Contains(pet) || !from.CheckAlive())
                return;

            if (CanClaim(from, pet))
            {
                DoClaim(from, pet);

                from.Stabled.Remove(pet);

                SayTo(from, 1042559); // Here you go... and good day to you!
            }
            else
            {
                SayTo(from, 1049612, pet.Name); // ~1_NAME~ remained in the stables because you have too many followers.
            }
        }

        public void BeginStable(Mobile from)
        {
            if (Deleted || !from.CheckAlive())
                return;

            if (from.Stabled.Count >= GetMaxStabled(from))
            {
                SayTo(from, 1042565); // You have too many pets in the stables!
            }
            else
            {
                SayTo(from, true, "I charge 30 copper per pet for a real week's stable time. I will withdraw it from thy bank account. Which animal wouldst thou like to stable here?");
                //SayTo( from, 1042558 ); /* I charge 30 gold per pet for a real week's stable time.
                //                         * I will withdraw it from thy bank account.
                //                         * Which animal wouldst thou like to stable here?
                //                         */

                from.Target = new StableTarget(this);
            }
        }

        public void EndStable(Mobile from, BaseCreature pet)
        {
            if (Deleted || !from.CheckAlive())
                return;

            if (!pet.Controlled || pet.ControlMaster != from)
            {
                SayTo(from, 1042562); // You do not own that pet!
            }
            else if (pet.IsDeadPet)
            {
                SayTo(from, 1049668); // Living pets only, please.
            }
            else if (pet.Summoned)
            {
                SayTo(from, 502673); // I can not stable summoned creatures.
            }
            else if (pet.Body.IsHuman)
            {
                SayTo(from, 502672); // HA HA HA! Sorry, I am not an inn.
            }
            else if ((pet is PackLlama || pet is PackHorse || pet is Beetle) && (pet.Backpack != null && pet.Backpack.Items.Count > 0))
            {
                SayTo(from, 1042563); // You need to unload your pet.
            }
            else if (pet.Combatant != null && pet.InRange(pet.Combatant, 12) && pet.Map == pet.Combatant.Map)
            {
                SayTo(from, 1042564); // I'm sorry.  Your pet seems to be busy.
            }
            else if (from.Stabled.Count >= GetMaxStabled(from))
            {
                SayTo(from, 1042565); // You have too many pets in the stables!
            }
            else
            {
                Container bank = from.FindBankNoCreate();

                if (bank != null && bank.ConsumeTotal(typeof(Valorite), 30))
                {
                    pet.ControlTarget = null;
                    pet.ControlOrder = OrderType.Stay;
                    pet.Internalize();

                    pet.SetControlMaster(null);
                    pet.SummonMaster = null;

                    pet.IsStabled = true;

                    if (Core.SE)
                        pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully happy

                    from.Stabled.Add(pet);

                    SayTo(from, 502679); // Very well, thy pet is stabled. Thou mayst recover it by saying 'claim' to me. In one real world week, I shall sell it off if it is not claimed!
                }
                else
                {
                    SayTo(from, 502677); // But thou hast not the funds in thy bank account!
                }
            }
        }

        public void Claim(Mobile from)
        {
            Claim(from, null);
        }

        public void Claim(Mobile from, string petName)
        {
            if (Deleted || !from.CheckAlive())
                return;

            bool claimed = false;
            int stabled = 0;

            bool claimByName = (petName != null);

            for (int i = 0; i < from.Stabled.Count; ++i)
            {
                BaseCreature pet = from.Stabled[i] as BaseCreature;

                if (pet == null || pet.Deleted)
                {
                    pet.IsStabled = false;
                    from.Stabled.RemoveAt(i);
                    --i;
                    continue;
                }

                ++stabled;

                if (claimByName && !Insensitive.Equals(pet.Name, petName))
                    continue;

                if (CanClaim(from, pet))
                {
                    DoClaim(from, pet);

                    from.Stabled.RemoveAt(i);
                    --i;

                    claimed = true;
                }
                else
                {
                    SayTo(from, 1049612, pet.Name); // ~1_NAME~ remained in the stables because you have too many followers.
                }
            }

            if (claimed)
                SayTo(from, 1042559); // Here you go... and good day to you!
            else if (stabled == 0)
                SayTo(from, 502671); // But I have no animals stabled with me at the moment!
            else if (claimByName)
                BeginClaimList(from);
        }

        public bool CanClaim(Mobile from, BaseCreature pet)
        {
            return ((from.Followers + pet.ControlSlots) <= from.FollowersMax);
        }

        private void DoClaim(Mobile from, BaseCreature pet)
        {
            pet.SetControlMaster(from);

            if (pet.Summoned)
                pet.SummonMaster = from;

            pet.ControlTarget = from;
            pet.ControlOrder = OrderType.Follow;

            pet.MoveToWorld(from.Location, from.Map);

            pet.IsStabled = false;

            if (Core.SE)
                pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully Happy
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            return true;
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (!e.Handled && e.HasKeyword(0x0008)) // *stable*
            {
                e.Handled = true;

                CloseClaimList(e.Mobile);
                BeginStable(e.Mobile);
            }
            else if (!e.Handled && e.HasKeyword(0x0009)) // *claim*
            {
                e.Handled = true;

                CloseClaimList(e.Mobile);

                int index = e.Speech.IndexOf(' ');

                if (index != -1)
                    Claim(e.Mobile, e.Speech.Substring(index).Trim());
                else
                    Claim(e.Mobile);
            }
            else
            {
                base.OnSpeech(e);
            }
        }

        public override void InitSBInfo()
        {
            if (Faction != null)
                m_SBInfos.Add(new HorseBreederSB(Faction));
        }

        public override VendorShoeType ShoeType
        {
            get { return Female ? VendorShoeType.ThighBoots : VendorShoeType.Boots; }
        }

        public override int GetShoeHue()
        {
            return 0;
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            AddItem(Utility.RandomBool() ? (Item)new QuarterStaff() : (Item)new ShepherdsCrook());
        }

        public HorseBreeder(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class Engineer        : FactionVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        public override NpcGuild NpcGuild { get { return NpcGuild.None; } }

        [Constructable]
        public Engineer()
            : base("the engineer")
        {
            m_SBInfos.Add(new EngineerSB(Faction));

            SetSkill(SkillName.ArmsLore, 86.0, 98.0);
            SetSkill(SkillName.Blacksmith, 85.0, 88.0);
            SetSkill(SkillName.Fencing, 80.0, 83.0);
            SetSkill(SkillName.Macing, 81.0, 93.0);
            SetSkill(SkillName.Swords, 80.0, 83.0);
            SetSkill(SkillName.Tactics, 80.0, 83.0);
            SetSkill(SkillName.Parry, 81.0, 93.0);
        }

        public Engineer(PlayerFaction a)
            : base(a, "the engineer")
        {
            Faction = a;
            m_SBInfos.Add(new EngineerSB(Faction));

            SetSkill(SkillName.ArmsLore, 86.0, 98.0);
            SetSkill(SkillName.Blacksmith, 85.0, 88.0);
            SetSkill(SkillName.Fencing, 80.0, 83.0);
            SetSkill(SkillName.Macing, 81.0, 93.0);
            SetSkill(SkillName.Swords, 80.0, 83.0);
            SetSkill(SkillName.Tactics, 80.0, 83.0);
            SetSkill(SkillName.Parry, 81.0, 93.0);
        }

        public override void InitSBInfo()
        {
            if (Faction != null)
                m_SBInfos.Add(new EngineerSB(Faction));
        }

        public override VendorShoeType ShoeType
        {
            get { return VendorShoeType.Boots; }
        }

        public override void InitOutfit()
        {
            Item item = new RingmailChest();
            item.Hue = (Faction == null ? 0 : Faction.primaryHue);
            AddItem(item);

            if (item != null && !EquipItem(item))
            {
                item.Delete();
                item = null;
            }

            Item apron = new FullApron();
            apron.Hue = (Faction == null ? 0 : Faction.secondaryHue);
            AddItem(apron);

            AddItem(new SmithHammer());

            base.InitOutfit();
        }

        public Engineer(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class ArmsDealer      : FactionVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        public override NpcGuild NpcGuild { get { return NpcGuild.None; } }

        [Constructable]
        public ArmsDealer()
            : base("the arms dealer")
        {
            m_SBInfos.Add(new ArmsDealerSB(Faction));

            SetSkill(SkillName.ArmsLore, 86.0, 98.0);
            SetSkill(SkillName.Blacksmith, 85.0, 88.0);
            SetSkill(SkillName.Fencing, 80.0, 83.0);
            SetSkill(SkillName.Macing, 81.0, 93.0);
            SetSkill(SkillName.Swords, 80.0, 83.0);
            SetSkill(SkillName.Tactics, 80.0, 83.0);
            SetSkill(SkillName.Parry, 81.0, 93.0);
        }

        public ArmsDealer(PlayerFaction a)
            : base(a, "the arms dealer")
        {
            m_SBInfos.Add(new ArmsDealerSB(Faction));

            SetSkill(SkillName.ArmsLore, 86.0, 98.0);
            SetSkill(SkillName.Blacksmith, 85.0, 88.0);
            SetSkill(SkillName.Fencing, 80.0, 83.0);
            SetSkill(SkillName.Macing, 81.0, 93.0);
            SetSkill(SkillName.Swords, 80.0, 83.0);
            SetSkill(SkillName.Tactics, 80.0, 83.0);
            SetSkill(SkillName.Parry, 81.0, 93.0);
        }

        public override void InitSBInfo()
        {
            if (Faction != null)
                m_SBInfos.Add(new ArmsDealerSB(Faction));
        }

        public override VendorShoeType ShoeType
        {
            get { return VendorShoeType.Boots; }
        }

        public override void InitOutfit()
        {
            Item item = new RingmailChest();
            item.Hue = (Faction == null ? 0 : Faction.primaryHue);
            AddItem(item);

            if (item != null && !EquipItem(item))
            {
                item.Delete();
                item = null;
            }

            Item apron = new FullApron();
            apron.Hue = (Faction == null ? 0 : Faction.secondaryHue);
            AddItem(apron);

            AddItem(new SmithHammer());

            base.InitOutfit();
        }

        public ArmsDealer(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class Outfitter       : FactionVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        public override NpcGuild NpcGuild { get { return NpcGuild.TailorsGuild; } }

        [Constructable]
        public Outfitter()
            : base("the outfitter")
        {
            m_SBInfos.Add(new OutfitterSB(Faction));

            SetSkill(SkillName.Tailoring, 94.0, 100.0);
        }

        public Outfitter(PlayerFaction a)
            : base(a, "the outfitter")
        {
            m_SBInfos.Add(new OutfitterSB(Faction));

            SetSkill(SkillName.Tailoring, 94.0, 100.0);
        }

        public override void InitSBInfo()
        {
            if (Faction != null)
                m_SBInfos.Add(new OutfitterSB(Faction));
        }

        public override VendorShoeType ShoeType
        {
            get { return Utility.RandomBool() ? VendorShoeType.Sandals : VendorShoeType.Shoes; }
        }

        public Outfitter(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class FactionMerchant : FactionVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        public override NpcGuild NpcGuild { get { return NpcGuild.TailorsGuild; } }

        [Constructable]
        public FactionMerchant()
            : base("the merchant")
        {
            m_SBInfos.Add(new MerchantSB(Faction));

            SetSkill(SkillName.Tailoring, 94.0, 100.0);
        }

        public FactionMerchant(PlayerFaction a)
            : base(a, "the merchant")
        {
            m_SBInfos.Add(new MerchantSB(Faction));

            SetSkill(SkillName.Tailoring, 94.0, 100.0);
        }

        public override void InitSBInfo()
        {
            if (Faction != null)
                m_SBInfos.Add(new MerchantSB(Faction));
        }

        public override VendorShoeType ShoeType
        {
            get { return Utility.RandomBool() ? VendorShoeType.Sandals : VendorShoeType.Shoes; }
        }

        public FactionMerchant(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
