using System;

namespace Server.Mobiles
{
	[CorpseName( "the corpse of a sphynx" )]
	public class Sphynx : BaseCreature
	{
		[Constructable]
		public Sphynx() : base( AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.1, 0.3 )
		{
			Name = "a sphynx";
			BodyValue = 788;
			BaseSoundID = 768;
			
			SetStr( 1000, 1200 );
			SetDex( 175, 195 );
			SetInt( 300, 400 );
			
			SetHits( 1000, 1200 );
			
			SetDamage( 18, 22 );
			SetDamageType( ResistanceType.Physical, 85 );
			SetDamageType( ResistanceType.Energy, 15 );
			
			SetResistance( ResistanceType.Physical, 60, 80 );
			SetResistance( ResistanceType.Fire, 30, 50 );
			SetResistance( ResistanceType.Cold, 40, 60 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 40, 50 );
			
			SetSkill( SkillName.Wrestling, 90, 100 );
			SetSkill( SkillName.Tactics, 90, 100 );
			SetSkill( SkillName.Anatomy, 25, 50 );
			SetSkill( SkillName.MagicResist, 100, 150 );
			SetSkill( SkillName.Magery, 95, 100 );
			SetSkill( SkillName.EvalInt, 90, 100 );
			SetSkill( SkillName.Meditation, 95, 120 );
			
			Fame = 15000;
			Karma = 15000;
			
			VirtualArmor = 85;
		}
		
		public override int TreasureMapLevel{ get{ return 6; } }
		public override int Meat{ get{ return 12; } }
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich );
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Gems, Utility.RandomMinMax( 4, 9 ) );
		}
		
		public Sphynx( Serial serial ) : base( serial )
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
