using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Mobiles;
using Server.Items;
using Server.SkillSelection;

namespace Server.Gumps
{
    public class RaceSelectionGump : Gump
    {
        Mobile caller;

        public RaceSelectionGump(Mobile from) : this()
        {
            caller = from;
        }

        public RaceSelectionGump()
            : base(0, 0)
        {
            this.Closable = false;
            this.Disposable = false;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);
            AddBackground(79, 21, 677, 481, 9200);
            AddAlphaRegion(595, 105, 125, 125);
            AddAlphaRegion(110, 105, 125, 125);
            AddAlphaRegion(265, 105, 125, 125);
            AddAlphaRegion(420, 105, 125, 125);
            AddAlphaRegion(595, 105, 125, 125);
            AddImageTiled(89, 54, 664, 21, 9267);
            AddLabel(121, 36, 0, @"Race Selection Gump");
            AddImage(706, 26, 9005);
            AddLabel(158, 109, 1152, @"Ogre");
            AddLabel(300, 109, 1152, @"Terathan");
            AddLabel(468, 109, 1152, @"Liche");
            AddLabel(145, 208, 1152, @"500,000");
            AddLabel(300, 208, 1152, @"500,000");
            AddLabel(458, 208, 1152, @"500,000");
            AddImage(30, 46, 10400);
            AddImage(29, 212, 10401);
            AddImage(31, 377, 10402);
            AddImage(443, 235, 1418);
            AddItem(153, 146, 8415);
            AddItem(300, 147, 8490);
            AddItem(472, 144, 8440);
            AddItem(626, 140, 8397);
            AddItem(665, 140, 8398);
            AddLabel(638, 109, 1152, @"Human");
            AddLabel(120, 76, 0, @"You may purchase non-human races with special abilities using E.o.C");
            AddAlphaRegion(109, 281, 125, 125);
            AddAlphaRegion(264, 281, 125, 125);
            AddAlphaRegion(419, 281, 125, 125);
            AddLabel(135, 285, 1152, @"Half Daemon");
            AddLabel(454, 283, 1152, @"Sea Djinn");
            AddLabel(290, 285, 1152, @"Shapeshifter");
            AddLabel(140, 380, 1152, @"1,000,000");
            AddLabel(295, 383, 1152, @"1,000,000");
            AddLabel(451, 383, 1152, @"1,000,000");
            AddItem(149, 314, 9737);
            AddItem(446, 307, 8459);
            AddItem(302, 319, 8397, 1304);
            AddLabel(92, 473, 0, @"*Once you hit the (Okay Button) there's no turning back.");

            AddButton(626, 197, 247, 248, 0, GumpButtonType.Reply, 0); // Human

            AddButton(449, 240, 247, 248, 3, GumpButtonType.Reply, 0); // Ogre
            AddButton(295, 240, 247, 248, 2, GumpButtonType.Reply, 0); // Terrathan
            AddButton(140, 240, 247, 248, 1, GumpButtonType.Reply, 0); // Liche

            AddButton(448, 416, 247, 248, 6, GumpButtonType.Reply, 0); // Half-Daemon
            AddButton(294, 416, 247, 248, 5, GumpButtonType.Reply, 0); // Shapeshifter
            AddButton(139, 416, 247, 248, 4, GumpButtonType.Reply, 0); // Elemental
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            ((Player)from).AbilityActive = false;

            from.HueMod = -1;
            from.BodyMod = 0;
            from.CantWalk = false;
            from.Frozen = false;

            switch(info.ButtonID)
            {
                case 0:
				{
                    if (((Player)from).EoC >= 0)
                    {
                        ((Player)from).Race = Race.Human;
                        ((Player)from).RaceBody = 0;
                        ((Player)from).EoC -= 0;
                        ((Player)from).StatCap = 300;
                        from.HueMod = -1;

                        from.Str = 60;
                        from.Dex = 60;
                        from.Int = 60;
                    }

                    else
                    {
                        ((Player)from).SendGump(new RaceSelectionGump((Player)from));
                        ((Player)from).SendMessage("You do not have enough EoC to continue with your selection.");
                    }

					break;
				}

                case 1:
				{
                    if (((Player)from).EoC >= 500000)
                    {
                        ((Player)from).Race = Race.Ogre;
                        ((Player)from).RaceBody = 1;
                        ((Player)from).EoC -= 500000;
                        ((Player)from).StatCap = 500;
                        ((Player)from).BodyDamageBonus = 18;

                        from.Str = 125;
                        from.Dex = 50;
                        from.Int = 50;
                    }

                    else
                    {

                        ((Player)from).SendGump(new RaceSelectionGump((Player)from));
                        ((Player)from).SendMessage("You do not have enough EoC to continue with your selection.");
                    }

					break;
				}

                case 2:
				{
                    if (((Player)from).EoC >= 500000)
                    {
                        ((Player)from).Race = Race.Terathan;
                        ((Player)from).RaceBody = 70;
                        ((Player)from).EoC -= 500000;
                        ((Player)from).StatCap = 500;
                        ((Player)from).BodyDamageBonus = 12;

                        from.Str = 100;
                        from.Dex = 100;
                        from.Int = 75;
                    }

                    else
                    {
                        ((Player)from).SendGump(new RaceSelectionGump((Player)from));
                        ((Player)from).SendMessage("You do not have enough EoC to continue with your selection.");
                    }

					break;
				}

                case 3:
				{
                    if (((Player)from).EoC >= 500000)
                    {
                        ((Player)from).Race = Race.Lich;
                        ((Player)from).RaceBody = 24;
                        ((Player)from).EoC -= 500000;
                        ((Player)from).StatCap = 500;
                        ((Player)from).BodyDamageBonus = 8;

                        from.Str = 75;
                        from.Dex = 50;
                        from.Int = 125;
                    }

                    else
                    {
                        ((Player)from).SendGump(new RaceSelectionGump((Player)from));
                        ((Player)from).SendMessage("You do not have enough EoC to continue with your selection.");
                    }

					break;
				}

                case 4:
                {
                    if (((Player)from).EoC >= 1000000)
                    {
                        ((Player)from).Race = Race.HalfDaemon;
                        ((Player)from).RaceBody = 0;
                        ((Player)from).EoC -= 1000000;
                        ((Player)from).StatCap = 600;       

                        from.Str = 150;
                        from.Dex = 75;
                        from.Int = 75;
                    }

                    else
                    {
                        ((Player)from).SendGump(new RaceSelectionGump((Player)from));
                        ((Player)from).SendMessage("You do not have enough EoC to continue with your selection.");
                    }

                    break;
                }

                case 5:
                {
                    if (((Player)from).EoC >= 1000000)
                    {
                        ((Player)from).Race = Race.Shapeshifter;
                        ((Player)from).RaceBody = 58;
                        ((Player)from).EoC -= 1000000;
                        ((Player)from).StatCap = 600;

                        from.Str = 75;
                        from.Dex = 125;
                        from.Int = 100;
                    }

                    else
                    {
                        ((Player)from).SendGump(new RaceSelectionGump((Player)from));
                        ((Player)from).SendMessage("You do not have enough EoC to continue with your selection.");
                    }
                    break;
                }

                case 6:
                {
                    if (((Player)from).EoC >= 1000000)
                    {
                        ((Player)from).Race = Race.Marid;
                        ((Player)from).RaceBody = 0;
                        ((Player)from).EoC -= 1000000;
                        ((Player)from).StatCap = 600;
                        ((Player)from).BodyDamageBonus = 10;
                        ((Player)from).Hue = 1304;

                        from.Str = 75;
                        from.Dex = 75;
                        from.Int = 150;
                    }

                    else
                    {
                        ((Player)from).SendGump(new RaceSelectionGump((Player)from));
                        ((Player)from).SendMessage("You do not have enough EoC to continue with your selection.");
                    }
                    break;
                }

                default: ((Player)from).SendGump(new RaceSelectionGump((Player)from)); break;
            }

            ((Player)from).AdjustBody();
        }
    }
}
