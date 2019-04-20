using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using AbilityUser;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        //For alternating fire on some weapons
        public static Dictionary<Thing, int> AlternatingFireTracker = new Dictionary<Thing, int>();

        private static readonly Type patchType = typeof(HarmonyPatches);
        static HarmonyPatches()
        {
            var harmony = HarmonyInstance.Create("rimworld.ogliss.adeptusmechanicus.main");

            var type = typeof(HarmonyPatches);

            harmony.Patch(
                AccessTools.Method(typeof(PawnGenerator), "GeneratePawn", new[] { typeof(PawnGenerationRequest) }), null,
                new HarmonyMethod(type, nameof(Post_GeneratePawn)));
            /*
            harmony.Patch(
                AccessTools.Method(typeof(WorkGiver_HunterHunt), "HasHuntingWeapon", new[] { typeof(Pawn) }), null,
                new HarmonyMethod(type, nameof(Post_HasHuntingWeapon)));

            harmony.Patch(typeof(HediffSet).GetMethods(AccessTools.all).First((MethodInfo mi) => GenAttribute.HasAttribute<CompilerGeneratedAttribute>(mi) && mi.ReturnType == typeof(bool) && mi.GetParameters().First<ParameterInfo>().ParameterType == typeof(Pawn)), null, new HarmonyMethod(HarmonyPatches.patchType, "HasHuntingWeaponPostfix", null), null);

            */
            harmony.Patch(
                original: AccessTools.Method(type: typeof(FoodUtility), name: "AddFoodPoisoningHediff"),
                prefix: new HarmonyMethod(type: type, name: nameof(Pre_AddFoodPoisoningHediff_CompCheck)),
                postfix: null);


            Type typeFromHandle3 = typeof(PawnRenderer);
            HarmonyPatches.pawnField_PawnRenderer = typeFromHandle3.GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo method5 = typeFromHandle3.GetMethod("RenderPawnAt", new Type[]
            {
                typeof(Vector3),
                typeof(RotDrawMode),
                typeof(bool)
            });
            MethodInfo method6 = typeof(HarmonyPatches).GetMethod("Patch_PawnRenderer_RenderPawnAt");
            harmony.Patch(method5, null, new HarmonyMethod(method6), null);
        }

        public static Pawn PawnRenderer_GetPawn(object instance)
        {
            return (Pawn)HarmonyPatches.pawnField_PawnRenderer.GetValue(instance);
        }

        // Token: 0x0600000C RID: 12 RVA: 0x0000283C File Offset: 0x00000A3C
        public static void Patch_PawnRenderer_RenderPawnAt(PawnRenderer __instance, ref Vector3 drawLoc, ref RotDrawMode bodyDrawType, ref bool headStump)
        {
            Pawn pawn = HarmonyPatches.PawnRenderer_GetPawn(__instance);
            foreach (var hd in pawn.health.hediffSet.hediffs)
            {
                HediffComp_DrawImplant comp = hd.TryGetComp<HediffComp_DrawImplant>();
                if (comp != null)
                {
                    comp.DrawImplant();
                }
            }

        }

        internal static List<HediffComp_DrawImplant> implantDrawers(Pawn pawn)
        {
            List<HediffComp_DrawImplant> list = new List<HediffComp_DrawImplant>();
            for (int l = 0; l < pawn.health.hediffSet.hediffs.Count; l++)
            {
                HediffComp_DrawImplant drawer;
                if ((drawer = pawn.health.hediffSet.hediffs[l].TryGetComp<HediffComp_DrawImplant>()) != null)
                {
                    list.Add(drawer);
                }
            }
            return list;
        }

        public static bool Pre_AddFoodPoisoningHediff_CompCheck(Pawn pawn, Thing ingestible, FoodPoisonCause cause)
        {
        //    Log.Message(string.Format("checkin if {0} can get food poisioning from {1} because {2}", pawn.Name, ingestible, cause));
            CompFoodPoisonProtection compFood = pawn.TryGetComp<CompFoodPoisonProtection>();
            if (compFood!=null)
            {
                if (!compFood.Props.Poisonable)
                {
                //    Log.Message(string.Format("stopped {0} getting food poisioning from {1} because compFood.Props.Poisonable {2}", pawn.Name, ingestible, compFood.Props.Poisonable));
                    return false;
                }
                if (!compFood.Props.FoodTypeFlags.NullOrEmpty<FoodTypeFlags>())
                {
                    foreach (var ftf in compFood.Props.FoodTypeFlags)
                    {
                        if (ftf == ingestible.def.ingestible.foodType)
                        {
                        //    Log.Message(string.Format("stopped {0} getting food poisioning from {1} because {2}", pawn.Name, ingestible, ingestible.def.ingestible.foodType));
                            return false;
                        }
                    }
                }
                if (!compFood.Props.FoodPoisonCause.NullOrEmpty<FoodPoisonCause>())
                {
                    foreach (var fpc in compFood.Props.FoodPoisonCause)
                    {
                        if (fpc == cause)
                        {
                        //    Log.Message(string.Format("stopped {0} getting food poisioning from {1} because {2}", pawn.Name, ingestible, cause));
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        internal static class Patch
        {
            public static void ChangeBodyType(Pawn pawn, BodyTypeDef bt)
            {
                var storyTrv = Traverse.Create(pawn.story);
                var newStory = new Pawn_StoryTracker(pawn);
                var newStoryTrv = Traverse.Create(newStory);
                AccessTools.GetFieldNames(typeof(Pawn_StoryTracker))
                        .ForEach(f => newStoryTrv.Field(f).SetValue(storyTrv.Field(f).GetValue()));
                newStory.bodyType = bt;
                pawn.story = newStory;
                pawn.Drawer.renderer.graphics.ResolveAllGraphics();
            }
        }

        public static void Post_GeneratePawn(PawnGenerationRequest request, ref Pawn __result)
        {
            var hediffGiverSet = __result?.def?.race?.hediffGiverSets;
            if (hediffGiverSet == null) return;
            foreach (var item in hediffGiverSet)
            {
                var hediffGivers = item.hediffGivers;
                if (hediffGivers == null) return;
                if (hediffGivers.Any(y => y is HediffGiver_StartWithHediff))
                {
                    foreach (var hdg in hediffGivers.Where(x => x is HediffGiver_StartWithHediff))
                    {
                        HediffGiver_StartWithHediff hediffGiver_StartWith = (HediffGiver_StartWithHediff)hdg;
                        hediffGiver_StartWith.GiveHediff(__result);
                    }
                }
            }
        }
        /*
        public static void Post_HasHuntingWeapon(Pawn p, ref bool __result)
        {
            Log.Message(string.Format("p.equipment.Primary: {0}, p.equipment.Primary.def.IsRangedWeapon: {1}, p.equipment.PrimaryEq.PrimaryVerb.HarmsHealth(): {2}, !p.equipment.PrimaryEq.PrimaryVerb.UsesExplosiveProjectiles(): {3}", p.equipment.Primary, p.equipment.Primary.def.IsRangedWeapon, p.equipment.PrimaryEq.PrimaryVerb.HarmsHealth() , !p.equipment.PrimaryEq.PrimaryVerb.UsesExplosiveProjectiles()));
            
        }

        
        public static void HasHuntingWeaponPostfix(Pawn p, ref bool __result)
        {
            Log.Message(string.Format("p.equipment.Primary: {0}, p.equipment.Primary.def.IsRangedWeapon: {1}, p.equipment.PrimaryEq.PrimaryVerb.HarmsHealth(): {2}, !p.equipment.PrimaryEq.PrimaryVerb.UsesExplosiveProjectiles(): {3}", p.equipment.Primary, p.equipment.Primary.def.IsRangedWeapon, p.equipment.PrimaryEq.PrimaryVerb.HarmsHealth(), !p.equipment.PrimaryEq.PrimaryVerb.UsesExplosiveProjectiles()));
        //    __result = ((HarmonyPatches.headPawnDef != null) ? (x.def == HarmonyPatches.headPawnDef) : __result);
        }
        */
        private static FieldInfo pawnField_PawnRenderer;
    }
    
}