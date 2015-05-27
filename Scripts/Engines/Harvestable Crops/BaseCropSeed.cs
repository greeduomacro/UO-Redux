using System;
using Server;
using Server.Regions;
using Server.Spells;

namespace Server.Engines.Crops
{
	public class BaseCropSeed : BaseCrop
	{
		private int m_SeedlingID;
		private Type m_FullCropType;

		public override double DefaultWeight { get { return 0.002; } }

		public BaseCropSeed( int seedlingID, Type fullCropType )
			: base( 0xF29 )
		{
			m_SeedlingID = seedlingID;
			m_FullCropType = fullCropType;

			Hue = 0x5E2;
			Movable = true;
			Stackable = true;
		}

		public BaseCropSeed( Serial serial )
			: base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( CommonLocs.MustBeInPack );
			}
            else if (from.Region is GuardedRegion)
            {
                from.SendMessage("You may not plant that in this area.");
            }
            else if (SpellHelper.CheckMulti(from.Location, from.Map, true))
            {
                from.SendMessage("You could not dig deep enough to plant the seed.");
            }
            else if (from.Mounted)
            {
                from.SendMessage("You cannot do that while mounted.");
            }
            else if (!CheckPlantSeed(from.Location, from.Map, 0))
            {
                from.SendMessage("You cannot plant this so close to another crop.");
            }
            else if (CheckWeeds(from.Location, from.Map))
            {
                from.SendMessage("These weeds will not help your crop grow!");
            }
            else if (CanGrow(from.Location, from.Map))
            {
                from.Animate(32, 5, 1, true, false, 0);
                from.SendMessage("You have planted the seed.");
                from.ClearHands();

                try
                {
                    CropSeedling seedling = Activator.CreateInstance
                        (typeof(CropSeedling), m_FullCropType, m_SeedlingID) as CropSeedling;

                    BaseCropSeed tempSeed = new BaseCropSeed(0, null);
                    tempSeed.ItemID = 0x913;
                    tempSeed.Movable = false;
                    tempSeed.MoveToWorld(from.Location, from.Map);

                    if (seedling != null)
                        seedling.Plant(from, tempSeed);
                }
                catch (Exception e)
                {
                    Server.Utilities.ExceptionManager.LogException("BaseCropSeed.cs", e);

                    from.SendMessage("A problem has ocurred while planting the seed. This exception has been logged. Please contact an administrator for further assistance.");
                }

                if (--(this.Amount) <= 0)
                    this.Delete();
            }
            else
            {
                from.SendMessage("You cannot plant this seed at this location.");
            }
		}

		public virtual bool CheckPlantSeed( Point3D location, Map map, int range )
		{
			IPooledEnumerable eable = map.GetItemsInRange( location, range );
			int cropCount = 0;

			foreach( Item i in eable )
			{
				if( i is BaseCrop )
					cropCount++;
			}

			eable.Free();

			return (cropCount == 0);
		}

		public virtual bool CheckWeeds( Point3D location, Map map )
		{
			IPooledEnumerable eable = map.GetItemsInRange( location, 1 );

			foreach( Item i in eable )
			{
				if( i is Weeds )
					return true;
			}

			return false;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );

			writer.Write( (int)m_SeedlingID );
			writer.Write( m_FullCropType == null ? null : m_FullCropType.FullName );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_SeedlingID = reader.ReadInt();

			string cropType = reader.ReadString();
			if( cropType != null )
				m_FullCropType = ScriptCompiler.FindTypeByFullName( cropType );

			if( this.ItemID == 0x913 )
				Timer.DelayCall( TimeSpan.FromSeconds( 30.0 ), new TimerCallback( Delete ) );
		}
	}
}