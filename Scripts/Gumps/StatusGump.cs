using System;
using Server.Mobiles;
using Server.Gumps;
using Server.Perks;
using Server.SkillCapSelection;

namespace Server.Gumps
{
    public class StatusGump : Gump
    {
        public StatusGump( Player from )
            : base(200, 200)
        {
            this.AddPage(0);
            this.AddBackground(4, 4, 271, 415, 9260);
            this.AddAlphaRegion(16, 134, 246, 82);
            this.AddAlphaRegion(15, 15, 247, 97);
            this.AddLabel(25, 15, 1410, @"" + from.Name);
            this.AddLabel(60, 85, 1149, @"E.o.C");
            this.AddLabel(60, 60, 1149, @"Skills");
            this.AddLabel(60, 35, 1149, @"Race");
            this.AddLabel(165, 35, 1149, @"" + ((Player)from).Race);
            this.AddLabel(165, 60, 1149, @"" + (int)(from.SkillsTotal / 10));
            this.AddLabel(165, 85, 1149, @"" + ((Player)from).EoC);
            this.AddLabel(65, 140, 1149, @"Strength :");
            this.AddLabel(62, 165, 1149, @"Dexterity :");
            this.AddLabel(55, 190, 1149, @"Intelligence :");
            this.AddLabel(150, 140, 1149, @"" + from.Str);
            this.AddLabel(150, 165, 1149, @"" + from.Dex);
            this.AddLabel(150, 190, 1149, @"" + from.Int);
            this.AddButton(190, 142, 5402, 5402, (int)Buttons.strButton, GumpButtonType.Reply, 0);
            this.AddButton(190, 167, 5402, 5402, (int)Buttons.dexButton, GumpButtonType.Reply, 0);
            this.AddButton(190, 192, 5402, 5402, (int)Buttons.intButton, GumpButtonType.Reply, 0);
            this.AddItem(29, 236, 4088);
            this.AddItem(141, 239, 4155);
            this.AddLabel(71, 236, 1149, String.Format("{0} / 20", from.Thirst));
            this.AddLabel(177, 236, 1149, String.Format("{0} / 20", from.Hunger));
            this.AddAlphaRegion(18, 267, 245, 35);
            this.AddLabel(25, 275, 1149, @"Knockout Counts :");
            this.AddLabel(205, 275, 1149, @"" + ((Player)from).KOCount);
            this.AddButton(240, 274, 252, 253, (int)Buttons.koBtn, GumpButtonType.Reply, 0);
            this.AddButton(50, 320, 234, 234, (int)Buttons.perkBtn, GumpButtonType.Reply, 0);
            this.AddButton(175, 320, 229, 229, (int)Buttons.skillsBtn, GumpButtonType.Reply, 0);
            this.AddLabel(46, 385, 1410, @"Perk Table");
            this.AddLabel(168, 385, 1410, @"Skill Levels");
        }

        public enum Buttons
        {
            invalid,
            strButton,
            dexButton,
            intButton,
            koBtn,
            perkBtn,
            skillsBtn,
        }

        public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
        {
            if( sender.Mobile == null || !(sender.Mobile is Player))
                return;
            sender.Mobile.CloseGump(typeof(StatusGump));

            Player p = sender.Mobile as Player;

            if (info.ButtonID == (int)Buttons.strButton)
            {
                if (p.Str >= 125 || (p.RawStatTotal + 1 > p.StatCap))
                {
                    p.SendMessage("You can not increase this attribute any further.");
                    p.SendGump(new StatusGump(p));
                    return;
                }

                if (p.EoC >= 1500)
                {
                    p.Str++;
                    p.EoC -= 1500;
                }

                else p.SendMessage("You must have atleast 1,500 eoc to do this.");
            }

            if (info.ButtonID == (int)Buttons.dexButton)
            {
                if (p.Dex >= 125 || (p.RawStatTotal + 1 > p.StatCap))
                {
                    p.SendMessage("You can not increase this attribute any further.");
                    p.SendGump(new StatusGump(p));
                    return;
                }

                if (p.EoC >= 1500)
                {
                    p.Dex++;
                    p.EoC -= 1500;
                }

                else p.SendMessage("You must have atleast 1,500 eoc to do this.");
            }

            if (info.ButtonID == (int)Buttons.intButton)
            {
                if (p.Int >= 125 || (p.RawStatTotal + 1 > p.StatCap))
                {
                    p.SendMessage("You can not increase this attribute any further.");
                    p.SendGump(new StatusGump(p));
                    return;
                }

                if (p.EoC >= 1500)
                {
                    p.Int++;
                    p.EoC -= 1500;
                }

                else p.SendMessage("You must have atleast 1,500 eoc to do this.");
            }

            if (info.ButtonID == (int)Buttons.koBtn)
            {
                if (p.KOCount <= 0)
                {
                    p.SendMessage("You can not lower you Knockout counts below zero.");
                    p.SendGump(new StatusGump(p));
                    return;
                }

                if (p.EoC >= 1500)
                {
                    p.KOCount--;
                    p.EoC -= 1500;
                }

                else p.SendMessage("You must have atleast 1,500 eoc to do this.");
            }

            if (info.ButtonID == (int)Buttons.perkBtn)
            {
                p.CloseGump(typeof(PerkOverviewGump));
                sender.Mobile.SendGump(new PerkOverviewGump((Player)sender.Mobile));
            }

            if (info.ButtonID == (int)Buttons.skillsBtn)
            {
                p.CloseGump(typeof(SkillSelectionGump));
                sender.Mobile.SendGump(new SkillSelectionGump((Player)sender.Mobile));
            }
        }
    }
}
