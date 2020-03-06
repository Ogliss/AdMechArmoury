using AlienRace;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace HediffCompRaceChanger
{
    public class ExtendedTraitEntry
    {
        public TraitDef def = null;
        public int degree = 0;
        public float Chance = 1f;
        public bool replaceIfFull = true;
    }
    // Token: 0x02000241 RID: 577
    public class HediffCompProperties_ChangeRace : HediffCompProperties
    {
        // Token: 0x06000FEE RID: 4078 RVA: 0x0005AFE0 File Offset: 0x000591E0
        public HediffCompProperties_ChangeRace()
        {
            this.compClass = typeof(HediffComp_ChangeRace);
        }

        public bool colourSkin = true;
        public bool colourSkinTwo = true;
        public bool colourHair = false;
        public bool colourHairTwo = false;
        public bool removeHair = true;
        public ThingDef raceDef = null;
        public BodyTypeDef bodyTypeDef = null;
        public List<ExtendedTraitEntry> traitsToAdd = new List<ExtendedTraitEntry>();
        public List<ExtendedTraitEntry> traitsToRemove = new List<ExtendedTraitEntry>();

        // Token: 0x04000BB0 RID: 2992
        public IntRange disappearsAfterTicks = new IntRange(1, 5);

        // Token: 0x04000BB1 RID: 2993
        public bool showRemainingTime = false;
    }
    // Token: 0x02000242 RID: 578
    public class HediffComp_ChangeRace : HediffComp
    {
        // Token: 0x17000327 RID: 807
        // (get) Token: 0x06000FEF RID: 4079 RVA: 0x0005AFF8 File Offset: 0x000591F8
        public HediffCompProperties_ChangeRace Props
        {
            get
            {
                return (HediffCompProperties_ChangeRace)this.props;
            }
        }

        // Token: 0x17000328 RID: 808
        // (get) Token: 0x06000FF0 RID: 4080 RVA: 0x0005B005 File Offset: 0x00059205
        public override bool CompShouldRemove
        {
            get
            {
                return base.CompShouldRemove || this.ticksToDisappear <= 0;
            }
        }

        // Token: 0x17000329 RID: 809
        // (get) Token: 0x06000FF1 RID: 4081 RVA: 0x0005B020 File Offset: 0x00059220
        public override string CompLabelInBracketsExtra
        {
            get
            {
                if (!this.Props.showRemainingTime)
                {
                    return base.CompLabelInBracketsExtra;
                }
                return this.ticksToDisappear.TicksToSeconds().ToString("0.0");
            }
        }
        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            CompPostPostRemoved();
        }
        public override void CompPostPostRemoved()
        {
            TransformPawn();
            base.CompPostPostRemoved();
        }

        private void TransformPawn(bool changeDef = true, bool keep = false)
        {
            //sets position, faction and map
            IntVec3 intv = parent.pawn.Position;
            Faction faction = parent.pawn.Faction;
            Map map = parent.pawn.Map;
            RegionListersUpdater.DeregisterInRegions(parent.pawn, map);

            //Change Race to Props.raceDef
            var thingDef = Props.raceDef ?? null;
            if (changeDef && thingDef != null && thingDef != parent.pawn.def)
            {
                parent.pawn.def = thingDef;
                long ageB = Pawn.ageTracker.AgeBiologicalTicks;
                long ageC = Pawn.ageTracker.AgeChronologicalTicks;
                Pawn.ageTracker = new Pawn_AgeTracker(Pawn);
                Pawn.ageTracker.AgeBiologicalTicks = ageB;
                Pawn.ageTracker.AgeChronologicalTicks = ageC;
                AlienRace.ThingDef_AlienRace alienRace = (AlienRace.ThingDef_AlienRace)thingDef;
                AlienRace.AlienPartGenerator.AlienComp alien = parent.pawn.TryGetComp<AlienRace.AlienPartGenerator.AlienComp>();
               
                if (Props.colourSkinTwo || Props.colourSkin)
                {
                    if (Props.colourSkin)
                    {
                        if (alien != null)
                        {
                            alien.skinColor = alienRace.alienRace.generalSettings.alienPartGenerator.SkinColor(parent.pawn);
                        }
                    }
                    if (Props.colourSkinTwo)
                    {
                        if (alien != null)
                        {
                            alien.skinColorSecond = alienRace.alienRace.generalSettings.alienPartGenerator.SkinColor(parent.pawn, false);
                        }
                    }
                    parent.pawn.Notify_ColorChanged();
                }
                if (Props.removeHair)
                {
                    parent.pawn.story.hairDef = PawnHairChooser.RandomHairDefFor(Pawn, noHairFaction);
                }
                else
                {
                    if (Props.colourHairTwo || Props.colourHair)
                    {

                        if (Props.colourHair)
                        {
                            if (alien != null)
                            {
                                Pawn.story.hairColor = alienRace.alienRace.generalSettings.alienPartGenerator.alienhaircolorgen.NewRandomizedColor(); ;
                            }
                        }
                        if (Props.colourHairTwo)
                        {
                            if (alien != null)
                            {
                                alien.hairColorSecond = alienRace.alienRace.generalSettings.alienPartGenerator.alienhaircolorgen.NewRandomizedColor();
                            }
                        }
                        parent.pawn.Notify_ColorChanged();
                    }
                }

                string head = alienRace.alienRace.graphicPaths.GetCurrentGraphicPath(Pawn.ageTracker.CurLifeStageRace.def).head;
                Traverse.Create(Pawn.story).Field("headGraphicPath").SetValue(alienRace.alienRace.generalSettings.alienPartGenerator.RandomAlienHead(head, Pawn));

            }
            //Remove Disallowed Traits
            int maxTraits;
            if (MoreTraitSlotsUtil.TryGetMaxTraitSlots(out int max))
            {
                maxTraits = max;
            }
            else { maxTraits = 4; }
            if (parent.pawn.story.traits.allTraits.Any(x => Props.traitsToRemove.Any(y => y.def == x.def)))
            {
                foreach (ExtendedTraitEntry item in Props.traitsToRemove)
                {
                    if (parent.pawn.story.traits.HasTrait(item.def))
                    {
                        Rand.PushState();
                        if (Rand.Chance(item.Chance))
                        {
                            parent.pawn.story.traits.allTraits.Remove(parent.pawn.story.traits.allTraits.Find(x => x.def == item.def));
                        }
                        Rand.PopState();
                    }
                }

            }
            if (Props.traitsToAdd.Any(x => !parent.pawn.story.traits.HasTrait(x.def)))
            {

                foreach (ExtendedTraitEntry item in Props.traitsToAdd)
                {
                    if (!parent.pawn.story.traits.HasTrait(item.def))
                    {
                        Rand.PushState();
                        if (Rand.Chance(item.Chance))
                        {
                            bool replace = false;
                            int countTraits = parent.pawn.story.traits.allTraits.Count;
                            if (countTraits >= maxTraits)
                            {
                                replace = true;
                            }
                            //   Log.Message(string.Format("i have {0} of a max of {1} traits", countTraits, maxTraits));
                            Trait replacedTrait = parent.pawn.story.traits.allTraits.Where(x => Props.traitsToAdd.Any(y => y.def != x.def)).RandomElement();
                            if (replace)
                            {
                                parent.pawn.story.traits.allTraits.Remove(replacedTrait);
                            }
                            parent.pawn.story.traits.allTraits.Add(new Trait(item.def, item.degree));
                        }
                        Rand.PopState();
                    }
                }
            }
            RegionListersUpdater.RegisterInRegions(parent.pawn, map);
            map.mapPawns.UpdateRegistryForPawn(parent.pawn);
            //Change BodyType to Props.bodyTypeDef
            if (!Pawn.RaceProps.hasGenders)
            {
                Pawn.gender = Gender.None;
            }
            if (Props.bodyTypeDef != null)
            {
                ChangeBodyType(parent.pawn, Props.bodyTypeDef);
            }

            //decache graphics
            parent.pawn.Drawer.renderer.graphics.ResolveAllGraphics();
            Find.ColonistBar.drawer.Notify_RecachedEntries();

            //save the pawn
            parent.pawn.ExposeData();
            if (parent.pawn.Faction != faction)
            {
                parent.pawn.SetFaction(faction);
            }
            //    pawn.Position = intv;
            
        }

        private void ChangeBodyType(Pawn pawn, BodyTypeDef bt)
        {
            var storyTrv = Traverse.Create(pawn.story);
            var newStory = new Pawn_StoryTracker(pawn);
            var newStoryTrv = Traverse.Create(newStory);
            AccessTools.GetFieldNames(typeof(Pawn_StoryTracker))
                    .ForEach(f => newStoryTrv.Field(f).SetValue(storyTrv.Field(f).GetValue()));
            newStory.bodyType = bt;
            pawn.story = newStory;
            pawn.Drawer.renderer.graphics.ResolveAllGraphics();
            Find.ColonistBar.drawer.Notify_RecachedEntries();
        }

        // Token: 0x06000FF2 RID: 4082 RVA: 0x0005B059 File Offset: 0x00059259
        public override void CompPostMake()
        {
            base.CompPostMake();
            this.ticksToDisappear = this.Props.disappearsAfterTicks.RandomInRange;
        }

        // Token: 0x06000FF3 RID: 4083 RVA: 0x0005B077 File Offset: 0x00059277
        public override void CompPostTick(ref float severityAdjustment)
        {
            this.ticksToDisappear--;
        }

        // Token: 0x06000FF4 RID: 4084 RVA: 0x0005B088 File Offset: 0x00059288
        public override void CompPostMerged(Hediff other)
        {
            base.CompPostMerged(other);
            HediffComp_ChangeRace hediffComp_Disappears = other.TryGetComp<HediffComp_ChangeRace>();
            if (hediffComp_Disappears != null && hediffComp_Disappears.ticksToDisappear > this.ticksToDisappear)
            {
                this.ticksToDisappear = hediffComp_Disappears.ticksToDisappear;
            }
        }

        // Token: 0x06000FF5 RID: 4085 RVA: 0x0005B0C0 File Offset: 0x000592C0
        public override void CompExposeData()
        {
            Scribe_Values.Look<int>(ref this.ticksToDisappear, "ticksToDisappear", 0, false);
        }

        // Token: 0x06000FF6 RID: 4086 RVA: 0x0005B0D4 File Offset: 0x000592D4
        public override string CompDebugString()
        {
            return "ticksToDisappear: " + this.ticksToDisappear;
        }

        // Token: 0x04000BB2 RID: 2994
        public int ticksToDisappear;
        private static readonly FactionDef noHairFaction = new FactionDef
        {
            hairTags = new List<string>
            {
                "alienNoHair"
            }
        };
    }
    static class MoreTraitSlotsUtil
    {
        private static bool initialized = false;
        private static FieldInfo settingsFieldInfo = null;
        private static FieldInfo maxFieldInfo = null;

        public static bool TryGetMaxTraitSlots(out int max)
        {
            if (!initialized)
            {
                initialized = true;
                Initialized();
            }

            if (settingsFieldInfo != null && maxFieldInfo != null)
            {
                object settings = settingsFieldInfo.GetValue(null);
                if (settings != null)
                {
                    max = (int)(float)maxFieldInfo.GetValue(settings);
                    return true;
                }
            }
            max = 0;
            return false;
        }

        private static void Initialized()
        {
            foreach (ModContentPack p in LoadedModManager.RunningMods)
            {
                if (p.Name.IndexOf("More Trait Slots") != -1)
                {
                    foreach (Assembly assembly in p.assemblies.loadedAssemblies)
                    {
                        Type type = assembly.GetType("MoreTraitSlots.RMTS");
                        if (type != null)
                        {
                            settingsFieldInfo = type.GetField("Settings", BindingFlags.Public | BindingFlags.Static);
                            if (settingsFieldInfo != null)
                            {
                                Type st = settingsFieldInfo.GetValue(null).GetType();
                                maxFieldInfo = st.GetField("traitsMax", BindingFlags.Public | BindingFlags.Instance);
                            }
                            return;
                        }
                    }
                }
            }
        }
    }
    // Token: 0x0200001C RID: 28
    internal static class GraphicPathsExtension
    {
        // Token: 0x060000AF RID: 175 RVA: 0x00009DC0 File Offset: 0x00007FC0
        public static GraphicPaths GetCurrentGraphicPath(this List<GraphicPaths> list, LifeStageDef lifeStageDef)
        {
            return list.FirstOrDefault(delegate (GraphicPaths gp)
            {
                List<LifeStageDef> lifeStageDefs = gp.lifeStageDefs;
                return lifeStageDefs != null && lifeStageDefs.Contains(lifeStageDef);
            }) ?? list.First<GraphicPaths>();
        }
    }
}
