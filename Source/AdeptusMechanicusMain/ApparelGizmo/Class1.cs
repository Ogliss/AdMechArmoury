using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x0200078C RID: 1932
    public abstract class CompUseEffectOGAG : CompWearable
    {
        // Token: 0x17000699 RID: 1689
        // (get) Token: 0x06002ABC RID: 10940 RVA: 0x00141FD2 File Offset: 0x001403D2
        public virtual float OrderPriority
        {
            get
            {
                return 0f;
            }
        }

        // Token: 0x1700069A RID: 1690
        // (get) Token: 0x06002ABD RID: 10941 RVA: 0x00141FD9 File Offset: 0x001403D9
        private CompProperties_UseEffect Props
        {
            get
            {
                return (CompProperties_UseEffect)this.props;
            }
        }

        // Token: 0x06002ABE RID: 10942 RVA: 0x00141FE8 File Offset: 0x001403E8
        public virtual void DoEffect(Pawn usedBy)
        {
            if (this.Props.doCameraShake && usedBy.Spawned && usedBy.Map == Find.CurrentMap)
            {
                Find.CameraDriver.shaker.DoShake(1f);
            }
        }

        // Token: 0x06002ABF RID: 10943 RVA: 0x00142034 File Offset: 0x00140434
        public virtual bool SelectedUseOption(Pawn p)
        {
            return false;
        }

        // Token: 0x06002AC0 RID: 10944 RVA: 0x00142037 File Offset: 0x00140437
        public virtual bool CanBeUsedBy(Pawn p, out string failReason)
        {
            failReason = null;
            return true;
        }

        // Token: 0x04001784 RID: 6020
        private const float CameraShakeMag = 1f;
    }
}
