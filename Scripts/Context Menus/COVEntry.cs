using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Gumps;

namespace Redux.ContextMenus
{
    public class COVEntry : ContextMenuEntry
    {
        private PlayerMobile pm;

        public COVEntry(PlayerMobile from)
            : base(1063490, -1)
        {
            pm = from;
        }

        public override void OnClick()
        {
            pm.CloseGump(typeof(StatusGump));
            pm.SendGump(new StatusGump(((Player)pm)));
        }
    }
}
