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

namespace AdeptusMechanicus.HarmonyInstance
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        public static Dictionary<Thing, int> AlternatingFireTracker = new Dictionary<Thing, int>();
        public static List<ResearchProjectDef> ReseachAstartes => DefDatabase<ResearchProjectDef>.AllDefs.Where(x => x.defName.Contains("OG_Astartes_Tech_")).ToList();
        public static List<ResearchProjectDef> ReseachImperial => DefDatabase<ResearchProjectDef>.AllDefs.Where(x => x.defName.Contains("OG_Imperial_Tech_")).ToList();
        public static List<ResearchProjectDef> ReseachMechanicus => DefDatabase<ResearchProjectDef>.AllDefs.Where(x => x.defName.Contains("OG_Mechanicus_Tech_")).ToList();
        public static List<ResearchProjectDef> ReseachOrk => DefDatabase<ResearchProjectDef>.AllDefs.Where(x => x.defName.Contains("OG_Ork_Tech_")).ToList();
        public static List<ResearchProjectDef> ReseachEldar => DefDatabase<ResearchProjectDef>.AllDefs.Where(x => x.defName.Contains("OG_Eldar_Tech_")).ToList();
        public static List<ResearchProjectDef> ReseachDarkEldar => DefDatabase<ResearchProjectDef>.AllDefs.Where(x => x.defName.Contains("OG_DarkEldar_Tech_")).ToList();
        public static List<ResearchProjectDef> ReseachTau => DefDatabase<ResearchProjectDef>.AllDefs.Where(x => x.defName.Contains("OG_Tau_Tech_")).ToList();
        public static List<ResearchProjectDef> ReseachKroot => DefDatabase<ResearchProjectDef>.AllDefs.Where(x => x.defName.Contains("OG_Kroot_Tech_")).ToList();
        public static List<ResearchProjectDef> ReseachVespid => DefDatabase<ResearchProjectDef>.AllDefs.Where(x => x.defName.Contains("OG_Vespid_Tech_")).ToList();
        public static List<ResearchProjectDef> ReseachChaos => DefDatabase<ResearchProjectDef>.AllDefs.Where(x => x.defName.Contains("OG_Chaos_Tech_")).ToList();
        public static List<ResearchProjectDef> ReseachNecron => DefDatabase<ResearchProjectDef>.AllDefs.Where(x => x.defName.Contains("OG_Necron_Tech_")).ToList();
        public static List<ResearchProjectDef> ReseachTyranid => DefDatabase<ResearchProjectDef>.AllDefs.Where(x => x.defName.Contains("OG_Tyranid_Tech_")).ToList();
        static HarmonyPatches()
        {
            if (AdeptusIntergrationUtil.enabled_AlienRaces)
            {
                AlienRacesPatch();
            }

            /*
            if (AdeptusIntergrationUtil.enabled_CombatExtended)
            {
                AMAMod.harmony.Patch(AccessTools.Method(typeof(CombatExtended.Verb_MeleeAttackCE), "DamageInfosToApply", null, null), null, new HarmonyMethod(typeof(AM_Verb_MeleeAttackDamage_DamageInfosToApply_ForceWeapon_Patch).GetMethod("Postfix")));
                AMAMod.harmony.Patch(AccessTools.Method(typeof(CombatExtended.Verb_LaunchProjectileCE), "HighlightFieldRadiusAroundTarget", null, null), null, new HarmonyMethod(typeof(AM_Verb_Shoot_HighlightFieldRadiusAroundTarget_CustomExplosiveProjectile_Patch).GetMethod("Postfix")));
                AMAMod.harmony.Patch(AccessTools.Method(typeof(CombatExtended.Verb_ShootCE), "TryCastShot", null, null), null, new HarmonyMethod(typeof(AM_Verb_Shoot_HighlightFieldRadiusAroundTarget_CustomExplosiveProjectile_Patch).GetMethod("Postfix")));
            }
            */

            if (AccessTools.GetMethodNames(typeof(PawnGraphicSet)).Contains("HeadMatAt_NewTemp"))
            {
                HarmonyPatches.HeadMatAt_NewTemp();
            }
            else
            {
                HarmonyPatches.HeadMatAt();
            }

            if (AccessTools.GetMethodNames(typeof(PawnGraphicSet)).Contains("HairMatAt_NewTemp"))
            {
                HarmonyPatches.HairMatAt_NewTemp();
            }
            else
            {
                HarmonyPatches.HairMatAt();
            }

            if (AccessTools.GetMethodNames(typeof(EquipmentUtility)).Contains("CanEquip_NewTmp"))
            {
                HarmonyPatches.CanEquip_NewTmp();
            }
            else
            {
                HarmonyPatches.CanEquip();
            }
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
        }

        public static void CanEquip()
        {

            AMAMod.harmony.Patch(AccessTools.Method(typeof(EquipmentUtility), "CanEquip", new Type[]
            {
                typeof(Thing),
                typeof(Pawn),
                typeof(string).MakeByRefType()
            }, null), null, new HarmonyMethod(typeof(EquipmentUtility_CanEquip_Restricted_Patch).GetMethod("Postfix")));
        }

        public static void CanEquip_NewTmp()
        {
            AMAMod.harmony.Patch(AccessTools.Method(typeof(EquipmentUtility), "CanEquip_NewTmp", new Type[]
            {
                typeof(Thing),
                typeof(Pawn),
                typeof(string).MakeByRefType(),
                typeof(bool)
            }, null), null, new HarmonyMethod(typeof(EquipmentUtility_CanEquip_Restricted_Patch).GetMethod("Postfix")));
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
            var storyTrv = Traverse.Create(pawn.story);
            var newStory = new Pawn_StoryTracker(pawn);
            var newStoryTrv = Traverse.Create(newStory);
            AccessTools.GetFieldNames(typeof(Pawn_StoryTracker))
                    .ForEach(f => newStoryTrv.Field(f).SetValue(storyTrv.Field(f).GetValue()));
            newStory.bodyType = bt;
            pawn.story = newStory;
            pawn.Drawer.renderer.graphics.ResolveAllGraphics();
        }

        public static void AlienRacesPatch()
        {

            if (AdeptusIntergrationUtil.enabled_XenobiologisEldar)
            {
                AlienRace.ThingDef_AlienRace alien = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Alien_Eldar") as AlienRace.ThingDef_AlienRace;
                if (alien != null)
                {
                    HarmonyPatches.TryAddRacialRestrictions(alien, "E");
                    if (!AdeptusIntergrationUtil.enabled_XenobiologisDarkEldar)
                    {
                        HarmonyPatches.TryAddRacialRestrictions(alien, "DE");
                    }
                }
            }
            if (AdeptusIntergrationUtil.enabled_XenobiologisDarkEldar)
            {
                AlienRace.ThingDef_AlienRace alien = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Alien_DarkEldar") as AlienRace.ThingDef_AlienRace;
                if (alien != null)
                {
                    HarmonyPatches.TryAddRacialRestrictions(alien, "DE");
                    if (!AdeptusIntergrationUtil.enabled_XenobiologisEldar)
                    {
                        HarmonyPatches.TryAddRacialRestrictions(alien, "E");
                    }
                }
            }
            if (AdeptusIntergrationUtil.enabled_XenobiologisTau)
            {
                AlienRace.ThingDef_AlienRace alien = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Alien_Tau") as AlienRace.ThingDef_AlienRace;
                if (alien != null)
                {
                    HarmonyPatches.TryAddRacialRestrictions(alien, "T");
                    AddRecipies(alien, ReseachTau);
                }
                alien = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Alien_Kroot") as AlienRace.ThingDef_AlienRace;
                if (alien != null)
                {
                    HarmonyPatches.TryAddRacialRestrictions(alien, "T");
                    HarmonyPatches.TryAddRacialRestrictions(alien, "K");
                    AddRecipies(alien, ReseachKroot);
                }
                alien = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Alien_Vespid") as AlienRace.ThingDef_AlienRace;
                if (alien != null)
                {
                    HarmonyPatches.TryAddRacialRestrictions(alien, "T");
                    HarmonyPatches.TryAddRacialRestrictions(alien, "V");
                    AddRecipies(alien, ReseachVespid);
                }

            }
            if (AdeptusIntergrationUtil.enabled_XenobiologisOrk)
            {
                AlienRace.ThingDef_AlienRace alien = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Alien_Ork") as AlienRace.ThingDef_AlienRace;
                if (alien != null)
                {
                    HarmonyPatches.TryAddRacialRestrictions(alien, "O");
                }
                alien = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Alien_Grot") as AlienRace.ThingDef_AlienRace;
                if (alien != null)
                {
                    HarmonyPatches.TryAddRacialRestrictions(alien, "O");
                }

            }
            if (AdeptusIntergrationUtil.enabled_XenobiologisChaos)
            {
                AlienRace.ThingDef_AlienRace alien = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Alien_Chaos") as AlienRace.ThingDef_AlienRace;
                if (alien != null)
                {
                    HarmonyPatches.TryAddRacialRestrictions(alien, "C");
                }

            }
            if (AdeptusIntergrationUtil.enabled_XenobiologisNecron)
            {
                AlienRace.ThingDef_AlienRace alien = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Alien_Necron") as AlienRace.ThingDef_AlienRace;
                if (alien != null)
                {
                    HarmonyPatches.TryAddRacialRestrictions(alien, "N");
                }

            }
            if (AdeptusIntergrationUtil.enabled_XenobiologisTyranid)
            {
                AlienRace.ThingDef_AlienRace alien = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Alien_Tyranid") as AlienRace.ThingDef_AlienRace;
                if (alien != null)
                {
                    HarmonyPatches.TryAddRacialRestrictions(alien, "TY");
                }

            }
            AlienRace.ThingDef_AlienRace Human = DefDatabase<ThingDef>.GetNamedSilentFail("Human") as AlienRace.ThingDef_AlienRace;
            if (Human != null)
            {
                if (Prefs.DevMode) Log.Message("Adding Human restrictions");
                HarmonyPatches.TryAddRacialRestrictions(Human, "I");
                List<ResearchProjectDef> projectDefs = new List<ResearchProjectDef>();
                projectDefs.AddRange(ReseachImperial);
                AddRecipies(Human, ReseachImperial);
            }
            AlienRace.ThingDef_AlienRace Mechanicus = DefDatabase<ThingDef>.GetNamedSilentFail("OG_Human_Mechanicus") as AlienRace.ThingDef_AlienRace;
            if (Mechanicus != null)
            {
                if (Prefs.DevMode) Log.Message("Adding Mechanicus restrictions");
                HarmonyPatches.TryAddRacialRestrictions(Mechanicus, "I");
                HarmonyPatches.TryAddRacialRestrictions(Mechanicus, "AM");
                List<ResearchProjectDef> projectDefs = new List<ResearchProjectDef>();
                projectDefs.AddRange(ReseachImperial);
                projectDefs.AddRange(ReseachMechanicus);
                AddRecipies(Mechanicus, projectDefs);
            }
        }
        public static void TryAddRacialRestrictions(ThingDef race, string Tag)
        {
            List<RecipeDef> things = DefDatabase<RecipeDef>.AllDefsListForReading.FindAll(x => (x.defName.Contains("OG" + Tag + "_Gun_") || x.defName.Contains("OG" + Tag + "_Melee_") || x.defName.Contains("OG" + Tag + "_Apparel_") || x.defName.Contains("OG" + Tag + "_Wargear_") || x.defName.Contains("OG" + Tag + "_GrenadePack_")) && (!x.defName.Contains("TOGGLEDEF_") || x.defName.Contains("TOGGLEDEF_S")));

            foreach (RecipeDef def in things)
            {
                if (!AlienRace.RaceRestrictionSettings.recipeRestrictionDict.ContainsKey(key: def))
                {
                    AlienRace.RaceRestrictionSettings.recipeRestrictionDict.Add(key: def, value: new List<AlienRace.ThingDef_AlienRace>());
                }
                AlienRace.RaceRestrictionSettings.recipeRestrictionDict[key: def].Add(item: race as AlienRace.ThingDef_AlienRace);
            }
            List<ThingDef> Apparel = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(x => (x.defName.Contains("OG" + Tag + "_Apparel_") || x.defName.Contains("OG" + Tag + "_Wargear_") || x.defName.Contains("OG" + Tag + "_GrenadePack_")) && (!x.defName.Contains("TOGGLEDEF_") || x.defName.Contains("TOGGLEDEF_S")));
        }

        public static void AddRecipies(ThingDef race, List<ResearchProjectDef> researches)
        {

            foreach (ResearchProjectDef def in researches)
            {
                if (!AlienRace.RaceRestrictionSettings.researchRestrictionDict.ContainsKey(key: def))
                    AlienRace.RaceRestrictionSettings.researchRestrictionDict.Add(key: def, value: new List<AlienRace.ThingDef_AlienRace>());
                AlienRace.RaceRestrictionSettings.researchRestrictionDict[key: def].Add(item: race as AlienRace.ThingDef_AlienRace);
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

        // Token: 0x0600000A RID: 10 RVA: 0x000022B0 File Offset: 0x000004B0
        public static void PawnWeaponGenerator_TryGenerateWeaponFor_PostFix(Pawn pawn)
        {
            HugsLib.Settings.SettingHandle<int> chance = Traverse.Create(typeof(DualWield.Base)).Field("NPCDualWieldChance").GetValue<HugsLib.Settings.SettingHandle<int>>();
            bool alwaysDW = (pawn.kindDef.weaponTags!=null && pawn.kindDef.weaponTags.Contains("AlwaysDualWield"));
            bool flag = !pawn.RaceProps.Humanlike && pawn.RaceProps.ToolUser && pawn.RaceProps.FleshType != FleshTypeDefOf.Mechanoid && pawn.equipment != null && (Rand.Chance((float)chance / 100f) || alwaysDW);
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
                            if (w.thing.generateAllowChance >= 1f || Rand.ChanceSeeded(w.thing.generateAllowChance, pawn.thingIDNumber ^ (int)w.thing.shortHash ^ 28554824))
                            {
                                workingWeapons.Add(w);
                            }
                        }
                    }
                }
                if (workingWeapons.Count == 0)
                {
                    return;
                }
                ThingStuffPair thingStuffPair;
                IEnumerable<ThingStuffPair> matchingWeapons = workingWeapons.Where((ThingStuffPair tsp) =>
                tsp.thing.CanBeOffHand() &&
                !tsp.thing.IsTwoHand());
                if (matchingWeapons != null && matchingWeapons.TryRandomElementByWeight((ThingStuffPair w) => w.Commonality * w.Price, out thingStuffPair))
                {
                    ThingWithComps thingWithComps = (ThingWithComps)ThingMaker.MakeThing(thingStuffPair.thing, thingStuffPair.stuff);
                    PawnGenerator.PostProcessGeneratedGear(thingWithComps, pawn);
                    AddOffHandEquipment(pawn.equipment, thingWithComps);
                }

            }

        }
        
        public static void PatchPawnsArrivalModeWorker(Harmony harmonyInstance)
        {
            var prefix = typeof(PawnsArrivalModeWorker_EdgeWalkIn_Arrive_DSI_Patch).GetMethod("Prefix");
            var baseType = typeof(PawnsArrivalModeWorker);
            var types = baseType.AllSubclassesNonAbstract();
            foreach (Type cur in types)
            {
                if (cur != typeof(PawnsArrivalModeWorker_CenterDrop) && cur != typeof(PawnsArrivalModeWorker_EdgeDrop) && cur != typeof(PawnsArrivalModeWorker_EdgeDropGroups) && cur != typeof(PawnsArrivalModeWorker_RandomDrop))
                {
                    harmonyInstance.Patch(cur.GetMethod("Arrive"), new HarmonyMethod(prefix));
                }
            }
        }

    }

    public abstract class CompWearable : ThingComp
    {
        public virtual IEnumerable<Gizmo> CompGetGizmosWorn()
        {
            // return no Gizmos
            return new List<Gizmo>();
        }
    }

}