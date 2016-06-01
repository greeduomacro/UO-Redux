using System;
using System.Collections.Generic;
using Server.Engines.PartySystem;
using Server.Guilds;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Spells;

namespace Server.Misc
{
    public class NotorietyHandlers
    {
        public static void Initialize()
        {
            Notoriety.Hues[Notoriety.Innocent] = 0x59;
            Notoriety.Hues[Notoriety.Ally] = 0x3F;
            Notoriety.Hues[Notoriety.CanBeAttacked] = 0x3B2;
            Notoriety.Hues[Notoriety.Criminal] = 0x3B2;
            Notoriety.Hues[Notoriety.Enemy] = 0x90;
            Notoriety.Hues[Notoriety.Murderer] = 0x22;
            Notoriety.Hues[Notoriety.Invulnerable] = 0x35;

            Notoriety.Handler = new NotorietyHandler(MobileNotoriety);

            Mobile.AllowBeneficialHandler = new AllowBeneficialHandler(Mobile_AllowBeneficial);
            Mobile.AllowHarmfulHandler = new AllowHarmfulHandler(Mobile_AllowHarmful);
        }

        private enum GuildStatus { None, Peaceful, Waring }

        private static GuildStatus GetGuildStatus(Mobile m)
        {
            if (m.Guild == null)
                return GuildStatus.None;
            else if (((Guild)m.Guild).Enemies.Count == 0 && m.Guild.Type == GuildType.Regular)
                return GuildStatus.Peaceful;

            return GuildStatus.Waring;
        }

        private static bool CheckBeneficialStatus(GuildStatus from, GuildStatus target)
        {
            if (from == GuildStatus.Waring || target == GuildStatus.Waring)
                return false;

            return true;
        }

        /*private static bool CheckHarmfulStatus( GuildStatus from, GuildStatus target )
        {
            if ( from == GuildStatus.Waring && target == GuildStatus.Waring )
                return true;

            return false;
        }*/

        public static bool Mobile_AllowBeneficial(Mobile from, Mobile target)
        {
            if (from == null || target == null || from.AccessLevel > AccessLevel.Player || target.AccessLevel > AccessLevel.Player)
                return true;

            Map map = from.Map;

            if (map != null && (map.Rules & MapRules.BeneficialRestrictions) == 0)
                return true; // In felucca, anything goes

            if (!from.Player)
                return true; // NPCs have no restrictions

            if (target is BaseCreature && !((BaseCreature)target).Controlled)
                return false; // Players cannot heal uncontrolled mobiles

            Guild fromGuild = from.Guild as Guild;
            Guild targetGuild = target.Guild as Guild;

            if (fromGuild != null && targetGuild != null && (targetGuild == fromGuild || fromGuild.IsAlly(targetGuild)))
                return true; // Guild members can be beneficial

            return CheckBeneficialStatus(GetGuildStatus(from), GetGuildStatus(target));
        }

        public static bool Mobile_AllowHarmful(Mobile from, Mobile target)
        {
            if (from == null || target == null || from.AccessLevel > AccessLevel.Player || target.AccessLevel > AccessLevel.Player)
                return true;

            Map map = from.Map;

            if (map != null && (map.Rules & MapRules.BeneficialRestrictions) == 0)
                return true; // In felucca, anything goes

            Guild fromGuild = GetGuildFor(from.Guild as Guild, from);
            Guild targetGuild = GetGuildFor(target.Guild as Guild, target);

            if (fromGuild != null && targetGuild != null && (fromGuild == targetGuild || fromGuild.IsAlly(targetGuild) || fromGuild.IsEnemy(targetGuild)))
                return true; // Guild allies or enemies can be harmful

            if (target is BaseCreature && (((BaseCreature)target).Controlled || (((BaseCreature)target).Summoned && from != ((BaseCreature)target).SummonMaster)))
                return false; // Cannot harm other controlled mobiles

            if (target.Player)
                return false; // Cannot harm other players

            if (!(target is BaseCreature && ((BaseCreature)target).InitialInnocent))
            {
                if (Notoriety.Compute(from, target) == Notoriety.Innocent)
                    return false; // Cannot harm innocent mobiles
            }

            return true;
        }

        public static Guild GetGuildFor(Guild def, Mobile m)
        {
            Guild g = def;

            BaseCreature c = m as BaseCreature;

            if (c != null && c.Controlled && c.ControlMaster != null)
            {
                c.DisplayGuildTitle = false;

                if (c.Map != Map.Internal && (Core.AOS || Guild.NewGuildSystem || c.ControlOrder == OrderType.Attack || c.ControlOrder == OrderType.Guard))
                    g = (Guild)(c.Guild = c.ControlMaster.Guild);
                else if (c.Map == Map.Internal || c.ControlMaster.Guild == null)
                    g = (Guild)(c.Guild = null);
            }

            return g;
        }

        public static int CorpseNotoriety(Mobile source, Corpse target)
        {
            if (target.AccessLevel > AccessLevel.Player)
                return Notoriety.CanBeAttacked;

            Body body = (Body)target.Amount;

            BaseCreature cretOwner = target.Owner as BaseCreature;

            if (cretOwner != null)
            {
                Guild sourceGuild = GetGuildFor(source.Guild as Guild, source);
                Guild targetGuild = GetGuildFor(target.Guild as Guild, target.Owner);

                if (sourceGuild != null && targetGuild != null)
                {
                    if (sourceGuild == targetGuild || sourceGuild.IsAlly(targetGuild))
                        return Notoriety.Ally;
                    else if (sourceGuild.IsEnemy(targetGuild))
                        return Notoriety.Enemy;
                }

                if (CheckHouseFlag(source, target.Owner, target.Location, target.Map))
                    return Notoriety.CanBeAttacked;

                int actual = Notoriety.CanBeAttacked;

                if (target.Kills >= 5 || (target.Owner is BaseCreature
                    && (((BaseCreature)target.Owner).AlwaysMurderer)))
                    actual = Notoriety.Murderer;

                //if( DateTime.Now >= (target.TimeOfDeath + Corpse.MonsterLootRightSacrifice) )
                //    return actual;

                Party sourceParty = Party.Get(source);

                List<Mobile> list = target.Aggressors;

                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i] == source ||
                        (sourceParty != null && Party.Get(list[i]) == sourceParty))
                        return actual;
                }

                return Notoriety.Innocent;
            }
            else
            {
                if (target.Kills >= 5 ||
                    (target.Owner is BaseCreature && (((BaseCreature)target.Owner).AlwaysMurderer)))
                    return Notoriety.Murderer;

                if (target.Criminal)
                    return Notoriety.Criminal;

                Guild sourceGuild = GetGuildFor(source.Guild as Guild, source);
                Guild targetGuild = GetGuildFor(target.Guild as Guild, target.Owner);

                if (sourceGuild != null && targetGuild != null)
                {
                    if (sourceGuild == targetGuild || sourceGuild.IsAlly(targetGuild))
                        return Notoriety.Ally;
                    else if (sourceGuild.IsEnemy(targetGuild))
                        return Notoriety.Enemy;
                }

                if (target.Owner != null && target.Owner is BaseCreature && ((BaseCreature)target.Owner).AlwaysAttackable)
                    return Notoriety.CanBeAttacked;

                if (CheckHouseFlag(source, target.Owner, target.Location, target.Map))
                    return Notoriety.CanBeAttacked;

                if (!(target.Owner is PlayerMobile) && !IsPet(target.Owner as BaseCreature))
                    return Notoriety.CanBeAttacked;

                List<Mobile> list = target.Aggressors;

                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i] == source)
                        return Notoriety.CanBeAttacked;
                }

                return Notoriety.Innocent;
            }
        }

        public static int MobileNotoriety(Mobile source, Mobile target)
        {
            if (Core.AOS && (target.Blessed || (target is BaseVendor && ((BaseVendor)target).IsInvulnerable) || target is PlayerVendor || target is TownCrier))
                return Notoriety.Invulnerable;

            if (target.Kills >= 5 || (target is BaseCreature && (((BaseCreature)target).AlwaysMurderer)))
                return Notoriety.Murderer;


            if (target is BaseCreature && ((BaseCreature)target).CreatureCharge != BaseCreature.Charge.None)
            {
                BaseCreature bc = target as BaseCreature;

                if (source is Player && bc.Combatant == source)
                    return Notoriety.Enemy;
            }

            if (target.Criminal)
                return Notoriety.Criminal;

            Guild sourceGuild = GetGuildFor(source.Guild as Guild, source);
            Guild targetGuild = GetGuildFor(target.Guild as Guild, target);

            if (sourceGuild != null && targetGuild != null)
            {
                if (sourceGuild == targetGuild || sourceGuild.IsAlly(targetGuild))
                    return Notoriety.Ally;
                else if (sourceGuild.IsEnemy(targetGuild))
                    return Notoriety.Enemy;
            }

            if (SkillHandlers.Stealing.ClassicMode && target is PlayerMobile && ((PlayerMobile)target).PermaFlags.Contains(source))
                return Notoriety.CanBeAttacked;

            if (target is BaseCreature && ((BaseCreature)target).AlwaysAttackable)
                return Notoriety.CanBeAttacked;

            if (CheckHouseFlag(source, target, target.Location, target.Map))
                return Notoriety.CanBeAttacked;

            if (!(target is BaseCreature && ((BaseCreature)target).InitialInnocent))
            {
                if (!target.Body.IsHuman && !target.Body.IsGhost && !IsPet(target as BaseCreature) && !Server.Spells.TransformationSpellHelper.UnderTransformation(target) && !IsTransformed(target))
                    return Notoriety.CanBeAttacked;
            }

            if (CheckAggressor(source.Aggressors, target))
                return Notoriety.CanBeAttacked;

            if (CheckAggressed(source.Aggressed, target))
                return Notoriety.CanBeAttacked;

            if (target is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)target;

                if (bc.Controlled && bc.ControlOrder == OrderType.Guard && bc.ControlTarget == source)
                    return Notoriety.CanBeAttacked;
            }

            if (source is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)source;

                Mobile master = bc.GetMaster();
                if (master != null && CheckAggressor(master.Aggressors, target))
                    return Notoriety.CanBeAttacked;
            }

            return Notoriety.Innocent;
        }

        public static bool IsTransformed(Mobile from)
        {
            if (from.Player && from.BodyMod != 0)
                return true;

            return false;
        }

        public static bool CheckHouseFlag(Mobile from, Mobile m, Point3D p, Map map)
        {
            BaseHouse house = BaseHouse.FindHouseAt(p, map, 16);

            if (house == null || house.Public || !house.IsFriend(from))
                return false;

            if (m != null && house.IsFriend(m))
                return false;

            BaseCreature c = m as BaseCreature;

            if (c != null && !c.Deleted && c.Controlled && c.ControlMaster != null)
                return !house.IsFriend(c.ControlMaster);

            return true;
        }

        public static bool IsPet(BaseCreature c)
        {
            return (c != null && c.Controlled);
        }

        public static bool CheckAggressor(List<AggressorInfo> list, Mobile target)
        {
            for (int i = 0; i < list.Count; ++i)
                if (list[i].Attacker == target)
                    return true;

            return false;
        }

        public static bool CheckAggressed(List<AggressorInfo> list, Mobile target)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                AggressorInfo info = list[i];

                if (!info.CriminalAggression && info.Defender == target)
                    return true;
            }

            return false;
        }
    }
}