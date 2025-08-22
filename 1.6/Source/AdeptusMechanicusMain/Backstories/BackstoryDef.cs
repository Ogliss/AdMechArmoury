using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using HarmonyLib;

namespace AdeptusMechanicus
{
    //Pulled from erdelf's Alien Races 2.0
    //Original credit and work belong to erdelf (https://github.com/erdelf)
    //Link -> https://github.com/RimWorld-CCL-Reborn/AlienRaces/blob/94bf6b6d7a91e9587bdc40e8a231b18515cb6bb7/Source/AlienRace/AlienRace/BackstoryDef.cs
    public class BackstoryDef : RimWorld.BackstoryDef
    {
        public bool addToDatabase = true;
        public WorkTags workAllows = WorkTags.AllWork;
        public float maleCommonality = 100f;
        public float femaleCommonality = 100f;
        public BackstoryDef linkedBackstory;
        //public RelationSettings relationSettings = new RelationSettings();
        public List<string> forcedHediffs = new List<string>();
        public IntRange bioAgeRange;
        public IntRange chronoAgeRange;
        public List<ThingDefCountRangeClass> forcedItems = new List<ThingDefCountRangeClass>();
        //   public Backstory backstory;
        public List<SkillGain> passions = new List<SkillGain>();

        public List<BackstoryTraitChance> forcedTraitsChance = new List<BackstoryTraitChance>();
        public List<BackstoryTraitChance> disallowedTraitsChance = new List<BackstoryTraitChance>();
		public static HashSet<BackstoryDef> checkBodyType = new HashSet<BackstoryDef>();

        public class BackstoryTraitChance
        {
            public TraitDef defName;
            public int degree = 0;
            public float chance = 100f;
            public float commonalityMale = -1f;
            public float commonalityFemale = -1f;
        }

        public bool CommonalityApproved(Gender g)
        {
            return (float)Rand.Range(0, 100) < ((g == Gender.Female) ? this.femaleCommonality : this.maleCommonality);
        }

        public bool Approved(Pawn p)
        {
            return this.CommonalityApproved(p.gender) && (this.bioAgeRange == default(IntRange) || (this.bioAgeRange.min < p.ageTracker.AgeBiologicalYears && p.ageTracker.AgeBiologicalYears < this.bioAgeRange.max)) && (this.chronoAgeRange == default(IntRange) || (this.chronoAgeRange.min < p.ageTracker.AgeChronologicalYears && p.ageTracker.AgeChronologicalYears < this.chronoAgeRange.max));
        }

        public override void ResolveReferences()
        {
            this.identifier = this.defName;
            base.ResolveReferences();
            List<BackstoryTrait> first;
            if ((first = this.forcedTraits) == null)
            {
                first = (this.forcedTraits = new List<BackstoryTrait>());
            }
            this.forcedTraits = first.Concat((from trait in this.forcedTraitsChance
                                              where Rand.Range(0, 100) < trait.chance
                                              select trait).ToList<BackstoryTraitChance>().ConvertAll<BackstoryTrait>((BackstoryTraitChance trait) => new BackstoryTrait
                                              {
                                                  def = trait.defName,
                                                  degree = trait.degree
                                              })).ToList<BackstoryTrait>();
            List<BackstoryTrait> first2;
            if ((first2 = this.disallowedTraits) == null)
            {
                first2 = (this.disallowedTraits = new List<BackstoryTrait>());
            }
            this.disallowedTraits = first2.Concat((from trait in this.disallowedTraitsChance
                                                   where (float)Rand.Range(0, 100) < trait.chance
                                                   select trait).ToList<BackstoryTraitChance>().ConvertAll<BackstoryTrait>((BackstoryTraitChance trait) => new BackstoryTrait
                                                   {
                                                       def = trait.defName,
                                                       degree = trait.degree
                                                   })).ToList<BackstoryTrait>();
            this.workDisables = (((this.workAllows & WorkTags.AllWork) != WorkTags.None) ? this.workDisables : (~this.workAllows));
            if (this.bodyTypeGlobal == null && this.bodyTypeFemale == null && this.bodyTypeMale == null)
            {
                BackstoryDef.checkBodyType.Add(this);
                this.bodyTypeGlobal = DefDatabase<BodyTypeDef>.GetRandom();
            }
        }
    }
}