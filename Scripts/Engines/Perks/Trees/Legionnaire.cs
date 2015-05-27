using Server.Mobiles;
using Server;
using Server.Items;

namespace Server.Perks
{
    public class Legionnaire : Perk
    {
        public bool ShieldEquipped
        {
            get
            {
                return Player != null && (Player.FindItemOnLayer(Layer.TwoHanded) is BaseShield);
            }
        }

        /// <summary>
        /// ctor
        /// </summary>
        public Legionnaire( Player player )
            : base(player)
        {
        }       
        
        public bool SpearConverted = false;

        public bool Hoplite()
        {
            return (Level > PerkLevel.Fourth);
        }

        /// <summary>
        /// Applies a 20% bonus to parry at <code>PerkLevel.Fifth</code>
        /// </summary>
        public double GetParryChanceBonus()
        {
            if( Level < PerkLevel.Fifth || !ShieldEquipped )
                return 0;

            return 0.20;
        }

        /// <summary>
        /// Lowers the damage to shields at <code>PerkLevel.Second</code>
        /// </summary>
        public int GetShieldWearBonus( int wear )
        {
            if( Level < PerkLevel.Second || !ShieldEquipped )
                return wear;

            return 2;
        }

        /// <summary>
        /// Applies a 20% chance to bash the attacker at <code>PerkLevel.Fourth</code>
        /// </summary>
        public void TryBash( BaseShield shield, BaseWeapon attackerWeapon )
        {
            if( Level < PerkLevel.Fourth || !ShieldEquipped || attackerWeapon is BaseRanged )
               return;

            if( Utility.RandomDouble() < 0.20 )
                return;

            Mobile aggressor = Player.Combatant as Mobile;

            if( aggressor == null || !Player.CanBeHarmful(aggressor, false) )
                return;

            int damage = (int)((shield.ArmorRating / 2.0) + shield.Weight);
            damage += (int)(Player.Str * 0.05);

            aggressor.Damage(damage, Player);

            aggressor.FixedEffect(0x37B9, 10, 16);
            aggressor.PlaySound(0x141);

            Player.SendMessage("You strike your opponent with your shield!");
            Player.Emote("*Sheild-strike*");
        }

        /// <summary>
        /// Applies a 15% chance to dodge weapon attacks at <code>PerkLevel.Third</code>
        /// </summary>
        public bool TryDodge( BaseWeapon weapon )
        {
            if (Level >= PerkLevel.Second && ShieldEquipped)
            {
                if (Utility.RandomDouble() < 0.15)
                {
                    Player.SendMessage("You side-step your opponents attack, leading them through with your shield.");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Applies a 66% chance to dodge missile attacks at <code>PerkLevel.First</code>
        /// </summary>
        public bool TryDodgeMissile()
        {
            if( Level < PerkLevel.First || !ShieldEquipped )
                return false;

            if (Utility.RandomDouble() < 0.66)
            {
                Player.Emote("*Deflects the projectile.*");
                Player.SendMessage("You have deflected a projectile!");
                Player.FixedEffect(0x37B9, 10, 16);
                return true;
            }

            else return false;
        }

        /// <summary>
        /// Serialization
        /// </summary>
        protected override void Serialize( GenericWriter writer )
        {
            base.Serialize(writer);

            writer.Write((int)1); //version
            writer.Write(SpearConverted);
        }

        /// <summary>
        /// Deserialization
        /// </summary>
        public Legionnaire( GenericReader reader )
            : base(reader)
        {
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    break;

                case 1:
                    SpearConverted = reader.ReadBool();
                    goto case 0;
            }
        }

        public override string Description { get { return "A master of defensive tactics and shielded combat."; } }
        public override int GumpID { get { return 2254; } }
        public override string Label { get { return "Legionnaire"; } }

        public override LabelEntryList LabelEntries
        {
            get
            {
                return new LabelEntryList(new LabelEntry[]
                {
                    new LabelEntry(PerkLevel.First, "Deflect Projectile", "Well-versed in the ways of the shield, you have become better at deflecting projectiles, such as arrows, bolts, and thrown knives."),
                    new LabelEntry(PerkLevel.Second, "Disperse Momentum", "After breaking many shields, you have become conscientious of the damage your shield takes, and have developed strategies to divert the momentum of incoming attacks to reduce impact damage."),
                    new LabelEntry(PerkLevel.Third, "Sidestep", "Like the cape-wielding Matador, the Legionnaire is capable of side-stepping an opponents attack."),
                    new LabelEntry(PerkLevel.Fourth, "Offensive Parry", "The Legionnaire is capable of exploiting even the smallest gap in an opponents defense."),
                    new LabelEntry(PerkLevel.Fifth, "Hoplite", "Master of the spear, the Legionnaire needs only one hand to wield them properly.")
                });
            }
        }
    }
}