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
    // Token: 0x020006E5 RID: 1765
    public class TeleportSpawner : ThingWithComps, IThingHolder
    {
        public TeleportSpawner()
        {
            this.innerContainer = new ThingOwner<Thing>(this, false, LookMode.Deep);
            ResetStaticData();
        }

        // Token: 0x060024F3 RID: 9459 RVA: 0x00116CE3 File Offset: 0x001150E3
        public ThingOwner GetDirectlyHeldThings()
        {
            return this.innerContainer;
        }

        // Token: 0x060024F4 RID: 9460 RVA: 0x00116CEB File Offset: 0x001150EB
        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
        }
        
        protected ThingOwner innerContainer;

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

        // Token: 0x06002627 RID: 9767 RVA: 0x00122274 File Offset: 0x00120674
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                this.secondarySpawnTick = Find.TickManager.TicksGame + this.ResultSpawnDelay.RandomInRange.SecondsToTicks();
            }
            this.CreateSustainer();
        }

        // Token: 0x06002628 RID: 9768 RVA: 0x001222BC File Offset: 0x001206BC
        public override void Tick()
        {
            if (base.Spawned)
            {
                this.sustainer.Maintain();
                Vector3 vector = base.Position.ToVector3Shifted();
                IntVec3 c;
                if (Rand.MTBEventOccurs(TeleportSpawner.FilthSpawnMTB, 1f, 1.TicksToSeconds()) && CellFinder.TryFindRandomReachableCellNear(base.Position, base.Map, TeleportSpawner.FilthSpawnRadius, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), null, null, out c, 999999))
                {
                    FilthMaker.TryMakeFilth(c, base.Map, TeleportSpawner.filthTypes.RandomElement<ThingDef>(), 1);
                }
                if (Rand.MTBEventOccurs(TeleportSpawner.DustMoteSpawnMTB, 1f, 1.TicksToSeconds()))
                {
                    MoteMaker.ThrowDustPuffThick(new Vector3(vector.x, 0f, vector.z)
                    {
                        y = AltitudeLayer.MoteOverhead.AltitudeFor()
                    }, base.Map, Rand.Range(1.5f, 3f), new Color(1f, 1f, 1f, 2.5f));
                }
                if (this.secondarySpawnTick <= Find.TickManager.TicksGame)
                {
                    this.sustainer.End();
                    Map map = base.Map;
                    IntVec3 position = base.Position;
                    if (!this.innerContainer.NullOrEmpty())
                    {
                        //    Log.Message(string.Format("{0} to drop", innerContainer.ContentsString));
                        this.innerContainer.TryDropAll(position, map, ThingPlaceMode.Near);
                    }
                    this.Destroy(DestroyMode.Vanish);
                }
            }
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
            pos.y += 0.046875f * Rand.Range(0f, 1f);
            Color value = new Color(0.470588237f, 0.384313732f, 0.3254902f, 0.7f);
            TeleportSpawner.matPropertyBlock.SetColor(ShaderPropertyIDs.Color, value);
            Matrix4x4 matrix = Matrix4x4.TRS(pos, Quaternion.Euler(0f, initialAngle + speedMultiplier * num, 0f), Vector3.one * scale);
            Graphics.DrawMesh(MeshPool.plane10, matrix, TeleportSpawner.TunnelMaterial, 0, null, 0, TeleportSpawner.matPropertyBlock);
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

        // Token: 0x04001574 RID: 5492
        private int secondarySpawnTick;

        // Token: 0x04001575 RID: 5493
        public bool spawnHive = true;

        // Token: 0x04001576 RID: 5494
        public float insectsPoints;

        // Token: 0x04001577 RID: 5495
        public bool spawnedByInfestationThingComp;

        // Token: 0x04001578 RID: 5496
        private Sustainer sustainer;

        // Token: 0x04001579 RID: 5497
        private static MaterialPropertyBlock matPropertyBlock = new MaterialPropertyBlock();

        // Token: 0x0400157A RID: 5498
        private readonly FloatRange ResultSpawnDelay = new FloatRange(0.25f, 0.5f);

        // Token: 0x0400157B RID: 5499
        [TweakValue("Gameplay", 0f, 1f)]
        private static float DustMoteSpawnMTB = 0.2f;

        // Token: 0x0400157C RID: 5500
        [TweakValue("Gameplay", 0f, 1f)]
        private static float FilthSpawnMTB = 0.3f;

        // Token: 0x0400157D RID: 5501
        [TweakValue("Gameplay", 0f, 10f)]
        private static float FilthSpawnRadius = 3f;

        // Token: 0x0400157E RID: 5502
        private static readonly Material TunnelMaterial = MaterialPool.MatFrom("Things/Filth/Grainy/GrainyA", ShaderDatabase.Transparent);

        // Token: 0x0400157F RID: 5503
        private static List<ThingDef> filthTypes = new List<ThingDef>();
    }
}
