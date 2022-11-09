using AdeptusMechanicus;
using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public class WeatherEvent_WarpLightningStrike : WeatherEvent_LightningFlash
    {
        public WeatherEvent_WarpLightningStrike(Map map, ThingDef thingtoSpawn = null, float thingSpawnChance = 0f, float spawnPoints = 0f, string spawnFilter = null, WarpStormType stormType = WarpStormType.Natural) : base(map)
        {
            this.thingDeftoSpawn = thingtoSpawn;
            this.thingDefSpawnChance = thingSpawnChance;
            this.spawnPoints = spawnPoints;
        }

        public WeatherEvent_WarpLightningStrike(Map map, IntVec3 forcedStrikeLoc, ThingDef thingtoSpawn = null, float thingSpawnChance = 0f, float spawnPoints = 0f, string spawnFilter = null, WarpStormType stormType = WarpStormType.Natural) : base(map)
        {
            this.strikeLoc = forcedStrikeLoc;
            this.thingDeftoSpawn = thingtoSpawn;
            this.thingDefSpawnChance = thingSpawnChance;
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
                GenExplosion.DoExplosion(this.strikeLoc, this.map, 3f, AdeptusDamageDefOf.OG_WarpStormStrike, null, -1, -1f, null, null, null, null, null, 0f, 1, GasType.BlindSmoke, false, null, 0f, 1, 0f, false);
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
                    if (spawnPoints>0f)TrySpawnPawn();
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
            Rand.PushState();
            bool chance = Rand.Chance(kindDefSpawnChance);
            Rand.PopState();
            if (chance)
            {
                PawnKindDef pawnkind = kindDeftoSpawn != null ? kindDeftoSpawn : DefDatabase<PawnKindDef>.AllDefs.Where(x=> x.race.thingClass == typeof(WarpBeing) && x.combatPower <= spawnPoints).RandomElement();
                if (pawnkind != null)
                {
                    PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest(
                        pawnkind,
                        Find.FactionManager.FirstFactionOfDef(pawnkind.defaultFactionType),
                        PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, 0f);
                    Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
                    GenSpawn.Spawn(pawn, strikeLoc, map, 0);
                    this.spawnPoints -= pawnkind.combatPower;
                }
            }
        }

        public override void WeatherEventDraw()
        {
            Graphics.DrawMesh(this.boltMesh, this.strikeLoc.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather), Quaternion.identity, FadedMaterialPool.FadedVersionOf(WeatherEvent_WarpLightningStrike.WarpLightningTex, base.LightningBrightness), 0);
        }

        private PawnKindDef kindDeftoSpawn = null;
        private float kindDefSpawnChance = 0f;
        private ThingDef thingDeftoSpawn = null;
        private float thingDefSpawnChance = 0f;
        private float spawnPoints = 0f;

        private IntVec3 strikeLoc = IntVec3.Invalid;
        private WarpStormType strikeType = WarpStormType.Natural;

        private Mesh boltMesh;

        private static Material WarpLightningTex = MaterialPool.MatFrom("Weather/PinkBolt", ShaderDatabase.TransparentPostLight, -1);
    }
}
