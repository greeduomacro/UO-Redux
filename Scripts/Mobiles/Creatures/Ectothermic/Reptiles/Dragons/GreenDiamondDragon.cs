using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles.Creatures.Reptiles
{
	[CorpseName( "the remains of a green diamond dragon" )]
	public class GreenDiamondDragon : BaseDragon
	{
		[Constructable]
		public GreenDiamondDragon() : base( AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName("ancient lich") + ",";
			Title = "the green diamond dragon";
			BodyValue = Utility.RandomList( 0xC, 0x3C );
			Hue = 2985;
			BaseSoundID = 0x16A;
			
			SetStr( 400, 500 );
			SetDex( 125, 150 );
			SetInt( 1250, 1400 );
			
			SetHits( 1450, 1500 );
			
			SetDamage( 11, 14 );
			SetDamageType( ResistanceType.Physical, 30 );
			SetDamageType( ResistanceType.Energy, 70 );
			
			SetResistance( ResistanceType.Physical, 35, 50 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 25, 35 );
			SetResistance( ResistanceType.Energy, 55, 70 );
			
			SetSkill( SkillName.Magery, 110, 120 );
			SetSkill( SkillName.EvalInt, 75, 85 );
			SetSkill( SkillName.MagicResist, 100, 120 );
			SetSkill( SkillName.Tactics, 35, 50 );
			SetSkill( SkillName.Wrestling, 35, 45 );
			
			Fame = 12500;
			Karma = 1750;
			
			VirtualArmor = 35;
			
			Diamond diamond = new Diamond();
			diamond.ItemID = 0xF29;
			AddToBackpack( diamond );
			
			Diamond diamonds = new Diamond();
			diamonds.Amount = Utility.RandomMinMax( 10, 15 );
			AddToBackpack( diamonds );
		}
		
		public GreenDiamondDragon( Serial serial ) : base( serial )
		{
		}
		
		public override bool BardImmune{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 5; } }
		public override int Meat{ get{ return Utility.RandomMinMax( 10, 20 ); } }
		public override int Scales{ get{ return Utility.RandomMinMax( 10, 20 ); } }
		public override ScaleType ScaleType{ get{ return ScaleType.Green; } }
		
		public override bool HasBreath{ get{ return true; } }
		public override int BreathFireDamage{ get{ return 0; } }
		public override int BreathEnergyDamage{ get{ return 100; } }
		public override int BreathEffectHue{ get{ return 2985; } }
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Gems, 7 );
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			if ( 0.2 > Utility.RandomDouble() && attacker is BaseCreature )
			{
				BaseCreature c = (BaseCreature)attacker;

				if ( c.Controlled && c.ControlMaster != null )
				{
					c.ControlTarget = c.ControlMaster;
					c.ControlOrder = OrderType.Attack;
					c.Combatant = c.ControlMaster;
				}
			}
		}

		public virtual void BonusAction( PlayerMobile player, Mobile attacker )
		{
			if( player != null && player.Backpack != null )
			{
				Diamond diamond = new Diamond();
				diamond.ItemID = 0xF29;
				player.AddToBackpack( diamond );
			}

			if( attacker != null )
				attacker.Poison = Poison.Lethal;
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
