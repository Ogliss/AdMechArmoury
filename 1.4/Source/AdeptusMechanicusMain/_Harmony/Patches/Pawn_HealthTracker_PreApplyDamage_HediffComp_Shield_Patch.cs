using System.Collections.Generic;
using Verse;
using HarmonyLib;
using System.Runtime.InteropServices;
using AdeptusMechanicus.settings;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), "PreApplyDamage", null)]
    public static class Pawn_HealthTracker_PreApplyDamage_HediffCompShield_Patch
    {
        public static void Postfix(Pawn ___pawn, ref DamageInfo dinfo, ref bool absorbed)
        {
            if (dinfo.Def != null && ___pawn != null && !___pawn.Downed)
            {
                if (!___pawn.Spawned || ___pawn.Map == null || ___pawn.health.hediffSet.hediffs.NullOrEmpty())
                {
                    return;
                }
                //    else Log.Message(___pawn + " spawned, checking damage");
                for (int i = 0; i < AdeptusHediffUtility.ShieldHediffs.Count; i++)
                {
                    if (___pawn.health.hediffSet.GetFirstHediffOfDef(AdeptusHediffUtility.ShieldHediffs[i]) is HediffWithComps item)
                    {
                        if (item.TryGetCompFast<HediffComp_Shield>() is HediffComp_Shield _Shield)
                        {
                            //    Log.Message(___pawn + " " + item + " Is _Shield");
                            absorbed = _Shield.CheckPreAbsorbDamage(dinfo);
                        }
                    }
                }
                for (int i = 0; i < AdeptusHediffUtility.PhasicHediffs.Count; i++)
                {
                    if (___pawn.health.hediffSet.GetFirstHediffOfDef(AdeptusHediffUtility.PhasicHediffs[i]) is HediffWithComps item)
                    {
                        if (item.TryGetCompFast<HediffComp_PhaseShifter>() is HediffComp_PhaseShifter _Shifter)
                        {
                            //    Log.Message(___pawn + " " + item + " Is _Shifter");
                            if (dinfo.Def.isExplosive)
                            {
                                absorbed = !_Shifter.isPhasedIn;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
