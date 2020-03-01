using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using AdeptusMechanicus;

namespace AdeptusMechanicus.HarmonyInstance
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = new Harmony("com.ogliss.rimworld.mod.AdeptusMechanicus");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            PatchPawnsArrivalModeWorker(harmony);
            if (AdeptusIntergrationUtil.enabled_rooloDualWield)
            {
                /*
                harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("CompOversizedWeapon.HarmonyCompOversizedWeapon", "CompOversizedWeapon"), "DrawEquipmentAimingPreFix", null, null), new HarmonyMethod(Main.patchType, "DrawEquipmentAiming_DualWield_OverSized_PreFix", null), new HarmonyMethod(Main.patchType, "DrawEquipmentAiming_DualWield_OverSized_PostFix", null));
                harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("CompActivatableEffect.HarmonyCompActivatableEffect", "CompActivatableEffect"), "DrawEquipmentAimingPostFix", null, null), new HarmonyMethod(Main.patchType, "DrawEquipmentAiming_DualWield_Activatable_PreFix", null));
                harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("DualWield.Harmony.PawnRenderer_DrawEquipmentAiming", "DualWield.Harmony"), "DrawEquipmentAimingOverride", null, null), new HarmonyMethod(Main.patchType, "DrawEquipmentAimingOverride_DualWield_compActivatableEffect_PreFix", null));
                harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("DualWield.Ext_Pawn_EquipmentTracker", "DualWield"), "AddOffHandEquipment", null, null),null , new HarmonyMethod(Main.patchType, "AddOffHandEquipment_PostFix", null));
                harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssembly("DualWield.Harmony.PawnWeaponGenerator_TryGenerateWeaponFor", "DualWield.Harmony"), "Postfix", null, null), new HarmonyMethod(Main.patchType, "PawnWeaponGenerator_TryGenerateWeaponFor_PostFix", null));
                */
            }
            else
            {
                /*
                harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssemblyNew("CompOversizedWeapon.HarmonyCompOversizedWeapon", "CompOversizedWeapon"), "DrawEquipmentAimingPreFix", null, null), new HarmonyMethod(Main.patchType, "DrawEquipmentAiming_ActivatableEffect_OverSized_PreFix", null), new HarmonyMethod(Main.patchType, "DrawEquipmentAiming_ActivatableEffect_OverSized_PostFix", null));
                harmony.Patch(AccessTools.Method(GenTypes.GetTypeInAnyAssemblyNew("CompActivatableEffect.HarmonyCompActivatableEffect", "CompActivatableEffect"), "DrawEquipmentAimingPostFix", null, null), new HarmonyMethod(Main.patchType, "DrawEquipmentAimingPostFix_OverSized_Activatable_PreFix", null));
                */
            }
            //    [HarmonyPatch(typeof(NeurotrainerDefGenerator), "ImpliedThingDefs"),StaticConstructorOnStartup]
            /*
            foreach (AbilityDef item in DefDatabase<EquipmentAbilityDef>.AllDefs)
            {
                Log.Message(string.Format("checking {0}", item.LabelCap));
                if (DefDatabase<ThingDef>.AllDefs.Any(x=> x.descriptionHyperlinks.Contains(item)))
                {
                    Log.Message(string.Format("Psytrainer for {0}", item.LabelCap));
                    DefDatabase<ThingDef>.AllDefsListForReading.RemoveAll(x=> x.descriptionHyperlinks.Contains(item));
                }
            }
            */
            IEnumerable<ThingDef> pystrainers = DefDatabase<ThingDef>.AllDefs.Where(x => x.defName.Contains(NeurotrainerDefGenerator.PsytrainerDefPrefix));
            foreach (AbilityDef item in DefDatabase<AdeptusMechanicus.EquipmentAbilityDef>.AllDefs)
            {
                if (pystrainers.Any(x=> x.defName.Contains(item.defName)))
                {
                    ThingDef trainer = pystrainers.First(x => x.defName.Contains(item.defName));
                    DefDatabase<ThingDef>.AllDefsListForReading.Remove(trainer);
                }
            }
            if (DefDatabase<ScenarioDef>.AllDefs.Any(x=> x.defName.Contains("OG_WeaponsTest")))
            {
                foreach (ScenarioDef ScenDef in DefDatabase<ScenarioDef>.AllDefs.Where(x => x.defName.Contains("OG_WeaponsTest")))
                {
                    if (ScenDef.defName.Contains("Imperial"))
                    {
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "I");
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "IG");
                    }
                    else if (ScenDef.defName.Contains("Mechanicus"))
                    {
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "AM");
                    }
                    else if (ScenDef.defName.Contains("Chaos"))
                    {
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "C");
                    }
                    else if (ScenDef.defName.Contains("Eldar") && !ScenDef.defName.Contains("DarkEldar"))
                    {
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "E");
                    }
                    else if (ScenDef.defName.Contains("DarkEldar"))
                    {
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "DE");
                    }
                    else if (ScenDef.defName.Contains("Tau"))
                    {
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "T");
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "K");
                    }
                    else if (ScenDef.defName.Contains("Ork"))
                    {
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "O");
                    }
                    else if (ScenDef.defName.Contains("Necron"))
                    {
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "N");
                    }
                    else if (ScenDef.defName.Contains("Tyranid"))
                    {
                        TryAddWeaponsStartingThingToTestScenario(ScenDef, "TY");
                    }
                }
            }
        }

        private static void TryAddWeaponsStartingThingToTestScenario(ScenarioDef ScenDef, string Tag)
        {
            List<ThingDef> things = DefDatabase<ThingDef>.AllDefsListForReading.FindAll(x => (x.defName.Contains("OG" + Tag + "_Gun_") || x.defName.Contains("OG" + Tag + "_Melee_") || x.defName.Contains("OG" + Tag + "_Apparel_") || x.defName.Contains("OG" + Tag + "_Wargear_") || x.defName.Contains("OG" + Tag + "_GrenadePack_")) && (!x.defName.Contains("TOGGLEDEF_") || x.defName.Contains("TOGGLEDEF_S")));

            foreach (ThingDef Weapon in things)
            {
                bool hasweapon = false;
                List<ScenPart> parts = Traverse.Create(ScenDef.scenario).Field("parts").GetValue<List<ScenPart>>();
                foreach (ScenPart scenpart in parts.Where(x => x.def == ScenPartDefOf.StartingThing_Defined))
                {
                    ThingDef td = Traverse.Create(scenpart).Field("thingDef").GetValue<ThingDef>();
                    if (td == Weapon)
                    {
                        hasweapon = true;
                    }
                }
                if (!hasweapon)
                {
                    ScenPart_StartingThing_Defined _Defined = new ScenPart_StartingThing_Defined() { def = ScenPartDefOf.StartingThing_Defined };
                    Traverse.Create(_Defined).Field("thingDef").SetValue(Weapon);
                    if (Weapon.MadeFromStuff)
                    {
                        ThingDef stuffdef = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.IsStuff && Weapon.stuffCategories.Any(y=> x.stuffProps.categories.Contains(y))).RandomElement();
                        if (stuffdef!=null)
                        {
                            Traverse.Create(_Defined).Field("stuff").SetValue(stuffdef);
                        }
                    }
                    parts.Add(_Defined);
                }
            }
        }

        private static readonly Type patchType = typeof(Main);
        /*
        // Token: 0x0600000A RID: 10 RVA: 0x000022B0 File Offset: 0x000004B0
        public static bool DrawEquipmentAiming_ActivatableEffect_OverSized_PreFix(PawnRenderer __instance, Thing eq, Vector3 drawLoc, float aimAngle)
        {
            if (eq!=null)
            {
                Pawn pawn = eq.TryGetComp<CompEquippable>().PrimaryVerb.CasterPawn;
                if (pawn != null)
                {
                    Pawn value2 = pawn;
                    CompActivatableEffect.CompActivatableEffect compActivatableEffect = eq.TryGetComp<CompActivatableEffect.CompActivatableEffect>();
                    CompOversizedWeapon.CompOversizedWeapon compOversizedWeapon = eq.TryGetComp<CompOversizedWeapon.CompOversizedWeapon>();
                    if (compActivatableEffect != null && compOversizedWeapon != null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // Token: 0x0600000A RID: 10 RVA: 0x000022B0 File Offset: 0x000004B0
        public static void DrawEquipmentAiming_ActivatableEffect_OverSized_PostFix(PawnRenderer __instance, Thing eq, Vector3 drawLoc, float aimAngle, ref bool __result)
        {
            if (eq != null)
            {
                Pawn pawn = eq.TryGetComp<CompEquippable>().PrimaryVerb.CasterPawn;
                if (pawn != null)
                {
                    Pawn value2 = pawn;
                    CompActivatableEffect.CompActivatableEffect compActivatableEffect = eq.TryGetComp<CompActivatableEffect.CompActivatableEffect>();
                    CompOversizedWeapon.CompOversizedWeapon compOversizedWeapon = eq.TryGetComp<CompOversizedWeapon.CompOversizedWeapon>();
                    if (compActivatableEffect != null && compOversizedWeapon != null)
                    {

                        ThingWithComps thingWithComps;
                        bool flag = (thingWithComps = (eq as ThingWithComps)) != null;
                        if (flag)
                        {
                            ThingComp thingComp = thingWithComps.AllComps.FirstOrDefault((ThingComp y) => y.GetType().ToString() == "CompDeflector.CompDeflector" || y.GetType().BaseType.ToString() == "CompDeflector.CompDeflector");
                            bool flag2 = thingComp != null;
                            if (flag2)
                            {
                                bool value = Traverse.Create(thingComp).Property("IsAnimatingNow", null).GetValue<bool>();
                                bool flag3 = value;
                                if (flag3)
                                {
                                    __result = false;
                                }
                            }
                            bool flag4 = compOversizedWeapon != null;
                            if (flag4)
                            {
                                bool flag5 = false;
                                float num = aimAngle - 90f;
                                bool flag6 = value2 == null;
                                if (flag6)
                                {
                                    __result = true;
                                }
                                bool flag7 = aimAngle > 20f && aimAngle < 160f;
                                if (flag7)
                                {
                                    Mesh mesh = MeshPool.plane10;
                                    num += eq.def.equippedAngleOffset;
                                }
                                else
                                {
                                    bool flag8 = aimAngle > 200f && aimAngle < 340f;
                                    if (flag8)
                                    {
                                        Mesh mesh = MeshPool.plane10Flip;
                                        flag5 = true;
                                        num -= 180f;
                                        num -= eq.def.equippedAngleOffset;
                                    }
                                    else
                                    {
                                        num =  AdjustOffsetAtPeace(eq, value2, compOversizedWeapon, num);
                                    }
                                }
                                bool f9pre = value2.TargetCurrentlyAimingAt == null || value2.stances.curStance.GetType() == typeof(Stance_Mobile);
                                bool flag9 = compOversizedWeapon.Props != null && (!value2.IsFighting() || f9pre) && compOversizedWeapon.Props.verticalFlipNorth && value2.Rotation == Rot4.North;
                                Log.Message(string.Format("f9pre: {0}, TargetCurrentlyAimingAtNull: {1}, curStanceMobile: {2}, flag9: {3}", f9pre, value2.TargetCurrentlyAimingAt == null, value2.stances.curStance.GetType() == typeof(Stance_Mobile), flag9));
                                if (flag9)
                                {
                                    num += 180f;
                                }
                                bool flag10 = !value2.IsFighting() || f9pre;
                                if (flag10)
                                {
                                    num = AdjustNonCombatRotation(value2, num, compOversizedWeapon);
                                }
                                num %= 360f;
                                Graphic_StackCount graphic_StackCount = eq.Graphic as Graphic_StackCount;
                                bool flag11 = graphic_StackCount != null;
                                Material matSingle;
                                if (flag11)
                                {
                                    matSingle = graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingle;
                                }
                                else
                                {
                                    matSingle = eq.Graphic.MatSingle;
                                }
                                Vector3 s = new Vector3(eq.def.graphicData.drawSize.x, 1f, eq.def.graphicData.drawSize.y);
                                Matrix4x4 matrix = default(Matrix4x4);
                                Vector3 vector = AdjustRenderOffsetFromDir(value2, compOversizedWeapon);
                                matrix.SetTRS(drawLoc + vector, Quaternion.AngleAxis(num, Vector3.up), s);
                                Graphics.DrawMesh((!flag5) ? MeshPool.plane10 : MeshPool.plane10Flip, matrix, matSingle, 0);
                                bool flag12 = compOversizedWeapon.Props != null && compOversizedWeapon.Props.isDualWeapon;
                                if (flag12)
                                {
                                    vector = new Vector3(-1f * vector.x, vector.y, vector.z);
                                    bool flag13 = value2.Rotation == Rot4.North || value2.Rotation == Rot4.South;
                                    Mesh mesh2;
                                    if (flag13)
                                    {
                                        num += 135f;
                                        num %= 360f;
                                        mesh2 = ((!flag5) ? MeshPool.plane10Flip : MeshPool.plane10);
                                    }
                                    else
                                    {
                                        vector = new Vector3(vector.x, vector.y - 0.1f, vector.z + 0.15f);
                                        mesh2 = ((!flag5) ? MeshPool.plane10 : MeshPool.plane10Flip);
                                    }
                                    matrix.SetTRS(drawLoc + vector, Quaternion.AngleAxis(num, Vector3.up), s);
                                    Graphics.DrawMesh(mesh2, matrix, matSingle, 0);
                                }
                                __result = false;
                            }
                        }
                        __result = true;
                    }
                }
            }
        }

        public static bool DrawEquipmentAimingPostFix_OverSized_Activatable_PreFix(PawnRenderer __instance, Thing eq, Vector3 drawLoc, float aimAngle)
        {
            Pawn pawn = eq.TryGetComp<CompEquippable>().PrimaryVerb.CasterPawn;
            if (pawn != null)
            {
                Pawn value2 = pawn;
                CompActivatableEffect.CompActivatableEffect compActivatableEffect = eq.TryGetComp<CompActivatableEffect.CompActivatableEffect>();
                CompOversizedWeapon.CompOversizedWeapon compOversizedWeapon = eq.TryGetComp<CompOversizedWeapon.CompOversizedWeapon>();
                if (compActivatableEffect != null && compOversizedWeapon != null)
                {
                    
                    Pawn_EquipmentTracker equipment = pawn.equipment;
                    ThingWithComps thingWithComps = (equipment != null) ? equipment.Primary : null;
                    bool flag = ((compActivatableEffect != null) ? compActivatableEffect.Graphic : null) == null;
                    if (!flag)
                    {
                        bool flag2 = compActivatableEffect.CurrentState != CompActivatableEffect.CompActivatableEffect.State.Activated;
                        if (!flag2)
                        {
                            float num = aimAngle - 90f;
                            bool flag3 = false;
                            bool flag4 = aimAngle > 20f && aimAngle < 160f;
                            if (flag4)
                            {
                                num += eq.def.equippedAngleOffset;
                            }
                            else
                            {
                                bool flag5 = aimAngle > 200f && aimAngle < 340f;
                                if (flag5)
                                {
                                    flag3 = true;
                                    num -= 180f;
                                    num -= eq.def.equippedAngleOffset;
                                }
                                else
                                {
                                    num += eq.def.equippedAngleOffset;
                                }
                            }
                            bool NonCombatRotation = !value2.IsFighting();
                            num = AdjustNonCombatRotation(value2, num, compOversizedWeapon);
                            num %= 360f;
                            Vector3 vector = Vector3.zero;
                            ThingWithComps thingWithComps2;
                            bool flag6 = (thingWithComps2 = (eq as ThingWithComps)) != null;
                            if (flag6)
                            {
                                bool flag7 = (compOversizedWeapon = (thingWithComps2.AllComps.FirstOrDefault((ThingComp z) => z is CompOversizedWeapon.CompOversizedWeapon) as CompOversizedWeapon.CompOversizedWeapon)) != null;
                                if (flag7)
                                {
                                    bool flag8 = pawn.Rotation == Rot4.East;
                                    if (flag8)
                                    {
                                        vector = compOversizedWeapon.Props.eastOffset;
                                    }
                                    else
                                    {
                                        bool flag9 = pawn.Rotation == Rot4.West;
                                        if (flag9)
                                        {
                                            vector = compOversizedWeapon.Props.westOffset;
                                        }
                                        else
                                        {
                                            bool flag10 = pawn.Rotation == Rot4.North;
                                            if (flag10)
                                            {
                                                vector = compOversizedWeapon.Props.northOffset;
                                            }
                                            else
                                            {
                                                bool flag11 = pawn.Rotation == Rot4.South;
                                                if (flag11)
                                                {
                                                    vector = compOversizedWeapon.Props.southOffset;
                                                }
                                            }
                                        }
                                    }
                                    vector += compOversizedWeapon.Props.offset;
                                }
                                ThingComp thingComp = thingWithComps2.AllComps.FirstOrDefault((ThingComp y) => y.GetType().ToString().Contains("Deflect"));
                                bool flag12 = thingComp != null;
                                if (flag12)
                                {
                                    bool flag13 = (bool)AccessTools.Property(thingComp.GetType(), "IsAnimatingNow").GetValue(thingComp, null);
                                    bool flag14 = flag13;
                                    if (flag14)
                                    {
                                        float num2 = (float)((int)AccessTools.Property(thingComp.GetType(), "AnimationDeflectionTicks").GetValue(thingComp, null));
                                        bool flag15 = num2 > 0f;
                                        if (flag15)
                                        {
                                            bool flag16 = !flag3;
                                            if (flag16)
                                            {
                                                num += (num2 + 1f) / 2f;
                                            }
                                            else
                                            {
                                                num -= (num2 + 1f) / 2f;
                                            }
                                        }
                                    }
                                }
                            }
                            num %= 360f;
                            Material matSingle = compActivatableEffect.Graphic.MatSingle;
                            Vector3 s = new Vector3(eq.def.graphicData.drawSize.x, 1f, eq.def.graphicData.drawSize.y);
                            Matrix4x4 matrix = default(Matrix4x4);
                            matrix.SetTRS(drawLoc + vector, Quaternion.AngleAxis(num, Vector3.up), s);
                            bool flag17 = !flag3;
                            
                            if (flag17)
                            {
                                Graphics.DrawMesh(MeshPool.plane10, matrix, matSingle, 0);
                            }
                            else
                            {
                                Graphics.DrawMesh(MeshPool.plane10Flip, matrix, matSingle, 0);
                            }
                            
                            bool flag18 = compOversizedWeapon.Props != null && compOversizedWeapon.Props.isDualWeapon;
                            if (flag18)
                            {
                                vector = new Vector3(-1f * vector.x, vector.y, vector.z);
                                bool flag13 = value2.Rotation == Rot4.North || value2.Rotation == Rot4.South;
                                Mesh mesh2;
                                if (flag13)
                                {
                                    num += 135f;
                                    num %= 360f;
                                    mesh2 = ((!flag3) ? MeshPool.plane10Flip : MeshPool.plane10);
                                }
                                else
                                {
                                    vector = new Vector3(vector.x, vector.y - 0.1f, vector.z + 0.15f);
                                    mesh2 = ((!flag3) ? MeshPool.plane10 : MeshPool.plane10Flip);
                                }
                                matrix.SetTRS(drawLoc + vector, Quaternion.AngleAxis(num, Vector3.up), s);
                                Graphics.DrawMesh(mesh2, matrix, matSingle, 0);
                            }
                        }
                    }
                    return false;
                }
            }
            return true;
        }

        private static Vector3 AdjustRenderOffsetFromDir(Pawn pawn, CompOversizedWeapon.CompOversizedWeapon compOversizedWeapon)
        {
            Rot4 rotation = pawn.Rotation;
            Vector3 result = Vector3.zero;
            bool flag = compOversizedWeapon.Props != null;
            if (flag)
            {
                result = compOversizedWeapon.Props.northOffset;
                bool flag2 = rotation == Rot4.East;
                if (flag2)
                {
                    result = compOversizedWeapon.Props.eastOffset;
                }
                else
                {
                    bool flag3 = rotation == Rot4.South;
                    if (flag3)
                    {
                        result = compOversizedWeapon.Props.southOffset;
                    }
                    else
                    {
                        bool flag4 = rotation == Rot4.West;
                        if (flag4)
                        {
                            result = compOversizedWeapon.Props.westOffset;
                        }
                    }
                }
            }
            return result;
        }
        private static float AdjustOffsetAtPeace(Thing eq, Pawn pawn, CompOversizedWeapon.CompOversizedWeapon compOversizedWeapon, float num)
        {
            Mesh plane = MeshPool.plane10;
            float num2 = eq.def.equippedAngleOffset;
            bool flag = compOversizedWeapon.Props != null && !pawn.IsFighting() && compOversizedWeapon.Props.verticalFlipOutsideCombat;
            if (flag)
            {
                num2 += 180f;
            }
            num += num2;
            return num;
        }
        private static float AdjustNonCombatRotation(Pawn pawn, float num, CompOversizedWeapon.CompOversizedWeapon compOversizedWeapon)
        {
            bool flag = compOversizedWeapon.Props != null;
            if (flag)
            {
                bool flag2 = pawn.Rotation == Rot4.North;
                if (flag2)
                {
                    num += compOversizedWeapon.Props.angleAdjustmentNorth;
                }
                else
                {
                    bool flag3 = pawn.Rotation == Rot4.East;
                    if (flag3)
                    {
                        num += compOversizedWeapon.Props.angleAdjustmentEast;
                    }
                    else
                    {
                        bool flag4 = pawn.Rotation == Rot4.West;
                        if (flag4)
                        {
                            num += compOversizedWeapon.Props.angleAdjustmentWest;
                        }
                        else
                        {
                            bool flag5 = pawn.Rotation == Rot4.South;
                            if (flag5)
                            {
                                num += compOversizedWeapon.Props.angleAdjustmentSouth;
                            }
                            num += compOversizedWeapon.Props.angleAdjustmentSouth;
                        }
                    }
                }
            }
            return num;
        }
        */
        // 
        /*
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
        public static void AddOffHandEquipment_PostFix(Pawn_EquipmentTracker __instance, ThingWithComps newEq)
        {
            Pawn pawn = newEq.TryGetComp<CompEquippable>().PrimaryVerb.CasterPawn;
            if (pawn != null)
            {
                foreach (ThingWithComps item in pawn.equipment.AllEquipmentListForReading)
                {
                    CompActivatableEffect.CompActivatableEffect comp = item.TryGetComp<CompActivatableEffect.CompActivatableEffect>();
                    bool flag = __instance != null && comp != null && comp.CurrentState == CompActivatableEffect.CompActivatableEffect.State.Activated;
                    if (flag)
                    {
                        comp.TryActivate();
                    }
                }
            }
        }

        // Token: 0x0600000A RID: 10 RVA: 0x000022B0 File Offset: 0x000004B0
        public static bool DrawEquipmentAiming_DualWield_Activatable_PreFix(PawnRenderer __instance, Thing eq, Vector3 drawLoc, float aimAngle)
        {
            Pawn pawn = eq.TryGetComp<CompEquippable>().PrimaryVerb.CasterPawn;
            if (pawn != null)
            {
                if (pawn.equipment.AdMechTryGetOffHandEquipment(out ThingWithComps thingy))
                {
                    return false;
                }
            }
            return true;
        }
        
        // Token: 0x06000079 RID: 121 RVA: 0x00005DB4 File Offset: 0x00003FB4
        public static bool DrawEquipmentAimingOverride_DualWield_compActivatableEffect_PreFix(Thing eq, Vector3 drawLoc, float aimAngle)
        {
            Pawn pawn = eq.TryGetComp<CompEquippable>().PrimaryVerb.CasterPawn;
            if (pawn != null)
            {
                if (pawn.equipment.AdMechTryGetOffHandEquipment(out ThingWithComps thingy))
                {
                    Pawn_EquipmentTracker equipment = pawn.equipment;
                //    ThingWithComps thingWithComps = (equipment != null) ? equipment.Primary : null;
                    ThingWithComps thingWithComps = (eq != null) ? (ThingWithComps)eq : null;
                    CompActivatableEffect.CompActivatableEffect compActivatableEffect = (thingWithComps != null) ? thingWithComps.GetComp<CompActivatableEffect.CompActivatableEffect>() : null;
                    bool flag = ((compActivatableEffect != null) ? compActivatableEffect.Graphic : null) == null;
                    if (!flag)
                    {
                        bool flag2 = compActivatableEffect.CurrentState != CompActivatableEffect.CompActivatableEffect.State.Activated;
                        if (!flag2)
                        {
                            float num = aimAngle - 90f;
                            bool flag3 = false;
                            bool flag4 = aimAngle > 20f && aimAngle < 160f;
                            if (flag4)
                            {
                                num += eq.def.equippedAngleOffset;
                            }
                            else
                            {
                                bool flag5 = aimAngle > 200f && aimAngle < 340f;
                                if (flag5)
                                {
                                    flag3 = true;
                                    num -= 180f;
                                    num -= eq.def.equippedAngleOffset;
                                }
                                else
                                {
                                    num += eq.def.equippedAngleOffset;
                                }
                            }
                            Vector3 vector = Vector3.zero;
                            ThingWithComps thingWithComps2;
                            bool flag6 = (thingWithComps2 = (eq as ThingWithComps)) != null;
                            if (flag6)
                            {
                                CompOversizedWeapon.CompOversizedWeapon compOversizedWeapon;
                                bool flag7 = (compOversizedWeapon = (thingWithComps2.AllComps.FirstOrDefault((ThingComp z) => z is CompOversizedWeapon.CompOversizedWeapon) as CompOversizedWeapon.CompOversizedWeapon)) != null;
                                if (flag7)
                                {
                                    bool flag8 = pawn.Rotation == Rot4.East;
                                    if (flag8)
                                    {
                                        vector = compOversizedWeapon.Props.eastOffset;
                                    }
                                    else
                                    {
                                        bool flag9 = pawn.Rotation == Rot4.West;
                                        if (flag9)
                                        {
                                            vector = compOversizedWeapon.Props.westOffset;
                                        }
                                        else
                                        {
                                            bool flag10 = pawn.Rotation == Rot4.North;
                                            if (flag10)
                                            {
                                                vector = compOversizedWeapon.Props.northOffset;
                                            }
                                            else
                                            {
                                                bool flag11 = pawn.Rotation == Rot4.South;
                                                if (flag11)
                                                {
                                                    vector = compOversizedWeapon.Props.southOffset;
                                                }
                                            }
                                        }
                                    }
                                    vector += compOversizedWeapon.Props.offset;
                                }
                                ThingComp thingComp = thingWithComps2.AllComps.FirstOrDefault((ThingComp y) => y.GetType().ToString().Contains("Deflect"));
                                bool flag12 = thingComp != null;
                                if (flag12)
                                {
                                    bool flag13 = (bool)AccessTools.Property(thingComp.GetType(), "IsAnimatingNow").GetValue(thingComp, null);
                                    bool flag14 = flag13;
                                    if (flag14)
                                    {
                                        float num2 = (float)((int)AccessTools.Property(thingComp.GetType(), "AnimationDeflectionTicks").GetValue(thingComp, null));
                                        bool flag15 = num2 > 0f;
                                        if (flag15)
                                        {
                                            bool flag16 = !flag3;
                                            if (flag16)
                                            {
                                                num += (num2 + 1f) / 2f;
                                            }
                                            else
                                            {
                                                num -= (num2 + 1f) / 2f;
                                            }
                                        }
                                    }
                                }
                            }
                            num %= 360f;
                            drawLoc.y = compActivatableEffect.Altitude(drawLoc);
                            Material matSingle = compActivatableEffect.Graphic.MatSingle;
                            Vector3 s = new Vector3(eq.def.graphicData.drawSize.x, 1f, eq.def.graphicData.drawSize.y);
                            Matrix4x4 matrix = default(Matrix4x4);
                            matrix.SetTRS(drawLoc + vector, Quaternion.AngleAxis(num, Vector3.up), s);
                        //    Log.Message(string.Format("thingy: {0}, thingWithComps: {1}, eq: {2}", thingy, thingWithComps, eq));
                            bool flag17 = !flag3;
                            if (flag17)
                            {
                                Graphics.DrawMesh(MeshPool.plane10, matrix, matSingle, 0);
                            }
                            else
                            {
                                Graphics.DrawMesh(MeshPool.plane10Flip, matrix, matSingle, 0);
                            }

                            float numA = aimAngle - 90f;
                            bool flagA = aimAngle > 20f && aimAngle < 160f;
                            Mesh mesh;
                            if (flagA)
                            {
                                mesh = MeshPool.plane10;
                                numA += eq.def.equippedAngleOffset;
                            }
                            else
                            {
                                bool flagB = aimAngle > 200f && aimAngle < 340f;
                                if (flagB)
                                {
                                    mesh = MeshPool.plane10Flip;
                                    numA -= 180f;
                                    numA -= eq.def.equippedAngleOffset;
                                }
                                else
                                {
                                    mesh = MeshPool.plane10;
                                    numA += eq.def.equippedAngleOffset;
                                }
                            }
                            numA %= 360f;
                            Graphic_StackCount graphic_StackCount = eq.Graphic as Graphic_StackCount;
                            bool flagC = graphic_StackCount != null;
                            Material matSingleA;
                            if (flagC)
                            {
                                matSingleA = graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingle;
                            }
                            else
                            {
                                matSingleA = eq.Graphic.MatSingle;
                            }
                            Matrix4x4 matrixA = default(Matrix4x4);
                            matrixA.SetTRS(drawLoc + vector, Quaternion.AngleAxis(numA, Vector3.up), s);
                            Graphics.DrawMesh(mesh, matrixA, matSingleA, 0);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        // Token: 0x0600000A RID: 10 RVA: 0x000022B0 File Offset: 0x000004B0
        public static bool DrawEquipmentAiming_DualWield_OverSized_PreFix(PawnRenderer __instance, Thing eq, Vector3 drawLoc, float aimAngle)
        {
            if (eq != null)
            {
                if (eq.TryGetComp<CompEquippable>() != null)
                {
                    if (eq.TryGetComp<CompOversizedWeapon.CompOversizedWeapon>() != null)
                    {
                        if (eq.TryGetComp<CompEquippable>().PrimaryVerb != null)
                        {
                            if (eq.TryGetComp<CompEquippable>().PrimaryVerb.CasterPawn != null)
                            {
                                Pawn pawn = eq.TryGetComp<CompEquippable>().PrimaryVerb.CasterPawn;
                                if (true)
                                {

                                }
                                bool result = !pawn.equipment.AdMechTryGetOffHandEquipment(out ThingWithComps thing) && eq.TryGetComp<CompOversizedWeapon.CompOversizedWeapon>() != null;
                                //    Log.Message(string.Format("DrawEquipmentAiming_DualWield_OverSized_PreFix __result:{0}", result));
                                return result;
                            }
                            else
                            {
                                //    Log.Message(string.Format("eq.CompEquippable.PrimaryVerb.CasterPawn is Null"));
                            }
                        }
                        else
                        {
                            //    Log.Message(string.Format("eq.CompEquippable.PrimaryVerb is Null"));
                        }
                    }
                }
                else
                {
                    //    Log.Message(string.Format("eq.CompEquippable is Null}"));
                }
            }
            else
            {
                //    Log.Message(string.Format("eq is Null}"));
            }
            return true;
        }

        // Token: 0x0600000A RID: 10 RVA: 0x000022B0 File Offset: 0x000004B0
        public static void DrawEquipmentAiming_DualWield_OverSized_PostFix(PawnRenderer __instance, Thing eq, Vector3 drawLoc, float aimAngle, ref bool __result)
        {
            if (eq != null)
            {
                if (eq.TryGetComp<CompEquippable>() != null)
                {
                    if (eq.TryGetComp<CompOversizedWeapon.CompOversizedWeapon>() != null)
                    {

                        if (eq.TryGetComp<CompEquippable>().PrimaryVerb != null)
                        {
                            if (eq.TryGetComp<CompEquippable>().PrimaryVerb.CasterPawn != null)
                            {
                                Pawn pawn = eq.TryGetComp<CompEquippable>().PrimaryVerb.CasterPawn;
                                __result = pawn.equipment.AdMechTryGetOffHandEquipment(out ThingWithComps thing);
                                //    Log.Message(string.Format("DrawEquipmentAiming_DualWield_OverSized_PostFix __result:{0}", __result));
                            }
                            else
                            {
                                //    Log.Message(string.Format("eq.CompEquippable.PrimaryVerb.CasterPawn is Null __result:{0}", __result));
                            }
                        }
                    }
                    else
                    {
                        //    Log.Message(string.Format("eq.CompEquippable.PrimaryVerb is Null __result:{0}", __result));
                    }
                }
                else
                {
                    //    Log.Message(string.Format("eq.CompEquippable is Null __result:{0}", __result));
                }
            }
            else
            {
                //    Log.Message(string.Format("eq is Null __result:{0}, eq: {1}", __result, eq));
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
        */
        private static void PatchPawnsArrivalModeWorker(Harmony harmonyInstance)
        {
            var prefix = typeof(AM_PawnsArrivalModeWorker_EdgeWalkIn_Arrive_DSI_Patch).GetMethod("Arrive_DSI");
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

        private static void PatchVerbsShoot(Harmony harmonyInstance)
        {
            var postfix = typeof(AM_Verb_Shoot_Get_ShotsPerBurst_RapidFire_Patch).GetMethod("ShotsPerBurst_RapidFire_Postfix");
            var baseType = typeof(Verb_LaunchProjectile);
            if (AdeptusIntergrationUtil.enabled_CombatExtended)
            {
                baseType = typeof(CombatExtended.Verb_LaunchProjectileCE);
            }
            var types = baseType.AllSubclassesNonAbstract();
            foreach (Type cur in types)
            {
                harmonyInstance.Patch(cur.GetMethod("get_ShotsPerBurst"),null, new HarmonyMethod(postfix));
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

    /*
    // RimWorld.FloatMenuMakerMap
    private static void AddHumanlikeOrders(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
    */
    
    /*
    [HarmonyPatch(typeof(PawnComponentsUtility), "AddAndRemoveDynamicComponents")]
    public static class AdMech_PawnComponentsUtility_AddAndRemoveDynamicComponents_Patch
    {
        private static void Postfix(Pawn pawn)
        {
            bool flag = pawn.Faction != null && pawn.Faction.IsPlayer;
            bool flag2 = pawn.def.defName == "Mechanicus_Sicarian";
            bool flag3 = flag && flag2;
            if (flag3)
            {
                pawn.drafter = new Pawn_DraftController(pawn);
            }
        }
    }

    // Token: 0x02000093 RID: 147
    [HarmonyPatch(typeof(Pawn_DraftController), "GetGizmos")]
    internal static class Pawn_DraftController_GetGizmos
    {
        // Token: 0x06000218 RID: 536 RVA: 0x0000FB83 File Offset: 0x0000DD83
        private static void Postfix(Pawn_DraftController __instance, ref IEnumerable<Gizmo> __result)
        {
            __result = Pawn_DraftController_GetGizmos.PatchGetGizmos(__instance, __result);
        }

        // Token: 0x06000219 RID: 537 RVA: 0x0000FB90 File Offset: 0x0000DD90
        private static IEnumerable<Gizmo> PatchGetGizmos(Pawn_DraftController __instance, IEnumerable<Gizmo> __result)
        {
            foreach (Gizmo gizmo in __result)
            {
                bool flag = gizmo is Command_Toggle;
                if (flag)
                {
                    Command_Toggle toggleCommand = gizmo as Command_Toggle;
                    bool flag2 = toggleCommand.defaultDesc != null && toggleCommand.defaultDesc == "CommandToggleDraftDesc".Translate();
                    if (flag2)
                    {
                        yield return toggleCommand;
                        continue;
                    }
                    toggleCommand = null;
                }
                yield return gizmo;
            }
            IEnumerator<Gizmo> enumerator = null;
            yield break;
        }
    }

    [HarmonyPatch(typeof(Pawn_DraftController), "set_Drafted")]
    internal static class Pawn_Draftcontroller_set_Drafted
    {
        // Token: 0x06000217 RID: 535 RVA: 0x0000FB40 File Offset: 0x0000DD40
        private static void Postfix(Pawn_DraftController __instance)
        {
            bool flag = true;
            if (flag)
            {
                bool drafted = __instance.Drafted;
                if (drafted)
                {

                }
            }
        }
    }

    [HarmonyPatch(typeof(Pawn), "get_IsColonistPlayerControlled")]
    public class Pawn_get_IsColonistPlayerControlled
    {
        // Token: 0x06000203 RID: 515 RVA: 0x0000ED24 File Offset: 0x0000CF24
        public static bool Prefix(Pawn __instance, ref bool __result)
        {
            bool flag = Pawn_get_IsColonistPlayerControlled.IsControlled(__instance);
            if (flag)
            {
                bool flag2 = __instance.Faction == Faction.OfPlayer && __instance.def.defName == "Mechanicus_Sicarian" && !__instance.Dead;
                if (flag2)
                {
                    __result = true;
                    return false;
                }
            }
            int ticksGame = Find.TickManager.TicksGame;
            return true;
        }

        // Token: 0x06000204 RID: 516 RVA: 0x0000ED88 File Offset: 0x0000CF88
        private static bool IsControlled(Pawn pawn)
        {
            bool flag = pawn.def.defName != "Mechanicus_Sicarian";
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }
    }
    */
    /*
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "Notify_EquipmentRemoved")]
    public static class AdMech_Pawn_EquipmentTracker_Notify_EquipmentRemoved_Patch
    {
        [HarmonyPostfix]
        public static void Notify_EquipmentRemovedPostfix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
        {
	        foreach (CompAbilityItem compAbilityItem in eq.GetComps<CompAbilityItem>())
	        {
		        foreach (CompAbilityUser compAbilityUser in ((Pawn)__instance.ParentHolder).GetComps<CompAbilityUser>())
		        {
			        bool flag = compAbilityUser.GetType() == compAbilityItem.Props.AbilityUserClass;
			        if (flag)
			        {
				        foreach (AbilityDef abilityDef in compAbilityItem.Props.Abilities)
				        {
				            compAbilityItem.AbilityUserTarget = compAbilityUser;
					        compAbilityUser.RemoveWeaponAbility(abilityDef);
				        }
			        }
		        }
	        }
        }
    }
    */

    /*
    [HarmonyPatch(typeof(Pawn_RecordsTracker), "Increment")]
    public static class AdMech_Pawn_RecordsTracker_Increment_Patch
    {
        [HarmonyPostfix]
        public static void IncrementPostfix(Pawn_RecordsTracker __instance, RecordDef def)
        {
            if (def == RecordDefOf.Kills)
            {
            }
        }
    }

    [HarmonyPatch(typeof(Verb_Shoot), "TryCastShot")]
    public static class AdMech_Verb_LaunchProjectile_TryCastShot_Patch
    {

        [HarmonyPostfix]
        public static void Postfix(ref Verb_Shoot __instance, MethodBase ___originalMethod)
        {
            if (__instance.Projectile.GetCompProperties<CompProperties_ShotgunShell>() != null && __instance.Projectile.GetCompProperties<CompProperties_ShotgunShell>() is CompProperties_ShotgunShell Shell)
            {
                if (Shell.PelletCount>1)
                {
                    for (int i = 0; i < Shell.PelletCount; i++)
                    {
                        __instance?.Invoke();
                        ;
                    }
                }
            }
        }
        
    }
    
    */
}