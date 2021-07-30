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
			
			if (__instance.ideo.culture.defName.StartsWith("OG_Imperial_"))
			{
				IdeoFoundation_Deity.Deity emperor = DeityUtility.Emperor.cloneDeity();
				FillDeity(__instance, emperor);
				__instance.deities.Add(emperor);
			}
			if (__instance.ideo.culture.defName.StartsWith("OG_Mechanicus"))
			{
				IdeoFoundation_Deity.Deity omnissiah = DeityUtility.Omnissiah.cloneDeity();
				FillDeity(__instance, omnissiah);
				__instance.deities.Add(omnissiah);
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
            if (deity.relatedMeme == null)
			{
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
            iconPath = "Ui/Gods/Icons/EmperorOfMan",
			relatedMeme = AdeptusMemeDefOf.OG_Imperial_Structure_TheistEmbodied

		};
        public static IdeoFoundation_Deity.Deity Omnissiah = new IdeoFoundation_Deity.Deity()
        {
            name = "Ommnissiah",
            gender = Gender.None,
            type = "AdeptusMechanicus.Imperial.Ommnissiah_Desc".Translate(),
            iconPath = "Ui/Gods/Icons/Ommnissiah",
			relatedMeme = MemeDefOf.Transhumanist

		};
        // Aeldari Gods
        public static IdeoFoundation_Deity.Deity Khaine = new IdeoFoundation_Deity.Deity()
        {
            name = "Khaine",
            gender = Gender.None,
            type = "AdeptusMechanicus.Eldar.Khaine_Desc".Translate(),
            iconPath = "Ui/Gods/Icons/Khaine"

		};
        public static IdeoFoundation_Deity.Deity Yeanned = new IdeoFoundation_Deity.Deity()
        {
            name = "Yeanned",
            gender = Gender.None,
            type = "AdeptusMechanicus.Eldar.Yeanned_Desc".Translate(),
            iconPath = "Ui/Gods/Icons/Yeanned"

		};
        public static IdeoFoundation_Deity.Deity Cregoarch = new IdeoFoundation_Deity.Deity()
        {
            name = "Cregoarch",
            gender = Gender.None,
            type = "AdeptusMechanicus.Eldar.Cregoarch_Desc".Translate(),
            iconPath = "Ui/Gods/Icons/Cregoarch"

		};
        // Orkish Gods
        public static IdeoFoundation_Deity.Deity Gork = new IdeoFoundation_Deity.Deity()
        {
            name = "Gork",
            gender = Gender.None,
            type = "AdeptusMechanicus.Ork.Gork_Desc".Translate(),
            iconPath = "Ui/Gods/Icons/Gork"

		};
        public static IdeoFoundation_Deity.Deity Mork = new IdeoFoundation_Deity.Deity()
        {
            name = "Mork",
            gender = Gender.None,
            type = "AdeptusMechanicus.Ork.Mork_Desc".Translate(),
            iconPath = "Ui/Gods/Icons/Mork"

		};
        // Kroot Gods
        public static IdeoFoundation_Deity.Deity Vawk = new IdeoFoundation_Deity.Deity()
        {
            name = "Vawk",
            gender = Gender.Female,
            type = "AdeptusMechanicus.Kroot.Vawk_Desc".Translate(),
            iconPath = "Ui/Gods/Icons/Vawk"

		};
        public static IdeoFoundation_Deity.Deity Gmork = new IdeoFoundation_Deity.Deity()
        {
            name = "Gmork",
            gender = Gender.Male,
            type = "AdeptusMechanicus.Kroot.Gmork_Desc".Translate(),
            iconPath = "Ui/Gods/Icons/Gmork"

		};
    }
}
