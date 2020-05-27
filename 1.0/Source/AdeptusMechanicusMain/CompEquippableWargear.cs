using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse
{
    public class CompProperties_EquippableWargear : CompProperties
    {
        public CompProperties_EquippableWargear()
        {
            this.compClass = typeof(CompEquippableWargear);
        }
    }

    // Token: 0x02000E58 RID: 3672
    public class CompEquippableWargear : ThingComp, IVerbOwner
    {
        public CompProperties_EquippableWargear Props => (CompProperties_EquippableWargear)props;
        // Token: 0x060053D1 RID: 21457 RVA: 0x00264D3F File Offset: 0x0026313F
        public CompEquippableWargear()
        {
            this.verbTracker = new VerbTracker(this);
        }

        // Token: 0x17000DA0 RID: 3488
        // (get) Token: 0x060053D2 RID: 21458 RVA: 0x00264D53 File Offset: 0x00263153
        private Pawn Holder
        {
            get
            {
                return this.PrimaryVerb.CasterPawn;
            }
        }

        // Token: 0x17000DA1 RID: 3489
        // (get) Token: 0x060053D3 RID: 21459 RVA: 0x00264D60 File Offset: 0x00263160
        public List<Verb> AllVerbs
        {
            get
            {
                return this.verbTracker.AllVerbs;
            }
        }

        // Token: 0x17000DA2 RID: 3490
        // (get) Token: 0x060053D4 RID: 21460 RVA: 0x00264D6D File Offset: 0x0026316D
        public Verb PrimaryVerb
        {
            get
            {
                return this.verbTracker.PrimaryVerb;
            }
        }

        // Token: 0x17000DA3 RID: 3491
        // (get) Token: 0x060053D5 RID: 21461 RVA: 0x00264D7A File Offset: 0x0026317A
        public VerbTracker VerbTracker
        {
            get
            {
                return this.verbTracker;
            }
        }

        // Token: 0x17000DA4 RID: 3492
        // (get) Token: 0x060053D6 RID: 21462 RVA: 0x00264D82 File Offset: 0x00263182
        public List<VerbProperties> VerbProperties
        {
            get
            {
                return this.parent.def.Verbs;
            }
        }

        // Token: 0x17000DA5 RID: 3493
        // (get) Token: 0x060053D7 RID: 21463 RVA: 0x00264D94 File Offset: 0x00263194
        public List<Tool> Tools
        {
            get
            {
                return this.parent.def.tools;
            }
        }

        // Token: 0x17000D9E RID: 3486
        // (get) Token: 0x060053D8 RID: 21464 RVA: 0x00264DA6 File Offset: 0x002631A6
        Thing IVerbOwner.ConstantCaster
        {
            get
            {
                return null;
            }
        }

        // Token: 0x17000D9F RID: 3487
        // (get) Token: 0x060053D9 RID: 21465 RVA: 0x00264DA9 File Offset: 0x002631A9
        ImplementOwnerTypeDef IVerbOwner.ImplementOwnerTypeDef
        {
            get
            {
                return ImplementOwnerTypeDefOf.Weapon;
            }
        }

        // Token: 0x060053DA RID: 21466 RVA: 0x00264DB0 File Offset: 0x002631B0
        public IEnumerable<Command> GetVerbsCommands()
        {
            return this.verbTracker.GetVerbsCommands(KeyCode.None);
        }

        /* Token: 0x060053DB RID: 21467 RVA: 0x00264DC0 File Offset: 0x002631C0
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            if (this.Holder != null && this.Holder.equipment != null && this.Holder.equipment.Primary == this.parent)
            {
                this.Holder.equipment.Notify_PrimaryDestroyed();
            }
        }*/

        // Token: 0x060053DC RID: 21468 RVA: 0x00264E1B File Offset: 0x0026321B
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look<VerbTracker>(ref this.verbTracker, "verbTracker", new object[]
            {
                this
            });
        }

        // Token: 0x060053DD RID: 21469 RVA: 0x00264E3D File Offset: 0x0026323D
        public override void CompTick()
        {
            base.CompTick();
            this.verbTracker.VerbsTick();
        }

        // Token: 0x060053DE RID: 21470 RVA: 0x00264E50 File Offset: 0x00263250
        public void Notify_EquipmentLost()
        {
            List<Verb> allVerbs = this.AllVerbs;
            for (int i = 0; i < allVerbs.Count; i++)
            {
                allVerbs[i].Notify_EquipmentLost();
            }
        }

        // Token: 0x060053DF RID: 21471 RVA: 0x00264E87 File Offset: 0x00263287
        string IVerbOwner.UniqueVerbOwnerID()
        {
            return "CompEquippableWargear_" + this.parent.ThingID;
        }

        // Token: 0x060053E0 RID: 21472 RVA: 0x00264EA0 File Offset: 0x002632A0
        bool IVerbOwner.VerbsStillUsableBy(Pawn p)
        {
            Apparel apparel = this.parent as Apparel;
            if (apparel != null)
            {
                return p.apparel.WornApparel.Contains(apparel);
            }
            return p.equipment.AllEquipmentListForReading.Contains(this.parent);
        }

        // Token: 0x04003732 RID: 14130
        public VerbTracker verbTracker;
    }
}
