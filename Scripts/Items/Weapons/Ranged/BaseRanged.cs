using Server.Mobiles;
using Server.Network;
using Server.Perks;
using Server.Spells;
using System;

namespace Server.Items
{
    public abstract class BaseRanged : BaseMeleeWeapon
    {
        public abstract int EffectID { get; }
        public abstract Type AmmoType { get; }
        public abstract Item Ammo { get; }

        public override int DefHitSound { get { return 0x234; } }
        public override int DefMissSound { get { return 0x238; } }

        public override SkillName DefSkill { get { return SkillName.Archery; } }
        public override WeaponType DefType { get { return WeaponType.Ranged; } }
        public override WeaponAnimation DefAnimation { get { return WeaponAnimation.ShootXBow; } }

        public override SkillName AccuracySkill { get { return SkillName.Archery; } }

        private Timer m_RecoveryTimer; // so we don't start too many timers

        public BaseRanged(int itemID)
            : base(itemID)
        {
            Resource = CraftResource.RegularWood;
        }

        public BaseRanged(Serial serial)
            : base(serial)
        {
        }

        public override bool OnEquip(Mobile from)
        {
            if(from is Player)
            {
                Marksman mm = Perk.GetByType<Marksman>((Player)from);

                if(mm != null && mm.LongShot() == true)
                {
                    this.MaxRange += 3;
                    mm.BowConverted = true;
                }
            }

            return base.OnEquip(from);
        }

        public override void OnRemoved(IEntity parent)
        {
            if(parent is Player)
            {
                Marksman mm = Perk.GetByType<Marksman>((Player)parent);

                if(mm != null && mm.LongShot() == true)
                {
                    if(mm.BowConverted)
                    {
                        this.MaxRange -= 3;
                        mm.BowConverted = false;
                    }
                }
            }

            base.OnRemoved(parent);
        }

        public override TimeSpan OnSwing(Mobile attacker, Mobile defender)
        {
            WeaponAbility a = WeaponAbility.GetCurrentAbility(attacker);

            if(this.Parent is Player)
            {
                Marksman mm = Perk.GetByType<Marksman>((Player)this.Parent);

                if(mm != null && mm.RunAndGun())
                {
                    bool canSwing = true;

                    if(Core.AOS)
                    {
                        canSwing = (!attacker.Paralyzed && !attacker.Frozen);

                        if(canSwing)
                        {
                            Spell sp = attacker.Spell as Spell;

                            canSwing = (sp == null || !sp.IsCasting || !sp.BlocksMovement);
                        }
                    }

                    if(canSwing && attacker.HarmfulCheck(defender))
                    {
                        attacker.DisruptiveAction();
                        attacker.Send(new Swing(0, attacker, defender));

                        Item weapon = this as BaseRanged;

                        if(weapon != null)
                        {
                            if(((Player)this.Parent).Stam < (int)(((weapon.Weight + 2) / 2) + 3))
                            {
                                canSwing = false;
                                ((Player)this.Parent).SendMessage("You do not have the stamina to draw your bow.");
                            }
                            else
                            {
                                ((Player)this.Parent).Stam -= (int)(((weapon.Weight + 2) / 2) + 3);
                            }
                        }

                        if(OnFired(attacker, defender))
                        {
                            if(CheckHit(attacker, defender))
                                OnHit(attacker, defender);
                            else
                                OnMiss(attacker, defender);
                        }
                    }

                    attacker.RevealingAction();

                    return GetDelay(attacker);
                }
            }

            // Make sure we've been standing still for .25/.5/1 second depending on Era
            if(DateTime.Now > (attacker.LastMoveTime + TimeSpan.FromSeconds(Core.SE ? 0.25 : (Core.AOS ? 0.5 : 1.0))) || (Core.AOS && WeaponAbility.GetCurrentAbility(attacker) is MovingShot))
            {
                bool canSwing = true;

                if(Core.AOS)
                {
                    canSwing = (!attacker.Paralyzed && !attacker.Frozen);

                    if(canSwing)
                    {
                        Spell sp = attacker.Spell as Spell;

                        canSwing = (sp == null || !sp.IsCasting || !sp.BlocksMovement);
                    }
                }

                if(canSwing && attacker.HarmfulCheck(defender))
                {
                    attacker.DisruptiveAction();
                    attacker.Send(new Swing(0, attacker, defender));

                    Item weapon = this as BaseRanged;


                    if(Parent is Player)
                    {
                        if(weapon != null)
                        {
                            if(((Player)this.Parent).Stam < (int)(((weapon.Weight + 2) / 2) + 3))
                            {
                                canSwing = false;
                                ((Player)this.Parent).SendMessage("You do not have the stamina to draw your bow.");
                            }
                            else
                            {
                                ((Player)this.Parent).Stam -= (int)(((weapon.Weight + 2) / 2) + 3);
                            }
                        }
                    }

                    if(OnFired(attacker, defender))
                    {
                        if(CheckHit(attacker, defender))
                            OnHit(attacker, defender);
                        else
                            OnMiss(attacker, defender);
                    }
                }

                attacker.RevealingAction();

                return GetDelay(attacker);
            }
            else
            {
                attacker.RevealingAction();

                return TimeSpan.FromSeconds(0.25);
            }
        }

        public override bool CheckHit(Mobile attacker, Mobile defender)
        {
            if(defender is Player)
            {
                Legionnaire leg = Perk.GetByType<Legionnaire>((Player)defender);

                if(leg != null)
                {
                    if(base.CheckHit(attacker, defender))
                    {
                        if(leg.TryDodgeMissile())
                        {
                            return false;
                        }
                    }
                }

                return base.CheckHit(attacker, defender);
            }

            return base.CheckHit(attacker, defender);
        }

        public override void OnHit(Mobile attacker, Mobile defender, double damageBonus)
        {
            if(attacker.Player && !defender.Player && (defender.Body.IsAnimal || defender.Body.IsMonster) && 0.4 >= Utility.RandomDouble())
                defender.AddToBackpack(Ammo);

            if(attacker.Player)
            {
                Marksman mm = Perk.GetByType<Marksman>((Player)attacker);
                if(mm != null)
                {
                    mm.WoundingShot(attacker, defender);
                }
            }

            base.OnHit(attacker, defender, damageBonus);
        }

        public override void OnMiss(Mobile attacker, Mobile defender)
        {
            if(attacker.Player && 0.4 >= Utility.RandomDouble())
            {
                if(Core.AOS)
                {
                    PlayerMobile p = attacker as PlayerMobile;

                    if(p != null)
                    {
                        Type ammo = AmmoType;

                        if(p.RecoverableAmmo.ContainsKey(ammo))
                            p.RecoverableAmmo[ammo]++;
                        else
                            p.RecoverableAmmo.Add(ammo, 1);

                        if(!p.Warmode)
                        {
                            if(m_RecoveryTimer == null)
                                m_RecoveryTimer = Timer.DelayCall(TimeSpan.FromSeconds(10), new TimerCallback(p.RecoverAmmo));

                            if(!m_RecoveryTimer.Running)
                                m_RecoveryTimer.Start();
                        }
                    }
                }
                else
                {
                    Ammo.MoveToWorld(new Point3D(defender.X + Utility.RandomMinMax(-1, 1), defender.Y + Utility.RandomMinMax(-1, 1), defender.Z), defender.Map);
                }
            }

            base.OnMiss(attacker, defender);
        }

        public virtual bool OnFired(Mobile attacker, Mobile defender)
        {
            Container pack = attacker.Backpack;
            Quiver quiver = attacker.FindItemOnLayer(Layer.Cloak) as Quiver;

            if(quiver != null && quiver.ConsumeTotal(AmmoType, 1))
            {
                quiver.UpdateTotals();
                quiver.InvalidateProperties();

                attacker.MovingEffect(defender, EffectID, 18, 1, false, false);
                return true;
            }
            else if(pack != null && pack.ConsumeTotal(AmmoType, 1))
            {
                attacker.MovingEffect(defender, EffectID, 18, 1, false, false);
                return true;
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch(version)
            {
                case 2:
                case 1:
                    {
                        break;
                    }
                case 0:
                    {
                        /*m_EffectID =*/
                        reader.ReadInt();
                        break;
                    }
            }

            if(version < 2)
            {
                WeaponAttributes.MageWeapon = 0;
                WeaponAttributes.UseBestSkill = 0;
            }
        }
    }
}