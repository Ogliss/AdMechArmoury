using AdeptusMechanicus;
using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public class WeatherEvent_WarpLightningStrike : WeatherEvent_LightningFlash
    {
        public WeatherEvent_WarpLightningStrike(Map map) : base(map)
        {
        }

        public WeatherEvent_WarpLightningStrike(Map map, ThingDef thingtoSpawn, float thingSpawnChance) : base(map)
        {
        }

        public WeatherEvent_WarpLightningStrike(Map map, IntVec3 forcedStrikeLoc) : base(map)
        {
            this.strikeLoc = forcedStrikeLoc;
        }

        public WeatherEvent_WarpLightningStrike(Map map, IntVec3 forcedStrikeLoc, ThingDef thingtoSpawn, float thingSpawnChance) : base(map)
        {
            this.strikeLoc = forcedStrikeLoc;
        }

        public WeatherEvent_WarpLightningStrike(Map map, IntVec3 forcedStrikeLoc, float spawnPoints, string spawnFilter ) : base(map)
        {
            this.strikeLoc = forcedStrikeLoc;
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
                GenExplosion.DoExplosion(this.strikeLoc, this.map, 3f, AdeptusDamageDefOf.OG_WarpStormStrike, null, -1, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
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
                    Rand.PushState();
                    chance = Rand.Chance(kindDefSpawnChance);
                    Rand.PopState();
                    if (chance && kindDeftoSpawn != null)
                    {
                        PawnKindDef pawnkind = kindDeftoSpawn != null ? kindDeftoSpawn : null ;
                        PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest(pawnkind, Find.FactionManager.FirstFactionOfDef(pawnkind.defaultFactionType), PawnGenerationContext.NonPlayer, -1, true, false, true, false, true, true, 20f);
                        Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
                        GenSpawn.Spawn(pawn, strikeLoc, map, 0);
                    }
                    FleckMaker.ThrowSmoke(loc, this.map, 1.5f);
                    FleckMaker.ThrowMicroSparks(loc, this.map);
                    FleckMaker.ThrowLightningGlow(loc, this.map, 1.5f);
                }
            }
            SoundInfo info = SoundInfo.InMap(new TargetInfo(this.strikeLoc, this.map, false), MaintenanceType.None);
            SoundDefOf.Thunder_OnMap.PlayOneShot(info);
        }

        public void TrySpawnPawn()
        {

        }

        public override void WeatherEventDraw()
        {
            Graphics.DrawMesh(this.boltMesh, this.strikeLoc.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather), Quaternion.identity, FadedMaterialPool.FadedVersionOf(WeatherEvent_WarpLightningStrike.WarpLightningTex, base.LightningBrightness), 0);
        }

        private PawnKindDef kindDeftoSpawn = null;
        private float kindDefSpawnChance = 0f;
        private ThingDef thingDeftoSpawn = null;
        private float thingDefSpawnChance = 0f;

        private IntVec3 strikeLoc = IntVec3.Invalid;

        private Mesh boltMesh;

        private static Material WarpLightningTex = MaterialPool.MatFrom("Weather/PinkBolt", ShaderDatabase.TransparentPostLight, -1);
    }
}
