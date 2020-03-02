using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000475 RID: 1141
    public class Verb_LaunchProjectileEquipment : Verb_CastAbility
    {
        public VerbProperties_EquipmentAbility verbProperties
        {
            get
            {
                return (VerbProperties_EquipmentAbility)this.verbProps;
            }
        }
        public EquipmentAbility equipmentAbility => (EquipmentAbility)this.ability;

        public new ThingWithComps EquipmentSource
        {
            get
            {
                if (equipmentAbility != null)
                {
                    if (equipmentAbility.sourceEquipment != null)
                    {
                        return equipmentAbility.sourceEquipment;
                    }
                }
                return CasterPawn;
            }
        }
        // Token: 0x170006A9 RID: 1705
        // (get) Token: 0x06002199 RID: 8601 RVA: 0x000CBC8C File Offset: 0x000C9E8C
        public virtual ThingDef Projectile
        {
            get
            {
                if (this.EquipmentSource != null)
                {
                    CompChangeableProjectile comp = this.EquipmentSource.GetComp<CompChangeableProjectile>();
                    if (comp != null && comp.Loaded)
                    {
                        return comp.Projectile;
                    }
                }
                else
                {
                    Log.Message("EquipmentSource == null");
                }
                return this.verbProps.defaultProjectile;
            }
        }

        // Token: 0x0600219A RID: 8602 RVA: 0x000CBCCC File Offset: 0x000C9ECC
        public override void WarmupComplete()
        {
            base.WarmupComplete();
            Find.BattleLog.Add(new BattleLogEntry_RangedFire(this.caster, this.currentTarget.HasThing ? this.currentTarget.Thing : null, (this.EquipmentSource != null) ? this.EquipmentSource.def : null, this.Projectile, this.ShotsPerBurst > 1));
        }

        // Token: 0x0600219B RID: 8603 RVA: 0x000CBD34 File Offset: 0x000C9F34
        protected override bool TryCastShot()
        {
            if (this.currentTarget.HasThing && this.currentTarget.Thing.Map != this.caster.Map)
            {
                return false;
            }
            ThingDef projectile = this.Projectile;
            if (projectile == null)
            {
                return false;
            }
            ShootLine shootLine;
            bool flag = base.TryFindShootLineFromTo(this.caster.Position, this.currentTarget, out shootLine);
            if (this.verbProps.stopBurstWithoutLos && !flag)
            {
                return false;
            }
            if (this.EquipmentSource != null)
            {
                CompChangeableProjectile comp = this.EquipmentSource.GetComp<CompChangeableProjectile>();
                if (comp != null)
                {
                    comp.Notify_ProjectileLaunched();
                }
            }
            Thing launcher = this.caster;
            Thing equipment = this.EquipmentSource;
            CompMannable compMannable = this.caster.TryGetComp<CompMannable>();
            if (compMannable != null && compMannable.ManningPawn != null)
            {
                launcher = compMannable.ManningPawn;
                equipment = this.caster;
            }
            Vector3 drawPos = this.caster.DrawPos;
            Projectile projectile2 = (Projectile)GenSpawn.Spawn(projectile, shootLine.Source, this.caster.Map, WipeMode.Vanish);
            if (this.verbProps.forcedMissRadius > 0.5f)
            {
                float num = VerbUtility.CalculateAdjustedForcedMiss(this.verbProps.forcedMissRadius, this.currentTarget.Cell - this.caster.Position);
                if (num > 0.5f)
                {
                    int max = GenRadial.NumCellsInRadius(num);
                    int num2 = Rand.Range(0, max);
                    if (num2 > 0)
                    {
                        IntVec3 c = this.currentTarget.Cell + GenRadial.RadialPattern[num2];
                        this.ThrowDebugText("ToRadius");
                        this.ThrowDebugText("Rad\nDest", c);
                        ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.NonTargetWorld;
                        if (Rand.Chance(0.5f))
                        {
                            projectileHitFlags = ProjectileHitFlags.All;
                        }
                        if (!this.canHitNonTargetPawnsNow)
                        {
                            projectileHitFlags &= ~ProjectileHitFlags.NonTargetPawns;
                        }
                        projectile2.Launch(launcher, drawPos, c, this.currentTarget, projectileHitFlags, equipment, null);
                        return true;
                    }
                }
            }
            ShotReport shotReport = ShotReport.HitReportFor(this.caster, this, this.currentTarget);
            Thing randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
            ThingDef targetCoverDef = (randomCoverToMissInto != null) ? randomCoverToMissInto.def : null;
            if (!Rand.Chance(shotReport.AimOnTargetChance_IgnoringPosture))
            {
                shootLine.ChangeDestToMissWild(shotReport.AimOnTargetChance_StandardTarget);
                this.ThrowDebugText("ToWild" + (this.canHitNonTargetPawnsNow ? "\nchntp" : ""));
                this.ThrowDebugText("Wild\nDest", shootLine.Dest);
                ProjectileHitFlags projectileHitFlags2 = ProjectileHitFlags.NonTargetWorld;
                if (Rand.Chance(0.5f) && this.canHitNonTargetPawnsNow)
                {
                    projectileHitFlags2 |= ProjectileHitFlags.NonTargetPawns;
                }
                projectile2.Launch(launcher, drawPos, shootLine.Dest, this.currentTarget, projectileHitFlags2, equipment, targetCoverDef);
                return true;
            }
            if (this.currentTarget.Thing != null && this.currentTarget.Thing.def.category == ThingCategory.Pawn && !Rand.Chance(shotReport.PassCoverChance))
            {
                this.ThrowDebugText("ToCover" + (this.canHitNonTargetPawnsNow ? "\nchntp" : ""));
                this.ThrowDebugText("Cover\nDest", randomCoverToMissInto.Position);
                ProjectileHitFlags projectileHitFlags3 = ProjectileHitFlags.NonTargetWorld;
                if (this.canHitNonTargetPawnsNow)
                {
                    projectileHitFlags3 |= ProjectileHitFlags.NonTargetPawns;
                }
                projectile2.Launch(launcher, drawPos, randomCoverToMissInto, this.currentTarget, projectileHitFlags3, equipment, targetCoverDef);
                return true;
            }
            ProjectileHitFlags projectileHitFlags4 = ProjectileHitFlags.IntendedTarget;
            if (this.canHitNonTargetPawnsNow)
            {
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetPawns;
            }
            if (!this.currentTarget.HasThing || this.currentTarget.Thing.def.Fillage == FillCategory.Full)
            {
                projectileHitFlags4 |= ProjectileHitFlags.NonTargetWorld;
            }
            this.ThrowDebugText("ToHit" + (this.canHitNonTargetPawnsNow ? "\nchntp" : ""));
            if (this.currentTarget.Thing != null)
            {
                projectile2.Launch(launcher, drawPos, this.currentTarget, this.currentTarget, projectileHitFlags4, equipment, targetCoverDef);
                this.ThrowDebugText("Hit\nDest", this.currentTarget.Cell);
            }
            else
            {
                projectile2.Launch(launcher, drawPos, shootLine.Dest, this.currentTarget, projectileHitFlags4, equipment, targetCoverDef);
                this.ThrowDebugText("Hit\nDest", shootLine.Dest);
            }
            return true;
        }

        // Token: 0x0600219C RID: 8604 RVA: 0x000CC13B File Offset: 0x000CA33B
        private void ThrowDebugText(string text)
        {
            if (DebugViewSettings.drawShooting)
            {
                MoteMaker.ThrowText(this.caster.DrawPos, this.caster.Map, text, -1f);
            }
        }

        // Token: 0x0600219D RID: 8605 RVA: 0x000CC165 File Offset: 0x000CA365
        private void ThrowDebugText(string text, IntVec3 c)
        {
            if (DebugViewSettings.drawShooting)
            {
                MoteMaker.ThrowText(c.ToVector3Shifted(), this.caster.Map, text, -1f);
            }
        }

        // Token: 0x0600219E RID: 8606 RVA: 0x000CC18C File Offset: 0x000CA38C
        public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
        {
            needLOSToCenter = true;
            ThingDef projectile = this.Projectile;
            if (projectile == null)
            {
                return 0f;
            }
            return projectile.projectile.explosionRadius;
        }

        // Token: 0x0600219F RID: 8607 RVA: 0x000CC1B8 File Offset: 0x000CA3B8
        public override bool Available()
        {
            if (!base.Available())
            {
                return false;
            }
            if (this.CasterIsPawn)
            {
                Pawn casterPawn = this.CasterPawn;
                if (casterPawn.Faction != Faction.OfPlayer && casterPawn.mindState.MeleeThreatStillThreat && casterPawn.mindState.meleeThreat.Position.AdjacentTo8WayOrInside(casterPawn.Position))
                {
                    return false;
                }
            }
            return this.Projectile != null;
        }
    }
}
