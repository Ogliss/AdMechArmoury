using System;
using System.Collections.Generic;
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

namespace AdeptusMechanicus.Harmony
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatchesOG
    {
        //For alternating fire on some weapons
        public static Dictionary<Thing, int> AlternatingFireTracker = new Dictionary<Thing, int>();

        private static readonly Type patchType = typeof(HarmonyPatchesOG);
        static HarmonyPatchesOG()
        {
            var harmony = HarmonyInstance.Create("rimworld.ogliss.adeptusmechanicus.main");

            var type = typeof(HarmonyPatchesOG);
            /*
            harmony.Patch(
                AccessTools.Method(typeof(PawnGenerator), "GeneratePawn", new[] { typeof(PawnGenerationRequest) }), null,
                new HarmonyMethod(type, nameof(Post_GeneratePawn)));
                */
            /*
            harmony.Patch(
                AccessTools.Method(typeof(WorkGiver_HunterHunt), "HasHuntingWeapon", new[] { typeof(Pawn) }), null,
                new HarmonyMethod(type, nameof(Post_HasHuntingWeapon)));

            harmony.Patch(typeof(HediffSet).GetMethods(AccessTools.all).First((MethodInfo mi) => GenAttribute.HasAttribute<CompilerGeneratedAttribute>(mi) && mi.ReturnType == typeof(bool) && mi.GetParameters().First<ParameterInfo>().ParameterType == typeof(Pawn)), null, new HarmonyMethod(HarmonyPatches.patchType, "HasHuntingWeaponPostfix", null), null);

            */
            /*
            harmony.Patch(
                original: AccessTools.Method(type: typeof(FoodUtility), name: "AddFoodPoisoningHediff"),
                prefix: new HarmonyMethod(type: type, name: nameof(Pre_AddFoodPoisoningHediff_CompCheck)),
                postfix: null);
            */

            Type typeFromHandle3 = typeof(PawnRenderer);
            HarmonyPatchesOG.pawnField_PawnRenderer = typeFromHandle3.GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo method5 = typeFromHandle3.GetMethod("RenderPawnAt", new Type[]
            {
                typeof(Vector3),
                typeof(RotDrawMode),
                typeof(bool)
            });
            MethodInfo method6 = typeof(HarmonyPatchesOG).GetMethod("Patch_PawnRenderer_RenderPawnAt");
            harmony.Patch(method5, null, new HarmonyMethod(method6), null);
            /*
            harmony.Patch(AccessTools.Method(typeof(Pawn), nameof(Pawn.GetGizmos)), null,
                new HarmonyMethod(type, nameof(GetGizmos_PostFix)));
            */
        }

        /*
         // T magic patch for orders
        [HarmonyPriority(100)]
        public static void AddHumanLikeOrders_RestrictEquipmentPatch(Vector3 clickPos, Pawn pawn, ref List<FloatMenuOption> opts)
        {
            IntVec3 c = IntVec3.FromVector3(clickPos);
            bool flag = pawn.equipment != null;
            if (flag)
            {
                ThingWithComps thingWithComps = null;
                List<Thing> thingList = c.GetThingList(pawn.Map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    bool flag2 = thingList[i].def == TorannMagicDefOf.TM_Artifact_BracersOfThePacifist;
                    if (flag2)
                    {
                        thingWithComps = (ThingWithComps)thingList[i];
                        break;
                    }
                }
                bool flag3 = thingWithComps != null;
                if (flag3)
                {
                    string labelShort = thingWithComps.LabelShort;
                    bool flag4 = !pawn.story.WorkTagIsDisabled(WorkTags.Violent);
                    if (flag4)
                    {
                        for (int j = 0; j < opts.Count; j++)
                        {
                            bool flag5 = opts[j].Label.Contains("wear");
                            if (flag5)
                            {
                                opts.Remove(opts[j]);
                            }
                        }
                        FloatMenuOption item = new FloatMenuOption("TM_ViolentCannotEquip".Translate(pawn.LabelShort, labelShort), null, MenuOptionPriority.Default, null, null, 0f, null, null);
                        opts.Add(item);
                    }
                }
            }
        }
        */
        
        /*
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
        */

        public static class Patch
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
        /*
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
        */
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