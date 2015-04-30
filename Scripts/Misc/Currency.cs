using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Server;
using Server.Items;

namespace Server.Currency
{
    public enum CurrencyType
    {
        Unknown = -1,
        Gold = 0,
        Verite = 1,
        Valorite = 2
    }

    public static class CurrencySystem
    {
        public static readonly Type typeofCopper = typeof(Gold);
        public static readonly Type typeofSilver = typeof(Verite);
        public static readonly Type typeofGold = typeof(Valorite);
        public static readonly Type[] AllCurrencies = new Type[] { typeofCopper, typeofSilver, typeofGold };

        private static readonly string savePath = "Saves/Currency";
        private static readonly string saveMintFile = Path.Combine(savePath, "mints.xml");

        private static List<CoinMint> mintList = new List<CoinMint>();

        public static void Configure()
        {
            EventSink.WorldLoad += new WorldLoadEventHandler(event_worldLoad);
            EventSink.WorldSave += new WorldSaveEventHandler(event_worldSave);
        }

        #region load/save
        private static void event_worldLoad()
        {
            if( !File.Exists(saveMintFile) )
                return;

            XmlDocument doc = new XmlDocument();
            XmlElement root;

            try
            {
                doc.Load(saveMintFile);
                root = doc["mints"];

                foreach( XmlElement ele in root.GetElementsByTagName("mint") )
                {
                    mintList.Add(new CoinMint(ele));
                }
            }
            catch( Exception e )
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine("\nCurrency load exception: " + e);
                Utility.PopColor();
            }
        }

        private static void event_worldSave( WorldSaveEventArgs args )
        {
            if( !Directory.Exists(savePath) )
                Directory.CreateDirectory(savePath);

            using( StreamWriter writer = new StreamWriter(saveMintFile) )
            {
                XmlTextWriter xml = new XmlTextWriter(writer);

                xml.Formatting = Formatting.Indented;
                xml.IndentChar = '\t';
                xml.Indentation = 1;

                xml.WriteStartDocument(true);
                xml.WriteStartElement("mints");
                xml.WriteAttributeString("count", mintList.Count.ToString());

                mintList.ForEach(delegate( CoinMint cm ) { cm.Save(xml); });

                xml.WriteEndElement();
                xml.WriteEndDocument();
                xml.Close();
            }
        }
        #endregion

        #region currency conversion

        private static double[,] conversionTable = new double[,]
            {
                /* Copper       Silver      Gold */
/* Copper */    { 1.000,       10.00,      1000.00 },
/* Silver */    { 0.100,       1.000,      100.000 },
/* Gold */      { 0.001,       0.010,      1.00000 }
            };

        /// <summary>
        /// Compresses the coin values into their most compact form
        /// </summary>
        /// <param name="copper">total amount of copper</param>
        /// <param name="silver">total amount of silver</param>
        /// <param name="gold">total amount of gold</param>
        /// <returns>int[3] containing output copper, silver, and gold values (respective indexes: 0, 1, 2)</returns>
        /// <example>15640 copper = 1 gold, 56 silver, 40 copper</example>
        /// 
        //Old and Busted!
        //public static int[] Compress( int copper, int silver, int gold )
        //{
        //    int[] res = new int[3] { 0, 0, 0 };
        //    double cg, cs, cc;
        //    long totalCopper = copper;

        //    if( silver > 0 )
        //        totalCopper += (int)(silver / conversionTable[(int)CurrencyType.Silver, (int)CurrencyType.Copper]);

        //    if( gold > 0 )
        //        totalCopper += (int)(gold / conversionTable[(int)CurrencyType.Gold, (int)CurrencyType.Copper]);

        //    cg = (totalCopper / conversionTable[0, 2]);
        //    cs = ((cg - Math.Floor(cg)) * conversionTable[0, 1]);
        //    cc = ((cs - Math.Floor(cs)) / 0.01);

        //    return new int[3] { (int)Math.Round(cc), (int)cs, (int)cg };
        //}

        public static int[] Compress(int gold, int verite, int valorite)
        {
            int[] res = new int[3] { 0, 0, 0 };
            double cg, cs, cc;
            long totalGold = gold;

            if (verite > 0)
                totalGold += (int)(verite / conversionTable[(int)CurrencyType.Verite, (int)CurrencyType.Gold]);

            if (gold > 0)
                totalGold += (int)(valorite / conversionTable[(int)CurrencyType.Valorite, (int)CurrencyType.Gold]);

            cg = (totalGold / conversionTable[(int)CurrencyType.Gold, (int)CurrencyType.Valorite]); //get total gold by converting all available copper to gold (same as ConvertTo() but using doubles)
            cs = ((cg - Math.Floor(cg)) * conversionTable[(int)CurrencyType.Verite, (int)CurrencyType.Valorite]); //convert remainder of gold conversion to as much silver as possible (silver <- gold in table)
            cc = ((cs - Math.Floor(cs)) * conversionTable[(int)CurrencyType.Gold, (int)CurrencyType.Verite]); //convert remainder of silver conversion to copper (copper <- silver in table)

            return new int[3] { (int)Math.Round(cc), (int)cs, (int)cg };
        }

        /// <summary>
        /// Converts one currency into another
        /// </summary>
        /// <param name="existingCurrency"><code>CurrencyType</code> to convert from</param>
        /// <param name="targetCurrency"><code>CurrencyType</code> to convert into</param>
        /// <param name="amount">total amount of <code>existingCurrency</code> to convert</param>
        /// <returns>the converted amount as a whole number</returns>
        public static int ConvertTo( CurrencyType existingCurrency, CurrencyType targetCurrency, int amount )
        {
            if( existingCurrency == CurrencyType.Unknown || targetCurrency == CurrencyType.Unknown || amount == 0 )
                return 0;

            int ec = (int)existingCurrency;
            int tc = (int)targetCurrency;

            return (int)(amount / conversionTable[ec, tc]);
        }

        /// <summary>
        /// Determines a coin type's designated <code>CurrencyType</code> value
        /// </summary>
        /// <param name="itemType">the System.Type of the item to check</param>
        /// <returns>the known CurrencyType for the given item Type</returns>
        public static CurrencyType FindType( Type itemType )
        {
            if( itemType == typeofCopper )
                return CurrencyType.Gold;
            if( itemType == typeofSilver )
                return CurrencyType.Verite;
            if( itemType == typeofGold )
                return CurrencyType.Valorite;

            return CurrencyType.Unknown;
        }

        #endregion

        #region coin consumption

        /// <summary>
        /// Tries to consume the given amount out of the total amount of currency in the provided container.
        /// Automatically redistributes coins.
        /// </summary>
        /// <param name="cont">the container to consume from</param>
        /// <param name="currencyType">the target currency to consume from</param>
        /// <param name="amount">the amount of the target currency to consume</param>
        /// <returns>true if the container has the exact amount of the currencyType, or if the container has enough total monies</returns>
        public static bool Consume( Container cont, Type currencyType, int amount )
        {
            if( cont == null )
                return false;

            if( cont.ConsumeTotal(currencyType, amount) )
                return true;

            int availableCopper = 0;
            Item[] coins = cont.FindItemsByType(AllCurrencies);

            //calculate monies as copper total
            for( int i = 0; i < coins.Length; i++ )
                availableCopper += ConvertTo(FindType(coins[i].GetType()), CurrencyType.Gold, coins[i].Amount);

            //if we have enough to pay...
            if( availableCopper >= amount )
            {
                //delete all the coins
                for( int i = 0; i < coins.Length; i++ )
                    coins[i].Consume(coins[i].Amount);

                availableCopper -= amount;

                //if we should still have money, redistribute
                if( availableCopper > 0 )
                    DistributeCoins(cont, Compress(availableCopper, 0, 0));

                return true;
            }

            return false;
        }

        #endregion

        #region coin distribution
        public static void DistributeCoins( Mobile m, int[] coinArray )
        {
            DistributeCoins(m.Backpack, coinArray);
        }

        public static void DistributeCoins( Container targetContainer, int[] coinArray )
        {
            if( targetContainer != null )
            {
                if( coinArray[2] > 0 ) targetContainer.DropItem(new Valorite(coinArray[2]));
                if( coinArray[1] > 0 ) targetContainer.DropItem(new Verite(coinArray[1]));
                if( coinArray[0] > 0 ) targetContainer.DropItem(new Gold(coinArray[0]));
            }
        }
        #endregion
    }

    #region +class CoinMint
    public class CoinMint
    {
        public enum Minters
        {
            Unknown,
            Yirla,
            Kyrline,
            Lienep,
            FracturedKingdom
        }

        private string _date;
        private string _description;
        private DateTime _mintDate;
        private Minters _minter;

        /// <summary>
        /// Describes the in-game date of the coin
        /// </summary>
        public string Date { get { return _date; } }

        /// <summary>
        /// A textual description of the images on the coin
        /// </summary>
        public string Description { get { return _description; } }

        /// <summary>
        /// The real-world date & time that this mint was engineered
        /// </summary>
        public DateTime MintDate { get { return _mintDate; } }

        /// <summary>
        /// An identifier for the original creator of this mint
        /// </summary>
        public Minters Minter { get { return _minter; } }

        public CoinMint( Minters minter, string gameDate, string description )
        {
            _date = gameDate;
            _description = description;
            _mintDate = DateTime.Now;
            _minter = minter;
        }

        public CoinMint( XmlElement node )
        {
            _date = Utility.GetText(node["gameDate"], "unknown");
            _description = Utility.GetText(node["description"], "");
            _mintDate = Utility.GetXMLDateTime(Utility.GetText(node["mintDate"], null), DateTime.MinValue);

            try
            {
                _minter = (Minters)Enum.Parse(typeof(Minters), Utility.GetText(node["minter"], Minters.Unknown.ToString()));
            }
            catch
            {
                _minter = Minters.Unknown;
            }
        }

        public void Save( XmlTextWriter writer )
        {
            writer.WriteStartElement("mint");

            writer.WriteStartElement("gameDate");
            writer.WriteString(Date);
            writer.WriteEndElement();

            writer.WriteStartElement("description");
            writer.WriteString(Description);
            writer.WriteEndElement();

            writer.WriteStartElement("mintDate");
            writer.WriteString(XmlConvert.ToString(MintDate, XmlDateTimeSerializationMode.Local));
            writer.WriteEndElement();

            writer.WriteStartElement("minter");
            writer.WriteString(Minter.ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
    #endregion
}