using System;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.SkillSelection;

namespace Server.Items
{
    public class RaceScroll : Item
    {
        [Constructable]
        public RaceScroll()
            : base(0x1F4F)
        {
            Name = "a mysterious scroll";
            Hue = 1206;
            Movable = false;
        }

        public RaceScroll(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("This must be in your backpack.");
            }
            else if (this != null && from is Player)
            {
                from.SendGump(new RaceSelectionGump((Player)from));
                this.Consume();
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
}