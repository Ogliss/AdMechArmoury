using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AdeptusMechanicus.HarmonyInstance
{
	[HarmonyPatch(typeof(Corpse), "get_IngestibleNow")]
    public class Corpse_IngestibleNow_Construct_Patch
	{
		[HarmonyPostfix]
		public static void Postfix(Corpse __instance, ref bool __result)
		{
			bool flag = __instance.InnerPawn.RaceProps.FleshType.defName.Contains("OG_Flesh_Construct");
			if (flag)
			{
				__result = false;
			}
		}
	}
}
