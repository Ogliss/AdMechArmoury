using AdeptusMechanicus;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public class WeatherEvent_WarpLightningStrike : WeatherEvent_LightningFlash
    {
        public WeatherEvent_WarpLightningStrike(Map map, ThingDef thingtoSpawn = null, float thingSpawnChance = 0f,  List<PawnKindDef> pawnKindstoSpawn = null) : base(map)
        {
            this.thingDeftoSpawn = thingtoSpawn;
            this.thingDefSpawnChance = thingSpawnChance;
            if (!pawnKindstoSpawn.NullOrEmpty())
            {
                kindDefstoSpawn = pawnKindstoSpawn;
            }
        }

        public WeatherEvent_WarpLightningStrike(Map map, IntVec3 forcedStrikeLoc, ThingDef thingtoSpawn = null, float thingSpawnChance = 0f, List<PawnKindDef> pawnKindstoSpawn = null) : base(map)
        {
            this.strikeLoc = forcedStrikeLoc;
            this.thingDeftoSpawn = thingtoSpawn;
            this.thingDefSpawnChance = thingSpawnChance;
            if (!pawnKindstoSpawn.NullOrEmpty())
            {
                kindDefstoSpawn = pawnKindstoSpawn;
            }
        }

        public override void FireEvent()
        {
            base.FireEvent();
            if (!this.strikeLoc.IsValid)
            {
                this.strikeLoc = CellFinderLoose.RandomCellWith((IntVec3 sq) => sq.Standable(this.map) && !this.map.roofGrid.Roofed(sq), this.map, 1000);
            }
            this.boltMesh = LightningBoltMeshPool.RandomBoltMesh;
            if (!this.strikeLoc.Fogged(this.map))
            {
                GenExplosion.DoExplosion(this.strikeLoc, this.map, 3f, AdeptusDamageDefOf.OG_WarpStormStrike, null, -1, -1f, null, null, null, null, null, 0f, 1, null, false, null, 0f, 1, 0f, false);
                Vector3 loc = this.strikeLoc.ToVector3Shifted();
                Rand.PushState();
                bool chance = Rand.Chance(thingDefSpawnChance);
                Rand.PopState();
                if (chance && thingDeftoSpawn!=null)
                {
                    GenSpawn.Spawn(ThingMaker.MakeThing(thingDeftoSpawn, null), strikeLoc, map);
                }
                for (int i = 0; i < 4; i++)
                {
                    FleckMaker.ThrowSmoke(loc, this.map, 1.5f);
                    FleckMaker.ThrowMicroSparks(loc, this.map);
                    FleckMaker.ThrowLightningGlow(loc, this.map, 1.5f);
                }
                if (!kindDefstoSpawn.NullOrEmpty()) TrySpawnPawn();
            }
            SoundInfo info = SoundInfo.InMap(new TargetInfo(this.strikeLoc, this.map, false), MaintenanceType.None);
            SoundDefOf.Thunder_OnMap.PlayOneShot(info);
        }

        public void TrySpawnPawn()
        {
            PawnKindDef pawnkind;
            if (kindDeftoSpawn == null)
            for (int i = 0; i < kindDefstoSpawn.Count; i++)
                {
                    pawnkind = kindDefstoSpawn[i];
                    if (pawnkind != null)
                    {
                        PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest(
                            pawnkind,
                            Find.FactionManager.FirstFactionOfDef(pawnkind.defaultFactionType),
                            PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, 0f);
                        Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
                        GenSpawn.Spawn(pawn, strikeLoc, map, 0);
                    }
                }
            else
            {
                pawnkind = kindDeftoSpawn;
                if (pawnkind != null)
                {
                    PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest(
                        pawnkind,
                        Find.FactionManager.FirstFactionOfDef(pawnkind.defaultFactionType),
                        PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, 0f);
                    Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
                    GenSpawn.Spawn(pawn, strikeLoc, map, 0);
                }
            }
        }

        public override void WeatherEventDraw()
        {
            Graphics.DrawMesh(this.boltMesh, this.strikeLoc.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather), Quaternion.identity, FadedMaterialPool.FadedVersionOf(WeatherEvent_WarpLightningStrike.WarpLightningTex, base.LightningBrightness), 0);
        }

        private PawnKindDef kindDeftoSpawn = null;
        private List<PawnKindDef> kindDefstoSpawn = new List<PawnKindDef>();
        private float kindDefSpawnChance = 0f;
        private ThingDef thingDeftoSpawn = null;
        private float thingDefSpawnChance = 0f;
        private IntVec3 strikeLoc = IntVec3.Invalid;

        private Mesh boltMesh;

        private static Material WarpLightningTex = MaterialPool.MatFrom("Weather/PinkBolt", ShaderDatabase.TransparentPostLight, -1);
    }
}
