using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a fading corpse" )]
	public class RuinedWizard : BaseCreature
	{
		[Constructable]
		public RuinedWizard() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "BacktrolMale" ) + ", the ruined wizard";
			BodyValue = 985;
			BaseSoundID = 0;
			
			SetStr( 400, 500 );
			SetDex( 145, 165 );
			SetInt( 550, 650 );
			
			SetHits( 250, 300 );
			
			SetDamage( 8, 10 );
			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Fire, 20 );
			SetDamageType( ResistanceType.Cold, 20 );
			SetDamageType( ResistanceType.Energy, 60 );
			
			SetResistance( ResistanceType.Physical, 40, 50 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 40, 50 );
			
			SetSkill( SkillName.Meditation, 65, 75 );
			SetSkill( SkillName.EvalInt, 90, 105 );
			SetSkill( SkillName.Magery, 85, 100 );
			SetSkill( SkillName.MagicResist, 100, 120 );
			SetSkill( SkillName.Tactics, 50, 70 );
			SetSkill( SkillName.Wrestling, 60, 75 );
			
			Fame = 12000;
			Karma = -12000;
			
			VirtualArmor = 35;
			
			Spellbook book = new Spellbook();
			book.Content = 74813425443667967;
			book.LootType = LootType.Cursed;
			PackItem( book );
		}
		
		public override int TreasureMapLevel{ get{ return 3; } }
		public override bool BardImmune{ get{ return true; } }
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Average );
		}
		
		public RuinedWizard( Serial serial ) : base( serial )
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
