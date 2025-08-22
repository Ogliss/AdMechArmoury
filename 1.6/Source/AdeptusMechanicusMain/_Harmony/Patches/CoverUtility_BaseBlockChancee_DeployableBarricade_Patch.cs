using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using AdeptusMechanicus;
using HarmonyLib;
using Verse.Sound;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Verse.CoverUtility), "BaseBlockChance", new Type[] { typeof(Thing) })]
    public static class CoverUtility_BaseBlockChancee_DeployableBarricade_Patch
    {
        [HarmonyPostfix] 
        public static void BaseBlockChancePostfix(Thing thing, ref float __result)
        {
            if (thing != null && thing is Building_DeployableBarricade Barricade)
            {
                if (Barricade.Toggled)
                {
                    __result += Barricade.Deployed.deployedfillPercent;
                }
            }
        }
    }

}
