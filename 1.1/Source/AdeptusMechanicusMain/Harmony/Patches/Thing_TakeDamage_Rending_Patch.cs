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
using AdeptusMechanicus.ExtensionMethods;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Thing), "TakeDamage")]
    public static class Thing_TakeDamage_Rending_Patch
    {
        public static void Prefix(Thing __instance, ref DamageInfo dinfo)
        {
            if (__instance != null)
            {
                if (dinfo.Instigator != null)
                {
                    Pawn Attacker = dinfo.Instigator as Pawn;
                    Pawn hitPawn = __instance as Pawn;
                    if (Attacker == null || hitPawn == null)
                    {
                        return;
                    }
                    if (dinfo.Weapon != null)
                    {
                        Log.Message("Thing_TakeDamage_Rending_Patch Prefix " + __instance + " hit by " + dinfo.Weapon);
                        if (AMSettings.Instance.AllowRendingMeleeEffect && dinfo.Def.rendingWeapon())
                        {
                            Log.Message(dinfo.Weapon+" Is Rending");
                        }
                    }
                    else
                    {
                        Log.Message("Thing_TakeDamage_Rending_Patch Prefix " + __instance + " hit by " + dinfo.Instigator);
                    }

                }
            }
        }

        public static void Postfix(Thing __instance, DamageInfo dinfo) 
        {
            if (__instance != null)
            {
                if (dinfo.Instigator != null)
                {
                    Pawn Attacker = dinfo.Instigator as Pawn;
                    Pawn hitPawn = __instance as Pawn;
                    if (Attacker == null || hitPawn == null)
                    {
                        return;
                    }
                    if (dinfo.Weapon!=null)
                    {
                        Log.Message("Thing_TakeDamage_Rending_Patch Postfix " + __instance + " hit by " + dinfo.Weapon);
                        if (AMSettings.Instance.AllowRendingMeleeEffect && dinfo.Def.rendingWeapon())
                        {
                            Log.Message(dinfo.Weapon + " Is Rending");
                        }
                    }
                    else
                    {
                        Log.Message("Thing_TakeDamage_Rending_Patch Postfix " + __instance + " hit by " + dinfo.Instigator);
                    }
                }
            }
        }


    }
}