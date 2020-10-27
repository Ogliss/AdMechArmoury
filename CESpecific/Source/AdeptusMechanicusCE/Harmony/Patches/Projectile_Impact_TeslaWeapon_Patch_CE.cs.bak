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

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(ProjectileCE), "Impact")]
    public static class Projectile_Impact_TeslaWeapon_Patch_CE
    {
        public static void Postfix(ref ProjectileCE __instance, ref Thing ___launcher, ref LocalTargetInfo ___intendedTarget, Thing hitThing)
        {
            if (hitThing !=null)
            {
                if (hitThing.def.thingClass == typeof(Pawn))
                {
                    Pawn hitPawn = (Pawn)hitThing;
                    if (hitPawn != null)
                    {
                        if (__instance.def.projectile.damageDef == DefDatabase<DamageDef>.GetNamed("OG_N_Tesla_Damage"))
                        {
                            //    Log.Message(string.Format("TeslaWeapon_launcher: {0} : Primary Fired: {1}", ___launcher.LabelShortCap, ((Pawn)___launcher).equipment.PrimaryEq.PrimaryVerb.GetProjectile()==__instance.def));
                            bool explode = true;// Rand.Chance(0.167f);
                            if (explode)
                            {
                            //    MoteMaker.ThrowText(hitPawn.Position.ToVector3(), hitPawn.Map, "AMA_Distorting_Shot".Translate(__instance.LabelCap, hitPawn.LabelShortCap), 3f);
                                Arc(__instance, ___launcher, hitPawn);
                            }
                        }
                        else
                        {
                        //    Log.Message(string.Format("damageDef not OG_E_Distortion_Damage: {0}", __instance.def.projectile.damageDef));
                        }
                    }
                    else
                    {
                    //    Log.Message(string.Format("hitPawn is null: {0}", hitThing.def.thingClass));
                    }
                }
                else
                {
                //    Log.Message(string.Format("Hitthing not pawn: {0}", hitThing.def.thingClass));
                }

            }
        }

        public static void Arc(ProjectileCE __instance, Thing ___launcher, Pawn hitPawn, float radius = 5f)
        {
            Map map = hitPawn.Map;
            if (hitPawn.Faction!=null)
            {
                if (Find.CurrentMap.mapPawns.AllPawns.Any(x => x.Position.InBounds(map) && x.Position.InHorDistOf(hitPawn.Position, radius) && x != hitPawn))
                {
                    IEnumerable<Pawn> pawns = Find.CurrentMap.mapPawns.AllPawns.Where(x => x.Position.InBounds(map) && x.Position.InHorDistOf(hitPawn.Position, radius) && x != hitPawn);
                    int t = Math.Min(pawns.Count(), 3);
                    List<Pawn> alreadyhit = new List<Pawn>();
                    for (int i = 0; i < t; i++)
                    {
                        Pawn target = pawns.Where(x=> !alreadyhit.Contains(x)).RandomElement();
                        if (target != null)
                        {
                            Projectile projectile = (Projectile)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("OGN_Bullet_TeslaCarbine_Arc"), null);
                            GenSpawn.Spawn(projectile, hitPawn.Position, hitPawn.Map, 0);
                            //    Log.Message(string.Format("Launch projectile2 {0} at {1}", projectile, OriginalPawn));
                            projectile.Launch(___launcher, hitPawn.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.Projectile), target, target, ProjectileHitFlags.All);
                            alreadyhit.Add(target);
                        }
                        else break;
                    }
                }
            }
            return;
        }
    }
}
