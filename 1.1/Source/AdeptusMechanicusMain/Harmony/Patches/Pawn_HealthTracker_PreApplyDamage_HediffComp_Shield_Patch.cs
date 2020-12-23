using System.Collections.Generic;
using Verse;
using HarmonyLib;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), "PreApplyDamage", null)]
    public class Pawn_HealthTracker_PreApplyDamage_HediffCompShield_Patch
    {
        // Token: 0x060011D5 RID: 4565 RVA: 0x000EC6D0 File Offset: 0x000EA8D0
        public static bool Prefix(Pawn ___pawn, ref DamageInfo dinfo, ref bool absorbed)
        {
            if (dinfo.Def != null && ___pawn != null && !___pawn.Downed)
            {
                if (!___pawn.Spawned || ___pawn.Map == null)
                {
                    return true;
                }
                for (int i = 0; i < ___pawn.health.hediffSet.hediffs.Count; i++)
                {
                    Hediff item = ___pawn.health.hediffSet.hediffs[i];
                    if (item != null)
                    {
                        HediffComp_Shield _Shield = item.TryGetComp<HediffComp_Shield>();
                        if (_Shield != null)
                        {
                            absorbed = _Shield.CheckPreAbsorbDamage(dinfo);
                            return false;
                        }
                        HediffComp_PhaseShifter _Shifter = item.TryGetComp<HediffComp_PhaseShifter>();
                        if (_Shifter != null)
                        {
                            if (dinfo.Def.isExplosive)
                            {
                                absorbed = !_Shifter.isPhasedIn;
                                return _Shifter.isPhasedIn;
                            }
                        }
                    }
                }

            }
            return true;
        }
    }
}
