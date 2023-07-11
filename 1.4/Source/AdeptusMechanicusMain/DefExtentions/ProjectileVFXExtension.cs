using CombatExtended;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.ProjectileVFX
    [StaticConstructorOnStartup]
    public class ProjectileVFX : DefModExtension, IMuzzlePosition
    {
        public bool scaleWithProjectile = true;
        public bool useGraphicColor = false;
        public bool useGraphicColorTwo = false;

        public string explosionMoteDef;
        private ThingDef _explosionMoteDef;
        public float explosionMoteSize = 1f;
        public FloatRange? explosionMoteSizeRange;

        public string impactMoteDef;
        public float impactMoteSize = 1f;
        private FleckDef _impactMoteDef;
        public FloatRange? impactMoteSizeRange;

        public string impactGlowMoteDef;
        public float impactGlowMoteSize = 1f;
        private FleckDef _impactGlowMoteDef;
        public FloatRange? impactGlowMoteSizeRange;

        public string muzzleFlareDef;
        public float muzzleFlareSize = -1f;
        private FleckDef _muzzleFlareDef;
        public FloatRange? muzzleFlareSizeRange;

        public string muzzleSmokeDef;
        public float muzzleSmokeSize = -0.35f;
        private FleckDef _muzzleSmokeDef;
        public FloatRange? muzzleSmokeSizeRange;


        public float barrelOffset = 0f;
        public float bulletOffset = 0.2f;
        public float barrelLength = 0f;

        public float BarrelOffset => barrelOffset;
        public float BarrelLength => barrelLength;
        public float BulletOffset => bulletOffset;

        public float ExplosionMoteSize => explosionMoteSizeRange?.RandomInRange ?? explosionMoteSize;
        public float ImpactMoteSize => impactMoteSizeRange?.RandomInRange ?? impactMoteSize;
        public float ImpactGlowMoteSize => impactGlowMoteSizeRange?.RandomInRange ?? impactGlowMoteSize;
        public float MuzzleSmokeSize => muzzleSmokeSizeRange?.RandomInRange ?? muzzleSmokeSize;
        public float MuzzleFlareSize => muzzleFlareSizeRange?.RandomInRange ?? muzzleFlareSize;

        public ThingDef ExplosionMoteDef => _explosionMoteDef ??= !explosionMoteDef.NullOrEmpty() ? (DefDatabase<ThingDef>.GetNamed(explosionMoteDef, false)) : null;
        public FleckDef ImpactMoteDef => _impactMoteDef ??= !impactMoteDef.NullOrEmpty() ? DefDatabase<FleckDef>.GetNamed(impactMoteDef, false) ?? DefDatabase<FleckDef>.GetNamed(impactMoteDef.Replace("Mote", "Fleck"), false) : null;
        public FleckDef ImpactGlowMoteDef => _impactGlowMoteDef ??= !impactGlowMoteDef.NullOrEmpty() ? DefDatabase<FleckDef>.GetNamed(impactGlowMoteDef, false) ?? DefDatabase<FleckDef>.GetNamed(impactGlowMoteDef.Replace("Mote", "Fleck"), false) : null;
        public FleckDef MuzzleFlareDef => _muzzleFlareDef ??= !muzzleFlareDef.NullOrEmpty() ? DefDatabase<FleckDef>.GetNamed(muzzleFlareDef, false) ?? DefDatabase<FleckDef>.GetNamed(muzzleFlareDef.Replace("Mote", "Fleck"), false) : null;
        public FleckDef MuzzleSmokeDef => _muzzleSmokeDef ??= !muzzleSmokeDef.NullOrEmpty() ? DefDatabase<FleckDef>.GetNamed(muzzleSmokeDef, false) ?? DefDatabase<FleckDef>.GetNamed(muzzleSmokeDef.Replace("Mote", "Fleck"), false) : null;

        private static bool enabled_CombatExtended = ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == "CETeam.CombatExtended");

        public void Impact(Thing thing, Thing hitThing = null)
        {
            Map map = thing.Map;
            Vector3 pos = thing.DrawPos;
            if (!pos.ShouldSpawnMotesAt(map))
            {
                return;
            }
            if (enabled_CombatExtended)
            {
                if (ImpactCE(thing, map, hitThing))
                {
                    return;
                }
            }
            else
            {
                if (Impact(thing, map, hitThing))
                {
                    return;
                }
            }
        }
        private bool Impact(Thing thing, Map map, Thing hitThing = null)
        {
            Projectile projectile = thing as Projectile;
            if (projectile != null)
            {
                Vector3 pos = projectile.ExactPosition;
                ThingDef explosionMoteDef = ExplosionMoteDef ?? projectile.def.projectile.damageDef.explosionCellMote ?? null;
                SoundDef sound = projectile.def.projectile.damageDef.soundExplosion;
            //    Color? color = projectile.def.projectile.damageDef.explosionColorCenter;
                Color? color = useGraphicColor ? projectile.def.graphic.color : (useGraphicColorTwo ? projectile.def.graphic.colorTwo : projectile.def.projectile.damageDef.explosionColorCenter);
                float scale = scaleWithProjectile ? projectile.def.graphic.drawSize.magnitude : 1f;
                ImpactEffects(pos, map, explosionMoteDef, ExplosionMoteSize * scale, color, sound, ImpactMoteDef, ImpactMoteSize * scale, ImpactGlowMoteDef, ImpactGlowMoteSize * scale, hitThing, projectile);
                return true;
            }
            return false;
        }
        private bool ImpactCE(Thing thing, Map map, Thing hitThing = null)
        {
            CombatExtended.ProjectileCE projectile = thing as CombatExtended.ProjectileCE;
            if (projectile != null)
            {
                Vector3 pos = projectile.ExactPosition;
                ThingDef explosionMoteDef = ExplosionMoteDef ?? projectile.def.projectile.damageDef.explosionCellMote ?? null;
                SoundDef sound = projectile.def.projectile.damageDef.soundExplosion;
                Color? color = projectile.def.projectile.damageDef.explosionColorCenter;
                float scale = scaleWithProjectile ? projectile.Graphic.drawSize.magnitude : 1f;
                ImpactEffects(pos, map, explosionMoteDef, ExplosionMoteSize * scale, color, sound, ImpactMoteDef, ImpactMoteSize * scale, ImpactGlowMoteDef, ImpactGlowMoteSize * scale, hitThing);
                return true;
            }
            return false;
        }

        public void ImpactEffects(Vector3 pos, Map map, ThingDef explosionMoteDef, float ExplosionMoteSize, Color? color, SoundDef sound, FleckDef ImpactMoteDef, float ImpactMoteSize, FleckDef ImpactGlowMoteDef, float ImpactGlowMoteSize, Thing hitThing = null, Thing projectile = null, int OverrideSolidTime = -1)
        {
            FleckDef impactMoteDef = ImpactMoteDef ?? this.ImpactMoteDef;
            Rand.PushState();
            float rotationRate = Rand.Range(-30f, 30f);
            float VelocityAngel = (float)Rand.Range(0, 360);
            float VelocitySpeed = Rand.Range(0.48f, 0.72f);
            Rand.PopState();
            if (ImpactGlowMoteDef != null)
            {
                try
                {
                    AdeptusFleckMaker.ThrowGlow(pos, map, ImpactGlowMoteSize, ImpactGlowMoteDef);
                    /*
                    mote.instanceColor.r = color.Value.r * 0.25f;
                    mote.instanceColor.g = color.Value.g * 0.25f;
                    mote.instanceColor.b = color.Value.b * 0.25f;
                    */
                }
                catch (System.Exception)
                {
               //     Log.Message("Waaah "+ projectile + " ImpactGlowMoteDef broke Vs " + hitThing);
                }
            }
            if (explosionMoteDef != null)
            {
                try
                {
                    Vector3 loc = pos;
                    loc.y = explosionMoteDef.Altitude;
                    MoteThrown moteThrown;
                    moteThrown = (MoteThrown)ThingMaker.MakeThing(explosionMoteDef, null);
                    moteThrown.Scale = ExplosionMoteSize;
                    moteThrown.rotationRate = rotationRate;
                    moteThrown.exactPosition = loc;
                    moteThrown.instanceColor = color.Value;
                    moteThrown.SetVelocity(VelocityAngel, VelocitySpeed);
                    if (OverrideSolidTime > -1)
                    {
                        moteThrown.solidTimeOverride = OverrideSolidTime;
                    }
                    GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
                }
                catch (System.Exception)
                {
               //     Log.Message("Waaah " + projectile + " explosionMoteDef broke Vs " + hitThing);
                }
            }
            if (ImpactMoteDef != null)
            {
                try
                {
                    Vector3 loc = pos;
                    if (hitThing != null && hitThing is Pawn pawn)
                    {
                        impactMoteDef = DefDatabase<FleckDef>.GetNamedSilentFail("AdeptusMechanicus_Mote_Blood_Puff");
                        if (sound != null)
                        {
                            sound.PlayOneShot(new TargetInfo(loc.ToIntVec3(), map, false));
                        }

                        AdeptusFleckMaker.Thrown(loc, map, impactMoteDef, ImpactMoteSize, pawn.RaceProps.BloodDef?.graphic.color, null, rotationRate, OverrideSolidTime > -1 ? (float?)OverrideSolidTime : null, VelocityAngel, VelocitySpeed);
                    }
                    else
                    {
                        Color c = Color.white;
                        if (color.HasValue)
                        {
                            c.r = color.Value.r * 0.25f;
                            c.g = color.Value.g * 0.25f;
                            c.b = color.Value.b * 0.25f;
                        }
                        AdeptusFleckMaker.Thrown(loc, map, impactMoteDef, ImpactGlowMoteSize, c, null, rotationRate, OverrideSolidTime > -1 ? (float?)OverrideSolidTime : null, VelocityAngel, VelocitySpeed);
                    }
                }
                catch (System.Exception)
                {
               //     Log.Message("Waaah " + projectile + " ImpactMoteDef broke Vs "+hitThing);
                }
            }
        }
    }

}
