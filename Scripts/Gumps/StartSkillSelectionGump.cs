using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.SkillSelection
{
    public class StaringSkillSelectionGump : Gump
    {
        public const double MaxIndividualCap = 100.0;

        private Mobile _from;
        private SkillGroup _skillsGroup;

        public StaringSkillSelectionGump(Mobile from)
            : this(from, null)
        {
        }

        public StaringSkillSelectionGump(Mobile from, SkillGroup selected)
            : base(0, 0)
        {
            Closable = false;

            _from = from;
            _skillsGroup = selected;

            AddPage(0);
            AddBackground(10, 10, 325, (205 + (SkillGroup.Groups.Length * 20) + ((selected == null ? 0 : selected.Skills.Length) * 20)), 9250);
            AddLabel(115, 20, 1152, "Starting Skills");

            AddHtml(27, 45, 290, 105, String.Format("On this menu, you can apply some \'free\' skill points to the skills of your choosing. "
                                    + "The first value following the skill name is the skill\'s current base value. The second value is its "
                                    + "overall maximum value. You can continually increase any given skill up to its maximum value or until you "
                                    + "use all available skill points.<BR>Your <U>total</U> cap for all skills combined is {0}, and once you "
                                    + "finish using these free points, you will be able to train the rest of your skills to that maximum value. "
                                    + "At any point during gameplay, you may also exchange skills by setting one to lower and one to raise. "
                                    + "You can do this in the Skills menu, accessible via the \'Skills\' button located on your paperdoll.<BR>If "
                                    + "you have any questions, feel free to contact a member of the staff team for assistance by choosing the "
                                    + "\'Help\' button on your paperdoll.", (from.SkillsCap / 10).ToString("F1")), false, true);
            AddAlphaRegion(27, 45, 290, 105);

            int y = 155;

            for (int i = 0; i < SkillGroup.Groups.Length; i++)
            {
                SkillGroup group = SkillGroup.Groups[i];

                if (group == selected)
                    AddButton(25, (y + 2), 9704, 9705, GetButtonID(0, i), GumpButtonType.Reply, 0);
                else
                    AddButton(25, (y + 2), 9702, 9703, GetButtonID(0, i), GumpButtonType.Reply, 0);

                AddLabel(45, y, 1152, group.Name);

                if (group == selected)
                {
                    for (int j = 0; j < group.Skills.Length; j++)
                    {
                        Skill sk = _from.Skills[group.Skills[j]];

                        if (sk != null)
                        {
                            AddLabel(55, (y + 20), 1152, sk.Name);
                            AddLabel(195, (y + 20), 1152, String.Format("{0} / {1}", sk.Base.ToString("F0"), sk.Cap.ToString("F0")));

                            //increment btn
                            AddButton(265, (y + 25), 2435, 2436, GetButtonID(1, j), GumpButtonType.Reply, 0);
                            //decrement btn
                            AddButton(280, (y + 25), 2437, 2438, GetButtonID(2, j), GumpButtonType.Reply, 0);

                            //max increment btn
                            AddButton(295, (y + 29), 2435, 2436, GetButtonID(3, j), GumpButtonType.Reply, 0);
                            AddButton(295, (y + 23), 2435, 2436, GetButtonID(3, j), GumpButtonType.Reply, 0);

                            //max decrement btn
                            AddButton(310, (y + 29), 2437, 2438, GetButtonID(4, j), GumpButtonType.Reply, 0);
                            AddButton(310, (y + 23), 2437, 2438, GetButtonID(4, j), GumpButtonType.Reply, 0);
                        }

                        y += 20;
                    }
                }

                y += 20;
            }

            AddLabel(30, (y + 20), 1152, "Points Remaining: " + GetRemainingPoints().ToString("F1"));
            AddButton(290, (y + 20), 4023, 4025, GetButtonID(5, 1), GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int buttonID = info.ButtonID - 1;
            int index = buttonID / 10;
            int type = buttonID % 10;

            switch (type)
            {
                case 0:
                    {
                        if (index >= 0 && index < SkillGroup.Groups.Length)
                        {
                            SkillGroup newGroup = SkillGroup.Groups[index];

                            if (_skillsGroup != newGroup)
                                Resend(newGroup);
                            else
                                Resend(null);
                        }

                        break;
                    }
                case 1:
                case 2:
                case 3:
                case 4:
                    {
                        string mode = "inc";

                        if (type == 2)
                            mode = "dec";
                        else if (type == 3)
                            mode = "maxInc";
                        else if (type == 4)
                            mode = "maxDec";

                        if (_skillsGroup != null && (index >= 0 && index < _skillsGroup.Skills.Length))
                        {
                            Skill sk = _from.Skills[_skillsGroup.Skills[index]];

                            if (sk != null)
                            {
                                switch (mode)
                                {
                                    case "inc":
                                        {
                                            if (GetRemainingPoints() <= 0.0)
                                            {
                                                _from.SendMessage("You do not have enough points remaining. Try lowering another skill first.");
                                            }
                                            else if (sk.Base >= 50.0)
                                            {
                                                _from.SendMessage("That skill is at its maximum value.");
                                            }
                                            else
                                            {
                                                sk.Base += 5.0;
                                            }

                                            break;
                                        }
                                    case "dec":
                                        {
                                            if (sk.Base <= 0.0)
                                            {
                                                _from.SendMessage("That skill is at its minimum value. It cannot be lowered further.");
                                            }
                                            else
                                            {
                                                sk.Base -= 5.0;
                                            }

                                            break;
                                        }
                                    case "maxInc":
                                        {
                                            if (IsRaceBonus(_from, sk))
                                            {
                                                sk.Base = sk.Cap;
                                            }
                                            else if (GetRemainingPoints() <= 0.0)
                                            {
                                                _from.SendMessage("You do not have enough points remaining. Try lowering another skill cap first.");
                                            }
                                            else if (sk.Base >= 50.0)
                                            {
                                                _from.SendMessage("That skill is at its maximum value.");
                                            }
                                            else
                                            {
                                                if (GetRemainingPoints() < (50.0 - sk.Base))
                                                    sk.Base = GetRemainingPoints();
                                                else
                                                    sk.Base = 50.0;
                                            }

                                            break;
                                        }
                                    case "maxDec":
                                        {
                                            if (sk.Base <= 0.0)
                                            {
                                                _from.SendMessage("That skill is at its minimum value. It cannot be lowered further.");
                                            }
                                            else
                                            {
                                                sk.Base = 0.0;
                                            }

                                            break;
                                        }
                                }
                            }
                        }

                        Resend(_skillsGroup);

                        break;
                    }
                case 5:
                    {
                        if (GetRemainingPoints() > 0.0)
                            Confirm();
                        else
                            _from.CloseGump(typeof(StaringSkillSelectionGump));

                        break;
                    }
            }
        }

        private int GetButtonID(int type, int index)
        {
            return (1 + (index * 10) + type);
        }

        private void Resend(SkillGroup selection)
        {
            _from.CloseGump(typeof(StaringSkillSelectionGump));
            _from.SendGump(new StaringSkillSelectionGump(_from, selection));
        }

        private double GetRemainingPoints()
        {
            double pointsLeft = 275;

            for (int i = 0; i < _from.Skills.Length; i++)
            {
                if (!IsRaceBonus(_from, _from.Skills[i]))
                    pointsLeft -= _from.Skills[i].Base;
            }

            return pointsLeft;
        }

        private void Confirm()
        {
            _from.CloseGump(typeof(PointsRemainingWarningGump));
            _from.SendGump(new PointsRemainingWarningGump(_from, this, Convert.ToInt32(GetRemainingPoints())));
        }

        private bool IsRaceBonus(Mobile m, Skill skill)
        {
            PlayerMobile pm = m as PlayerMobile;

            //if( pm == null || pm.Race <= 0 )
            //return false;

            if (pm.Skills[skill.SkillID].Cap > 100.0)
                return true;

            return false;
        }
    }

    public class SkillGroup
    {
        private string m_Name;
        private SkillName[] m_Skills;

        public string Name { get { return m_Name; } }
        public SkillName[] Skills { get { return m_Skills; } }

        public SkillGroup(string name, SkillName[] skills)
        {
            m_Name = name;
            m_Skills = skills;

            Array.Sort(m_Skills, new SkillNameComparer());
        }

        private class SkillNameComparer : IComparer
        {
            public SkillNameComparer()
            {
            }

            public int Compare(object x, object y)
            {
                SkillName a = (SkillName)x;
                SkillName b = (SkillName)y;

                string aName = SkillInfo.Table[(int)a].Name;
                string bName = SkillInfo.Table[(int)b].Name;

                return aName.CompareTo(bName);
            }
        }

        private static SkillGroup[] m_Groups = new SkillGroup[]
			{
				new SkillGroup( "Crafting", new SkillName[]
				{
					SkillName.Alchemy,
					SkillName.Blacksmith,
					SkillName.Cartography,
					SkillName.Carpentry,
					SkillName.Cooking,
					SkillName.Fletching,
					SkillName.Inscribe,
					SkillName.Tailoring,
					SkillName.Tinkering
				} ),
				new SkillGroup( "Bardic", new SkillName[]
				{
					SkillName.Discordance,
					SkillName.Musicianship,
					SkillName.Peacemaking,
					SkillName.Provocation
				} ),
				new SkillGroup( "Magical", new SkillName[]
				{
                    SkillName.Chivalry,
					SkillName.EvalInt,
					SkillName.Magery,
					SkillName.MagicResist,
					SkillName.Meditation,
                    SkillName.Necromancy,
                    //SkillName.Ninjitsu,
					SkillName.SpiritSpeak,
				} ),
				new SkillGroup( "Miscellaneous", new SkillName[]
				{
					SkillName.Camping,
					SkillName.Fishing,
					SkillName.Focus,
					SkillName.Healing,
					SkillName.Herding,
					SkillName.Lockpicking,
					SkillName.Lumberjacking,
					SkillName.Mining,
					SkillName.Snooping,
					SkillName.Veterinary
				} ),
				new SkillGroup( "Combat Ratings", new SkillName[]
				{
					SkillName.Archery,
                    //SkillName.Bushido,
					SkillName.Fencing,
					SkillName.Macing,
					SkillName.Parry,
					SkillName.Swords,
					SkillName.Tactics,
					SkillName.Wrestling
				} ),
				new SkillGroup( "Actions", new SkillName[]
				{
					SkillName.AnimalTaming,
					SkillName.Begging,
					SkillName.DetectHidden,
					SkillName.Hiding,
					SkillName.RemoveTrap,
					SkillName.Poisoning,
					SkillName.Stealing,
					SkillName.Stealth,
					SkillName.Tracking
				} ),
				new SkillGroup( "Lore & Knowledge", new SkillName[]
				{
					SkillName.Anatomy,
					SkillName.AnimalLore,
					SkillName.ArmsLore,
					SkillName.Forensics,
					SkillName.ItemID,
					SkillName.TasteID
				} )
			};

        public static SkillGroup[] Groups
        {
            get { return m_Groups; }
        }
    }
}
