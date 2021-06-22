using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000002 RID: 2
	public class IncidentWorker_AMXBTauUpdateNote : IncidentWorker
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			Find.LetterStack.ReceiveLetter(Translator.Translate("LetterLabelAMXBTauUpdate"), Translator.Translate("AMXBTauUpdate", new object[0]), LetterDefOf.PositiveEvent, null);
			//	Find.TickManager.slower.SignalForceNormalSpeedShort();
			return true;
		}
	}
}
