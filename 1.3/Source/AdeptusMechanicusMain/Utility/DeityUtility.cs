using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public static class DeityUtility
    {
        static DeityUtility()
        {

		}
		public static void GenerateSpecificDeities(IdeoFoundation_Deity __instance)
		{
			__instance.deities.Clear();
			if (__instance.ideo.culture.defName.StartsWith("OG_Greenskin"))
			{
				IdeoFoundation_Deity.Deity gork = DeityUtility.Gork.cloneDeity();
				IdeoFoundation_Deity.Deity mork = DeityUtility.Mork.cloneDeity();
				FillDeity(__instance, gork);
				__instance.deities.Add(gork);
				FillDeity(__instance, mork);
				__instance.deities.Add(mork);
			}
		}

		public static IdeoFoundation_Deity.Deity cloneDeity(this IdeoFoundation_Deity.Deity deity)
		{
			return new IdeoFoundation_Deity.Deity()
			{
				name = deity.name,
				iconPath = deity.iconPath,
				type = deity.type,
				gender = deity.gender,
				relatedMeme = deity.relatedMeme
			};
		}

		private static void FillDeity(IdeoFoundation_Deity __instance, IdeoFoundation_Deity.Deity deity)
		{
			/*
			Gender supremeGender = __instance.ideo.SupremeGender;
			if (supremeGender != Gender.None)
			{
				deity.gender = supremeGender;
			}
			else
			{
				deity.gender = Gen.RandomEnumValue<Gender>(true);
			}
			*/
			MemeDef relatedMeme;
			MemeDef relatedMeme2;
			if ((from x in __instance.ideo.memes
				 where !__instance.deities.Any((IdeoFoundation_Deity.Deity y) => y.relatedMeme == x)
				 select x).TryRandomElement(out relatedMeme))
			{
				deity.relatedMeme = relatedMeme;
			}
			else if (__instance.ideo.memes.TryRandomElement(out relatedMeme2))
			{
				deity.relatedMeme = relatedMeme2;
			}
		}

		private static IEnumerable<string> AllExistingDeities(IdeoFoundation_Deity __instance)
		{
			int num;
			for (int i = 0; i < __instance.deities.Count; i = num + 1)
			{
				yield return __instance.deities[i].name;
				num = i;
			}
			if (Find.World != null)
			{
				List<Ideo> ideos = Find.IdeoManager.IdeosListForReading;
				for (int i = 0; i < ideos.Count; i = num + 1)
				{
					IdeoFoundation_Deity deityFoundation;
					if ((deityFoundation = (ideos[i].foundation as IdeoFoundation_Deity)) != null)
					{
						for (int j = 0; j < deityFoundation.deities.Count; j = num + 1)
						{
							yield return deityFoundation.deities[j].name;
							num = j;
						}
					}
					deityFoundation = null;
					num = i;
				}
				ideos = null;
			}
			yield break;
		}

		// Imperial Gods
		public static IdeoFoundation_Deity.Deity Emperor = new IdeoFoundation_Deity.Deity()
        {
            name = "Emperor of Mankind",
            gender = Gender.Male,
            type = "AdeptusMechanicus.Imperial.Emperor_Desc".Translate(),
            iconPath = "UI/Gods/Icons/Emperor"

        };
        public static IdeoFoundation_Deity.Deity Ommnissiah = new IdeoFoundation_Deity.Deity()
        {
            name = "Ommnissiah",
            gender = Gender.None,
            type = "AdeptusMechanicus.Imperial.Ommnissiah_Desc".Translate(),
            iconPath = "UI/Gods/Icons/Ommnissiah"

        };
        // Aeldari Gods
        public static IdeoFoundation_Deity.Deity Khaine = new IdeoFoundation_Deity.Deity()
        {
            name = "Khaine",
            gender = Gender.None,
            type = "AdeptusMechanicus.Eldar.Khaine_Desc".Translate(),
            iconPath = "UI/Gods/Icons/Khaine"

        };
        public static IdeoFoundation_Deity.Deity Yeanned = new IdeoFoundation_Deity.Deity()
        {
            name = "Yeanned",
            gender = Gender.None,
            type = "AdeptusMechanicus.Eldar.Yeanned_Desc".Translate(),
            iconPath = "UI/Gods/Icons/Yeanned"

        };
        public static IdeoFoundation_Deity.Deity Cregoarch = new IdeoFoundation_Deity.Deity()
        {
            name = "Cregoarch",
            gender = Gender.None,
            type = "AdeptusMechanicus.Eldar.Cregoarch_Desc".Translate(),
            iconPath = "UI/Gods/Icons/Cregoarch"

        };
        // Orkish Gods
        public static IdeoFoundation_Deity.Deity Gork = new IdeoFoundation_Deity.Deity()
        {
            name = "Gork",
            gender = Gender.None,
            type = "AdeptusMechanicus.Ork.Gork_Desc".Translate(),
            iconPath = "UI/Gods/Icons/Gork"

        };
        public static IdeoFoundation_Deity.Deity Mork = new IdeoFoundation_Deity.Deity()
        {
            name = "Mork",
            gender = Gender.None,
            type = "AdeptusMechanicus.Ork.Mork_Desc".Translate(),
            iconPath = "UI/Gods/Icons/Mork"

        };
    }
}
