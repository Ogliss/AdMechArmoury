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
using System.Text.RegularExpressions;
using System.Xml;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(ParseHelper), "ParseType")]
    public static class ParseHelper_ParseType_Patch
    {
        public static void Prefix(ref string str)
        {
            if (str.Contains("AdeptusMechanicus.LaserGunDef"))
            {
                str = "AdeptusMechanicus.Lasers.LaserGunDef";
            }
        }
    }

}
