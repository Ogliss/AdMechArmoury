using RimWorld;
using Verse;
using HarmonyLib;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnHairColors), "HasGreyHair")]
    public static class PawnHairColors_HasGreyHair_Alien_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result, Pawn pawn, int ageYears)
        {
            if (__result && pawn.def.defName.StartsWith("OG") && pawn.def is AlienRace.ThingDef_AlienRace alien)
            {
                if (alien.alienRace.generalSettings.alienPartGenerator.getsGreyAt > ageYears)
                {
                    __result = false;
                }
            }

        }
    }
    /*

    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal"), HarmonyPriority(Priority.Last)]
    public static class PawnRenderer_RenderPawnInternal__Patch
    {
        
        [HarmonyPrefix]
        public static void Prefix(ref PawnRenderer __instance)
        {
            Pawn pawn = __instance.pawn;
            if (!pawn.RaceProps.Humanlike)
            {
                return;
            }
            if (ModLister.BiotechInstalled && __instance.graphics.geneGraphics == null)
            {
                __instance.graphics.geneGraphics = new List<GeneGraphicRecord>();
            }
        }
        

    }
    */
}
