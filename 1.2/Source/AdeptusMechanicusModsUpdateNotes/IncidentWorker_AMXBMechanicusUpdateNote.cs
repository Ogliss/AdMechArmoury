using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000005 RID: 5
	public class IncidentWorker_AMXBMechanicusUpdateNote : IncidentWorker
	{
		// Token: 0x06000007 RID: 7 RVA: 0x0000216C File Offset: 0x0000036C
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			Find.LetterStack.ReceiveLetter(Translator.Translate("LetterLabelAMXBMechanicusUpdate"), Translator.Translate("AMXBMechanicusUpdate", new object[0]), LetterDefOf.PositiveEvent, null);
			//	Find.TickManager.slower.SignalForceNormalSpeedShort();
			return true;
		}
	}
}
