using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a vampire's corpse" )]
	public class VampireLord : BaseCreature
	{
		[Constructable]
		public VampireLord()
			: base( AIType.AI_Mage, FightMode.Weakest, 10, 1, 0.1, 0.3 )
		{
			Hue = 0;
			BaseSoundID = 0;
			BodyValue = Utility.RandomList( 0x190, 0x191 );

			if( BodyValue == 0x191 )
			{
				Female = true;
				Name = NameList.RandomName( "female" );
			}
			else
			{
				Female = false;
				Name = NameList.RandomName( "male" );

				FacialHairItemID = Utility.RandomList( 0x2040, 0x204B, 0x204C, 0x204D );
				FacialHairHue = 1;
			}

			HairItemID = Utility.RandomList( 0x203B, 0x203C, 0x2048 );
			HairHue = 1;

			AddItem( new Shoes( 1 ) );
			AddItem( new Robe( 1 ) );
			AddItem( new LeatherLegs() );
			AddItem( new LeatherChest() );
			AddItem( new LeatherGloves() );

			SetStr( 750, 850 );
			SetDex( 180, 190 );
			SetInt( 400, 450 );

			SetHits( 500, 600 );

			SetDamage( 15, 20 );

			SetDamageType( ResistanceType.Physical, 35 );
			SetDamageType( ResistanceType.Cold, 50 );
			SetDamageType( ResistanceType.Fire, 0 );
			SetDamageType( ResistanceType.Energy, 15 );
			SetDamageType( ResistanceType.Poison, 0 );

			SetResistance( ResistanceType.Physical, 65, 70 );
			SetResistance( ResistanceType.Fire, 45, 50 );
			SetResistance( ResistanceType.Cold, 60, 75 );
			SetResistance( ResistanceType.Poison, 80, 95 );
			SetResistance( ResistanceType.Energy, 50, 60 );

			SetSkill( SkillName.Magery, 110, 115 );
			SetSkill( SkillName.EvalInt, 100, 110 );
			SetSkill( SkillName.Meditation, 100 );
			SetSkill( SkillName.Necromancy, 100 );
			SetSkill( SkillName.SpiritSpeak, 100 );
			SetSkill( SkillName.Tactics, 100, 110 );
			SetSkill( SkillName.Anatomy, 100, 110 );
			SetSkill( SkillName.Wrestling, 100, 105 );

			Fame = 15000;
			Karma = -3000;

			VirtualArmor = 25;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Average );
			AddLoot( LootPack.MedScrolls, 2 );
			AddLoot( LootPack.Potions );
		}

		public override bool ShowFameTitle { get { return true; } }
		public override bool CanOpenDoors { get { return true; } }
		public override bool CanRummageCorpses { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Deadly; } }
		public override bool BardImmune { get { return true; } }
		public override bool AutoDispel { get { return true; } }
		public override bool AlwaysMurderer { get { return true; } }

		public void Morph()
		{
			Name = "a vampire bat";
			Hue = 1175;
			BodyValue = 0x13D;
			BaseSoundID = 0x270;

			RawStr = 200;
			RawInt = 350;
			RawDex = 130;

			int i = 0;

			while( i < 52 )
			{
				Skills[i].Base = 100;
				i++;
			}
		}

		public void SpawnVampires( Mobile target )
		{
			Map map = this.Map;

			if( map == null )
				return;

			int newVampires = 2;

			for( int i = 0; i < newVampires; ++i )
			{
				Vampire vamp = new Vampire();

				vamp.Team = this.Team;
				vamp.FightMode = FightMode.Closest;

				bool validLocation = false;
				Point3D loc = this.Location;

				for( int j = 0; !validLocation && j < 10; ++j )
				{
					int x = X + Utility.Random( 3 ) - 1;
					int y = Y + Utility.Random( 3 ) - 1;
					int z = map.GetAverageZ( x, y );

					if( validLocation = map.CanFit( x, y, this.Z, 16, false, false ) )
						loc = new Point3D( x, y, Z );
					else if( validLocation = map.CanFit( x, y, z, 16, false, false ) )
						loc = new Point3D( x, y, z );
				}

				vamp.MoveToWorld( loc, map );
				vamp.Combatant = target;
			}
		}

		public override bool OnBeforeDeath()
		{
			Mobile m = this;

			SpawnVampires( m.Combatant );

			Effects.SendLocationEffect( new Point3D( this.X, this.Y, this.Z ), this.Map, 0x3728, 13 );
			Morph();

			return base.OnBeforeDeath();
		}

		public void SpawnVampireBats( Mobile target )
		{
			Map map = this.Map;

			if( map == null )
				return;

			int newVampireBats = Utility.RandomMinMax( 1, 2 );

			for( int i = 0; i < newVampireBats; ++i )
			{
				VampireBat vb = new VampireBat();

				vb.Team = this.Team;
				vb.FightMode = FightMode.Closest;

				bool validLocation = false;
				Point3D loc = this.Location;

				for( int j = 0; !validLocation && j < 10; ++j )
				{
					int x = X + Utility.Random( 3 ) - 1;
					int y = Y + Utility.Random( 3 ) - 1;
					int z = map.GetAverageZ( x, y );

					if( validLocation = map.CanFit( x, y, this.Z, 16, false, false ) )
						loc = new Point3D( x, y, Z );
					else if( validLocation = map.CanFit( x, y, z, 16, false, false ) )
						loc = new Point3D( x, y, z );
				}

				vb.MoveToWorld( loc, map );
				vb.Combatant = target;
			}
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			if( 0.1 >= Utility.RandomDouble() )
				SpawnVampireBats( attacker );
		}

		public VampireLord( Serial serial )
			: base( serial )
		{
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
