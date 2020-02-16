using System;
using System.Collections.Generic;
using System.Reflection;
using HoloEmitters.Things;
using RimWorld;
using Verse;
using Verse.AI;

namespace HoloEmitters
{
    // Token: 0x0200004B RID: 75
    public class CompProperties_Osiris : CompProperties
    {
        // Token: 0x060000F9 RID: 249 RVA: 0x0000A0E0 File Offset: 0x000084E0
        public CompProperties_Osiris()
        {
            this.compClass = typeof(CompOsiris);
        }

        // Token: 0x040000AD RID: 173
        public float tickCharge = 0.5f;
    }

    // Token: 0x0200004C RID: 76
    [StaticConstructorOnStartup]
    public class CompOsiris : ThingComp
    {
        // Token: 0x1700000E RID: 14
        // (get) Token: 0x060000FB RID: 251 RVA: 0x0000A10C File Offset: 0x0000850C
        public Building_Casket Casket
        {
            get
            {
                return this.parent as Building_Casket;
            }
        }

        // Token: 0x1700000F RID: 15
        // (get) Token: 0x060000FC RID: 252 RVA: 0x0000A12C File Offset: 0x0000852C
        private bool ReadyToHeal
        {
            get
            {
                return this.Casket.ContainedThing is Pawn || (this.Casket.ContainedThing is Corpse && !RottableUtility.IsNotFresh(this.Casket.ContainedThing as Corpse) && this.parent.GetComp<CompPowerTrader>().PowerOn && this.parent.GetComp<CompRefuelable>().Fuel >= 50f);
            }
        }

        // Token: 0x060000FD RID: 253 RVA: 0x0000A1BA File Offset: 0x000085BA
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            if (mode == DestroyMode.KillFinalize)
            {
                GenSpawn.Spawn(ThingDef.Named("OsirisAI"), this.parent.Position, previousMap);
            }
        }

        // Token: 0x060000FE RID: 254 RVA: 0x0000A1E8 File Offset: 0x000085E8
        public static void AddComponentsForResurrection(Pawn pawn)
        {
            pawn.carryTracker = new Pawn_CarryTracker(pawn);
            pawn.needs = new Pawn_NeedsTracker(pawn);
            pawn.mindState = new Pawn_MindState(pawn);
            if (pawn.RaceProps.Humanlike)
            {
                pawn.workSettings = new Pawn_WorkSettings(pawn);
                pawn.workSettings.EnableAndInitialize();
            }
            PawnComponentsUtility.AddAndRemoveDynamicComponents(pawn, false);
        }

        // Token: 0x060000FF RID: 255 RVA: 0x0000A24C File Offset: 0x0000864C
        public static void Ressurrect(Pawn pawn, Thing thing)
        {
            pawn.health.Reset();
            if (pawn.Corpse != null && pawn.Corpse.Spawned)
            {
                pawn.Corpse.DeSpawn();
            }
            CompOsiris.AddComponentsForResurrection(pawn);
            Type typeFromHandle = typeof(Thing);
            FieldInfo field = typeFromHandle.GetField("mapIndexOrState", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(pawn, (SByte)(-1));
            CompOsiris.FixPawnRelationships(pawn);
            if (thing is HoloEmitter)
            {
                if (!pawn.Dead)
                {
                }
                if (pawn.Corpse.holdingOwner != null)
                {
                    pawn.Corpse.GetDirectlyHeldThings().TryTransferToContainer(pawn, pawn.Corpse.holdingOwner, true);
                }
                else if (pawn.Corpse.Spawned)
                {
                    GenSpawn.Spawn(pawn, pawn.Corpse.Position, pawn.Corpse.Map);
                }
                if (pawn.Corpse != null)
                {
                    pawn.Corpse.Destroy(0);
                }
            }
            else
            {
                GenSpawn.Spawn(pawn, thing.Position, thing.Map);
                Building_Casket building_Casket = thing as Building_Casket;
                if (building_Casket != null)
                {
                    building_Casket.GetDirectlyHeldThings().Clear();
                }
            }
        }

        // Token: 0x06000100 RID: 256 RVA: 0x0000A384 File Offset: 0x00008784
        public void HealContained()
        {
            if (this.Casket.ContainedThing != null)
            {
                Corpse corpse = this.Casket.ContainedThing as Corpse;
                Pawn pawn;
                if (corpse != null)
                {
                    pawn = corpse.InnerPawn;
                }
                else
                {
                    pawn = (this.Casket.ContainedThing as Pawn);
                }
                if (pawn != null)
                {
                    if (pawn.Dead)
                    {
                        CompOsiris.Ressurrect(pawn, this.parent);
                    }
                    else
                    {
                        pawn.health.Reset();
                    }
                    if (pawn.RaceProps.Humanlike)
                    {
                        pawn.ageTracker.AgeBiologicalTicks = 90000000L;
                        if (Rand.Value < 0.65f)
                        {
                            pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("ReturnedFromTheDeadBad"), null);
                        }
                        else
                        {
                            pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("ReturnedFromTheDeadGood"), null);
                        }
                    }
                    else if (pawn.RaceProps.Animal)
                    {
                        pawn.ageTracker.AgeBiologicalTicks = (long)(pawn.RaceProps.lifeStageAges[2].minAge * 3600000f);
                    }
                    pawn.health.AddHediff(HediffDef.Named("LuciferiumAddiction"), null, null);
                    pawn.health.AddHediff(HediffDef.Named("LuciferiumHigh"), null, null);
                }
            }
        }

        // Token: 0x06000101 RID: 257 RVA: 0x0000A510 File Offset: 0x00008910
        private static void FixPawnRelationships(Pawn p)
        {
            foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive)
            {
                if (pawn != p)
                {
                    if (pawn != null && pawn.needs != null && pawn.needs.mood != null && pawn.needs.mood.thoughts != null && pawn.needs.mood.thoughts.memories != null)
                    {
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.KnowColonistDied, p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.PawnWithBadOpinionDied, p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.PawnWithGoodOpinionDied, p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("BondedAnimalDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MySonDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyDaughterDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyHusbandDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyWifeDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyFianceDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyFianceeDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyLoverDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyBrotherDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MySisterDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyGrandchildDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyFatherDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyMotherDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyNieceDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyNephewDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyHalfSiblingDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyAuntDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyUncleDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyGrandparentDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyCousinDied"), p);
                        pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDef.Named("MyKinDied"), p);
                    }
                }
            }
        }
        /*
        // Token: 0x06000102 RID: 258 RVA: 0x0000A93C File Offset: 0x00008D3C
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            string Label = "Resurrect contained";
            Action action = delegate ()
            {
                Job job = new Job(JobDefOfHoloEmitters.ActivateOsirisCasket, this.parent);
                job.playerForced = true;
                selPawn.jobs.TryTakeOrderedJob(job, 0);
            };
            FloatMenuOption floatMenuOption = new FloatMenuOption(Label,action);
            if (this.ReadyToHeal)
            {
                list.Add(floatMenuOption);
            }
            return list;
        }
        */
    }
}
