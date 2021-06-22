using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000008 RID: 8
	public class IncidentWorker_AMXenoBiologiespdateNote : IncidentWorker
	{
		// Token: 0x0600000D RID: 13 RVA: 0x00002280 File Offset: 0x00000480
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			Find.LetterStack.ReceiveLetter(Translator.Translate("LetterLabelAMXenoBiologiesUpdate"), Translator.Translate("AMXenoBiologiespdate", new object[0]), LetterDefOf.PositiveEvent, null);
			//	Find.TickManager.slower.SignalForceNormalSpeedShort();
			return true;
		}
	}
}
