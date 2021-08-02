using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using AdeptusMechanicus.Lasers;
using RimWorld;
using AdeptusMechanicus.ExtensionMethods;
using AdeptusMechanicus;

namespace AdeptusMechanicus.Lasers
{
    [StaticConstructorOnStartup]
    public class LaserBeamGraphicCE :Thing
    {
        #region Vars
        public LaserBeamDefCE projDef;
        float beamWidth;
        float beamLength;
        float flareWidth = -1f;
        float flareWidthMod = 1f;
        float flareLength = -1f;
        float flareLengthMod = 1f;
        int ticks;
        int colorIndex = 2;
        Vector3 a;
        Vector3 b;

        public Matrix4x4 drawingMatrixBeam = default(Matrix4x4);
        public Matrix4x4 drawingMatrixFlare = default(Matrix4x4);
        Material materialBeam;
        Mesh mesh;
        Thing launcher;
        Verb verb;
        Thing hitThing;
        ThingDef equipmentDef;
        public List<Mesh> meshes = new List<Mesh>();
        public List<Material> mats = new List<Material>();
        EffecterDef effecterDef;
        Effecter effecter;
        public int ticksToDetonation;


        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref beamWidth, "beamWidth");
            Scribe_Values.Look(ref beamLength, "beamLength");
            Scribe_Values.Look(ref flareWidth, "flareWidth");
            Scribe_Values.Look(ref flareWidthMod, "flareWidthMod");
            Scribe_Values.Look(ref flareLength, "flareLength");
            Scribe_Values.Look(ref flareLengthMod, "flareLengthMod");
            Scribe_Values.Look(ref ticks, "ticks");
            Scribe_Values.Look(ref colorIndex, "colorIndex");
            Scribe_Values.Look(ref a, "a");
            Scribe_Values.Look(ref b, "b");
            Scribe_Defs.Look(ref projDef, "projectileDef");
            Scribe_Defs.Look(ref effecterDef, "effecterDef");
            Scribe_References.Look(ref launcher, "launcher");
            Scribe_References.Look(ref verb, "weapon");
            Scribe_References.Look(ref hitThing, "hitThing");
        }
        #endregion Vars
        public float Lifetime => Mathf.InverseLerp(0, projDef.lifetime, ticks);
        public float Opacity => (float)Math.Sin(Math.Pow(Lifetime, projDef.impulseCurve?.Evaluate(Lifetime) ?? projDef.impulse) * Math.PI);
        public float ArcOpacity => (float)Math.Sin(Math.Pow(1.0 - 1.0 * ticks / projDef.lifetime, projDef.impulse) * Math.PI);
        public bool Lightning => projDef.LightningBeam;
        public bool Static => projDef.StaticLightning;

        public override void Tick()
        {
            if (def == null || (ticks++ > projDef.lifetime && (effecter == null || this.effecter.ticksLeft <= 0)))
            {
                if (effecter != null)
                {
                    effecter.Cleanup();
                }
                Destroy(DestroyMode.Vanish);
            }
            if (effecter != null)
            {
                if (this.effecter.ticksLeft > 0)
                {
                    var targetInfo = hitThing ?? new TargetInfo(IntVec3.FromVector3(b), launcher.Map, false);
                    effecter.EffectTick(targetInfo, targetInfo);
                    effecter.ticksLeft--;
                }
                else
                {
                    this.effecter.Cleanup();
                }
            }
            if (this.ticksToDetonation > 0)
            {
                this.ticksToDetonation--;
                if (this.ticksToDetonation <= 0)
                {
                    this.Explode();
                }
            }
        }

        public void TriggerEffect(EffecterDef effect, Vector3 position, Thing hitThing = null)
        {
            if (effect == null) return;
            var targetInfo = hitThing ?? new TargetInfo(IntVec3.FromVector3(position), launcher.Map, false);
            effecter = effect.Spawn();
            effecter.offset = (position - targetInfo.CenterVector3);
            effecter.ticksLeft = this.projDef.effecterLifetime;
            effecter.Trigger(targetInfo, null);
            //    effecter.Cleanup();
        }

        void SetColor(Thing launcher)
        {
            IBeamColorThing gun = null;

            Pawn pawn = launcher as Pawn;
            if (pawn != null && pawn.equipment != null) gun = pawn.equipment.Primary as IBeamColorThing;
            if (gun == null) gun = launcher as IBeamColorThing;

            if (gun != null && gun.BeamColor != -1)
            {
                colorIndex = gun.BeamColor;
            }
            if (gun !=null)
            {
                this.equipmentDef = pawn.equipment.Primary.def;
            }
        }

        public void Setup(Thing launcher, Vector3 origin, Vector3 destination, Verb verb = null, Thing hitThing = null, Effecter effecter = null, EffecterDef effecterDef = null)
        {
            //SetColor(launcher);
            this.launcher = launcher;
            this.verb = verb;
            this.a = origin;
            this.b = destination;
            this.hitThing = hitThing ?? null;
            this.effecter = effecter ?? null;
            this.effecterDef = effecterDef ?? null;
            Map map = verb?.Caster?.Map ?? launcher.Map;
            Vector3 dir = (destination - origin).normalized;
            dir.y = 0;

            Vector3 a = origin;// += dir * (defWeapon == null ? 0.9f : defWeapon.barrelLength);

            if (verb != null)
            {
                if (verb.Muzzle(out float barrelLength, out float barrelOffset, out float bulletOffset, out ThingDef flareDef, out float flareSize, out ThingDef smokeDef, out float smokeSize))
                {
                    a = origin -= dir * (barrelOffset * (verb.EquipmentSource.def.graphicData.drawSize.magnitude / 4));
                //    a.y += 0.0367346928f;
                    if (flareDef != null)
                    {
                        MoteThrown m = AdeptusMoteMaker.MakeStaticMote(a, map, flareDef, flareSize) as MoteThrown;
                        if (m != null)
                        {
                            m.solidTimeOverride = projDef.lifetime;
                            AdvancedVerbPropertiesCE properties = verb.verbProps as AdvancedVerbPropertiesCE;
                            if (properties == null || properties.MuzzleFlareRotates)
                            {
                                if (properties != null)
                                {
                                    m.instanceColor = properties.MuzzleFlareColor;
                                }
                                Rand.PushState();
                                m.exactRotation = Rand.Range(0, 350);
                                Rand.PopState();
                            }
                        }
                    }
                    if (smokeDef != null)
                    {
                        AdeptusMoteMaker.ThrowSmoke(a, smokeSize, map, smokeDef);
                    }
                }
                FleckMaker.Static(a, launcher.Map, FleckDefOf.ShotFlash, verb.verbProps.muzzleFlashScale);
            }
            if (effecter == null)
            {
                TriggerEffect(effecterDef, b, hitThing);
            }
            ProjectileVFX ext = this.projDef.GetModExtensionFast<ProjectileVFX>();
            if (ext != null && destination.InBounds(launcher.Map))
            {
                Vector3 pos = destination;
                ThingDef explosionMoteDef = ext.ExplosionMoteDef ?? projDef.projectile.damageDef.explosionCellMote ?? null;
                SoundDef sound = projDef.projectile.damageDef.soundExplosion;
                Color? color = ext.useGraphicColor ? projDef.graphic.color : (ext.useGraphicColorTwo ? projDef.graphic.colorTwo : projDef.projectile.damageDef.explosionColorCenter);
                float scale = ext.scaleWithProjectile ? projDef.graphic.drawSize.magnitude : 1f;
                ext.ImpactEffects(pos, map, explosionMoteDef, ext.ExplosionMoteSize * scale, color, sound, ext.ImpactMoteDef, ext.ImpactMoteSize * scale, ext.ImpactGlowMoteDef, ext.ImpactGlowMoteSize * scale, hitThing);
                //    ext.ImpactEffects(destination, launcher.Map, ext.ExplosionMoteDef ?? this.projDef.projectile.damageDef.explosionCellMote, ext.ExplosionMoteSize, this.projDef.projectile.damageDef.explosionColorCenter, this.projDef.projectile.damageDef.soundExplosion, ext.ImpactMoteDef, ext.ImpactMoteSize, ext.ImpactGlowMoteDef, ext.ImpactGlowMoteSize, hitThing);
            }
        }

        public void SetupDrawing()
        {
            if (mesh != null) return;

            materialBeam = projDef.GetBeamMaterial(colorIndex) ?? LaserBeamGraphicCE.BeamMat;

            if (this.projDef.graphicData != null)
            {
                if (this.projDef.graphicData.graphicClass != null)
                {
                    if (this.projDef.graphicData.graphicClass == typeof(Graphic_Flicker))
                    {
                        //    Log.Message("Graphic_Flicker get mat for arc " + (i + 1));
                        materialBeam = projDef.GetBeamMaterial((this.projDef.materials.IndexOf(materialBeam) + 1 < this.projDef.materials.Count ? this.projDef.materials.IndexOf(materialBeam) + 1 : 0)) ?? projDef.graphicData.Graphic.MatSingle;
                    }
                    if (this.projDef.graphicData.graphicClass == typeof(Graphic_Random))
                    {
                        materialBeam = projDef.GetBeamMaterial(0) ?? projDef.graphicData.Graphic.MatSingle;
                    }
                }
            }
            beamWidth = projDef.beamWidth;
            flareWidth = projDef.flareWidth;
            flareWidthMod = projDef.flareWidthMod;
            flareLength = projDef.flareLength;
            flareLengthMod = projDef.flareLengthMod;
            Quaternion rotation = Quaternion.LookRotation(a - b);
            //    Quaternion rotation = Quaternion.LookRotation(b - a);
            Vector3 dir = (b - a).normalized;
            this.beamLength = (b - a).magnitude;

            Vector3 drawingScale = new Vector3(beamWidth, 1f, beamLength);
            Vector3 drawingPosition = (a + b) / 2;
            drawingMatrixBeam.SetTRS(drawingPosition, rotation, drawingScale);

            if (flareWidth == -1)
            {
                flareWidth = beamWidth * flareWidthMod;
            }
            if (flareLength == -1)
            {
                flareLength = beamLength * flareLengthMod;
            }
            Quaternion rotationFlare = Quaternion.LookRotation(a - b);
            Vector3 drawingScaleFlare = new Vector3(flareWidth, 1f, flareLength);
            Vector3 drawingPositionFlare = a + (dir * flareLength) / 2; //FlarePos(a,b, 0.1f);
            drawingMatrixFlare.SetTRS(drawingPositionFlare, rotationFlare, drawingScaleFlare);
            float textureRatio = 1.0f * materialBeam.mainTexture.width / materialBeam.mainTexture.height;
            float seamTexture = projDef.seam < 0 ? textureRatio : projDef.seam;
            float capLength = beamWidth / textureRatio / 2f * seamTexture;
            float seamGeometry = beamLength <= capLength * 2 ? 0.5f : capLength * 2 / beamLength;

            if (Lightning && meshes.Count < projDef.ArcCount * (Static ? 1 : (projDef.lifetime / projDef.LightningFrameTime)))
            {
                float distance = Vector3.Distance(a, b);
                for (int i = 0; i < projDef.ArcCount; i++)
                {
                    if (projDef.lightningArcVarianceDistCurve != null && projDef.lightningArcWidthDistCurve != null)
                    {
                        meshes.Add(LightningLaserBoltMeshMaker.NewBoltMesh(new Vector2(0, -(distance + 0.25f)), projDef.lightningArcVarianceDistCurve, projDef.lightningArcWidthDistCurve, projDef.lightningArcVarianceTimeCurve, projDef.lightningArcWidthTimeCurve, i == 0 ? 0f : 0.5f, projDef.capSize));
                    }
                    else
                    {
                        meshes.Add(LightningLaserBoltMeshMaker.NewBoltMesh(new Vector2(0, -(distance + 0.25f)), projDef.LightningVariance, beamWidth, i * 0.1f, projDef.capSize));
                    }
                    mats.Add(projDef.GetBeamMaterial(i) ?? LaserBeamGraphicCE.BeamMat);
                }
                //    this.mesh = LightningBoltMeshMakerOG.NewBoltMesh(new Vector2(0, -(distance + 0.25f)), def.LightningVariance);
            }
            else
            {
                this.mesh = MeshMakerLaser.Mesh(seamTexture, seamGeometry);
            }
        }

        public override void Draw()
        {
            SetupDrawing();

            Color color = projDef.graphicData.color;
            Color colorTwo = projDef.graphicData.colorTwo;
            color.a *= Opacity;
            colorTwo.a *= (Opacity * projDef.flareOpacityMod);
            LaserBeamGraphicCE.BeamMatPropertyBlock.SetColor(ShaderPropertyIDs.Color, color);
            LaserBeamGraphicCE.FlareMatPropertyBlock.SetColor(ShaderPropertyIDs.Color, colorTwo);
            LaserBeamGraphicCE.FlareMatPropertyBlock.SetColor(ShaderPropertyIDs.ColorTwo, color);
            if (Lightning)
            {
                Vector3 vector;
                vector.x = (float)b.x;
                vector.y = (float)b.y;
                vector.z = (float)b.z;
                float distance = Vector3.Distance(a, b);

                for (int i = 0; i < projDef.ArcCount; i++)
                {
                    if (this.projDef.graphicData != null)
                    {
                        if (this.projDef.graphicData.graphicClass != null)
                        {
                            if (!Find.TickManager.Paused && Find.TickManager.TicksGame % this.projDef.flickerFrameTime == 0)
                            {
                                if (this.projDef.graphicData.graphicClass == typeof(Graphic_Flicker))
                                {
                                    //    Log.Message("Graphic_Flicker get mat for arc " + (i + 1));
                                    mats[i] = projDef.GetBeamMaterial((this.projDef.materials.IndexOf(mats[i]) + 1 < this.projDef.materials.Count ? this.projDef.materials.IndexOf(mats[i]) + 1 : 0)) ?? projDef.graphicData.Graphic.MatSingle;
                                }
                                else if (this.projDef.graphicData.graphicClass == typeof(Graphic_Random))
                                {
                                    //    Log.Message("Graphic_Random get mat for arc " + (i + 1));
                                    mats[i] = this.projDef.materials.RandomElement() ?? projDef.graphicData.Graphic.MatSingle;
                                }
                            }
                        }
                    }
                    float mult = Mathf.InverseLerp(projDef.lifetime, 0f, ticks);
                    //    if (Find.TickManager.TicksGame % this.projDef.flickerFrameTime != 0)  Log.Message("Mult for Arc "+(i+1)+" : "+mult);
                    if (!Find.TickManager.Paused && Find.TickManager.TicksGame % this.projDef.LightningFrameTime == 0)
                    {
                        if (projDef.lightningArcVarianceDistCurve != null && projDef.lightningArcWidthDistCurve != null)
                        {
                            meshes[i] = meshes[i] != null && Static ? meshes[i] : LightningLaserBoltMeshMaker.NewBoltMesh(new Vector2(0, -(distance + 0.25f)), projDef.lightningArcVarianceDistCurve, projDef.lightningArcWidthDistCurve, projDef.lightningArcVarianceTimeCurve, projDef.lightningArcWidthTimeCurve, i == 0 ? 0f : 0.5f, projDef.capSize);
                        }
                        else
                        {
                            meshes[i] = meshes[i] != null && Static ? meshes[i] : LightningLaserBoltMeshMaker.NewBoltMesh(new Vector2(0, -(distance + 0.25f)), projDef.LightningVariance, beamWidth * mult, i == 0 ? 0f : 0.5f, projDef.capSize);
                        }
                    }
                    Graphics.DrawMesh(this.meshes[i], this.b, Quaternion.LookRotation((vector - this.a).normalized), FadedMaterialPool.FadedVersionOf(mats[i], Opacity), 0, null, 0, LaserBeamGraphicCE.BeamMatPropertyBlock, 0);
                    Graphics.DrawMesh(this.meshes[i], this.b, Quaternion.LookRotation((vector - this.a).normalized), FadedMaterialPool.FadedVersionOf(projDef.flareMat ?? FlareMat, Opacity * 0.5f), 0, null, 0, LaserBeamGraphicCE.FlareMatPropertyBlock, 0);

                }
            }
            else
            {
                if (this.projDef.graphicData != null)
                {
                    if (this.projDef.graphicData.graphicClass != null)
                    {
                        if (!Find.TickManager.Paused && Find.TickManager.TicksGame % this.projDef.flickerFrameTime == 0)
                        {
                            if (this.projDef.graphicData.graphicClass == typeof(Graphic_Flicker))
                            {
                                //    Log.Message("Graphic_Flicker get mat for beam ");
                                materialBeam = projDef.GetBeamMaterial((this.projDef.materials.IndexOf(materialBeam) + 1 < this.projDef.materials.Count ? this.projDef.materials.IndexOf(materialBeam) + 1 : 0)) ?? projDef.graphicData.Graphic.MatSingle;
                            }
                            if (this.projDef.graphicData.graphicClass == typeof(Graphic_Random))
                            {
                                //    Log.Message("Graphic_Random get mat for beam ");
                                materialBeam = projDef.GetBeamMaterial(0) ?? projDef.graphicData.Graphic.MatSingle;
                            }
                        }
                    }
                }
                Graphics.DrawMesh(mesh, drawingMatrixBeam, FadedMaterialPool.FadedVersionOf(materialBeam, Opacity), 0, null, 0, LaserBeamGraphicCE.BeamMatPropertyBlock);
                Graphics.DrawMesh(mesh, drawingMatrixFlare, projDef.flareMat ?? FlareMat, 0, null, 0, LaserBeamGraphicCE.FlareMatPropertyBlock);
                //    Graphics.DrawMesh(mesh, drawingMatrix, FadedMaterialPool.FadedVersionOf(materialBeam, opacity), 0);
            }
        }
        
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            if (def == null || projDef.decorations == null || respawningAfterLoad) return;

            foreach (var decoration in projDef.decorations)
            {
                float spacing = decoration.spacing * projDef.beamWidth;
                float initalOffset = decoration.initialOffset * projDef.beamWidth;

                Vector3 dir = (b - a).normalized;
                float angle = (b - a).AngleFlat();
                Vector3 offset = dir * spacing;
                Vector3 position = a + offset * 0.5f + dir * initalOffset;
                float length = (b - a).magnitude - spacing;

                int i = 0;
                while (length > 0)
                {
                    MoteLaserDectorationCE moteThrown = ThingMaker.MakeThing(decoration.mote, null) as MoteLaserDectorationCE;
                    if (moteThrown == null) break;

                    moteThrown.beam = this;
                    moteThrown.airTimeLeft = projDef.lifetime;
                    moteThrown.Scale = projDef.beamWidth;
                    moteThrown.exactRotation = angle;
                    moteThrown.exactPosition = position;
                    moteThrown.SetVelocity(angle, decoration.speed);
                    moteThrown.baseSpeed = decoration.speed;
                    moteThrown.speedJitter = decoration.speedJitter;
                    moteThrown.speedJitterOffset = decoration.speedJitterOffset * i;
                    GenSpawn.Spawn(moteThrown, a.ToIntVec3(), map, WipeMode.Vanish);

                    position += offset;
                    length -= spacing;
                    i++;
                }
            }
        }
        protected virtual void Explode()
        {
            Map map = base.Map;
            IntVec3 intVec = this.b.ToIntVec3();
            bool flag = this.def.projectile.explosionEffect != null;
            if (flag)
            {
                Effecter effecter = this.def.projectile.explosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(intVec, map, false), new TargetInfo(intVec, map, false));
                effecter.Cleanup();
            }
            IntVec3 center = intVec;
            Map map2 = map;
            float explosionRadius = this.def.projectile.explosionRadius;
            DamageDef damageDef = this.def.projectile.damageDef;
            Thing launcher = this.launcher;
            int damageAmount = this.def.projectile.GetDamageAmount(1f, null);
            SoundDef soundExplode = this.def.projectile.soundExplode;
            ThingDef equipmentDef = this.equipmentDef;
            ThingDef def = this.def;
            ThingDef postExplosionSpawnThingDef = this.def.projectile.postExplosionSpawnThingDef;
            float postExplosionSpawnChance = this.def.projectile.postExplosionSpawnChance;
            int postExplosionSpawnThingCount = this.def.projectile.postExplosionSpawnThingCount;
            ThingDef preExplosionSpawnThingDef = this.def.projectile.preExplosionSpawnThingDef;
            GenExplosion.DoExplosion(center, map2, explosionRadius, damageDef, launcher, damageAmount, 0f, soundExplode, equipmentDef, def, null, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, this.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, this.def.projectile.preExplosionSpawnChance, this.def.projectile.preExplosionSpawnThingCount, this.def.projectile.explosionChanceToStartFire, this.def.projectile.explosionDamageFalloff);
        }

        private static readonly Material BeamMat = MaterialPool.MatFrom("Other/OrbitalBeam", ShaderDatabase.MoteGlow, MapMaterialRenderQueues.OrbitalBeam);
        private static readonly Material FlareMat = MaterialPool.MatFrom("Other/OrbitalBeamEnd", ShaderDatabase.MoteGlow, MapMaterialRenderQueues.OrbitalBeam);
        private static readonly MaterialPropertyBlock FlareMatPropertyBlock = new MaterialPropertyBlock();
        private static readonly MaterialPropertyBlock BeamMatPropertyBlock = new MaterialPropertyBlock();
    }
}
