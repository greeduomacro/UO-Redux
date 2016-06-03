using Server;
using Server.Commands;
using Server.Misc;
using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EqPvP
{
    /// <summary>
    /// Interface required by mobiles to be engaged in the system.
    /// </summary>
    interface IPlayerCombatant
    {
        CombatantProfile CombatProfile { get; set; }
    }

    /// <summary>
    /// Main module responsible for data structure and inter-module operations.
    /// </summary>
    internal class Handler
    {
        private static readonly string SavePath = "Saves\\eqPvp";
        private static readonly string SaveFile = Path.Combine(SavePath, "eqPvp.bin");

        private static List<CombatantProfile> 
            m_CombatProfiles = new List<CombatantProfile>();

        internal List<CombatantProfile> CombatProfiles
        {
            get { return m_CombatProfiles; }
            set { m_CombatProfiles = value; }
        }

        [CommandAttribute("IncEqDeaths", AccessLevel.GameMaster)]
        public static void IncDeaths_OnCommand(CommandEventArgs args)
        {
            try
            {
                string arg = args.ArgString.Trim();
                int mod; Int32.TryParse(arg, out mod);
                args.Mobile.Target = new ProfileModTarget(args.Mobile, mod, ProfileModTarget.ModType.Deaths);
            }
            catch (Exception e) { eqUtility.HandleMobileException(e, args.Mobile); }
        }

        [CommandAttribute("IncEqKills", AccessLevel.GameMaster)]
        public static void IncKills_OnCommand(CommandEventArgs args)
        {
            try
            {
                string arg = args.ArgString.Trim();
                int mod; Int32.TryParse(arg, out mod);
                args.Mobile.Target = new ProfileModTarget(args.Mobile, mod, ProfileModTarget.ModType.Kills);
            }
            catch (Exception e) { eqUtility.HandleMobileException(e, args.Mobile); }
        }

        [CommandAttribute("IncEqRating", AccessLevel.GameMaster)]
        public static void IncRating_OnCommand(CommandEventArgs args)
        {
            try
            {
                string arg = args.ArgString.Trim();
                int mod; Int32.TryParse(arg, out mod);
                args.Mobile.Target = new ProfileModTarget(args.Mobile, mod, ProfileModTarget.ModType.Rating);
            }
            catch (Exception e) { eqUtility.HandleMobileException(e, args.Mobile); }
        }

        internal enum CombatResults
        {
            None,
            Victory,
            Loss,
            Draw
        }

        public static void Configure()
        {
            EventSink.WorldLoad += new WorldLoadEventHandler(Event_WorldLoad);
        }

        public static void Initialize()
        {
            EventSink.Speech += new SpeechEventHandler(HookSpeech);
            EventSink.WorldSave += new WorldSaveEventHandler(Event_WorldSave);
            EventSink.PlayerDeath += new PlayerDeathEventHandler(OnCombatantDeath);
            EventSink.CharacterCreated += new CharacterCreatedEventHandler(OnCombatantCreated);
            EventSink.Login += new LoginEventHandler(QueryCombatProfile);

            Console.WriteLine("[EqPvP]: Engine Initialized.");
        }

        private static void QueryCombatProfile(LoginEventArgs e)
        {
            try
            {
                if (e.Mobile is PlayerMobile)
                {
                    IPlayerCombatant combatant = e.Mobile as IPlayerCombatant;
                    if (combatant.CombatProfile == null)
                        CreateCombatProfile(e.Mobile);
                }
            }
            catch (Exception x) { eqUtility.HandleGenericException(x); }
        }

        private static void OnCombatantCreated(CharacterCreatedEventArgs e)
        {
            CreateCombatProfile(e.Mobile);
        }

        private static void CreateCombatProfile(Mobile m)
        {
            if (m is PlayerMobile)
            {
                try
                {
                    CombatantProfile cProfile = new CombatantProfile(m as PlayerMobile);
                    m_CombatProfiles.Add(cProfile);
                    Console.WriteLine
                        ("New Combat Profile Created For: ({0}) [{1}]", m.Name, m.Serial);
                }
                catch (Exception e) { eqUtility.HandleGenericException(e); }
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

                writer.Close();
            }

            catch (ArgumentException e)
            {
                eqUtility.HandleGenericException(e);
            }
        }

        private static void Serialize(BinaryFileWriter writer)
        {
            writer.Write((int)0); //Version

            if (m_CombatProfiles != null)
                writer.Write((int)m_CombatProfiles.Count);

            else writer.Write((int)0);

            foreach (CombatantProfile p in m_CombatProfiles)
            {
                p.Serialize(writer);
            }
        }

        private static void Event_WorldLoad()
        {
            if (!File.Exists(SaveFile))
                return;

            try
            {
                using (FileStream stream = new FileStream(SaveFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryFileReader reader = new BinaryFileReader(new BinaryReader(stream));
                    Deserialize(reader);
                    reader.Close();
                }
            }

            catch (ArgumentException e)
            {
                Console.WriteLine("Error: Event_WorldLoad Failed in eqPvp Definition..!");
                eqUtility.HandleGenericException(e);
            }
        }

        private static void Deserialize(BinaryFileReader reader)
        {
            int version = reader.ReadInt();

            if (version >= 0)
            {
                int profileCount = reader.ReadInt();
                if (profileCount > 0)
                {
                    for (int z = 0; z < profileCount; z++)
                    {
                        CombatantProfile p = new CombatantProfile();
                        p.Deserialize(reader);
                        m_CombatProfiles.Add(p);
                    }
                }
            }
        }

        private static void HookSpeech(SpeechEventArgs e)
        {
        }

        private static void OnCombatantDeath(PlayerDeathEventArgs e)
        {
            try
            {
                PlayerMobile victim, killer;
                if (e.Mobile is PlayerMobile && e.Mobile.LastKiller is PlayerMobile)
                {
                    victim = e.Mobile as PlayerMobile;
                    killer = victim.LastKiller as PlayerMobile;

                    CombatantProfile victor = ((IPlayerCombatant)killer).CombatProfile;
                    CombatantProfile defeated = ((IPlayerCombatant)victim).CombatProfile;

                    Handler.IncreaseDeaths(defeated);
                    Handler.IncreaseKills(victor);

                    QueryRankChange(victor);
                    QueryRankChange(defeated);

                    Handler.CalculateNewRating(victor, defeated.CombatRating, CombatResults.Victory);
                    Handler.CalculateNewRating(defeated, victor.CombatRating, CombatResults.Loss);
                }
            }
            catch (Exception x) 
            {
                eqUtility.HandleGenericException(x);
            }
        }

        internal static void QueryRankChange(CombatantProfile profile)
        {
            int kdSum = profile.TotalKills - profile.TotalDeaths;
            int rank = kdSum / 20;
            switch (rank)
            {
                case 0:
                    {
                        profile.Rank = CombatantProfile.CombatRank.Pawn;
                        break;
                    }
                case 1:
                    {
                        profile.Rank = CombatantProfile.CombatRank.Bishop;
                        break;
                    }
                case 2:
                    {
                        profile.Rank = CombatantProfile.CombatRank.Knight;
                        break;
                    }
                case 4:
                    {
                        profile.Rank = CombatantProfile.CombatRank.Rook;
                        break;
                    }
                case 5:
                    {
                        profile.Rank = (profile.IsFemale() 
                            ? CombatantProfile.CombatRank.Queen : CombatantProfile.CombatRank.King);
                        break;
                    }
                case 6:
                    {
                        profile.Rank = CombatantProfile.CombatRank.Master;
                        break;
                    }

                default: break;
            }
        }

        internal static void IncreaseKills(CombatantProfile profile)
        {
            try
            {
                profile.TotalKills++;
            }
            catch (Exception e) { eqUtility.HandleGenericException(e); }
        }

        internal static void IncreaseDeaths(CombatantProfile profile)
        {
            try
            {
                profile.TotalDeaths++;
            }
            catch (Exception e) { eqUtility.HandleGenericException(e); }
        }

        internal static int CalculateNewRating(CombatantProfile profile, int opRating, CombatResults results)
        {
            try
            {
                int currentRating = profile.CombatRating, newRating = currentRating;

                switch (results)
                {
                    case CombatResults.Victory: 
                        {
                            newRating = CalculateVictory(currentRating, opRating); break;
                        }
                    case CombatResults.Loss:
                        {
                            newRating = CalculateDefeat(currentRating, opRating); break;
                        }
                    case CombatResults.Draw:
                        {
                            newRating = CalculateDraw(currentRating, opRating); break;
                        }
                    default: break;
                }

                return newRating;
            }

            catch (Exception e)
            {
                if (profile.Combatant.AccessLevel == AccessLevel.Administrator)
                    profile.Combatant.SendMessage("Error Updating Combat Rating!");
                eqUtility.HandleGenericException(e);
                return profile.CombatRating; 
            }
        }

        internal static int CalculateDraw(int currentRating, int opRating)
        {
            if (currentRating >= opRating)
            {
                int difference = currentRating - opRating;
                int additionalSum = ((int)(difference * 0.618) / 2);

                if (additionalSum < 1)
                    additionalSum = 1;
                else if (additionalSum > 15)
                    additionalSum = 15;

                return currentRating + additionalSum;
            }

            else
            {
                int difference = opRating - currentRating;
                int additionalSum = ((int)(difference * 0.618) / 2);

                if (additionalSum < 1)
                    additionalSum = 1;
                else if (additionalSum > 15)
                    additionalSum = 15;

                return currentRating - additionalSum;
            }
        }

        internal static int CalculateVictory(int currentRating, int opRating)
        {
            if (currentRating >= opRating)
            {
                int difference = currentRating - opRating;
                int additionalSum = ((int)(difference * 0.618));

                if (additionalSum < 1)
                    additionalSum = 1;
                else if (additionalSum > 15)
                    additionalSum = 15;

                return currentRating + additionalSum;
            }

            else
            {
                int difference = opRating - currentRating;
                int additionalSum = ((int)(difference * 0.618));

                if (additionalSum < 1)
                    additionalSum = 1;
                else if (additionalSum > 15)
                    additionalSum = 15;

                return currentRating + additionalSum;
            }
        }

        internal static int CalculateDefeat(int currentRating, int opRating)
        {
            if (currentRating >= opRating)
            {
                int difference = currentRating - opRating;
                int additionalSum = ((int)(difference * 0.618));

                if (additionalSum < 1)
                    additionalSum = 1;
                else if (additionalSum > 15)
                    additionalSum = 15;

                return currentRating - additionalSum;
            }

            else
            {
                int difference = opRating - currentRating;
                int additionalSum = ((int)(difference * 0.618));

                if (additionalSum < 1)
                    additionalSum = 1;
                else if (additionalSum > 15)
                    additionalSum = 15;

                return currentRating - additionalSum;
            }
        }
    }

    /// <summary>
    /// Object used to track EqPvP stats.
    /// </summary>
    public class CombatantProfile
    {
        /// <summary>
        /// Possible ranks obtained by players engaged in the EqPvP system.
        /// </summary>
        internal enum CombatRank
        {
            None, Pawn, Bishop, Knight, Rook, King, Queen, Master
        }

        public CombatantProfile()
        {
            ///
        }

        public CombatantProfile(Mobile m)
        {
            if (m is PlayerMobile)
                m_CombatantMobile = m as PlayerMobile;
        }

        PlayerMobile m_CombatantMobile;

        CombatRank m_Rank;

        int m_combatRating;
        int m_totalKills; 
        int m_totalDeaths;

        internal CombatRank Rank
        {
            get { return m_Rank; }
            set { m_Rank = value; }
        }

        internal PlayerMobile Combatant
        {
            get { return m_CombatantMobile; }
        }

        internal float KillDeathRatio
        {
            get { return m_totalKills / m_totalDeaths; }
        }

        internal int TotalKills
        {
            get { return m_totalKills; }
            set { m_totalKills = value; }
        }

        internal int TotalDeaths
        {
            get { return m_totalKills; }
            set { m_totalKills = value; }
        }

        internal int CombatRating
        {
            get { return m_combatRating; }
            set { m_combatRating = value; }
        }

        internal bool IsFemale() 
        {
            return Combatant.Female;
        }

        public void Serialize(BinaryFileWriter writer)
        {
            writer.Write((int)0); //Version

            writer.Write((PlayerMobile)m_CombatantMobile);
            writer.Write((int)m_combatRating);
            writer.Write((int)m_Rank);
            writer.Write((int)m_totalDeaths);
            writer.Write((int)m_totalKills);
        }

        public void Deserialize(BinaryFileReader reader)
        {
            int version = reader.ReadInt();

            if (version >= 0)
            {
                m_CombatantMobile = reader.ReadMobile() as PlayerMobile;
                m_combatRating = reader.ReadInt();

                m_Rank = (CombatRank)reader.ReadInt();

                m_totalDeaths = reader.ReadInt();
                m_totalKills = reader.ReadInt();
            }
        }
    }

    internal class ProfileModTarget : Target
    {
        internal enum ModType
        {
            Kills,
            Deaths,
            Rating
        }

        ModType m_ModType;
        int modifierValue;

        public ProfileModTarget(Mobile m, int modVal, ModType type)
            : base(12, true, TargetFlags.None)
        {
            m_ModType = type;
            modifierValue = modVal;
            m.SendMessage("Select the mobile which to affect.");
        }

        protected override void OnTarget(Mobile from, object o)
        {
            try
            {
                if (o is PlayerMobile)
                {
                    CombatantProfile profile = ((IPlayerCombatant)o).CombatProfile;
                    switch (m_ModType)
                    {
                        case ModType.Deaths:
                            {
                                profile.TotalDeaths = modifierValue;
                                break;
                            }
                        case ModType.Kills:
                            {
                                profile.TotalKills = modifierValue;
                                break;
                            }
                        case ModType.Rating:
                            {
                                profile.CombatRating = modifierValue;
                                break;
                            }
                        default: break;
                    }
                }
            }
            catch (Exception e) { eqUtility.HandleGenericException(e); }
        }
    }

    #region PostScript
    // Add to PlayerMobile global members:
    /*
     *  CombatantProfile m_combatProfile { get; set; }
        public CombatantProfile CombatProfile
        {
            get { return m_combatProfile; }
            set { m_combatProfile = value; }
        }
     * 
    // Add to PlayerMobile public override void GetProperties(ObjectPropertyList list)
    /*
            if (((IPlayerCombatant)this).m_PlayerProfile != null)
            {
                    string color = "";
                    int rating = ((IPlayerCombatant)this).m_PlayerProfile.CombatRating;
                    CombatantProfile profile = ((IPlayerCombatant)this).m_PlayerProfile;

                    switch (profile.Rank)
                    {
                        case EqPvP.CombatantProfile.CombatRank.Pawn: color = "#5A5ABA"; break;
                        case EqPvP.CombatantProfile.CombatRank.Bishop: color = "#5A5ABA"; break;
                        case EqPvP.CombatantProfile.CombatRank.Knight: color = "#C10000"; break;
                        case EqPvP.CombatantProfile.CombatRank.Rook: color = "#C10000"; break;
                        case EqPvP.CombatantProfile.CombatRank.King: color = "#006A00"; break;
                        case EqPvP.CombatantProfile.CombatRank.Queen: color = "#006A00"; break;
                        case EqPvP.CombatantProfile.CombatRank.Master: color = "#010198"; break;
                    }

                    list.Add(1060658, "{0}\t{1}", "Combat Rating", String.Format("<BASEFONT COLOR={0}>{1}", color, profile.CombatRating));
            }
     * */
    #endregion

}
