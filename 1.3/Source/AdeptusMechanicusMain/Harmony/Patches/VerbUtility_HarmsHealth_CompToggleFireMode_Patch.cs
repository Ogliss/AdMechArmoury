using Verse;
using HarmonyLib;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(VerbUtility), "HarmsHealth")]
    public class VerbUtility_HarmsHealth_CompToggleFireMode_Patch
    {
        public static void Postfix(Verb verb, ref bool __result)
        {
            if (!__result)
            {
                CompToggleFireMode comp = verb.EquipmentSource.GetComp<CompToggleFireMode>();
                if (comp != null)
                {
                    foreach (var verb2 in comp.Equippable.AllVerbs)
                    {
                        if (verb2.GetDamageDef()?.harmsHealth ?? false)
                        {
                            __result = true;
                            return;
                        }
                    }
                }
            }
        }
    }
}