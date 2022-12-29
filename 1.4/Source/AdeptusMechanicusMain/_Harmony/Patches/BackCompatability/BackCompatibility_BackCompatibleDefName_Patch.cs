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
using System.Text.RegularExpressions;

namespace AdeptusMechanicus.HarmonyInstance
{

    [HarmonyPatch(typeof(BackCompatibility), "BackCompatibleDefName")]
    public static class BackCompatibility_BackCompatibleDefName_Patch
    {
        [HarmonyPostfix]
        public static void BackCompatibleDefName_Postfix(Type defType, string defName, bool forDefInjections, ref string __result)
        {
            if (GenDefDatabase.GetDefSilentFail(defType, defName, false) == null)
            {
                string newName = string.Empty;
            //    Log.Message(string.Format("Checking for replacement for {0} Type: {1}", defName, defType));
                if (defType == typeof(ThingDef))
                {
                    if (defName.Contains("Ammo_OG"))
                    {
                        newName = defName.Replace("Ammo_OG", "OG_Ammo_");
                    }
                    if (defName.Contains("OGAM_Apparel_SkitariiLegionnaireHelmet"))
                    {
                        newName = "OGAM_Apparel_SkitariiRangerHelmet";
                    }
                    if (defName.Contains("OGAM_Apparel_SkitariiPrimusHelmet"))
                    {
                        newName = "OGAM_Apparel_SkitariiVanguardHelmet";
                    }
                    if (defName.Contains("OG_Cybork_Ork"))
                    {
                        __result = Regex.Replace(defName, "OG_Cybork_Ork", "OG_Alien_Ork");
                        return;
                    }
                    if (defName.Contains("OGIG_Apparel_Armageddon_FlakHelmet"))
                    {
                        __result = "OGIG_Apparel_Armageddon_FlakHelmet";
                        return;
                    }
                    if (defName.Contains("OGIG_Apparel_Krieg_FlakHelmet"))
                    {
                        __result = "OGIG_Apparel_Krieg_FlakHelmet";
                        return;
                    }
                    if (defName.Contains("OGIG_Apparel_GasMask"))
                    {
                        __result = "OGIG_Apparel_Cadian_GasMask";
                        return;
                    }
                    if (defName.Contains("OG_Squig_Ork"))
                    {
                        __result = Regex.Replace(defName, "OG_Squig_Ork", "OG_Squig");
                        return;
                    }
                    if (defName.Contains("OG_Kroothound_Kindred"))
                    {
                        __result = Regex.Replace(defName, "OG_Kroothound_Kindred", "OG_Kroothound");
                        return;
                    }
                    if (defName.Contains("OG_KrootOx_Kindred"))
                    {
                        __result = Regex.Replace(defName, "OG_KrootOx_Kindred", "OG_KrootOx");
                        return;
                    }
                    if (defName.Contains("OG_Knarloc_Kindred"))
                    {
                        __result = Regex.Replace(defName, "OG_Knarloc_Kindred", "OG_Knarloc");
                        return;
                    }
                    if (defName.Contains("OG_Alien_Eldar"))
                    {
                        __result = Regex.Replace(defName, "OG_Alien_Eldar", "OG_Alien_Aeldari");
                        return;
                    }
                    if (defName.Contains("OG_Alien_DarkEldar"))
                    {
                        __result = Regex.Replace(defName, "OG_Alien_DarkEldar", "OG_Alien_Aeldari");
                        return;
                    }
                    if (defName.Contains("OGIG_Apparel_Armageddon_GasMask"))
                    {
                        __result = "OGIG_Apparel_Armageddon_Gasmask";
                        return;
                    }
                    if (defName.Contains("OGI_Gun_BoltGun_GodwynDeaz"))
                    {
                        __result = "OGAS_Gun_BoltGun_GodwynDeaz";
                        return;
                    }
                    if (defName.Contains("OGIG_Apparel_CadianFlakHelmet"))
                    {
                        __result = "OGIG_Apparel_FlakHelmetLight";
                        return;
                    }
                    if (defName.Contains("OGI_Gun_BoltGun_GodwynDeaz"))
                    {
                        __result = "OGAS_Gun_BoltGun_GodwynDeaz";
                        return;
                    }
                    if (defName.Contains("OGE_Apparel_"))
                    {
                        newName = defName;
                        if (newName.Contains("ArmourHelmet"))
                        {
                            newName = newName.Replace("ArmourHelmet", "Helmet");
                        }
                        if (defName.Contains("DireAvenger"))
                        {
                            newName = newName.Replace("DireAvenger", "Avenger");
                        }
                        else if (defName.Contains("StrikingScorpion"))
                        {
                            newName = newName.Replace("StrikingScorpion", "Scorpion");
                        }
                        else if (defName.Contains("OGE_Apparel_RuneArmour"))
                        {
                            newName += "Warlock";
                        }
                        if (!newName.NullOrEmpty())
                        {
                            __result = newName;
                            return;
                        }

                    }
                    if (defName == "OG_Cyborg_Ork")
                    {
                        __result = "OG_Alien_Ork";
                        return;
                    }
                    if (defName == "OG_Chaos_Deamon_Horror")
                    {
                        __result = "OG_Chaos_Deamon_Horror_Pink";
                        return;
                    }
                    if (defName == "OGN_Gun_SynapticDisintergrator")
                    {
                        __result = "OGN_Gun_SynapticDisintegrator";
                        return;
                    }
                    if (defName == "OGDE_Melee_WytchKnife")
                    {
                        __result = "OGDE_Melee_WychKnife";
                        return;
                    }
                    if (defName == "OGIG_Apparel_BasicFlakHelm")
                    {
                        __result = "OGIG_Apparel_Cadia_FlakHelmet_TOGGLEDEF_GogglesUp";
                        return;
                    }
                    if (defName == "OGIG_Apparel_GoggledFlakHelm")
                    {
                        __result = "OGIG_Apparel_Cadia_FlakHelmet_TOGGLEDEF_GogglesDown";
                        return;
                    }
                    if (defName == "OGDE_Gun_Shardcarbine")
                    {
                        __result = "OGDE_Gun_ShardCarbine";
                        return;
                    }
                    if (defName == "OGDE_Gun_DisintergratorCannon")
                    {
                        __result = "OGDE_Gun_DisintegratorCannon";
                        return;
                    }
                    if (defName == "OGDE_Bullet_DisintergratorCannon")
                    {
                        __result = "OGDE_Bullet_DisintegratorCannon";
                        return;
                    }
                    if (defName == "OGK_Apparel_TribalKroot")
                    {
                        __result = "OGK_Apparel_Tribalwear";
                        return;
                    }
                    if (defName == "Apparel_TribalKrootHeaddress")
                    {
                        __result = "OGK_Apparel_TribalHeaddress";
                        return;
                    }
                    if (defName == "OGIG_Apparel_LightFlakArmour")
                    {
                        __result = "OGIG_Apparel_FlakArmourLight";
                        return;
                    }
                    if (defName == "OGIG_Apparel_CadianFlakGreaves")
                    {
                        __result = "OGIG_Apparel_FlakArmourLightGreaves";
                        return;
                    }
                    if (defName.Contains("BoltGun"))
                    {
                        if (defName.Contains("Gun_CM") || defName.Contains("Gun_TG"))
                        {
                            newName = "OGC_Gun_BoltGun";
                        }
                        else
                        {
                            if (defName.Contains("T3BoltGun"))
                            {
                                newName = "OGI_Gun_T3BoltGun";
                            }
                            else if (defName.Contains("T3BoltGun"))
                            {
                                newName = "OGI_Gun_T2BoltGun";
                            }
                            else
                            newName = "OGI_Gun_BoltGun";
                        }
                    }
                    if (defName.Contains("IGStd"))
                    {
                        if (defName.Contains("ArmorMk"))
                        {
                            newName = "OGIG_Apparel_FlakArmour";
                        }
                        else
                        if (defName.Contains("HelmetMk"))
                        {
                            newName = "OGIG_Apparel_CadianFlakHelmet";
                        }
                        else
                        if (defName.Contains("CaraArmorMk"))
                        {
                            newName = "OGIG_Apparel_TempestusScion_CarapaceArmour";
                        }
                        else
                        if (defName.Contains("CaraHelmetMk"))
                        {
                            newName = "OGIG_Apparel_TempestusScion_CarapaceHelmet";
                        }
                    }

                    if (defName == "IGBrain")
                    {
                        newName = "Brain";
                    }
                    if (defName == "LAP_Brain" || defName == "HAA_Brain")
                    {
                        newName = "Brain";
                    }
                    if (defName == "IG_ARM_KriegHelmet")
                    {
                        newName = "OGIG_Apparel_Krieg_FlakHelmet_TOGGLEDEF_A";
                    }
                    if (defName == "IG_ARM_KriegArmor")
                    {
                        newName = "OGIG_Apparel_FlakArmour";
                    }
                    if (defName == "IG_Gun_T3PlasmaGun")
                    {
                        newName = "OGI_Gun_T3PlasmaGun";
                    }
                    else
                    if (defName.Contains("PlasmaRifle"))
                    {

                        if (defName.Contains("Gun_CM") || defName.Contains("Gun_TG"))
                        {
                            newName = "OGC_Gun_PlasmaGun";
                        }
                        else
                        {
                            newName = "OGI_Gun_PlasmaGun";
                        }
                    }
                    if (defName.Contains("LasPistol"))
                    {
                        if (defName.Contains("Gun_CM") || defName.Contains("Gun_TG"))
                        {
                            newName = "OGC_Gun_LasPistol";
                        }
                        else
                        {
                            newName = "OGI_Gun_LasPistol";
                        }
                    }
                    if (defName.Contains("LasGun") || defName.Contains("Lasgun"))
                    {
                        if (!defName.Contains("Bullet"))
                        {
                            if (defName.Contains("Gun_CM") || defName.Contains("Gun_TG"))
                            {
                                newName = "OGC_Gun_LasGun";
                            }
                            else
                            {
                                if (defName.Contains("Lucius"))
                                {
                                    newName = "OGI_Gun_LasGun_Lucius";
                                }
                                else
                                    newName = "OGI_Gun_LasGun";
                            }
                            if (GenDefDatabase.GetDefSilentFail(defType, newName, false) == null)
                            {
                                newName = DefDatabase<ThingDef>.AllDefs.Where(x => x.defName.Contains("LasGun") || x.defName.Contains("Lasgun") && (!x.defName.Contains("Lucius") || (defName.Contains("Lucius") && x.defName.Contains("Lucius"))) && !x.defName.Contains("Bullet")).ToList().First().defName;
                            }
                        }
                    }
                    if (defName == "IG_Melee_ChaosChainsword")
                    {
                        newName = "OGC_Melee_ChainSword";
                    }
                    if (defName == "Melee_Chainsword")
                    {
                        newName = "OGI_Melee_ChainSword";
                    }
                    if (defName == "IG_Melee_ChaosPowerAxe")
                    {
                        newName = "OGC_Melee_PowerAxe";
                    }
                    if (defName == "Melee_PowerAxe")
                    {
                        newName = "OGI_Melee_PowerAxe";
                    }
                    if (defName == "Melee_ForceSword")
                    {
                        newName = "OGI_Melee_ForceSword";
                    }
                    if (defName == "Melee_ThunderHammer")
                    {
                        newName = "OGI_Melee_ThunderHammer";
                    }
                    if (defName == "Gun_ExitusSniper")
                    {
                        newName = "OGI_Gun_ExitusRifle";
                    }
                    if (defName == "IG_Gun_AssCan")
                    {
                        newName = "OGI_Gun_AssaultCannon";
                    }
                    if (defName == "Melee_ForceSword")
                    {
                        newName = "OGI_Melee_ForceSword";
                    }
                    if (defName.Contains("ChaosDeamon_"))
                    {
                        if (defName.Contains("Corpse_"))
                        {
                            newName = Regex.Replace(defName, "Corpse_ChaosDeamon_", "Corpse_OG_Chaos_Deamon_");
                        }
                        else
                            newName = Regex.Replace(defName, "ChaosDeamon_", "OG_Chaos_Deamon_");
                    }
                    if (defName.Contains("OGO_Bullet_O"))
                    {
                        newName = Regex.Replace(defName, "OGO_Bullet_O", "OGO_Bullet_");
                    }
                    if (defName.Contains("OG_AstartesOrgans_"))
                    {
                        if (defName.Contains("ProgenoidGland"))
                        {
                            newName = "OG_Zygote_Organ_ProgenoidGlands"; //OG_Hediff_AstartesOrgans_
                        }
                        else
                        newName = Regex.Replace(defName, "OG_AstartesOrgans_", "OG_Zygote_Organ_");
                    }
                    if (defName.Contains("OGK_Bullet_Hunter"))
                    {
                        if (defName.Contains("Pulse"))
                        {
                            newName = "OGK_Bullet_HunterPulse";
                        }
                        else
                        newName = "OGK_Bullet_HunterSolid";
                    }
                    if (defName == "OG_Ork_TableMachining")
                    {
                        newName = "OGO_TableMachining";
                    }
                    if (defName == "OG_Tau_TableMachining")
                    {
                        newName = "OGT_TableMachining";
                    }
                    if (defName == "OG_Eldar_TableMachining")
                    {
                        newName = "OGE_TableMachining";
                    }
                    if (defName == "GlowPodLike")
                    {
                        newName = "GlowPod";
                    }
                    if (defName == "IG_Aug_GENE")
                    {
                        newName = "OG_Astartes_Geneseed";
                    }
                    if (defName == "OG_AA_Building_OrganVat")
                    {
                        newName = "OGAA_Building_OrganVat";
                    }
                    if (defName == "OGIG_Apparel_CarapaceArmourTS")
                    {
                        newName = "OGIG_Apparel_TempestusScion_CarapaceArmour";
                    }
                    if (defName == "OGIG_Apparel_CarapaceHelmTS")
                    {
                        newName = "OGIG_Apparel_TempestusScion_CarapaceHelmet";
                    }
                    if (defName == "OGAM_Apparel_SkitariiLegionnaireHelmet")
                    {
                        newName = "OGAM_Apparel_SkitariiLegionnaireHelmet_TOGGLEDEF_Hooded";
                    }
                    else
                    if (defName == "OGAM_Apparel_SkitariiPrimusHelmet")
                    {
                        newName = "OGAM_Apparel_SkitariiPrimusHelmet_TOGGLEDEF_Unhooded";
                    }
                    else
                    if (defName.Contains("OGT_Gun_TKroot"))
                    {
                        newName = Regex.Replace(defName, "OGT_Gun_TKroot", "OGK_Gun_Kroot");
                    }
                    else
                    if (defName.Contains("Alien_Kroot"))
                    {
                        newName = "OG_Alien_Kroot";
                    }
                    else
                    if (defName.Contains("Alien_Tau"))
                    {
                        newName ="OG_Alien_Tau";
                    }
                    else
                    if (defName.Contains("Alien_Ork"))
                    {
                        newName ="OG_Alien_Ork";
                    }
                    else
                    if (defName.Contains("Cybork"))
                    {
                        newName ="OG_Alien_Cybork";
                    }
                    else
                    if (defName.Contains("Alien_Cybork"))
                    {
                        newName ="OG_Alien_Cybork";
                    }
                    else
                    if (defName.Contains("Alien_Grot"))
                    {
                        newName ="OG_Alien_Grot";
                    }
                    else
                    if (defName.Contains("Alien_Eldar"))
                    {
                        newName ="OG_Alien_Eldar";
                    }
                    else
                    if (defName.Contains("AttackSquig"))
                    {
                        newName ="OG_Squig_Ork";
                    }
                    else
                    if (defName.Contains("Squig"))
                    {
                        newName ="OG_Squig";
                    }
                    else
                    if (defName.Contains("Snotling"))
                    {
                        newName ="OG_Snotling";
                    }

                    if (defName.Contains("OG_Knarloc_Kroot"))
                    {
                        newName = "OG_Knarloc_Kindred";
                    }

                    if (defName.Contains("OG_Kroot_Hound"))
                    {
                        newName = "OG_Kroothound";
                    }

                    if (defName.Contains("KindredKrootHound"))
                    {
                        newName = "OG_Kroothound_Kindred";
                    }

                    if (defName == "OGE_Gun_EBrightlance")
                    {
                        newName = "OGE_Gun_Brightlance";
                    }
                    if (defName == "OGAM_Gun_CognisFlamer")
                    {
                        newName = "OGAM_Gun_FlamerCognis";
                    }
                    if (defName == "OGAM_Gun_Transuranic_Arquebus")
                    {
                        newName = "OGAM_Gun_TransuranicArquebus";
                    }


                    if (defName.Contains("OGC_Melee_") && defName.Contains("LightningClaw"))
                    {
                        if (defName.Contains("Dual"))
                        {
                            newName = "OGC_Melee_LightningClawD";
                        }
                        else if (defName.Contains("Single"))
                        {
                            newName = "OGC_Melee_LightningClawS";
                        }
                    }
                    if (defName.Contains("OGI_Gun_") && defName.Contains("Flamer"))
                    {
                        if (defName.Contains("Hand"))
                        {
                            newName = "OGI_Gun_FlamerHand";
                        }
                        else if (defName.Contains("Heavy"))
                        {
                            newName = "OGI_Gun_FlamerHeavy";
                        }
                    }

                    if (defName.Contains("Kroot") || defName.Contains("Knarloc"))
                    {
                        if (defName.Contains("ox") || defName.Contains("Ox"))
                        {
                            newName = "OG_KrootOx";
                        }
                        else
                        if (defName.Contains("hound") || defName.Contains("Hound"))
                        {
                            newName = "OG_Kroothound";
                        }
                        else
                        if (defName.Contains("knarloc") || defName.Contains("Knarloc"))
                        {
                            newName = "OG_Knarloc";
                        }
                        else
                        {
                            if (!defName.Contains("_Gun_") && !defName.Contains("_Melee_") && !defName.Contains("_Apparel_") && !defName.Contains("_Armour_") && !defName.Contains("_Armor_") && !defName.Contains("_Wargear_"))
                            {
                                newName = "OG_Alien_Kroot";
                            }
                        }

                        if (defName.Contains("Corpse_"))
                        {
                            newName = "Corpse_" + newName;
                        }
                        if (defName.Contains("Meat_"))
                        {
                            newName = "Meat_" + newName;
                        }
                        else
                        if (defName.Contains("_Kindred"))
                        {
                            newName = newName + "_Kindred";
                        }
                    }

                    if (defName == "Meat_OG_Tau_GunDrone")
                    {
                        newName = "Steel";
                    }
                    if (defName == "OG_Abhuman_Ratling")
                    {
                        newName = "OG_Abhuman_Ratlin";
                    }
                    if (defName == "Corpse_OG_Abhuman_Ratling")
                    {
                        newName = "Corpse_OG_Abhuman_Ratlin";
                    }
                    if (defName == "OG_Human_Imperial" || defName == "OG_Human_ELT")
                    {
                        newName = "Human";
                    }
                    if (defName == "Corpse_OG_Human_Imperial" || defName == "Corpse_OG_Human_ELT")
                    {
                        newName = "Corpse_Human";
                    }
                    if (defName == "Tau_Kroot_Warrior")
                    {
                        newName = "OG_Alien_Kroot";
                    }
                    if (defName == "Corpse_Tau_Kroot_Warrior")
                    {
                        newName = "Corpse_OG_Alien_Kroot";
                    }

                    if (defName != __result)
                    {
                        if (defName.Contains("Corpse"))
                        {
                            newName ="Corpse_" + __result;
                        }
                        if (defName.Contains("Meat"))
                        {
                            if (defName.Contains("Cyborg_Ork"))
                            {
                                newName ="Meat_OG_Alien_Ork";
                            }
                            else
                            if (defName.Contains("Squig"))
                            {
                                newName ="Meat_OG_Squig";
                            }
                            else
                            newName ="Meat_" + __result;
                        }
                    }
                    if (defName.Contains("OrkGrog"))
                    {
                        newName ="OG_Ork_Grog";
                    }
                    if (defName.Contains("Plant_OrkFungus"))
                    {
                        newName ="OG_Plant_OrkoidFungus";
                    }
                    if (defName.Contains("Meat_Cyborg_Ork"))
                    {
                        newName ="Meat_OG_Alien_Ork";
                    }
                    if (defName.Contains("CadianFlakArmour"))
                    {
                        newName ="OGIG_Apparel_FlakArmour";
                    }
                    if (defName.Contains("Rosarius"))
                    {
                        newName =DefDatabase<ThingDef>.AllDefs.Where(x => x.defName.Contains("Rosarius")).ToList().First().defName;
                    }
                    if (defName.Contains("IronHalo"))
                    {
                        newName =DefDatabase<ThingDef>.AllDefs.Where(x => x.defName.Contains("IronHalo")).ToList().First().defName;
                    }
                    if (defName.Contains("PuritySealA"))
                    {
                        newName =DefDatabase<ThingDef>.AllDefs.Where(x => x.defName.Contains("PuritySealA")).ToList().First().defName;
                    }
                    if (defName.Contains("Apparel_TribalKroot"))
                    {
                        newName ="OGK_Apparel_TribalKroot";
                    }
                    
                    if (defName.Contains("OGT_CombatHelmet"))
                    {
                        newName ="OGT_Apparel_CombatHelmet";
                    }
                    if (defName.Contains("OGT_CombatArmour"))
                    {
                        newName ="OGT_Apparel_CombatArmour";
                    }
                    if (defName.Contains("OGCDWarpRift"))
                    {
                        newName =DefDatabase<ThingDef>.AllDefs.Where(x=> x.defName.Contains("Warp") && x.defName.Contains("Rift")).RandomElement().defName;
                    }

                    if (defName.Contains("OGC_Gun_C"))
                    {
                        newName = Regex.Replace(defName, "OGC_Gun_C", "OGC_Gun_");
                    }
                    if (defName.Contains("OGC_Melee_C"))
                    {
                        newName = Regex.Replace(defName, "OGC_Melee_C", "OGC_Melee_");
                    }

                    if (defName.Contains("OGE_Gun_E"))
                    {
                        newName = Regex.Replace(defName, "OGE_Gun_E", "OGE_Gun_");
                    }
                    if (defName.Contains("OGE_Melee_E"))
                    {
                        newName = Regex.Replace(defName, "OGE_Melee_E", "OGE_Melee_");
                    }

                    if (defName.Contains("OGN_Gun_N"))
                    {
                        newName = Regex.Replace(defName, "OGN_Gun_N", "OGN_Gun_");
                    }
                    if (defName.Contains("OGN_Melee_N"))
                    {
                        newName = Regex.Replace(defName, "OGN_Melee_N", "OGN_Melee_");
                    }

                    if (defName.Contains("OGO_Gun_O"))
                    {
                        newName = Regex.Replace(defName, "OGO_Gun_O", "OGO_Gun_");
                    }
                    if (defName.Contains("OGO_Melee_O"))
                    {
                        newName = Regex.Replace(defName, "OGO_Melee_O", "OGO_Melee_");
                    }

                    if (defName.Contains("OGT_Gun_T"))
                    {
                        newName = Regex.Replace(defName, "OGT_Gun_T", "OGT_Gun_");
                    }
                    if (defName.Contains("OGT_Melee_T"))
                    {
                        newName = Regex.Replace(defName, "OGT_Melee_T", "OGT_Melee_");
                    }
                    /*
                    if (defName.Contains("OGT_Gun_") && defName.Contains("NeutronBlaster"))
                    {

                    }
                    */
                }
                else if (defType == typeof(FactionDef))
                {
                    if (defName == "OGChaosDeamonFaction")
                    {
                        newName ="OG_Chaos_Deamon_Faction";
                    }
                    if (defName == "OG_GreenskinTribal_Culture")
                    {
                        newName = "OG_Ork_Feral_Faction";
                    }
                    if (defName == "OG_Ork_Hulk" || defName == "OG_Ork_Rok")
                    {
                        newName = "OG_Ork_Waaagh";
                    }
                    if (defName == "MechanicusFaction")
                    {
                        newName ="OG_Mechanicus_Faction";
                    }
                    if (defName == "NecronFaction")
                    {
                        newName ="OG_Necron_Faction";
                    }
                    // Eldar Factions
                    if (defName == "EldarFaction")
                    {
                        newName ="OG_Eldar_Craftworld_Faction";
                    }
                    if (defName == "OG_Eldar_Player_Craftworld")
                    {
                        newName = "OG_Eldar_Craftworld_PlayerColony";
                    }
                    if (defName == "EldarPlayerColony")
                    {
                        newName = "OG_Eldar_Craftworld_PlayerColony";
                    }
                    // Ork factions
                    if (defName == "OrkFaction")
                    {
                        newName ="OG_Ork_Tek_Faction";
                    }
                    if (defName == "FeralOrkFaction")
                    {
                        newName ="OG_Ork_Feral_Faction";
                    }
                    if (defName.Contains("Ork") && defName.Contains("Player"))
                    {
                        if (defName.Contains("Trib"))
                        {
                            newName ="OG_Ork_PlayerTribe";
                        }
                        else
                        {
                            newName ="OG_Ork_PlayerColony";
                        }
                    }
                    if (defName == "RokOrkz")
                    {
                        newName ="OG_Ork_Rok";
                    }
                    if (defName == "HulkOrkz")
                    {
                        newName ="OG_Ork_Hulk";
                    }

                    // Tau factions
                    if (defName == "TauFaction")
                    {
                        newName ="OG_Tau_Faction";
                    }
                    if (defName == "TauPlayerColony")
                    {
                        newName ="OG_Tau_Player";
                    }

                    // Kroot factions
                    if (defName == "KrootFaction")
                    {
                        newName ="OG_Kroot_Faction";
                    }
                    if (defName == "KrootPlayerColonyTribal")
                    {
                        newName ="OG_Kroot_Player_Tribal";
                    }

                }
                else if (defType == typeof(PawnKindDef))
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
                        newName =list.RandomElement().defName;
                        
                    }
                    if (defName.Contains("Grot"))
                    {
                        list = DefDatabase<PawnKindDef>.AllDefs.Where(x => x.defName.Contains("Grot")).ToList();
                        if (defName.Contains("Colonist"))
                        {
                            list = list.Where(x => x.defName.Contains("Colonist")).ToList();
                        }
                        newName =list.RandomElement().defName;
                    }
                    if (defName.Contains("Snotling"))
                    {
                        list = DefDatabase<PawnKindDef>.AllDefs.Where(x => x.defName.Contains("Snotling")).ToList();
                        
                        newName =list.RandomElement().defName;
                    }
                    if (defName.Contains("Squig"))
                    {
                        list = DefDatabase<PawnKindDef>.AllDefs.Where(x => x.defName.Contains("Squig")).ToList();
                        
                        newName =list.RandomElement().defName;
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
                        newName =list.RandomElement().defName;
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
                        newName =list.RandomElement().defName;
                    }
                    if (defName.Contains("Guevesa"))
                    {
                        list = DefDatabase<PawnKindDef>.AllDefs.Where(x => x.defName.Contains("Guevesa")).ToList();

                        if (defName.Contains("Colonist"))
                        {
                            list = list.Where(x => x.defName.Contains("Colonist")).ToList();
                        }
                        newName =list.RandomElement().defName;
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
                        newName =list.RandomElement().defName;
                    }
                    if (defName == "StrangerInBlackImperiualGuard") newName = "StrangerInBlack_Militarum";
                }
                else if (defType == typeof(ResearchProjectDef))
                {
                    // Common Reseach renames
                    if (defName == "WRPowerWeapons" || defName == "OG_Weapons_Power_Imperial")
                    {
                        newName = "OG_Common_Tech_Weapons_Powered";
                    }
                    if (defName == "ImperialSpecialPowerWeapons" || defName == "OG_Weapons_SpecialPower_Imperial")
                    {
                        newName = "OG_Common_Tech_Weapons_Powered_Special";
                    }
                    if (defName == "WRForceWeapons" || defName == "OG_Weapons_Force_Imperial")
                    {
                        newName = "OG_Common_Tech_Weapons_Force";
                    }
                    if (defName == "ImperialSpecialWeapons" || defName == "OG_Weapons_Special_Imperial")
                    {
                        newName = "OG_Common_Tech_Weapons_Special";
                    }
                    if (defName == "ImperialHeavyWeapons" || defName == "OG_Weapons_Heavy_Imperial")
                    {
                        newName = "OG_Common_Tech_Weapons_Heavy";
                    }
                    if (defName == "WRImpLasTech" || defName == "WRLasPistol" || defName == "WRLasGun" || defName == "OG_Weapons_Laser_Imperial")
                    {
                        newName = "OG_Common_Tech_Weapons_Laser";
                    }
                    if (defName == "WRImpBoltTech" ||defName == "WRBoltGun" || defName == "OG_Weapons_Bolter_Imperial")
                    {
                        newName = "OG_Imperial_Tech_Weapons_Bolt";
                    }
                    if (defName == "WRImpPlasmaTech" ||defName == "WRPlasmaRifle" || defName == "OG_Weapons_Plasma_Imperial")
                    {
                        newName = "OG_Common_Tech_Weapons_Plasma";
                    }
                    if (defName == "WRExitusSniper")
                    {
                        newName = "OG_Assassinorum_Tech_Weapons_Vindicare";
                    }
                    // Imperial Reseach renames
                    if (defName == "ImperialTechBase" || defName == "OG_Tech_Base_Imperial" || defName == "OG_Imperial_Tech_Base_T1")
                    {
                        newName = "OG_Imperial_Tech_Base_T0";
                    }
                    /*
                    if (defName == "OG_Weapons_Base_Imperial")
                    {
                        newName = "OG_Imperial_Tech_Weapons_T1";
                    }
                    */
                    if (defName == "WGRConversionField" || defName == "OG_Wargear_ConversionField_Imperial")
                    {
                        newName ="OG_Imperial_Tech_Wargear_Shield";
                    }
                    if (defName == "ARBasicServoSkull" || defName == "OG_Wargear_ServoSkull_Imperial")
                    {
                        newName ="OG_Imperial_Tech_Wargear_ServoSkull";
                    }
                    // Mechanicus Reseach renames
                    if (defName == "MechanicusTechBase" || defName == "OG_Tech_Base_Mechanicus")
                    {
                        newName = "OG_Mechanicus_Tech_Base_T1";
                    }
                    if (defName == "WRRadiumWeapons" || defName == "OG_Weapons_Radium_Mechanicus")
                    {
                        newName = "OG_Mechanicus_Tech_Weapons_Radium";
                    }
                    if (defName == "WRMechAdvBallistics" || defName == "OG_Weapons_AdvBallistics_Mechanicus")
                    {
                        newName = "OG_Mechanicus_Tech_Weapons_AdvancedBallistics";
                    }
                    /*
                    if (defName == "WRMechanicusPlasma" || defName == "OG_Weapons_Plasma_Mechanicus")
                    {
                        newName = "OG_Mechanicus_Tech_Weapons_Ranged_T3";
                    }
                    */
                    // Eldar Reseach renames
                    if (defName == "EldarTechBase")
                    {
                        newName = "OG_Eldar_Tech_Base_T1";
                    }
                    if (defName == "EldarBasicWeaponsTech" || defName == "OG_Eldar_Tech_Weapons_Ranged_T1")
                    {
                        newName = "OG_Eldar_Tech_Weapons_Shuriken"; 
                    }
                    if (defName == "EldarAdvancedWeapons" || defName == "OG_Eldar_Tech_Weapons_Ranged_T2")
                    {
                        newName = "OG_Aeldari_Tech_Weapons_Monofilament";
                    }
                    if (defName == "EldarHeavyWeapons" || defName == "OG_Eldar_Tech_Weapons_Ranged_T3")
                    {
                        newName = "OG_Eldar_Tech_Weapons_Vortex";
                    }
                    if (defName == "EldarWraithTech")
                    {
                        newName = "OG_Eldar_Tech_Base_T2";
                    }
                    /*
                    if (defName == "EldarBasicMeleeTech")
                    {
                        newName = "OG_Eldar_Tech_Weapons_Melee_T1"; 
                    }
                    */
                    if (defName == "EldarPowerWeapons")
                    {
                        newName = "OG_Common_Tech_Weapons_Powered";
                    }
                    if (defName == "EldarAdvancedMelee" || defName == "OG_Eldar_Tech_Weapons_Melee_T3")
                    {
                        newName = "OG_Eldar_Tech_Weapons_Witchblade";
                    }
                    if (defName == "EldarArmour" || defName == "OG_Eldar_Tech_Apparel_Armour_T1")
                    {
                        newName = "OG_Aeldari_Tech_Apparel_Armour_T1"; 
                    }
                    if (defName == "EldarAspectArmour")
                    {
                        newName = "OG_Eldar_Tech_Apparel_Armour_T2";
                    }
                    if (defName == "EldarAdvancedArmour")
                    {
                        newName = "OG_Eldar_Tech_Apparel_Armour_T3";
                    }

                    // Tau Reseach renames TauTechBase
                    if (defName == "TauTechBase")
                    {
                        newName = "OG_Tau_Tech_Base_T1";
                    }
                    if (defName == "TauPlasmaTech" || defName == "OG_Tau_Tech_Weapons_Ranged_T1")
                    {
                        newName = "OG_Tau_Tech_Weapons_PlasmaPulse";
                    }
                    if (defName == "TauAdvancedWeapons" || defName == "OG_Tau_Tech_Weapons_Ranged_T2")
                    {
                        newName = "OG_Tau_Tech_Weapons_Railgun";
                    }
                    if (defName == "TauIonTech" || defName == "OG_Tau_Tech_Weapons_Ranged_T3")
                    {
                        newName = "OG_Tau_Tech_Weapons_Ion";
                    }
                    if (defName == "TauArmour")
                    {
                        newName = "OG_Tau_Tech_Apparel_Armour_T1";
                    }
                    if (defName == "TauDroneTech" || defName == "OG_Tau_Tech_Wargear_Drone")
                    {
                        newName = "OG_Tau_Tech_Apparel_Wargear_Drone";
                    }
                    if (defName == "TauShieldTech" || defName == "OG_Tau_Tech_Wargear_Shield")
                    {
                        newName = "OG_Tau_Tech_Apparel_Wargear_Shield";
                    }

                    // Ork Reseach renames
                    if (defName == "OrkTekBase")
                    {
                        newName = "OG_Ork_Tech_Base_T1";
                    }
                    if (defName == "OrkishBrutality")
                    {
                        newName = "OG_Ork_Tech_Weapons_Melee_T1";
                    }
                    if (defName == "OrkishExtremeBrutality")
                    {
                        newName = "OG_Ork_Tech_Weapons_Melee_T2";
                    }
                    if (defName == "OrkishPowerField")
                    {
                        newName = "OG_Ork_Tech_Weapons_Melee_T3";
                    }
                    if (defName == "OrkishCunning")
                    {
                        newName = "OG_Ork_Tech_Weapons_Ranged_T1";
                    }
                    if (defName == "OrkishIntenseCunning")
                    {
                        newName = "OG_Ork_Tech_Weapons_Ranged_T2";
                    }
                    if (defName == "OrkishMekTek")
                    {
                        newName = "OG_Ork_Tech_Base_T2";
                    }
                    if (defName == "OrkishBigMekBrainz")
                    {
                        newName = "OG_Ork_Tech_Base_T3";
                    }
                    if (defName == "OrkishArmour")
                    {
                        newName = "OG_Ork_Tech_Apparel_Armour_T1";
                    }
                    if (defName == "OrkishEavyArmour")
                    {
                        newName = "OG_Ork_Tech_Apparel_Armour_T2";
                    }
                    if (defName == "OrkishMegaArmour")
                    {
                        newName = "OG_Ork_Tech_Apparel_Armour_T3";
                    }
                }
                else if (defType == typeof(HediffDef))
                {
                    if (defName == "PlasmaBurn")
                    {
                        newName = "OG_Hediff_PlasmaBurn";
                    }
                    if (defName == "OG_Hediff_PlasmaBurn")
                    {
                        newName = "OG_Burn_Plasma";
                    }
                    if (defName == "LaserBurn")
                    {
                        newName = "OG_Burn_Laser";
                    }
                    if (defName == "MeltaBurn")
                    {
                        newName = "OG_Burn_Melta";
                    }
                    if (defName == "ElectricalBurn")
                    {
                        newName = "OG_Burn_Electrical";
                    }

                    if (defName == "RadiumGunshot")
                    {
                        newName = "OG_Gunshot_Radium";
                    }
                    if (defName == "GalvanicWound")
                    {
                        newName = "OG_Gunshot_Galvanic";
                    }
                    if (defName == "TransuranicWound")
                    {
                        newName = "OG_Gunshot_Transuranic";
                    }
                    if (defName == "GaussWound")
                    {
                        newName = "OG_Gunshot_Gauss";
                    }

                    if (defName == "RadiationPoisioning")
                    {
                        newName = "OG_Hediff_RadiationPoisioning";
                    }
                    if (defName == "FWPsychicShock")
                    {
                        newName = "OG_Hediff_FWPsychicShock";
                    }
                    if (defName == "Regenerated_Part_OG")
                    {
                        newName = "OG_Hediff_Regenerated_Part";
                    }
                    if (defName == "Regenerating_Part_OG")
                    {
                        newName = "OG_Hediff_Regenerating_Part";
                    }
                    if (defName == "HyperactiveNymuneOrgan")
                    {
                        newName = "OG_Kroot_Mutation_HyperactiveNymuneOrgan";
                    }
                    if (defName.Contains("OG_Hediff_AstartesOrgans_"))
                    {
                        if (defName.Contains("ProgenoidGland"))
                        {
                            newName = "OG_Zygote_Hediff_ProgenoidGland"; //OG_Hediff_AstartesOrgans_
                        }
                        else
                            newName = Regex.Replace(defName, "OG_Hediff_AstartesOrgans_", "OG_Zygote_Hediff_");
                    }
                }
                else if (defType == typeof(DamageDef))
                {
                    if (defName == "OGForceStrike")
                    {
                        newName = "OG_ForceStrike";
                    }
                }
                else if (defType == typeof(GameConditionDef))
                {
                    if (defName == "OG_Warpstorm")
                    {
                        newName = "OG_Condition_Warpstorm";
                    }
                }
                else if (defType == typeof(WorkGiverDef))
                {
                    if (defName == "DoBillsImperialMachining" || defName == "DoBillsOrkMachining" || defName == "DoBillsTauMachining" || defName == "DoBillsEldarMachining")
                    {
                        newName = "DoBillsMachiningTable";
                    }
                }
                else if (defType == typeof(BackstoryDef))
                {
                    if (defName=="OG_Imperial_Inquisitor")
                    {
                        newName = "OG_Imperial_Inquisiton_Inquisitor_Neophyte";
                    }
                }
                else if (defType == typeof(BodyDef))
                {
                    if (defName.Contains("Kroot") || defName.Contains("Tau") || defName.Contains("Vespid"))
                    {
                        newName = defName + "_Body";
                    }
                }
                else if (defType == typeof(LifeStageDef))
                {
                    newName = "OG_Lifestage_";
                    if (defName.Contains("Orkoid"))
                    {
                        newName += "Ork_";
                        newName = Regex.Replace(defName, "Orkoid", newName);
                    }
                }
                else if (defType == typeof(HairDef))
                {
                    if (defName.Contains("OG"))
                    {
                        newName = Regex.Replace(defName, "OG", "");
                    }
                }
                else if (defType == typeof(ScenarioDef))
                {
                    if (defName == "OG_Militarum_Start")
                    {
                        newName = "OGAM_Scenario_Militarum_Crashlanded";
                    }
                    if (defName == "OG_Mechanicus_Start")
                    {
                        newName = "OGAM_Scenario_Mechanicus_Crashlanded";
                    }
                    
                    if (defName == "OG_Eldar_Craftworld_Scenario_Test")
                    {
                        newName = "OGAM_Scenario__Eldar_Craftworld_Crashlanded";
                    }

                    if (defName == "OG_Ork_Tek_Scenario_Test")
                    {
                        newName = "OGAM_Scenario__Ork_Crashlanded";
                    }
                    if (defName == "OG_Ork_Feral_Tribe")
                    {
                        newName = "OGAM_Scenario_Ork_LostTribe";
                    }

                    if (defName == "OG_Tau_Crashlanded")
                    {
                        newName = "OGAM_Scenario_Tau_Crashlanded";
                    }
                    if (defName == "OG_Kroot_Lost_Tribe")
                    {
                        newName = "OGAM_Scenario_Kroot_LostTribe";
                    }
                }
                else if (defType == typeof(TraderKindDef))
                {
                    if (defName == "Caravan_Ork_Exotic")
                    {
                        __result = "OG_Ork_Caravan_Exotic";
                        return;
                    }
                }
                else if (defType == typeof(RecipeDef))
                {

                    if (defName.Contains("Ammo_OG"))
                    {
                        newName = defName.Replace("Ammo_OG", "OG_Ammo_");
                    }
                    if (defName == "Make_OGK_Apparel_TribalKroot")
                    {
                        __result = "Make_OGK_Apparel_Tribalwear";
                        return;
                    }
                    if (defName == "Make_Apparel_TribalKrootHeaddress")
                    {
                        __result = "Make_OGK_Apparel_TribalHeaddress";
                        return;
                    }
                }
                if (!newName.NullOrEmpty())
                {
                    __result = newName;
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
