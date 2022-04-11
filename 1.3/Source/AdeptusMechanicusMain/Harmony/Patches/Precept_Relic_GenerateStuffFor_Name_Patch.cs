using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse.AI;
using Verse.AI.Group;
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using Verse.Sound;
using System;
using RimWorld;
using Verse;
using HarmonyLib;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Precept_Relic), "GenerateStuffFor")]
    public static class Precept_Relic_GenerateStuffFor_Name_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Precept_Relic __instance, ThingDef thing, Ideo ideo)
        {
			return false;
        }

        [HarmonyPostfix]
        public static void Postfix(Precept_Relic __instance, ThingDef thing, Ideo ideo, ThingDef __result)
        {
            try
            {
                IEnumerable<ThingDef> source = GenStuff.AllowedStuffsFor(thing, TechLevel.Undefined);
                if (ideo != null)
                {
                    IEnumerable<ThingDef> alreadyUsedStuffs = (from p in ideo.PreceptsListForReading.Where(delegate (Precept p)
                    {
                        Precept_Relic precept_Relic;
                        return (precept_Relic = (p as Precept_Relic)) != null && precept_Relic.ThingDef == thing;
                    })
                                                               select ((Precept_Relic)p).stuff into p
                                                               where p != null
                                                               select p).Distinct<ThingDef>();
                    source = from stuff in source
                             where !alreadyUsedStuffs.Contains(stuff)
                             select stuff;
                }
                __result = source.RandomElementByWeight((ThingDef stuff) => stuff.BaseMarketValue);
            }
            catch (Exception e)
            {
                Log.Error($"{thing} {e}");
                throw;
            }
		}
    }
}
