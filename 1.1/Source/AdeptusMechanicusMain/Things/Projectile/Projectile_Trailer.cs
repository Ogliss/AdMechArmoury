using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdeptusMechanicus
{
    // Token: 0x02000003 RID: 3 
    public class Projectile_Trailer : Bullet
    {
        // Token: 0x06000003 RID: 3 RVA: 0x000020FC File Offset: 0x000010FC
        public TrailerProjectileExtension Trailer =>this.def.GetModExtensionFast<TrailerProjectileExtension>() ?? null;

        public virtual string trailDef
        {
            get
            {
                return Trailer?.trailMoteDef ?? "Mote_Smoke";
            }
        }

        public virtual float trailDrawSize
        {
            get
            {
                return Trailer?.trailMoteSize ?? 1f;
            }
        }

        public virtual int trailInterval
        {
            get
            {
                return Trailer?.trailerMoteInterval ?? 6;
            }
        }

        public override void Tick()
        {
            base.Tick();
            checked
            {
                this.TicksforAppearence--;
                bool flag = this.TicksforAppearence == 0 && base.Map != null;
                if (flag)
                {
                    TrailThrower.ThrowSmokeTrail(base.Position.ToVector3Shifted(), 0.7f, base.Map, this.trailDef, this);
                    this.TicksforAppearence = 6;
                }
            }
        }
        // Token: 0x04000001 RID: 1
        private int TicksforAppearence = 3;
    }
}
