using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using AdeptusMechanicus.Lasers;

namespace AdeptusMechanicus
{
    public class ThingDef_GunOG : ThingWithComps, IDrawnWeaponWithRotation
    {
        public override Graphic Graphic
        {
            get
            {
                return base.Graphic;
            }
        }

        new public LaserGunDef def => base.def as LaserGunDef ?? LaserGunDef.defaultObj;

        int ticksPreviously = 0;
        public float RotationOffset
        {
            get
            {
                int ticks = Find.TickManager.TicksGame;
                UpdateRotationOffset(ticks - ticksPreviously);
                ticksPreviously = ticks;

                return rotationOffset;
            }
            set
            {
                rotationOffset = value;
                rotationSpeed = 0;
            }
        }

        void UpdateRotationOffset(int ticks)
        {
            if (rotationOffset == 0) return;
            if (ticks <= 0) return;
            if (ticks > 30) ticks = 30;

            if (rotationOffset > 0)
            {
                rotationOffset -= rotationSpeed;
                if (rotationOffset < 0) rotationOffset = 0;
            }
            else if (rotationOffset < 0)
            {
                rotationOffset += rotationSpeed;
                if (rotationOffset > 0) rotationOffset = 0;
            }

            rotationSpeed += ticks * 0.01f;
        }

        private float rotationSpeed = 0;
        private float rotationOffset = 0;
    }
}
