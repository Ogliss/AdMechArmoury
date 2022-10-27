using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    class FlyerIncoming : DropPodIncoming
    {

        public override Graphic Graphic
        {
            get
            {
                return this.innerContainer.First().Graphic;
            }
        }

        private Thing thingforgfx;
        public new Thing GetThingForGraphic()
        {
            if (thingforgfx==null)
            {
                ActiveFlyer flyer = innerContainer[0] as ActiveFlyer;
                if (flyer != null)
                {
                    if (flyer.GetThingForGraphic() != null)
                    {
                        Pawn p = flyer.GetThingForGraphic() as Pawn;
                        if (p != null)
                        {
                            thingforgfx = flyer.GetThingForGraphic();
                        }
                    }

                }
            }
            return thingforgfx;
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            if (this.innerContainer.Any)
            {
                foreach (var item in innerContainer)
                {
                    ActiveFlyer flyer = item as ActiveFlyer;
                    if (flyer != null)
                    {
                        if (flyer.GetThingForGraphic() != null)
                        {
                            Pawn p = flyer.GetThingForGraphic() as Pawn;
                            if (p !=null)
                            {
                                //    p.rotationTracker.Face(base.Position.ToVector3());
                                float angle = (base.Position.ToVector3() - drawLoc).AngleFlat();
                                p.Rotation = Pawn_RotationTracker.RotFromAngleBiased(angle);
                                p.Drawer.DrawAt(drawLoc);
                                return;
                            }
                        }
                        
                    }
                }
            }
            base.DrawAt(drawLoc, flip);
        }

        // Token: 0x06004F05 RID: 20229 RVA: 0x001A99DC File Offset: 0x001A7BDC
        public override void Impact()
        {
            for (int i = 0; i < 6; i++)
            {
                AdeptusFleckMaker.ThrowDustPuff(base.Position.ToVector3Shifted() + Gen.RandomHorizontalVector(1f), base.Map, 1.2f);
            }
        //    FleckMaker.ThrowLightningGlow(base.Position.ToVector3Shifted(), base.Map, 2f);
            GenClamor.DoClamor(this, 15f, ClamorDefOf.Impact);
            if (this.def.skyfaller.CausesExplosion)
            {
                GenExplosion.DoExplosion(base.Position, base.Map, this.def.skyfaller.explosionRadius, this.def.skyfaller.explosionDamage, null, GenMath.RoundRandom((float)this.def.skyfaller.explosionDamage.defaultDamage * this.def.skyfaller.explosionDamageFactor), -1f, null, null, null, null, null, 0f, 1, GasType.BlindSmoke, false, null, 0f, 1, 0f, false, null, (!this.def.skyfaller.damageSpawnedThings) ? this.innerContainer.ToList<Thing>() : null);
            }
            this.SpawnThings();
            this.innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
            CellRect cellRect = this.OccupiedRect();
            for (int i = 0; i < cellRect.Area * this.def.skyfaller.motesPerCell; i++)
            {
                AdeptusFleckMaker.ThrowDustPuff(cellRect.RandomVector3, base.Map, 2f);
            }
            if (this.def.skyfaller.MakesShrapnel)
            {
                SkyfallerShrapnelUtility.MakeShrapnel(base.Position, base.Map, this.shrapnelDirection, this.def.skyfaller.shrapnelDistanceFactor, this.def.skyfaller.metalShrapnelCountRange.RandomInRange, this.def.skyfaller.rubbleShrapnelCountRange.RandomInRange, true);
            }
            if (this.def.skyfaller.cameraShake > 0f && base.Map == Find.CurrentMap)
            {
                Find.CameraDriver.shaker.DoShake(this.def.skyfaller.cameraShake);
            }
            if (this.def.skyfaller.impactSound != null)
            {
                this.def.skyfaller.impactSound.PlayOneShot(SoundInfo.InMap(new TargetInfo(base.Position, base.Map, false), MaintenanceType.None));
            }
            this.Destroy(DestroyMode.Vanish);
        }

    }
}
