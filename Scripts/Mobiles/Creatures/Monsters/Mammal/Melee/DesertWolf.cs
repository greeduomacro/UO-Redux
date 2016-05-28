using System;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	public class DesertWolf : BaseCreature
	{
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.BleedAttack;
        }

		[Constructable]
		public DesertWolf() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.1, 0.3 )
		{
			Name = "a gaunt wolf";
			BodyValue = 0xE1;
			Hue = 546;
			BaseSoundID = 0xE5;
			
			SetStr( 95, 120 );
			SetDex( 100, 110 );
			SetInt( 35, 50 );
			
			SetHits( 90, 100 );
			
			SetDamage( 12, 16 );
			SetDamageType( ResistanceType.Physical, 100 );
			
			SetResistance( ResistanceType.Physical, 25, 35 );
			SetResistance( ResistanceType.Fire, 25, 35 );
			SetResistance( ResistanceType.Cold, 0, 5 );
			SetResistance( ResistanceType.Poison, 5, 10 );
			SetResistance( ResistanceType.Energy, 5, 15 );
			
			SetSkill( SkillName.MagicResist, 50, 75 );
			SetSkill( SkillName.Tactics, 50, 65 );
			SetSkill( SkillName.Wrestling, 90, 100 );
			SetSkill( SkillName.Anatomy, 20, 35 );
			
			Fame = 1500;
			Karma = -500;
			
			VirtualArmor = 15;
		}
		
		public override int Meat{ get{ return 2; } }
		public override int Hides{ get{ return 3; } }
		
		public DesertWolf( Serial serial ) : base( serial )
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
