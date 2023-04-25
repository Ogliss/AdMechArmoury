using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using AdeptusMechanicus;
using DualWield;
using AdeptusMechanicus.settings;
using RimWorld.QuestGen;

namespace AdeptusMechanicus.HarmonyInstance
{
    [StaticConstructorOnStartup]
    public static class AdeptusHarmonyPatches
    {
        static AdeptusHarmonyPatches()
        {
            if (AdeptusIntergrationUtility.enabled_SOS2)
            {
                SOSConstructPatch();
            }
            MethodInfo checkFaction = null;
            foreach (var item in typeof(PreceptWorker_Apparel).GetMethods(BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (item.Name.Contains("CheckFaction"))
                {
                    checkFaction = item;
                }
            }
            if (checkFaction != null)
            {
                MethodInfo patch = typeof(PreceptWorker_Apparel_CanUse_FactionRoleApparel_Patch).GetMethod("CheckFaction");
                if (patch != null)
                {
                    AMAMod.harmony.Patch(checkFaction, null, new HarmonyMethod(patch));
                }
                else
                {
                    Log.Error("Couldnt find patch method");
                }
            }

            MethodInfo QuestGen_Pawns_ = AccessTools.TypeByName("RimWorld.QuestGen.QuestGen_Pawns").GetMethod("GeneratePawn", new Type[] { typeof(Quest), typeof(PawnKindDef), typeof(Faction), typeof(bool), typeof(IEnumerable<TraitDef>), typeof(float), typeof(bool), typeof(Pawn), typeof(float), typeof(float), typeof(bool), typeof(bool) });
            if (QuestGen_Pawns_ != null)
            {
                QuestGen_Pawns_GeneratePawn_Patch();
            }
            AdeptusHarmonyPatches.HeadMatAt();
            AdeptusHarmonyPatches.HairMatAt();
            AdeptusHarmonyPatches.CanEquip();

            MethodInfo ObserveSurroundingThings = AccessTools.TypeByName("RimWorld.PawnObserver").GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrFallback(x => x.GetParameters().Length == 1 && x.GetParameters().First().ParameterType == typeof(Region));
            if (ObserveSurroundingThings != null) AMAMod.harmony.Patch(original: ObserveSurroundingThings, transpiler: new HarmonyMethod(typeof(PawnObserver_ObserveSurroundingThings_CompilerGenerated_ObserveablePawn_Transpiler), nameof(PawnObserver_ObserveSurroundingThings_CompilerGenerated_ObserveablePawn_Transpiler.Transpiler)));
            else Log.Error($"ObserveSurroundingThings.Name NOT FOUND");

            MethodInfo PostProcessApparel_ = typeof(PawnApparelGenerator).GetMethod("PostProcessApparel", BindingFlags.Static | BindingFlags.Public);
            if (PostProcessApparel_ != null)
            {
            //    AMAMod.harmony.Patch(PostProcessApparel_, null, new HarmonyMethod(typeof(PawnApparelGenerator_PostProcessApparel_FactionColours_Patch), "Postfix", null), null, null);
            }
            else Log.Error($"PawnApparelGenerator.PostProcessApparel NOT FOUND");
            if (AdeptusIntergrationUtility.enabled_FacialStuff)
            {
            //    HarmonyPatches.FacialStuffPatches();
            }
            //            AMAMod.harmony.Patch(original: AccessTools.Constructor(typeof(Verse.PawnTextureAtlas)), transpiler: new HarmonyMethod(typeof(PawnTextureAtlas_Constructor_Name_Patch), nameof(PawnTextureAtlas_Constructor_Name_Patch.Transpiler)));
            /*
            MethodInfo targetmethod1 = AccessTools.TypeByName("ResearchProjectDef.ConfigErrors").GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Instance).First().
                GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).
                MaxBy(mi => mi.GetMethodBody()?.GetILAsByteArray().Length ?? -1);
            */
            /*
            if (AccessTools.GetMethodNames(typeof(PawnRenderer)).Contains("OverrideMaterialIfNeeded_NewTemp"))
            {
                HarmonyPatches.OverrideMaterialIfNeeded_NewTemp();
            }
            else
            {
                HarmonyPatches.OverrideMaterialIfNeeded();
            }
            */
            //    AMAMod.harmony.Patch(AccessTools.Method(typeof(PawnApparelGenerator), "GenerateStartingApparelFor", null, null), null, new HarmonyMethod(typeof(PawnApparelGenerator_GenerateStartingApparelFor_FactionColors_Patch), "Postfix", null), null, null);
        }

        public static void SOSConstructPatch()
        {
            //       AMAMod.harmony.Patch(typeof(SaveOurShip2.ShipInteriorMod2).GetMethod("HasSpaceSuitSlow", BindingFlags.NonPublic | BindingFlags.Instance), null, new HarmonyMethod(typeof(HarmonyPatches), nameof(SOSSpaceSuitPostfix_Flesh_Construct)));
        }

        public static void FacialStuffPatches()
        {
       //     Log.Message("FacialStuff detected: attempting to Patch Draw method");
            MethodInfo method = AccessTools.TypeByName("FacialStuff.HumanBipedDrawer").GetMethod("DrawApparel");
            MethodInfo method2 = null;// typeof(HumanBipedDrawer_DrawApparel_FacialStuff_Transpiler).GetMethod("Transpiler");
            bool flag = method == null;
            bool flag1 = method2 == null;
            if (flag)
            {
                Log.Error("HumanBipedDrawer Method is null");
            }
            else
            if (flag1)
            {
                Log.Error("Patch Method is null");
            }
            else
            {
                bool flag2 = AMAMod.harmony.Patch(method, null, null, new HarmonyMethod(method2)) == null;
                if (flag2)
                {
                    Log.Error("Adeptus Mechanicus: Facial Stuff patch failed.");
                }
            }
            //       AMAMod.harmony.Patch(typeof(SaveOurShip2.ShipInteriorMod2).GetMethod("HasSpaceSuitSlow", BindingFlags.NonPublic | BindingFlags.Instance), null, new HarmonyMethod(typeof(HarmonyPatches), nameof(SOSSpaceSuitPostfix_Flesh_Construct)));
        }

        private static void SOSSpaceSuitPostfix_Flesh_Construct(Pawn pawn, ref bool __result)
        {
            if (pawn.RaceProps.FleshType.defName.Contains("OG_Flesh_Construct"))
            {
                __result = true;
            }
        }
        public static void CanEquip()
        {
            MethodInfo v11 = AccessTools.Method(typeof(EquipmentUtility), "CanEquip", new Type[]
            {
                typeof(Thing),
                typeof(Pawn),
                typeof(string).MakeByRefType()
            });
            if (v11 != null)
            {
                AMAMod.harmony.Patch(v11, null, new HarmonyMethod(typeof(EquipmentUtility_CanEquip_Restricted_Patch).GetMethod("Postfix")));
            }
            MethodInfo v12 = AccessTools.Method(typeof(EquipmentUtility), "CanEquip_NewTmp", new Type[]
            {
                typeof(Thing),
                typeof(Pawn),
                typeof(string).MakeByRefType(),
                typeof(bool)
            });
            if (v12 != null)
            {
                AMAMod.harmony.Patch(v12, null, new HarmonyMethod(typeof(EquipmentUtility_CanEquip_Restricted_Patch).GetMethod("Postfix")));
            }
            MethodInfo v13 = AccessTools.Method(typeof(EquipmentUtility), "CanEquip", new Type[]
            {
                typeof(Thing),
                typeof(Pawn),
                typeof(string).MakeByRefType(),
                typeof(bool)
            });
            if (v13 != null)
            {
                AMAMod.harmony.Patch(v13, null, new HarmonyMethod(typeof(EquipmentUtility_CanEquip_Restricted_Patch).GetMethod("Postfix")));
            }
        }

        public static void QuestGen_Pawns_GeneratePawn_Patch()
        {
            AMAMod.harmony.Patch(AccessTools.Method(typeof(QuestGen_Pawns), "GeneratePawn", new Type[] { typeof(Quest), typeof(PawnKindDef), typeof(Faction), typeof(bool), typeof(IEnumerable<TraitDef>), typeof(float), typeof(bool), typeof(Pawn), typeof(float), typeof(float), typeof(bool), typeof(bool) }, null), new HarmonyMethod(typeof(QuestGen_Pawns_GeneratePawn_Refugee_Patch), "Prefix", null), null, null);
        }

        /*
        public static void OverrideMaterialIfNeeded()
        {
            AMAMod.harmony.Patch(AccessTools.Method(typeof(PawnRenderer), "OverrideMaterialIfNeeded", null, null), null, new HarmonyMethod(typeof(AvP_PawnRenderer_OverrideMaterialIfNeeded_Xenomorph_Patch), "Postfix", null), null, null);
        }

        public static void OverrideMaterialIfNeeded_NewTemp()
        {
            AMAMod.harmony.Patch(AccessTools.Method(typeof(PawnRenderer), "OverrideMaterialIfNeeded_NewTemp", null, null), null, new HarmonyMethod(typeof(AvP_PawnRenderer_OverrideMaterialIfNeeded_NewTemp_Xenomorph_Patch), "Postfix", null), null, null);
        }
        */
        public static void HairMatAt()
        {
            AMAMod.harmony.Patch(AccessTools.Method(typeof(PawnGraphicSet), "HairMatAt", null, null), null, new HarmonyMethod(typeof(PawnGraphicSet_HairMatAt_Test_Patch).GetMethod("Postfix"), Priority.Last), null, null);
        }

        public static void HairMatAt_NewTemp()
        {
            AMAMod.harmony.Patch(AccessTools.Method(typeof(PawnGraphicSet), "HairMatAt_NewTemp", null, null), null, new HarmonyMethod(typeof(PawnGraphicSet_HairMatAt_NewTemp_Test_Patch).GetMethod("Postfix"), Priority.Last), null, null);
        }
        public static void HeadMatAt()
        {
            AMAMod.harmony.Patch(AccessTools.Method(typeof(PawnGraphicSet), "HeadMatAt", null, null), null, new HarmonyMethod(typeof(PawnGraphicSet_HeadMatAt_Test_Patch).GetMethod("Postfix"), Priority.Last), null, null);
        }

        public static void HeadMatAt_NewTemp()
        {
            AMAMod.harmony.Patch(AccessTools.Method(typeof(PawnGraphicSet), "HeadMatAt_NewTemp", null, null), null, new HarmonyMethod(typeof(PawnGraphicSet_HeadMatAt_NewTemp_Test_Patch).GetMethod("Postfix"), Priority.Last), null, null);
        }

        public static void ChangeBodyType(Pawn pawn, BodyTypeDef bt)
        {
            pawn.story.bodyType = bt;
            IntVec3 pos = pawn.Position;
            Map map = pawn.Map;
            Building_Bed bed = null;
            Rot4 rot = pawn.Rotation;
            bool spawned = pawn.Map != null;
            bool selected = spawned ? Find.Selector.SelectedPawns.Contains(pawn) : false;
            bool drafted = pawn.Drafted;
            bool inBed = pawn.InBed();
            pawn.Drawer.renderer.graphics.SetAllGraphicsDirty();
        //    pawn.Drawer.renderer.graphics.ResolveAllGraphics();
            if (spawned)
            {
            //    pawn.DeSpawn();
            //    GenSpawn.Spawn(pawn, pos, map);
            //    pawn.Rotation = rot;
            }
            /*
            if (drafted)
            {
                pawn.drafter.Drafted = true;
            }
            if (selected)
            {
                Find.Selector.SelectedObjects.Add(pawn);
            }
            */
            if (inBed)
            {
                if (bed != null)
                {
                    pawn.jobs.Notify_TuckedIntoBed(bed);
                    pawn.mindState.Notify_TuckedIntoBed();
                }

            }
        }


        public static void AddOffHandEquipment(Pawn_EquipmentTracker instance, ThingWithComps newEq)
        {
            ThingOwner<ThingWithComps> value = Traverse.Create(instance).Field("equipment").GetValue<ThingOwner<ThingWithComps>>();
            DualWield.Storage.ExtendedDataStorage extendedDataStorage = Base.Instance.GetExtendedDataStorage();
            bool flag = extendedDataStorage != null;
            if (flag)
            {
                extendedDataStorage.GetExtendedDataFor(newEq).isOffHand = true;
                LessonAutoActivator.TeachOpportunity(DW_DefOff.DW_Penalties, 0);
                LessonAutoActivator.TeachOpportunity(DW_DefOff.DW_Settings, 0);
                value.TryAdd(newEq, true);
            }
        }

        public static void PawnWeaponGenerator_TryGenerateWeaponFor_PostFix(Pawn pawn)
        {
            HugsLib.Settings.SettingHandle<int> chance = Traverse.Create(typeof(DualWield.Base)).Field("NPCDualWieldChance").GetValue<HugsLib.Settings.SettingHandle<int>>();
            bool alwaysDW = (pawn.kindDef.weaponTags!=null && pawn.kindDef.weaponTags.Contains("AlwaysDualWield"));
            Rand.PushState();
            bool flag = !pawn.RaceProps.Humanlike && pawn.RaceProps.ToolUser && pawn.RaceProps.FleshType != FleshTypeDefOf.Mechanoid && pawn.equipment != null && (Rand.Chance((float)chance / 100f) || alwaysDW);
            Rand.PopState();
            if (flag)
            {

                float randomInRange = pawn.kindDef.weaponMoney.RandomInRange;
                List<ThingStuffPair> allWeaponPairs = Traverse.Create(typeof(PawnWeaponGenerator)).Field("allWeaponPairs").GetValue<List<ThingStuffPair>>();
                List<ThingStuffPair> workingWeapons = Traverse.Create(typeof(PawnWeaponGenerator)).Field("workingWeapons").GetValue<List<ThingStuffPair>>();
                if (pawn.equipment != null && pawn.equipment.Primary != null && pawn.equipment.Primary.def.IsTwoHand())
                {
                    return;
                }
                if (pawn.equipment == null || pawn.equipment.Primary == null)
                {
                    return;
                }
                for (int i = 0; i < allWeaponPairs.Count; i++)
                {
                    ThingStuffPair w = allWeaponPairs[i];
                    if (w.Price <= randomInRange)
                    {
                        if (pawn.kindDef.weaponTags == null || pawn.kindDef.weaponTags.Any((string tag) => w.thing.weaponTags.Contains(tag)))
                        {
                            Rand.PushState();
                            if (w.thing.generateAllowChance >= 1f || Rand.ChanceSeeded(w.thing.generateAllowChance, pawn.thingIDNumber ^ (int)w.thing.shortHash ^ 28554824))
                            {
                                workingWeapons.Add(w);
                            }
                            Rand.PopState();
                        }
                    }
                }
                if (workingWeapons.Count == 0)
                {
                    return;
                }
                IEnumerable<ThingStuffPair> matchingWeapons = workingWeapons.Where((ThingStuffPair tsp) =>
                tsp.thing.CanBeOffHand() &&
                !tsp.thing.IsTwoHand());
                if (matchingWeapons != null && matchingWeapons.TryRandomElementByWeight((ThingStuffPair w) => w.Commonality * w.Price, out ThingStuffPair thingStuffPair))
                {
                    ThingWithComps thingWithComps = (ThingWithComps)ThingMaker.MakeThing(thingStuffPair.thing, thingStuffPair.stuff);
                    PawnGenerator.PostProcessGeneratedGear(thingWithComps, pawn);
                    AddOffHandEquipment(pawn.equipment, thingWithComps);
                }

            }

        }
        
        public static void PatchPawnsArrivalModeWorker(Harmony harmonyInstance)
        {
            var prefix = typeof(PawnsArrivalModeWorker_EdgeWalkIn_Arrive_Reserves_Patch).GetMethod("Prefix");
            var baseType = typeof(PawnsArrivalModeWorker);
            var types = baseType.AllSubclassesNonAbstract();
            foreach (Type cur in types)
            {
                try
                {
                    if (cur.Name.Contains("PawnsArrivalModeWorker_Gauntlet"))
                    {
                        continue;
                    }
                    if (cur != typeof(PawnsArrivalModeWorker_CenterDrop) && cur != typeof(PawnsArrivalModeWorker_EdgeDrop) && cur != typeof(PawnsArrivalModeWorker_EdgeDropGroups) && cur != typeof(PawnsArrivalModeWorker_RandomDrop))
                    {
                        harmonyInstance.Patch(cur.GetMethod("Arrive"), new HarmonyMethod(prefix));
                    }
                }
                catch (Exception e)
                {
                    Log.Warning($"Deep Strike/Infiltrators patch on {cur} failed, throwing \n{e}");
                    continue;
                }
            }
        }

    }

}