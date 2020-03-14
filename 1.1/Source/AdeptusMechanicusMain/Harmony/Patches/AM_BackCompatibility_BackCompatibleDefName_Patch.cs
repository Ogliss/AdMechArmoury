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

namespace AdeptusMechanicus.HarmonyInstance
{

    [HarmonyPatch(typeof(BackCompatibility), "BackCompatibleDefName")]
    public static class AM_BackCompatibility_BackCompatibleDefName_Patch
    {
        [HarmonyPostfix]
        public static void BackCompatibleDefName_Postfix(Type defType, string defName, bool forDefInjections, ref string __result)
        {
            if (GenDefDatabase.GetDefSilentFail(defType, defName, false) == null)
            {
            //    Log.Message(string.Format("Checking for replacement for {0} Type: {1}", defName, defType));
                if (defType == typeof(ThingDef))
                {
                    if (defName.Contains("Alien_Kroot"))
                    {
                        __result = "OG_Alien_Kroot";
                    }
                    else
                    if (defName.Contains("Alien_Tau"))
                    {
                        __result = "OG_Alien_Tau";
                    }
                    else
                    if (defName.Contains("Alien_Ork"))
                    {
                        __result = "OG_Alien_Ork";
                    }
                    else
                    if (defName.Contains("Cybork"))
                    {
                        __result = "OG_Alien_Cybork";
                    }
                    else
                    if (defName.Contains("Alien_Cybork"))
                    {
                        __result = "OG_Alien_Cybork";
                    }
                    else
                    if (defName.Contains("Alien_Grot"))
                    {
                        __result = "OG_Alien_Grot";
                    }
                    else
                    if (defName.Contains("Alien_Eldar"))
                    {
                        __result = "OG_Alien_Eldar";
                    }
                    else
                    if (defName.Contains("AttackSquig"))
                    {
                        __result = "OG_Squig_Ork";
                    }
                    else
                    if (defName.Contains("Squig"))
                    {
                        __result = "OG_Squig";
                    }
                    else
                    if (defName.Contains("Snotling"))
                    {
                        __result = "OG_Ork_Snotling";
                    }
                    
                    if (defName != __result)
                    {
                        if (defName.Contains("Corpse"))
                        {
                            __result = "Corpse_" + __result;
                        }
                        if (defName.Contains("Meat"))
                        {
                            if (defName.Contains("Cyborg_Ork"))
                            {
                                __result = "Meat_OG_Alien_Ork";
                            }
                            else
                            if (defName.Contains("Squig"))
                            {
                                __result = "Meat_OG_Squig";
                            }
                            else
                            __result = "Meat_" + __result;
                        }
                    }
                    if (defName.Contains("OrkGrog"))
                    {
                        __result = "OG_Ork_Grog";
                    }
                    if (defName.Contains("Plant_OrkFungus"))
                    {
                        __result = "OG_Plant_OrkoidFungus";
                    }
                    if (defName.Contains("Meat_Cyborg_Ork"))
                    {
                        __result = "Meat_OG_Alien_Ork";
                    }
                    if (defName.Contains("CadianFlakArmour"))
                    {
                        __result = "OGIG_Apparel_FlakArmour";
                    }
                    if (defName.Contains("Rosarius"))
                    {
                        __result = DefDatabase<ThingDef>.AllDefs.Where(x => x.defName.Contains("Rosarius")).ToList().First().defName;
                    }
                    if (defName.Contains("IronHalo"))
                    {
                        __result = DefDatabase<ThingDef>.AllDefs.Where(x => x.defName.Contains("IronHalo")).ToList().First().defName;
                    }
                    if (defName.Contains("PuritySealA"))
                    {
                        __result = DefDatabase<ThingDef>.AllDefs.Where(x => x.defName.Contains("PuritySealA")).ToList().First().defName;
                    }
                    if (defName.Contains("Apparel_TribalKroot"))
                    {
                        __result = "OGK_Apparel_TribalKroot";
                    }
                    
                    if (defName.Contains("OGT_CombatHelmet"))
                    {
                        __result = "OGT_Apparel_CombatHelmet";
                    }
                    if (defName.Contains("OGT_CombatArmour"))
                    {
                        __result = "OGT_Apparel_CombatArmour";
                    }
                    if (defName.Contains("OGE_Apparel_RuneArmour"))
                    {
                        __result = "OGE_Apparel_RuneArmourWarlock";
                    }
                    if (defName.Contains("OGCDWarpRift"))
                    {
                        __result = DefDatabase<ThingDef>.AllDefs.Where(x=> x.defName.Contains("Warp") && x.defName.Contains("Rift")).RandomElement().defName;
                    }
                    
                }
                if (defType == typeof(FactionDef))
                {
                    if (defName == "OGChaosDeamonFaction")
                    {
                        __result = "OG_Chaos_Deamon_Faction";
                    }
                    if (defName == "MechanicusFaction")
                    {
                        __result = "OG_Mechanicus_Faction";
                    }
                    if (defName == "NecronFaction")
                    {
                        __result = "OG_Necron_Faction";
                    }
                    // Eldar Factions
                    if (defName == "EldarFaction")
                    {
                        __result = "OG_Eldar_Craftworld_Faction";
                    }
                    if (defName == "EldarPlayerColony")
                    {
                        __result = "OG_Eldar_Player_Craftworld";
                    }
                    // Ork factions
                    if (defName == "OrkFaction")
                    {
                        __result = "OG_Ork_Tek_Faction";
                    }
                    if (defName == "FeralOrkFaction")
                    {
                        __result = "OG_Ork_Feral_Faction";
                    }
                    if (defName.Contains("Ork") && defName.Contains("Player"))
                    {
                        if (defName.Contains("Trib"))
                        {
                            __result = "OG_Ork_PlayerTribe";
                        }
                        else
                        {
                            __result = "OG_Ork_PlayerColony";
                        }
                    }
                    if (defName == "RokOrkz")
                    {
                        __result = "OG_Ork_Rok";
                    }
                    if (defName == "HulkOrkz")
                    {
                        __result = "OG_Ork_Hulk";
                    }

                    // Tau factions
                    if (defName == "TauFaction")
                    {
                        __result = "OG_Tau_Faction";
                    }
                    if (defName == "TauPlayerColony")
                    {
                        __result = "OG_Tau_Player";
                    }

                    // Kroot factions
                    if (defName == "KrootFaction")
                    {
                        __result = "OG_Kroot_Faction";
                    }
                    if (defName == "KrootPlayerColonyTribal")
                    {
                        __result = "OG_Kroot_Player_Tribal";
                    }

                }
                if (defType == typeof(PawnKindDef))
                {
                    List<PawnKindDef> list;
                    if (defName.Contains("Ork"))
                    {
                        list = DefDatabase<PawnKindDef>.AllDefs.Where(x => x.defName.Contains("Ork")).ToList();
                        if (defName.Contains("Choppa"))
                        {
                            list = list.Where(x => x.defName.Contains("Choppa")).ToList();
                        }
                        else
                        if (defName.Contains("Shoota"))
                        {
                            list = list.Where(x => x.defName.Contains("Shoota")).ToList();
                        }
                        else
                        if (defName.Contains("Slugga"))
                        {
                            list = list.Where(x => x.defName.Contains("Slugga")).ToList();
                        }

                        if (defName.Contains("Mek"))
                        {
                            list = list.Where(x => x.defName.Contains("Mek")).ToList();
                        }
                        if (defName.Contains("Warboss"))
                        {
                            list = list.Where(x => x.defName.Contains("Warboss")).ToList();
                        }
                        else
                        if (defName.Contains("Nob"))
                        {
                            list = list.Where(x => x.defName.Contains("Nob")).ToList();
                        }
                        else
                        {
                            list = list.Where(x => !x.defName.Contains("Nob") && !x.defName.Contains("Warboss")).ToList();
                        }
                        __result = list.RandomElement().defName;
                        
                    }
                    if (defName.Contains("Grot"))
                    {
                        list = DefDatabase<PawnKindDef>.AllDefs.Where(x => x.defName.Contains("Grot")).ToList();
                        if (defName.Contains("Colonist"))
                        {
                            list = list.Where(x => x.defName.Contains("Colonist")).ToList();
                        }
                        __result = list.RandomElement().defName;
                    }
                    if (defName.Contains("Snotling"))
                    {
                        list = DefDatabase<PawnKindDef>.AllDefs.Where(x => x.defName.Contains("Snotling")).ToList();
                        
                        __result = list.RandomElement().defName;
                    }
                    if (defName.Contains("Squig"))
                    {
                        list = DefDatabase<PawnKindDef>.AllDefs.Where(x => x.defName.Contains("Squig")).ToList();
                        
                        __result = list.RandomElement().defName;
                    }
                    if (defName.Contains("Tau"))
                    {
                        list = DefDatabase<PawnKindDef>.AllDefs.Where(x => x.defName.Contains("Tau")).ToList();
                        if (defName.Contains("Aun"))
                        {
                            list = list.Where(x => x.defName.Contains("Aun")).ToList();
                        }
                        if (defName.Contains("Shas"))
                        {
                            list = list.Where(x => x.defName.Contains("Shas")).ToList();
                        }
                        if (defName.Contains("Kor"))
                        {
                            list = list.Where(x => x.defName.Contains("Kor")).ToList();
                        }
                        if (defName.Contains("Por"))
                        {
                            list = list.Where(x => x.defName.Contains("Por")).ToList();
                        }
                        if (defName.Contains("Fio"))
                        {
                            list = list.Where(x => x.defName.Contains("Fio")).ToList();
                        }
                        if (defName.Contains("Colonist"))
                        {
                            list = list.Where(x => x.defName.Contains("Colonist")).ToList();
                        }
                        __result = list.RandomElement().defName;
                    }
                    if (defName.Contains("Kroot"))
                    {
                        list = DefDatabase<PawnKindDef>.AllDefs.Where(x => x.defName.Contains("Kroot")).ToList();
                        if (defName.Contains("Shaper"))
                        {
                            list = list.Where(x => x.defName.Contains("Shaper")).ToList();
                        }
                        if (defName.Contains("Colonist"))
                        {
                            list = list.Where(x => x.defName.Contains("Colonist")).ToList();
                        }
                        __result = list.RandomElement().defName;
                    }
                    if (defName.Contains("Guevesa"))
                    {
                        list = DefDatabase<PawnKindDef>.AllDefs.Where(x => x.defName.Contains("Guevesa")).ToList();

                        if (defName.Contains("Colonist"))
                        {
                            list = list.Where(x => x.defName.Contains("Colonist")).ToList();
                        }
                        __result = list.RandomElement().defName;
                    }
                    if (defName.Contains("Eldar"))
                    {
                        list = DefDatabase<PawnKindDef>.AllDefs.Where(x => x.defName.Contains("Eldar")).ToList();
                        if (defName.Contains("Exarch"))
                        {
                            list = list.Where(x => x.defName.Contains("Exarch")).ToList();
                        }
                        if (defName.Contains("Colonist"))
                        {
                            list = list.Where(x => x.defName.Contains("Colonist")).ToList();
                        }
                        __result = list.RandomElement().defName;
                    }

                }
                if (defType == typeof(ResearchProjectDef))
                {
                    if (defName == "ImperialTechBase")
                    {
                        __result = "OG_Tech_Base_Imperial";
                    }
                    if (defName == "WRPowerWeapons")
                    {
                        __result = "OG_Weapons_Power_Imperial";
                    }
                    if (defName == "WGRConversionField")
                    {
                        __result = "OG_Wargear_ConversionField_Imperial";
                    }
                    if (defName == "ImperialSpecialWeapons")
                    {
                        __result = "OG_Weapons_Special_Imperial";
                    }
                    if (defName == "ImperialSpecialPowerWeapons")
                    {
                        __result = "OG_Weapons_SpecialPower_Imperial";
                    }
                    if (defName == "ImperialHeavyWeapons")
                    {
                        __result = "OG_Weapons_Heavy_Imperial";
                    }
                    if (defName == "WRForceWeapons")
                    {
                        __result = "OG_Weapons_Force_Imperial";
                    }
                    if (defName == "WRImpLasTech")
                    {
                        __result = "OG_Weapons_Laser_Imperial";
                    }
                    if (defName == "WRImpBoltTech")
                    {
                        __result = "OG_Weapons_Bolter_Imperial";
                    }
                    if (defName == "WRImpPlasmaTech")
                    {
                        __result = "OG_Weapons_Plasma_Imperial";
                    }
                    if (defName == "ARBasicServoSkull")
                    {
                        __result = "OG_Wargear_ServoSkull_Imperial";
                    }
                    if (defName == "MechanicusTechBase")
                    {
                        __result = "OG_Tech_Base_Mechanicus";
                    }
                    if (defName == "WRRadiumWeapons")
                    {
                        __result = "OG_Weapons_Radium_Mechanicus";
                    }
                    if (defName == "WRMechAdvBallistics")
                    {
                        __result = "OG_Weapons_AdvBallistics_Mechanicus";
                    }
                    if (defName == "WRMechanicusPlasma")
                    {
                        __result = "OG_Weapons_Plasma_Mechanicus";
                    }
                }
                if (defType == typeof(HediffDef))
                {
                    if (defName == "HyperactiveNymuneOrgan")
                    {
                        __result = "OG_Kroot_Mutation_HyperactiveNymuneOrgan";
                    }
                }
                if (defName == __result)
                {
                //    Log.Warning(string.Format("AMA No replacement found for: {0} T:{1}", defName, defType));
                }
                else
                {
                //    Log.Message(string.Format("Replacement found: {0} T:{1}", __result, defType));
                }
            }
        }
    }

}
