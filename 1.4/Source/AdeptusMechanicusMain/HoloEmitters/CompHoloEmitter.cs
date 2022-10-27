using System;
using System.Collections.Generic;
using System.Linq;
using HoloEmitters.Things;
using RimWorld;
using Verse;

namespace HoloEmitters
{
    // Token: 0x02000051 RID: 81
    public class CompProperties_HoloEmitter : CompProperties
    {
        // Token: 0x06000120 RID: 288 RVA: 0x0000BD6C File Offset: 0x0000A16C
        public CompProperties_HoloEmitter()
        {
            this.compClass = typeof(CompHoloEmitter);
        }

        // Token: 0x040000B4 RID: 180
        public float tickCharge = 0.5f;
    }

    // Token: 0x02000050 RID: 80
    [StaticConstructorOnStartup]
    public class CompHoloEmitter : ThingComp
    {
        // Token: 0x17000015 RID: 21
        // (get) Token: 0x06000113 RID: 275 RVA: 0x0000B574 File Offset: 0x00009974
        // (set) Token: 0x06000114 RID: 276 RVA: 0x0000B58F File Offset: 0x0000998F
        public Pawn SimPawn
        {
            get
            {
                return this.pawn;
            }
            set
            {
                this.pawn = value;
            }
        }

        // Token: 0x06000115 RID: 277 RVA: 0x0000B59C File Offset: 0x0000999C
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            if (this.pawn != null)
            {
                DamageInfo value = new DamageInfo (DamageDefOf.Blunt, 1000, -1f);
                this.SimPawn.Kill(new DamageInfo?(value));
                this.SimPawn.Corpse.Destroy(0);
            }
        }

        // Token: 0x06000116 RID: 278 RVA: 0x0000B5FB File Offset: 0x000099FB
        public override void PostDeSpawn(Map map)
        {
            if (this.pawn != null && this.pawn.Spawned)
            {
                this.pawn.DeSpawn();
            }
            base.PostDeSpawn(map);
        }

        // Token: 0x17000016 RID: 22
        // (get) Token: 0x06000117 RID: 279 RVA: 0x0000B62C File Offset: 0x00009A2C
        private HoloEmitter Emitter
        {
            get
            {
                return this.parent as HoloEmitter;
            }
        }

        // Token: 0x06000118 RID: 280 RVA: 0x0000B64C File Offset: 0x00009A4C
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            if (this.pawn != null)
            {
                string Label = "Format Occupant";
                Action action = delegate ()
                {
                    DamageInfo value = new DamageInfo(DamageDefOf.ExecutionCut, 1000, -1f, instigator: selPawn);
                    this.pawn.Kill(new DamageInfo?(value));
                    this.pawn.Corpse.Destroy(0);
                    this.pawn = null;
                };
                FloatMenuOption floatMenuOption = new FloatMenuOption(Label, action, (MenuOptionPriority)4);
                string Label2 = "Remove Occupant";
                Action action2 = delegate ()
                {
                    DamageInfo value = new DamageInfo(DamageDefOf.ExecutionCut, 1000, -1f, instigator: selPawn);
                    this.pawn.Kill(new DamageInfo?(value));
                    this.pawn.Corpse.Position = this.parent.Position.RandomAdjacentCell8Way();
                    this.pawn = null;
                };
                FloatMenuOption floatMenuOption2 = new FloatMenuOption(Label2, action2, (MenuOptionPriority)4);
                if (selPawn != this.pawn)
                {
                    list.Add(floatMenuOption);
                    list.Add(floatMenuOption2);
                }
            }
            else if (selPawn.health.hediffSet.HasHediff(HediffDef.Named("Holographic"))) // (selPawn.story.traits.HasTrait(TraitDef.Named("Holographic")))
            {
                string Label = "Transfer to this emitter";
                Action action = delegate ()
                {
                    foreach (Thing thing in this.parent.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("HolographicEmitter")))
                    {
                        HoloEmitter holoEmitter = thing as HoloEmitter;
                        if (holoEmitter == null)
                        {
                            break;
                        }
                        if (holoEmitter.GetComp<CompHoloEmitter>().SimPawn == selPawn)
                        {
                            holoEmitter.GetComp<CompHoloEmitter>().SimPawn = null;
                            this.pawn = selPawn;
                            this.parent.Map.areaManager.AllAreas.Remove(this.pawn.playerSettings.AreaRestriction);
                            this.MakeValidAllowedZone();
                            break;
                        }
                    }
                };
                FloatMenuOption floatMenuOption2 = new FloatMenuOption(Label,action,(MenuOptionPriority)4);
                if (this.pawn == null)
                {
                    list.Add(floatMenuOption2);
                }
            }
            return list;
        }

        // Token: 0x06000119 RID: 281 RVA: 0x0000B728 File Offset: 0x00009B28
        public override void PostExposeData()
        {
            Scribe_References.Look<Pawn>(ref this.pawn, "simulatedPawn", false);
        }

        // Token: 0x0600011A RID: 282 RVA: 0x0000B73C File Offset: 0x00009B3C
        public void SetUpPawn()
        {
            if (this.pawn.Spawned)
            {
                this.pawn.DeSpawn();
            }
            this.pawn.health.Reset();
            this.pawn.health.AddHediff(HediffDef.Named("Holographic"));
            GenSpawn.Spawn(this.pawn, this.parent.Position, this.parent.Map);
            if (pawn.apparel != null)
            {
                List<Apparel> wornApparel = pawn.apparel.WornApparel;
                for (int j = 0; j < wornApparel.Count; j++)
                {
                    wornApparel[j].Notify_PawnResurrected();
                }
            }
            this.MakeValidAllowedZone();
        }

        // Token: 0x0600011B RID: 283 RVA: 0x0000B7C4 File Offset: 0x00009BC4
        private void HoloTickPawn()
        {
            if (this.pawn != null)
            {
                if (this.pawn.Dead)
                {
                    Log.Message(string.Format("{0} is dead.", this.pawn.Name.ToStringShort));
                    if (this.pawn.Corpse.holdingOwner != this.Emitter.GetDirectlyHeldThings())
                    {
                        if (this.Emitter.TryAcceptThing(this.pawn.Corpse, true))
                        {
                        }
                    }
                }
                else
                {
                    if (!this.pawn.health.hediffSet.HasHediff(HediffDef.Named("Holographic")))
                    {
                        this.SetUpPawn();
                    }
                    if (!this.pawn.Spawned)
                    {
                        GenSpawn.Spawn(this.pawn, this.parent.Position, this.parent.Map);
                    }
                    this.pawn.needs.food.CurLevel = 1f;
                    this.pawn.needs.joy.CurLevel = 1f;
                    this.pawn.needs.rest.CurLevel = 1f;
                    this.pawn.needs.outdoors.CurLevel = 1f;
                    this.pawn.needs.comfort.CurLevel = 1f;
                    this.pawn.needs.beauty.CurLevel = 1f;
                    if (!this.pawn.Position.InHorDistOf(this.parent.Position, 12f) || !GenSight.LineOfSight(this.parent.Position, this.pawn.Position, this.parent.Map, true, null, 0, 0))
                    {
                        this.pawn.inventory.DropAllNearPawn(this.pawn.Position, false, false);
                        this.pawn.equipment.DropAllEquipment(this.pawn.Position, false);
                        this.pawn.DeSpawn();
                        GenSpawn.Spawn(this.pawn, this.parent.Position, this.parent.Map);
                    }
                    this.pawn.health.Reset();
                    this.pawn.health.AddHediff(HediffDef.Named("Holographic"));
                }
            }
        }

        // Token: 0x0600011C RID: 284 RVA: 0x0000B999 File Offset: 0x00009D99
        public void Scan(Corpse c)
        {
            
            if (this.Emitter.TryAcceptThing(c, true))
            {
                c.InnerPawn.health.AddHediff(HediffDef.Named("Holographic"));
            }

            /*
            if (this.Emitter.TryAcceptThing(c, true))
            {
                c.InnerPawn.story.traits.GainTrait(new Trait(TraitDef.Named("Holographic"), 0, false));
            }
            */
        }

        // Token: 0x0600011D RID: 285 RVA: 0x0000B9D8 File Offset: 0x00009DD8
        public void MakeValidAllowedZone()
        {
            IEnumerable<IntVec3> enumerable = from cell in GenRadial.RadialCellsAround(this.parent.Position, 18f, true)
                                              where cell.InHorDistOf(this.parent.Position, 12f) && GenSight.LineOfSight(this.parent.Position, cell, this.parent.Map, true, null, 0, 0)
                                              select cell;
            Area_Allowed area_Allowed;
            this.parent.Map.areaManager.TryMakeNewAllowed(out area_Allowed);
            foreach (IntVec3 intVec in enumerable)
            {
                area_Allowed[this.parent.Map.cellIndices.CellToIndex(intVec)] = true;
            }
            area_Allowed.SetLabel(string.Format("HoloEmitter area for {0}.", this.pawn.Name.ToStringShort));
            this.pawn.playerSettings.AreaRestriction = area_Allowed;
        }

        // Token: 0x0600011E RID: 286 RVA: 0x0000BAB4 File Offset: 0x00009EB4
        public override void CompTickRare()
        {
            base.CompTickRare();
            if (this.pawn != null)
            {
                foreach (Designation designation in this.parent.Map.designationManager.AllDesignationsOn(this.parent))
                {
                    if (designation.def == DesignationDefOf.Uninstall)
                    {
                        if (this.pawn.Spawned)
                        {
                            this.pawn.DeSpawn();
                        }
                        return;
                    }
                }
                if (this.parent.GetComp<CompPowerTrader>().PowerOn)
                {
                    this.HoloTickPawn();
                }
                else if (this.pawn.Spawned)
                {
                    this.pawn.DeSpawn();
                }
            }
        }

        // Token: 0x040000B3 RID: 179
        private Pawn pawn;
    }
}
