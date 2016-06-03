using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a withered corpse" )]
	public class CrazyMage : BaseCreature
	{
		private static bool m_Talked;
		
		string[] Blabber = new string[]
		{
			"Haha! Thou shalt never have it!",
			"Oh, ho! An adventurer come to steal my treasures!",
			"Foul beast! I throw myself upon thee!",
			"Begone from this cavern, you abominal creature!",
			"Nooo! It is mine, all mine! Never shall ye have it!",
			"Ahahahaha! Time to die!",
			"Thou shalt not escape me this time, ugly one!",
			"Sooo collldd.... and thine blood shall warm the air! Hahaha!",
			"Hah... *weezes*...aha...*coughs and hacks*...ha!",
			"NO! I am not finished here!"
		};
		
		[Constructable]
		public CrazyMage() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "male" ) + ", the insane mage";
			Body = 400;
			Hue = Utility.RandomSkinHue();

			HairItemID = Hair.LongHair;
			FacialHairItemID = Beard.MediumLongBeard;

			HairHue = FacialHairHue = Utility.RandomHairHue();
			
			Robe robe = new Robe();
			robe.Hue = Utility.RandomList( 1157, 1109, 1156, 1175, 1 );
			robe.Name = "a ragged robe";
			AddItem( robe );
			
			WizardsHat hat = new WizardsHat();
			hat.Hue = Utility.RandomMetalHue();
			hat.Name = "a well-used wizard's hat";
			AddItem( hat );
			
			SetStr( 120, 150 );
			SetDex( 80, 100 );
			SetInt( 150, 180 );
			
			SetHits( 200, 300 );
			SetMana( 200, 250 );
			
			SetDamage( 5, 10 );
			
			SetDamageType( ResistanceType.Energy, 100 );
			
			SetResistance( ResistanceType.Physical, 40, 50 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Fire, 25, 35 );
			SetResistance( ResistanceType.Energy, 50, 60 );
			SetResistance( ResistanceType.Poison, 35, 45 );
			
			SetSkill( SkillName.EvalInt, 85, 95 );
			SetSkill( SkillName.Meditation, 75, 80 );
			SetSkill( SkillName.Magery, 95, 100 );
			SetSkill( SkillName.Tactics, 70, 80 );
			SetSkill( SkillName.Wrestling, 60, 75 );
			
			Fame = 3000;
			Karma = -15000;
			
			VirtualArmor = 20;
			
			PackItem( new BagOfReagents( 50 ) );
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.MedScrolls, 4 );
			AddLoot( LootPack.HighScrolls, 3 );
			AddLoot( LootPack.Average, 2 );
		}
		
		public override bool Uncalmable{ get{ return true; } }
		public override bool BardImmune{ get{ return true; } }
		public override bool ShowFameTitle{ get{ return false; } }
		public override bool AlwaysAttackable{ get{ return true; } }
		
		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if( m_Talked == false )
			{
				if( m.InRange( this, 6 ) )
				{
					m_Talked = true;
					SayRandom( Blabber, this );
					this.Move( GetDirectionTo( m.Location ) );
					SpamTimer timer = new SpamTimer();
					timer.Start();
				}
			}
		}
		
		private class SpamTimer : Timer
		{
			public SpamTimer() : base( TimeSpan.FromSeconds( 30 ) )
			{
			}
			
			protected override void OnTick()
			{
				m_Talked = false;

				Stop();
			}
		}
		
		public CrazyMage( Serial serial ) : base( serial )
		{
		}
		
		public static void SayRandom( string[] say, Mobile m )
		{
			m.Say( say[Utility.Random( say.Length )] );
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
