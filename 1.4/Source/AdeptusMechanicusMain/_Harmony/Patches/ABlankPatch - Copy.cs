using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse.AI;
using Verse.AI.Group;
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using Verse.Sound;
using System;
using Verse;
using HarmonyLib;
using UnityEngine;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(UIRoot_Entry), "Init")]
    public static class UIRoot_Entry_Init_HereticalModifications_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            AdeptusDialogMaker.CreateWarningDialogIfNecessary();
        }
    }
    
}
