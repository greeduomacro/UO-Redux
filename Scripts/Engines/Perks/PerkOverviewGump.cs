using System;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Utilities;

namespace Server.Perks
{
    public class PerkOverviewGump : Gump
    {
        public enum Page
        {
            Overview,
            Detail
        }

        private const int LabelHue = 1152;
        private const int HtmlBlack = 0x111111;
        private const int HtmlWhite = 0xFFFFFF;

        public Player Player { get; private set; }
        public Perk Perk { get; private set; }
        public PerkLevel CurrentLevel { get; private set; }

        public PerkOverviewGump( Player from )
            : this(from, Page.Overview, null, PerkLevel.First)
        {
        }

        public PerkOverviewGump( Player from, Page page, Perk perk, PerkLevel displayLevel )
            : this(from, page, perk, displayLevel, PerkLevel.None)
        {
        }

        public PerkOverviewGump( Player from, Page page, Perk perk, PerkLevel displayLevel, PerkLevel targetLevel )
            : base(10, 10)
        {
            Player = from;
            Perk = perk;
            CurrentLevel = displayLevel;
            bool hasPerk = Perk.HasPerk(from, perk);

            AddPage(0);

            int bkHeight = 135, bkWidth = 30;

            if( page == Page.Detail )
            {
                if( targetLevel > PerkLevel.None && (!hasPerk || targetLevel != Perk.Get(from, perk).Level) )
                    bkHeight = 475;
                else
                    bkHeight = 415;

                bkWidth = 450;
            }
            else
            {
                int columns = (int)Math.Ceiling((double)Perk.AllPerks.Count / 4);
                int rows = (Perk.AllPerks.Count >= 4 ? 4 : Perk.AllPerks.Count);

                bkHeight += (rows * 70);
                bkWidth += (columns * 105);
            }

            AddBackground(0, 0, bkWidth, bkHeight, 9250);

            switch( page )
            {
                case Page.Overview:
                    AddHtml(15, 15, (bkWidth - 30), 15, Color(Center("Perks Overview"), HtmlWhite), false, false);
                    AddHtml(15, 35, (bkWidth - 30), 70, String.Format("<center>Upgrade your character's Abilities using Essence of Character." +
                                                "<br><br><center>You currently have {0}",
                                                Color(String.Format("{0:N0} Essence of Character", from.EssenceOfCharacter), HtmlBlack)
                                                ), false, true);
                    AddImageTiled(15, 110, (bkWidth - 30), 4, 9151);

                    int x = 20, y = 120;

                    for( int i = 0; i < Perk.AllPerks.Count; i++ )
                    {
                        hasPerk = Perk.HasPerk(from, Perk.AllPerks[i]);

                        if( i > 0 && (i % 4) == 0 )
                        {
                            x += 105;
                            y = 120;
                        }

                        AddBlackAlpha((x - 2), (y + 3), 90, 65);
                        AddLabel(x, y, LabelHue, Perk.AllPerks[i].Label);

                        int perkIcon = Perk.AllPerks[i].GumpID;

                        if( perkIcon == 0 )
                            perkIcon = 21280;

                        AddButton((x + 30), (y + 20), perkIcon, perkIcon, GetButtonId(1, i), GumpButtonType.Reply, 0);
                        AddButton((x + 5), (y + 49), (hasPerk ? 2361 : 2360),
                            (hasPerk ? 2361 : 2360), GetButtonId(1, i), GumpButtonType.Reply, 0);

                        y += 70;
                    }
                    break;
                case Page.Detail:
                    if( perk == null )
                        return;

                    AddHtml(15, 15, (bkWidth - 30), 18, Color(Center(perk.Label), HtmlWhite), false, false);

                    int imgId = perk.GumpID;

                    if( imgId == 0 )
                        imgId = 21280;

                    AddImage(15, 38, imgId);
                    AddHtml(45, 38, (bkWidth - 75), 40, Center(perk.Description), false, false);
                    AddImageTiled(15, 84, (bkWidth - 30), 4, 9151);
                    AddButton(15, 15, 4014, 4015, GetButtonId(2, 0), GumpButtonType.Reply, 0);

                    AddImage(45, 90, 2200);

                    if( displayLevel > PerkLevel.First )
                        AddButton(68, 94, 2205, 2205, GetButtonId(2, 1), GumpButtonType.Reply, 0);

                    if( displayLevel < PerkLevel.Fifth )
                        AddButton(339, 94, 2206, 2206, GetButtonId(2, 2), GumpButtonType.Reply, 0);

                    LabelEntry left = perk.LabelEntries[displayLevel];

                    AddHtml(70, 117, 140, 40, Center(left.Label), false, false);
                    AddHtml(70, 162, 140, 110, Color(left.Description, HtmlBlack), false, true);
                    AddLabel(135, 275, 0, LevelToNumeral(displayLevel));

                    if( displayLevel != PerkLevel.Fifth )
                    {
                        LabelEntry right = perk.LabelEntries[(displayLevel + 1)];

                        AddHtml(230, 117, 140, 40, Center(right.Label), false, false);
                        AddHtml(230, 162, 140, 110, Color(right.Description, HtmlBlack), false, true);
                        AddLabel(295, 275, 0, LevelToNumeral(displayLevel + 1));
                    }

                    string trackProgressLbl = "";

                    if( !hasPerk )
                    {
                        if( Perk.HasFreeSlot(from) )
                        {
                            trackProgressLbl = "You have not started on this perk track.";

                            if( from.EssenceOfCharacter >= Perk.FirstLevelCost )
                            {
                                AddLabel(15, 350, LabelHue, "You can begin developing this perk for 10,000 Essence of Character.");
                                AddButton(205, 380, 4023, 4024, GetButtonId(6, 0), GumpButtonType.Reply, 0);
                                AddLabel(245, 380, LabelHue, String.Format("Begin {0}", perk.Label));
                            }
                            else
                            {
                                AddLabel(15, 350, LabelHue, "You need at least 10,000 Essence of Character to develop a perk.");
                            }
                        }
                        else
                        {
                            trackProgressLbl = "You already have two other perks and cannot begin another track.";
                        }
                    }
                    else
                    {
                        Perk playerPerk = Perk.Get(from, perk);
                        from.ValidateEquipment();

                        if( playerPerk.Level == PerkLevel.Fifth )
                            trackProgressLbl = "You have mastered this perk, congratulations!";
                        else
                            trackProgressLbl = "You are currently progressing on this perk track.";

                        AddLabel(30, 365, LabelHue, "Level:");
                        AddImage(75, 370, 2053); //base progress bar

                        if( targetLevel > PerkLevel.None && targetLevel != playerPerk.Level )
                        {
                            AddImageTiled(75, 370, (4 + GetProgressionDim(targetLevel)), 11, 2057); //yellow progress overlay

                            if( targetLevel == PerkLevel.Fifth )
                            {
                                AddButton(190, 370, 2437, 2438, GetButtonId(4, (int)(targetLevel - 1)), GumpButtonType.Reply, 0);
                                AddLabel(205, 365, LabelHue, "- level");
                            }
                            else
                            {
                                AddButton(190, 360, 2435, 2436, GetButtonId(3, (int)(targetLevel + 1)), GumpButtonType.Reply, 0);
                                AddLabel(205, 355, LabelHue, "+ level");

                                AddButton(190, 380, 2437, 2438, GetButtonId(4, (int)(targetLevel - 1)), GumpButtonType.Reply, 0);
                                AddLabel(205, 375, LabelHue, "- level");
                            }

                            int upgradeCost = GetUpgradeCost(playerPerk.Level, targetLevel);

                            AddLabel(80, 395, LabelHue, "EoC Cost:");
                            AddLabel(155, 395, LabelHue, upgradeCost.ToString("N0"));

                            AddLabel(80, 415, LabelHue, "You have:");
                            AddLabel(155, 415, LabelHue, from.EssenceOfCharacter.ToString("N0"));

                            AddLabel(80, 435, LabelHue, "Remainder:");
                            AddLabel(155, 435, (upgradeCost > from.EssenceOfCharacter ? 32 : 1154), (from.EssenceOfCharacter - upgradeCost).ToString("N0"));

                            if( upgradeCost <= from.EssenceOfCharacter )
                            {
                                AddLabel(285, 440, LabelHue, "Purchase Upgrade");
                                AddButton(405, 440, 4023, 4024, GetButtonId(5, (int)targetLevel), GumpButtonType.Reply, 0);
                            }
                        }
                        else if( playerPerk.Level != PerkLevel.Fifth )
                        {
                            AddButton(190, 370, 2435, 2436, GetButtonId(3, (int)(playerPerk.Level + 1)), GumpButtonType.Reply, 0);
                            AddLabel(205, 365, LabelHue, "+ level");
                        }

                        int progress = GetProgressionDim(playerPerk.Level);

                        AddLabel((75 + progress), 350, LabelHue, ((int)playerPerk.Level).ToString()); //progress level indicator
                        AddImageTiled(75, 370, (4 + progress), 11, 2054); //blue progress overlay
                        AddImage((75 + progress), 370, 2104); //progress pin
                    }

                    AddLabel(15, 330, LabelHue, trackProgressLbl);
                    break;
            }
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            int val, type, index;
            DecodeButtonId(info.ButtonID, out val, out type, out index);

            if( val < 0 || Player == null )
                return;

            Player.CloseGump(typeof(PerkOverviewGump));

            switch( type )
            {
                case 1: //view details
                    Player.SendGump(new PerkOverviewGump(Player, Page.Detail, Perk.AllPerks[index], PerkLevel.First));
                    break;
                case 2: //change detail view
                    switch( index )
                    {
                        case 0: //back to overview
                            Player.SendGump(new PerkOverviewGump(Player));
                            break;
                        case 1: //next page
                            Player.SendGump(new PerkOverviewGump(Player, Page.Detail, Perk, PreviousDisplayLevel(CurrentLevel)));
                            break;
                        case 2: //prev page
                            Player.SendGump(new PerkOverviewGump(Player, Page.Detail, Perk, NextDisplayLevel(CurrentLevel)));
                            break;
                    }
                    break;
                case 3: //buy next level
                case 4: //sell level
                    Player.SendGump(new PerkOverviewGump(Player, Page.Detail, Perk, CurrentLevel, (PerkLevel)index));
                    break;
                case 5: //purchase
                    PurchaseUpgrade(Player, Perk.Get(Player, Perk), (PerkLevel)index);
                    Player.SendGump(new PerkOverviewGump(Player, Page.Detail, Perk, PerkLevel.First));
                    break;
                case 6: //purchase first level
                    Player.SendGump(new WarningGump(1060635, 30720,
                        String.Format("You are about to unlock the {0} perk track. You may only unlock <U>two (2)</U> perks per character, and this action is irreversible. Do you want to continue?", Perk.Label),
                        0xFFC000, 360, 260, new WarningGumpCallback(OnFirstLevelResponse), Tuple.Create<Player, Perk>(Player, Perk)));
                    break;
            }
        }

        /// <summary>
        /// Handles the first level purchase of a perk
        /// </summary>
        public void OnFirstLevelResponse( Mobile from, bool okay, object state )
        {
            if( !okay )
            {
            }
            else if( Perk.HasPerk(Player, Perk) )
            {
                Player.SendMessage("You have already unlocked the {0} perk!", Perk.Label);
            }
            else if( !Perk.HasFreeSlot(Player) )
            {
                Player.SendMessage("You cannot unlock this perk because you have already started developing two other perks.");
            }
            else if( Player.EssenceOfCharacter < Perk.FirstLevelCost )
            {
                Player.SendMessage("You need at least {0:N0} Essence of Character to begin developing perks!", Perk.FirstLevelCost);
            }
            else
            {
                Perk newPerk = null;

                try
                {
                    newPerk = Activator.CreateInstance(Perk.GetType(), Player) as Perk;
                }
                catch( Exception e )
                {
                    ExceptionManager.LogException("PerkOverviewGump.OnFirstLevelResponse()", e);
                }

                if( Perk.Set(Player, newPerk, PerkPosition.Primary, false) )
                {
                    Player.EssenceOfCharacter -= Perk.FirstLevelCost;

                    PlayUpgradeEffect(Player, newPerk, PerkLevel.First);
                }
                else
                {
                    Player.SendMessage("There was a problem unlocking this perk. The server staff has been notified and will contact you soon.");
                    CommandHandlers.BroadcastMessage(AccessLevel.Administrator, 32, String.Format("Alert: a problem has occurred unlocking a perk for '{0}'", Player.RawName));
                }
            }

            Player.SendGump(new PerkOverviewGump(Player, Page.Detail, Perk, PerkLevel.First));
        }

        /// <summary>
        /// Gets the progression dimension coordinate for the given perk level
        /// </summary>
        private int GetProgressionDim( PerkLevel level )
        {
            if( level == PerkLevel.None )
                return 0;

            if( level == PerkLevel.First )
                return 14;

            return 14 + (22 * ((int)level - 1));
        }

        /// <summary>
        /// Gets the cost to upgrade from one perk level to another
        /// </summary>
        private int GetUpgradeCost( PerkLevel currentLevel, PerkLevel targetLevel )
        {
            if( targetLevel <= currentLevel )
                return 0;

            int diff = (targetLevel - currentLevel);
            int sum = 0;

            for( int i = 1; i <= diff; i++ )
            {
                sum += (5000 * (int)(Math.Pow(2, ((int)currentLevel + i))));
            }

            return sum;
        }

        /// <summary>
        /// Converts a perk level to a Roman numeral
        /// </summary>
        private string LevelToNumeral( PerkLevel value )
        {
            string res = "";

            switch( value )
            {
                case PerkLevel.First: res = "I"; break;
                case PerkLevel.Second: res = "II"; break;
                case PerkLevel.Third: res = "III"; break;
                case PerkLevel.Fourth: res = "IV"; break;
                case PerkLevel.Fifth: res = "V"; break;
            }

            return res;
        }

        /// <summary>
        /// Gets the next perk detail page to display
        /// </summary>
        private PerkLevel NextDisplayLevel( PerkLevel level )
        {
            switch( level )
            {
                default:
                case PerkLevel.First:
                    return PerkLevel.Third;
                case PerkLevel.Third:
                    return PerkLevel.Fifth;
                case PerkLevel.Fifth:
                    return PerkLevel.First;
            }
        }

        /// <summary>
        /// Gets the previous perk detail page to display
        /// </summary>
        private PerkLevel PreviousDisplayLevel( PerkLevel level )
        {
            switch( level )
            {
                default:
                case PerkLevel.First:
                    return PerkLevel.First;
                case PerkLevel.Fifth:
                    return PerkLevel.Third;
            }
        }

        /// <summary>
        /// Completes a perk level upgrade
        /// </summary>
        private void PurchaseUpgrade( Player player, Perk currentPerk, PerkLevel targetLevel )
        {
            if( currentPerk == null )
            {
                player.SendMessage("You have not started on this perk track!");
            }
            else if( targetLevel <= currentPerk.Level )
            {
                player.SendMessage("Your current abilities exceed your selected upgrade.");
            }
            else
            {
                int upgradeCost = GetUpgradeCost(currentPerk.Level, targetLevel);

                if( upgradeCost > player.EssenceOfCharacter )
                {
                    player.SendMessage("You do not have enough Essence of Character to afford this upgrade!");
                }
                else
                {
                    player.EssenceOfCharacter -= upgradeCost;
                    currentPerk.Level = targetLevel;

                    PlayUpgradeEffect(player, currentPerk, targetLevel);
                }
            }
        }

        /// <summary>
        /// Plays a special effect for upgrading a perk level
        /// </summary>
        private void PlayUpgradeEffect( Player player, Perk perk, PerkLevel newLevel )
        {
            Effects.SendMovingEffect(new Entity(Server.Serial.Zero, new Point3D(player.Location, player.Z + 20), player.Map), player, 0x373A, 10, 10, false, false);
            Effects.SendTargetEffect(player, 0x3779, 10, 10);
            player.PlaySound(0x0F8);

            if( newLevel == PerkLevel.First )
            {
                player.SendMessage("You have unlocked the first level of the {0} perk!", perk.Label);
            }
            else
            {
                player.SendMessage("You have upgraded {0} to the {1} level!", perk.Label, newLevel);
            }
        }
    }
}