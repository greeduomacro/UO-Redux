using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Currency;
using Server.Items;

namespace Server.Mobiles
{
    public class Banker : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        public override NpcGuild NpcGuild { get { return NpcGuild.MerchantsGuild; } }

        [Constructable]
        public Banker()
            : base("the banker")
        {
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBBanker());
        }

        public static int GetBalance( Mobile from )
        {
            Item[] gold, checks;

            return GetBalance(from, out gold, out checks);
        }

        public static int GetBalance( Mobile from, out Item[] gold, out Item[] checks )
        {
            int balance = 0;

            Container bank = from.FindBankNoCreate();

            if( bank != null )
            {
                gold = bank.FindItemsByType(typeof(Valorite));
                checks = bank.FindItemsByType(typeof(BankCheck));

                for( int i = 0; i < gold.Length; ++i )
                    balance += gold[i].Amount;

                for( int i = 0; i < checks.Length; ++i )
                    balance += ((BankCheck)checks[i]).Worth;
            }
            else
            {
                gold = checks = new Item[0];
            }

            return balance;
        }

        public static bool Withdraw( Mobile from, int amount )
        {
            Item[] gold, checks;
            int balance = GetBalance(from, out gold, out checks);

            if( balance < amount )
                return false;

            for( int i = 0; amount > 0 && i < gold.Length; ++i )
            {
                if( gold[i].Amount <= amount )
                {
                    amount -= gold[i].Amount;
                    gold[i].Delete();
                }
                else
                {
                    gold[i].Amount -= amount;
                    amount = 0;
                }
            }

            for( int i = 0; amount > 0 && i < checks.Length; ++i )
            {
                BankCheck check = (BankCheck)checks[i];

                if( check.Worth <= amount )
                {
                    amount -= check.Worth;
                    check.Delete();
                }
                else
                {
                    check.Worth -= amount;
                    amount = 0;
                }
            }

            return true;
        }

        public static bool Deposit( Mobile from, int amount )
        {
            BankBox box = from.FindBankNoCreate();
            if( box == null )
                return false;

            List<Item> items = new List<Item>();

            while( amount > 0 )
            {
                Item item;
                if( amount < 5000 )
                {
                    item = new Valorite(amount);
                    amount = 0;
                }
                else if( amount <= 1000000 )
                {
                    item = new BankCheck(amount);
                    amount = 0;
                }
                else
                {
                    item = new BankCheck(1000000);
                    amount -= 1000000;
                }

                if( box.TryDropItem(from, item, false) )
                {
                    items.Add(item);
                }
                else
                {
                    item.Delete();
                    foreach( Item curItem in items )
                    {
                        curItem.Delete();
                    }

                    return false;
                }
            }

            return true;
        }

        public static int DepositUpTo( Mobile from, int amount )
        {
            BankBox box = from.FindBankNoCreate();
            if( box == null )
                return 0;

            int amountLeft = amount;
            while( amountLeft > 0 )
            {
                Item item;
                int amountGiven;

                if( amountLeft < 5000 )
                {
                    item = new Valorite(amountLeft);
                    amountGiven = amountLeft;
                }
                else if( amountLeft <= 1000000 )
                {
                    item = new BankCheck(amountLeft);
                    amountGiven = amountLeft;
                }
                else
                {
                    item = new BankCheck(1000000);
                    amountGiven = 1000000;
                }

                if( box.TryDropItem(from, item, false) )
                {
                    amountLeft -= amountGiven;
                }
                else
                {
                    item.Delete();
                    break;
                }
            }

            return amount - amountLeft;
        }

        public static void Deposit( Container cont, int amount )
        {
            while( amount > 0 )
            {
                Item item;

                if( amount < 5000 )
                {
                    item = new Valorite(amount);
                    amount = 0;
                }
                else if( amount <= 1000000 )
                {
                    item = new BankCheck(amount);
                    amount = 0;
                }
                else
                {
                    item = new BankCheck(1000000);
                    amount -= 1000000;
                }

                cont.DropItem(item);
            }
        }

        public Banker( Serial serial )
            : base(serial)
        {
        }

        public override bool HandlesOnSpeech( Mobile from )
        {
            if( from.InRange(this.Location, 12) )
                return true;

            return base.HandlesOnSpeech(from);
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            if( !e.Handled && e.Mobile.InRange(this.Location, 12) )
            {
                for( int i = 0; i < e.Keywords.Length; ++i )
                {
                    int keyword = e.Keywords[i];

                    switch( keyword )
                    {
                        case 0x0000: // *withdraw*
                            {
                                //e.Handled = true;

                                //if( e.Mobile.Criminal )
                                //{
                                //    this.Say( 500389 ); // I will not do business with a criminal!
                                //    break;
                                //}

                                //string[] split = e.Speech.Split( ' ' );

                                //if( split.Length >= 2 )
                                //{
                                //    int amount;

                                //    try
                                //    {
                                //        amount = Convert.ToInt32( split[1] );
                                //    }
                                //    catch
                                //    {
                                //        break;
                                //    }

                                //    if( amount > 5000 )
                                //    {
                                //        this.Say( 500381 ); // Thou canst not withdraw so much at one time!
                                //    }
                                //    else if( amount > 0 )
                                //    {
                                //        BankBox box = e.Mobile.FindBankNoCreate();

                                //        if( box == null || !box.ConsumeTotal( typeof( Gold ), amount ) )
                                //        {
                                //            this.Say( 500384 ); // Ah, art thou trying to fool me? Thou hast not so much gold!
                                //        }
                                //        else
                                //        {
                                //            e.Mobile.AddToBackpack( new Gold( amount ) );

                                //            this.Say( 1010005 ); // Thou hast withdrawn gold from thy account.
                                //        }
                                //    }
                                //}

                                break;
                            }
                        case 0x0001: // *balance*
                            {
                                e.Handled = true;

                                if( e.Mobile.Criminal )
                                {
                                    this.Say(500389); // I will not do business with a criminal!
                                    break;
                                }

                                BankBox box = e.Mobile.FindBankNoCreate();

                                //if( box != null )
                                //    this.Say( 1042759, box.TotalGold.ToString() ); // Thy current bank balance is ~1_AMOUNT~ gold.
                                //else
                                //    this.Say( 1042759, "0" ); // Thy current bank balance is ~1_AMOUNT~ gold.

                                if( box != null )
                                {
                                    Item[] coins = box.FindItemsByType(new Type[] { CurrencySystem.typeofGold, CurrencySystem.typeofVerite, CurrencySystem.typeofValorite });
                                    int valorite = 0, verite = 0, gold = 0;

                                    for( int c = 0; c < coins.Length; c++ )
                                    {
                                        if( coins[c].GetType() == CurrencySystem.typeofGold ) gold += coins[c].Amount;
                                        else if( coins[c].GetType() == CurrencySystem.typeofVerite ) verite += coins[c].Amount;
                                        else if( coins[c].GetType() == CurrencySystem.typeofValorite ) valorite += coins[c].Amount;
                                    }

                                    Say(String.Format("Thy current bank balance is {0} valorite, {1} verite, and {2} gold.", valorite, verite, gold));
                                }
                                else
                                {
                                    Say("Thy bank box doth not have any coins.");
                                }

                                break;
                            }
                        case 0x0002: // *bank*
                            {
                                e.Handled = true;

                                if( e.Mobile.Criminal )
                                {
                                    this.Say(500378); // Thou art a criminal and cannot access thy bank box.
                                    break;
                                }

                                e.Mobile.BankBox.Open();

                                break;
                            }
                        case 0x0003: // *check*
                            {
                                //e.Handled = true;

                                //if( e.Mobile.Criminal )
                                //{
                                //    this.Say( 500389 ); // I will not do business with a criminal!
                                //    break;
                                //}

                                //string[] split = e.Speech.Split( ' ' );

                                //if( split.Length >= 2 )
                                //{
                                //    int amount;

                                //    try
                                //    {
                                //        amount = Convert.ToInt32( split[1] );
                                //    }
                                //    catch
                                //    {
                                //        break;
                                //    }

                                //    if( amount < 5000 )
                                //    {
                                //        this.Say( 1010006 ); // We cannot create checks for such a paltry amount of gold!
                                //    }
                                //    else if( amount > 1000000 )
                                //    {
                                //        this.Say( 1010007 ); // Our policies prevent us from creating checks worth that much!
                                //    }
                                //    else
                                //    {
                                //        BankCheck check = new BankCheck( amount );

                                //        BankBox box = e.Mobile.BankBox;

                                //        if( !box.TryDropItem( e.Mobile, check, false ) )
                                //        {
                                //            this.Say( 500386 ); // There's not enough room in your bankbox for the check!
                                //            check.Delete();
                                //        }
                                //        else if( !box.ConsumeTotal( typeof( Gold ), amount ) )
                                //        {
                                //            this.Say( 500384 ); // Ah, art thou trying to fool me? Thou hast not so much gold!
                                //            check.Delete();
                                //        }
                                //        else
                                //        {
                                //            this.Say( 1042673, AffixType.Append, amount.ToString(), "" ); // Into your bank box I have placed a check in the amount of:
                                //        }
                                //    }
                                //}

                                break;
                            }
                    }
                }

                if( e.Speech.ToLower().IndexOf("exchange") > -1 )
                {
                    BankBox box = e.Mobile.FindBankNoCreate();

                    if( box != null )
                    {
                        int cc = 0, sc = 0, gc = 0;
                        Point3D ccLoc = Point3D.Zero, scLoc = Point3D.Zero, gcLoc = Point3D.Zero;

                        List<BaseCoin> coins = box.FindItemsByType<BaseCoin>();

                        coins.ForEach(
                            delegate( BaseCoin coin )
                            {
                                if( coin.GetType() == CurrencySystem.typeofGold )
                                {
                                    cc += coin.Amount;
                                    ccLoc = coin.Location;
                                }
                                else if( coin.GetType() == CurrencySystem.typeofVerite )
                                {
                                    sc += coin.Amount;
                                    scLoc = coin.Location;
                                }
                                else if( coin.GetType() == CurrencySystem.typeofValorite )
                                {
                                    gc += coin.Amount;
                                    gcLoc = coin.Location;
                                }

                                coin.Delete();
                            });

                        int[] newAmts = CurrencySystem.Compress(cc, sc, gc);

                        if( newAmts[0] > 0 )
                        {
                            Gold gold = new Gold(newAmts[0]);

                            box.AddItem(gold);
                            gold.Location = ccLoc;
                        }

                        if( newAmts[1] > 0 )
                        {
                            Verite silver = new Verite(newAmts[1]);

                            box.DropItem(silver);
                            silver.Location = scLoc;
                        }

                        if( newAmts[2] > 0 )
                        {
                            Valorite gold = new Valorite(newAmts[2]);

                            box.DropItem(gold);
                            gold.Location = gcLoc;
                        }
                    }
                }
            }

            base.OnSpeech(e);
        }

        public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
        {
            if( from.Alive )
                list.Add(new OpenBankEntry(from, this));

            base.AddCustomContextEntries(from, list);
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}