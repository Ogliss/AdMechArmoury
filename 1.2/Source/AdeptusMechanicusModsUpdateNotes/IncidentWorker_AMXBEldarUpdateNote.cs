using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000003 RID: 3
	public class IncidentWorker_AMXBEldarUpdateNote : IncidentWorker
	{
		// Token: 0x06000003 RID: 3 RVA: 0x000020B4 File Offset: 0x000002B4
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			Find.LetterStack.ReceiveLetter(Translator.Translate("LetterLabelAMXBEldarUpdate"), Translator.Translate("AMXBEldarUpdate", new object[0]), LetterDefOf.PositiveEvent, null);
			//	Find.TickManager.slower.SignalForceNormalSpeedShort();
			return true;
		}
	}
}
