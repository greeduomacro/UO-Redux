using System;
using Server;

namespace Server.Items
{
	public class ZyronicClaw : ExecutionersAxe
	{
		public override int LabelNumber{ get{ return 1061593; } } // Zyronic Claw
		public override int RelicLevel{ get{ return 10; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public ZyronicClaw()
		{
			Hue = 0x485;
			Slayer = SlayerName.ElementalBan;
			WeaponAttributes.HitLeechMana = 50;
			Attributes.AttackChance = 30;
			Attributes.WeaponDamage = 50;
		}

		public ZyronicClaw( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Slayer == SlayerName.None )
				Slayer = SlayerName.ElementalBan;
		}
	}
}