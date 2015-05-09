using Server.Mobiles;
using Server;

namespace Server.Perks
{
    public class Craftsman : Perk
    {
        public bool WorkHorse()
        {
            return (Level >= PerkLevel.First);
        }

        public bool Craftsmanship()
        {
            return (Level >= PerkLevel.Second);
        }

        public bool Efficient()
        {
            return (Level >= PerkLevel.Third);
        }

        public bool Savvy()
        {
            return (Level >= PerkLevel.Fourth);
        }

        public bool Master()
        {
            return (Level >= PerkLevel.Fifth);
        }

        /// <summary>
        /// ctor
        /// </summary>
        public Craftsman( Player player )
            : base(player)
        {
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
        public Craftsman( GenericReader reader )
            : base(reader)
        {
        }

        public override string Description { get { return "A master of molding the world around them."; } }
        public override int GumpID { get { return 2246; } }
        public override string Label { get { return "Craftsman"; } }

        public override LabelEntryList LabelEntries
        {
            get
            {
                return new LabelEntryList(new LabelEntry[]
                {
                    new LabelEntry(PerkLevel.First, "Work Horse", 
                        "Years of carrying goods to and fro gives you a stronger back than most."),
                    new LabelEntry(PerkLevel.Second, "Craftsmanship", 
                        "The years have tought you to take your time and forego no effort."),
                    new LabelEntry(PerkLevel.Third, "Efficiency", 
                        "You are capable of crafting items with less material than most."),
                    new LabelEntry(PerkLevel.Fourth, "Resource Savvy", 
                        "The experienced craftsman is capable of extracting more resources when recycling."),
                    new LabelEntry(PerkLevel.Fifth, "Master Craftsman", 
                        "You've reached a point in your training where you begin to produce masterworks.")
                });
            }
        }
    }
}