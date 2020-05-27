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
using System.Reflection;
using AdeptusMechanicus;
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), "PreApplyDamage", null)]
    public class AM_PreApplyDamage_HediffComp_Shield_Patch
    {
        // Token: 0x060011D5 RID: 4565 RVA: 0x000EC6D0 File Offset: 0x000EA8D0
        public static bool Prefix(Pawn ___pawn, ref DamageInfo dinfo, out bool absorbed)
        {
            bool flag = dinfo.Def != null && ___pawn != null && !___pawn.Downed;
            if (flag)
            {
                if (___pawn.health.hediffSet.hediffs.Any(x => x.TryGetComp<HediffComp_Shield>() != null))
                {
                    List<Hediff> list = ___pawn.health.hediffSet.hediffs.FindAll(x => x.TryGetComp<HediffComp_Shield>() != null);
                    foreach (Hediff item in list)
                    {
                        HediffComp_Shield _Shield = item.TryGetComp<HediffComp_Shield>();
                        if (_Shield != null)
                        {
                            absorbed = _Shield.CheckPreAbsorbDamage(dinfo);
                            return false;
                        }
                    }
                }
                if (___pawn.health.hediffSet.hediffs.Any(x => x.TryGetComp<HediffComp_PhaseShifter>() != null))
                {
                    List<Hediff> list = ___pawn.health.hediffSet.hediffs.FindAll(x => x.TryGetComp<HediffComp_PhaseShifter>() != null);
                    foreach (Hediff item in list)
                    {
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
            absorbed = false;
            return true;
        }

        // Token: 0x04001325 RID: 4901
        public static FieldInfo pawn = typeof(Pawn_HealthTracker).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
    }
}
