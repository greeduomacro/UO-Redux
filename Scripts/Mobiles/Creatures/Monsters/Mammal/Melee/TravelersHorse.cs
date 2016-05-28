using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a horse corpse" )]
	public class TravelersHorse : BaseMount
	{
		[Constructable]
		public TravelersHorse() : this( "a traveler's horse" )
		{
		}
		
		[Constructable]
		public TravelersHorse( string name ) : base( name, 0xE2, 0x3EA0, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			SetStr( 22, 98  );
			SetDex( 56, 75 );
			SetInt( 6, 10 );
			
			SetHits( 28, 45 );
			SetMana( 0 );
			
			SetDamage( 3, 4 );
			
			SetDamageType( ResistanceType.Physical, 100 );
			
			SetResistance( ResistanceType.Physical, 15, 20 );
			
			SetSkill( SkillName.MagicResist, 25.1, 30.0 );
			SetSkill( SkillName.Tactics, 29.3, 44.0 );
			SetSkill( SkillName.Wrestling, 29.3, 44.0 );
			
			Fame = 300;
			Karma = 300;
			
			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 29.1;
			
			Container pack = Backpack;
			
			if ( pack != null )
				pack.Delete();
			
			pack = new StrongBackpack();
			pack.Movable = false;
			
			AddItem( pack );
		}
		
		public override int GetAngerSound()
		{
			return 0xA8;
		}
		
		public override int GetIdleSound()
		{
			return 0xA8;
		}
		
		public override int GetAttackSound()
		{
			return 0xA8;
		}
		
		public override int GetHurtSound()
		{
			return 0xA8;
		}
		
		public override int GetDeathSound()
		{
			return 0xA8;
		}
		
		public override int Meat{ get{ return 6; } }
		public override int Hides{ get{ return 10; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }
		
		public TravelersHorse( Serial serial ) : base( serial )
		{
		}
		
		#region Pack Animal Methods
		public override bool OnBeforeDeath()
		{
			if ( !base.OnBeforeDeath() )
				return false;
			
			PackAnimal.CombineBackpacks( this );
			
			return true;
		}
		
		public override bool IsSnoop( Mobile from )
		{
			if ( PackAnimal.CheckAccess( this, from ) )
				return false;
			
			return base.IsSnoop( from );
		}
		
		public override bool OnDragDrop( Mobile from, Item item )
		{
			if ( CheckFeed( from, item ) )
				return true;
			
			if ( PackAnimal.CheckAccess( this, from ) )
			{
				AddToBackpack( item );
				return true;
			}
			
			return base.OnDragDrop( from, item );
		}
		
		public override bool CheckNonlocalDrop( Mobile from, Item item, Item target )
		{
			return PackAnimal.CheckAccess( this, from );
		}
		
		public override bool CheckNonlocalLift( Mobile from, Item item )
		{
			return PackAnimal.CheckAccess( this, from );
		}
		
		public override void GetContextMenuEntries( Mobile from, System.Collections.Generic.List<Server.ContextMenus.ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
			
			PackAnimal.GetContextMenuEntries( this, from, list );
		}
		#endregion
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
		}
	}
}
