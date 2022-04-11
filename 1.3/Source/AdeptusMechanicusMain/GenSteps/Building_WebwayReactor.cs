using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus
{
    public class Building_WebwayReactor : Building
	{
		public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			if (this.escapeReactor)
			{
				QuestUtility.SendQuestTargetSignals(base.Map.Parent.questTags, "ReactorDestroyed");
			}
			base.Destroy(mode);
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}
			IEnumerator<Gizmo> enumerator = null;
			foreach (Gizmo gizmo2 in ShipUtility.ShipStartupGizmos(this))
			{
				yield return gizmo2;
			}
			enumerator = null;
			yield break;
			yield break;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.escapeReactor, "escapeReactor", false, false);
		}

		public bool escapeReactor;
	}
}
