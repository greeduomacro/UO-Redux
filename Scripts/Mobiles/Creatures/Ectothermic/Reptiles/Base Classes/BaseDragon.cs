using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Mobiles.Creatures.Reptiles
{
    public class BaseDragon : BaseReptile
    {
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.CrushingBlow;
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
