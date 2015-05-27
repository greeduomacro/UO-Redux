using System;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Perks;

namespace Server.Misc
{
	public class FoodDecayTimer : Timer
	{

		public static void Initialize()
		{
			new FoodDecayTimer().Start();
		}

        //public TimeSpan minDecay = TimeSpan.FromMinutes(Utility.RandomMinMax(5, 10));

		public FoodDecayTimer()
			: base( TimeSpan.FromMinutes( 10.0 ), TimeSpan.FromMinutes( 15.0 ) )
		{    
			Priority = TimerPriority.OneMinute;
		}

		protected override void OnTick()
		{
            try
            {
                FoodDecay();
            }
            
            catch {}
		}

		public static void FoodDecay()
		{
			foreach( NetState state in NetState.Instances )
			{
                if (state.Mobile.AccessLevel == AccessLevel.Player)
                {
                    if (state.Mobile is Player)
                    {
                        if (((Player)state.Mobile).Race != Race.Lich)
                        {
                            if (((Player)state.Mobile).Race != Race.Marid)
                                HungerDecay(state.Mobile);

                            ThirstDecay(state.Mobile);
                        }
                    }
                }
			}
		}

		public static void HungerDecay( Mobile m )
		{         
            if ( m != null && m is Player) 
            {
                Adventurer adv = Perk.GetByType<Adventurer>((Player)m);

                if (adv != null) 

                    if (Utility.RandomDouble() < 0.50 && m.Hunger < 20)
                    {
                        m.Hunger += adv.HungerThirstBonus();
                    }                                  
            }

            if ((m != null && m.Alive && m.Hunger >= 1 && m.AccessLevel == AccessLevel.Player) &&
                ((Player)m).Race != Race.Lich && ((Player)m).Race != Race.Marid)
                m.Hunger --;

			if( m != null && m.Alive )
			{
				if( m.Hunger > 14 )
					return;

				else if( m.Hunger >= 10 && m.Hunger <= 14 )
					m.SendMessage( "You feel somewhat hungry." );

				else if( m.Hunger >= 5 && m.Hunger < 10 )
					m.SendMessage( "You feel quite hungry." );

				else if( m.Hunger > 0 && m.Hunger < 5 )
					m.SendMessage( "You are very hungry, and should find something to eat." );

				else
				{
					m.SendMessage( "Your lack of food is wasting away at your body!" );
					m.Damage( Utility.RandomMinMax(25, 45 ) );
                    m.Stam -= Utility.RandomMinMax(20, 30);
                    m.Mana -= Utility.RandomMinMax(20, 30);
				}

				if( m.Hunger < 10 )
				{
					m.PublicOverheadMessage( MessageType.Regular, m.EmoteHue, true, String.Format( "*{0} stomach growls softly*", m.Female ? "her" : "his" ) );

					if( m.Hidden )
						m.RevealingAction();
				}
			}
		}

		public static void ThirstDecay( Mobile m )
		{
            if (m != null && m is Player)
            {
                Adventurer adv = Perk.GetByType<Adventurer>((Player)m);

                if (adv != null)
                {
                    if (Utility.RandomDouble() < 0.50 && m.Thirst < 20)
                    {
                        m.Thirst += adv.HungerThirstBonus();
                    }
                }
            }

            if ((m != null && m.Alive && m.Hunger >= 1 && m.AccessLevel == AccessLevel.Player) &&
                ((Player)m).Race != Race.Lich)
				m.Thirst --;

            if (((Player)m).Race == Race.Marid)
                m.Thirst--;

			if( m != null && m.Alive )
			{
				if( m.Thirst > 14 )
					return;

				else if( m.Thirst >= 10 && m.Thirst <= 14 )
					m.SendMessage( "You feel somewhat parched." );

				else if( m.Thirst >= 5 && m.Thirst < 10 )
					m.SendMessage( "Your throat feels quite dry." );

				else if( m.Thirst > 0 && m.Thirst < 5 )
					m.SendMessage( "You feel extremely thirsty, and should find something to drink." );

				else
				{
					m.SendMessage( "Your lack of drink is wasting away at your body!" );
                    m.Damage(Utility.RandomMinMax(35, 55));
                    m.Stam -= Utility.RandomMinMax(40, 60);
                    m.Mana -= Utility.RandomMinMax(30, 40);
				}
			}
		}
	}
}
