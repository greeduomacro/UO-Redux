using System;
using Server;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a slimey corpse" )]
	public class AbyssalSlug : BaseCreature
	{
		[Constructable]
		public AbyssalSlug()
			: base( AIType.AI_Melee, FightMode.Strongest, 10, 1, 0.3, 0.5 )
		{
			Body = 0x100;
			Name = "a giant abyssal slug";

			SetDex( 65, 100 );
			SetInt( 200, 285 );
			SetStr( 750, 950 );

			SetHits( 2800, 3200 );

			SetDamage( 12, 19 );

			SetResistance( ResistanceType.Cold, 15, 30 );
			SetResistance( ResistanceType.Energy, 40, 55 );
			SetResistance( ResistanceType.Fire, 65, 95 );
			SetResistance( ResistanceType.Physical, 60, 70 );
			SetResistance( ResistanceType.Poison, 65, 80 );

			SetSkill( SkillName.MagicResist, 95.0, 105.0 );
			SetSkill( SkillName.Tactics, 80.0, 95.0 );
			SetSkill( SkillName.Wrestling, 85.0, 100.0 );

			Fame = 12000;
			Karma = -20000;
		}

		public AbyssalSlug( Serial serial )
			: base( serial )
		{
		}

		public override bool AlwaysMurderer { get { return true; } }
		public override bool BardImmune { get { return true; } }
		public override int Meat { get { return Utility.RandomMinMax( 12, 24 ); } }
		public override bool IsScaryToPets { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Greater; } }

		public override int GetAngerSound() { return 1394; }
		public override int GetDeathSound() { return 1391; }
		public override int GetHurtSound() { return 1396; }
		public override int GetIdleSound() { return 1393; }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 2 );
			AddLoot( LootPack.Gems, Utility.RandomMinMax( 3, 7 ) );
		}

		public override void OnThink()
		{
			base.OnThink();

			if( Combatant != null && this.InRange( Combatant, 2 ) )
			{
				if( Combatant is BaseCreature && ((BaseCreature)Combatant).Summoned )
				{
					if( 0.05 > Utility.RandomDouble() )
					{
						this.Direction = GetDirectionTo( Combatant );
						this.Say( true, "*engulfs the summoned creature with its entire mouth*" );

						Combatant.Kill();
					}
				}
			}
		}

		public override void AlterSpellDamageFrom( Mobile from, ref int damage )
		{
			if( 0.10 > Utility.RandomDouble() )
			{
				this.Direction = GetDirectionTo( from );
				this.Say( true, "*the slug\'s slimey rolls of blubber seem to absorb most of the spell\'s damage*" );

				damage = ((int)(damage * 0.7));
				this.Hits += damage;
			}
		}

		public override void AlterMeleeDamageFrom( Mobile from, ref int damage )
		{
			if( 0.20 > Utility.RandomDouble() )
			{
				this.Direction = GetDirectionTo( from );
				this.Say( true, "*the mounds of slobber and fat seem to absorb some of the damage*" );

				damage = ((int)(damage * 0.75));
				this.Hits += damage;
			}
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