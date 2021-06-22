using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    // Token: 0x0200045E RID: 1118
    [StaticConstructorOnStartup]
    public class WeatherEvent_DeepStrike_Teleport : WeatherEvent_LightningFlash
    {
        // Token: 0x0600139C RID: 5020 RVA: 0x00096060 File Offset: 0x00094460
        public WeatherEvent_DeepStrike_Teleport(Map map) : base(map)
        {
        }

        // Token: 0x0600139D RID: 5021 RVA: 0x00096074 File Offset: 0x00094474
        public WeatherEvent_DeepStrike_Teleport(Map map, IntVec3 forcedStrikeLoc, float minX = -200f, float maxX = 200f, float Z = -200f, string boltstring = "") : base(map)
        {
            this.strikeLoc = forcedStrikeLoc;
            this.minX = minX;
            this.maxX = maxX;
            this.Z = Z;
            if (boltstring!="")
            {
                this.boltstring = boltstring;
            }

        }

        public WeatherEvent_DeepStrike_Teleport(Map map, IntVec3 forcedStrikeLoc, Thing spawnThing, float minX = -200f, float maxX = 200f, float Z = -200f, string boltstring = "") : base(map)
        {
            this.strikeLoc = forcedStrikeLoc;
            this.minX = minX;
            this.maxX = maxX;
            this.Z = Z;
            if (boltstring != "")
            {
                this.boltstring = boltstring;
            }
        }

        public WeatherEvent_DeepStrike_Teleport(Map map, IntVec3 forcedStrikeLoc, List<Thing> spawnThings, float minX = -200f, float maxX = 200f, float Z = -200f, string boltstring = "") : base(map)
        {
            this.strikeLoc = forcedStrikeLoc;
            this.minX = minX;
            this.maxX = maxX;
            this.Z = Z;
            if (boltstring != "")
            {
                this.boltstring = boltstring;
            }
        }

        public WeatherEvent_DeepStrike_Teleport(Map map, IntVec3 forcedStrikeLoc, Pawn spawnPawn, float minX = -200f, float maxX = 200f, float Z = -200f, string boltstring = "") : base(map)
        {
            this.strikeLoc = forcedStrikeLoc;
            this.minX = minX;
            this.maxX = maxX;
            this.Z = Z;
            if (boltstring != "")
            {
                this.boltstring = boltstring;
            }
        }

        public WeatherEvent_DeepStrike_Teleport(Map map, IntVec3 forcedStrikeLoc, List<Pawn> spawnPawns, float minX = -200f, float maxX = 200f, float Z = -200f, string boltstring = "") : base(map)
        {
            this.strikeLoc = forcedStrikeLoc;
            this.minX = minX;
            this.maxX = maxX;
            this.Z = Z;
            if (boltstring != "")
            {
                this.boltstring = boltstring;
            }
        }

        // Token: 0x0600139E RID: 5022 RVA: 0x00096090 File Offset: 0x00094490
        public override void FireEvent()
        {
            base.FireEvent();
            if (!this.strikeLoc.IsValid)
            {
                this.strikeLoc = CellFinderLoose.RandomCellWith((IntVec3 sq) => sq.Standable(this.map) && !this.map.roofGrid.Roofed(sq), this.map, 1000);
            }
            this.boltMesh = AdeptusLightningBoltMeshMaker.NewBoltMesh(minX, maxX, Z);
            if (!this.strikeLoc.Fogged(this.map))
            {
                GenExplosion.DoExplosion(this.strikeLoc, this.map, 2.9f, DamageDefOf.Smoke, null, 0, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
                Vector3 loc = this.strikeLoc.ToVector3Shifted();
                for (int i = 0; i < 4; i++)
                {
                    AdeptusMeshBolt bolt = new AdeptusMeshBolt(strikeLoc, loc, MaterialPool.MatFrom(boltstring, ShaderDatabase.Transparent, -1));
                    bolt.CreateBolt();
                    MoteMaker.ThrowSmoke(loc, this.map, 1.5f);
                    MoteMaker.ThrowMicroSparks(loc, this.map);
                    MoteMaker.ThrowLightningGlow(loc, this.map, 1.5f);
                }
            }
            SoundInfo info = SoundInfo.InMap(new TargetInfo(this.strikeLoc, this.map, false), MaintenanceType.None);
        }

        // Token: 0x0600139F RID: 5023 RVA: 0x000961A1 File Offset: 0x000945A1
        public override void WeatherEventDraw()
        {
            Graphics.DrawMesh(this.boltMesh, this.strikeLoc.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather), Quaternion.identity, FadedMaterialPool.FadedVersionOf(MaterialPool.MatFrom(boltstring, ShaderDatabase.Transparent, -1), base.LightningBrightness), 0);
        }
        private float minX;
        private float maxX;
        private float Z;
        // Token: 0x04000C07 RID: 3079
        private IntVec3 strikeLoc = IntVec3.Invalid;
        // Token: 0x04000C08 RID: 3080
        private Mesh boltMesh;
        private string boltstring = "Weather/LightningBolt";

     //   private Thing spawnThing = null;
        private List<Thing> spawnThings = new List<Thing>();
    //    private Pawn spawnPawn = null;
        private List<Pawn> spawnPawns = new List<Pawn>();

        // Token: 0x04000C09 RID: 3081
        private static Material WarpLightningTex = MaterialPool.MatFrom("Weather/PinkBolt", ShaderDatabase.Transparent, -1);
        private static Material EldarLightningTex = MaterialPool.MatFrom("Weather/BlueBolt", ShaderDatabase.Transparent, -1);
        private static Material NecronLightningTex = MaterialPool.MatFrom("Weather/GreenBolt", ShaderDatabase.Transparent, -1);
        private static Material RedLightningTex = MaterialPool.MatFrom("Weather/RedBolt", ShaderDatabase.Transparent, -1);
        private static Material ModdedLightningMat = MatLoader.LoadMat("Weather/LightningBolt", -1);
        private static Material LightningMat = MatLoader.LoadMat("Weather/LightningBolt", -1);
    }
}
