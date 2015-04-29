using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Server.Currency;
using Server.Utilities;
using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Curr = Server.Currency.CurrencySystem;

namespace Server.Market
{
    public static class Market
    {
        /// <summary>
        /// Path to the SQLite3 database file
        /// </summary>
        internal static string DBFile = "Data\\market.db3";
        /// <summary>
        /// Indicates if the system is enabled (db passed all tests, SQLite connections successful)
        /// </summary>
        internal static bool Enabled = false;

        private static SQLiteConnection SQLConn;

        public static void Initialize()
        {
            if( Enabled ) 
            {
                SQLConn = new SQLiteConnection(String.Format("Data Source={0}", DBFile));
                SetupDatabase();

                CommandSystem.Register("MyMarket", AccessLevel.Player, new CommandEventHandler(MyMarket_OnCommand));
                CommandSystem.Register("ViewMarket", AccessLevel.Player, new CommandEventHandler(ViewMarket_OnCommand));
            }
        }

        #region command handlers
        [Usage("MyMarket")]
        [Description("Opens your market management panel.")]
        private static void MyMarket_OnCommand( CommandEventArgs args )
        {
            try
            {
                //args.Mobile.SendGump(new PersonalMarketGump(args.Mobile));
            }

            catch { }
        }

        [Usage("ViewMarket")]
        [Description("Opens the player market window.")]
        private static void ViewMarket_OnCommand( CommandEventArgs args )
        {
            try
            {
                //args.Mobile.SendGump(new MarketGump());
            }

            catch { }
        }
        #endregion

        #region +static void AddNewOrder( Mobile, MarketEntry )
        public static void AddNewOrder( Mobile seller, MarketEntry entry )
        {
            seller.SendMessage("Select the object for this order.");
            seller.BeginTarget(12, false, Server.Targeting.TargetFlags.None, new TargetCallback(
                delegate( Mobile from, object targeted )
                {
                    if( targeted is IEntity )
                    {
                        if( targeted is Mobile )
                        {
                            Mobile mob = (Mobile)targeted;

                            if( mob is BaseCreature && ((BaseCreature)mob).ControlMaster == from )
                                FinalizeNewOrder(seller, entry, mob);
                            else
                                from.SendMessage("You do not have the right to sell that.");
                        }
                        else if( targeted is Item )
                        {
                            Item item = (Item)targeted;

                            if( item.IsChildOf(from.Backpack) || item.RootParent == from )
                                FinalizeNewOrder(seller, entry, item);
                            else
                                from.SendMessage("You do not have the right to sell that.");
                        }
                    }
                    else
                    {
                        from.SendMessage("That is not a valid market entity.");
                    }
                }));
        }
        #endregion

        #region +static bool BuyItem( Mobile, MarketEntry )
        public static bool BuyItem( Mobile buyer, MarketEntry entry )
        {
            if( buyer == null || !buyer.Alive )
            {
                buyer.SendMessage("You cannot purchase a market order while knocked out.");
            }
            else if( buyer == entry.Seller )
            {
                buyer.SendMessage("You cannot purchase something from yourself.");
            }
            else
            {
                IEntity entity = World.FindEntity(entry.ObjectSerial);

                if( entity == null || IsSold(entry) )
                {
                    buyer.SendMessage("The order has expired.");
                }
                else
                {
                    Type[] coinTypes = new Type[] { Curr.typeofCopper, Curr.typeofSilver, Curr.typeofGold };
                    int[] compressedCost = Curr.Compress(entry.Cost, 0, 0);
                    Container cont = buyer.FindBankNoCreate();

                    if( cont != null && cont.ConsumeTotal(coinTypes, compressedCost) == -1 )
                    {
                        if( entity is Item )
                        {
                            Item item = (Item)entity;
                            cont = buyer.Backpack;

                            if( cont != null && !cont.TryDropItem(buyer, item, false) )
                                item.MoveToWorld(buyer.Location, buyer.Map);

                            CloseOrder(entry);

                            if( buyer.HasGump(typeof(MarketGump)) )
                            {
                                buyer.CloseGump(typeof(MarketGump));
                                buyer.SendGump(new MarketGump(entry.Category, 0));
                            }

                            return true;
                        }
                        else if( entity is Mobile )
                        {
                            Mobile mob = (Mobile)entity;
                            mob.Direction = (Direction)Utility.Random(8);
                            mob.MoveToWorld(buyer.Location, buyer.Map);
                            mob.PlaySound(mob.GetIdleSound());

                            if( mob is BaseCreature )
                                ((BaseCreature)mob).SetControlMaster(buyer);

                            CloseOrder(entry);

                            if( buyer.HasGump(typeof(MarketGump)) )
                            {
                                buyer.CloseGump(typeof(MarketGump));
                                buyer.SendGump(new MarketGump(entry.Category, 0));
                            }

                            return true;
                        }
                    }
                    else
                    {
                        buyer.SendMessage("You cannot afford that item.");
                    }
                }
            }

            return false;
        }
        #endregion

        #region +static void CloseOrder( ... )
        /// <summary>
        /// Closes an order after being sold
        /// </summary>
        public static void CloseOrder( MarketEntry entry )
        {
            CloseOrder(entry, true);
        }

        /// <summary>
        /// Closes an order. If wasSold=false, the order is deleted from the entries table history.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="wasSold"></param>
        public static void CloseOrder( MarketEntry entry, bool wasSold )
        {
            ExecuteNonQuery("UPDATE entries SET active = 0 WHERE entryid = " + entry.TableId);

            if( wasSold )
            {
                ExecuteNonQuery("UPDATE sellers SET earnings = earnings + " + entry.Cost + " WHERE serial = " + (int)entry.Seller.Serial);
                Container cont = entry.Seller.FindBankNoCreate();

                if( cont != null )
                {
                    int[] coins = Curr.Compress(entry.Cost, 0, 0);
                    cont.DropItem(new Valorite(coins[2]));
                    cont.DropItem(new Verite(coins[1]));
                    cont.DropItem(new Gold(coins[0]));
                }
                else
                {
                    ExceptionManager.LogException("Market", new Exception(String.Format("Unable to find bank for seller {0}.", LogManager.Format(entry.Seller))));
                }
            }
            else
            {
                ExecuteNonQuery("DELETE FROM entries WHERE entryid = " + entry.TableId);
                ExecuteNonQuery("UPDATE sellers SET totalorders = totalorders-1 WHERE serial = " + (int)entry.Seller.Serial);

                IEntity entity = World.FindEntity(entry.ObjectSerial);

                if( entity != null )
                {
                    if( entity.Serial.IsItem )
                    {
                        Container cont = entry.Seller.FindBankNoCreate();

                        if( cont == null )
                            cont = entry.Seller.Backpack;

                        if( cont == null )
                        {
                            ((Item)entity).MoveToWorld(entry.Seller.Location, entry.Seller.Map);
                        }
                        else
                        {
                            cont.DropItem((Item)entity);
                            entry.Seller.SendMessage("Your item has been returned to your {0}.", (cont is BankBox ? "bank box" : "backpack"));
                        }
                    }
                    else if( entity.Serial.IsMobile )
                    {
                        ((Mobile)entity).MoveToWorld(entry.Seller.Location, entry.Seller.Map);
                        ((Mobile)entity).PlaySound(((Mobile)entity).GetIdleSound());
                    }
                }
            }
        }
        #endregion

        #region +static List<MarketEntry> EntriesByCategory( Category )
        /// <summary>
        /// Retrieve all market orders matching <code>category</code>
        /// </summary>
        public static List<MarketEntry> EntriesByCategory( Category category )
        {
            List<MarketEntry> list = new List<MarketEntry>();
            SQLiteDataReader reader = ExecuteQuery("SELECT entryid,cost,description,itemserial,sellerserial FROM entries WHERE active = 1 AND category = " + (int)category);

            while( reader.Read() )
            {
                Mobile seller = World.FindMobile(reader.GetInt32(reader.GetOrdinal("sellerserial")));
                IEntity saleObject = World.FindEntity(reader.GetInt32(reader.GetOrdinal("itemserial")));

                if( seller == null || saleObject == null )
                    continue;

                MarketEntry entry = new MarketEntry(seller);
                entry.Category = category;
                entry.Description = reader.GetString(reader.GetOrdinal("description"));
                entry.TableId = reader.GetInt32(reader.GetOrdinal("entryid"));

                entry.ChangeCost(reader.GetInt32(reader.GetOrdinal("cost")), 0, 0);
                entry.SetSellItem(saleObject);

                list.Add(entry);
            }

            reader.Close();

            return list;
        }
        #endregion

        #region +static List<MarketEntry> EntriesBySeller( Mobile )
        /// <summary>
        /// Retrieve all market orders created by <code>seller</code>
        /// </summary>
        public static List<MarketEntry> EntriesBySeller( Mobile seller )
        {
            List<MarketEntry> list = new List<MarketEntry>();
            SQLiteDataReader reader = ExecuteQuery("SELECT entryid,active,category,cost,description,itemserial FROM entries WHERE sellerserial = " + (int)seller.Serial);

            while( reader.Read() )
            {
                IEntity saleObject = World.FindEntity(reader.GetInt32(reader.GetOrdinal("itemserial")));
                MarketEntry entry = new MarketEntry(seller);

                entry.Active = reader.GetBoolean(reader.GetOrdinal("active"));
                entry.Category = (Category)reader.GetInt32(reader.GetOrdinal("category"));
                entry.Description = reader.GetString(reader.GetOrdinal("description"));
                entry.TableId = reader.GetInt32(reader.GetOrdinal("entryid"));

                if( saleObject != null )
                    entry.SetSellItem(saleObject);

                entry.ChangeCost(reader.GetInt32(reader.GetOrdinal("cost")), 0, 0);

                list.Add(entry);
            }

            reader.Close();

            return list;
        }
        #endregion

        #region +static bool IsSold( MarketEntry )
        public static bool IsSold( MarketEntry entry )
        {
            SQLiteDataReader reader = ExecuteQuery("SELECT active FROM entries WHERE entryid=" + entry.TableId);
            bool active = false;

            if( reader.Read() )
                active = reader.GetBoolean(reader.GetOrdinal("active"));

            return !active;
        }
        #endregion

        #region +static string PrintOrderData( Mobile )
        /// <summary>
        /// Returns a string representation of the active and total orders created by <code>seller</code>
        /// </summary>
        public static string PrintOrderData( Mobile seller )
        {
            int active = 0, total = 0;

            SQLiteDataReader reader = ExecuteQuery("SELECT totalorders FROM sellers WHERE serial = $seller", new SQLiteParameter[] { new SQLiteParameter("$seller", (int)seller.Serial) });

            if( reader != null )
            {
                if( reader.Read() )
                    total = reader.GetInt32(reader.GetOrdinal("totalorders"));

                reader.Close();
            }

            reader = ExecuteQuery("SELECT entryid FROM entries WHERE active = 1 AND sellerserial = $seller", new SQLiteParameter[] { new SQLiteParameter("$seller", (int)seller.Serial) });

            if( reader != null )
            {
                while( reader.Read() )
                    active++;

                reader.Close();
            }

            return String.Format("{0} / {1}", active, total);
        }
        #endregion

        #region +static string PrintOrderEarnings( Mobile )
        /// <summary>
        /// Returns a string represenation of the lifetime market earnings from orders created by <code>seller</code>
        /// </summary>
        public static string PrintOrderEarnings( Mobile seller )
        {
            int copper = 0, silver = 0, gold = 0;

            SQLiteDataReader reader = ExecuteQuery("SELECT earnings FROM sellers WHERE serial = $seller", new SQLiteParameter[] { new SQLiteParameter("$seller", (int)seller.Serial) });

            if( reader != null )
            {
                if( reader.Read() )
                    copper = reader.GetInt32(reader.GetOrdinal("earnings"));

                reader.Close();
            }

            if( copper >= 100 )
            {
                int[] compressed = Curr.Compress(copper, 0, 0);
                copper = compressed[0];
                silver = compressed[1];
                gold = compressed[2];
            }

            return String.Format("{0}c, {1}s, {2}g", copper, silver, gold);
        }
        #endregion

        #region +static void UpdateOrder( Mobile, MarketEntry )
        /// <summary>
        /// Updates the properties of a market order. If the entry's TableId property is 0, the order is added as new instead of updating.
        /// </summary>
        /// <param name="entry">MarketEntry with the new order properties</param>
        public static void UpdateOrder( Mobile seller, MarketEntry entry )
        {
            if( entry.TableId <= 0 )
            {
                AddNewOrder(seller, entry);
                return;
            }

            if( String.IsNullOrEmpty(entry.Description) )
            {
                IEntity entity = World.FindEntity(entry.ObjectSerial);
                entry.Description = (entity is Item ? ((Item)entity).Name : (entity is Mobile ? ((Mobile)entity).Name : "an object"));
            }

            SQLiteParameter[] paramSet = new SQLiteParameter[]
				{
					new SQLiteParameter( "$cat", (int)entry.Category ),
					new SQLiteParameter( "$cost", entry.Cost ),
					new SQLiteParameter( "$desc", entry.Description.Substring( 0, Math.Min( entry.Description.Length, 255 ) ) )
				};
            ExecuteNonQuery("UPDATE entries SET active=1,category=$cat,cost=$cost,description=$desc WHERE entryid = " + entry.TableId, paramSet);

            seller.SendGump(new PersonalMarketGump(seller));
            seller.SendMessage("Your order has been updated.");
        }
        #endregion

        #region -static void FinalizeNewOrder( Mobile, MarketEntry, IEntity )
        private static void FinalizeNewOrder( Mobile seller, MarketEntry entry, IEntity saleEntity )
        {
            entry.SetSellItem(saleEntity);

            //do this to prevent a scenario where the market db is more up to date than the latest server save (market serials could be mismatched to server items, in that case)
            World.Save(false, true);

            if( saleEntity is Item )
                ((Item)saleEntity).MoveToWorld(new Point3D(0, 0, 1), Map.Internal);
            else if( saleEntity is Mobile )
                ((Mobile)saleEntity).Internalize();

            if( String.IsNullOrEmpty(entry.Description) )
            {
                if( saleEntity is Item )
                {
                    Item i = saleEntity as Item;

                    if( String.IsNullOrEmpty(i.Name) )
                    {
                        if( (i.ItemData.Flags & TileFlag.ArticleA) != 0 )
                            entry.Description = "a " + i.ItemData.Name;
                        else if( (i.ItemData.Flags & TileFlag.ArticleAn) != 0 )
                            entry.Description = "an " + i.ItemData.Name;
                        else
                            entry.Description = i.ItemData.Name;
                    }
                    else
                    {
                        entry.Description = i.Name;
                    }
                }
                else
                {
                    entry.Description = "a pet for sale by " + seller.Name;
                }
            }

            //add into the entries table
            SQLiteParameter[] paramSet = new SQLiteParameter[]
				{
					new SQLiteParameter( "$category", (int)entry.Category ),
					new SQLiteParameter( "$cost", entry.Cost ),
					new	SQLiteParameter( "$desc", entry.Description.Substring( 0, Math.Min( entry.Description.Length, 255 ) ) ),
					new SQLiteParameter( "$item", (int)saleEntity.Serial ),
					new SQLiteParameter( "$seller", (int)seller.Serial )
				};
            ExecuteNonQuery("INSERT INTO entries (category,cost,description,itemserial,sellerserial) " +
                "VALUES ($category,$cost,$desc,$item,$seller)", paramSet);

            //add/update the seller detail table
            try
            {
                ExecuteNonQuery("INSERT INTO sellers (serial,totalorders) VALUES ($seller,1)", new SQLiteParameter[] { new SQLiteParameter("$seller", (int)seller.Serial) });
            }
            catch
            {
                ExecuteNonQuery("UPDATE sellers SET totalorders=totalorders+1 WHERE serial=$seller", new SQLiteParameter[] { new SQLiteParameter("$seller", (int)seller.Serial) });
            }

            seller.SendGump(new PersonalMarketGump(seller));
            seller.SendMessage("Your order has been added to the marketplace.");
        }
        #endregion

        internal static void SetupDatabase()
        {
            Console.Write("Market: Confirming database parameters...");

            try
            {
                if( !File.Exists(DBFile) )
                    SQLiteConnection.CreateFile(DBFile);

                SQLConn.Open();
                SQLiteCommand sqlCmd = SQLConn.CreateCommand();

                //BEGIN table generation
                sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS [entries] ([entryid] integer PRIMARY KEY AUTOINCREMENT NOT NULL DEFAULT 0,[active] bit DEFAULT 1,[category] smallint NOT NULL,[cost] integer NOT NULL,[description] varchar(255) NOT NULL,[itemserial] integer NOT NULL,[sellerserial] integer NOT NULL);";
                sqlCmd.ExecuteNonQuery();

                sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS [sellers] ([serial] integer PRIMARY KEY NOT NULL,[totalorders] integer NOT NULL,[earnings] bigint DEFAULT 0);";
                sqlCmd.ExecuteNonQuery();
                //END table generation

                //BEGIN column test
                sqlCmd.CommandText = "SELECT entryid,active,category,cost,description,itemserial,sellerserial FROM entries LIMIT 1";
                sqlCmd.ExecuteNonQuery();

                sqlCmd.CommandText = "SELECT serial,totalorders,earnings FROM sellers LIMIT 1";
                sqlCmd.ExecuteNonQuery();
                //END column test

                SQLConn.Close();

                Enabled = true;
                Console.WriteLine("done (using '{0}')", DBFile);
            }
            catch( Exception e )
            {
                Console.WriteLine("failed!");

                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine(e.ToString());
                Console.WriteLine("\n\nPlayer market disabled. Please repair your database file.");
                Utility.PopColor();
            }
        }

        #region SQLite helpers
        private static int ExecuteNonQuery( string commandText )
        {
            return ExecuteNonQuery(commandText, null);
        }

        private static int ExecuteNonQuery( string commandText, SQLiteParameter[] parameters )
        {
            if( !Enabled )
                return 0;

            int data = 0;

            try
            {
                SQLiteCommand sqlCmd = SQLConn.CreateCommand();
                sqlCmd.CommandText = commandText;

                if( parameters != null && parameters.Length > 0 )
                    sqlCmd.Parameters.AddRange(parameters);

                SQLConn.Open();
                data = sqlCmd.ExecuteNonQuery();
                SQLConn.Close();
            }
            catch( Exception e )
            {
                SQLConn.Close();
                throw e;
            }

            return data;
        }

        private static SQLiteDataReader ExecuteQuery( string commandText )
        {
            return ExecuteQuery(commandText, null);
        }

        private static SQLiteDataReader ExecuteQuery( string commandText, SQLiteParameter[] parameters )
        {
            if( !Enabled )
                return null;

            SQLiteDataReader reader = null;

            try
            {
                SQLiteCommand sqlCmd = SQLConn.CreateCommand();
                sqlCmd.CommandText = commandText;

                if( parameters != null && parameters.Length > 0 )
                    sqlCmd.Parameters.AddRange(parameters);

                SQLConn.Open();
                reader = sqlCmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            }
            catch( Exception e )
            {
                SQLConn.Close();
                throw e;
            }

            return reader;
        }
        #endregion
    }
}