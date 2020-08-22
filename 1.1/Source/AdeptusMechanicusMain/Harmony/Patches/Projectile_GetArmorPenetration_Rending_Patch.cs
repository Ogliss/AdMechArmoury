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

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Projectile), "get_ArmorPenetration")]
    public static class Projectile_GetArmorPenetration_Rending_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Projectile __instance, ref LocalTargetInfo ___usedTarget, ref Thing ___launcher, ref float __result)
        {
            if (___usedTarget.HasThing)
            {
                bool Rending = false;
                float RendingChance = 0.167f;
                Pawn caster = ___launcher as Pawn;
                Thing hitThing = ___usedTarget.Thing;
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
                            CompWeapon_GunSpecialRules _GunSpecialRules = Launcher.TryGetComp<CompWeapon_GunSpecialRules>();
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
                            if (caster.health.hediffSet.hediffs.Any(x => x.TryGetComp<HediffComp_VerbGiverExtra>() != null))
                            {
                                //    Log.Warning(string.Format("HediffComp_VerbGiverExtra: {0}", ___launcher));
                                HediffComp_VerbGiverExtra _VGE = caster.health.hediffSet.hediffs.Find(x => x.TryGetComp<HediffComp_VerbGiverExtra>() is HediffComp_VerbGiverExtra z && z.verbTracker.AllVerbs.Any(y => y.verbProps.defaultProjectile == __instance.def)).TryGetComp<HediffComp_VerbGiverExtra>();
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
                    bool RendingEffect = Rand.Chance(RendingChance);
                    if (RendingEffect)
                    {
                    //    Log.Warning(string.Format("RendingEffect: {0}", ___launcher));
                        DamageDef damageDef = __instance.def.projectile.damageDef;
                    //    Log.Warning(string.Format("damageDef: {0}", damageDef.LabelCap));
                        DamageArmorCategoryDef armorCategory = damageDef.armorCategory!=null ? damageDef.armorCategory: null;
                    //    Log.Warning(string.Format("armorCategory: {0}", armorCategory));
                        StatDef armorcatdef = damageDef.armorCategory != null ? armorCategory.armorRatingStat : null;
                    //    Log.Warning(string.Format("armorcatdef: {0}", armorcatdef));
                        float num = 0f;
                        float num2 = Mathf.Clamp01((armorcatdef!=null ? hitThing.GetStatValue(armorcatdef, true) : 0f) / 2f);
                    //    Log.Warning(string.Format("num2: {0}", num2));
                        if (hitThing is Pawn hitPawn)
                        {
                            List<BodyPartRecord> allParts = hitPawn.RaceProps.body.AllParts;
                        //    Log.Warning(string.Format("allParts: {0}", allParts.Count));
                            List<Apparel> list = (hitPawn.apparel == null) ? null : hitPawn.apparel.WornApparel;
                        //    Log.Warning(string.Format("list: {0}", list.Count));
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
                        float ArmorPenetration = num;

                        MoteMaker.ThrowText(hitThing.PositionHeld.ToVector3(), hitThing.MapHeld, "AMA_Rending_Shot".Translate(__instance.LabelCap ,hitThing.LabelShortCap), 3f);
                        //    Log.Warning(string.Format("ArmorPenetration: {0}", ArmorPenetration));
                        __result = ArmorPenetration;
                    }
                }
            }
        }
    }

}
