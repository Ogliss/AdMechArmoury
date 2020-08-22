using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    // Token: 0x02000020 RID: 32
    public class TrailerProjectileExtension : DefModExtension
    {
        public string trailMoteDef = "Mote_Smoke";
        public float trailMoteSize = 1f;
        public int trailerMoteInterval = 30;
    }

    public class GlowerProjectileExtension : DefModExtension
    {
        public string GlowMoteDef = string.Empty;
        public float GlowMoteSize = 1f;
        public int GlowMoteInterval = 30;
    }
    public class EffectProjectileExtension : DefModExtension
    {
        public bool explosionMote = false;
        public float explosionMoteSize = 1f;
        public FloatRange? explosionMoteSizeRange;
        public string ImpactMoteDef = string.Empty;
        public float ImpactMoteSize = 1f;
        public FloatRange? ImpactMoteSizeRange;
        public string ImpactGlowMoteDef = string.Empty;
        public float ImpactGlowMoteSize = 1f;
        public FloatRange? ImpactGlowMoteSizeRange;
        // Token: 0x060051CE RID: 20942 RVA: 0x001B90BC File Offset: 0x001B72BC
        public void ThrowMote(Vector3 loc, Map map, ThingDef explosionMoteDef, float explosionSize, Color color, SoundDef sound, ThingDef ImpactMoteDef, float ImpactMoteSize, ThingDef ImpactGlowMoteDef, float ImpactGlowMoteSize, Thing hitThing = null)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }
            float rotationRate = Rand.Range(-30f, 30f);
            float VelocityAngel = (float)Rand.Range(0, 360);
            float VelocitySpeed = Rand.Range(0.48f, 0.72f);
            if (ImpactGlowMoteDef != null)
            {
                MoteMaker.MakeStaticMote(loc, map, ImpactGlowMoteDef, ImpactGlowMoteSize);
            }
            if (explosionMoteDef!=null)
            {
                MoteThrown moteThrown;
                moteThrown = (MoteThrown)ThingMaker.MakeThing(explosionMoteDef, null);
                moteThrown.Scale = explosionSize;
                moteThrown.rotationRate = Rand.Range(-30f, 30f);
                moteThrown.exactPosition = loc;
                moteThrown.instanceColor = color;
                moteThrown.SetVelocity(VelocityAngel, VelocitySpeed);
                GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
            }
            if (ImpactMoteDef != null)
            {
                if (hitThing != null && hitThing is Pawn pawn)
                {
                    ImpactMoteDef = ThingDef.Named("AdeptusMechanicus_Mote_Blood_Puff");
                    if (sound!=null)
                    {
                        sound.PlayOneShot(new TargetInfo(loc.ToIntVec3(), map, false));
                    }
                    MoteThrown moteThrown;
                    moteThrown = (MoteThrown)ThingMaker.MakeThing(ImpactMoteDef, null);
                    moteThrown.Scale = ImpactMoteSize;
                    moteThrown.rotationRate = Rand.Range(-30f, 30f);
                    moteThrown.exactPosition = loc;
                    moteThrown.instanceColor = pawn.RaceProps.BloodDef.graphic.color;
                    moteThrown.SetVelocity(VelocityAngel, VelocitySpeed);
                    GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
                }
                else
                {
                    MoteMaker.MakeStaticMote(loc, map, ImpactMoteDef, ImpactMoteSize);
                }
            }
        }
    }
}
