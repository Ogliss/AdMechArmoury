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
        /*
        public override void ResolveReferences()
        {

            base.ResolveReferences();


            if (!this.addToDatabase || BackstoryDatabase.allBackstories.ContainsKey(key: this.defName) || this.title.NullOrEmpty() || this.spawnCategories.NullOrEmpty()) return;

            Rand.PushState();
            this.backstory = new Backstory
            {
                slot = this.slot,
                shuffleable = this.shuffleable,
                spawnCategories = this.spawnCategories,
                forcedTraits = this.forcedTraits.NullOrEmpty() ? null : this.forcedTraits.Where(predicate: trait => Rand.Range(min: 0, max: 100) < trait.chance).ToList().ConvertAll(converter: trait => new TraitEntry(def: TraitDef.Named(defName: trait.defName), degree: trait.degree)),
                disallowedTraits = this.disallowedTraits.NullOrEmpty() ? null : this.disallowedTraits.Where(predicate: trait => Rand.Range(min: 0, max: 100) < trait.chance).ToList().ConvertAll(converter: trait => new TraitEntry(def: TraitDef.Named(defName: trait.defName), degree: trait.degree)),
                workDisables = this.workAllows.NullOrEmpty() ? this.workDisables.NullOrEmpty() ? WorkTags.None : ((Func<WorkTags>)delegate
                {
                    WorkTags wt = WorkTags.None;
                    this.workDisables.ForEach(action: tag => wt |= tag);
                    return wt;
                })() : ((Func<WorkTags>)delegate
                {
                    WorkTags wt = WorkTags.None;
                    Enum.GetValues(enumType: typeof(WorkTags)).Cast<WorkTags>().Where(predicate: tag => !this.workAllows.Contains(item: tag)).ToList().ForEach(action: tag => wt |= tag);
                    return wt;
                })(),
                identifier = this.defName,
                requiredWorkTags = ((Func<WorkTags>)delegate
                {
                    WorkTags wt = WorkTags.None;
                    this.requiredWorkTags.ForEach(action: tag => wt |= tag);
                    return wt;
                })()
                
            };
            Rand.PopState();
            if (!this.nameMaker.NullOrEmpty())
            {
                Traverse.Create(root: this.backstory).Field(name: "nameMaker").SetValue(value: this.nameMaker);
            }
            Traverse.Create(root: this.backstory).Field(name: "bodyTypeGlobalResolved").SetValue(value: this.bodyTypeGlobal);
            Traverse.Create(root: this.backstory).Field(name: "bodyTypeFemaleResolved").SetValue(value: this.bodyTypeFemale);
            Traverse.Create(root: this.backstory).Field(name: "bodyTypeMaleResolved").SetValue(value: this.bodyTypeMale);

            Traverse.Create(root: this.backstory).Field(name: nameof(this.skillGains)).SetValue(value: this.SkillListItems.ToDictionary(keySelector: i => i.defName, elementSelector: i => i.amount));

            UpdateTranslateableFields(bs: this);

            this.backstory.ResolveReferences();
            this.backstory.PostLoad();

            this.backstory.identifier = this.defName;

            IEnumerable<string> errors;
            if (!(errors = this.backstory.ConfigErrors(ignoreNoSpawnCategories: false)).Any())
                BackstoryDatabase.AddBackstory(bs: this.backstory);
            else
                Log.Error(text: this.defName + " has errors:\n" + string.Join(separator: "\n", value: errors.ToArray()));
        }
        internal static void UpdateTranslateableFields(BackstoryDef bs)
        {
            if (bs.backstory == null) return;

            bs.backstory.baseDesc = bs.baseDescription.NullOrEmpty() ? "Empty." : bs.baseDescription;
            bs.backstory.SetTitle(newTitle: bs.title, newTitleFemale: bs.titleFemale);
            bs.backstory.SetTitleShort(newTitleShort: bs.titleShort.NullOrEmpty() ? bs.title : bs.titleShort,
                newTitleShortFemale: bs.titleShortFemale.NullOrEmpty() ? bs.titleFemale : bs.titleShortFemale);
        }
        
        */
    }
}