using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus
{
    public class PawnBioDef : Def
    {
        // Token: 0x060000BB RID: 187 RVA: 0x0000B466 File Offset: 0x00009666
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
            IEnumerator<string> enumerator = null;
            yield break;
            yield break;
        }

        // Token: 0x060000BC RID: 188 RVA: 0x0000B478 File Offset: 0x00009678
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
        }

        // Token: 0x04000031 RID: 49
        public BackstoryDef childhood;

        // Token: 0x04000032 RID: 50
        public BackstoryDef adulthood;

        // Token: 0x04000033 RID: 51
        public GenderPossibility gender;

        // Token: 0x04000034 RID: 52
        public NameTriple name;

        // Token: 0x04000035 RID: 53
        public List<ThingDef> validRaces;

        // Token: 0x04000036 RID: 54
        public bool factionLeader;

        // Token: 0x04000037 RID: 55
        public List<string> forcedHediffs = new List<string>();

        // Token: 0x04000038 RID: 56
        public List<ThingDefCountRangeClass> forcedItems = new List<ThingDefCountRangeClass>();
    }
}