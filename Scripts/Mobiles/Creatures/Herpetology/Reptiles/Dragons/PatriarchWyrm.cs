using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles.Creatures.Reptiles
{
	[CorpseName( "the corpse of a patriarch wyrm" )]
	public class PatriarchWyrm : BaseDragon
	{
		[Constructable]
		public PatriarchWyrm()
			: base( AIType.AI_Mage, FightMode.Aggressor, 14, 1, 0.2, 0.3 )
		{
			Name = "an abnormally large dragon";
			BodyValue = 12;
			Hue = 2213;
			BaseSoundID = 362;

			SetStr( 1150, 1275 );
			SetDex( 200, 245 );
			SetInt( 1000, 1100 );

			SetHits( 2000, 2150 );

			SetDamage( 33, 38 );
			SetDamageType( ResistanceType.Physical, 30 );
			SetDamageType( ResistanceType.Fire, 40 );
			SetDamageType( ResistanceType.Energy, 30 );

			SetResistance( ResistanceType.Physical, 80, 90 );
			SetResistance( ResistanceType.Cold, 75, 80 );
			SetResistance( ResistanceType.Energy, 65, 70 );
			SetResistance( ResistanceType.Fire, 85, 95 );
			SetResistance( ResistanceType.Poison, 65, 75 );

			SetSkill( SkillName.EvalInt, 95, 105 );
			SetSkill( SkillName.Magery, 100, 105 );
			SetSkill( SkillName.Meditation, 70, 85 );
			SetSkill( SkillName.MagicResist, 110, 125 );
			SetSkill( SkillName.Tactics, 100, 110 );
			SetSkill( SkillName.Wrestling, 90, 105 );
			SetSkill( SkillName.Anatomy, 95, 100 );

			Fame = 25000;
			Karma = 25500;

			VirtualArmor = 85;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.SuperBoss, 2 );
			AddLoot( LootPack.UltraRich, 2 );
			AddLoot( LootPack.MedScrolls, 3 );
			AddLoot( LootPack.HighScrolls, 2 );
			AddLoot( LootPack.Gems, Utility.RandomMinMax( 4, 8 ) );
			AddLoot( LootPack.Rich, 2 );
		}

		public override bool HasBreath { get { return true; } }
		public override bool BreathEffectExplodes { get { return true; } }
		public override int BreathFireDamage { get { return 50; } }
		public override int BreathEnergyDamage { get { return 30; } }
		public override int BreathPhysicalDamage { get { return 20; } }
		public override int BreathEffectSpeed { get { return 10; } }

		public override bool CanDestroyObstacles { get { return true; } }
		public override int Meat { get { return 27; } }
		public override int Hides { get { return 31; } }
		public override HideType HideType { get { return HideType.Barbed; } }
		public override bool IsScaryToPets { get { return true; } }
		public override int Scales { get { return 15; } }
		public override ScaleType ScaleType { get { return ScaleType.All; } }
		public override int TreasureMapLevel { get { return 5; } }
		public override Poison PoisonImmune { get { return Poison.Greater; } }
		public override bool BardImmune { get { return true; } }
		public override bool ReacquireOnMovement { get { return true; } }
		public override bool CanAngerOnTame { get { return true; } }

		public void SpawnDragons( Mobile target )
		{
			Map map = this.Map;
			if( map == null )
				return;

			int newDragons = Utility.RandomMinMax( 1, 2 );

			for( int i = 0; i < newDragons; ++i )
			{
				switch( Utility.Random( 3 ) )
				{
					default:
					case 0:
						{
							Dragon dragon = new Dragon();

							dragon.Team = this.Team;
							dragon.FightMode = this.FightMode;

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

							dragon.MoveToWorld( loc, map );
							dragon.Combatant = target;

							break;
						}
					case 1:
						{
							IronDragon idragon = new IronDragon();

							idragon.Team = this.Team;
							idragon.FightMode = this.FightMode;

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

							idragon.MoveToWorld( loc, map );
							idragon.Combatant = target;

							break;
						}
					case 2:
						{
							GreenDiamondDragon gdragon = new GreenDiamondDragon();

							gdragon.Team = this.Team;
							gdragon.FightMode = this.FightMode;

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

							gdragon.MoveToWorld( loc, map );
							gdragon.Combatant = target;

							break;
						}
				}
			}
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			if( 0.05 >= Utility.RandomDouble() )
				SpawnDragons( attacker );
		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			if( 0.05 >= Utility.RandomDouble() )
				SpawnDragons( defender );
		}

		public PatriarchWyrm( Serial serial )
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
