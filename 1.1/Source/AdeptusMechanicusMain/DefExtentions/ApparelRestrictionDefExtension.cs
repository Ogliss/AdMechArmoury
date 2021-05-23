using Verse;
using RimWorld;
using System.Collections.Generic;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.ApparelRestrictionDefExtension
    public class ApparelRestrictionDefExtension : DefModExtension
    {
        public bool Any = false;
        public List<ThingDef> RaceDefs = new List<ThingDef>();
        public List<ThingDef> ApparelDefs = new List<ThingDef>();
        public List<HediffDef> HediffDefs = new List<HediffDef>();
        public List<TraitDef> TraitDefs = new List<TraitDef>();
        public List<BodyTypeDef> BodytypeDefs = new List<BodyTypeDef>();
        public Gender gender = Gender.None;
        public BodyTypeDef forcedBodytype = null;
        public List<RaceSpecificTexturePath> raceSpecifics = new List<RaceSpecificTexturePath>();

    }
    public struct RaceSpecificTexturePath
    {
        public string raceDef;
        public string texPath;
    }
}
