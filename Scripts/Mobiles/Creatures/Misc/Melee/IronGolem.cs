using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "an iron golems corpse" )]
	public class IronGolem : BaseCreature
	{
		[Constructable]
		public IronGolem()
			: base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 752;
			Name = "an iron golem";
			BaseSoundID = 268;

			SetStr( 400, 500 );
			SetDex( 66, 80 );
			SetInt( 73, 92 );
			SetSkill( SkillName.Wrestling, 70, 92 );
			SetSkill( SkillName.Tactics, 65, 91 );
			SetSkill( SkillName.MagicResist, 52, 87 );

			ControlSlots = 2;

			VirtualArmor = 80;
			SetFameLevel( 4 );
			SetKarmaLevel( 4 );

			PackGem();

			PackItem( new IronIngot( 50 ) );
		}

		public override int TreasureMapLevel { get { return 1; } }

		public IronGolem( Serial serial )
			: base( serial )
		{
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
}