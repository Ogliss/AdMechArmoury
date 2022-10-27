using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{

	/*
	[HarmonyPatch(typeof(ArmorUtility), "ApplyArmor")]
	public static class ArmorUtility_ApplyArmor_TechLevelTweak_Patch
	{

		[HarmonyPrefix]
		public static void Pre(ref float armorPenetration, float armorRating, Thing armorThing, ref DamageDef damageDef)
		{
			if (damageDef == AdeptusDamageDefOf.OGIVolkite)
			{
				if (armorThing.def.techLevel < TechLevel.Spacer)
				{
					armorPenetration += armorPenetration * ((armorThing.def.techLevel - TechLevel.Spacer)/10f);

				}
			}
		}
	}
	*/

}
