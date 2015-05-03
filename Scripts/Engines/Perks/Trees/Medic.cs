using Server.Mobiles;
using Server;
using Server.Items;

namespace Server.Perks
{
    public class Medic : Perk
    {

        public bool IsDoctor()
        {
            return (Level >= PerkLevel.Fourth);
        }

        /// <summary>
        /// Allows full-handed use of bandages at <code>PerkLevel.Fifth</code>
        /// </summary>
        public bool CanUseBandages
        {
            get
            {
                return (Level >= PerkLevel.Fifth);
            }
        }

        /// <summary>
        /// ctor
        /// </summary>
        public Medic( Player player )
            : base(player)
        {
        }

        /// <summary>
        /// Applies a 33% bonus to heal time at <code>PerkLevel.Third</code>
        /// </summary>
        public double GetHealTimeBonus( double seconds )
        {
            if( Level < PerkLevel.Third )
                return 0;

            return (seconds * 0.33);
        }

        /// <summary>
        /// Applies a 50% chance to avoid slipping on damage while healing at <code>PerkLevel.Second</code>
        /// </summary>
        public bool TryNoSlip()
        {
            if( Level < PerkLevel.Second )
                return false;

            return Utility.RandomDouble() <= 0.50;
        }

        /// <summary>
        /// 100% chance to recover a bandage during healing at <code>PerkLevel.First</code>
        /// </summary>
        public bool TryRecoverBandage()
        {
            return (Level < PerkLevel.First);
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
        public Medic( GenericReader reader )
            : base(reader)
        {
        }

        public override string Description { get { return "Highly effective at changing the tides of war."; } }
        public override int GumpID { get { return 2243; } }
        public override string Label { get { return "Medic"; } }

        public override LabelEntryList LabelEntries
        {
            get
            {
                return new LabelEntryList(new LabelEntry[]
                {
                    new LabelEntry(PerkLevel.First, "Bandage Recovery", "You've learned the necessity of recycling after one or two occasions of taking your supplies for granted."),
                    new LabelEntry(PerkLevel.Second, "Field Medic", "No longer do you feel the heat of battle breathing down your neck, nor the tremble of fear that comes with it."),
                    new LabelEntry(PerkLevel.Third, "Effortless", "There's not much you haven't seen and the amount of time it takes you to bandage a wound is near awe-inspiring."),
                    new LabelEntry(PerkLevel.Fourth, "Surgeon", "Having dressed untold number of wounds, the Medic mends more damage with bandages than most."),
                    new LabelEntry(PerkLevel.Fifth, "Battle Medic", "Having spent years in field you've mastered the art of applying bandages with only your finger-tips.")
                });
            }
        }
    }
}