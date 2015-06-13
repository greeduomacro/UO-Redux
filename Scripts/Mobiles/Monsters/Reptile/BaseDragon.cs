using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Mobiles
{
    public class BaseDragon : BaseCreature
    {
        public override void OnCarve(Mobile from, Items.Corpse corpse, Item with)
        {
            base.OnCarve(from, corpse, with);

            if (Utility.RandomDouble() == 0.002)
            {
                corpse.AddItem(new DragonEgg());
                from.SendMessage("Carving into the dragon you notice something pearl white.");
            }  
        }

        public BaseDragon
            (AIType ai, FightMode mode, int iRangePerception, int iRangeFight, double activeSpeed, double passiveSpeed) 
            :   base(ai, mode, iRangePerception, iRangeFight, activeSpeed, passiveSpeed)
        {

        }

        public BaseDragon(Serial serial) : base(serial)
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }
}
