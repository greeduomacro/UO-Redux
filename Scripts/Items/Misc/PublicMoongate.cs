using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;

namespace Server.Items
{
    public class PublicMoongate : Item
    {
        [Constructable]
        public PublicMoongate()
            : base(0xF6C)
        {
            Movable = false;
            Light = LightType.Circle300;
        }

        public PublicMoongate(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if(!from.Player)
                return;

            if(from.InRange(GetWorldLocation(), 1))
                UseGate(from);
            else
                from.SendLocalizedMessage(500446); // That is too far away.
        }

        public override bool OnMoveOver(Mobile m)
        {
            return !m.Player || UseGate(m);
        }

        public bool UseGate(Mobile m)
        {
            if(Server.Spells.SpellHelper.CheckCombat(m))
            {
                m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                return false;
            }
            else
            {
                m.CloseGump(typeof(MoongateGump));
                m.SendGump(new MoongateGump(m, this));

                Effects.PlaySound(m.Location, m.Map, 0x20E);
                return true;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class PMEntry
    {
        private Point3D m_Location;
        private int m_Number = 0;
        private string m_Label;

        public Point3D Location
        {
            get
            {
                return m_Location;
            }
        }

        public int Number
        {
            get
            {
                return m_Number;
            }
        }

        public string Label
        {
            get
            {
                return m_Label;
            }
        }

        public PMEntry(Point3D loc, int number)
        {
            m_Location = loc;
            m_Number = number;
        }

        public PMEntry(Point3D loc, string label)
        {
            m_Location = loc;
            m_Label = label;
        }

    }

    public class PMList
    {
        private int m_Number, m_SelNumber;
        private Map m_Map;
        private PMEntry[] m_Entries;

        public int Number
        {
            get
            {
                return m_Number;
            }
        }

        public int SelNumber
        {
            get
            {
                return m_SelNumber;
            }
        }

        public Map Map
        {
            get
            {
                return m_Map;
            }
        }

        public PMEntry[] Entries
        {
            get
            {
                return m_Entries;
            }
        }

        public PMList(int number, int selNumber, Map map, PMEntry[] entries)
        {
            m_Number = number;
            m_SelNumber = selNumber;
            m_Map = map;
            m_Entries = entries;
        }

        public static readonly PMList Felucca =
            new PMList(1012002, 1012014, Map.Felucca, new PMEntry[]
				{
					new PMEntry( new Point3D( 2816, 2252, 0 ), "Noripsni" )
				});

        public static readonly PMList[] SoleList = new PMList[] { Felucca };
    }

    public class MoongateGump : Gump
    {
        private Mobile m_Mobile;
        private Item m_Moongate;
        private PMList[] m_Lists;

        public MoongateGump(Mobile mobile, Item moongate)
            : base(100, 100)
        {
            m_Mobile = mobile;
            m_Moongate = moongate;

            PMList[] checkLists = PMList.SoleList;

            m_Lists = new PMList[checkLists.Length];

            for(int i = 0; i < m_Lists.Length; ++i)
                m_Lists[i] = checkLists[i];

            for(int i = 0; i < m_Lists.Length; ++i)
            {
                if(m_Lists[i].Map == mobile.Map)
                {
                    PMList temp = m_Lists[i];

                    m_Lists[i] = m_Lists[0];
                    m_Lists[0] = temp;

                    break;
                }
            }

            AddPage(0);

            AddBackground(0, 0, 380, 283, 5054);

            AddButton(10, 210, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 210, 140, 25, 1011036, false, false); // OKAY

            AddButton(10, 235, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 235, 140, 25, 1011012, false, false); // CANCEL

            AddHtmlLocalized(5, 5, 200, 20, 1012011, false, false); // Pick your destination:

            for(int i = 0; i < checkLists.Length; ++i)
            {
                AddButton(10, 35 + (i * 25), 2117, 2118, 0, GumpButtonType.Page, Array.IndexOf(m_Lists, checkLists[i]) + 1);
                AddHtmlLocalized(30, 35 + (i * 25), 150, 20, checkLists[i].Number, false, false);
            }

            for(int i = 0; i < m_Lists.Length; ++i)
                RenderPage(i, Array.IndexOf(checkLists, m_Lists[i]));
        }

        private void RenderPage(int index, int offset)
        {
            PMList list = m_Lists[index];

            AddPage(index + 1);

            AddButton(10, 35 + (offset * 25), 2117, 2118, 0, GumpButtonType.Page, index + 1);
            AddHtmlLocalized(30, 35 + (offset * 25), 150, 20, list.SelNumber, false, false);

            PMEntry[] entries = list.Entries;

            for(int i = 0; i < entries.Length; ++i)
            {
                AddRadio(200, 35 + (i * 25), 210, 211, false, (index * 100) + i);
                if(entries[i].Number != 0)
                    AddHtmlLocalized(225, 35 + (i * 25), 150, 20, entries[i].Number, false, false);
                else
                    AddHtml(225, 35 + (i * 25), 150, 20, entries[i].Label, false, false);
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if(info.ButtonID == 0) // Cancel
                return;
            else if(m_Mobile.Deleted || m_Moongate.Deleted || m_Mobile.Map == null)
                return;

            int[] switches = info.Switches;

            if(switches.Length == 0)
                return;

            int switchID = switches[0];
            int listIndex = switchID / 100;
            int listEntry = switchID % 100;

            if(listIndex < 0 || listIndex >= m_Lists.Length)
                return;

            PMList list = m_Lists[listIndex];

            if(listEntry < 0 || listEntry >= list.Entries.Length)
                return;

            PMEntry entry = list.Entries[listEntry];

            if(!m_Mobile.InRange(m_Moongate.GetWorldLocation(), 1) || m_Mobile.Map != m_Moongate.Map)
            {
                m_Mobile.SendLocalizedMessage(1019002); // You are too far away to use the gate.
            }
            else if(Server.Spells.SpellHelper.CheckCombat(m_Mobile))
            {
                m_Mobile.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
            }
            else if(m_Mobile.Map == list.Map && m_Mobile.InRange(entry.Location, 1))
            {
                m_Mobile.SendLocalizedMessage(1019003); // You are already there.
            }
            else
            {
                BaseCreature.TeleportPets(m_Mobile, entry.Location, list.Map);

                m_Mobile.Combatant = null;
                m_Mobile.Warmode = false;
                m_Mobile.Map = list.Map;
                m_Mobile.Location = entry.Location;

                Effects.PlaySound(entry.Location, list.Map, 0x1FE);
            }
        }
    }
}