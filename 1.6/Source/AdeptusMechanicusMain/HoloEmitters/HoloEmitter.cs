using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace HoloEmitters.Things
{
    // Token: 0x02000034 RID: 52
    public class HoloEmitter : Building, IThingHolder
    {
        // Token: 0x060000A1 RID: 161 RVA: 0x000073C7 File Offset: 0x000057C7
        public HoloEmitter()
        {
            this.innerContainer = new ThingOwner<Thing>(this, false, (Verse.LookMode)2);
        }

        // Token: 0x060000A2 RID: 162 RVA: 0x000073DE File Offset: 0x000057DE
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look<ThingOwner>(ref this.innerContainer, "innerContainer", new object[]
            {
                this
            });
        }

        // Token: 0x060000A3 RID: 163 RVA: 0x00007404 File Offset: 0x00005804
        public override void TickRare()
        {
            base.TickRare();
            if (this.innerContainer == null)
            {
                this.innerContainer = new ThingOwner<Thing>(this, false, (Verse.LookMode)2);
            }
            IEnumerable<Thing> enumerable = from t in this.innerContainer
                                            where t is Corpse
                                            select t;
            foreach (Thing thing in enumerable)
            {
                Corpse corpse = thing as Corpse;
                if (corpse != null)
                {
                    Pawn innerPawn = corpse.InnerPawn;
                    if (innerPawn == null)
                    {
                        break;
                    }
                    if (innerPawn.Dead)
                    {
                        CompOsiris.Ressurrect(innerPawn, this);
                        CompHoloEmitter comp = base.GetComp<CompHoloEmitter>();
                        if (comp.SimPawn != innerPawn)
                        {
                            comp.SimPawn = innerPawn;
                            comp.MakeValidAllowedZone();
                        }
                    }
                }
            }
        }

        // Token: 0x060000A4 RID: 164 RVA: 0x00007500 File Offset: 0x00005900
        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
        }

        // Token: 0x060000A5 RID: 165 RVA: 0x00007510 File Offset: 0x00005910
        public ThingOwner GetDirectlyHeldThings()
        {
            return this.innerContainer;
        }

        // Token: 0x060000A6 RID: 166 RVA: 0x0000752C File Offset: 0x0000592C
        public virtual bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
        {
            bool result;
            if (!(thing is Corpse) && !(thing is Pawn))
            {
                result = false;
            }
            else
            {
                bool flag;
                if (thing.holdingOwner != null)
                {
                    thing.holdingOwner.TryTransferToContainer(thing, this.innerContainer, thing.stackCount, true);
                    flag = true;
                }
                else
                {
                    flag = this.innerContainer.TryAdd(thing, true);
                }
                result = flag;
            }
            return result;
        }
        
        // Token: 0x060000A8 RID: 168 RVA: 0x000075BE File Offset: 0x000059BE
        public new IThingHolder ParentHolder()
        {
            return base.ParentHolder;
        }
        
        // Token: 0x04000061 RID: 97
        private ThingOwner innerContainer;
    }
}
