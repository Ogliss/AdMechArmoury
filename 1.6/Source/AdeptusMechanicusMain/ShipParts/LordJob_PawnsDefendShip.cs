using RimWorld;
using System;
using Verse;
using Verse.AI.Group;

namespace AdeptusMechanicus
{
    // Token: 0x02000175 RID: 373
    public class LordJob_PawnsDefendShip : LordJob
    {
        // Token: 0x060007BF RID: 1983 RVA: 0x00043D8B File Offset: 0x0004218B
        public LordJob_PawnsDefendShip()
        {
        }

        // Token: 0x060007C0 RID: 1984 RVA: 0x00043D93 File Offset: 0x00042193
        public LordJob_PawnsDefendShip(Thing shipPart, Faction faction, float defendRadius, IntVec3 defSpot)
        {
            this.shipPart = shipPart;
            this.faction = faction;
            this.defendRadius = defendRadius;
            this.defSpot = defSpot;
        }

        // Token: 0x1700013A RID: 314
        // (get) Token: 0x060007C1 RID: 1985 RVA: 0x00043DB8 File Offset: 0x000421B8
        public override bool CanBlockHostileVisitors
        {
            get
            {
                return false;
            }
        }

        // Token: 0x1700013B RID: 315
        // (get) Token: 0x060007C2 RID: 1986 RVA: 0x00043DBB File Offset: 0x000421BB
        public override bool AddFleeToil
        {
            get
            {
                return false;
            }
        }

        // Token: 0x060007C3 RID: 1987 RVA: 0x00043DC0 File Offset: 0x000421C0
        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();
            if (!this.defSpot.IsValid)
            {
                Log.Warning("LordJob_PawnsDefendShip defSpot is invalid. Returning graph for LordJob_AssaultColony.");
                stateGraph.AttachSubgraph(new LordJob_AssaultColony(this.faction, true, true, false, false, true).CreateGraph());
                return stateGraph;
            }
            LordToil_DefendPoint lordToil_DefendPoint = new LordToil_DefendPoint(this.defSpot, this.defendRadius);
            stateGraph.StartingToil = lordToil_DefendPoint;
            LordToil_AssaultColony lordToil_AssaultColony = new LordToil_AssaultColony(false);
            stateGraph.AddToil(lordToil_AssaultColony);
            LordToil_AssaultColony lordToil_AssaultColony2 = new LordToil_AssaultColony(false);
            stateGraph.AddToil(lordToil_AssaultColony2);
            Transition transition = new Transition(lordToil_DefendPoint, lordToil_AssaultColony2, false, true);
            transition.AddSource(lordToil_AssaultColony);
            transition.AddTrigger(new Trigger_PawnCannotReachMapEdge());
            stateGraph.AddTransition(transition, false);
            Transition transition2 = new Transition(lordToil_DefendPoint, lordToil_AssaultColony, false, true);
            transition2.AddTrigger(new Trigger_PawnHarmed(0.5f, true, null));
            transition2.AddTrigger(new Trigger_PawnLostViolently(true));
            transition2.AddTrigger(new Trigger_Memo(CompPawnSpawnerOnDamaged.MemoDamaged));
            transition2.AddPostAction(new TransitionAction_EndAllJobs());
            stateGraph.AddTransition(transition2, false);
            Transition transition3 = new Transition(lordToil_AssaultColony, lordToil_DefendPoint, false, true);
            transition3.AddTrigger(new Trigger_TicksPassedWithoutHarmOrMemos(1380, new string[]
            {
                CompPawnSpawnerOnDamaged.MemoDamaged
            }));
            transition3.AddPostAction(new TransitionAction_EndAttackBuildingJobs());
            stateGraph.AddTransition(transition3, false);
            Transition transition4 = new Transition(lordToil_DefendPoint, lordToil_AssaultColony2, false, true);
            transition4.AddSource(lordToil_AssaultColony);
            transition4.AddTrigger(new Trigger_ThingDamageTaken(this.shipPart, 0.5f));
            transition4.AddTrigger(new Trigger_Memo(HediffGiver_Heat.MemoPawnBurnedByAir));
            stateGraph.AddTransition(transition4, false);
            return stateGraph;
        }

        // Token: 0x060007C4 RID: 1988 RVA: 0x00043F40 File Offset: 0x00042340
        public override void ExposeData()
        {
            Scribe_References.Look<Thing>(ref this.shipPart, "shipPart", false);
            Scribe_References.Look<Faction>(ref this.faction, "faction", false);
            Scribe_Values.Look<float>(ref this.defendRadius, "defendRadius", 0f, false);
            Scribe_Values.Look<IntVec3>(ref this.defSpot, "defSpot", default(IntVec3), false);
        }

        // Token: 0x0400035F RID: 863
        private Thing shipPart;

        // Token: 0x04000360 RID: 864
        private Faction faction;

        // Token: 0x04000361 RID: 865
        private float defendRadius;

        // Token: 0x04000362 RID: 866
        private IntVec3 defSpot;
    }
}
