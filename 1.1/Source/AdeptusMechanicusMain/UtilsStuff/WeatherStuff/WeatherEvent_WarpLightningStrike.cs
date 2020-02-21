using AdeptusMechanicus;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
    // Token: 0x0200045E RID: 1118
    [StaticConstructorOnStartup]
    public class WeatherEvent_WarpLightningStrike : WeatherEvent_LightningFlash
    {
        // Token: 0x0600139C RID: 5020 RVA: 0x000960E8 File Offset: 0x000944E8
        public WeatherEvent_WarpLightningStrike(Map map) : base(map)
        {
        }

        // Token: 0x0600139C RID: 5020 RVA: 0x000960E8 File Offset: 0x000944E8
        public WeatherEvent_WarpLightningStrike(Map map, ThingDef thingtoSpawn, float thingSpawnChance) : base(map)
        {
        }

        // Token: 0x0600139D RID: 5021 RVA: 0x000960FC File Offset: 0x000944FC
        public WeatherEvent_WarpLightningStrike(Map map, IntVec3 forcedStrikeLoc) : base(map)
        {
            this.strikeLoc = forcedStrikeLoc;
        }

        // Token: 0x0600139D RID: 5021 RVA: 0x000960FC File Offset: 0x000944FC
        public WeatherEvent_WarpLightningStrike(Map map, IntVec3 forcedStrikeLoc, ThingDef thingtoSpawn, float thingSpawnChance) : base(map)
        {
            this.strikeLoc = forcedStrikeLoc;
        }

        // Token: 0x0600139E RID: 5022 RVA: 0x00096118 File Offset: 0x00094518
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
                GenExplosion.DoExplosion(this.strikeLoc, this.map, 3f, OGDamageDefOf.OG_WarpStormStrike, null, -1, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
                Vector3 loc = this.strikeLoc.ToVector3Shifted();
                bool chance = Rand.Chance(thingDefSpawnChance);
                if (chance && thingDeftoSpawn!=null)
                {
                    GenSpawn.Spawn(ThingMaker.MakeThing(thingDeftoSpawn, null), strikeLoc, map);
                }
                for (int i = 0; i < 4; i++)
                {
                    chance = Rand.Chance(kindDefSpawnChance);
                    if (chance && kindDeftoSpawn != null)
                    {
                        PawnKindDef pawnkind = kindDeftoSpawn != null ? kindDeftoSpawn : null ;
                        PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest(pawnkind, Find.FactionManager.FirstFactionOfDef(pawnkind.defaultFactionType), PawnGenerationContext.NonPlayer, -1, true, false, true, false, true, true, 20f);
                        Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
                        GenSpawn.Spawn(pawn, strikeLoc, map, 0);
                    }
                    MoteMaker.ThrowSmoke(loc, this.map, 1.5f);
                    MoteMaker.ThrowMicroSparks(loc, this.map);
                    MoteMaker.ThrowLightningGlow(loc, this.map, 1.5f);
                }
            }
            SoundInfo info = SoundInfo.InMap(new TargetInfo(this.strikeLoc, this.map, false), MaintenanceType.None);
            SoundDefOf.Thunder_OnMap.PlayOneShot(info);
        }

        public void TrySpawnPawn()
        {

        }

        // Token: 0x0600139F RID: 5023 RVA: 0x00096229 File Offset: 0x00094629
        public override void WeatherEventDraw()
        {
            Graphics.DrawMesh(this.boltMesh, this.strikeLoc.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather), Quaternion.identity, FadedMaterialPool.FadedVersionOf(WeatherEvent_WarpLightningStrike.WarpLightningTex, base.LightningBrightness), 0);
        }

        private PawnKindDef kindDeftoSpawn = null;
        private float kindDefSpawnChance = 0f;
        private ThingDef thingDeftoSpawn = null;
        private float thingDefSpawnChance = 0f;

        // Token: 0x04000C07 RID: 3079
        private IntVec3 strikeLoc = IntVec3.Invalid;

        // Token: 0x04000C08 RID: 3080
        private Mesh boltMesh;

        // Token: 0x04000C09 RID: 3081
        private static Material WarpLightningTex = MaterialPool.MatFrom("Weather/PinkBolt", ShaderDatabase.TransparentPostLight, -1);
    }
}
