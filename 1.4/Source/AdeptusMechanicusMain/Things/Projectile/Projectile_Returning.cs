using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.ArcingBullet
    public class Projectile_Returning : Projectile, IThingHolder
    {
        public Projectile_Returning()
        {
            this.innerContainer = new ThingOwner<Thing>(this);
        }

        public override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Map map = base.Map;
            BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(this.launcher, hitThing, this.intendedTarget.Thing, this.equipmentDef, this.def, this.targetCoverDef);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            if (hitThing != null)
            {
                DamageDef damageDef = this.def.projectile.damageDef;
                float amount = (float)base.DamageAmount;
                float armorPenetration = base.ArmorPenetration;
                float y = this.ExactRotation.eulerAngles.y;
                Thing launcher = this.launcher;
                ThingDef equipmentDef = this.equipmentDef;
                DamageInfo dinfo = new DamageInfo(damageDef, amount, armorPenetration, y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
                hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                if (hitThing is Pawn pawn && pawn.stances != null && pawn.BodySize <= this.def.projectile.StoppingPower + 0.001f)
                {
                    pawn.stances.stagger.StaggerFor(95);
                }
            }
            else
            {
                SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(base.Position, map, false));
                FleckMaker.Static(this.ExactPosition, map, FleckDefOf.ShotHit_Dirt, 1f);
                if (base.Position.GetTerrain(map).takeSplashes)
                {
                    FleckMaker.WaterSplash(this.ExactPosition, map, Mathf.Sqrt((float)base.DamageAmount) * 1f, 4f);
                }
            }
            if (intendedTarget == launcherPawn)
            {
                if (launcherPawn.Dead || launcherPawn.Downed)
                {
                    this.innerContainer.TryDrop(equipment, ThingPlaceMode.Near, out Thing outThing);
                    outThing.SetForbidden(true);
                }
                else
                {
                    this.launcherPawn.equipment.equipment.TryAddOrTransfer(equipment);
                    if (equipment.def.soundInteract != null)
                    {
                        equipment.def.soundInteract.PlayOneShot(new TargetInfo(this.launcherPawn.Position, this.launcherPawn.Map, false));
                    }
                }
                this.DeSpawn();
            }
            else this.Bounce(this.DrawPos, launcherPawn, launcherPawn, ProjectileHitFlags.IntendedTarget, true, equipment);
        }


        public void Bounce(Vector3 origin, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, bool preventFriendlyFire = false, Thing equipment = null, ThingDef targetCoverDef = null)
        {
            this.origin = origin;
            this.usedTarget = usedTarget;
            this.intendedTarget = intendedTarget;
            this.targetCoverDef = targetCoverDef;
            this.preventFriendlyFire = preventFriendlyFire;
            this.HitFlags = hitFlags;
            if (equipment != null)
            {
                this.equipmentDef = equipment.def;
                this.weaponDamageMultiplier = equipment.GetStatValue(StatDefOf.RangedWeapon_DamageMultiplier, true);
            }
            else
            {
                this.equipmentDef = null;
                this.weaponDamageMultiplier = 1f;
            }
            this.destination = usedTarget.Cell.ToVector3Shifted() + Gen.RandomHorizontalVector(0.3f);
            this.ticksToImpact = Mathf.CeilToInt(this.StartingTicksToImpact);
            if (this.ticksToImpact < 1)
            {
                this.ticksToImpact = 1;
            }
            if (!this.def.projectile.soundAmbient.NullOrUndefined())
            {
                SoundInfo info = SoundInfo.InMap(this, MaintenanceType.PerTick);
                this.ambientSustainer = this.def.projectile.soundAmbient.TrySpawnSustainer(info);
            }
        }

        public override void Tick()
        {
            base.Tick();
        //    AnimateExtraBit();
            if (rotrate != 0)
            {
                this.rotinc += rotrate;
            }
            if (usedTarget.HasThing)
            {
                destination = usedTarget.Thing.DrawPos;
            }
        }

        public override void Draw()
        {
            float num = this.ArcHeightFactor * GenMath.InverseParabola(this.DistanceCoveredFraction);
            Vector3 drawPos = this.DrawPos;
            Vector3 position = drawPos + new Vector3(0f, 0f, 1f) * num;
        //    position += bitPosition;
            if (this.def.projectile.shadowSize > 0f)
            {
                this.DrawShadow(position, num);
            }
            Graphics.DrawMesh(MeshPool.GridPlane(this.def.graphicData.drawSize), position, this.ExactRotation, this.Graphic.MatSingleFor(this), 0);
       //     Graphics.DrawMesh(MeshPool.GridPlane(this.equipment.def.graphicData.drawSize), position + new Vector3(0f, 0.01f, 0f), this.ExtraRotation, equipment.Graphic.MatSingle, 0);
            base.Comps_PostDraw();
        }

        public override Quaternion ExactRotation
        {
            get
            {
                // Time.deltaTime

                return Quaternion.LookRotation((this.destination - this.origin).Yto0());
            }
        }
        public Quaternion ExtraRotation
        {
            get
            {
                return Quaternion.LookRotation((this.destination - this.origin).Yto0().RotatedBy(rotinc));
            }
        }

        public float animSpeed = 0.01f;
        private void AnimateExtraBit()
        {
            if (!Find.TickManager.Paused)
            {
                if (bitFloatingDown)
                {
                    if (this.bitOffsetZ < -Zlimit)
                    {
                        this.bitFloatingDown = false;
                    }
                    this.bitOffsetZ -= animSpeed * Find.TickManager.TickRateMultiplier;
                }
                else
                {
                    if (this.bitOffsetZ > Zlimit)
                    {
                        this.bitFloatingDown = true;
                    }
                    this.bitOffsetZ += animSpeed * Find.TickManager.TickRateMultiplier;
                }
                Rand.PushState();
                if (bitFloatingRight)
                {
                    if (this.bitOffsetX > Xlimit || Rand.Chance(Mathf.InverseLerp(0, Xlimit, this.bitOffsetX) * 0.1f))
                    {
                        this.bitFloatingRight = false;
                    }
                    this.bitOffsetX += animSpeed * Find.TickManager.TickRateMultiplier;
                }
                else
                {
                    if (this.bitOffsetX < -Xlimit || Rand.Chance(Mathf.InverseLerp(0, -Xlimit, this.bitOffsetX) * 0.1f))
                    {
                        this.bitFloatingRight = true;
                    }
                    this.bitOffsetX -= animSpeed * Find.TickManager.TickRateMultiplier;
                }
                Rand.PopState();
            }
            this.bitPosition = default;
            this.bitPosition.x += this.bitOffsetX;
            this.bitPosition.z += this.bitOffsetZ;
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return this.innerContainer;
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
        }

        public ThingOwner innerContainer;

        public Vector3 bitPosition = Vector3.zero;
        private bool bitFloatingDown = true;
        private bool bitFloatingRight = true;
        private float bitOffsetZ = 0.0f;
        private float bitOffsetX = 0.0f;
        private float Xlimit = 0.5f;
        private float Zlimit = 0.5f;
        public int rotinc = 0;
        public int rotrate = 0;
        public bool returnsSafely = false;
        public bool canBounce = false;
        public Pawn launcherPawn;
        public Thing equipment;
        public int timesBounced = 0;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.timesBounced, "timesBounced");
            Scribe_References.Look<Pawn>(ref this.launcherPawn, "OriginalPawnRef");
            Scribe_References.Look<Thing>(ref this.equipment, "OriginalWeaponRef");
            Scribe_Values.Look<int>(ref rotinc, "rotinc", 0);
            Scribe_Deep.Look<ThingOwner>(ref this.innerContainer, "innerContainer", new object[]
            {
                this
            });
        }

    }
}
