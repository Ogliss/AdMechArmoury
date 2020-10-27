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

    // AdeptusMechanicus.EffecterProjectileExtension
    public class EffecterProjectileExtension : DefModExtension
    {
        public float AddHediffChance = 0.05f; //The default chance of adding a hediff.
        public HediffDef HediffToAdd = null;
        public bool CanResistHediff = false; //The default chance of adding a hediff.
        public float ResistHediffChance = 0.00f; //The default chance of adding a hediff.
        public StatDef ResistHediffStat = null; //The default chance of adding a hediff.

        public void ApplyEffect(Thing hitThing)
        {

            if (hitThing != null && hitThing is Pawn hitPawn)
            {
                if (HediffToAdd != null)
                {
                    Rand.PushState();
                    var rand = Rand.Value; // This is a random percentage between 0% and 100%
                    Rand.PopState();
                    if (CanResistHediff == true)
                    {
                        /*
                        if (ResistHediffChance!=0)
                        {
                            rand = rand + ResistHediffChance;
                        }
                        else */
                        if (ResistHediffStat != null)
                        {
                            ResistHediffChance = hitPawn.GetStatValue(ResistHediffStat, true);
                        }
                        AddHediffChance = AddHediffChance * ResistHediffChance;
                    }

                    if (rand <= AddHediffChance)
                    {
                        var effectOnPawn = hitPawn?.health?.hediffSet?.GetFirstHediffOfDef(HediffToAdd);
                        Rand.PushState();
                        var randomSeverity = Rand.Range(0.15f, 0.30f);
                        Rand.PopState();
                        if (effectOnPawn != null)
                        {
                            effectOnPawn.Severity += randomSeverity;
                        }
                        else
                        {
                            Hediff hediff = HediffMaker.MakeHediff(HediffToAdd, hitPawn, null);
                            hediff.Severity = randomSeverity;
                            hitPawn.health.AddHediff(hediff, null, null);
                        }
                    }
                    /*
                    else
                    {
                        MoteMaker.ThrowText(hitThing.PositionHeld.ToVector3(), hitThing.MapHeld, "FailureMote".Translate(Def.AddHediffChance), 12f);
                    }
                    */
                }
            }
        }
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
