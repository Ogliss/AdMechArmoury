using CombatExtended;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.Bullet_Explosive
    public class Bullet_ExplosiveCE : BulletCE
    {
        // Token: 0x060052C6 RID: 21190 RVA: 0x0026286F File Offset: 0x00260C6F
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticksToDetonation, "ticksToDetonation", 0, false);
        }

        // Token: 0x060052C7 RID: 21191 RVA: 0x00262889 File Offset: 0x00260C89
        public override void Tick()
        {
            base.Tick();
            if (this.ticksToDetonation > 0)
            {
                this.ticksToDetonation--;
                if (this.ticksToDetonation <= 0)
                {
                    base.Impact(null);
                }
            }
        }

        // Token: 0x060052C8 RID: 21192 RVA: 0x002628C0 File Offset: 0x00260CC0
        protected override void Impact(Thing hitThing)
        {
            bool flag = hitThing is Pawn;
            if (flag)
            {
                Vector3 drawPos = hitThing.DrawPos;
                drawPos.y = this.ExactPosition.y;
                this.ExactPosition = drawPos;
                base.Position = this.ExactPosition.ToIntVec3();
            }
            bool flag2 = this.def.projectile.explosionDelay == 0;
            if (flag2)
            {
                base.Impact(null);
            }
            else
            {
                this.landed = true;
                this.ticksToDetonation = this.def.projectile.explosionDelay;
                DamageDef damageDef = this.def.projectile.damageDef;
                Thing launcher = this.launcher;
                GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, damageDef, (launcher != null) ? launcher.Faction : null);
            }
        }

        // Token: 0x04003719 RID: 14105
        private int ticksToDetonation;
    }
}
