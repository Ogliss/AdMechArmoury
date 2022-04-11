using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AdeptusMechanicus
{
    public class WorldObjectCompProperties_EscapePortalWebway : WorldObjectCompProperties
	{
		public WorldObjectCompProperties_EscapePortalWebway()
		{
			this.compClass = typeof(EscapePortalWebwayComp);
		}

		public override IEnumerable<string> ConfigErrors(WorldObjectDef parentDef)
		{
			foreach (string text in base.ConfigErrors(parentDef))
			{
				yield return text;
			}
			if (!typeof(MapParent).IsAssignableFrom(parentDef.worldObjectClass))
			{
				yield return parentDef.defName + " has WorldObjectCompProperties_EscapePortalWebway but it's not MapParent.";
			}
			yield break;
		}
	}

	[StaticConstructorOnStartup]
	public class EscapePortalWebwayComp : WorldObjectComp
	{
		public override void PostMapGenerate()
		{
			Building building = ((MapParent)this.parent).Map.listerBuildings.AllBuildingsColonistOfDef(AdeptusThingDefOf.OG_Webway_Core).FirstOrDefault<Building>();
			Building_ShipReactor building_ShipReactor;
			if (building != null && (building_ShipReactor = (building as Building_ShipReactor)) != null)
			{
				building_ShipReactor.charlonsReactor = true;
			}
		}

		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan)
		{
			foreach (FloatMenuOption floatMenuOption in CaravanArrivalAction_VisitEscapeShip.GetFloatMenuOptions(caravan, (MapParent)this.parent))
			{
				yield return floatMenuOption;
			}
			yield break;
		}
	}
}
