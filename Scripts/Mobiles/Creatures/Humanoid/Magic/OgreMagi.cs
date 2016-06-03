//Made By Robert Michaud
using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an Ogre Magi's corpse" )]
	public class OgreMagi : BaseCreature
	{
		[Constructable]
		public OgreMagi() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name =  "ogre magi";
			Body = 766;
			Hue = 657;
			BaseSoundID = 0x3E9;
			
			SetStr( 400, 550 );
			SetDex( 91, 115 );
			SetInt( 96, 175 );
			
			SetHits( 100, 150 );
			
			SetDamage( 15, 25 );
			SetDamageType( ResistanceType.Physical, 100 );
			
			SetResistance( ResistanceType.Physical, 50, 80 );
			SetResistance( ResistanceType.Fire, 50, 70 );
			SetResistance( ResistanceType.Poison, 50, 70 );
			SetResistance( ResistanceType.Energy, 50, 70 );
			
			SetSkill( SkillName.Swords, 90.5, 100.0 );
			SetSkill( SkillName.EvalInt, 75.1, 100.0 );
			SetSkill( SkillName.Magery, 75.1, 100.0 );
			SetSkill( SkillName.MagicResist, 75.0, 97.5 );
			SetSkill( SkillName.Tactics, 100.0, 115.0 );
			SetSkill( SkillName.Wrestling, 80.0, 100.0 );
			SetSkill( SkillName.Necromancy, 80.0,100.0 );
			
			Fame = 2500;
			Karma = -2500;
			
			VirtualArmor = 45;
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.MedScrolls, 2 );
		}
		
		public override bool CanRummageCorpses{ get{ return true; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override int Meat{ get{ return 1; } }
		
		public OgreMagi( Serial serial ) : base( serial )
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
