using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000009 RID: 9
	public class IncidentWorker_AMArmouryUpdateNote : IncidentWorker
	{
		// Token: 0x0600000F RID: 15 RVA: 0x000022DC File Offset: 0x000004DC
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			Find.LetterStack.ReceiveLetter(Translator.Translate("LetterLabelAMArmouryUpdate"), Translator.Translate("AMArmouryUpdate", new object[0]), LetterDefOf.PositiveEvent, null);
			//	Find.TickManager.slower.SignalForceNormalSpeedShort();
			return true;
		}
	}
}
