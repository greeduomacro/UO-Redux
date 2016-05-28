using System;
using Server;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "the remains of a jumping spider" )]
	public class JumpingSpider : BaseCreature
	{
		private DateTime _nextJump;

		[Constructable]
		public JumpingSpider()
			: base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.1, 0.3 )
		{
			Body = 28;
			BaseSoundID = 0x388;
			Name = "a large jumping spider";

			SetDex( 100, 140 );
			SetInt( 20, 40 );
			SetStr( 95, 150 );

			SetHits( 95, 200 );

			SetDamage( 6, 14 );
			SetDamageType( ResistanceType.Physical, 80 );
			SetDamageType( ResistanceType.Poison, 20 );

			SetResistance( ResistanceType.Physical, 20, 30 );
			SetResistance( ResistanceType.Poison, 35, 55 );

			SetSkill( SkillName.MagicResist, 35.0, 50.0 );
			SetSkill( SkillName.Tactics, 50.0, 60.0 );
			SetSkill( SkillName.Wrestling, 50.0, 65.0 );

			Fame = 850;
			Karma = -600;

			VirtualArmor = 15;

			PackItem( new Server.Items.SpidersSilk( 8 ) );

			if( 0.65 > Utility.RandomDouble() )
				PackItem( new Server.Items.SpiderSac() );

			_nextJump = DateTime.Now;
		}

		public JumpingSpider( Serial serial )
			: base( serial )
		{
		}

		public override PackInstinct PackInstinct { get { return PackInstinct.Arachnid; } }
		public override Poison PoisonImmune { get { return Poison.Regular; } }
		public override Poison HitPoison { get { return Poison.Regular; } }
		public override double HitPoisonChance { get { return 0.20; } }

		public override void OnThink()
		{
			base.OnThink();

			if( this.Combatant != null )
			{
				if( DateTime.Now >= _nextJump && 0.55 > Utility.RandomDouble() )
					Jump();
			}

		}

		public void Jump()
		{
			_nextJump = DateTime.Now + TimeSpan.FromSeconds( 20.0 );

			bool validLocation = false;
			Point3D newLoc = this.Location;

			for( int i = 0; !validLocation && i < 20; i++ )
			{
				int x = this.X + Utility.Random( 8 );
				int y = this.Y + Utility.Random( 8 );
				int z = this.Map.GetAverageZ( x, y );

				if( (validLocation = this.Map.CanFit( x, y, this.Z, 16, false, true )) )
					newLoc = new Point3D( x, y, this.Z );
				else if( (validLocation = this.Map.CanFit( x, y, z, 16, false, true )) )
					newLoc = new Point3D( x, y, z );
			}

			this.MoveToWorld( newLoc, this.Map );

			if( this.Combatant != null )
				this.Direction = GetDirectionTo( this.Combatant );

			Effects.PlaySound( newLoc, this.Map, 0x388 );
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

			_nextJump = DateTime.Now;
		}
	}
}