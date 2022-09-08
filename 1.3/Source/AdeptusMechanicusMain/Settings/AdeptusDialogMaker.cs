using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Steam;

namespace AdeptusMechanicus.settings
{
    public static class AdeptusDialogMaker
    {
        // AdeptusDialogMaker.CreateWarningDialogIfNecessary
        public static void CreateWarningDialogIfNecessary()
        {
            List<ModMetaData> hereticalModifications = new List<ModMetaData>();
            if (ModLister.GetActiveModWithIdentifier("emitbreaker.ModIntegrationMod.40kFactions") is ModMetaData data)
            {
                hereticalModifications.Add(data);
            }
            if (ModLister.GetActiveModWithIdentifier("Archaon.Primarch") is ModMetaData data2)
            {
                hereticalModifications.Add(data2);
            }
            if (!AdeptusDialogMaker.dialogDone && !hereticalModifications.NullOrEmpty())
            {
                AdeptusDialogMaker.CreateHereticalModificationsDialog(hereticalModifications);
            }
        }

        private static void CreateHereticalModificationsDialog(List<ModMetaData> hereticalModifications)
        {
            string text = "Adeptus Mechanicus";
            text += "\n\n";
            text += "AdeptusMechanicus.Warning.HereticalModifications".Translate();
            text += "\n";
            foreach (ModMetaData item in hereticalModifications)
            {
                text += "\n"+item.Name;
            }
            text += "\n\n";
            text += "AdeptusMechanicus.Warning.HereticalModifications.Error".Translate();
            Find.WindowStack.Add(new Dialog_MessageBox(text, null, null, null, null, null, false, null, null, WindowLayer.Dialog));
            AdeptusDialogMaker.dialogDone = true;
        }

        private static bool dialogDone;
    }
    
}