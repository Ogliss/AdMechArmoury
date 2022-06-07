using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using System.Reflection;
using UnityEngine;
using CombatExtended;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    //     Impact(Thing hitThing)
//    [HarmonyPatch(typeof(ProjectileCE), "Impact")] 
    public static class Projectile_Impact_Rending_Patch_CE
    {
        public static bool Prefix(ProjectileCE __instance, Thing ___launcher, IntVec3 ___originInt, Vector2 ___origin, Vector2 ___destinationInt, int ___startingTicksToImpactInt, int ___ticksToImpact, int ___intTicksToImpact, ThingDef ___equipmentDef, ref float ___suppressionAmount, Thing hitThing)
        {
            if (hitThing !=null)
            {
                bool Rending = false;
                float RendingChance = 0.167f;
                Pawn caster = ___launcher as Pawn;
                Thing Launcher = ___launcher;
                if (caster!=null)
                {
                    if (caster.equipment != null)
                    {
                        //    Log.Warning(string.Format("___launcher: {0}", ___launcher));
                        if (caster.equipment.Primary != null)
                        {
                            //    Log.Warning(string.Format("caster.equipment != null"));
                            Launcher = caster.equipment.Primary;
                            //    Log.Warning(string.Format("Launcher = caster.equipment.Primary"));
                            CompWeapon_GunSpecialRules _GunSpecialRules = Launcher.TryGetCompFast<CompWeapon_GunSpecialRules>();
                            if (_GunSpecialRules != null)
                            {
                                //    Log.Warning(string.Format("_GunSpecialRules != null"));
                                Rending = _GunSpecialRules.Rending;
                                RendingChance = _GunSpecialRules.RendingChance;
                            }
                        }
                        else
                        {
                            //    Log.Warning(string.Format("caster.equipment == null"));
                            if (caster.health.hediffSet.hediffs.Any(x => x.TryGetCompFast<HediffComp_VerbGiverExtra>() != null))
                            {
                                //    Log.Warning(string.Format("HediffComp_VerbGiverExtra: {0}", ___launcher));
                                HediffComp_VerbGiverExtra _VGE = caster.health.hediffSet.hediffs.Find(x => x.TryGetCompFast<HediffComp_VerbGiverExtra>() is HediffComp_VerbGiverExtra z && z.verbTracker.AllVerbs.Any(y => y.verbProps.defaultProjectile == __instance.def)).TryGetCompFast<HediffComp_VerbGiverExtra>();
                                if (_VGE != null)
                                {
                                    //    Log.Warning(string.Format("_VGE != null: {0}", _VGE.parent.LabelCap));
                                    GunVerbEntry entry = _VGE.Props.verbEntrys.Find(x => x.VerbProps.defaultProjectile == __instance.def);
                                    if (entry != null)
                                    {
                                        //    Log.Warning(string.Format("{0}, Rending: {1}, Chance: {2}", entry.VerbProps.label, entry.Rending, entry.RendingChance));
                                        Rending = entry.Rending;
                                        RendingChance = entry.RendingChance;
                                    }
                                }

                            }
                        }
                    }
                }
                if (Rending)
                {
                    //    Log.Warning(string.Format("Rending: {0}", ___launcher));
                    Rand.PushState();
                    bool RendingEffect = Rand.Chance(RendingChance);
                    Rand.PopState();
                    if (RendingEffect)
                    {
                        DamageDef damageDef = __instance.def.projectile.damageDef;
                        DamageArmorCategoryDef armorCategory = damageDef.armorCategory!=null ? damageDef.armorCategory: null;
                        StatDef armorcatdef = damageDef.armorCategory != null ? armorCategory.armorRatingStat : null;
                        float num = 0f;
                        float num2 = Mathf.Clamp01((armorcatdef!=null ? hitThing.GetStatValue(armorcatdef, true) : 0f) / 2f);
                        if (hitThing is Pawn hitPawn)
                        {
                            List<BodyPartRecord> allParts = hitPawn.RaceProps.body.AllParts;
                            List<Apparel> list = (hitPawn.apparel == null) ? null : hitPawn.apparel.WornApparel;
                            for (int i = 0; i < allParts.Count; i++)
                            {
                                float num3 = 1f - num2;
                                if (list != null)
                                {
                                    for (int j = 0; j < list.Count; j++)
                                    {
                                        if (list[j].def.apparel.CoversBodyPart(allParts[i]))
                                        {
                                            float num4 = Mathf.Clamp01((armorcatdef != null ? list[j].GetStatValue(armorcatdef, true) : 0f) / 2f);
                                            num3 *= 1f - num4;
                                        }
                                    }
                                }
                                num += allParts[i].coverageAbs * (1f - num3);
                            }
                        }
                        num = Mathf.Clamp(num * 2f, 0f, 2f);
                        float armorPenetration = num;

                        MoteMaker.ThrowText(hitThing.PositionHeld.ToVector3(), hitThing.MapHeld, "AdeptusMechanicus.Rending_Shot".Translate(__instance.LabelCap ,hitThing.LabelShortCap), 3f);
                        //    Log.Warning(string.Format("ArmorPenetration: {0}", ArmorPenetration));

                        bool flag = ___launcher is AmmoThing;
                        Map map = __instance.Map;
                        LogEntry_DamageResult logEntry_DamageResult = null;
                        bool flag2 = __instance.logMisses || (!__instance.logMisses && hitThing != null && (hitThing is Pawn || hitThing is Building_Turret));
                        if (flag2)
                        {
                            bool flag3 = !flag;
                            if (flag3)
                            {
                                LogImpact(__instance, ___launcher, ___equipmentDef, hitThing, out logEntry_DamageResult);
                            }
                        }
                        bool flag4 = hitThing != null;
                        if (flag4)
                        {
                            int damageAmount = __instance.def.projectile.GetDamageAmount(1f, null);
                            DamageDefExtensionCE damageDefExtensionCE = __instance.def.projectile.damageDef.GetModExtension<DamageDefExtensionCE>() ?? new DamageDefExtensionCE();
                            ProjectilePropertiesCE projectilePropertiesCE = (ProjectilePropertiesCE)__instance.def.projectile;
                        //    float armorPenetration = (this.def.projectile.damageDef.armorCategory == DamageArmorCategoryDefOf.Sharp) ? projectilePropertiesCE.armorPenetrationSharp : projectilePropertiesCE.armorPenetrationBlunt;
                            DamageInfo damageInfo = new DamageInfo(__instance.def.projectile.damageDef, (float)damageAmount, armorPenetration, __instance.ExactRotation.eulerAngles.y, ___launcher, null, __instance.def, DamageInfo.SourceCategory.ThingOrUnknown, null);
                            BodyPartDepth depth = (damageDefExtensionCE != null && damageDefExtensionCE.harmOnlyOutsideLayers) ? BodyPartDepth.Outside : BodyPartDepth.Undefined;
                            BodyPartHeight collisionBodyHeight = new CollisionVertical(hitThing).GetCollisionBodyHeight(__instance.ExactPosition.y);
                            damageInfo.SetBodyRegion(collisionBodyHeight, depth);
                            bool flag5 = damageDefExtensionCE != null && damageDefExtensionCE.harmOnlyOutsideLayers;
                            if (flag5)
                            {
                                damageInfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                            }
                            bool flag6 = flag && hitThing is Pawn;
                            if (flag6)
                            {
                                logEntry_DamageResult = new BattleLogEntry_DamageTaken((Pawn)hitThing, DefDatabase<RulePackDef>.GetNamed("DamageEvent_CookOff", true), null);
                                Find.BattleLog.Add(logEntry_DamageResult);
                            }
                            try
                            {
                                hitThing.TakeDamage(damageInfo).AssociateWithLog(logEntry_DamageResult);
                                bool flag7 = !(hitThing is Pawn) && projectilePropertiesCE != null && !projectilePropertiesCE.secondaryDamage.NullOrEmpty<SecondaryDamage>();
                                if (flag7)
                                {
                                    foreach (SecondaryDamage secondaryDamage in projectilePropertiesCE.secondaryDamage)
                                    {
                                        bool destroyed = hitThing.Destroyed;
                                        if (destroyed)
                                        {
                                            break;
                                        }
                                        DamageInfo dinfo = secondaryDamage.GetDinfo(damageInfo);
                                        hitThing.TakeDamage(dinfo).AssociateWithLog(logEntry_DamageResult);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error("CombatExtended :: BulletCE impacting thing " + hitThing.LabelCap + " of def " + hitThing.def.LabelCap + " added by mod " + hitThing.def.modContentPack.Name + ". See following stacktrace for information.", false);
                                throw ex;
                            }
                            finally
                            {
                                Impact(__instance, ___launcher, ___equipmentDef, hitThing, ___originInt, ___origin, ___destinationInt, ___startingTicksToImpactInt, ___ticksToImpact, ___intTicksToImpact, ref ___suppressionAmount);
                            }
                        }
                        else
                        {
                            SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(__instance.Position, map, false));
                            bool castShadow = __instance.castShadow;
                            if (castShadow)
                            {
                                FleckMaker.Static(__instance.ExactPosition, map, FleckDefOf.ShotHit_Dirt, 1f);
                                bool takeSplashes = __instance.Position.GetTerrain(map).takeSplashes;
                                if (takeSplashes)
                                {
                                    FleckMaker.WaterSplash(__instance.ExactPosition, map, Mathf.Sqrt((float)__instance.def.projectile.GetDamageAmount(___launcher, null)) * 1f, 4f);
                                }
                            }
                            Impact(__instance, ___launcher, ___equipmentDef, hitThing, ___originInt, ___origin, ___destinationInt, ___startingTicksToImpactInt, ___ticksToImpact, ___intTicksToImpact, ref ___suppressionAmount);
                        }
                        NotifyImpact(__instance, ___launcher, hitThing, map, __instance.Position);
                    }
                    return false;
                }
            }
            return true;
        }

        private static void Impact(ProjectileCE __instance, Thing launcher, ThingDef equipmentDef, Thing hitThing, IntVec3 OriginIV3, Vector2 origin, Vector2 Destination, int StartingTicksToImpact, int ticksToImpact, int IntTicksToImpact, ref float suppressionAmount)
        {
            List<Thing> list = new List<Thing>();
            Rand.PushState();
            if (__instance.Position.IsValid && __instance.def.projectile.preExplosionSpawnChance > 0f && __instance.def.projectile.preExplosionSpawnThingDef != null && (Controller.settings.EnableAmmoSystem || !(__instance.def.projectile.preExplosionSpawnThingDef is AmmoDef)) && Rand.Value < __instance.def.projectile.preExplosionSpawnChance)
            {
                ThingDef preExplosionSpawnThingDef = __instance.def.projectile.preExplosionSpawnThingDef;
                if (preExplosionSpawnThingDef.IsFilth && __instance.Position.Walkable(__instance.Map))
                {
                    FilthMaker.TryMakeFilth(__instance.Position, __instance.Map, preExplosionSpawnThingDef, 1, FilthSourceFlags.None);
                }
                else
                {
                    bool reuseNeolithicProjectiles = Controller.settings.ReuseNeolithicProjectiles;
                    if (reuseNeolithicProjectiles)
                    {
                        Thing thing = ThingMaker.MakeThing(preExplosionSpawnThingDef, null);
                        thing.stackCount = 1;
                        thing.SetForbidden(true, false);
                        GenPlace.TryPlaceThing(thing, __instance.Position, __instance.Map, ThingPlaceMode.Near, null, null, default(Rot4));
                        LessonAutoActivator.TeachOpportunity(CE_ConceptDefOf.CE_ReusableNeolithicProjectiles, thing, OpportunityType.GoodToKnow);
                        list.Add(thing);
                    }
                }
            }
            Rand.PopState();
            Vector3 vector = (hitThing != null) ? hitThing.DrawPos : __instance.ExactPosition;
            bool flag3 = !vector.ToIntVec3().IsValid;
            if (flag3)
            {
                __instance.Destroy(DestroyMode.Vanish);
            }
            else
            {
                CompExplosiveCE compExplosiveCE = __instance.TryGetCompFast<CompExplosiveCE>();
                if (compExplosiveCE == null)
                {
                    CompFragments compFragments = __instance.TryGetCompFast<CompFragments>();
                    if (compFragments != null)
                    {
                        compFragments.Throw(vector, __instance.Map, launcher, 1f);
                    }
                }
                if (compExplosiveCE != null || __instance.def.projectile.explosionRadius > 0f)
                {
                    if (hitThing is Pawn && (hitThing as Pawn).Dead)
                    {
                        list.Add((hitThing as Pawn).Corpse);
                    }
                    List<Pawn> list2 = new List<Pawn>();
                    float? direction = new float?(origin.AngleTo(Vec2Position(__instance, origin, Destination, StartingTicksToImpact, ticksToImpact, IntTicksToImpact, -1f)));
                    bool flag7 = __instance.def.projectile.explosionRadius > 0f;
                    if (flag7)
                    {
                        GenExplosionCE.DoExplosion(vector.ToIntVec3(), __instance.Map, __instance.def.projectile.explosionRadius, __instance.def.projectile.damageDef, launcher, __instance.def.projectile.GetDamageAmount(1f, null), __instance.def.projectile.GetExplosionArmorPenetration(), __instance.def.projectile.soundExplode, equipmentDef, __instance.def, null, __instance.def.projectile.postExplosionSpawnThingDef, __instance.def.projectile.postExplosionSpawnChance, __instance.def.projectile.postExplosionSpawnThingCount, __instance.def.projectile.applyDamageToExplosionCellsNeighbors, __instance.def.projectile.preExplosionSpawnThingDef, __instance.def.projectile.preExplosionSpawnChance, __instance.def.projectile.preExplosionSpawnThingCount, __instance.def.projectile.explosionChanceToStartFire, __instance.def.projectile.explosionDamageFalloff, direction, list, vector.y, 1f, false, null);
                        bool flag8 = vector.y < 3f;
                        if (flag8)
                        {
                            list2.AddRange(from x in GenRadial.RadialDistinctThingsAround(vector.ToIntVec3(), __instance.Map, 3f + __instance.def.projectile.explosionRadius, true)
                                           where x is Pawn
                                           select x as Pawn);
                        }
                    }
                    bool flag9 = compExplosiveCE != null;
                    if (flag9)
                    {
                        compExplosiveCE.Explode(__instance, vector, __instance.Map, 1f, direction, list);
                        bool flag10 = vector.y < 3f;
                        if (flag10)
                        {
                            list2.AddRange(from x in GenRadial.RadialDistinctThingsAround(vector.ToIntVec3(), __instance.Map, 3f + (compExplosiveCE.props as CompProperties_ExplosiveCE).explosiveRadius, true)
                                           where x is Pawn
                                           select x as Pawn);
                        }
                    }
                    foreach (Pawn pawn in list2)
                    {
                        ApplySuppression(__instance, OriginIV3, pawn, launcher, ref suppressionAmount);
                    }
                }
                __instance.Destroy(DestroyMode.Vanish);
            }
        }

        private static void LogImpact(ProjectileCE __instance, Thing launcher, ThingDef equipmentDef, Thing hitThing, out LogEntry_DamageResult logEntry)
        {
            logEntry = new BattleLogEntry_RangedImpact(launcher, hitThing, __instance.intendedTarget.Thing, equipmentDef, __instance.def, null);
            bool flag = !(launcher is AmmoThing);
            if (flag)
            {
                Find.BattleLog.Add(logEntry);
            }
        }
        private static void NotifyImpact(ProjectileCE __instance, Thing launcher, Thing hitThing, Map map, IntVec3 position)
        {
            Bullet bullet = GenerateVanillaBullet(__instance, launcher);
            BulletImpactData impactData = new BulletImpactData
            {
                bullet = bullet,
                hitThing = hitThing,
                impactPosition = position
            };
            bool flag = hitThing != null;
            if (flag)
            {
                hitThing.Notify_BulletImpactNearby(impactData);
            }
            int num = 9;
            for (int i = 0; i < num; i++)
            {
                IntVec3 c = position + GenRadial.RadialPattern[i];
                bool flag2 = c.InBounds(map);
                if (flag2)
                {
                    List<Thing> thingList = c.GetThingList(map);
                    for (int j = 0; j < thingList.Count; j++)
                    {
                        bool flag3 = thingList[j] != hitThing;
                        if (flag3)
                        {
                            thingList[j].Notify_BulletImpactNearby(impactData);
                        }
                    }
                }
            }
            bullet.Destroy(DestroyMode.Vanish);
        }
        private static Bullet GenerateVanillaBullet(ProjectileCE __instance, Thing launcher)
        {
            Bullet bullet = new Bullet
            {
                def = __instance.def,
                intendedTarget = __instance.intendedTarget
            };
            Traverse.Create(bullet).Field("launcher").SetValue(launcher);
            return bullet;
        }
        private static void ApplySuppression(ProjectileCE __instance, IntVec3 OriginIV3, Pawn pawn, Thing launcher, ref float suppressionAmount)
        {
            ShieldBelt shieldBelt = null;
            bool humanlike = pawn.RaceProps.Humanlike;
            if (humanlike)
            {
                List<Apparel> wornApparel = pawn.apparel.WornApparel;
                for (int i = 0; i < wornApparel.Count; i++)
                {
                    ShieldBelt shieldBelt2 = wornApparel[i] as ShieldBelt;
                    bool flag = shieldBelt2 != null;
                    if (flag)
                    {
                        shieldBelt = shieldBelt2;
                        break;
                    }
                }
            }
            CompSuppressable compSuppressable = pawn.TryGetCompFast<CompSuppressable>();
            bool flag2;
            if (compSuppressable != null)
            {
                Faction faction = pawn.Faction;
                Thing thing = launcher;
                if (faction != ((thing != null) ? thing.Faction : null))
                {
                    flag2 = (shieldBelt == null || shieldBelt.ShieldState == ShieldState.Resetting);
                    goto IL_93;
                }
            }
            flag2 = false;
            IL_93:
            bool flag3 = flag2;
            if (flag3)
            {
                suppressionAmount = (float)__instance.def.projectile.GetDamageAmount(1f, null);
                ProjectilePropertiesCE projectilePropertiesCE = __instance.def.projectile as ProjectilePropertiesCE;
                float num = (projectilePropertiesCE != null) ? projectilePropertiesCE.armorPenetrationSharp : 0f;
                float num2 = (num <= 0f) ? 0f : (1f - Mathf.Clamp(pawn.GetStatValue(CE_StatDefOf.AverageSharpArmor, true) * 0.5f / num, 0f, 1f));
                suppressionAmount *= num2;
                compSuppressable.AddSuppression(suppressionAmount, OriginIV3);
            }
        }
        private static int FlightTicks(int ticksToImpact, int IntTicksToImpact)
        {
            return IntTicksToImpact - ticksToImpact;
        }

        // Token: 0x1700008A RID: 138
        // (get) Token: 0x060002A2 RID: 674 RVA: 0x00019DE0 File Offset: 0x00017FE0
        private static float fTicks(int StartingTicksToImpact, int ticksToImpact, int intTicksToImpact)
        {
            return (ticksToImpact == 0) ? StartingTicksToImpact : ((float)FlightTicks(ticksToImpact, intTicksToImpact));
        }
        private static Vector2 Vec2Position(ProjectileCE __instance, Vector2 origin, Vector2 Destination, int StartingTicksToImpact, int ticksToImpact, int intTicksToImpact, float ticks = -1f)
        {
            bool flag = ticks < 0f;
            if (flag)
            {
                ticks = fTicks(StartingTicksToImpact, ticksToImpact, intTicksToImpact);
            }
            return Vector2.Lerp(origin, Destination, ticks / StartingTicksToImpact);
        }
    }

}
