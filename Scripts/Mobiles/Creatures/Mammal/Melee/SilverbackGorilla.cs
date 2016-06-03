using System;
using Server;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a large gorilla corpse" )]
	public class SilverbackGorilla : BaseCreature
	{
		[Constructable]
		public SilverbackGorilla() : base( AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a large, silverback gorilla";
			BodyValue = 29;
			BaseSoundID = 0x9E;
			
			SetStr( 90, 125 );
			SetDex( 60, 95 );
			SetInt( 40, 75 );
			
			SetHits( 170, 210 );
			
			SetDamage( 8, 13 );
			
			SetDamageType( ResistanceType.Physical, 100 );
			
			SetResistance( ResistanceType.Physical, 50, 65 );
			SetResistance( ResistanceType.Energy, 30, 40 );
			SetResistance( ResistanceType.Cold, 35, 45 );
			SetResistance( ResistanceType.Fire, 25, 30 );
			SetResistance( ResistanceType.Poison, 30, 35 );
			
			SetSkill( SkillName.Anatomy, 60, 75 );
			SetSkill( SkillName.Wrestling, 80, 90 );
			SetSkill( SkillName.MagicResist, 40, 50 );
			SetSkill( SkillName.Tactics, 50, 65 );
			
			Fame = 2500;
			Karma = 0;
			
			VirtualArmor = 25;
			
			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 65.8;
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
		}
		
		public override int Meat{ get{ return 6; } }
		public override int Hides{ get{ return 14; } }
		public override HideType HideType{ get{ return HideType.Regular; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies; } }
		
		public SilverbackGorilla( Serial serial ) : base( serial )
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
