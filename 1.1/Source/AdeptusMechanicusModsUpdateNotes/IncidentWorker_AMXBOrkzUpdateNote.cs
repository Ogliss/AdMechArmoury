using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000007 RID: 7
	public class IncidentWorker_AMXBOrkzUpdateNote : IncidentWorker
	{
		// Token: 0x0600000B RID: 11 RVA: 0x00002224 File Offset: 0x00000424
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			Find.LetterStack.ReceiveLetter(Translator.Translate("LetterLabelAMXBOrkzUpdate"), Translator.Translate("AMXBOrkzUpdate", new object[0]), LetterDefOf.PositiveEvent, null);
			//	Find.TickManager.slower.SignalForceNormalSpeedShort();
			return true;
		}
	}
}
