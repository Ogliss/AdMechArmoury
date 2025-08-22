using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus
{
    public class PawnBioDef : Def
    {
        public override IEnumerable<string> ConfigErrors()
        {
            if (this.childhood == null)
            {
                yield return "Error in " + this.defName + ": Childhood backstory not found";
            }
            if (this.adulthood == null)
            {
                yield return "Error in " + this.defName + ": Childhood backstory not found";
            }
            foreach (string text in base.ConfigErrors())
            {
                yield return text;
            }
            yield break;
        }

        public override void ResolveReferences()
        {
            base.ResolveReferences();
            PawnBio item = new PawnBio
            {
                gender = this.gender,
                name = this.name,
                childhood = this.childhood,
                adulthood = this.adulthood,
                pirateKing = this.factionLeader
            };
            if (this.adulthood.spawnCategories.Count == 1 && this.adulthood.spawnCategories[0] == "Trader")
            {
                this.adulthood.spawnCategories.Add("Civil");
            }
            SolidBioDatabase.allBios.Add(item);
            // if (SolidBioDatabase.allBios.Contains(item)) Log.Message($"{item.name}");
        }

        public BackstoryDef childhood;
        public BackstoryDef adulthood;
        public GenderPossibility gender;
        public NameTriple name;
        public List<ThingDef> validRaces;
        public bool factionLeader;
        public List<string> forcedHediffs = new List<string>();
        public List<ThingDefCountRangeClass> forcedItems = new List<ThingDefCountRangeClass>();
    }
}