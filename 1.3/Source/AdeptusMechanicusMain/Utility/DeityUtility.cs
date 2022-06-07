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
			AdeptusMechanicus.CultureDef culture = __instance.ideo.culture as AdeptusMechanicus.CultureDef;
			if (culture != null)
			{
				bool required = !culture.deities.requiredDeities.NullOrEmpty();
				bool possible = !culture.deities.possibleDeities.NullOrEmpty();
				bool fill = required || possible;
				if (fill)
				{
					List<DeityDef> usedDefs = new List<DeityDef>();
					if (required)
					{
						foreach (var def in culture.deities.requiredDeities)
						{
							usedDefs.Add(def);
						}
					}
					if (possible)
					{
						int max = culture.deities.max > culture.deities.possibleDeities.Count ? culture.deities.possibleDeities.Count : culture.deities.max;
						int min = culture.deities.min;
						Rand.PushState();
						int take = Rand.RangeInclusive(min, max);
						Rand.PopState();
						foreach (var def in culture.deities.possibleDeities.TakeRandom(take))
						{
							usedDefs.Add(def);
						}
					}
					if (!usedDefs.NullOrEmpty())
					{
						__instance.deities.Clear();
						if (culture.deities.randomizeOrder)
						{
							usedDefs = usedDefs.InRandomOrder().ToList();
						}
						foreach (var def in usedDefs)
						{
							IdeoFoundation_Deity.Deity god = def.Deity();
							FillDeity(__instance, god);
							__instance.deities.Add(god);
						}
						return;
					}
				}
			}

            if (__instance.ideo.culture.defName.StartsWith("OG_"))
			{
				__instance.deities.Clear();
				if (__instance.ideo.culture.defName.Contains("Mechanicus"))
				{
					IdeoFoundation_Deity.Deity omnissiah = DeityUtility.Omnissiah.cloneDeity();
					FillDeity(__instance, omnissiah);
					__instance.deities.Add(omnissiah);
					return;
				}
				if (__instance.ideo.culture.defName.Contains("Imperial"))
				{
					IdeoFoundation_Deity.Deity emperor = DeityUtility.Emperor.cloneDeity();
					FillDeity(__instance, emperor);
					__instance.deities.Add(emperor);
					return;
				}
				if (__instance.ideo.culture.defName.Contains("Greenskin") || __instance.ideo.culture.defName.Contains("Orkoid"))
				{
					IdeoFoundation_Deity.Deity gork = DeityUtility.Gork.cloneDeity();
					IdeoFoundation_Deity.Deity mork = DeityUtility.Mork.cloneDeity();
					FillDeity(__instance, gork);
					__instance.deities.Add(gork);
					FillDeity(__instance, mork);
					__instance.deities.Add(mork);
					return;
				}
				if (__instance.ideo.culture.defName.Contains("Kroot"))
				{
					IdeoFoundation_Deity.Deity vawk = DeityUtility.Vawk.cloneDeity();
					IdeoFoundation_Deity.Deity gmork = DeityUtility.Gmork.cloneDeity();
					FillDeity(__instance, vawk);
					__instance.deities.Add(vawk);
					FillDeity(__instance, gmork);
					__instance.deities.Add(gmork);
					return;
				}
				if (__instance.ideo.culture.defName.Contains("Aeldari"))
				{
					IdeoFoundation_Deity.Deity khaine = DeityUtility.Khaine.cloneDeity();
					IdeoFoundation_Deity.Deity ynnead = DeityUtility.Ynnead.cloneDeity();
					IdeoFoundation_Deity.Deity cegorach = DeityUtility.Cegorach.cloneDeity();
					FillDeity(__instance, khaine);
					__instance.deities.Add(khaine);
					FillDeity(__instance, ynnead);
					__instance.deities.Add(ynnead);
					FillDeity(__instance, Cegorach);
					__instance.deities.Add(cegorach);
					return;
				}
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
            iconPath = "Ui/Gods/Icons/Icon_EmperorOfMan",
			relatedMeme = AdeptusMemeDefOf.OG_Imperial_Structure_TheistEmbodied

		};
        public static IdeoFoundation_Deity.Deity Omnissiah = new IdeoFoundation_Deity.Deity()
        {
            name = "Ommnissiah",
            gender = Gender.None,
            type = "AdeptusMechanicus.Imperial.Omnissiah_Desc".Translate(),
            iconPath = "Ui/Gods/Icons/Icon_Omnissiah",
			relatedMeme = MemeDefOf.Transhumanist

		};
        // Aeldari Gods
        public static IdeoFoundation_Deity.Deity Khaine = new IdeoFoundation_Deity.Deity()
        {
            name = "Khaine",
            gender = Gender.Male,
            type = "AdeptusMechanicus.Aeldari.Khaine_Desc".Translate(),
            iconPath = "Ui/Gods/Icons/Icon_Khaine"

		};
        public static IdeoFoundation_Deity.Deity Ynnead = new IdeoFoundation_Deity.Deity()
        {
            name = "Ynnead",
            gender = Gender.Male,
            type = "AdeptusMechanicus.Aeldari.Ynnead_Desc".Translate(),
            iconPath = "Ui/Gods/Icons/Icon_Ynnead"

		};
        public static IdeoFoundation_Deity.Deity Cegorach = new IdeoFoundation_Deity.Deity()
        {
            name = "Cegorach",
            gender = Gender.Male,
            type = "AdeptusMechanicus.Aeldari.Cegorach_Desc".Translate(),
            iconPath = "Ui/Gods/Icons/Icon_Cegorach"

		};
        // Orkish Gods
        public static IdeoFoundation_Deity.Deity Gork = new IdeoFoundation_Deity.Deity()
        {
            name = "Gork",
            gender = Gender.None,
            type = "AdeptusMechanicus.Ork.Gork_Desc".Translate(),
            iconPath = "Ui/Gods/Icons/Icon_Gork"

		};
        public static IdeoFoundation_Deity.Deity Mork = new IdeoFoundation_Deity.Deity()
        {
            name = "Mork",
            gender = Gender.None,
            type = "AdeptusMechanicus.Ork.Mork_Desc".Translate(),
            iconPath = "Ui/Gods/Icons/Icon_Mork"

		};
        // Kroot Gods
        public static IdeoFoundation_Deity.Deity Vawk = new IdeoFoundation_Deity.Deity()
        {
            name = "Vawk",
            gender = Gender.Female,
            type = "AdeptusMechanicus.Kroot.Vawk_Desc".Translate(),
            iconPath = "Ui/Gods/Icons/Icon_Vawk"

		};
        public static IdeoFoundation_Deity.Deity Gmork = new IdeoFoundation_Deity.Deity()
        {
            name = "Gmork",
            gender = Gender.Male,
            type = "AdeptusMechanicus.Kroot.Gmork_Desc".Translate(),
            iconPath = "Ui/Gods/Icons/Icon_Gmork"

		};
    }
}
