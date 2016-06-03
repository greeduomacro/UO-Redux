using System;
using Server;
using Server.Mobiles;

namespace Server.Mobiles
{
	public class WildBoar : BaseCreature
	{
		[Constructable]
		public WildBoar() : base( AIType.AI_Melee, FightMode.Strongest, 10, 1, 0.2, 0.4 )
		{
			Name = "a wild boar";
			BodyValue = 0x122;
			Hue = 1105;
			BaseSoundID = 0xC4;
			
			SetStr( 60, 80 );
			SetDex( 35, 65 );
			SetInt( 10, 25 );
			
			SetHits( 250, 350 );
			
			SetDamage( 8, 13 );
			SetDamageType( ResistanceType.Physical, 100 );
			
			SetResistance( ResistanceType.Physical, 40, 55 );
			SetResistance( ResistanceType.Fire, 10, 15 );
			SetResistance( ResistanceType.Cold, 15, 25 );
			SetResistance( ResistanceType.Poison, 5, 10 );
			SetResistance( ResistanceType.Energy, 15, 20 );
			
			SetSkill( SkillName.MagicResist, 75, 85 );
			SetSkill( SkillName.Tactics, 85, 95 );
			SetSkill( SkillName.Wrestling, 75, 90 );
			
			Fame = 2750;
			Karma = -1000;
			
			VirtualArmor = 25;
		}
		
		public override int Meat{ get{ return 11; } }
		public override HideType HideType{ get{ return HideType.Regular; } }
		public override int Hides{ get{ return 6; } }
		
		public WildBoar( Serial serial ) : base( serial )
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
