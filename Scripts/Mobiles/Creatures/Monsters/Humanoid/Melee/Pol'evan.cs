using System;
using Server;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a pol'evan corpse" )]
	public class Polevan : BaseCreature
	{
		[Constructable]
		public Polevan() : base( AIType.AI_Thief, FightMode.Closest, 10, 1, 0.1, 0.3 )
		{
			Name = "a pol'evan";
			BodyValue = 31;
			Hue = 738;
			BaseSoundID = 679;
			
			SetStr( 165, 225 );
			SetDex( 120, 160 );
			SetInt( 50, 85 );
			
			SetHits( 150, 190 );
			
			SetDamage( 7, 11 );
			SetDamageType( ResistanceType.Physical, 85 );
			SetDamageType( ResistanceType.Fire, 15 );
			
			SetResistance( ResistanceType.Physical, 25, 35 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Cold, 30, 40 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 15, 20 );
			
			SetSkill( SkillName.Wrestling, 60, 75 );
			SetSkill( SkillName.Tactics, 65, 80 );
			SetSkill( SkillName.Anatomy, 20, 35 );
			SetSkill( SkillName.MagicResist, 50, 65 );
			
			Fame = 3500;
			Karma = -8000;
			
			VirtualArmor = 25;
		}
		
		public override bool Unprovokable{ get{ return true; } }
		public override int Meat{ get{ return 4; } }
		public override int Hides{ get{ return 7; } }
		public override HideType HideType{ get{ return HideType.Horned; } }
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
		}
		
		public Polevan( Serial serial ) : base( serial )
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
