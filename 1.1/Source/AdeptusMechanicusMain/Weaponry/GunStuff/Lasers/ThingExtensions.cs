using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace AdeptusMechanicus
{
    public static class ThingExtensions
    {
        public static bool IsShielded(this Thing thing)
        {
            Pawn p = thing as Pawn;
            if (p == null)
            {
                return false;
            }
            return p.IsShielded();
        }
        
        public static bool IsShielded(this Pawn pawn)
        {
            if (pawn == null || pawn?.apparel == null) return false;
            if (pawn.apparel.WornApparel.NullOrEmpty()) return false;

            DamageInfo damageTest = new DamageInfo(DamageDefOf.Bomb, 0f, 0f, -1, null);
            for (int i = 0; i < pawn.apparel.WornApparel.Count; i++)
            {
                Apparel apparel = pawn.apparel.WornApparel[i];
                if (apparel != null)
                {
                    if (apparel.CheckPreAbsorbDamage(damageTest)) return true;
                }
            }
            return false;
        }
    }
}
