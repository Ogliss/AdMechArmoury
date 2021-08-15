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
    
	public static class BodyPartUtils
	{
		public static bool ExistsDeep(BodyPartRecord part, string groupDefName)
		{
			if (part.groups.Exists((BodyPartGroupDef x) => ((Def)x).defName.Equals(groupDefName)))
			{
				return true;
			}
			if (part.parts != null)
			{
				return part.parts.Any((BodyPartRecord nestedPart) => ExistsDeep(nestedPart, groupDefName));
			}
			return false;
		}

		public static bool ExistsByGroupAndParent(List<BodyPartRecord> parts, string parentDefName, string groupDefName)
		{
			return parts.FindAll((BodyPartRecord x) => x.def?.defName != null && x.def.defName.Equals(parentDefName)).Exists((BodyPartRecord x) => ExistsDeep(x, groupDefName));
		}
	}
	/*
    [HarmonyPatch(typeof(ApparelUtility), "HasPartsToWear")]
    public static class ApparelUtility_HasPartsToWear_Restricted_Patch
    {
        [HarmonyPrefix]
        public static void Pre_(Pawn p, ThingDef apparel, ref bool __result)
        {
            if (apparel.HasModExtension<ApparelRestrictionDefExtension>())
            {

			}
			List<Hediff> hediffs = p.health.hediffSet.hediffs;
			bool flag = false;
			for (int j = 0; j < hediffs.Count; j++)
			{
				if (hediffs[j] is Hediff_MissingPart)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				__result = true;
				return;
			}
			IEnumerable<BodyPartRecord> notMissingParts = p.health.hediffSet.GetNotMissingParts();
			List<BodyPartGroupDef> groups = apparel.apparel.bodyPartGroups;
			int i;
			for (i = 0; i < groups.Count; i++)
			{
				if (notMissingParts.Any(x => x.IsInGroup(groups[i])))
				{
					__result = true;
					return;
				}
			}
		//	ProstheticNoMissingBodyPartsSettings settings = ((Mod)LoadedModManager.GetMod<ProstheticNoMissingBodyPartsMod>()).GetSettings<ProstheticNoMissingBodyPartsSettings>();
			HashSet<string> armsWhitelist = new HashSet<string>();
			HashSet<string> legsWhitelist = new HashSet<string>();
			if (settings.ArmsWhitelist != null)
			{
				GenCollection.AddRange<string>(armsWhitelist, settings.ArmsWhitelist);
			}
			if (settings.LegsWhitelist != null)
			{
				GenCollection.AddRange<string>(legsWhitelist, settings.LegsWhitelist);
			}
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			foreach (BodyPartGroupDef item in groups)
			{
				if (item.defName.Equals("LeftHand"))
				{
					flag2 = true;
					break;
				}
				if (item.defName.Equals("RightHand"))
				{
					flag3 = true;
					break;
				}
				if (item.defName.Equals("Hands"))
				{
					flag4 = true;
					break;
				}
				if (item.defName.Equals("Feet"))
				{
					flag5 = true;
					break;
				}
				List<BodyPartRecord> allParts = p.def.race.body.AllParts;
				if (BodyPartUtils.ExistsByGroupAndParent(allParts, "Shoulder", item.defName))
				{
					flag4 = true;
					break;
				}
				if (BodyPartUtils.ExistsByGroupAndParent(allParts, "Leg", item.defName))
				{
					flag5 = true;
					break;
				}
			}
			if (flag2)
			{
				__result = hediffs.Exists(h => h.Part?.customLabel != null && h.def?.defName != null && h.Part.def.defName.Equals("Shoulder") && armsWhitelist.Contains(h.def.defName) && BodyPartUtils.ExistsDeep(h.Part, "LeftHand"));
				return;
			}
			if (flag3)
			{
				__result = hediffs.Exists(h => h.Part?.customLabel != null && h.def?.defName != null && h.Part.def.defName.Equals("Shoulder") && armsWhitelist.Contains(h.def.defName) && BodyPartUtils.ExistsDeep(h.Part, "RightHand"));
				return;
			}
			if (flag4)
			{
				__result = hediffs.Exists(h => h.Part?.def?.defName != null && h.def?.defName != null && h.Part.def.defName.Equals("Shoulder") && armsWhitelist.Contains(h.def.defName));
				return;
			}
			if (flag5)
			{
				__result = hediffs.Exists(h => h.Part?.def?.defName != null && h.def?.defName != null && h.Part.def.defName.Equals("Leg") && legsWhitelist.Contains(h.def.defName));
				return;
			}
			__result = false;
			return;
		}

        [HarmonyPostfix]
        public static void Post_(Pawn p, ThingDef apparel, ref bool __result)
        {
            if (apparel.HasModExtension<ApparelRestrictionDefExtension>())
            {
            //    __result = false;

                ApparelRestrictionDefExtension defExtension = apparel.GetModExtensionFast<ApparelRestrictionDefExtension>();
                if (defExtension!=null)
                {
                    bool gender = defExtension.gender == Gender.None || p.gender == defExtension.gender;
                    bool race = false;
                    bool hediff = false;
                    bool trait = false;
                    if (!defExtension.RaceDefs.NullOrEmpty())
                    {
                        race = defExtension.RaceDefs.Contains(p.def);
                    }
                    if (!defExtension.HediffDefs.NullOrEmpty())
                    {
                        hediff = p.health.hediffSet.hediffs.Any(x => defExtension.HediffDefs.Contains(x.def));
                    }
                    if (!defExtension.TraitDefs.NullOrEmpty())
                    {
                        trait = p.story.traits.allTraits.Any(x => defExtension.TraitDefs.Contains(x.def));
                    }
                    __result = gender && (race || hediff || trait);
                }
            }
        }

    }
	*/
    
}
