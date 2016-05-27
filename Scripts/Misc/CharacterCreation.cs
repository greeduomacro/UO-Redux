using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Accounting;
using Server.Spells;
using Server.Currency;

namespace Server.Misc
{
    public class CharacterCreation
    {
        public static void Initialize()
        {
            // Register our event handler
            EventSink.CharacterCreated += new CharacterCreatedEventHandler(EventSink_CharacterCreated);
        }

        private static void AddBackpack(Mobile m)
        {
            Container pack = m.Backpack;

            if (pack == null)
            {
                pack = new Backpack();
                pack.Movable = false;

                m.AddItem(pack);
            }
        }

        private static Item MakeNewbie(Item item)
        {
            if (!Core.AOS)
                item.LootType = LootType.Newbied;

            return item;
        }

        private static void PlaceItemIn(Container parent, int x, int y, Item item)
        {
            parent.AddItem(item);
            item.Location = new Point3D(x, y, 0);
        }

        private static Item MakePotionKeg(PotionEffect type, int hue)
        {
            PotionKeg keg = new PotionKeg();

            keg.Held = 100;
            keg.Type = type;
            keg.Hue = hue;

            return MakeNewbie(keg);
        }

        private static void FillBankAOS(Mobile m)
        {
        }

        private static void FillBankbox(Mobile m)
        {
        }

        private static void AddPowerScrolls(BankBox bank)
        {
            Bag bag = new Bag();

            for (int i = 0; i < PowerScroll.Skills.Count; ++i)
                bag.DropItem(new PowerScroll(PowerScroll.Skills[i], 120.0));

            bag.DropItem(new StatCapScroll(250));

            bank.DropItem(bag);
        }

        private static void AddShirt(Mobile m, int shirtHue)
        {
            int hue = Utility.ClipDyedHue(shirtHue & 0x3FFF);


            switch (Utility.Random(3))
            {
                case 0: EquipItem(new Shirt(hue), true); break;
                case 1: EquipItem(new FancyShirt(hue), true); break;
                case 2: EquipItem(new Doublet(hue), true); break;
            }

        }

        private static void AddPants(Mobile m, int pantsHue)
        {
            int hue = Utility.ClipDyedHue(pantsHue & 0x3FFF);

            if (m.Female)
            {
                switch (Utility.Random(2))
                {
                    case 0: EquipItem(new Skirt(hue), true); break;
                    case 1: EquipItem(new Kilt(hue), true); break;
                }
            }
            else
            {
                switch (Utility.Random(2))
                {
                    case 0: EquipItem(new LongPants(hue), true); break;
                    case 1: EquipItem(new ShortPants(hue), true); break;
                }
            }
           
        }

        private static void AddShoes(Mobile m)
        {
                EquipItem(new Shoes(Utility.RandomYellowHue()), true);
        }

        private static Mobile CreateMobile(Account a)
        {
            if (a.Count >= a.Limit)
                return null;

            for (int i = 0; i < a.Length; ++i)
            {
                if (a[i] == null)
                    return (a[i] = new Player());
            }

            return null;
        }

        private static void EventSink_CharacterCreated(CharacterCreatedEventArgs args)
        {
            if (!VerifyProfession(args.Profession))
                args.Profession = 0;

            NetState state = args.State;

            if (state == null)
                return;

            Mobile newChar = CreateMobile(args.Account as Account);

            args.Mobile = newChar;
            m_Mobile = newChar;

            if (newChar == null)
            {
                Console.WriteLine("Login: {0}: Character creation failed, account full", state);
                return;
            }

            if (newChar is Player)
            {
                Player pm = (Player)newChar;
                pm.Profession = args.Profession;
            }

            newChar.Player = true;
            newChar.AccessLevel = args.Account.AccessLevel;
            newChar.Female = args.Female;
            newChar.Body = newChar.Female ? 0x191 : 0x190;
            newChar.Hue = Utility.ClipSkinHue(args.Hue & 0x3FFF) | 0x8000;
            newChar.Hunger = 20;
            newChar.Thirst = 20;
            newChar.SkillsCap = 8000;
            newChar.StatCap = 250;
            newChar.Name = args.Name;

            ((Player)newChar).EoC += 20000;
            ((Player)newChar).Race = Race.Human;

            newChar.CantWalk = false;
            newChar.Frozen = false;

            newChar.Hits = newChar.HitsMax;
            newChar.Mana = newChar.ManaMax;
            newChar.Stam = newChar.StamMax;

            AddBackpack(newChar);

            newChar.AddToBackpack(new Verite(150));

            Bag bag = new Bag();
            bag.Name = "A torn bag";
            bag.Hue = Utility.RandomNeutralHue();
            newChar.AddToBackpack(bag);

            //bag.TryDropItem(newChar, new ShepherdsCrook(), false);
            //bag.TryDropItem(newChar, new Bandage(Utility.Random(20,30)), false);
            //bag.TryDropItem(newChar, new Kindling(15), false);
            //bag.TryDropItem(newChar, new Torch(), false);
            //bag.TryDropItem(newChar, new Dagger(), false);
            //bag.TryDropItem(newChar, new TotalRefreshPotion(), false);
            //bag.TryDropItem(newChar, new GreaterHealPotion(), false);
            //bag.TryDropItem(newChar, new GreaterCurePotion(), false);
            //bag.TryDropItem(newChar, new IDWand(Utility.Random(10,15)), false);
            //bag.TryDropItem(newChar, new Pitcher(BeverageType.Water), false);

            bag.AddItem(new Bandage(30));
            bag.AddItem(new Kindling(10));
            bag.AddItem(new Torch());
            bag.AddItem(new Dagger());
            bag.AddItem(new Scissors());
            bag.AddItem(new TotalRefreshPotion());
            bag.AddItem(new GreaterCurePotion());
            bag.AddItem(new GreaterHealPotion());
            bag.AddItem(new IDWand());
            bag.AddItem(new Pitcher());

            newChar.AddToBackpack(new SkillScroll());

            for (int i = 0; i < SkillInfo.Table.Length; i++)
            {
                newChar.Skills[i].Base = 0;

                if (SkillInfo.Table[i].SkillID > (int)SkillName.Ninjitsu)
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

            SetStats(newChar, state, args.Str, args.Dex, args.Int);
            SetSkills(newChar, args.Skills, args.Profession);

            newChar.MoveToWorld(new Point3D(2756, 3808, 50), Map.Felucca); /// Isle of Kesna'buir
            ((Player)newChar).RespawnLocation = (new Point3D(2756, 3808, 50));
            ((Player)newChar).RespawnMap = Map.Felucca;

            Console.WriteLine("Login: {0}: New character being created (account={1})", args.State, args.Account.Username);
            Console.WriteLine(" - Character: {0} (serial={1})", newChar.Name, newChar.Serial);
        }

        public static bool VerifyProfession(int profession)
        {
            if (profession < 0)
                return false;
            else if (profession < 4)
                return true;
            else if (Core.AOS && profession < 6)
                return true;
            else if (Core.SE && profession < 8)
                return true;
            else
                return false;
        }

        private class BadStartMessage : Timer
        {
            Mobile m_Mobile;
            int m_Message;
            public BadStartMessage(Mobile m, int message)
                : base(TimeSpan.FromSeconds(3.5))
            {
                m_Mobile = m;
                m_Message = message;
                this.Start();
            }

            protected override void OnTick()
            {
                m_Mobile.SendLocalizedMessage(m_Message);
            }
        }

        private static readonly CityInfo m_NewHavenInfo = new CityInfo("New Haven", "The Bountiful Harvest Inn", 3503, 2574, 14, Map.Trammel);

        private static CityInfo GetStartLocation(CharacterCreatedEventArgs args, bool isYoung)
        {
            if (Core.ML)
            {
                //if( args.State != null && args.State.NewHaven )
                return m_NewHavenInfo;	//We don't get the client Version until AFTER Character creation

                //return args.City;  TODO: Uncomment when the old quest system is actually phased out
            }

            bool useHaven = isYoung;

            ClientFlags flags = args.State == null ? ClientFlags.None : args.State.Flags;
            Mobile m = args.Mobile;

            switch (args.Profession)
            {
                case 4: //Necro
                    {
                        if ((flags & ClientFlags.Malas) != 0)
                        {
                            return new CityInfo("Umbra", "Mardoth's Tower", 2114, 1301, -50, Map.Malas);
                        }
                        else
                        {
                            useHaven = true;

                            new BadStartMessage(m, 1062205);
                            /*
                             * Unfortunately you are playing on a *NON-Age-Of-Shadows* game 
                             * installation and cannot be transported to Malas.  
                             * You will not be able to take your new player quest in Malas 
                             * without an AOS client.  You are now being taken to the city of 
                             * Haven on the Trammel facet.
                             * */
                        }

                        break;
                    }
                case 5:	//Paladin
                    {
                        return m_NewHavenInfo;
                    }
                case 6:	//Samurai
                    {
                        if ((flags & ClientFlags.Tokuno) != 0)
                        {
                            return new CityInfo("Samurai DE", "Haoti's Grounds", 368, 780, -1, Map.Malas);
                        }
                        else
                        {
                            useHaven = true;

                            new BadStartMessage(m, 1063487);
                            /*
                             * Unfortunately you are playing on a *NON-Samurai-Empire* game 
                             * installation and cannot be transported to Tokuno. 
                             * You will not be able to take your new player quest in Tokuno 
                             * without an SE client. You are now being taken to the city of 
                             * Haven on the Trammel facet.
                             * */
                        }

                        break;
                    }
                case 7:	//Ninja
                    {
                        if ((flags & ClientFlags.Tokuno) != 0)
                        {
                            return new CityInfo("Ninja DE", "Enimo's Residence", 414, 823, -1, Map.Malas);
                        }
                        else
                        {
                            useHaven = true;

                            new BadStartMessage(m, 1063487);
                            /*
                             * Unfortunately you are playing on a *NON-Samurai-Empire* game 
                             * installation and cannot be transported to Tokuno. 
                             * You will not be able to take your new player quest in Tokuno 
                             * without an SE client. You are now being taken to the city of 
                             * Haven on the Trammel facet.
                             * */
                        }

                        break;
                    }
            }

            if (useHaven)
                return m_NewHavenInfo;
            else
                return args.City;
        }

        private static void FixStats(ref int str, ref int dex, ref int intel, int max)
        {
            int vMax = max - 30;

            int vStr = str - 10;
            int vDex = dex - 10;
            int vInt = intel - 10;

            if (vStr < 0)
                vStr = 0;

            if (vDex < 0)
                vDex = 0;

            if (vInt < 0)
                vInt = 0;

            int total = vStr + vDex + vInt;

            if (total == 0 || total == vMax)
                return;

            double scalar = vMax / (double)total;

            vStr = (int)(vStr * scalar);
            vDex = (int)(vDex * scalar);
            vInt = (int)(vInt * scalar);

            FixStat(ref vStr, (vStr + vDex + vInt) - vMax, vMax);
            FixStat(ref vDex, (vStr + vDex + vInt) - vMax, vMax);
            FixStat(ref vInt, (vStr + vDex + vInt) - vMax, vMax);

            str = vStr + 10;
            dex = vDex + 10;
            intel = vInt + 10;
        }

        private static void FixStat(ref int stat, int diff, int max)
        {
            stat += diff;

            if (stat < 0)
                stat = 0;
            else if (stat > max)
                stat = max;
        }

        private static void SetStats(Mobile m, NetState state, int str, int dex, int intel)
        {
            int max = state.NewCharacterCreation ? 90 : 80;

            FixStats(ref str, ref dex, ref intel, max);

            if (str < 10 || str > 60 || dex < 10 || dex > 60 || intel < 10 || intel > 60 || (str + dex + intel) != max)
            {
                str = 10;
                dex = 10;
                intel = 10;
            }

            m.InitStats(str, dex, intel);
        }

        private static void SetName(Mobile m, string name)
        {
            name = name.Trim();

            if (!NameVerification.Validate(name, 2, 16, true, false, true, 1, NameVerification.SpaceDashPeriodQuote))
                name = "Generic Player";

            m.Name = name;
        }

        private static bool ValidSkills(SkillNameValue[] skills)
        {
            int total = 0;

            for (int i = 0; i < skills.Length; ++i)
            {
                if (skills[i].Value < 0 || skills[i].Value > 50)
                    return false;

                total += skills[i].Value;

                for (int j = i + 1; j < skills.Length; ++j)
                {
                    if (skills[j].Value > 0 && skills[j].Name == skills[i].Name)
                        return false;
                }
            }

            return (total == 100 || total == 120);
        }

        private static Mobile m_Mobile;

        private static void SetSkills(Mobile m, SkillNameValue[] skills, int prof)
        {
            switch (prof)
            {
                case 1: // Warrior
                    {
                        skills = new SkillNameValue[]
						{
							new SkillNameValue( SkillName.Anatomy, 30 ),
							new SkillNameValue( SkillName.Healing, 45 ),
							new SkillNameValue( SkillName.Swords, 35 ),
							new SkillNameValue( SkillName.Tactics, 50 )
						};

                        break;
                    }
                case 2: // Magician
                    {
                        skills = new SkillNameValue[]
						{
							new SkillNameValue( SkillName.EvalInt, 30 ),
							new SkillNameValue( SkillName.Wrestling, 30 ),
							new SkillNameValue( SkillName.Magery, 50 ),
							new SkillNameValue( SkillName.Meditation, 50 )
						};

                        break;
                    }
                case 3: // Blacksmith
                    {
                        skills = new SkillNameValue[]
						{
							new SkillNameValue( SkillName.Mining, 30 ),
							new SkillNameValue( SkillName.ArmsLore, 30 ),
							new SkillNameValue( SkillName.Blacksmith, 50 ),
							new SkillNameValue( SkillName.Tinkering, 50 )
						};

                        break;
                    }
                case 4: // Necromancer
                    {
                        skills = new SkillNameValue[]
						{
							new SkillNameValue( SkillName.Necromancy, 50 ),
							new SkillNameValue( SkillName.Focus, 30 ),
							new SkillNameValue( SkillName.SpiritSpeak, 30 ),
							new SkillNameValue( SkillName.Swords, 30 ),
							new SkillNameValue( SkillName.Tactics, 20 )
						};

                        break;
                    }
                case 5: // Paladin
                    {
                        skills = new SkillNameValue[]
						{
							new SkillNameValue( SkillName.Chivalry, 51 ),
							new SkillNameValue( SkillName.Swords, 49 ),
							new SkillNameValue( SkillName.Focus, 30 ),
							new SkillNameValue( SkillName.Tactics, 30 )
						};

                        break;
                    }
                case 6:	//Samurai
                    {
                        skills = new SkillNameValue[]
						{
							new SkillNameValue( SkillName.Bushido, 50 ),
							new SkillNameValue( SkillName.Swords, 50 ),
							new SkillNameValue( SkillName.Anatomy, 30 ),
							new SkillNameValue( SkillName.Healing, 30 )
					};
                        break;
                    }
                case 7:	//Ninja
                    {
                        skills = new SkillNameValue[]
						{
							new SkillNameValue( SkillName.Ninjitsu, 50 ),
							new SkillNameValue( SkillName.Hiding, 50 ),
							new SkillNameValue( SkillName.Fencing, 30 ),
							new SkillNameValue( SkillName.Stealth, 30 )
						};
                        break;
                    }
                default:
                    {
                        if (!ValidSkills(skills))
                            return;

                        break;
                    }
            }

            bool addSkillItems = true;

            switch (prof)
            {
                case 1: // Warrior
                    {
                            EquipItem(new LeatherChest());
                        break;
                    }
                case 4: // Necromancer
                    {
                        Container regs = new BagOfNecroReagents(50);

                        if (!Core.AOS)
                        {
                            foreach (Item item in regs.Items)
                                item.LootType = LootType.Newbied;
                        }

                        PackItem(regs);

                        regs.LootType = LootType.Regular;

                        EquipItem(new BoneHelm());


                        EquipItem(new BoneHarvester());
                        EquipItem(NecroHue(new LeatherChest()));
                        EquipItem(NecroHue(new LeatherArms()));
                        EquipItem(NecroHue(new LeatherGloves()));
                        EquipItem(NecroHue(new LeatherGorget()));
                        EquipItem(NecroHue(new LeatherLegs()));
                        EquipItem(NecroHue(new Skirt()));
                        EquipItem(new Sandals(0x8FD));


                        Spellbook book = new NecromancerSpellbook((ulong)0x8981); // animate dead, evil omen, pain spike, summon familiar, wraith form

                        PackItem(book);

                        book.LootType = LootType.Blessed;

                        addSkillItems = false;

                        break;
                    }
                case 5: // Paladin
                    {

                        EquipItem(new Broadsword());
                        EquipItem(new Helmet());
                        EquipItem(new PlateGorget());
                        EquipItem(new RingmailArms());
                        EquipItem(new RingmailChest());
                        EquipItem(new RingmailLegs());
                        EquipItem(new ThighBoots(0x748));
                        EquipItem(new Cloak(0xCF));
                        EquipItem(new BodySash(0xCF));


                        Spellbook book = new BookOfChivalry((ulong)0x3FF);

                        PackItem(book);

                        book.LootType = LootType.Blessed;

                        addSkillItems = false;

                        break;
                    }

                case 6: // Samurai
                    {
                        addSkillItems = false;
                        EquipItem(new HakamaShita(0x2C3));
                        EquipItem(new Hakama(0x2C3));
                        EquipItem(new SamuraiTabi(0x2C3));
                        EquipItem(new TattsukeHakama(0x22D));
                        EquipItem(new Bokuto());
                        EquipItem(new LeatherJingasa());

                        PackItem(new Scissors());
                        PackItem(new Bandage(50));

                        Spellbook book = new BookOfBushido();
                        PackItem(book);

                        break;
                    }
                case 7: // Ninja
                    {
                        addSkillItems = false;
                        EquipItem(new Kasa());

                        int[] hues = new int[] { 0x1A8, 0xEC, 0x99, 0x90, 0xB5, 0x336, 0x89 };
                        //TODO: Verify that's ALL the hues for that above.

                        EquipItem(new TattsukeHakama(hues[Utility.Random(hues.Length)]));

                        EquipItem(new HakamaShita(0x2C3));
                        EquipItem(new NinjaTabi(0x2C3));
                        EquipItem(new Tekagi());

                        PackItem(new SmokeBomb());

                        Spellbook book = new BookOfNinjitsu();
                        PackItem(book);

                        break;
                    }
            }

            for (int i = 0; i < skills.Length; ++i)
            {
                SkillNameValue snv = skills[i];

                if (snv.Value > 0 && (snv.Name != SkillName.Stealth || prof == 7) 
                    && snv.Name != SkillName.RemoveTrap && snv.Name != SkillName.Spellweaving)
                {
                    Skill skill = m.Skills[snv.Name];

                    if (skill != null)
                    {
                        skill.BaseFixedPoint = snv.Value * 10;

                        if (addSkillItems)
                            AddSkillItems(snv.Name, m);
                    }
                }
            }
        }

        private static void EquipItem(Item item)
        {
            EquipItem(item, false);
        }

        private static void EquipItem(Item item, bool mustEquip)
        {
            if (!Core.AOS)
                item.LootType = LootType.Newbied;

            if (m_Mobile != null && m_Mobile.EquipItem(item))
                return;

            Container pack = m_Mobile.Backpack;

            if (!mustEquip && pack != null)
                pack.DropItem(item);
            else
                item.Delete();
        }

        private static void PackItem(Item item)
        {
            if (!Core.AOS)
                item.LootType = LootType.Newbied;

            Container pack = m_Mobile.Backpack;

            if (pack != null)
                pack.DropItem(item);
            else
                item.Delete();
        }

        private static void PackInstrument()
        {
            switch (Utility.Random(6))
            {
                case 0: PackItem(new Drums()); break;
                case 1: PackItem(new Harp()); break;
                case 2: PackItem(new LapHarp()); break;
                case 3: PackItem(new Lute()); break;
                case 4: PackItem(new Tambourine()); break;
                case 5: PackItem(new TambourineTassel()); break;
            }
        }

        private static void PackScroll(int circle)
        {
            switch (Utility.Random(8) * (circle + 1))
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
                case 16: PackItem(new BlessScroll()); break;
                case 17: PackItem(new FireballScroll()); break;
                case 18: PackItem(new MagicLockScroll()); break;
                case 19: PackItem(new PoisonScroll()); break;
                case 20: PackItem(new TelekinisisScroll()); break;
                case 21: PackItem(new TeleportScroll()); break;
                case 22: PackItem(new UnlockScroll()); break;
                case 23: PackItem(new WallOfStoneScroll()); break;
            }
        }

        private static Item NecroHue(Item item)
        {
            item.Hue = 0x2C3;

            return item;
        }

        private static void AddSkillItems(SkillName skill, Mobile m)
        {
            switch (skill)
            {
                case SkillName.Alchemy:
                    {
                        PackItem(new Bottle(4));
                        PackItem(new MortarPestle());

                        int hue = Utility.RandomPinkHue();


                        EquipItem(new Robe(Utility.RandomPinkHue()));
                        break;
                    }
                case SkillName.Anatomy:
                    {
                        PackItem(new Bandage(3));

                        int hue = Utility.RandomYellowHue();

                        EquipItem(new Robe(hue));

                        break;
                    }
                case SkillName.AnimalLore:
                    {
                        int hue = Utility.RandomBlueHue();

                        EquipItem(new ShepherdsCrook());
                        EquipItem(new Robe(hue));

                        break;
                    }
                case SkillName.Archery:
                    {
                        PackItem(new Arrow(25));

                        EquipItem(new Bow());

                        break;
                    }
                case SkillName.ArmsLore:
                    {
                            switch (Utility.Random(3))
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
                case SkillName.Bushido:
                    {
                        EquipItem(new Hakama());
                        EquipItem(new Kasa());
                        EquipItem(new BookOfBushido());
                        break;
                    }
                case SkillName.Fletching:
                    {
                        PackItem(new Board(14));
                        PackItem(new Feather(5));
                        PackItem(new Shaft(5));
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
                        PackItem(new Board(10));
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
                case SkillName.Chivalry:
                    {
                        if (Core.ML)
                            PackItem(new BookOfChivalry((ulong)0x3FF));

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

                        int hue = Utility.RandomYellowHue();

                        EquipItem(new FloppyHat(hue));

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
                        PackItem(new BlankScroll(2));
                        PackItem(new BlueBook());
                        break;
                    }
                case SkillName.ItemID:
                    {
                            EquipItem(new GnarledStaff());
                        break;
                    }
                case SkillName.Lockpicking:
                    {
                        PackItem(new Lockpick(20));
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
                        BagOfReagents regs = new BagOfReagents(30);

                        PackItem(regs);

                        regs.LootType = LootType.Regular;

                        PackScroll(0);
                        PackScroll(1);
                        PackScroll(2);

                        Spellbook book = new Spellbook((ulong)0x382A8C38);

                        EquipItem(book);

                        book.LootType = LootType.Blessed;

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
                case SkillName.Necromancy:
                    {
                        if (Core.ML)
                        {
                            Container regs = new BagOfNecroReagents(50);

                            PackItem(regs);

                            regs.LootType = LootType.Regular;
                        }

                        break;
                    }
                case SkillName.Ninjitsu:
                    {
                        EquipItem(new Hakama(0x2C3));	//Only ninjas get the hued one.
                        EquipItem(new Kasa());
                        EquipItem(new BookOfNinjitsu());
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
                        PackItem(new Lockpick(20));
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
                case SkillName.Tactics:
                    {

                            EquipItem(new Katana());

                        break;
                    }
                case SkillName.Tailoring:
                    {
                        PackItem(new BoltOfCloth());
                        PackItem(new SewingKit());
                        break;
                    }
                case SkillName.Tracking:
                    {
                        if (m_Mobile != null)
                        {
                            Item shoes = m_Mobile.FindItemOnLayer(Layer.Shoes);

                            if (shoes != null)
                                shoes.Delete();
                        }

                        int hue = Utility.RandomYellowHue();

                            EquipItem(new Boots(hue));

                        EquipItem(new SkinningKnife());
                        break;
                    }
                case SkillName.Veterinary:
                    {
                        PackItem(new Bandage(5));
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
    }
}