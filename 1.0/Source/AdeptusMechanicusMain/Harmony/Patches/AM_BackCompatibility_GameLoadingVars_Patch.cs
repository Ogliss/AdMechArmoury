﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Harmony;
using Verse.Sound;

namespace AdeptusMechanicus.Harmony
{

    [HarmonyPatch(typeof(BackCompatibility), "GameLoadingVars")]
    public static class AM_BackCompatibility_GameLoadingVars_Patch
    {
        [HarmonyPostfix]
        public static void GameLoadingVars_Postfix(Game game)
        {
        //    Log.Message("dud outfit finder");
            foreach (Outfit set in game.outfitDatabase.AllOutfits)
            {
            //    Log.Message(string.Format("checking Outfit: {0}", set.label));
                if (set.filter.AllowedThingDefs.Count()>0)
                {
                    foreach (ThingDef item in set.filter.AllowedThingDefs)
                    {
                     //   Log.Message(string.Format("checking :{0}", item.LabelCap));
                    }
                }
                else
                {
            //        Log.Message(string.Format("Outfit: {0} is empty", set.label));
                }
            }
            /*
            if (game.outfitDatabase.AllOutfits.Any(x=> x.filter.AllowedThingDefs.Any(y=> y ==null)))
            {
            }
            */
        }
    }

}
