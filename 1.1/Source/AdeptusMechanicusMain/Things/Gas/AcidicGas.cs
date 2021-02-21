using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.AcidicGas
    public class AcidicGas : Gas
    {
        public AdeptusGasProperties GasProps => this.def.gas as AdeptusGasProperties;
        public object cachedLabelMouseover { get; private set; }

        // Token: 0x06000019 RID: 25 RVA: 0x000027B3 File Offset: 0x000009B3
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }

        // Token: 0x0600001A RID: 26 RVA: 0x000027C0 File Offset: 0x000009C0
        public override void Tick()
        {
            bool destroyed = base.Destroyed;
            if (!destroyed)
            {
                base.Tick();
                bool flag = this.destroyTick <= Find.TickManager.TicksGame && !base.Destroyed;
                if (flag)
                {
                    this.Destroy(0);
                }
                else
                {
                    this.graphicRotation += this.graphicRotationSpeed;
                    this.Ticks--;
                    bool flag2 = this.Ticks <= 0;
                    if (flag2)
                    {
                        this.TickTack();
                        this.Ticks = this.TickRate;
                    }
                }
            }
        }

        // Token: 0x0600001B RID: 27 RVA: 0x00002854 File Offset: 0x00000A54
        public void TickTack()
        {
            bool destroyed = base.Destroyed;
            if (!destroyed)
            {
                List<Thing> thingList = GridsUtility.GetThingList(base.Position, base.Map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    bool flag = thingList[i] != null;
                    if (flag)
                    {
                        Thing thing = thingList[i];
                        Pawn pawn = thingList[i] as Pawn;
                        bool flag2 = thing != null && !this.touchingThings.Contains(thing);
                        if (flag2)
                        {
                            this.touchingThings.Add(thing);
                            Rand.PushState();
                            this.DamageEntities(thing, Mathf.RoundToInt((float)this.AcidDamage * Rand.Range(0.5f, 1.25f)));
                            Rand.PopState();
                            AdeptusMoteMaker.ThrowDustPuff(thing.DrawPos, base.Map, 0.2f);
                        }
                        bool flag3 = pawn != null;
                        if (flag3)
                        {
                            this.touchingPawns.Add(pawn);
                            bool flag4 = !pawn.RaceProps.Animal;
                            if (flag4)
                            {
                                this.AddAcidDamage(pawn);
                                AdeptusMoteMaker.ThrowDustPuff(pawn.DrawPos, base.Map, 0.2f);
                            }
                        }
                    }
                }
                for (int j = 0; j < this.touchingPawns.Count; j++)
                {
                    Pawn pawn2 = this.touchingPawns[j];
                    bool flag5 = !pawn2.Spawned || pawn2.Position != base.Position;
                    if (flag5)
                    {
                        this.touchingPawns.Remove(pawn2);
                    }
                    else
                    {
                        bool flag6 = !pawn2.RaceProps.Animal;
                        if (flag6)
                        {
                            this.AddAcidDamage(pawn2);
                        }
                    }
                }
                for (int k = 0; k < this.touchingThings.Count; k++)
                {
                    Thing thing2 = this.touchingThings[k];
                    bool flag7 = !thing2.Spawned || thing2.Position != base.Position;
                    if (flag7)
                    {
                        this.touchingThings.Remove(thing2);
                    }
                    else
                    {
                        Rand.PushState();
                        this.DamageEntities(thing2, Mathf.RoundToInt((float)this.AcidDamage * Rand.Range(0.5f, 1.25f)));
                        Rand.PopState();
                    }
                }
                Rand.PushState();
                this.DamageBuildings(Mathf.RoundToInt((float)this.AcidDamage * Rand.Range(0.5f, 1.25f)));
                Rand.PopState();
                this.cachedLabelMouseover = null;
            }
        }

        // Token: 0x0600001C RID: 28 RVA: 0x00002AD4 File Offset: 0x00000CD4
        public void DamageEntities(Thing e, int amt)
        {
            DamageInfo damageInfo = new DamageInfo(DamageDefOf.Burn, (float)amt, 0f, -1f, null, null, null, 0, null);
            bool flag = e != null;
            if (flag)
            {
                e.TakeDamage(damageInfo);
                AdeptusMoteMaker.ThrowDustPuff(e.DrawPos, base.Map, 0.2f);
            }
        }

        // Token: 0x0600001D RID: 29 RVA: 0x00002B28 File Offset: 0x00000D28
        public void DamageBuildings(int amt)
        {
            IntVec3 intVec = GenAdj.RandomAdjacentCell8Way(this);
            bool flag = GenGrid.InBounds(intVec, base.Map);
            bool flag2 = flag;
            if (flag2)
            {
                Building firstBuilding = GridsUtility.GetFirstBuilding(intVec, base.Map);
                DamageInfo damageInfo = new DamageInfo(DamageDefOf.Burn, (float)amt, 0f, -1f, null, null, null, 0, null);
                bool flag3 = firstBuilding != null;
                bool flag4 = flag3;
                if (flag4)
                {
                    firstBuilding.TakeDamage(damageInfo);
                    AdeptusMoteMaker.ThrowDustPuff(firstBuilding.DrawPos, base.Map, 0.2f);
                }
            }
        }

        // Token: 0x0600001E RID: 30 RVA: 0x00002BAC File Offset: 0x00000DAC
        public void AddAcidDamage(Pawn p)
        {
            List<BodyPartRecord> list = new List<BodyPartRecord>();
            List<Apparel> wornApparel = p.apparel.WornApparel;
            Rand.PushState();
            int num = Mathf.RoundToInt((float)this.AcidDamage * Rand.Range(0.5f, 1.25f));
            Rand.PopState();
            DamageInfo damageInfo = default(DamageInfo);
            MoteMaker.ThrowDustPuff(p.Position, base.Map, 0.2f);
            foreach (BodyPartRecord bodyPartRecord in p.health.hediffSet.GetNotMissingParts(0, BodyPartDepth.Outside, null, null))
            {
                bool flag = wornApparel.Count > 0;
                if (flag)
                {
                    bool flag2 = false;
                    for (int i = 0; i < wornApparel.Count; i++)
                    {
                        bool flag3 = wornApparel[i].def.apparel.CoversBodyPart(bodyPartRecord);
                        if (flag3)
                        {
                            flag2 = true;
                            break;
                        }
                    }
                    bool flag4 = !flag2;
                    if (flag4)
                    {
                        list.Add(bodyPartRecord);
                    }
                }
                else
                {
                    list.Add(bodyPartRecord);
                }
            }
            for (int j = 0; j < wornApparel.Count; j++)
            {
                this.DamageEntities(wornApparel[j], num);
            }
            for (int k = 0; k < list.Count; k++)
            {
                damageInfo = new DamageInfo(DamageDefOf.Burn, (float)Mathf.RoundToInt((float)num * list[k].coverage), 0f, -1f, this, list[k], null, 0, null);
                p.TakeDamage(damageInfo);
            }
        }

        // Token: 0x0400000F RID: 15
        private List<Pawn> touchingPawns = new List<Pawn>();

        // Token: 0x04000010 RID: 16
        private List<Thing> touchingThings = new List<Thing>();

        // Token: 0x04000011 RID: 17
        private int Ticks = 100;

        // Token: 0x04000012 RID: 18
        private int TickRate = 100;

        // Token: 0x04000013 RID: 19
        private int AcidDamage = 3;
    }
}
