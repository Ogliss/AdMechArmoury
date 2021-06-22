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
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(GenSpawn), "Spawn")]
    [HarmonyPatch(new Type[] { typeof(Thing), typeof(IntVec3), typeof(Map), typeof(Rot4), typeof(WipeMode), typeof(bool) })]
    public static class GenSpawn_Spawn_ActivatableEffect_Patch
    {
        [HarmonyPostfix]
        public static void SpawnPostfix(Thing newThing)
        {
            Pawn p = newThing as Pawn;
            if (p == null)
            {
                return;
            }
            if (p.equipment != null)
            {
                if (p.equipment.Primary != null)
                {
                    if (p.equipment.Primary.TryGetCompFast<CompPowerWeaponActivatableEffect>() != null && p.equipment.Primary.TryGetCompFast<CompPowerWeaponActivatableEffect>() is CompPowerWeaponActivatableEffect compPowerWeapon)
                    {
                        bool flag = compPowerWeapon.CurrentState == OgsCompActivatableEffect.CompActivatableEffect.State.Deactivated;
                        if (flag)
                        {
                            compPowerWeapon.TryActivate();
                        }
                    }
                    if (p.equipment.Primary.TryGetCompFast<CompForceWeaponActivatableEffect>() != null && p.equipment.Primary.TryGetCompFast<CompForceWeaponActivatableEffect>() is CompForceWeaponActivatableEffect compForceWeapon)
                    {
                        bool flag = compForceWeapon.CurrentState == OgsCompActivatableEffect.CompActivatableEffect.State.Deactivated;
                        if (flag)
                        {
                            compForceWeapon.TryActivate();
                        }
                    }
                    /*
                    if (p.equipment.Primary.TryGetCompFast<CompAbilityItem>() != null && p.equipment.Primary.TryGetCompFast<CompAbilityItem>() is CompAbilityItem compAbilityItem)
                    {
                        foreach (var item in compAbilityItem.Abilities)
                        {
                            bool flag = !p.TryGetCompFast<CompAbilityUser>().AbilityData.TemporaryWeaponPowers.Contains(item);
                            if (flag)
                            {
                                p.TryGetCompFast<CompAbilityUser>().AddWeaponAbility(item.Def);
                            }
                        }
                    }
                    */
                }
            }
        }
    }
}
