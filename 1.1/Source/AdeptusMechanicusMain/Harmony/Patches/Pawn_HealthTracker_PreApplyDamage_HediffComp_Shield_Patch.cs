using System.Collections.Generic;
using Verse;
using HarmonyLib;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), "PreApplyDamage", null)]
    public class Pawn_HealthTracker_PreApplyDamage_HediffCompShield_Patch
    {
        // Token: 0x060011D5 RID: 4565 RVA: 0x000EC6D0 File Offset: 0x000EA8D0
        public static void Postfix(Pawn ___pawn, ref DamageInfo dinfo, ref bool absorbed)
        {
            if (dinfo.Def != null && ___pawn != null && !___pawn.Downed)
            {
                if (!___pawn.Spawned || ___pawn.Map == null)
                {
                    return;
                }
            //    else Log.Message(___pawn + " spawned, checking damage");
                for (int i = 0; i < ___pawn.health.hediffSet.hediffs.Count; i++)
                {
                    HediffWithComps item = ___pawn.health.hediffSet.hediffs[i] as HediffWithComps;
                    if (item != null)
                    {
                    //    Log.Message(___pawn + " checking " + item);
                        HediffComp_Shield _Shield = item.TryGetComp<HediffComp_Shield>();
                        if (_Shield != null)
                        {
                        //    Log.Message(___pawn + " " + item + " Is _Shield");
                            absorbed = _Shield.CheckPreAbsorbDamage(dinfo);
                            return;
                        }
                        else
                        {
                        //    Log.Message(___pawn + " " + item + " Is Not _Shield");
                        }
                        HediffComp_PhaseShifter _Shifter = item.TryGetComp<HediffComp_PhaseShifter>();
                        if (_Shifter != null)
                        {
                        //    Log.Message(___pawn + " " + item + " Is _Shifter");
                            if (dinfo.Def.isExplosive)
                            {
                                absorbed = !_Shifter.isPhasedIn;
                                return;
                            }
                        }
                    //    else Log.Message(___pawn + " " + item + " Is Not _Shifter");
                    }
                }

            }
        }
    }
}
