using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000006 RID: 6
	public class IncidentWorker_AMXBTraitorUpdateNote : IncidentWorker
	{
		// Token: 0x06000009 RID: 9 RVA: 0x000021C8 File Offset: 0x000003C8
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			Find.LetterStack.ReceiveLetter(Translator.Translate("LetterLabelAMXBTraitorUpdate"), Translator.Translate("AMXBTraitorUpdate", new object[0]), LetterDefOf.PositiveEvent, null);
			//	Find.TickManager.slower.SignalForceNormalSpeedShort();
			return true;
		}
	}
}
