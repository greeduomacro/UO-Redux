using System;

using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "the remains of a militia warrior" )]
	public class MilitiaWarrior : BaseCreature
	{
		[Constructable]
		public MilitiaWarrior()
			: base( AIType.AI_Melee, FightMode.Closest, 12, 1, 0.2, 0.4 )
		{
			Body = 0x190;
			Hue = Utility.RandomSkinHue();
			Name = NameList.RandomName( "male" ) + ",";
			Title = "the militia fighter";

			HairItemID = Hair.ShortHair;
			HairHue = Utility.RandomHairHue();

			EquipItems( this );

			SetDex( 95, 115 );
			SetInt( 40, 50 );
			SetStr( 300, 375 );

			SetHits( 250, 325 );

			SetDamage( 15, 20 );

			SetResistance( ResistanceType.Cold, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );
			SetResistance( ResistanceType.Fire, 35, 50 );
			SetResistance( ResistanceType.Physical, 45, 60 );
			SetResistance( ResistanceType.Poison, 10, 25 );

			SetSkill( SkillName.Anatomy, 60.0, 75.0 );
			SetSkill( SkillName.Healing, 85.0, 95.0 );
			SetSkill( SkillName.MagicResist, 85.0, 100.0 );
			SetSkill( SkillName.Parry, 80.0, 100.0 );
			SetSkill( SkillName.Swords, 90.0, 100.0 );
			SetSkill( SkillName.Tactics, 95.0, 105.0 );
			SetSkill( SkillName.Wrestling, 80.0, 95.0 );

			Backpack pack = new Backpack();
			pack.AddItem( new Bandage( Utility.RandomMinMax( 100, 200 ) ) );

			this.AddItem( pack );
		}

		public override bool BardImmune{ get{ return true; } }

		public MilitiaWarrior( Serial serial )
			: base( serial )
		{
		}

		private void EquipItems( BaseCreature bc )
		{
			bc.AddItem( new RingmailChest() );
			bc.AddItem( new RingmailLegs() );
			bc.AddItem( new RingmailArms() );
			bc.AddItem( new RingmailGloves() );
			bc.AddItem( new PlateGorget() );
			bc.AddItem( new LeatherCap() );

			switch( Utility.Random( 4 ) )
			{
				case 0: bc.AddItem( new Halberd() ); break;
				case 1: bc.AddItem( new Longsword() ); break;
				case 2: bc.AddItem( new VikingSword() ); break;
				case 3: bc.AddItem( new Broadsword() ); break;
			}

			if( bc.FindItemOnLayer( Layer.TwoHanded ) == null )
			{
				switch( Utility.Random( 4 ) )
				{
					case 0: bc.AddItem( new BronzeShield() ); break;
					case 1: bc.AddItem( new MetalKiteShield() ); break;
					case 2: bc.AddItem( new HeaterShield() ); break;
					case 3: bc.AddItem( new WoodenKiteShield() ); break;
				}
			}

			switch( Utility.Random( 3 ) )
			{
				case 0:
				case 1: bc.AddItem( new Boots() ); break;
				case 2: bc.AddItem( new ThighBoots() ); break;
			}

			if( Utility.RandomBool() )
				bc.AddItem( new Cloak( Utility.RandomDyedHue() ) );
		}

		public override bool IsEnemy( Mobile m )
		{
			int noto = Server.Misc.NotorietyHandlers.MobileNotoriety( this, m );

			if( noto == Notoriety.Criminal || noto == Notoriety.Murderer || noto == Notoriety.Enemy )
				return true;

			return false;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}