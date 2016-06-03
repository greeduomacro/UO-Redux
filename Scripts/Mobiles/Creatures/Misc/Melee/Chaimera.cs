using System;
using Server;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "the corpse of a chaimera" )]
	public class Chaimera : BaseCreature
	{
		[Constructable]
		public Chaimera() : base( AIType.AI_Melee, FightMode.Strongest, 10, 1, 0.2, 0.4 )
		{
			Name = "a chaimera";
			BodyValue = 251;
			BaseSoundID = 1302;
			
			SetStr( 275, 320 );
			SetDex( 155, 175 );
			SetInt( 75, 105 );
			
			SetHits( 250, 280 );
			
			SetDamage( 9, 14 );
			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 40 );
			SetDamageType( ResistanceType.Energy, 40 );
			
			SetResistance( ResistanceType.Physical, 40, 60 );
			SetResistance( ResistanceType.Fire, 45, 65 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 35, 50 );
			SetResistance( ResistanceType.Energy, 40, 55 );
			
			SetSkill( SkillName.MagicResist, 70, 85 );
			SetSkill( SkillName.Tactics, 80, 90 );
			SetSkill( SkillName.Wrestling, 80, 90 );
			
			Fame = 7500;
			Karma = -7500;
			
			VirtualArmor = 35;
		}
		
		public override int Meat{ get{ return 9; } }
		public override int Hides{ get{ return 13; } }
		public override HideType HideType{ get{ return HideType.Horned; } }
		
		public override bool HasBreath{ get{ return true; } }
		public override int BreathFireDamage{ get{ return 0; } }
		public override int BreathPoisonDamage{ get{ return 100; } }
		public override int BreathEffectHue{ get{ return 2988; } }
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
		}

		public override void AlterMeleeDamageFrom( Mobile from, ref int damage )
		{
			if( from is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)from;

				if( bc.Controlled || bc.BardTarget == this )
					damage /= 2;
			}
		}
		
		public Chaimera( Serial serial ) : base( serial )
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
