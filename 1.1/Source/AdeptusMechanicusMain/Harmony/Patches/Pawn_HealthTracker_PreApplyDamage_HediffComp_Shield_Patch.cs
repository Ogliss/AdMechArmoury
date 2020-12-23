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
                    
                    if (___pawn.holdingOwner != null) Log.Message(___pawn + " is held by "+ ___pawn.holdingOwner + ", skipping");
                    else Log.Message(___pawn + " not spawned, skipping");
                    
                    return true;
                }
                Log.Message(___pawn + " spawned, checking damage");
                List<Hediff> list = ___pawn.health.hediffSet.hediffs;
                foreach (Hediff item in list)
                {
                    Log.Message(___pawn + " checking " + item );
                    HediffComp_Shield _Shield = item.TryGetComp<HediffComp_Shield>();
                    if (_Shield != null)
                    {
                        Log.Message(___pawn + " " + item + " Is _Shield");
                        absorbed = _Shield.CheckPreAbsorbDamage(dinfo);
                        return false;
                    }
                    HediffComp_PhaseShifter _Shifter = item.TryGetComp<HediffComp_PhaseShifter>();
                    if (_Shifter != null)
                    {
                        Log.Message(___pawn + " " + item + " Is _Shifter");
                        if (dinfo.Def.isExplosive)
                        {
                            absorbed = !_Shifter.isPhasedIn;
                            return _Shifter.isPhasedIn;
                        }
                    }
                }

            }
            return true;
        }
    }
}
