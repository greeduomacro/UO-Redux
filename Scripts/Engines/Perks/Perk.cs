using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Server.Mobiles;

namespace Server.Perks
{
    public enum PerkLevel : byte
    {
        None = 0,
        First = 1,
        Second = 2,
        Third = 3,
        Fourth = 4,
        Fifth = 5
    }

    public enum PerkPosition : byte
    {
        Primary = 1,
        Secondary = 2
    }

    public abstract class Perk
    {
        private static string DataPath = "Saves/Perks";
        private static string DataFile = Path.Combine(DataPath, "perks.bin");

        public static Dictionary<Player, Tuple<Perk, Perk>> PerkTable { get; private set; }

        /// <summary>
        /// EoC cost of the first Perk level
        /// </summary>
        public const int FirstLevelCost = 10000;

        /// <summary>
        /// List of all defined perk types
        /// </summary>
        public static List<Perk> AllPerks { get; private set; }
        /// <summary>
        /// Empty perk placeholder
        /// </summary>
        public static Perk Empty = new Empty();

        public static void Configure()
        {
            AllPerks = new List<Perk>();
            PerkTable = new Dictionary<Player, Tuple<Perk, Perk>>();

            EventSink.WorldLoad += OnWorldLoad;
            EventSink.WorldSave += OnWorldSave;

            //populates the AllPerks list by creating a dummy perk of each type
            for( int i = 0; i < ScriptCompiler.Assemblies.Length; i++ )
            {
                Type[] types = ScriptCompiler.Assemblies[i].GetTypes();

                for( int j = 0; j < types.Length; j++ )
                {
                    Type t = types[j];

                    if( !t.IsSubclassOf(typeof(Perk)) || t.IsAssignableFrom(typeof(Empty)) )
                        continue;

                    ConstructorInfo ctor = t.GetConstructor(new Type[] { typeof(Player) });

                    if( ctor != null )
                    {
                        AllPerks.Add((Perk)ctor.Invoke(new object[] { null }));
                    }
                }
            }

            AllPerks.Sort(PerkComparer.Instance);
        }

        /// <summary>
        /// Determines if the player has a free perk slot available
        /// </summary>
        public static bool HasFreeSlot( Player player )
        {
            if( !PerkTable.ContainsKey(player) )
                return true;

            return (PerkTable[player].Item1 is Empty || PerkTable[player].Item2 is Empty);
        }

        /// <summary>
        /// Determines if the player has the specified perk
        /// </summary>
        public static bool HasPerk( Player player, Perk perk )
        {
            if( !PerkTable.ContainsKey(player) || perk == null || perk is Empty )
                return false;

            return (PerkTable[player].Item1.GetType() == perk.GetType() || (PerkTable[player].Item2.GetType() == perk.GetType()));
        }

        /// <summary>
        /// Gets a perk of the same type as given for the specified player
        /// </summary>
        /// <param name="player">the player to lookup</param>
        /// <param name="perk">the perk type to match</param>
        /// <returns>instance of the perk or null if the player does not have that perk</returns>
        public static Perk Get( Player player, Perk perk )
        {
            Perk res = null;

            if( !PerkTable.ContainsKey(player) || !HasPerk(player, perk) )
                return res;

            if( PerkTable[player].Item1.GetType() == perk.GetType() )
            {
                res = PerkTable[player].Item1;
            }
            else if( PerkTable[player].Item2.GetType() == perk.GetType() )
            {
                res = PerkTable[player].Item2;
            }

            return res;
        }

        /// <summary>
        /// Gets a perk by type for a specific player
        /// </summary>
        /// <returns>instance of the <code>Perk</code> or <code>null</code> if the player does not have the type of Perk</returns>
        /// <typeparam name="T"><code>Perk</code> to retrieve from the player</typeparam>
        public static T GetByType<T>( Player player ) where T : Perk
        {
            T perk = default(T);

            if( !PerkTable.ContainsKey(player) )
                return perk;

            if( PerkTable[player].Item1 is T )
            {
                perk = (T)(object)PerkTable[player].Item1;
            }
            else if( PerkTable[player].Item2 is T )
            {
                perk = (T)(object)PerkTable[player].Item2;
            }

            return perk;
        }

        /// <summary>
        /// Returns the set of perks belonging to the specified player
        /// </summary>
        public static Tuple<Perk, Perk> GetPerks( Player player )
        {
            if( !PerkTable.ContainsKey(player) )
                return Tuple.Create<Perk, Perk>(Empty, Empty);

            return PerkTable[player];
        }

        /// <summary>
        /// Removes the perk at the given position
        /// </summary>
        public static void Remove( Player player, PerkPosition position )
        {
            if( !PerkTable.ContainsKey(player) )
                return;

            switch( position )
            {
                case PerkPosition.Primary:
                    PerkTable[player] = Tuple.Create<Perk, Perk>(Empty, PerkTable[player].Item2);
                    break;
                case PerkPosition.Secondary:
                    PerkTable[player] = Tuple.Create<Perk, Perk>(PerkTable[player].Item1, Empty);
                    break;
            }

            if( PerkTable[player].Item1 is Empty && PerkTable[player].Item2 is Empty )
                PerkTable.Remove(player);
        }

        /// <summary>
        /// Sets the player perk at the chosen position
        /// </summary>
        /// <returns>true if the perk could be assigned, false if the perk tree is full and overwriteExisting is false</returns>
        public static bool Set( Player player, Perk perk, PerkPosition position, bool overwriteExisting )
        {
            if( perk == null || !HasFreeSlot(player) || HasPerk(player, perk) )
                return false;

            if( !PerkTable.ContainsKey(player) )
                PerkTable[player] = Tuple.Create<Perk, Perk>(Perk.Empty, Perk.Empty);

            if( !overwriteExisting )
            {
                if( position == PerkPosition.Primary && !(PerkTable[player].Item1 is Empty) )
                {
                    position = PerkPosition.Secondary;
                }

                if( position == PerkPosition.Secondary && !(PerkTable[player].Item2 is Empty) )
                {
                    if( PerkTable[player].Item1 is Empty )
                    {
                        position = PerkPosition.Primary;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            switch( position )
            {
                case PerkPosition.Primary:
                    PerkTable[player] = Tuple.Create<Perk, Perk>(perk, PerkTable[player].Item2);
                    break;
                case PerkPosition.Secondary:
                    PerkTable[player] = Tuple.Create<Perk, Perk>(PerkTable[player].Item1, perk);
                    break;
            }

            return true;
        }

        /// <summary>
        /// Finds a perk constructor by type
        /// </summary>
        private static ConstructorInfo GetPerkCtor( string typeName )
        {
            Type type = ScriptCompiler.FindTypeByFullName(typeName);

            if( type == null )
            {
                throw new Exception(String.Format("Error: Perk type '{0}' was not found.", typeName));
            }

            ConstructorInfo ctor = type.GetConstructor(new Type[] { typeof(GenericReader) });

            if( ctor == null )
            {
                throw new Exception(String.Format("Error: Deserialization ctor not found on type '{0}'", typeName));
            }

            return ctor;
        }

        /// <summary>
        /// Loads serialized data
        /// </summary>
        private static void OnWorldLoad()
        {
            if( !File.Exists(DataFile) )
                return;

            using( FileStream stream = new FileStream(DataFile, FileMode.Open, FileAccess.Read, FileShare.Read) )
            {
                BinaryFileReader reader = new BinaryFileReader(new BinaryReader(stream));

                int version = reader.ReadInt();

                switch( version )
                {
                    case 0:
                        int tableCount = reader.ReadInt();
                        PerkTable = new Dictionary<Player, Tuple<Perk, Perk>>(tableCount);

                        Player player;
                        Perk p1, p2;

                        try
                        {

                            for (int i = 0; i < tableCount; i++)
                            {
                                player = reader.ReadMobile<Player>();

                                if (player == null || player.Deleted)
                                    continue;

                                ConstructorInfo ctor = GetPerkCtor(reader.ReadString());
                                p1 = ctor.Invoke(new object[] { reader }) as Perk;

                                ctor = GetPerkCtor(reader.ReadString());
                                p2 = ctor.Invoke(new object[] { reader }) as Perk;

                                PerkTable[player] = Tuple.Create<Perk, Perk>(p1, p2);
                            }
                        }

                        catch (Exception e)
                        {
                            Utilities.ExceptionManager.LogException("Perk", e);
                        }

                        break;
                }

                reader.Close();
            }
        }

        /// <summary>
        /// Saves persistent data
        /// </summary>
        private static void OnWorldSave( WorldSaveEventArgs args )
        {
            if( !Directory.Exists(DataPath) )
                Directory.CreateDirectory(DataPath);

            BinaryFileWriter writer = new BinaryFileWriter(DataFile, true);

            writer.Write((int)0); //version

            writer.Write((int)PerkTable.Count);

            foreach( KeyValuePair<Player, Tuple<Perk, Perk>> kvp in PerkTable )
            {
                if( kvp.Key == null || kvp.Key.Deleted )
                    continue;

                writer.WriteMobile<Player>(kvp.Key);

                writer.Write(kvp.Value.Item1.GetType().FullName);
                kvp.Value.Item1.Serialize(writer);

                writer.Write(kvp.Value.Item2.GetType().FullName);
                kvp.Value.Item2.Serialize(writer);
            }

            writer.Close();
        }

        /// <summary>
        /// Display description
        /// </summary>
        public abstract string Description { get; }
        /// <summary>
        /// ID of the image to use in the perk interface gump
        /// </summary>
        public abstract int GumpID { get; }
        /// <summary>
        /// Perk display label
        /// </summary>
        public abstract string Label { get; }
        /// <summary>
        /// Series of <code>LabelEntry</code> models containing the label and description for each perk level
        /// </summary>
        public abstract LabelEntryList LabelEntries { get; }

        /// <summary>
        /// Current perk level
        /// </summary>
        public PerkLevel Level { get; set; }
        /// <summary>
        /// Reference to the owning player
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        protected Perk( Player player )
        {
            Level = PerkLevel.First;
            Player = player;
        }

        /// <summary>
        /// Serializes the perk data
        /// </summary>
        protected virtual void Serialize( GenericWriter writer )
        {
            writer.Write((int)0);

            writer.WriteMobile<Player>(Player);
            writer.Write((int)Level);
        }

        /// <summary>
        /// Deserialization ctor
        /// </summary>
        protected Perk( GenericReader reader )
        {
            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    Player = reader.ReadMobile<Player>();
                    Level = (PerkLevel)reader.ReadInt();

                    break;
            }
        }
    }

    public class Empty : Perk
    {
        /// <summary>
        /// ctor
        /// </summary>
        public Empty( Player player = null )
            : base(player)
        {
            Level = PerkLevel.None;
        }

        /// <summary>
        /// Serialization
        /// </summary>
        protected override void Serialize( GenericWriter writer )
        {
            base.Serialize(writer);
        }

        /// <summary>
        /// Deserialization
        /// </summary>
        public Empty( GenericReader reader )
            : base(reader)
        {
        }

        public override string Description { get { return ""; } }
        public override int GumpID { get { return 0; } }
        public override string Label { get { return "-empty-"; } }

        public override LabelEntryList LabelEntries
        {
            get
            {
                return new LabelEntryList();
            }
        }
    }

    public class PerkComparer : IComparer<Perk>
    {
        public static readonly IComparer<Perk> Instance = new PerkComparer();

        public int Compare( Perk a, Perk b )
        {
            if( a == null && b == null )
                return 0;
            else if( a == null )
                return -1;
            else if( b == null )
                return 1;

            return Insensitive.Compare(a.Label, b.Label);
        }
    }
}