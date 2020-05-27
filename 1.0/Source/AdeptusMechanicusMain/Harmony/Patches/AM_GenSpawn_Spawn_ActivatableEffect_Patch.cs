﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Harmony;
using Verse.Sound;

namespace AdeptusMechanicus.Harmony
{
    [HarmonyPatch(typeof(GenSpawn), "Spawn")]
    [HarmonyPatch(new Type[] { typeof(Thing), typeof(IntVec3), typeof(Map), typeof(Rot4), typeof(WipeMode), typeof(bool) })]
    public static class AM_GenSpawn_Spawn_ActivatableEffect_Patch
    {
        [HarmonyPostfix]
        public static void SpawnPostfix(Thing newThing)
        {
            if (newThing is Pawn p && p.equipment != null)
            {
                if (p.equipment.Primary != null)
                {
                    if (p.equipment.Primary.TryGetComp<CompPowerWeaponActivatableEffect>() != null && p.equipment.Primary.TryGetComp<CompPowerWeaponActivatableEffect>() is CompPowerWeaponActivatableEffect compPowerWeapon)
                    {
                        bool flag = compPowerWeapon.CurrentState == CompActivatableEffect.CompActivatableEffect.State.Deactivated;
                        if (flag)
                        {
                            compPowerWeapon.TryActivate();
                        }
                    }
                    if (p.equipment.Primary.TryGetComp<CompForceWeaponActivatableEffect>() != null && p.equipment.Primary.TryGetComp<CompForceWeaponActivatableEffect>() is CompForceWeaponActivatableEffect compForceWeapon)
                    {
                        bool flag = compForceWeapon.CurrentState == CompActivatableEffect.CompActivatableEffect.State.Deactivated;
                        if (flag)
                        {
                            compForceWeapon.TryActivate();
                        }
                    }
                    /*
                    if (p.equipment.Primary.TryGetComp<CompAbilityItem>() != null && p.equipment.Primary.TryGetComp<CompAbilityItem>() is CompAbilityItem compAbilityItem)
                    {
                        foreach (var item in compAbilityItem.Abilities)
                        {
                            bool flag = !p.TryGetComp<CompAbilityUser>().AbilityData.TemporaryWeaponPowers.Contains(item);
                            if (flag)
                            {
                                p.TryGetComp<CompAbilityUser>().AddWeaponAbility(item.Def);
                            }
                        }
                    }
                    */
                }
            }
        }
    }
}
