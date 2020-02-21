using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
    // Token: 0x02000AAA RID: 2730
    public class CompAbilityEffect_Shoot : CompAbilityEffect_WithDest
    {
        // Token: 0x17000B65 RID: 2917
        // (get) Token: 0x06003FB8 RID: 16312 RVA: 0x001522F1 File Offset: 0x001504F1
        public new CompProperties_EffectWithDest Props
        {
            get
            {
                return (CompProperties_EffectWithDest)this.props;
            }
        }
    }
}
