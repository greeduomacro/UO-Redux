using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Gumps;
using Server.Commands;

namespace Redux.ContextMenus
{
    public class JumpEntry : ContextMenuEntry
    {
        private PlayerMobile pm;

        public JumpEntry(PlayerMobile from)
            : base(1063492, -1)
        {
            pm = from;
        }

        public override void OnClick()
        {
            JumpCommand.BeginJump(pm as Player);
        }
    }
}