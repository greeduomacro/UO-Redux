using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Perks;
using Server.Gumps;
using Server.Items;

namespace Aberration.RuneScribing
{
    public class ApplicationTarget : Target
    {
        RuneApplicationTool tool;

        public ApplicationTarget(RuneApplicationTool t)
            : base(1, false, TargetFlags.None)
        {
            tool = t;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is Item)
            {
                Item item = targeted as Item;

                if (tool.Type == RuneApplicationTool.ApplyType.Metal)
                {
                    if (item is BaseJewel)
                    {
                        BaseJewel jewel = item as BaseJewel;

                        if (!jewel.Attributes.IsEmpty)
                        {
                            from.SendMessage("This object already has already been inscribed with runes."); return;
                        }

                        if (CraftResources.GetType(jewel.Resource) == CraftResourceType.Metal
                            || CraftResources.GetType(jewel.Resource) == CraftResourceType.Wood)
                        {
                            from.SendGump(new BasicAttributeGump(item, from, tool)); return;
                        }

                        else from.SendMessage("This is for use on metal and wooden objects only.");
                    }

                    if (item is BaseWeapon)
                    {
                        BaseWeapon weapon = item as BaseWeapon;

                        if (!weapon.Attributes.IsEmpty || !weapon.WeaponAttributes.IsEmpty)
                        {
                            from.SendMessage("This object already has already been inscribed with runes."); return;
                        }

                        if (CraftResources.GetType(weapon.Resource) == CraftResourceType.Metal
                            || CraftResources.GetType(weapon.Resource) == CraftResourceType.Wood)
                        {
                            from.SendGump(new BasicAttributeGump(item, from, tool)); return;
                        }

                        else from.SendMessage("This is for use on metal and wooden objects only.");
                    }

                    if (item is BaseWeapon)
                    {
                        BaseWeapon weapon = item as BaseWeapon;

                        if (!weapon.Attributes.IsEmpty || !weapon.WeaponAttributes.IsEmpty)
                        {
                            from.SendMessage("This object already has already been inscribed with runes."); return;
                        }

                        if (CraftResources.GetType(weapon.Resource) == CraftResourceType.Metal 
                            || CraftResources.GetType(weapon.Resource) == CraftResourceType.Wood)
                        {
                            from.SendGump(new BasicAttributeGump(item, from, tool)); return;
                        }

                        else from.SendMessage("This is for use on metal and wooden objects only.");
                    }

                    if (item is BaseArmor)
                    {
                        BaseArmor armor = item as BaseArmor;

                        if (!armor.Attributes.IsEmpty || !armor.ArmorAttributes.IsEmpty)
                        {
                            from.SendMessage("This object already has already been inscribed with runes."); return;
                        }

                        if (CraftResources.GetType(armor.Resource) == CraftResourceType.Metal
                            || CraftResources.GetType(armor.Resource) == CraftResourceType.Wood)
                        {
                            from.SendGump(new BasicAttributeGump(item, from, tool)); return;
                        }

                        else from.SendMessage("This is for use on metal and wooden objects only.");
                    }
                }

                if (tool.Type == RuneApplicationTool.ApplyType.Leather)
                {
                    if (item is BaseClothing)
                    {
                        BaseClothing cloth = item as BaseClothing;

                        if (!cloth.Attributes.IsEmpty)
                        {
                            from.SendMessage("This object already has already been inscribed with runes."); return;
                        }

                        if (CraftResources.GetType(cloth.Resource) == CraftResourceType.Leather)
                        {
                            from.SendGump(new BasicAttributeGump(item, from, tool)); return;
                        }

                        else from.SendMessage("This is for use on leather objects only."); return;
                    }

                    if (item is BaseArmor)
                    {
                        BaseArmor armor = item as BaseArmor;

                        if (!armor.Attributes.IsEmpty || !armor.ArmorAttributes.IsEmpty)
                        {
                            from.SendMessage("This object already has already been inscribed with runes."); return;
                        }

                        if (CraftResources.GetType(armor.Resource) == CraftResourceType.Leather)
                        {
                            from.SendGump(new BasicAttributeGump(item, from, tool)); return;
                        }

                        else from.SendMessage("This is for use on leather objects only.");
                    }

                    if (item is Spellbook)
                    {
                        if (!((Spellbook)item).Attributes.IsEmpty)
                        {
                            from.SendMessage("This object already has already been inscribed with runes."); return;
                        }

                        else  { from.SendGump(new BasicAttributeGump(item, from, tool)); return; }
                    }

                    //if (tool.Type == RuneApplicationTool.ApplyType.Wood)
                    //{
                    //    if (item is BaseWeapon)
                    //    {
                    //        BaseWeapon weapon = item as BaseWeapon;

                    //        if (!weapon.Attributes.IsEmpty || !weapon.WeaponAttributes.IsEmpty)
                    //        {
                    //            from.SendMessage("This object already has already been inscribed with runes."); return;
                    //        }

                    //        if (CraftResources.GetType(weapon.Resource) == CraftResourceType.Wood)
                    //        {
                    //            from.SendGump(new BasicAttributeGump(item, from));
                    //        }

                    //        else from.SendMessage("This is for use on wooden objects only.");
                    //    }

                    //    if (item is BaseArmor)
                    //    {
                    //        BaseArmor armor = item as BaseArmor;

                    //        if (!armor.Attributes.IsEmpty || !armor.ArmorAttributes.IsEmpty)
                    //        {
                    //            from.SendMessage("This object already has already been inscribed with runes."); return;
                    //        }

                    //        if (CraftResources.GetType(armor.Resource) == CraftResourceType.Wood)
                    //        {
                    //            from.SendGump(new BasicAttributeGump(item, from));
                    //        }

                    //    else from.SendMessage("This is for use on wooden objects only.");

                    //    }
                    //}

                    if (tool.Type == RuneApplicationTool.ApplyType.Cloth)
                    {
                        if (item is BaseClothing)
                        {
                            BaseClothing cloth = item as BaseClothing;

                            if (!cloth.Attributes.IsEmpty)
                            {
                                from.SendMessage("This object already has already been inscribed with runes."); return;
                            }

                            if (CraftResources.GetType(cloth.Resource) == CraftResourceType.None)
                            {
                                from.SendGump(new BasicAttributeGump(item, from, tool));
                            }

                            else from.SendMessage("This is for use on cloth objects only.");
                        }             
                    }
                }
            }
        }
    }


    public class BasicAttributeGump     : Gump
    {
        int[] _values = new int[13]; public int imbuePoints = 0;

        Item beheld; Mobile from; RuneApplicationTool tool;

        public WeaponAttributeGump child;

        int[] _caps = new int[]
        {
            0, 20, 10, 7, 7, 7, 7, 10, 10, 10, 5, 5, 5        
        };

        int[] _multipliers = new int[]
        {
            0, 3, 3, 3, 3, 2, 2, 1, 1, 1, 1, 1, 1        
        };

        AosAttribute[] attributes = new AosAttribute[] 
        {
            AosAttribute.Luck,
            AosAttribute.WeaponDamage,
            AosAttribute.WeaponSpeed,
            AosAttribute.AttackChance,
            AosAttribute.DefendChance,
            AosAttribute.SpellDamage,
            AosAttribute.LowerManaCost,
            AosAttribute.BonusStr,
            AosAttribute.BonusStam,
            AosAttribute.BonusInt,
            AosAttribute.RegenHits,
            AosAttribute.RegenStam,
            AosAttribute.RegenMana
        };

        public BasicAttributeGump
            (int[] vals, int points, Item item, Mobile m, WeaponAttributeGump gump, RuneApplicationTool rt)
                : base(0, 0)
        {
            _values = vals;
            imbuePoints = points;
            child = gump;
            beheld = item; from = m;
            tool = rt;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(12, 0, 283, 480, 9380);
            this.AddImage(212, 392, 51);
            this.AddLabel(233, 407, 0, imbuePoints.ToString());

            this.AddItem(31, 33, 3676);
            this.AddItem(80, 29, 3688);
            this.AddItem(132, 33, 3686);
            this.AddItem(231, 33, 3679);
            this.AddItem(182, 29, 3682);

            this.AddLabel(41, 59, 1209, @"Damage Increase");
            this.AddLabel(41, 81, 1409, @"Speed Increase");
            this.AddLabel(41, 104, 1309, @"Improved Accuracy");
            this.AddLabel(41, 127, 1109, @"Improved Defense");
            this.AddButton(253, 61, 55, 55, (int)Buttons.dmgIncBtn, GumpButtonType.Reply, 0);
            this.AddLabel(41, 150, 1509, @"Inc. Spell Damage");
            this.AddLabel(41, 173, 1609, @"Lower Mana Cost");
            this.AddLabel(41, 196, 2115, @"Strength Bonus");
            this.AddLabel(41, 219, 2120, @"Dexterity Bonus");
            this.AddLabel(41, 241, 2113, @"Intelligence Bonus");
            this.AddLabel(41, 263, 2201, @"Hitpoint Regen");
            this.AddLabel(41, 286, 2211, @"Stamina Regen");
            this.AddLabel(42, 309, 2222, @"Mana Regen");

            this.AddItem(24, 339, 3913);
            this.AddItem(34, 337, 5041);
            this.AddLabel(72, 342, 0, @"Weapon Abilities");
            this.AddButton(185, 345, 1209, 1210, (int)Buttons.weaponBtn, GumpButtonType.Reply, 0);

            this.AddButton(253, 83, 55, 55, (int)Buttons.spdIncBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 106, 55, 55, (int)Buttons.accIncBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 129, 55, 55, (int)Buttons.DefIncBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 152, 55, 55, (int)Buttons.SpellDmgIncBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 175, 55, 55, (int)Buttons.LowerManaBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 198, 55, 55, (int)Buttons.strBonusBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 221, 55, 55, (int)Buttons.dexBonusBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 243, 55, 55, (int)Buttons.intBonusBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 265, 55, 55, (int)Buttons.hpRegenBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 288, 55, 55, (int)Buttons.stamRegenBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 311, 55, 55, (int)Buttons.manaRegenBtn, GumpButtonType.Reply, 0);

            this.AddButton(204, 61, 56, 56, (int)Buttons.dmgDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 83, 56, 56, (int)Buttons.spdDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 106, 56, 56, (int)Buttons.accDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 129, 56, 56, (int)Buttons.defDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 152, 56, 56, (int)Buttons.SpellDmgDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 175, 56, 56, (int)Buttons.lowerManaDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 198, 56, 56, (int)Buttons.StrBonusDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 221, 56, 56, (int)Buttons.DexBonusDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 243, 56, 56, (int)Buttons.intBonusDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 265, 56, 56, (int)Buttons.hpRegenDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 288, 56, 56, (int)Buttons.stamRegenDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 311, 56, 56, (int)Buttons.manaRegenDecBtn, GumpButtonType.Reply, 0);

            this.AddButton(50, 399, 239, 239, (int)Buttons.applyButton, GumpButtonType.Reply, 0);

            this.AddLabel(231, 59, 0, _values[1].ToString());
            this.AddLabel(231, 82, 0, _values[2].ToString());
            this.AddLabel(231, 105, 0, _values[3].ToString());
            this.AddLabel(231, 128, 0, _values[4].ToString());
            this.AddLabel(231, 151, 0, _values[5].ToString());
            this.AddLabel(231, 174, 0, _values[6].ToString());
            this.AddLabel(231, 197, 0, _values[7].ToString());
            this.AddLabel(231, 220, 0, _values[8].ToString());
            this.AddLabel(231, 242, 0, _values[9].ToString());
            this.AddLabel(231, 264, 0, _values[10].ToString());
            this.AddLabel(231, 287, 0, _values[11].ToString());
            this.AddLabel(231, 310, 0, _values[12].ToString());
        }

        public BasicAttributeGump(Item item, Mobile m, RuneApplicationTool rt)
            : base(0, 0)
        {
            beheld = item; from = m;
            tool = rt;
            imbuePoints = (int)((m.Skills.Inscribe.Value / 5) - 12);

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(12, 0, 283, 480, 9380);
            this.AddImage(212, 392, 51);
            this.AddLabel(233, 407, 0, imbuePoints.ToString());

            this.AddItem(31, 33, 3676);
            this.AddItem(80, 29, 3688);
            this.AddItem(132, 33, 3686);
            this.AddItem(231, 33, 3679);
            this.AddItem(182, 29, 3682);

            this.AddLabel(41, 59, 1209, @"Damage Increase");
            this.AddLabel(41, 81, 1409, @"Speed Increase");
            this.AddLabel(41, 104, 1309, @"Improved Accuracy");
            this.AddLabel(41, 127, 1109, @"Improved Defense");
            this.AddButton(253, 61, 55, 55, (int)Buttons.dmgIncBtn, GumpButtonType.Reply, 0);
            this.AddLabel(41, 150, 1509, @"Inc. Spell Damage");
            this.AddLabel(41, 173, 1609, @"Lower Mana Cost");
            this.AddLabel(41, 196, 2115, @"Strength Bonus");
            this.AddLabel(41, 219, 2120, @"Dexterity Bonus");
            this.AddLabel(41, 241, 2113, @"Intelligence Bonus");
            this.AddLabel(41, 263, 2201, @"Hitpoint Regen");
            this.AddLabel(41, 286, 2211, @"Stamina Regen");
            this.AddLabel(42, 309, 2222, @"Mana Regen");

            this.AddItem(24, 339, 3913);
            this.AddItem(34, 337, 5041);
            this.AddLabel(72, 342, 0, @"Weapon Abilities");
            this.AddButton(185, 345, 1209, 1210, (int)Buttons.weaponBtn, GumpButtonType.Reply, 0);

            this.AddButton(253, 83, 55, 55, (int)Buttons.spdIncBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 106, 55, 55, (int)Buttons.accIncBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 129, 55, 55, (int)Buttons.DefIncBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 152, 55, 55, (int)Buttons.SpellDmgIncBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 175, 55, 55, (int)Buttons.LowerManaBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 198, 55, 55, (int)Buttons.strBonusBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 221, 55, 55, (int)Buttons.dexBonusBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 243, 55, 55, (int)Buttons.intBonusBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 265, 55, 55, (int)Buttons.hpRegenBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 288, 55, 55, (int)Buttons.stamRegenBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 311, 55, 55, (int)Buttons.manaRegenBtn, GumpButtonType.Reply, 0);

            this.AddButton(204, 61, 56, 56, (int)Buttons.dmgDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 83, 56, 56, (int)Buttons.spdDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 106, 56, 56, (int)Buttons.accDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 129, 56, 56, (int)Buttons.defDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 152, 56, 56, (int)Buttons.SpellDmgDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 175, 56, 56, (int)Buttons.lowerManaDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 198, 56, 56, (int)Buttons.StrBonusDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 221, 56, 56, (int)Buttons.DexBonusDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 243, 56, 56, (int)Buttons.intBonusDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 265, 56, 56, (int)Buttons.hpRegenDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 288, 56, 56, (int)Buttons.stamRegenDecBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 311, 56, 56, (int)Buttons.manaRegenDecBtn, GumpButtonType.Reply, 0);

            this.AddButton(50, 399, 239, 239, (int)Buttons.applyButton, GumpButtonType.Reply, 0);

            this.AddLabel(231, 59, 0, _values[1].ToString());
            this.AddLabel(231, 82, 0, _values[2].ToString());
            this.AddLabel(231, 105, 0, _values[3].ToString());
            this.AddLabel(231, 128, 0, _values[4].ToString());
            this.AddLabel(231, 151, 0, _values[5].ToString());
            this.AddLabel(231, 174, 0, _values[6].ToString());
            this.AddLabel(231, 197, 0, _values[7].ToString());
            this.AddLabel(231, 220, 0, _values[8].ToString());
            this.AddLabel(231, 242, 0, _values[9].ToString());
            this.AddLabel(231, 264, 0, _values[10].ToString());
            this.AddLabel(231, 287, 0, _values[11].ToString());
            this.AddLabel(231, 310, 0, _values[12].ToString());
        }

        public enum Buttons
        {
            None,
            //Increase
            dmgIncBtn,          
            spdIncBtn,
            accIncBtn,
            DefIncBtn,
            SpellDmgIncBtn,
            LowerManaBtn,
            strBonusBtn,
            dexBonusBtn,
            intBonusBtn,
            hpRegenBtn,
            stamRegenBtn,
            manaRegenBtn,
            //Decrease
            dmgDecBtn,
            spdDecBtn,
            accDecBtn,
            defDecBtn,
            SpellDmgDecBtn,
            lowerManaDecBtn,
            StrBonusDecBtn,
            DexBonusDecBtn,
            intBonusDecBtn,
            hpRegenDecBtn,
            stamRegenDecBtn,
            manaRegenDecBtn,
            //Functions
            applyButton,
            weaponBtn
        }

        void ApplyAttributes()
        {    
            double failChance = (100 / (from.Skills.Inscribe.Value * 10)) * 2.5;

            bool empty = true;

            for (int i = 1; i <= 12; i++)
            {
                if (_values[i] > 0)
                {
                    empty = false;
                    break;
                }
            }

            if (empty)
            {
                from.SendMessage("You have not chosen any runes to apply."); return;
            }

            if (Utility.RandomDouble() <= failChance)
            {
                beheld.Delete(); Effects.SendLocationEffect(from.Location, from.Map, 14000, 16, 3, 0, 0); 
                from.PlaySound(0x207);
                from.SendMessage("You overlap a two runes, causing an ethereal reaction, destroying the object!");
                from.Damage(Utility.RandomMinMax(20, 40)); tool.Delete(); return;
            }

            Effects.PlaySound(from.Location, from.Map, 0x243);
            from.SendMessage("You successfully inscribe the runes to the object, imbuing it with magic properties.");

            if (beheld is BaseArmor)
            {
                BaseArmor armor = beheld as BaseArmor;

                armor.Identified = true;

                for (int i = 1; i < _values.Length; i++)
                {
                    if (_values[i] > 0)
                    {
                        armor.Attributes[attributes[i]] = _values[i] * _multipliers[i];
                    }
                }            
            }

            if (beheld is BaseWeapon)
            {
                BaseWeapon weapon = beheld as BaseWeapon;

                weapon.Identified = true;

                for (int i = 1; i < _values.Length; i++)
                {
                    if (_values[i] > 0)
                    {
                        weapon.Attributes[attributes[i]] = _values[i] * _multipliers[i];
                    }
                }
            }

            if (beheld is BaseJewel)
            {
                BaseJewel jewel = beheld as BaseJewel;

                jewel.Identified = true;

                for (int i = 1; i < _values.Length; i++)
                {
                    if (_values[i] > 0)
                    {
                        jewel.Attributes[attributes[i]] = _values[i] * _multipliers[i];
                    }
                }
            }

            if (beheld is BaseClothing)
            {
                BaseClothing clothing = beheld as BaseClothing;

                clothing.Identified = true;

                for (int i = 1; i < _values.Length; i++)
                {
                    if (_values[i] > 0)
                    {
                        clothing.Attributes[attributes[i]] = _values[i] * _multipliers[i];
                    }
                }
            }

            if (beheld is Spellbook)
            {
                Spellbook book = beheld as Spellbook;

                for (int i = 1; i < _values.Length; i++)
                {
                    if (_values[i] > 0)
                    {
                        book.Attributes[attributes[i]] = _values[i] * _multipliers[i];
                    }
                }
            }
        }

        public void UpdateGump()
        {
            from.CloseGump(typeof(BasicAttributeGump));
            from.SendGump(new BasicAttributeGump(_values, imbuePoints, beheld, from, child, tool));
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == (int)Buttons.weaponBtn)
            {
                if (beheld is BaseWeapon)
                {
                    if (!from.HasGump(typeof(WeaponAttributeGump)))
                    {
                        WeaponAttributeGump gump = new WeaponAttributeGump(beheld, from, imbuePoints, this);
                        from.SendGump(gump);
                        child = gump;
                    }

                    from.SendGump(new BasicAttributeGump(_values, imbuePoints, beheld, from, child, tool));
                }

                else
                {
                    from.SendMessage("This is not a weapon!");
                    from.SendGump(new BasicAttributeGump(_values, imbuePoints, beheld, from, child, tool));
                }
            }

            if (info.ButtonID == (int)Buttons.applyButton)
            {
                ApplyAttributes();
                return;
            }

            //Increase
            else if (info.ButtonID >= (int)Buttons.dmgIncBtn && info.ButtonID <= (int)Buttons.manaRegenBtn)
            {
                if (imbuePoints <= 0)
                {
                    from.SendMessage("You do not have any imbue points lefts.");
                    UpdateGump(); return;
                }

                if (_values[info.ButtonID] + 1 > _caps[info.ButtonID])
                {
                    from.SendMessage("This object can not withstand more imbuement of that type.");
                    UpdateGump(); return;
                }

                imbuePoints--; 
                _values[info.ButtonID]++;
                UpdateGump();

                if (child != null)
                {
                    child.imbuePoints--;
                    child.parent = this;
                    child.UpdateGump();
                }
            }

            //Decrease
            else if (info.ButtonID >= (int)Buttons.dmgDecBtn && info.ButtonID <= (int)Buttons.manaRegenDecBtn)
            {
                if (_values[(info.ButtonID - 12)] <= 0)
                {
                    from.SendMessage("You can not lower this value any further.");
                    UpdateGump(); return;
                }

                _values[(info.ButtonID - 12)]--; 
                imbuePoints++; UpdateGump();
 
                if (child != null)
                {
                    child.imbuePoints++;
                    child.parent = this;
                    child.UpdateGump();
                }
            }
        }
    }

    public class WeaponAttributeGump    : Gump
    {
        int[] _values = new int[11]; 
        public int imbuePoints = 0;

        public BasicAttributeGump parent;

        Item beheld; Mobile from;

        int[] _caps = new int[]
        {
            0, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10        
        };

        int[] _multipliers = new int[]
        {
            0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5        
        };

        AosWeaponAttribute[] attributes = new AosWeaponAttribute[] 
        {
           AosWeaponAttribute.DurabilityBonus,
           AosWeaponAttribute.HitDispel,
           AosWeaponAttribute.HitFireball,
           AosWeaponAttribute.HitHarm,
           AosWeaponAttribute.HitLightning,
           AosWeaponAttribute.HitMagicArrow,
           AosWeaponAttribute.HitLeechHits,
           AosWeaponAttribute.HitLeechMana,
           AosWeaponAttribute.HitLeechStam,
           AosWeaponAttribute.HitLowerAttack,
           AosWeaponAttribute.HitLowerDefend,

        };

        public WeaponAttributeGump(int[] vals, int points, Item item, Mobile m, BasicAttributeGump gump)
            : base(60, 0)
        {
            _values = vals;
            imbuePoints = points;
            parent = gump;
            beheld = item; from = m;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(12, 0, 283, 380, 9380);

            this.AddImage(212, 292, 51);
            this.AddLabel(233, 307, 0, imbuePoints.ToString());

            this.AddItem(31, 33, 3676);
            this.AddItem(80, 29, 3688);
            this.AddItem(132, 33, 3686);
            this.AddItem(231, 33, 3679);
            this.AddItem(182, 29, 3682);

            this.AddLabel(41, 59, 1209, @"Hit Dispel");
            this.AddLabel(41, 81, 1409, @"Hit Fireball");
            this.AddLabel(41, 104, 1309, @"Hit Harm");
            this.AddLabel(41, 127, 1109, @"Hit Lightning");
            this.AddLabel(41, 150, 1509, @"Hit Magic Arrow");
            this.AddLabel(41, 173, 1609, @"Life Leech");
            this.AddLabel(41, 196, 2115, @"Mana Leech");
            this.AddLabel(41, 219, 2120, @"Stam Leech");
            this.AddLabel(41, 241, 2113, @"Lower Attack");
            this.AddLabel(41, 263, 2201, @"Lower Defense");

            this.AddButton(253, 61, 55, 55, (int)Buttons.dispelBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 83, 55, 55, (int)Buttons.fireballBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 106, 55, 55, (int)Buttons.harmcBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 129, 55, 55, (int)Buttons.lightningBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 152, 55, 55, (int)Buttons.mArrowBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 175, 55, 55, (int)Buttons.lifeLeechBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 198, 55, 55, (int)Buttons.manaLeechBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 221, 55, 55, (int)Buttons.stamLeechBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 243, 55, 55, (int)Buttons.lowAttBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 265, 55, 55, (int)Buttons.lowDefBtn, GumpButtonType.Reply, 0);

            this.AddButton(204, 61, 56, 56, (int)Buttons._dispelBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 83, 56, 56, (int)Buttons._fireballBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 106, 56, 56, (int)Buttons._harmcBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 129, 56, 56, (int)Buttons._lightningBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 152, 56, 56, (int)Buttons._mArrowBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 175, 56, 56, (int)Buttons._lifeLeechBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 198, 56, 56, (int)Buttons._manaLeechBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 221, 56, 56, (int)Buttons._stamLeechBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 243, 55, 55, (int)Buttons._lowAttBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 265, 55, 55, (int)Buttons._lowDefBtn, GumpButtonType.Reply, 0);

            this.AddButton(50, 300, 239, 239, (int)Buttons.applyButton, GumpButtonType.Reply, 0);

            this.AddLabel(231, 59, 0, _values[1].ToString());
            this.AddLabel(231, 82, 0, _values[2].ToString());
            this.AddLabel(231, 105, 0, _values[3].ToString());
            this.AddLabel(231, 128, 0, _values[4].ToString());
            this.AddLabel(231, 151, 0, _values[5].ToString());
            this.AddLabel(231, 174, 0, _values[6].ToString());
            this.AddLabel(231, 197, 0, _values[7].ToString());
            this.AddLabel(231, 220, 0, _values[8].ToString());
            this.AddLabel(231, 242, 0, _values[9].ToString());
            this.AddLabel(231, 264, 0, _values[10].ToString());
        }

        public WeaponAttributeGump(Item item, Mobile m, int points, BasicAttributeGump gump)
            : base(0, 60)
        {
            beheld = item; from = m;
            parent = gump;
            imbuePoints = points;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(12, 0, 283, 380, 9380);

            this.AddImage(212, 292, 51);
            this.AddLabel(233, 307, 0, imbuePoints.ToString());

            this.AddItem(31, 33, 3676);
            this.AddItem(80, 29, 3688);
            this.AddItem(132, 33, 3686);
            this.AddItem(231, 33, 3679);
            this.AddItem(182, 29, 3682);

            this.AddLabel(41, 59, 1209, @"Hit Dispel");
            this.AddLabel(41, 81, 1409, @"Hit Fireball");
            this.AddLabel(41, 104, 1309, @"Hit Harm");
            this.AddLabel(41, 127, 1109, @"Hit Lightning");
            this.AddLabel(41, 150, 1509, @"Hit Magic Arrow");
            this.AddLabel(41, 173, 1609, @"Life Leech");
            this.AddLabel(41, 196, 2115, @"Mana Leech");
            this.AddLabel(41, 219, 2120, @"Stam Leech");
            this.AddLabel(41, 241, 2113, @"Lower Attack");
            this.AddLabel(41, 263, 2201, @"Lower Defense");

            this.AddButton(253, 61, 55, 55, (int)Buttons.dispelBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 83, 55, 55, (int)Buttons.fireballBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 106, 55, 55, (int)Buttons.harmcBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 129, 55, 55, (int)Buttons.lightningBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 152, 55, 55, (int)Buttons.mArrowBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 175, 55, 55, (int)Buttons.lifeLeechBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 198, 55, 55, (int)Buttons.manaLeechBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 221, 55, 55, (int)Buttons.stamLeechBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 243, 55, 55, (int)Buttons.lowAttBtn, GumpButtonType.Reply, 0);
            this.AddButton(253, 265, 55, 55, (int)Buttons.lowDefBtn, GumpButtonType.Reply, 0);

            this.AddButton(204, 61, 56, 56, (int)Buttons._dispelBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 83, 56, 56, (int)Buttons._fireballBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 106, 56, 56, (int)Buttons._harmcBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 129, 56, 56, (int)Buttons._lightningBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 152, 56, 56, (int)Buttons._mArrowBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 175, 56, 56, (int)Buttons._lifeLeechBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 198, 56, 56, (int)Buttons._manaLeechBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 221, 56, 56, (int)Buttons._stamLeechBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 243, 56, 56, (int)Buttons._lowAttBtn, GumpButtonType.Reply, 0);
            this.AddButton(204, 265, 56, 56, (int)Buttons._lowDefBtn, GumpButtonType.Reply, 0);

            this.AddButton(50, 300, 239, 239, (int)Buttons.applyButton, GumpButtonType.Reply, 0);

            this.AddLabel(231, 59, 0, _values[1].ToString());
            this.AddLabel(231, 82, 0, _values[2].ToString());
            this.AddLabel(231, 105, 0, _values[3].ToString());
            this.AddLabel(231, 128, 0, _values[4].ToString());
            this.AddLabel(231, 151, 0, _values[5].ToString());
            this.AddLabel(231, 174, 0, _values[6].ToString());
            this.AddLabel(231, 197, 0, _values[7].ToString());
            this.AddLabel(231, 220, 0, _values[8].ToString());
            this.AddLabel(231, 242, 0, _values[9].ToString());
            this.AddLabel(231, 264, 0, _values[10].ToString());
        }

        public enum Buttons
        {
            None,
            //Increase
            dispelBtn,
            fireballBtn,
            harmcBtn,
            lightningBtn,
            mArrowBtn,
            lifeLeechBtn,
            manaLeechBtn,
            stamLeechBtn,
            lowAttBtn,
            lowDefBtn,
            //Decrease
            _dispelBtn,
            _fireballBtn,
            _harmcBtn,
            _lightningBtn,
            _mArrowBtn,
            _lifeLeechBtn,
            _manaLeechBtn,
            _stamLeechBtn,
            _lowAttBtn,
            _lowDefBtn,
            //Functions
            applyButton,
        }

        void ApplyAttributes()
        {      
            double failChance = (100 / (from.Skills.Inscribe.Value * 10)) * 2.5;
            bool empty = true;

            for (int i = 1; i <= 10; i++)
            {
                if (_values[i] > 0)
                {
                    empty = false;
                    break;
                }
            }

            if(empty)
            {
                from.SendMessage("You have not chosen any runes to apply."); return;
            }

            if (Utility.RandomDouble() <= failChance)
            {
                beheld.Delete(); Effects.SendLocationEffect(from.Location, from.Map, 14000, 16, 3, 0, 0); 
                from.PlaySound(0x207);
                from.SendMessage("You overlap a two runes, causing an ethereal reaction, destroying the object!");
                from.Damage(Utility.RandomMinMax(20, 40)); return;
            }

            Effects.PlaySound(from.Location, from.Map, 0x243);
            from.SendMessage("You successfully inscribe the runes to the object, imbuing it with magic properties.");

            if (beheld is BaseWeapon)
            {
                BaseWeapon weapon = beheld as BaseWeapon;

                weapon.Identified = true;

                for (int i = 1; i < _values.Length; i++)
                {
                    if (_values[i] > 0)
                    {
                        weapon.WeaponAttributes[attributes[i]] = _values[i] * _multipliers[i];
                    }
                }
            }
        }

        public void UpdateGump()
        {
            from.CloseGump(typeof(WeaponAttributeGump));
            from.SendGump(new WeaponAttributeGump(_values, imbuePoints, beheld, from, parent));
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
                if (info.ButtonID == (int)Buttons.applyButton)
                {
                    ApplyAttributes();                  
                    return;
                }
                //Increase
                else if (info.ButtonID >= (int)Buttons.dispelBtn && info.ButtonID <= (int)Buttons.lowDefBtn)
                {
                    if (imbuePoints <= 0)
                    {
                        from.SendMessage("You do not have any imbue points lefts.");
                        UpdateGump(); return;
                    }

                    if (_values[info.ButtonID] + 1 > _caps[info.ButtonID])
                    {
                        from.SendMessage("This object can not withstand more imbuement of that type.");
                        UpdateGump(); return;
                    }

                    imbuePoints--; _values[info.ButtonID]++; UpdateGump();
                    parent.imbuePoints--; parent.child = this; parent.UpdateGump(); return;
                }
                //Decrease
                else if (info.ButtonID >= (int)Buttons._dispelBtn && info.ButtonID <= (int)Buttons._lowDefBtn)
                {
                    if (_values[(info.ButtonID - 10)] <= 0)
                    {
                        from.SendMessage("You can not lower this value any further.");
                        UpdateGump(); return;
                    }

                    _values[(info.ButtonID - 10)]--; imbuePoints++; UpdateGump();
                    parent.imbuePoints++; parent.child = this; parent.UpdateGump(); return;

                }
            }
    }


	public class RuneApplicationTool : Item
	{
        public enum ApplyType
        {
            None,
            Leather,
            Metal,
            Cloth
        }

        public ApplyType Type = ApplyType.None;

		[Constructable]
		public RuneApplicationTool( int itemid )
			: base( itemid )
		{
			Weight = 1.0;
            Stackable = false;
		}

		public override void OnDoubleClick( Mobile from )
		{
            if (IsChildOf(from.Backpack))
            {
                if (from.Skills.Inscribe.Value >= 80.0)
                    from.Target = new ApplicationTarget(this);

                else from.SendMessage("You are not skilled enough in the art of runic inscription.");
            }

            else from.SendMessage("You must have this in your backpack to use it.");
        }

        public RuneApplicationTool(Serial serial)
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
            writer.Write((int)Type);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
            Type = (ApplyType)reader.ReadInt();
		}
	}


    public class GemTippedChisel    : RuneApplicationTool
    {
        [Constructable]
        public GemTippedChisel()
            : base(4787)
        {
            Name = "A gem-tipped chisel and mallet";
            Type = ApplyType.Metal;
        }

        public GemTippedChisel(Serial serial)
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

    public class BrandingIron       : RuneApplicationTool
    {
        [Constructable]
        public BrandingIron()
            : base(4024)
        {
            Name = "A branding iron";
            Type = ApplyType.Leather;
        }

        public BrandingIron(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class EaselAndBrush      : RuneApplicationTool
    {
        [Constructable]
        public EaselAndBrush()
            : base(4033)
        {
            Name = "A paint brush and easel";
            Type = ApplyType.Cloth;
        }

        public EaselAndBrush(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

}