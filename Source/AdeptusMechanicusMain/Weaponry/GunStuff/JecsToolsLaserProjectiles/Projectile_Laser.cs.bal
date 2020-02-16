using System.Collections.Generic;
using AdeptusMechanicus.Harmony;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    public class Projectile_LaserConfig
    {
        public Vector3 offset;
    }


    // Token: 0x02000024 RID: 36
    public class Projectile_Laser : Projectile
    {
        // Token: 0x060000FB RID: 251 RVA: 0x00009248 File Offset: 0x00007448
        protected virtual void Explode(Thing hitThing, bool destroy = false)
        {
            Map map = base.Map;
            IntVec3 intVec = (hitThing != null) ? hitThing.PositionHeld : this.destination.ToIntVec3();
            if (destroy)
            {
                this.Destroy(DestroyMode.Vanish);
            }
            bool flag = this.def.projectile.explosionEffect != null;
            if (flag)
            {
                Effecter effecter = this.def.projectile.explosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(intVec, map, false), new TargetInfo(intVec, map, false));
                effecter.Cleanup();
            }
            IntVec3 center = intVec;
            Map map2 = map;
            float explosionRadius = this.def.projectile.explosionRadius;
            DamageDef damageDef = this.def.projectile.damageDef;
            Thing launcher = this.launcher;
            int damageAmount = this.def.projectile.GetDamageAmount(1f, null);
            SoundDef soundExplode = this.def.projectile.soundExplode;
            ThingDef equipmentDef = this.equipmentDef;
            ThingDef def = this.def;
            ThingDef postExplosionSpawnThingDef = this.def.projectile.postExplosionSpawnThingDef;
            float postExplosionSpawnChance = this.def.projectile.postExplosionSpawnChance;
            int postExplosionSpawnThingCount = this.def.projectile.postExplosionSpawnThingCount;
            ThingDef preExplosionSpawnThingDef = this.def.projectile.preExplosionSpawnThingDef;
            GenExplosion.DoExplosion(center, map2, explosionRadius, damageDef, launcher, damageAmount, 0f, soundExplode, equipmentDef, def, null, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, this.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, this.def.projectile.preExplosionSpawnChance, this.def.projectile.preExplosionSpawnThingCount, this.def.projectile.explosionChanceToStartFire, this.def.projectile.explosionDamageFalloff);
        }

        // Token: 0x060000FC RID: 252 RVA: 0x000093FB File Offset: 0x000075FB
        public override void SpawnSetup(Map map, bool blabla)
        {
            base.SpawnSetup(map, blabla);
            this.drawingTexture = this.def.DrawMatSingle;
        }

        // Token: 0x060000FD RID: 253 RVA: 0x00009418 File Offset: 0x00007618
        public void GetParametersFromXml()
        {
            ThingDef_LaserProjectile thingDef_LaserProjectile = this.def as ThingDef_LaserProjectile;
            this.preFiringDuration = thingDef_LaserProjectile.preFiringDuration;
            this.postFiringDuration = thingDef_LaserProjectile.postFiringDuration;
            this.preFiringInitialIntensity = thingDef_LaserProjectile.preFiringInitialIntensity;
            this.preFiringFinalIntensity = thingDef_LaserProjectile.preFiringFinalIntensity;
            this.postFiringInitialIntensity = thingDef_LaserProjectile.postFiringInitialIntensity;
            this.postFiringFinalIntensity = thingDef_LaserProjectile.postFiringFinalIntensity;
            this.startFireChance = thingDef_LaserProjectile.StartFireChance;
            this.canStartFire = thingDef_LaserProjectile.CanStartFire;
        }

        // Token: 0x060000FE RID: 254 RVA: 0x00009494 File Offset: 0x00007694
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.tickCounter, "tickCounter", 0, false);
            bool flag = Scribe.mode == LoadSaveMode.PostLoadInit;
            if (flag)
            {
                this.GetParametersFromXml();
            }
        }

        // Token: 0x060000FF RID: 255 RVA: 0x000094D4 File Offset: 0x000076D4
        public override void Tick()
        {
            try
            {
                bool flag = this.tickCounter == 0;
                if (flag)
                {
                    this.GetParametersFromXml();
                    this.PerformPreFiringTreatment();
                }
                bool flag2 = this.tickCounter < this.preFiringDuration;
                if (flag2)
                {
                    this.GetPreFiringDrawingParameters();
                }
                else
                {
                    bool flag3 = this.tickCounter == this.preFiringDuration;
                    if (flag3)
                    {
                        this.Fire();
                        this.GetPostFiringDrawingParameters();
                    }
                    else
                    {
                        this.GetPostFiringDrawingParameters();
                    }
                }
                bool flag4 = this.tickCounter == this.preFiringDuration + this.postFiringDuration && !base.Destroyed;
                if (flag4)
                {
                    this.Destroy(DestroyMode.Vanish);
                }
                bool flag5 = this.launcher != null;
                if (flag5)
                {
                    bool flag6 = this.launcher is Pawn;
                    if (flag6)
                    {
                        Pawn pawn = this.launcher as Pawn;
                        bool flag7 = pawn.Dead && !base.Destroyed;
                        if (flag7)
                        {
                            this.Destroy(DestroyMode.Vanish);
                        }
                    }
                }
                this.tickCounter++;
            }
            catch
            {
                this.Destroy(DestroyMode.Vanish);
            }
        }

        // Token: 0x06000100 RID: 256 RVA: 0x0000960C File Offset: 0x0000780C
        public virtual void PerformPreFiringTreatment()
        {
            this.DetermineImpactExactPosition();
            Vector3 a = (this.destination - this.origin).normalized * 0.9f;
            bool flag = this.Def.graphicSettings.NullOrEmpty<Projectile_LaserConfig>();
            if (flag)
            {
                Vector3 s = new Vector3(1f, 1f, (this.destination - this.origin).magnitude - a.magnitude);
                Vector3 pos = this.origin + a / 2f + (this.destination - this.origin) / 2f + Vector3.up * this.def.Altitude;
                this.drawingMatrix = new List<Matrix4x4>();
                Matrix4x4 item = default(Matrix4x4);
                item.SetTRS(pos, this.ExactRotation, s);
                this.drawingMatrix.Add(item);
            }
            else
            {
                this.drawingMatrix = new List<Matrix4x4>();
                bool flag2 = !this.Def.cycleThroughFiringPositions;
                if (flag2)
                {
                    foreach (Projectile_LaserConfig setting in this.Def.graphicSettings)
                    {
                        this.AddLaserGraphicUsing(setting);
                    }
                }
                else
                {
                    int num = 0;
                    bool flag3 = HarmonyPatchesOG.AlternatingFireTracker.ContainsKey(this.launcher);
                    if (flag3)
                    {
                        num = (HarmonyPatchesOG.AlternatingFireTracker[this.launcher] + 1) % this.Def.graphicSettings.Count;
                        HarmonyPatchesOG.AlternatingFireTracker[this.launcher] = num;
                    }
                    else
                    {
                        HarmonyPatchesOG.AlternatingFireTracker.Add(this.launcher, num);
                    }
                    this.AddLaserGraphicUsing(this.Def.graphicSettings[num]);
                }
            }
        }

        // Token: 0x06000101 RID: 257 RVA: 0x0000981C File Offset: 0x00007A1C
        private void AddLaserGraphicUsing(Projectile_LaserConfig setting)
        {
            Vector3 a = (this.destination - this.origin).normalized * 0.9f;
            Vector3 s = new Vector3(1f, 1f, (this.destination - this.origin).magnitude - a.magnitude);
            Vector3 vector = this.origin + a / 2f + (this.destination - this.origin) / 2f + Vector3.up * this.def.Altitude;
            float angle = 0f;
            bool flag = (this.destination - this.origin).MagnitudeHorizontalSquared() > 0.001f;
            if (flag)
            {
                angle = (this.destination - this.origin).AngleFlat();
            }
            vector += setting.offset.RotatedBy(angle);
            Matrix4x4 item = default(Matrix4x4);
            item.SetTRS(vector, this.ExactRotation, s);
            this.drawingMatrix.Add(item);
        }

        // Token: 0x17000025 RID: 37
        // (get) Token: 0x06000102 RID: 258 RVA: 0x0000994E File Offset: 0x00007B4E
        public ThingDef_LaserProjectile Def
        {
            get
            {
                return this.def as ThingDef_LaserProjectile;
            }
        }

        // Token: 0x06000103 RID: 259 RVA: 0x0000995C File Offset: 0x00007B5C
        public virtual void GetPreFiringDrawingParameters()
        {
            bool flag = this.preFiringDuration != 0;
            if (flag)
            {
                this.drawingIntensity = this.preFiringInitialIntensity + (this.preFiringFinalIntensity - this.preFiringInitialIntensity) * (float)this.tickCounter / (float)this.preFiringDuration;
            }
        }

        // Token: 0x06000104 RID: 260 RVA: 0x000099A4 File Offset: 0x00007BA4
        public virtual void GetPostFiringDrawingParameters()
        {
            bool flag = this.postFiringDuration != 0;
            if (flag)
            {
                this.drawingIntensity = this.postFiringInitialIntensity + (this.postFiringFinalIntensity - this.postFiringInitialIntensity) * (((float)this.tickCounter - (float)this.preFiringDuration) / (float)this.postFiringDuration);
            }
        }

        // Token: 0x06000105 RID: 261 RVA: 0x000099F4 File Offset: 0x00007BF4
        protected void DetermineImpactExactPosition()
        {
            Vector3 a = this.destination - this.origin;
            int num = (int)a.magnitude;
            Vector3 b = a / a.magnitude;
            Vector3 destination = this.origin;
            Vector3 vector = this.origin;
            IntVec3 intVec = vector.ToIntVec3();
            for (int i = 1; i <= num; i++)
            {
                vector += b;
                intVec = vector.ToIntVec3();
                bool flag = !vector.InBounds(base.Map);
                if (flag)
                {
                    this.destination = destination;
                    break;
                }
                bool flag2 = !this.def.projectile.flyOverhead && i >= 5;
                if (flag2)
                {
                    List<Thing> list = base.Map.thingGrid.ThingsListAt(base.Position);
                    for (int j = 0; j < list.Count; j++)
                    {
                        Thing thing = list[j];
                        bool flag3 = thing.def.Fillage == FillCategory.Full;
                        if (flag3)
                        {
                            this.destination = intVec.ToVector3Shifted() + new Vector3(Rand.Range(-0.3f, 0.3f), 0f, Rand.Range(-0.3f, 0.3f));
                            this.hitThing = thing;
                            break;
                        }
                        bool flag4 = thing.def.category == ThingCategory.Pawn;
                        if (flag4)
                        {
                            Pawn pawn = thing as Pawn;
                            float num2 = 0.45f;
                            bool downed = pawn.Downed;
                            if (downed)
                            {
                                num2 *= 0.1f;
                            }
                            float num3 = (this.ExactPosition - this.origin).MagnitudeHorizontal();
                            bool flag5 = num3 < 4f;
                            if (flag5)
                            {
                                num2 *= 0f;
                            }
                            else
                            {
                                bool flag6 = num3 < 7f;
                                if (flag6)
                                {
                                    num2 *= 0.5f;
                                }
                                else
                                {
                                    bool flag7 = num3 < 10f;
                                    if (flag7)
                                    {
                                        num2 *= 0.75f;
                                    }
                                }
                            }
                            num2 *= pawn.RaceProps.baseBodySize;
                            bool flag8 = Rand.Value < num2;
                            if (flag8)
                            {
                                this.destination = intVec.ToVector3Shifted() + new Vector3(Rand.Range(-0.3f, 0.3f), 0f, Rand.Range(-0.3f, 0.3f));
                                this.hitThing = pawn;
                                break;
                            }
                        }
                    }
                }
                destination = vector;
            }
        }

        // Token: 0x06000106 RID: 262 RVA: 0x00009C82 File Offset: 0x00007E82
        public virtual void Fire()
        {
            this.ApplyDamage(this.hitThing);
        }

        // Token: 0x06000107 RID: 263 RVA: 0x00009C94 File Offset: 0x00007E94
        protected void ApplyDamage(Thing hitThing)
        {
            bool flag = hitThing != null;
            if (flag)
            {
                this.Impact(hitThing);
            }
            else
            {
                this.ImpactSomething();
            }
        }

        // Token: 0x06000108 RID: 264 RVA: 0x00009CC0 File Offset: 0x00007EC0
        protected void ImpactSomething()
        {
            bool flyOverhead = this.def.projectile.flyOverhead;
            if (flyOverhead)
            {
                RoofDef roofDef = base.Map.roofGrid.RoofAt(base.DestinationCell);
                bool flag = roofDef != null && roofDef.isThickRoof;
                if (flag)
                {
                    SoundInfo info = SoundInfo.InMap(new TargetInfo(base.DestinationCell, base.Map, false), MaintenanceType.None);
                    this.def.projectile.soundHitThickRoof.PlayOneShot(info);
                    return;
                }
            }
            bool flag2 = this.usedTarget != null;
            if (flag2)
            {
                Pawn pawn = this.usedTarget.Thing as Pawn;
                bool flag3 = pawn != null && pawn.Downed && (this.origin - this.destination).magnitude > 5f && Rand.Value < 0.2f;
                if (flag3)
                {
                    this.Impact(null);
                }
                else
                {
                    this.Impact(this.usedTarget.Thing);
                }
            }
            else
            {
                Thing thing = base.Map.thingGrid.ThingAt(base.DestinationCell, ThingCategory.Pawn);
                bool flag4 = thing != null;
                if (flag4)
                {
                    this.Impact(thing);
                }
                else
                {
                    foreach (Thing thing2 in base.Map.thingGrid.ThingsAt(base.DestinationCell))
                    {
                        bool flag5 = thing2.def.fillPercent > 0f || thing2.def.passability > Traversability.Standable;
                        if (flag5)
                        {
                            this.Impact(thing2);
                            return;
                        }
                    }
                    this.Impact(null);
                }
            }
        }

        // Token: 0x06000109 RID: 265 RVA: 0x00009E9C File Offset: 0x0000809C
        protected override void Impact(Thing hitThing)
        {
            bool createsExplosion = this.Def.createsExplosion;
            if (createsExplosion)
            {
                this.Explode(hitThing, false);
                GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, this.def.projectile.damageDef, this.launcher.Faction);
            }
            bool flag = hitThing != null;
            if (flag)
            {
                Map map = base.Map;
                BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(this.launcher, hitThing, this.intendedTarget.Thing, this.equipmentDef, this.def, this.targetCoverDef);
                Find.BattleLog.Add(battleLogEntry_RangedImpact);
                int damageAmount = this.def.projectile.GetDamageAmount(1f, null);
                DamageDef damageDef = this.def.projectile.damageDef;
                int num = damageAmount;
                float y = this.ExactRotation.eulerAngles.y;
                Thing launcher = this.launcher;
                ThingDef equipmentDef = this.equipmentDef;
                DamageInfo dinfo = new DamageInfo(damageDef, (float)num, this.def.projectile.GetArmorPenetration(1f, null), y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, null);
                hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                bool flag2 = this.canStartFire && Rand.Range(0f, 1f) > this.startFireChance;
                if (flag2)
                {
                    hitThing.TryAttachFire(0.05f);
                }
                Pawn pawn = hitThing as Pawn;
                bool flag3 = pawn != null;
                if (flag3)
                {
                    this.PostImpactEffects(this.launcher as Pawn, pawn);
                    MoteMaker.ThrowMicroSparks(this.destination, base.Map);
                    MoteMaker.MakeStaticMote(this.destination, base.Map, ThingDefOf.Mote_ShotHit_Dirt, 1f);
                }
            }
            else
            {
                SoundInfo info = SoundInfo.InMap(new TargetInfo(base.Position, base.Map, false), MaintenanceType.None);
                SoundDefOf.BulletImpact_Ground.PlayOneShot(info);
                MoteMaker.MakeStaticMote(this.ExactPosition, base.Map, ThingDefOf.Mote_ShotHit_Dirt, 1f);
                MoteMaker.ThrowMicroSparks(this.ExactPosition, base.Map);
            }
        }

        // Token: 0x0600010A RID: 266 RVA: 0x00003BD7 File Offset: 0x00001DD7
        public virtual void PostImpactEffects(Pawn launcher, Pawn hitTarget)
        {
        }

        // Token: 0x0600010B RID: 267 RVA: 0x0000A0A4 File Offset: 0x000082A4
        public override void Draw()
        {
            base.Comps_PostDraw();
            bool flag = !this.drawingMatrix.NullOrEmpty<Matrix4x4>();
            if (flag)
            {
                foreach (Matrix4x4 matrix in this.drawingMatrix)
                {
                    Graphics.DrawMesh(MeshPool.plane10, matrix, FadedMaterialPool.FadedVersionOf(this.drawingTexture, this.drawingIntensity), 0);
                }
            }
        }

        // Token: 0x040000B7 RID: 183
        public int tickCounter = 0;

        // Token: 0x040000B8 RID: 184
        public Thing hitThing = null;

        // Token: 0x040000B9 RID: 185
        public float preFiringInitialIntensity = 0f;

        // Token: 0x040000BA RID: 186
        public float preFiringFinalIntensity = 0f;

        // Token: 0x040000BB RID: 187
        public float postFiringInitialIntensity = 0f;

        // Token: 0x040000BC RID: 188
        public float postFiringFinalIntensity = 0f;

        // Token: 0x040000BD RID: 189
        public int preFiringDuration = 0;

        // Token: 0x040000BE RID: 190
        public int postFiringDuration = 0;

        // Token: 0x040000BF RID: 191
        public float startFireChance = 0f;

        // Token: 0x040000C0 RID: 192
        public bool canStartFire = false;

        // Token: 0x040000C1 RID: 193
        public Material preFiringTexture;

        // Token: 0x040000C2 RID: 194
        public Material postFiringTexture;

        // Token: 0x040000C3 RID: 195
        public List<Matrix4x4> drawingMatrix = null;

        // Token: 0x040000C4 RID: 196
        public float drawingIntensity = 0f;

        // Token: 0x040000C5 RID: 197
        public Material drawingTexture;

        // Token: 0x040000C6 RID: 198
        private int ticksToDetonation;
    }
}