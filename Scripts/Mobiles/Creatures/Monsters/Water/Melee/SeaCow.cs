using System;
using Server;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a sea cow corpse" )]
	public class SeaCow : BaseCreature
	{
		[Constructable]
		public SeaCow() : base( AIType.AI_Melee, FightMode.Aggressor, 12, 1, 0.1, 0.2 )
		{
			Name = "a sea cow";
			BodyValue = 231;
			Hue = 1160;
			BaseSoundID = 0x78;
			
			CanSwim = true;
			
			SetStr( 100, 125 );
			SetDex( 50, 75 );
			SetInt( 50, 75 );
			
			SetHits( 65000 );
			
			SetDamage( 5, 10 );
			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Cold, 50 );
			
			SetResistance( ResistanceType.Physical, 100 );
			SetResistance( ResistanceType.Fire, 100 );
			SetResistance( ResistanceType.Cold, 100 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 100 );
			
			for( int i = 0; i < 52; i++ )
				Skills[i].Base = Utility.RandomMinMax( 105, 115 );
			
			VirtualArmor = 175;
			
			Fame = 5000;
			Karma = 0;
		}
		
		public override bool CanRegenHits{ get{ return true; } }
		public override bool InitialInnocent{ get{ return true; } }
		
		public SeaCow( Serial serial ) : base( serial )
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
