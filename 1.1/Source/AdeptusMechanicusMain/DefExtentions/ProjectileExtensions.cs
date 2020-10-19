using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.ScattershotProjectileExtension
    public class ScattershotProjectileExtension : DefModExtension
    {
        public int projectileCount = 0;
    }

    // AdeptusMechanicus.BarrelOffsetExtension
    public class BarrelOffsetExtension : DefModExtension
    {
        public float barrellength = 1f;
        public bool muzzleFlare = true;
        public string muzzleFlareDef = string.Empty;
        public float muzzleFlareSize = 1f;
        public bool muzzleSmoke = true;
        public string muzzleSmokeDef = string.Empty;
        public float muzzleSmokeSize = 0.35f;
    }
    // AdeptusMechanicus.GlowerProjectileExtension
    public class GlowerProjectileExtension : DefModExtension
    {
        public string GlowMoteDef = string.Empty;
        public float GlowMoteSize = 1f;
        public int GlowMoteInterval = 30;
    }

    // AdeptusMechanicus.TrailerProjectileExtension
    public class TrailerProjectileExtension : DefModExtension
    {
        public string trailMoteDef = "Mote_Smoke";
        public float trailMoteSize = 0.5f;
        public int trailerMoteInterval = 30;
        public int motesThrown = 1;
    }

    // AdeptusMechanicus.EffectProjectileExtension
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
        public bool muzzleFlare = false;
        public string muzzleFlareDef = string.Empty;
        public float muzzleFlareSize = 1f;
        public bool muzzleSmoke = false;
        public string muzzleSmokeDef = string.Empty;
        public float muzzleSmokeSize = 0.35f;
        // Token: 0x060051CE RID: 20942 RVA: 0x001B90BC File Offset: 0x001B72BC
        public void ThrowMote(Vector3 loc, Map map, ThingDef explosionMoteDef, float explosionSize, Color color, SoundDef sound, ThingDef ImpactMoteDef, float ImpactMoteSize, ThingDef ImpactGlowMoteDef, float ImpactGlowMoteSize, Thing hitThing = null)
        {
            if (!loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
            {
                return;
            }
            Rand.PushState();
            float rotationRate = Rand.Range(-30f, 30f);
            float VelocityAngel = (float)Rand.Range(0, 360);
            float VelocitySpeed = Rand.Range(0.48f, 0.72f);
            Rand.PopState();
            if (ImpactGlowMoteDef != null)
            {
                MoteMaker.MakeStaticMote(loc, map, ImpactGlowMoteDef, ImpactGlowMoteSize);
            }
            if (explosionMoteDef!=null)
            {
                MoteThrown moteThrown;
                moteThrown = (MoteThrown)ThingMaker.MakeThing(explosionMoteDef, null);
                moteThrown.Scale = explosionSize;
                Rand.PushState();
                moteThrown.rotationRate = Rand.Range(-30f, 30f);
                Rand.PopState();
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
                    Rand.PushState();
                    moteThrown.rotationRate = Rand.Range(-30f, 30f);
                    Rand.PopState();
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
