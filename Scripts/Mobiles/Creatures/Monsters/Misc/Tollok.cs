using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	public class Tollok : BaseCreature
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.ParalyzingBlow;
		}

		[Constructable]
		public Tollok() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a tollok";
			BodyValue = 249;
			BaseSoundID = 1261;
			Hue = 743;
			
			SetStr( 800, 900 );
			SetDex( 250, 300 );
			SetInt( 100, 125 );
			
			SetHits( 950, 1100 );
			
			SetDamage( 12, 17 );
			SetDamageType( ResistanceType.Physical, 100 );
			
			SetResistance( ResistanceType.Physical, 75, 85 );
			SetResistance( ResistanceType.Fire, 50, 70 );
			SetResistance( ResistanceType.Cold, 10, 20 );
			SetResistance( ResistanceType.Poison, 30, 50 );
			SetResistance( ResistanceType.Energy, 35, 45 );
			
			SetSkill( SkillName.MagicResist, 80, 90 );
			SetSkill( SkillName.Wrestling, 95, 110 );
			SetSkill( SkillName.Tactics, 100, 120 );
			SetSkill( SkillName.Anatomy, 50, 65 );
			SetSkill( SkillName.Focus, 75, 85 );
			
			Fame = 12500;
			Karma = -10000;
			
			VirtualArmor = 55;
		}
		
		public override double WeaponAbilityChance{ get{ return 0.75; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }
		public override int Hides{ get{ return Utility.RandomMinMax( 12, 18 ); } }
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich );
			AddLoot( LootPack.Rich );
		}
		
		public Tollok( Serial serial ) : base( serial )
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
