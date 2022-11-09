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
        public override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            return !map.gameConditionManager.ConditionIsActive(AdeptusGameConditionDefOf.OG_Condition_Warpstorm);
        }

        public override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            int duration = Mathf.RoundToInt(this.def.durationDays.RandomInRange * 60000f);
            GameCondition_Warpstorm gameCondition_Warpstorm = (GameCondition_Warpstorm)GameConditionMaker.MakeCondition(AdeptusGameConditionDefOf.OG_Condition_Warpstorm, duration);
            gameCondition_Warpstorm.points = parms.points;
            Rand.PushState();
            gameCondition_Warpstorm.severity = Rand.Value;
            Rand.PopState();
            map.gameConditionManager.RegisterCondition(gameCondition_Warpstorm);
            base.SendStandardLetter(this.def.letterLabel, this.def.letterText, this.def.letterDef, parms, new TargetInfo(gameCondition_Warpstorm.centerLocation.ToIntVec3, map, false), Array.Empty<NamedArgument>());
            if (map.weatherManager.curWeather.rainRate > 0.1f)
            {
                map.weatherDecider.StartNextWeather();
            }
            return true;
        }
    }
}
