using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000004 RID: 4
	public class IncidentWorker_AMXBDarkEldarUpdateNote : IncidentWorker
	{
		// Token: 0x06000005 RID: 5 RVA: 0x00002110 File Offset: 0x00000310
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			Find.LetterStack.ReceiveLetter(Translator.Translate("LetterLabelAMXBDarkEldarUpdate"), Translator.Translate("AMXBDarkEldarUpdate", new object[0]), LetterDefOf.PositiveEvent, null);
			//	Find.TickManager.slower.SignalForceNormalSpeedShort();
			return true;
		}
	}
}
