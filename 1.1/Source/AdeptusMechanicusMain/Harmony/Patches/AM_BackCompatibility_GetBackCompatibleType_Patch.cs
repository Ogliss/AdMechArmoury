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
    [HarmonyPatch(typeof(BackCompatibility), "GetBackCompatibleType")]
    public static class AM_BackCompatibility_GetBackCompatibleType_Patch
    {
        [HarmonyPostfix]
        public static void GetBackCompatibleType_Postfix(Type baseType, string providedClassName, XmlNode node, ref Type __result)
        {
            if (providedClassName.Contains("AdeptusMechanicus.settings.AMASettings"))
            {
                string name = Regex.Replace(providedClassName, "AMASettings", "AMSettings");
                Type type = GenTypes.GetTypeInAnyAssembly(name, null);
                if (type!=null)
                {
                    __result = type;
                }
            }
            if (providedClassName.Contains("EquipmentAbility"))
            {
                string name = "AbilitesExtended.EquipmentAbility";
                Type type = GenTypes.GetTypeInAnyAssembly(name, null);
                if (type!=null)
                {
                    __result = type;
                }
            }
            if (providedClassName.Contains("AdeptusMechanicus.Verb_ShootEquipment"))
            {
                string name = "AbilitesExtended.Verb_ShootEquipment";
                Type type = GenTypes.GetTypeInAnyAssembly(name, null);
                if (type!=null)
                {
                    __result = type;
                }
            }
        }
    }

}
