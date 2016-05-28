using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a medium horde daemons corpse" )]
	public class MHordeD : BaseCreature
	{
		[Constructable]
		public MHordeD() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 795;
			Name = "a medium horde daemon";
			Kills = 20;

			SetStr( 170, 200 );
			SetDex( 80, 110 );
			SetInt( 20, 25 );
			SetSkill( SkillName.Wrestling, 60, 80 );
			SetSkill( SkillName.Tactics, 45, 80 );
			SetSkill( SkillName.MagicResist, 46, 70 );

			VirtualArmor = 40;
			SetFameLevel( 3 );
			SetKarmaLevel( 3 );
		}

		public MHordeD( Serial serial ) : base( serial )
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
