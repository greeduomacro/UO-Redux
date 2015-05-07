using System;
using Server.Gumps;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an ethereal corpse" )]
	public class EssenceVendor : BaseCreature
	{
		public override bool InitialInnocent { get { return true; } }

		[Constructable]
		public EssenceVendor()
			: base( AIType.AI_Mage, FightMode.Evil, 10, 1, 0.2, 0.4 )
		{
            Name = NameList.RandomName("ethereal warrior") + ",";
            Title = "The hoarder of essence";
			Body = 123;
            Blessed = true;
		}

		public EssenceVendor( Serial serial )
			: base( serial )
		{
		}

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (from.InRange(this.Location, 5))
                return true;

            return base.HandlesOnSpeech(from);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (!e.Handled && e.Mobile.InRange(Location, 3))
            {
                if (e.Speech.ToLower().IndexOf("eidenai amhxania") >= 0)
                {
                    e.Mobile.CloseGump(typeof(EtherealSaleInterface));
                    e.Mobile.SendGump(new EtherealSaleInterface());
                    Direction = GetDirectionTo(e.Mobile.Location);
                }
            }

            base.OnSpeech(e);
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

    public class EtherealSaleInterface : Gump
    {
        public EtherealSaleInterface()
            : base(30, 30)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(61, 12, 372, 351, 9390);
            this.AddImage(101, 58, 1417);
            this.AddImage(101, 147, 1417);
            this.AddImage(101, 237, 1417);
            this.AddLabel(196, 85, 1410, @"Ancient Scroll");
            this.AddItem(122, 87, 3638, 1305);
            this.AddItem(117, 172, 7962);
            this.AddLabel(207, 176, 1310, @"Soul Stone");
            this.AddItem(116, 267, 3628);
            this.AddLabel(199, 267, 1210, @"Daemon Blood");
            this.AddLabel(200, 102, 1000, @"100,000 E.o.C");
            this.AddLabel(200, 191, 1000, @"100,000 E.o.C");
            this.AddLabel(200, 282, 1000, @"100,000 E.o.C");
            this.AddButton(324, 95, 247, 248, (int)Buttons.ancientScrollBtn, GumpButtonType.Reply, 0);
            this.AddButton(323, 186, 247, 248, (int)Buttons.soulStoneBtn, GumpButtonType.Reply, 0);
            this.AddButton(321, 274, 247, 248, (int)Buttons.daemonBloodBtn, GumpButtonType.Reply, 0);

        }

        public enum Buttons
        {
            none,
            ancientScrollBtn,
            soulStoneBtn,
            daemonBloodBtn,
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from is Player)
            {
                Player p = from as Player;

                if (p.EoC < 10000)
                {
                    p.SendMessage("You need atleast 10,000 EoC to purchase this item.");
                    p.CloseGump(typeof(EtherealSaleInterface));
                    p.SendGump(new EtherealSaleInterface()); return;
                }

                else
                {
                    switch (info.ButtonID)
                    {
                        case (int)Buttons.ancientScrollBtn:
                            {

                                p.ConsumeEoC(100000);
                                p.AddToBackpack(new AncientScroll());
                                p.SendMessage("You feel part of your life-force being drained..");
                                break;
                            }

                        case (int)Buttons.daemonBloodBtn:
                            {
                                p.ConsumeEoC(100000);
                                p.AddToBackpack(new DaemonBlood());
                                p.SendMessage("You feel part of your life-force being drained..");
                                break;
                            }

                        case (int)Buttons.soulStoneBtn:
                            {
                                p.ConsumeEoC(100000);
                                p.AddToBackpack(new SoulStone());
                                p.SendMessage("You feel part of your life-force being drained..");
                                break;
                            }

                        default: break;
                    }

                    p.PlaySound(0x1F9);
                }
            }
        }
    }
}