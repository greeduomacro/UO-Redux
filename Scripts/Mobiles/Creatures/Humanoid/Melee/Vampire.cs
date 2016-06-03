using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a vampire's corpse" )]
	public class Vampire : BaseCreature
	{
		[Constructable]
		public Vampire()
			: base( AIType.AI_Melee, FightMode.Weakest, 10, 1, 0.1, 0.3 )
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
			AddItem( new Cloak( 1 ) );
			AddItem( new Robe( 1 ) );
			AddItem( new LeatherLegs() );
			AddItem( new LeatherChest() );
			AddItem( new LeatherGloves() );
			AddItem( new BoneHarvester() );

			SetStr( 165, 185 );
			SetDex( 155, 170 );
			SetInt( 100, 110 );

			SetHits( 500, 750 );

			SetDamage( 15, 22 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Cold, 50 );
			SetDamageType( ResistanceType.Fire, 0 );
			SetDamageType( ResistanceType.Energy, 0 );
			SetDamageType( ResistanceType.Poison, 0 );

			SetResistance( ResistanceType.Physical, 75 );
			SetResistance( ResistanceType.Fire, 35 );
			SetResistance( ResistanceType.Cold, 65 );
			SetResistance( ResistanceType.Poison, 80 );
			SetResistance( ResistanceType.Energy, 40 );

			SetSkill( SkillName.Wrestling, 90, 100 );
			SetSkill( SkillName.Tactics, 100, 110 );
			SetSkill( SkillName.Swords, 100, 110 );
			SetSkill( SkillName.Anatomy, 95, 105 );
			SetSkill( SkillName.Focus, 100 );

			Fame = 2000;
			Karma = -6500;

			VirtualArmor = 50;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override bool ShowFameTitle { get { return false; } }
		public override bool CanOpenDoors { get { return true; } }
		public override bool CanRummageCorpses { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Greater; } }
		public override bool Uncalmable { get { return true; } }
		public override bool AlwaysMurderer { get { return true; } }

		public override bool IsEnemy( Mobile m )
		{
			//savage kin paint
			if( m.BodyMod == 183 || m.BodyMod == 184 )
				return false;

			return base.IsEnemy( m );
		}

		public void SpawnVampireBats( Mobile target )
		{
			Map map = this.Map;

			if( map == null )
				return;

			int newVampireBats = 3;

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

		public override bool OnBeforeDeath()
		{
			//SpawnVampireBats();

			return base.OnBeforeDeath();
		}

		public Vampire( Serial serial )
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
