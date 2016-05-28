using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a large horde daemons corpse" )]
	public class LHordeD : BaseCreature
	{
		[Constructable]
		public LHordeD() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 796;
			Name = "a large horde daemon";
			Kills=20;

			SetStr( 170, 200 );
			SetDex( 80, 110 );
			SetInt( 20, 25 );
			SetSkill( SkillName.Wrestling, 80, 90 );
			SetSkill( SkillName.Tactics, 75, 90 );
			SetSkill( SkillName.MagicResist,66, 90 );

			VirtualArmor = 70;
			SetFameLevel( 5 );
			SetKarmaLevel( 5 );
		}

		public LHordeD( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
