using System;
using Server.Items;
using Server.Mobiles;
using Server.Perks;
using Server.Spells;
using Server.Events;

namespace Server.Misc
{
    public class RegenRates
    {
        [CallPriority(10)]
        public static void Configure()
        {
            Mobile.DefaultHitsRate = TimeSpan.FromSeconds( 10.0 );
            Mobile.DefaultStamRate = TimeSpan.FromSeconds( 9.0 );
            Mobile.DefaultManaRate = TimeSpan.FromSeconds( 9.0 );

            Mobile.ManaRegenRateHandler = new RegenRateHandler(Mobile_ManaRegenRate);

            if( Core.AOS )
            {
                Mobile.StamRegenRateHandler = new RegenRateHandler(Mobile_StamRegenRate);
                Mobile.HitsRegenRateHandler = new RegenRateHandler(Mobile_HitsRegenRate);
            }
        }

        private static void CheckBonusSkill( Mobile m, int cur, int max, SkillName skill )
        {
            if( !m.Alive )
                return;

            double n = (double)cur / max;
            double v = Math.Sqrt(m.Skills[skill].Value * 0.005);

            n *= (1.0 - v);
            n += v;

            m.CheckSkill(skill, n);
        }

        private static bool CheckTransform( Mobile m, Type type )
        {
            return TransformationSpellHelper.UnderTransformation(m, type);
        }

        private static TimeSpan Mobile_HitsRegenRate( Mobile from )
        {
            int points = AosAttributes.GetValue(from, AosAttribute.RegenHits);

            if (from is BaseCreature)
            {
                BaseCreature bc = from as BaseCreature;
                if (bc.ControlMaster != null && bc.ControlMaster is Player)
                {
                    Player master = bc.ControlMaster as Player;
                    Beastmaster bmr = Perk.GetByType<Beastmaster>((Player)master);

                    if (bmr != null)
                    {
                        points += bmr.PackRegenBonus();
                    }
                }
            }

            if (from is Player)
            {
                if (((Player)from).Race == Race.HalfDaemon && ((Player)from).AbilityActive == true)
                {
                    points += 16;
                }

                if (from.Hunger != 0 && from.Thirst != 0)
                {
                    int hungerRegen = (int)((from.Hunger + from.Thirst) * 0.085);

                    if (hungerRegen != 0)
                    {
                        points += hungerRegen;
                    }
                }

                Beastmaster bmr = Perk.GetByType<Beastmaster>((Player)from);

                if (bmr != null)
                    points += bmr.BMasterRegenBonus();

                Monk mk = Perk.GetByType<Monk>((Player)from);

                if (mk != null)
                    points += mk.MeditationMastery();

            }

            if (!(from is Player))
            {
                points += (int)(from.Str * 0.03);
            }

            if( points < 0 )
                points = 0;

            return TimeSpan.FromSeconds(1.0 / (0.1 * (1 + points)));
        }

        private static TimeSpan Mobile_StamRegenRate( Mobile from )
        {
            if( from.Skills == null )
                return Mobile.DefaultStamRate;

            CheckBonusSkill(from, from.Stam, from.StamMax, SkillName.Focus);

            int points = (int)(from.Skills[SkillName.Focus].Value * 0.08);

            int cappedPoints = AosAttributes.GetValue(from, AosAttribute.RegenStam);

            points += cappedPoints;

            if( from.Player )
            {
                bool running = ((Player)from).isRunning && from.Mounted == false;

                if (((Player)from).lastMove < DateTime.Now)
                    running = false;

                if (running)
                    points -= 32;

                Beastmaster bmr = Perk.GetByType<Beastmaster>((Player)from);

                if (bmr != null)
                    points += bmr.BMasterRegenBonus();

                Acrobat acr = Perk.GetByType<Acrobat>((Player)from);

                if( acr != null )
                    points += acr.GetStamRegenBonus();

                Adventurer adv = Perk.GetByType<Adventurer>((Player)from);

                if( adv != null )
                    points += adv.GetStamRegenBonus();

                Scout sct = Perk.GetByType<Scout>((Player)from);

                if (sct != null)
                    points += sct.StamRegenBonus();

                Dragoon drg = Perk.GetByType<Dragoon>((Player)from);

                if (drg != null)
                    points += drg.Symbiosis();

                Monk mk = Perk.GetByType<Monk>((Player)from);

                if (mk != null)
                    points += mk.MeditationMastery();
 
                int hungerRegen = (int)((from.Hunger + from.Thirst) * 0.10);

                if (hungerRegen != 0)
                {
                    points += hungerRegen;
                }

                //if (from.Hits != 0 && from.HitsMax != 0)
                //{
                //    double hitsratio = (int)((from.HitsMax / from.Hits) / 3);

                //    if (hitsratio >= 1.0)
                //        points = (int)(points * hitsratio);
                //}
            }

            if(!(from is Player))
            points += (int)(from.Dex * 0.04);

            if (from is BaseCreature)
            {
                BaseCreature bc = from as BaseCreature;

                if (bc.ControlMaster != null && bc.ControlMaster is Player)
                {
                    Player master = bc.ControlMaster as Player;
                    Beastmaster bmr = Perk.GetByType<Beastmaster>((Player)master);

                    if (bmr != null)
                    {
                        points += bmr.PackRegenBonus();
                    }
                }                     
            }

            if (from is BaseMount)
            {
                BaseMount mount = from as BaseMount;
                if (mount.Rider != null && mount.Rider is Player)
                {
                    Player rider = mount.Rider as Player;
                    Dragoon drg = Perk.GetByType<Dragoon>((Player)rider);

                        if(drg != null)
                        {
                            points += drg.Symbiosis();
                        }
                }
            }

            if( points < -1 )
                points = -1;

            return TimeSpan.FromSeconds(1.0 / (0.1 * (2 + points)));
        }

        private static TimeSpan Mobile_ManaRegenRate( Mobile from )
        {
            if( from.Skills == null )
                return Mobile.DefaultManaRate;

            if( !from.Meditating )
                CheckBonusSkill(from, from.Mana, from.ManaMax, SkillName.Meditation);

            double rate;
            double armorPenalty = GetArmorOffset(from);

            if( Core.AOS )
            {
                double medPoints = from.Int + (from.Skills[SkillName.Meditation].Value * 3);

                if (from is Player)
                {
                    if (((Player)from).Race == Race.Marid && ((Player)from).AbilityActive == true)
                    {
                        medPoints += 24;
                    }

                    //Arcanite arc = Perk.GetByType<Arcanite>((Player)from);
                    //if (arc != null)
                    //{
                    //    medPoints += arc.RegenBonus();
                    //}

                    Beastmaster bmr = Perk.GetByType<Beastmaster>((Player)from);

                    if (bmr != null)
                        medPoints += bmr.BMasterRegenBonus();
                }

                if (from is BaseCreature && !from.Player)
                {
                    BaseCreature bc = from as BaseCreature;
                    if (bc.ControlMaster != null && bc.ControlMaster is Player)
                    {
                        Player master = bc.ControlMaster as Player;
                        Beastmaster bmr = Perk.GetByType<Beastmaster>((Player)master);

                        if (bmr != null)
                        {
                            medPoints += bmr.PackRegenBonus();
                        }
                    }
                }

                medPoints += (int)((from.Hunger + from.Thirst) * 0.06 - 1);
                medPoints *= (from.Skills[SkillName.Meditation].Value < 100.0) ? 0.025 : 0.0275;

                CheckBonusSkill(from, from.Mana, from.ManaMax, SkillName.Focus);

                double focusPoints = (from.Skills[SkillName.Focus].Value * 0.05);

                if( armorPenalty > 0 )
                    medPoints = 0; // In AOS, wearing any meditation-blocking armor completely removes meditation bonus

                double totalPoints = focusPoints + medPoints + (from.Meditating ? (medPoints > 13.0 ? 13.0 : medPoints) : 0.0);

                int cappedPoints = AosAttributes.GetValue(from, AosAttribute.RegenMana);

                totalPoints += cappedPoints;

                if( totalPoints < -1 )
                    totalPoints = -1;

                rate = 1.0 / (0.1 * (2 + totalPoints));
            }

            else
            {
                double medPoints = (from.Int + from.Skills[SkillName.Meditation].Value) * 0.5;

                if( medPoints <= 0 )
                    rate = 7.0;
                else if( medPoints <= 100 )
                    rate = 7.0 - (239 * medPoints / 2400) + (19 * medPoints * medPoints / 48000);
                else if( medPoints < 120 )
                    rate = 1.0;
                else
                    rate = 0.75;

                rate += armorPenalty;

                if( from.Meditating )
                    rate *= 0.5;

                if( rate < 0.5 )
                    rate = 0.5;
                else if( rate > 7.0 )
                    rate = 7.0;
            }

            return TimeSpan.FromSeconds(rate);
        }

        public static double GetArmorOffset( Mobile from )
        {
            double rating = 0.0;

            if( !Core.SE )
                rating += GetArmorMeditationValue(from.ShieldArmor as BaseArmor);

            rating += GetArmorMeditationValue(from.NeckArmor as BaseArmor);
            rating += GetArmorMeditationValue(from.HandArmor as BaseArmor);
            rating += GetArmorMeditationValue(from.HeadArmor as BaseArmor);
            rating += GetArmorMeditationValue(from.ArmsArmor as BaseArmor);
            rating += GetArmorMeditationValue(from.LegsArmor as BaseArmor);
            rating += GetArmorMeditationValue(from.ChestArmor as BaseArmor);

            return rating / 4;
        }

        private static double GetArmorMeditationValue( BaseArmor ar )
        {
            if( ar == null || ar.ArmorAttributes.MageArmor != 0 || ar.Attributes.SpellChanneling != 0 )
                return 0.0;

            switch( ar.MeditationAllowance )
            {
                default:
                case ArmorMeditationAllowance.None: return ar.BaseArmorRatingScaled;
                case ArmorMeditationAllowance.Half: return ar.BaseArmorRatingScaled / 2.0;
                case ArmorMeditationAllowance.All: return 0.0;
            }
        }
    }
}