using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.FactionDefExtension
    public class FactionDefExtension : DefModExtension
    {
        public List<FactionDef> RefugeeFactions = new List<FactionDef>();
        public List<ChanceTraitEntry> ForcedTraits = new List<ChanceTraitEntry>();
        public List<HediffGiverSetDef> hediffGivers = new List<HediffGiverSetDef>();
        public ThingDef ActiveDropPod = DefDatabase<ThingDef>.GetNamedSilentFail("ActiveDropPod");
        public ThingDef DropPodIncoming = DefDatabase<ThingDef>.GetNamedSilentFail("DropPodIncoming");
        public ReserveDeploymentType DropPodOverride = ReserveDeploymentType.DropPod;
        public string TeleportBoltTexPath = "Weather/DefaultBolt";
        public float DeepStrikeChance = 0.1f;
        public FloatRange DeepStrikeDelayMin = new FloatRange(30, 120);
        public FloatRange DeepStrikeDelayMax = new FloatRange(180, 240);
        public float InfiltrateChance = 0.1f;
        public FloatRange InfiltrateDelayMin = new FloatRange(15, 90);
        public FloatRange InfiltrateDelayMax = new FloatRange(90, 180);
        public string factionColourTag = string.Empty;
        public string factionTextureTag = string.Empty;
        public string factionMaskTag = string.Empty;
        public Color? factionColor;
        public Color? factionColorTwo;
        public FactionDef linkedFaction;

        public FactionDefExtension()
        { }

        public FactionDefExtension(FactionDefExtension copyFrom)
        {
            RefugeeFactions = copyFrom.RefugeeFactions;
            ForcedTraits = copyFrom.ForcedTraits;
            hediffGivers = copyFrom.hediffGivers;
            ActiveDropPod = copyFrom.ActiveDropPod;
            DropPodIncoming = copyFrom.DropPodIncoming;
            DropPodOverride = copyFrom.DropPodOverride;
            TeleportBoltTexPath = copyFrom.TeleportBoltTexPath;
            DeepStrikeChance = copyFrom.DeepStrikeChance;
            DeepStrikeDelayMin = copyFrom.DeepStrikeDelayMin;
            DeepStrikeDelayMax = copyFrom.DeepStrikeDelayMax;
            InfiltrateChance = copyFrom.InfiltrateChance;
            InfiltrateDelayMin = copyFrom.InfiltrateDelayMin;
            InfiltrateDelayMax = copyFrom.InfiltrateDelayMax;
            factionColourTag = copyFrom.factionColourTag;
            factionTextureTag = copyFrom.factionTextureTag;
            factionMaskTag = copyFrom.factionMaskTag;
            factionColor = copyFrom.factionColor;
            factionColorTwo = copyFrom.factionColorTwo;
            linkedFaction = copyFrom.linkedFaction;
        }

    }
}
