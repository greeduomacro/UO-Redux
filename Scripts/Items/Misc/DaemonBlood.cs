using System;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
	public class ConsumableDaemonBlood : Item
	{
		[Constructable]
		public ConsumableDaemonBlood() : this(1)
		{
		}

		[Constructable]
        public ConsumableDaemonBlood(int amount)
            : base(3628)
		{
			Name = "a bottle of viscous red liquid";
			Hue = 0;
            Movable = true;
            LootType = LootType.Blessed;
		}

        public ConsumableDaemonBlood(Serial serial)
            : base(serial)
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
            if (!IsChildOf(from.Backpack))
			{
                from.SendMessage("This must be in your backpack."); return;
			}

            from.SendMessage("You consume the thick liquid; it makes you nauseous.");
            from.Poison = Poison.Regular;

            if (from.Female) from.PlaySound(0x32D);
            else from.PlaySound(0x43F);

            Consume();

            if (from.StatCap >= from.RawStatTotal + 5)
            {
                if (from.RawStr + 5 < 125)
                    from.RawStr += 5;

                else from.RawStr = 125;
            }
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}     
	}
}