using Verse;
using AdeptusMechanicus.settings;
using UnityEngine;
using System.Collections.Generic;

namespace AdeptusMechanicus
{
    public class CompProperties_ProjectileTrail : CompProperties
    {
        public CompProperties_ProjectileTrail()
        {
            this.compClass = typeof(CompProjectileTrail);
        }

        public bool trailWhenLanded = false;
        public bool useGraphicColor = false;
        public bool useGraphicColorTwo = false;
        public string trailMoteDef = "Mote_Smoke";
        public float trailMoteSize = 0.5f;
        public int trailerMoteInterval = 30;
        public int trailInitalDelay = -1;
        public int motesThrown = 1;

        public FleckDef TrailMoteDef;

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            TrailMoteDef = DefDatabase<FleckDef>.GetNamed(trailMoteDef);
            return base.ConfigErrors(parentDef);
        }
    }

    public class CompProjectileTrail : ThingComp
    {
        public CompProperties_ProjectileTrail Props => this.props as CompProperties_ProjectileTrail;
        public Projectile __instance => this.parent as Projectile;
        public int ___ticksToImpact => __instance.ticksToImpact;
        public Vector3 ___origin => __instance.origin;
        public Vector3 ___destination => __instance.destination;
        public override void CompTick()
        {
            if (___ticksToImpact % Props.trailerMoteInterval == 0)
            {
                for (int ii = 0; ii < Props.motesThrown; ii++)
                {
                    Color? DC = null;
                    if (Props.useGraphicColor)
                    {
                        DC = __instance.DrawColor;
                    }
                    else
                    if (Props.useGraphicColorTwo)
                    {
                        DC = __instance.DrawColorTwo;
                    }
                //    if (AMAMod.Dev) Log.Message($"TrailThrower.ThrowSprayTrail for: {__instance} {___ticksToImpact} {___origin} {___destination} Mote= {Props.TrailMoteDef}");
                    TrailThrower.ThrowSprayTrail(__instance.DrawPos, __instance.Map, ___origin, ___destination, Props.TrailMoteDef, Props.trailMoteSize, 240, __instance.def.projectile.SpeedTilesPerTick, DC);
                }
            }
            base.CompTick();
        }
    }

}