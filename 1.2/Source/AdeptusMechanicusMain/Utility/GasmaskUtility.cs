using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public static class GasmaskUtility
    {
        public static void DoPawnToxicDamage(Pawn p, float mod = 1f)
        {
            if (p.Spawned && p.Position.Roofed(p.Map))
            {
                return;
            }
            if (!p.RaceProps.IsFlesh)
            {
                return;
            }
            float num = 0.028758334f;
            num *= p.GetStatValue(StatDefOf.ToxicSensitivity, true);
            if (num != 0f)
            {
                float num2 = Mathf.Lerp(0.85f, 1.15f, Rand.ValueSeeded(p.thingIDNumber ^ 74374237));
                num *= num2;
                num *= mod;
                HealthUtility.AdjustSeverity(p, HediffDefOf.ToxicBuildup, num);
            }
        }

        public static bool WearingGasmask(Pawn p, out Apparel a, out CompLungProtectionApparel b)
        {
            a = null;
            b = null;
            if (p.apparel != null)
            {
                for (int i = 0; i < p.apparel.WornApparelCount; i++)
                {
                    if (p.apparel.WornApparel[i] is Apparel apparel && GasmaskUtility.GasMasks.Contains(apparel.def))
                    {
                        a = apparel;
                        if (apparel.TryGetCompFast<CompLungProtectionApparel>() is CompLungProtectionApparel protectionApparel)
                        {
                            b = protectionApparel;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static List<ThingDef> gasMasks;
        public static List<ThingDef> GasMasks
        {
            get
            {
                if (gasMasks.NullOrEmpty())
                {
                    if (gasMasks == null)
                    {
                        gasMasks = new List<ThingDef>();
                        gasMasks.AddRange(DefDatabase<ThingDef>.AllDefs.Where(x => x.IsApparel && x.HasComp(typeof(CompLungProtectionApparel))));
                    }
                }
                return gasMasks;
            }
        }
    }
    
}
