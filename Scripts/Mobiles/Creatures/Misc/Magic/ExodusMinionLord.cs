using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "Exodus Minion Lords corpse" )]
	public class ExodusMinionLord : BaseCreature
	{
		[Constructable]
		public ExodusMinionLord () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 763;
			Name = "an exodus minion lord";
			BaseSoundID = 357;
			Kills = 20;

			SetStr( 1500, 1600 );
			SetDex( 71, 71 );
			SetInt( 71, 73 );
			SetSkill( SkillName.Wrestling, 70, 80 );
			SetSkill( SkillName.Tactics, 70, 90 );
			SetSkill( SkillName.MagicResist, 86, 95 );
			SetSkill( SkillName.Magery, 75, 100 );

			VirtualArmor = Utility.RandomMinMax( 52, 58 );
			SetFameLevel( 5 );
			SetKarmaLevel( 5 );

			PackWeapon( 5, 5 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Gems, 4 );
		}

		public override int TreasureMapLevel{ get{ return 4; } }
		public override int Meat{ get{ return 1; } }

		public ExodusMinionLord( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
