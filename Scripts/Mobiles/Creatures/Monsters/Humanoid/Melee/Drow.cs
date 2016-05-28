using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a drow's corpse" )]
	public class Drow : BaseCreature
	{
		[Constructable]
		public Drow() : base( AIType.AI_Melee, FightMode.Weakest, 10, 1, 0.1, 0.3 )
		{
			Hue = 0x497;
			BaseSoundID = 0;
			BodyValue = Utility.RandomList( 0x190, 0x191 );
			
			if( BodyValue == 0x191 )
			{
				Female = true;
				Name = NameList.RandomName( "drowfemale" );
			}
			else
			{
				Female = false;
				Name = NameList.RandomName( "drowmale" );

				FacialHairItemID = Utility.RandomList( 0x2040, 0x204B, 0x204C, 0x204D );
				FacialHairHue = 1;
			}
			
			HairItemID = Utility.RandomList( 0x203B, 0x203C, 0x2048 );
			HairHue = 1;
			
			AddItem( new Shoes( 1 ) );
			AddItem( new Cloak( 1 ) );
			AddItem( new Robe( 1 ) );
			AddItem( new LeatherLegs() );
			AddItem( new LeatherChest() );
			AddItem( new LeatherGloves() );
			AddItem( new Kryss() );
			
			SetStr( 100, 110 );
			SetDex( 155, 170 );
			SetInt( 125, 140 );
			
			SetHits( 500, 600 );
			
			SetDamage( 15, 22 );

			SetDamageType( ResistanceType.Physical, 80 );

			SetResistance( ResistanceType.Physical, 60 );
			SetResistance( ResistanceType.Poison, 100 );

			SetSkill( SkillName.Wrestling, 90, 100 );
			SetSkill( SkillName.Tactics, 100, 110 );
			SetSkill( SkillName.Fencing, 100, 110 );
			SetSkill( SkillName.Anatomy, 95, 105 );
			SetSkill( SkillName.Focus, 100 );
			
			Fame = 2000;
			Karma = -6500;

			VirtualArmor = 50;
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
		}
		
		public override bool CanOpenDoors{ get{ return true; } }
		public override bool CanRummageCorpses{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override bool Uncalmable{ get{ return true; } }
		
		public Drow( Serial serial ) : base( serial )
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
