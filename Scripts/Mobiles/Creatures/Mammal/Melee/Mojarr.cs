using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	public class Mojarr : BaseCreature
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.CrushingBlow;
		}

		[Constructable]
		public Mojarr()
			: base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a mojarr";
			BodyValue = 241;
			BaseSoundID = 1249;

			SetStr( 220, 280 );
			SetDex( 130, 165 );
			SetInt( 100, 140 );

			SetHits( 240, 360 );

			SetDamage( 12, 14 );
			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 45, 50 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 60, 70 );
			SetResistance( ResistanceType.Poison, 55, 70 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, 65, 80 );
			SetSkill( SkillName.Wrestling, 95, 100 );
			SetSkill( SkillName.Tactics, 75, 90 );
			SetSkill( SkillName.Anatomy, 30, 45 );

			Fame = 5500;
			Karma = -4000;

			VirtualArmor = 25;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 99.1;
		}

		public override HideType HideType { get { return HideType.Regular; } }
		public override int Hides { get { return Utility.RandomMinMax( 5, 11 ); } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
		}

		public Mojarr( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
