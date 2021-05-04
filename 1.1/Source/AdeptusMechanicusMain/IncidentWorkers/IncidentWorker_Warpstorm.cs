using AdeptusMechanicus;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.IncidentWorker_Warpstorm
    public class IncidentWorker_Warpstorm : IncidentWorker
    {
        // Token: 0x06000E54 RID: 3668 RVA: 0x0006B310 File Offset: 0x00069710
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            return !map.gameConditionManager.ConditionIsActive(AdeptusGameConditionDefOf.OG_Condition_Warpstorm);
        }

        // Token: 0x06000E55 RID: 3669 RVA: 0x0006B33C File Offset: 0x0006973C
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            int duration = Mathf.RoundToInt(this.def.durationDays.RandomInRange * 60000f);
            GameCondition_Warpstorm gameCondition_Warpstorm = (GameCondition_Warpstorm)GameConditionMaker.MakeCondition(AdeptusGameConditionDefOf.OG_Condition_Warpstorm, duration);
            map.gameConditionManager.RegisterCondition(gameCondition_Warpstorm);
            base.SendStandardLetter(this.def.letterLabel, GameConditionDefOf.Flashstorm.letterText, this.def.letterDef, parms, new TargetInfo());
            if (map.weatherManager.curWeather.rainRate > 0.1f)
            {
                map.weatherDecider.StartNextWeather();
            }
            return true;
        }
    }
}
