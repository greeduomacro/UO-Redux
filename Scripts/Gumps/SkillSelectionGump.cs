using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.SkillCapSelection {
	public class SkillSelectionGump : Gump {
		public const double MaxIndividualCap = 120.0;

		private Mobile _from;
		private SkillGroup _skillsGroup;

		public SkillSelectionGump( Mobile from )
			: this( from, null ) {
		}

		public SkillSelectionGump( Mobile from, SkillGroup selected )
			: base( 0, 0 ) {

			_from = from;
			_skillsGroup = selected;

			AddPage( 0 );
			AddBackground( 10, 10, 325, (205 + (SkillGroup.Groups.Length * 20) + ((selected == null ? 0 : selected.Skills.Length) * 20)), 9250 );
			AddLabel( 115, 20, 1152, "Skill Cap Overview" );

			AddHtml( 27, 45, 290, 105, String.Format( "By using Essence of Character your may increase your individual skill levels."
                                    + ""
                                    + " Please note: decreasing a skillcap does not refund Essence of Character."
									+ "", (from.SkillsCap / 10).ToString( "F1" ) ), false, true );
			AddAlphaRegion( 27, 45, 290, 105 );

			int y = 155;

			for( int i = 0; i < SkillGroup.Groups.Length; i++ ) {
				SkillGroup group = SkillGroup.Groups[i];

				if( group == selected )
					AddButton( 25, (y + 2), 9704, 9705, GetButtonID( 0, i ), GumpButtonType.Reply, 0 );
				else
					AddButton( 25, (y + 2), 9702, 9703, GetButtonID( 0, i ), GumpButtonType.Reply, 0 );

				AddLabel( 45, y, 1152, group.Name );

				if( group == selected ) {
					for( int j = 0; j < group.Skills.Length; j++ ) {
						Skill sk = _from.Skills[group.Skills[j]];

						if( sk != null ) {
							AddLabel( 55, (y + 20), 1152, sk.Name );
							AddLabel( 195, (y + 20), 1152, String.Format( "{0}", sk.Base.ToString( "F0" ) ) );

							//increment btn
							AddButton( 265, (y + 25), 2435, 2436, GetButtonID( 1, j ), GumpButtonType.Reply, 0 );
							//decrement btn
							//AddButton( 280, (y + 25), 2437, 2438, GetButtonID( 2, j ), GumpButtonType.Reply, 0 );

						}

						y += 20;
					}
				}

				y += 20;
			}

			AddLabel( 30, (y + 20), 1152, "Essence Of Character: " + GetRemainingPoints().ToString( "F1" ) );
			AddButton( 290, (y + 20), 4023, 4025, GetButtonID( 5, 1 ), GumpButtonType.Reply, 0 );
		}

		public override void OnResponse( NetState sender, RelayInfo info ) {
			int buttonID = info.ButtonID - 1;
			int index = buttonID / 10;
			int type = buttonID % 10;

			switch( type ) {
				case 0: {
						if( index >= 0 && index < SkillGroup.Groups.Length ) {
							SkillGroup newGroup = SkillGroup.Groups[index];

							if( _skillsGroup != newGroup )
								Resend( newGroup );
							else
								Resend( null );
						}

						break;
					}
				case 1:
				case 2:
				case 3:
				case 4: {
						string mode = "inc";

						if( type == 2 )
							mode = "dec";
						else if( type == 3 )
							mode = "maxInc";
						else if( type == 4 )
							mode = "maxDec";

						if( _skillsGroup != null && (index >= 0 && index < _skillsGroup.Skills.Length) ) {
							Skill sk = _from.Skills[_skillsGroup.Skills[index]];

							if( sk != null ) 
                            {
								switch( mode ) 
                                {
									case "inc": 
                                        {
                                            if (sk.Base >= sk.Cap)
                                                _from.SendMessage("That skill is at its maximum level. It cannot be raised any further");

                                            else if (((Player)_from).EoC < sk.Base * 10)
                                                _from.SendMessage("You need {0} Essence of Character to increase this skill's level.", sk.Base * 10);

                                            else
                                            {
                                                sk.Base += 1.0;
                                                ((Player)_from).EoC -= (int)(sk.Base * 10);
                                            }
											break;
										}

									case "dec": 
                                        {
                                            if (sk.Base <= 0.0) 
												_from.SendMessage( "That skill is at its minimum level. It cannot be lowered further." );

                                            else 
                                            {
                                                sk.Base -= 1.0;
                                                ((Player)_from).EoC += (int)(sk.Base * 10);
                                                if (sk.Base > sk.Cap)
                                                    sk.Base = sk.Cap;
											}

											break;
										}
								}
							}
						}

						Resend( _skillsGroup );

						break;
					}
				case 5: {
						if( GetRemainingPoints() > 0.0 )
							Confirm();
						else
							_from.CloseGump( typeof( SkillSelectionGump ) );

						break;
					}
			}
		}

		private int GetButtonID( int type, int index ) {
			return (1 + (index * 10) + type);
		}

		private void Resend( SkillGroup selection ) {
			_from.CloseGump( typeof( SkillSelectionGump ) );
			_from.SendGump( new SkillSelectionGump( _from, selection ) );
		}

		private double GetRemainingPoints() 
        {
			double pointsLeft = ((Player)_from).EoC;
			return pointsLeft;
		}

		private void Confirm() {
		}

		private bool IsRaceBonus( Mobile m, Skill skill ) {
			PlayerMobile pm = m as PlayerMobile;


			if( pm.Skills[skill.SkillID].Cap > 100.0 )
				return true;

			return false;
		}
	}

	public class SkillGroup {
		private string m_Name;
		private SkillName[] m_Skills;

		public string Name { get { return m_Name; } }
		public SkillName[] Skills { get { return m_Skills; } }

		public SkillGroup( string name, SkillName[] skills ) {
			m_Name = name;
			m_Skills = skills;

			Array.Sort( m_Skills, new SkillNameComparer() );
		}

		private class SkillNameComparer : IComparer {
			public SkillNameComparer() {
			}

			public int Compare( object x, object y ) {
				SkillName a = (SkillName)x;
				SkillName b = (SkillName)y;

				string aName = SkillInfo.Table[(int)a].Name;
				string bName = SkillInfo.Table[(int)b].Name;

				return aName.CompareTo( bName );
			}
		}

		private static SkillGroup[] m_Groups = new SkillGroup[]
			{
				new SkillGroup( "Crafting", new SkillName[]
				{
					SkillName.Alchemy,
					SkillName.Blacksmith,
					SkillName.Cartography,
					SkillName.Carpentry,
					SkillName.Cooking,
					SkillName.Fletching,
					SkillName.Inscribe,
					SkillName.Tailoring,
					SkillName.Tinkering
				} ),
				new SkillGroup( "Bardic", new SkillName[]
				{
					SkillName.Discordance,
					SkillName.Musicianship,
					SkillName.Peacemaking,
					SkillName.Provocation
				} ),
				new SkillGroup( "Magical", new SkillName[]
				{
					SkillName.EvalInt,
					SkillName.Magery,
					SkillName.MagicResist,
					SkillName.Meditation,
					SkillName.SpiritSpeak,
				} ),
				new SkillGroup( "Miscellaneous", new SkillName[]
				{
					SkillName.Camping,
					SkillName.Fishing,
					SkillName.Focus,
					SkillName.Healing,
					SkillName.Herding,
					SkillName.Lockpicking,
					SkillName.Lumberjacking,
					SkillName.Mining,
					SkillName.Snooping,
					SkillName.Veterinary
				} ),
				new SkillGroup( "Combat Ratings", new SkillName[]
				{
					SkillName.Archery,
					SkillName.Fencing,
					SkillName.Macing,
					SkillName.Parry,
					SkillName.Swords,
					SkillName.Tactics,
					SkillName.Wrestling
				} ),
				new SkillGroup( "Actions", new SkillName[]
				{
					SkillName.AnimalTaming,
					SkillName.Begging,
					SkillName.DetectHidden,
					SkillName.Hiding,
					SkillName.RemoveTrap,
					SkillName.Poisoning,
					SkillName.Stealing,
					SkillName.Stealth,
					SkillName.Tracking
				} ),
				new SkillGroup( "Lore & Knowledge", new SkillName[]
				{
					SkillName.Anatomy,
					SkillName.AnimalLore,
					SkillName.ArmsLore,
					SkillName.Forensics,
					SkillName.ItemID,
					SkillName.TasteID
				} )
			};

		public static SkillGroup[] Groups {
			get { return m_Groups; }
		}
	}
}
