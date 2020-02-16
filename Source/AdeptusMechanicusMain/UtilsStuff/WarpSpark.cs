using System;
using Verse;

namespace RimWorld
{
    // Token: 0x02000702 RID: 1794
    public class WarpSpark : Projectile
    {
        // Token: 0x0600272B RID: 10027 RVA: 0x0012A5A4 File Offset: 0x001289A4
        protected override void Impact(Thing hitThing)
        {
            Map map = base.Map;
            base.Impact(hitThing);
            WarpfireUtility.TryStartWarpfireIn(base.Position, map, 0.1f);
        }
    }
}
