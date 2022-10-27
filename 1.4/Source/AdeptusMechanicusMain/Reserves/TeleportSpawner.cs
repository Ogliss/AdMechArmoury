using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace AdeptusMechanicus
{
    public class TeleportSpawnerExtension : DefModExtension
    {
        public TeleportSpawnerExtension()
        {

        }

        public EffecterDef effecter = null;
        public SoundDef soundSustainer = null;
        public bool thowSparksinDust = true;
        public bool strikespreexplode = true;
        public bool explodesprespawn = false;
        public Color? dustColor;
        public DamageDef damageDef;
        public float blastradius = 2f;


        public void ResolveReferences()
        {
            if (effecter == null)
            {
                effecter = DefDatabase<EffecterDef>.GetNamed("OG_Effecter_EMP");
            }
            if (soundSustainer == null)
            {
                soundSustainer = DefDatabase<SoundDef>.GetNamed("EmpDisabled");
            }
        }
    }
    public class TeleportSpawner : ThingWithComps, IThingHolder
    {
        public TeleportSpawnerExtension Ext => this.def.GetModExtensionFast<TeleportSpawnerExtension>() ?? null;
        public FactionDefExtension extFaction;
        public TeleportSpawner()
        {
            this.innerContainer = new ThingOwner<Thing>(this, false, LookMode.Deep);
            ResetStaticData();
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return this.innerContainer;
        }

        protected ThingOwner innerContainer;
        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
        }
        

        // Token: 0x06002627 RID: 9767 RVA: 0x00122274 File Offset: 0x00120674
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                this.secondarySpawnTick = Find.TickManager.TicksGame + this.ResultSpawnDelay.RandomInRange.SecondsToTicks();
                this.spawnTick = Find.TickManager.TicksGame;
            }
            this.CreateSustainer();
        }

        // Token: 0x06002628 RID: 9768 RVA: 0x001222BC File Offset: 0x001206BC
        public override void Tick()
        {

            if (!base.Spawned)
            {
                return;
            }
            this.sustainer.Maintain();
            Vector3 vector = base.Position.ToVector3Shifted();
            TargetInfo localTarget = new TargetInfo(this);
            Rand.PushState();
            if (Rand.MTBEventOccurs(FilthSpawnMTB, 1f, 1.TicksToSeconds()) && CellFinder.TryFindRandomReachableCellNear(base.Position, base.Map, FilthSpawnRadius, TraverseParms.For(TraverseMode.NoPassClosedDoors), null, null, out IntVec3 result) && !filthTypes.NullOrEmpty())
            {
                FilthMaker.TryMakeFilth(result, base.Map, filthTypes.RandomElement());
            }
            if (Rand.MTBEventOccurs(DustMoteSpawnMTB, 1f, 1.TicksToSeconds()))
            {
                Vector3 loc = new Vector3(vector.x, 0f, vector.z);
                loc.y = AltitudeLayer.MoteOverhead.AltitudeFor();
                FleckMaker.ThrowDustPuffThick(loc, base.Map, Rand.Range(1.5f, 3f), Ext?.dustColor ?? new Color(1f, 1f, 1f, 2.5f));
                
                if (Ext != null && Ext.thowSparksinDust)
                {
                    if (Rand.MTBEventOccurs((EMPMoteSpawnMTB * TimeRemaining), 1f, 0.25f))
                    {
                        FleckMaker.ThrowMicroSparks(loc, base.Map);
                    }
                }
            }
            if (Ext?.effecter != null)
            {
                if (Rand.MTBEventOccurs((EMPMoteSpawnMTB * TimeRemaining), 0.5f, 0.25f))
                {
                    if (this.Effecter == null && Ext.effecter != null)
                    {
                        this.Effecter = new Effecter(Ext.effecter);
                    }
                    if (Effecter != null)
                    {
                        Effecter.EffectTick(localTarget, localTarget);
                    }
                    else
                    {
                        this.Effecter.EffectTick(localTarget, localTarget);
                    }
                }
            }
            if (Rand.MTBEventOccurs(TeleportSpawner.FilthSpawnMTB, 1f, 1.TicksToSeconds()) && CellFinder.TryFindRandomReachableCellNear(base.Position, base.Map, TeleportSpawner.FilthSpawnRadius, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), null, null, out IntVec3 c, 999999))
            {
                FilthMaker.TryMakeFilth(c, base.Map, TeleportSpawner.filthTypes.RandomElement<ThingDef>(), 1);
            }
            if (Rand.MTBEventOccurs(TeleportSpawner.DustMoteSpawnMTB, 1f, 1.TicksToSeconds()))
            {
                FleckMaker.ThrowDustPuffThick(new Vector3(vector.x, 0f, vector.z)
                {
                    y = AltitudeLayer.MoteOverhead.AltitudeFor()
                }, base.Map, Rand.Range(1.5f, 3f), new Color(1f, 1f, 1f, 2.5f));
            }
            Rand.PopState();
            if (secondarySpawnTick > Find.TickManager.TicksGame)
            {
                return;
            }
            if (this.Effecter != null)
            {
                this.Effecter.Cleanup();
            }
            this.sustainer.End();
            Map map = base.Map;
            IntVec3 position = base.Position;
            if (Ext != null)
            {
                if (Ext.strikespreexplode)
                {
                    FireEvent(map, position);
                }
                if (Ext.explodesprespawn)
                {
                    GenExplosion.DoExplosion(position, map, Ext.blastradius, Ext.damageDef, null, -1, -1f, null, null, null, null, null, 0f, 1, GasType.BlindSmoke, false, null, 0f, 1, 0f, false);
                }
            }
            if (!this.innerContainer.NullOrEmpty())
            {
                //    Log.Message(string.Format("{0} to drop", innerContainer.ContentsString));
                this.innerContainer.TryDropAll(position, map, ThingPlaceMode.Near);
            }
            this.Destroy(DestroyMode.Vanish);
        }
        

        // Token: 0x06002629 RID: 9769 RVA: 0x001225E4 File Offset: 0x001209E4
        public override void Draw()
        {
            Rand.PushState();
            Rand.Seed = this.thingIDNumber;
            for (int i = 0; i < 6; i++)
            {
                this.DrawDustPart(Rand.Range(0f, 360f), Rand.Range(0.9f, 1.1f) * (float)Rand.Sign * 4f, Rand.Range(1f, 1.5f));
            }
            Rand.PopState();
        }

        // Token: 0x0600262A RID: 9770 RVA: 0x00122658 File Offset: 0x00120A58
        private void DrawDustPart(float initialAngle, float speedMultiplier, float scale)
        {
            float num = (Find.TickManager.TicksGame - this.secondarySpawnTick).TicksToSeconds();
            Vector3 pos = base.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.Filth);
            Rand.PushState();
            pos.y += 0.046875f * Rand.Range(0f, 1f);
            Rand.PopState();
            Color value = new Color(0.470588237f, 0.384313732f, 0.3254902f, 0.7f);
            matPropertyBlock.SetColor(ShaderPropertyIDs.Color, value);
            Matrix4x4 matrix = Matrix4x4.TRS(pos, Quaternion.Euler(0f, initialAngle + speedMultiplier * num, 0f), Vector3.one * scale);
            Graphics.DrawMesh(MeshPool.plane10, matrix, SpawnerStatic.BlastEMPMaterial, 0, null, 0, matPropertyBlock);
        }

        // Token: 0x0600139E RID: 5022 RVA: 0x00096118 File Offset: 0x00094518
        public void FireEvent(Map map, IntVec3 strikeLoc)
        {
            if (!strikeLoc.IsValid)
            {
                strikeLoc = CellFinderLoose.RandomCellWith((IntVec3 sq) => sq.Standable(map) && !map.roofGrid.Roofed(sq), map, 1000);
            }
            Mesh boltMesh = LightningBoltMeshPool.RandomBoltMesh;
            Material boltMat = null;
            if (!strikeLoc.Fogged(map))
            {
                Vector3 loc = strikeLoc.ToVector3Shifted();
                for (int i = 0; i < 4; i++)
                {
                    FleckMaker.ThrowSmoke(loc, map, 1.5f);
                    FleckMaker.ThrowMicroSparks(loc, map);
                    FleckMaker.ThrowLightningGlow(loc, map, 1.5f);
                }
            }
            if (extFaction != null)
            {
                Graphic g = GraphicDatabase.Get<Graphic_Single>(extFaction.TeleportBoltTexPath, ShaderTypeDefOf.Transparent.Shader);
                boltMat = g.MatSingle;
            }
            SoundInfo info = SoundInfo.InMap(new TargetInfo(strikeLoc, map, false), MaintenanceType.None);
            SoundDefOf.Thunder_OnMap.PlayOneShot(info);
            EventDraw(map, strikeLoc, boltMesh, boltMat);
        }

        // Token: 0x0600139F RID: 5023 RVA: 0x00096229 File Offset: 0x00094629
        public void EventDraw(Map map, IntVec3 strikeLoc, Mesh boltMesh, Material boltMat = null)
        {
            /*
            WeatherEvent @event = new WeatherEvent_DeepStrike_Teleport(map, strikeLoc, boltstring: (extension != null ? extension.TeleportBoltTexPath : ""));
            map.weatherManager.eventHandler.AddEvent(@event);
            */
            Graphics.DrawMesh(boltMesh, strikeLoc.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather), Quaternion.identity, FadedMaterialPool.FadedVersionOf(boltMat ?? SpawnerStatic.LightningMat, 3f), 0);
        }

        // Token: 0x0600262B RID: 9771 RVA: 0x0012271A File Offset: 0x00120B1A
        private void CreateSustainer()
        {
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                SoundDef tunnel = SoundDefOf.Tunnel;
                this.sustainer = tunnel.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
            });
        }

        public float TimeRemaining
        {
            get
            {
                return Math.Max(Mathf.InverseLerp(secondarySpawnTick, spawnTick, Find.TickManager.TicksGame), 0.0001f);
            }
        }

        // Token: 0x06002625 RID: 9765 RVA: 0x001221C0 File Offset: 0x001205C0
        public static void ResetStaticData()
        {
            TeleportSpawner.filthTypes.Clear();
            TeleportSpawner.filthTypes.Add(ThingDefOf.Filth_Dirt);
            TeleportSpawner.filthTypes.Add(ThingDefOf.Filth_Dirt);
            TeleportSpawner.filthTypes.Add(ThingDefOf.Filth_Dirt);
            TeleportSpawner.filthTypes.Add(ThingDefOf.Filth_RubbleRock);
        }

        // Token: 0x06002626 RID: 9766 RVA: 0x00122214 File Offset: 0x00120614
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.secondarySpawnTick, "secondarySpawnTick", 0, false);
            Scribe_Values.Look<bool>(ref this.spawnHive, "spawnHive", true, false);
            Scribe_Values.Look<float>(ref this.insectsPoints, "insectsPoints", 0f, false);
            Scribe_Values.Look<bool>(ref this.spawnedByInfestationThingComp, "spawnedByInfestationThingComp", false, false);
            Scribe_Deep.Look<ThingOwner>(ref this.innerContainer, "innerContainer", new object[]
            {
                this
            });
        }

        private int spawnTick;
        private int secondarySpawnTick;

        // Token: 0x04001575 RID: 5493
        public bool spawnHive = true;

        // Token: 0x04001576 RID: 5494
        public float insectsPoints;

        // Token: 0x04001577 RID: 5495
        public bool spawnedByInfestationThingComp;

        private Effecter Effecter;
        private Sustainer sustainer;

        // Token: 0x04001579 RID: 5497
        private MaterialPropertyBlock matPropertyBlock = new MaterialPropertyBlock();

        // Token: 0x0400157A RID: 5498
        private FloatRange ResultSpawnDelay = new FloatRange(0.25f, 3.5f);

        // Token: 0x0400157B RID: 5499
        [TweakValue("Gameplay", 0f, 1f)]
        private static float DustMoteSpawnMTB = 0.2f;

        [TweakValue("Gameplay", 0f, 1f)]
        private static float EMPMoteSpawnMTB = 1f;
        // Token: 0x0400157C RID: 5500
        [TweakValue("Gameplay", 0f, 1f)]
        private static float FilthSpawnMTB = 0.3f;

        // Token: 0x0400157D RID: 5501
        [TweakValue("Gameplay", 0f, 10f)]
        private static float FilthSpawnRadius = 3f;

        // Token: 0x0400157E RID: 5502
    //   private static readonly Material TunnelMaterial = MaterialPool.MatFrom("Things/Filth/Grainy/GrainyA", ShaderDatabase.Transparent);

        // Token: 0x0400157F RID: 5503
        private static List<ThingDef> filthTypes = new List<ThingDef>();
    }
    [StaticConstructorOnStartup]
    public static class SpawnerStatic
    {
        // Token: 0x04000C09 RID: 3081
        public static readonly Material LightningMat = MatLoader.LoadMat("Weather/LightningBolt", -1);
        // Token: 0x04001579 RID: 5497
        public static MaterialPropertyBlock matPropertyBlock = new MaterialPropertyBlock();
        // Token: 0x0400157E RID: 5502
        public static readonly Material TunnelMaterial = MaterialPool.MatFrom("Things/Filth/Grainy/GrainyA", ShaderDatabase.Transparent);
        public static readonly Material BlastEMPMaterial = MaterialPool.MatFrom("Things/Mote/BlastEMP", ShaderDatabase.Transparent);
    }
}
