using System;
using System.Collections;
using Server;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "the corpse of a silver dragon" )]
	public class SilverDragon : BaseCreature
	{
		[Constructable]
		public SilverDragon() : base( AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.175, 0.3 )
		{
			Name = NameList.RandomName("ancient lich") + ",";
			Title = "the silver dragon";
			BodyValue = Utility.RandomList( 12, 49, 59, 60, 61 );
			BaseSoundID = 0x16A;
			Hue = Utility.RandomList( 1150, 1153, 1154 );
			
			SetStr( 750, 900 );
			SetDex( 300, 400 );
			SetInt( 1350, 1500 );
			
			SetHits( 1250, 1350 );
			
			SetDamage( 15, 20 );
			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Cold, 40 );
			SetDamageType( ResistanceType.Energy, 40 );
			
			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 65, 80 );
			SetResistance( ResistanceType.Cold, 75, 85 );
			SetResistance( ResistanceType.Poison, 45, 50 );
			SetResistance( ResistanceType.Energy, 65, 75 );
			
			SetSkill( SkillName.Magery, 85, 95 );
			SetSkill( SkillName.EvalInt, 50, 60 );
			SetSkill( SkillName.Meditation, 35, 45 );
			SetSkill( SkillName.MagicResist, 95, 110 );
			SetSkill( SkillName.Tactics, 50, 65 );
			SetSkill( SkillName.Wrestling, 75, 85 );
			
			Fame = 12500;
			Karma = 15000;
			
			VirtualArmor = 65;

            Tamable = false;
		}
		
		public override bool BardImmune{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 5; } }
		public override int Meat{ get{ return Utility.RandomMinMax( 10, 20 ); } }
		public override int Hides{ get{ return Utility.RandomMinMax( 5, 15 ); } }
		public override HideType HideType{ get{ return Utility.RandomBool() ? HideType.Barbed : HideType.Horned; } }
		public override int Scales{ get{ return Utility.RandomMinMax( 10, 20 ); } }
		public override ScaleType ScaleType { get { return ScaleType.White; } }
		public override bool CanAngerOnTame { get { return true; } }
		
		public override bool HasBreath{ get{ return true; } }
		public override bool BreathEffectExplodes{ get{ return true; } }
		public override int BreathEffectHue{ get{ return 1153; } }
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Rich, 2 );
			AddLoot( LootPack.Gems, 10 );
			AddLoot( LootPack.MedScrolls, 2 );
		}
		
		public SilverDragon( Serial serial ) : base( serial )
		{
		}

		public virtual void BonusAction( PlayerMobile player, Mobile attacker )
		{
			IPooledEnumerable eable = player.GetMobilesInRange( 16 );
			ArrayList list = new ArrayList();

			foreach( Mobile m in eable )
			{
				if( m.Player || this == m || !this.CanBeHarmful( m ) )
					continue;

				list.Add( m );
			}
			
			for( int i = 0; i < list.Count; i++ )
			{
				Mobile m = (Mobile)list[i];
				
				int dmg = i * Utility.RandomMinMax( 10, 25 );
				
				m.Damage( dmg, this );
				
				if( i >= list.Count || i == (list.Count - 1) )
					continue;
				else if( i == 0 )
					Effects.SendMovingEffect( this, m, 0x36F4, 5, 10, false, true, 1154, 0 );
				else
					Effects.SendMovingEffect( m, ((Mobile)list[i + 1]), 0x36F4, 5, 10, false, true, 1154, 0 );
			}
			
			eable.Free();
			list.Clear();
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
