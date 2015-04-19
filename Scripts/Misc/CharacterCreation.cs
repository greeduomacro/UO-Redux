using Server.Accounting;
using Server.Currency;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System;
using System.Text.RegularExpressions;

namespace Server.Misc
{
    public class CharacterCreation
    {
        private static Mobile m_Mobile;

        public static void Initialize()
        {
            EventSink.CharacterCreated += new CharacterCreatedEventHandler(EventSink_CharacterCreated);
        }

        #region +static bool VerifyProfession( int )
        public static bool VerifyProfession(int profession)
        {
            if(profession < 0)
                return false;
            else if(profession < 4)
                return true;
            else if(Core.AOS && profession < 6)
                return true;
            else if(Core.SE && profession < 8)
                return true;
            else
                return false;
        }
        #endregion

        #region -static void AddBackpack( Mobile )
        private static void AddBackpack(Mobile m)
        {
            Container pack = m.Backpack;

            if(pack == null)
            {
                pack = new Backpack();
                pack.Movable = false;

                m.AddItem(pack);
            }
        }
        #endregion

        #region -static void AddSkillItems( SkillName, Mobile )
        private static void AddSkillItems(SkillName skill, Mobile m)
        {
            switch(skill)
            {
                case SkillName.Alchemy:
                    {
                        PackItem(new Bottle(20));
                        PackItem(new MortarPestle());

                        EquipItem(new Robe(Utility.RandomPinkHue()));

                        break;
                    }
                case SkillName.Anatomy:
                    {
                        PackItem(new Bandage(80));

                        EquipItem(new Robe(Utility.RandomPinkHue()));

                        break;
                    }
                case SkillName.AnimalLore:
                    {
                        EquipItem(new ShepherdsCrook());
                        EquipItem(new Robe(Utility.RandomDyedHue()));

                        break;
                    }
                case SkillName.Archery:
                    {
                        PackItem(new Arrow(150));

                        EquipItem(new Bow());

                        break;
                    }
                case SkillName.ArmsLore:
                    {
                        switch(Utility.Random(3))
                        {
                            case 0: EquipItem(new Kryss()); break;
                            case 1: EquipItem(new Katana()); break;
                            case 2: EquipItem(new Club()); break;
                        }

                        break;
                    }
                case SkillName.Begging:
                    {
                        EquipItem(new GnarledStaff());

                        break;
                    }
                case SkillName.Blacksmith:
                    {
                        PackItem(new Tongs());
                        PackItem(new Pickaxe());
                        PackItem(new Pickaxe());
                        PackItem(new IronIngot(50));

                        EquipItem(new HalfApron(Utility.RandomYellowHue()));

                        break;
                    }
                case SkillName.Fletching:
                    {
                        PackItem(new Board(28));
                        PackItem(new Feather(50));
                        PackItem(new Shaft(50));

                        break;
                    }
                case SkillName.Camping:
                    {
                        PackItem(new Bedroll());
                        PackItem(new Kindling(5));

                        break;
                    }
                case SkillName.Carpentry:
                    {
                        PackItem(new Board(50));
                        PackItem(new Saw());

                        EquipItem(new HalfApron(Utility.RandomYellowHue()));

                        break;
                    }
                case SkillName.Cartography:
                    {
                        PackItem(new BlankMap());
                        PackItem(new BlankMap());
                        PackItem(new BlankMap());
                        PackItem(new BlankMap());
                        PackItem(new Sextant());

                        break;
                    }
                case SkillName.Cooking:
                    {
                        PackItem(new Kindling(2));
                        PackItem(new RawLambLeg());
                        PackItem(new RawChickenLeg());
                        PackItem(new RawFishSteak());
                        PackItem(new SackFlour());
                        PackItem(new Pitcher(BeverageType.Water));

                        break;
                    }
                case SkillName.DetectHidden:
                    {
                        EquipItem(new Cloak(0x455));

                        break;
                    }
                case SkillName.Discordance:
                    {
                        PackInstrument();

                        break;
                    }
                case SkillName.Fencing:
                    {
                        EquipItem(new Kryss());

                        break;
                    }
                case SkillName.Fishing:
                    {
                        EquipItem(new FishingPole());
                        EquipItem(new FloppyHat(Utility.RandomYellowHue()));

                        break;
                    }
                case SkillName.Healing:
                    {
                        PackItem(new Bandage(50));
                        PackItem(new Scissors());

                        break;
                    }
                case SkillName.Herding:
                    {
                        EquipItem(new ShepherdsCrook());

                        break;
                    }
                case SkillName.Hiding:
                    {
                        EquipItem(new Cloak(0x455));

                        break;
                    }
                case SkillName.Inscribe:
                    {
                        PackItem(new BlankScroll(15));
                        PackItem(new BlueBook());
                        PackItem(new ScribesPen(50));

                        break;
                    }
                case SkillName.ItemID:
                    {
                        EquipItem(new GnarledStaff());

                        break;
                    }
                case SkillName.Lockpicking:
                    {
                        PackItem(new Lockpick(30));

                        break;
                    }
                case SkillName.Lumberjacking:
                    {
                        EquipItem(new Hatchet());

                        break;
                    }
                case SkillName.Macing:
                    {
                        EquipItem(new Club());

                        break;
                    }
                case SkillName.Magery:
                    {
                        BagOfReagents regs = new BagOfReagents(50);
                        regs.LootType = LootType.Regular;

                        if(!Core.ML)
                        {
                            foreach(Item item in regs.Items)
                                item.LootType = LootType.Newbied;
                        }

                        PackItem(regs);
                        PackScroll(0);
                        PackScroll(1);
                        PackScroll(2);

                        Spellbook book = new Spellbook();
                        book.LootType = LootType.Blessed;

                        EquipItem(book);
                        EquipItem(new WizardsHat());
                        EquipItem(new Robe(Utility.RandomBlueHue()));

                        break;
                    }
                case SkillName.Mining:
                    {
                        PackItem(new Pickaxe());

                        break;
                    }
                case SkillName.Musicianship:
                    {
                        PackInstrument();

                        break;
                    }
                case SkillName.Parry:
                    {
                        EquipItem(new WoodenShield());

                        break;
                    }
                case SkillName.Peacemaking:
                    {
                        PackInstrument();

                        break;
                    }
                case SkillName.Poisoning:
                    {
                        PackItem(new LesserPoisonPotion());
                        PackItem(new LesserPoisonPotion());

                        break;
                    }
                case SkillName.Provocation:
                    {
                        PackInstrument();

                        break;
                    }
                case SkillName.Snooping:
                    {
                        PackItem(new Lockpick(30));

                        break;
                    }
                case SkillName.SpiritSpeak:
                    {
                        EquipItem(new Cloak(0x455));

                        break;
                    }
                case SkillName.Stealing:
                    {
                        PackItem(new Lockpick(20));

                        break;
                    }
                case SkillName.Swords:
                    {
                        EquipItem(new Katana());

                        break;
                    }
                case SkillName.Tailoring:
                    {
                        PackItem(new BoltOfCloth());
                        PackItem(new SewingKit());
                        PackItem(new DyeTub());
                        PackItem(new Dyes());

                        break;
                    }
                case SkillName.Tracking:
                    {
                        if(m_Mobile != null)
                        {
                            Item shoes = m_Mobile.FindItemOnLayer(Layer.Shoes);

                            if(shoes != null)
                                shoes.Delete();
                        }

                        EquipItem(new Boots(Utility.RandomYellowHue()));
                        EquipItem(new SkinningKnife());

                        break;
                    }
                case SkillName.Veterinary:
                    {
                        PackItem(new Bandage(45));
                        PackItem(new Scissors());

                        break;
                    }
                case SkillName.Wrestling:
                    {
                        EquipItem(new LeatherGloves());

                        break;
                    }
            }
        }
        #endregion

        private static Mobile CreateMobile(Account a)
        {
            if(a.Count >= a.Limit || a.Count >= AccountHandler.ServerLimit)
                return null;

            for(int i = 0; i < a.Length; ++i)
            {
                if(a[i] == null)
                    return (a[i] = new Player());
            }

            return null;
        }

        #region -static void EquipItem(...)
        private static void EquipItem(Item item)
        {
            EquipItem(item, false);
        }

        private static void EquipItem(Item item, bool mustEquip)
        {
            if(!Core.SE)
                item.LootType = LootType.Newbied;

            if(m_Mobile != null && m_Mobile.EquipItem(item))
                return;

            Container pack = m_Mobile.Backpack;

            if(!mustEquip && pack != null)
                pack.DropItem(item);
            else
                item.Delete();
        }
        #endregion

        private static void EventSink_CharacterCreated(CharacterCreatedEventArgs args)
        {
            if(!VerifyProfession(args.Profession))
                args.Profession = 0;

            Mobile newChar = CreateMobile(args.Account as Account);

            if(newChar == null)
            {
                Console.WriteLine("Login: {0}: Character creation failed, account full", args.State);
                return;
            }

            args.Mobile = newChar;
            m_Mobile = newChar;

            newChar.Player = true;
            newChar.AccessLevel = args.Account.AccessLevel;
            newChar.Female = args.Female;
            newChar.Body = newChar.Female ? 0x191 : 0x190;
            newChar.Hue = Utility.ClipSkinHue(args.Hue & 0x3FFF) | 0x8000;
            newChar.Hunger = 20;
            newChar.Thirst = 20;
            newChar.SkillsCap = 7000;
            newChar.StatCap = 225;
            newChar.Name = args.Name;
            newChar.RawInt = 75;
            newChar.RawDex = 75;
            newChar.RawStr = 75;

            ((Player)newChar).EoC += 20000;
            ((Player)newChar).Race = Race.Human;

            newChar.CantWalk = false;
            newChar.Frozen = false;

            newChar.Hits = newChar.HitsMax;
            newChar.Mana = newChar.ManaMax;
            newChar.Stam = newChar.StamMax;

            AddBackpack(newChar);

            newChar.AddToBackpack(new Silver(100));
            newChar.AddToBackpack(new Copper(500));

            newChar.AddToBackpack(new SkillScroll());

            for(int i = 0; i < SkillInfo.Table.Length; i++)
            {
                newChar.Skills[i].Base = 0;

                if(SkillInfo.Table[i].SkillID >= (int)SkillName.Necromancy)
                    newChar.Skills[i].Cap = 0;
                else
                    newChar.Skills[i].Cap = 100;
            }

            newChar.HairItemID = args.HairID;
            newChar.FacialHairItemID = args.BeardID;

            newChar.HairHue = Utility.ClipHairHue(args.HairHue & 0x3FFF);
            newChar.FacialHairHue = Utility.ClipHairHue(args.BeardHue & 0x3FFF);

            AddShirt(newChar, args.ShirtHue);
            AddPants(newChar, args.PantsHue);
            AddShoes(newChar);

            FillBankbox(newChar);

            CityInfo ci = args.City;
            newChar.MoveToWorld(ci.Location, ci.Map);

            ((Player)newChar).RespawnLocation = ci.Location;
            ((Player)newChar).RespawnMap = ci.Map;

            Console.WriteLine("Login: {0}: New character being created (account={1})", args.State, args.Account.Username);
            Console.WriteLine(" - Character: {0} (serial={1})", newChar.Name, newChar.Serial);
            Console.WriteLine(" - Started: {0} {1} in {2}", ci.City, ci.Location, ci.Map.ToString());

            new WelcomeTimer(newChar).Start();
        }

        #region -static void FixStat( ref int, int)
        private static void FixStat(ref int stat, int diff)
        {
            stat += diff;

            if(stat < 0)
                stat = 0;
            else if(stat > 50)
                stat = 50;
        }
        #endregion

        #region -static void FixStats( ref int, ref int, ref int )
        private static void FixStats(ref int str, ref int dex, ref int intel)
        {
            int vStr = str - 10;
            int vDex = dex - 10;
            int vInt = intel - 10;

            if(vStr < 0)
                vStr = 0;

            if(vDex < 0)
                vDex = 0;

            if(vInt < 0)
                vInt = 0;

            int total = vStr + vDex + vInt;

            if(total == 0 || total == 50)
                return;

            double scalar = 50 / (double)total;

            vStr = (int)(vStr * scalar);
            vDex = (int)(vDex * scalar);
            vInt = (int)(vInt * scalar);

            FixStat(ref vStr, (vStr + vDex + vInt) - 50);
            FixStat(ref vDex, (vStr + vDex + vInt) - 50);
            FixStat(ref vInt, (vStr + vDex + vInt) - 50);

            str = vStr + 10;
            dex = vDex + 10;
            intel = vInt + 10;
        }
        #endregion

        #region -static void PackInstrument()
        private static void PackInstrument()
        {
            switch(Utility.Random(6))
            {
                case 0: PackItem(new Drums()); break;
                case 1: PackItem(new Harp()); break;
                case 2: PackItem(new LapHarp()); break;
                case 3: PackItem(new Lute()); break;
                case 4: PackItem(new Tambourine()); break;
                case 5: PackItem(new TambourineTassel()); break;
            }
        }
        #endregion

        #region -static void PackItem( Item )
        private static void PackItem(Item item)
        {
            if(!Core.SE)
                item.LootType = LootType.Newbied;

            Container pack = m_Mobile.Backpack;

            if(pack != null)
                pack.DropItem(item);
            else
                item.Delete();
        }
        #endregion

        #region -static void PackScroll( int )
        private static void PackScroll(int circle)
        {
            switch(Utility.Random(8) * (circle * 8))
            {
                case 0: PackItem(new ClumsyScroll()); break;
                case 1: PackItem(new CreateFoodScroll()); break;
                case 2: PackItem(new FeeblemindScroll()); break;
                case 3: PackItem(new HealScroll()); break;
                case 4: PackItem(new MagicArrowScroll()); break;
                case 5: PackItem(new NightSightScroll()); break;
                case 6: PackItem(new ReactiveArmorScroll()); break;
                case 7: PackItem(new WeakenScroll()); break;
                case 8: PackItem(new AgilityScroll()); break;
                case 9: PackItem(new CunningScroll()); break;
                case 10: PackItem(new CureScroll()); break;
                case 11: PackItem(new HarmScroll()); break;
                case 12: PackItem(new MagicTrapScroll()); break;
                case 13: PackItem(new MagicUnTrapScroll()); break;
                case 14: PackItem(new ProtectionScroll()); break;
                case 15: PackItem(new StrengthScroll()); break;
                case 16: PackItem(new SkillScroll()); break;
                case 17: PackItem(new FireballScroll()); break;
                case 18: PackItem(new MagicLockScroll()); break;
                case 19: PackItem(new PoisonScroll()); break;
                case 20: PackItem(new TelekinisisScroll()); break;
                case 21: PackItem(new TeleportScroll()); break;
                case 22: PackItem(new UnlockScroll()); break;
                case 23: PackItem(new WallOfStoneScroll()); break;
            }
        }
        #endregion

        #region -static void SetName( Mobile, string )
        private static void SetName(Mobile m, string name)
        {
            bool badName = false;
            name = name.Trim();

            for(int i = 0; i < m.Account.Length; i++)
            {
                if(m.Account[i] != null)
                {
                    if(Insensitive.Compare(m.Account[i].RawName, name) == 0)
                        badName = true;
                }
            }

            Regex rx = new Regex(" ");
            Match match = rx.Match(name);

            if(match.Success)
                badName = true;

            if(badName || !NameVerification.Validate(name, 2, 16, true, true, true, 1, NameVerification.SpaceDashPeriodQuote))
                name = NameList.RandomName(m.Female ? "female" : "male");

            m.Name = name;
        }
        #endregion


        #region -static void SetStats( Mobile, int, int, int )
        private static void SetStats(Mobile m, int str, int dex, int intel)
        {
            FixStats(ref str, ref dex, ref intel);

            if(str < 10 || str > 60 || dex < 10 || dex > 60 || intel < 10 || intel > 60 || (str + dex + intel) != 80)
            {
                str = 10;
                dex = 10;
                intel = 10;
            }

            m.InitStats(str, dex, intel);
        }
        #endregion

        #region -static bool ValidSkills( SkillNameValue[] )
        private static bool ValidSkills(SkillNameValue[] skills)
        {
            int total = 0;

            for(int i = 0; i < skills.Length; ++i)
            {
                if(skills[i].Value < 0 || skills[i].Value > 50)
                    return false;

                total += skills[i].Value;

                for(int j = i + 1; j < skills.Length; ++j)
                {
                    if(skills[j].Value > 0 && skills[j].Name == skills[i].Name)
                        return false;
                }
            }

            return (total == 100);
        }
        #endregion

        #region Item/BankBox Additions
        private static void PlaceItemIn(Container parent, int x, int y, Item item)
        {
            parent.AddItem(item);
            item.Location = new Point3D(x, y, 0);
        }

        private static void FillBankbox(Mobile m)
        {
            BankBox bank = m.BankBox;

            if(bank == null)
                return;
        }

        private static void AddShirt(Mobile m, int shirtHue)
        {
            int hue = Utility.ClipDyedHue(shirtHue & 0x3FFF);

            switch(Utility.Random(3))
            {
                case 0: EquipItem(new Shirt(hue), true); break;
                case 1: EquipItem(new FancyShirt(hue), true); break;
                case 2: EquipItem(new Doublet(hue), true); break;
            }
        }

        private static void AddPants(Mobile m, int pantsHue)
        {
            int hue = Utility.ClipDyedHue(pantsHue & 0x3FFF);

            if(m.Female)
                EquipItem(new Skirt(hue), true);
            else
                EquipItem(new LongPants(hue), true);
        }

        private static void AddShoes(Mobile m)
        {
            EquipItem(new Shoes(Utility.RandomYellowHue()), true);
        }
        #endregion
    }
}
