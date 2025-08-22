using Verse;
using HarmonyLib;
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
