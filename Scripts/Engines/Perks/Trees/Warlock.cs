using Server.Mobiles;
using Server;
using Server.Items;

namespace Server.Perks
{
    public class Warlock : Perk
    {
        /// <summary>
        /// ctor
        /// </summary>
        public Warlock( Player player )
            : base(player)
        {
        }

        public bool Conduit()
        {
            return (Level >= PerkLevel.First);
        }

        public bool Overclock()
        {
            return (Level >= PerkLevel.Second);
        }

        public void SoulEater(Mobile attacker, Mobile defender, BaseWeapon weapon)
        {
            if (Level >= PerkLevel.Fifth)
            {
                if (!(weapon is Fists) && !(weapon is BaseRanged) && weapon is BaseWeapon)
                {
                    int manaLeech = (int)(weapon.Weight * 1.165);

                    if (defender.Mana >= manaLeech)
                    {
                        defender.Mana -= manaLeech;
                        attacker.Mana += manaLeech;
                        attacker.PlaySound(0x44D);
                    }

                    if (defender.Mana < manaLeech && defender.Mana > 0)
                    {
                        attacker.PlaySound(0x44D);
                        attacker.Mana += defender.Mana;
                        defender.Mana = 0;
                    }
                }
            }
        }

        public void Capacitor(Mobile attacker, Mobile defender, BaseWeapon weapon)
        {
            if (Level >= PerkLevel.Fourth)
            {
                if (!(weapon is Fists) && !(weapon is BaseRanged) && weapon is BaseWeapon && Utility.RandomDouble() <= 0.10)
                {
                    int element = Utility.RandomMinMax(1, 4);

                    switch (element)
                    {
                        case 1:
                            {
                                weapon.DoLightning(attacker, defender);
                                break;
                            }
                        case 2:
                            {
                                weapon.DoMagicArrow(attacker, defender);
                                break;
                            }
                        case 3:
                            {
                                weapon.DoHarm(attacker, defender);
                                break;
                            }
                        case 4:
                            {
                                weapon.DoFireball(attacker, defender);
                                break;
                            }

                    }
                }
            }
        }

        public bool ShieldEquipped
        {
            get
            {
                return Player != null && (Player.FindItemOnLayer(Layer.TwoHanded) is BaseShield);
            }
        }

        public bool ParrySpell()
        {
            if (Level >= PerkLevel.Third)
            {
                if (ShieldEquipped && Utility.RandomDouble() <= 0.25)
                {
                    Player.FixedEffect(0x37B9, 10, 16);
                    Player.SendMessage("You deflect your opponent's spell with your shield!");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Serialization
        /// </summary>
        protected override void Serialize( Server.GenericWriter writer )
        {
            base.Serialize(writer);
        }

        /// <summary>
        /// Deserialization
        /// </summary>
        public Warlock( GenericReader reader )
            : base(reader)
        {
        }

        public override string Description { get { return "Master of manipulating Ether through metal."; } }
        public override int GumpID { get { return 20741; } }
        public override string Label { get { return "Warlock"; } }

        public override LabelEntryList LabelEntries
        {
            get
            {
                return new LabelEntryList(new LabelEntry[]
                {
                    new LabelEntry(PerkLevel.First, "Ethereal Conduit", "By using mana to produce electrical charges that manipulate ether, the Warlock is able to channel through metal."),
                    new LabelEntry(PerkLevel.Second, "Arcane Overclock", "The Warlock is capable of overriding the bodies natural limitation as an ethereal circuit."),
                    new LabelEntry(PerkLevel.Third, "The Aegis", "By producing a mana-field around a shield, the Warlock is capable of parrying enemy spells."),
                    new LabelEntry(PerkLevel.Fourth, "Arcane Capacitor", "Channeling through weapons leaves a residual charge behind, causing them to occasionally produce arcane effects."),
                    new LabelEntry(PerkLevel.Fifth, "Soul Eater", "The Warlock is capable inserting a weapon into flesh and using it as bridge between life-forces.")
                });
            }
        }
    }
}