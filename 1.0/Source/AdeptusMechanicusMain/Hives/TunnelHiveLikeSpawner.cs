﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace AdeptusMechanicus
{
    public class ThingDef_TunnelHiveLikeSpawner : ThingDef
    {
        public FactionDef Faction;
        public ThingDef HiveDef;
        public List<PawnKindDef> PawnKinds = new List<PawnKindDef>();
        public bool strikespreexplode = false;
        public bool explodesprespawn = false;
        public DamageDef damageDef;
        public float blastradius = 2f;
    }
    // Token: 0x020006E5 RID: 1765
    public class TunnelHiveLikeSpawner : ThingWithComps
    {
        public ThingDef_TunnelHiveLikeSpawner Def
        {
            get
            {
                return this.def as ThingDef_TunnelHiveLikeSpawner;
            }
        }


        public Faction faction
        {
            get
            {
                Faction faction;
                if (Def.Faction != null)
                {
                    faction = Find.FactionManager.FirstFactionOfDef(Def.Faction);
                }
                else if (hiveDef != null)
                {
                    ThingDef_HiveLike def_HiveLike = (ThingDef_HiveLike)hiveDef;
                    faction = Find.FactionManager.FirstFactionOfDef(def_HiveLike.Faction);
                }
                else
                {
                    faction = Find.FactionManager.FirstFactionOfDef(FactionDefOf.Insect);
                }
                return faction;
            }
        }


        public ThingDef_HiveLike hiveDef
        {
            get
            {
                   ThingDef thing;
                if (Def.HiveDef != null)
                {
                    thing = Def.HiveDef;
                }
                else
                {
                    ThingDef_HiveLike var;
                    if (!(from def in DefDatabase<ThingDef_HiveLike>.AllDefs
                          where ((def.Faction == faction.def && def.Faction != null))
                          select def).TryRandomElement(out var))
                    {
                        thing = var;
                    }
                    else
                    {
                        thing = ThingDefOf.Hive;
                    }
                }
                return (ThingDef_HiveLike)thing;
            }
        }

        public List<PawnKindDef> pawnKinds
        {
            get
            {
                List<PawnKindDef> pawnKinds;
                if (Def.PawnKinds != null && Def.PawnKinds.Count>0)
                {
                    pawnKinds = Def.PawnKinds;
                }
                else if (hiveDef != null)
                {
                    ThingDef_HiveLike def_HiveLike = (ThingDef_HiveLike)hiveDef;
                    if (def_HiveLike.PawnKinds != null && def_HiveLike.PawnKinds.Count > 0)
                    {
                        pawnKinds = def_HiveLike.PawnKinds;
                    }
                    else if (faction != null)
                    {
                        var list = (from def in DefDatabase<PawnKindDef>.AllDefs
                                    where ((def.defaultFactionType == faction.def && def.defaultFactionType != null) || (def.defaultFactionType == null && faction.def.pawnGroupMakers.Any(pgm => pgm.options.Any(opt => opt.kind == def) && pgm.kindDef != PawnGroupKindDefOf.Trader && pgm.kindDef != PawnGroupKindDefOf.Peaceful))) && def.isFighter
                                    select def).ToList();
                        if (list.Count > 0)
                        {
                            pawnKinds = list;
                        }
                        else
                        {
                            pawnKinds = Hive.spawnablePawnKinds;
                        }
                    }
                    else
                    {
                        pawnKinds = Hive.spawnablePawnKinds;
                    }
                }
                else
                {
                    pawnKinds = Hive.spawnablePawnKinds;
                }
                return pawnKinds;
            }
        }

        // Token: 0x0600139E RID: 5022 RVA: 0x00096118 File Offset: 0x00094518
        public void FireEvent(Map map, IntVec3 strikeLoc)
        {
            //Log.Message(string.Format("1"));
            if (!strikeLoc.IsValid)
            {
                //Log.Message(string.Format("1 a"));
                strikeLoc = CellFinderLoose.RandomCellWith((IntVec3 sq) => sq.Standable(map) && !map.roofGrid.Roofed(sq), map, 1000);
                //Log.Message(string.Format("1 b"));
            }
            //Log.Message(string.Format("2"));
            boltMesh = LightningBoltMeshPoolOG.RandomBoltMesh;
            //Log.Message(string.Format("3"));
            if (!strikeLoc.Fogged(map))
            {
                //Log.Message(string.Format("3 a"));
            //    GenExplosion.DoExplosion(strikeLoc, map, 1.9f, DamageDefOf.Flame, null, -1, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
                //Log.Message(string.Format("3 b"));
                Vector3 loc = this.strikeLoc.ToVector3Shifted();
                //Log.Message(string.Format("3 c"));
                for (int i = 0; i < 4; i++)
                {
                    //Log.Message(string.Format("3 c 1"));
                    MoteMaker.ThrowSmoke(loc, map, 1.5f);
                    //Log.Message(string.Format("3 c 2"));
                    MoteMaker.ThrowMicroSparks(loc, map);
                    //Log.Message(string.Format("3 c 3"));
                    MoteMaker.ThrowLightningGlow(loc, map, 1.5f);
                    //Log.Message(string.Format("3 c 4"));
                }
                //Log.Message(string.Format("3 d"));
            }
            //Log.Message(string.Format("4"));
            SoundInfo info = SoundInfo.InMap(new TargetInfo(this.strikeLoc, map, false), MaintenanceType.None);
            //Log.Message(string.Format("5"));
            SoundDefOf.Thunder_OnMap.PlayOneShot(info);
            //Log.Message(string.Format("6"));
        }

        // Token: 0x0600139F RID: 5023 RVA: 0x00096229 File Offset: 0x00094629
        public void EventDraw(Map map, IntVec3 strikeLoc)
        {
            Graphics.DrawMesh(boltMesh, strikeLoc.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather), Quaternion.identity, FadedMaterialPool.FadedVersionOf(TunnelHiveLikeSpawnerStatic.LightningMat, 3f), 0);
        }

        // Token: 0x06002624 RID: 9764 RVA: 0x00122114 File Offset: 0x00120514
        public static void ResetStaticData()
        {
            filthTypes.Clear();
            filthTypes.Add(ThingDefOf.Filth_Dirt);
            filthTypes.Add(ThingDefOf.Filth_Dirt);
            filthTypes.Add(ThingDefOf.Filth_Dirt);
            filthTypes.Add(ThingDefOf.Filth_RubbleRock);
        }

        // Token: 0x06002625 RID: 9765 RVA: 0x00122168 File Offset: 0x00120568
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.secondarySpawnTick, "secondarySpawnTick", 0, false);
            Scribe_Values.Look<bool>(ref this.spawnHive, "spawnHive", true, false);
            Scribe_Values.Look<float>(ref this.hivePoints, "insectsPoints", 0f, false);
            Scribe_Values.Look<bool>(ref this.spawnedByInfestationThingComp, "spawnedByInfestationThingComp", false, false);
        }

        // Token: 0x06002626 RID: 9766 RVA: 0x001221C8 File Offset: 0x001205C8
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                this.secondarySpawnTick = Find.TickManager.TicksGame + this.ResultSpawnDelay.RandomInRange.SecondsToTicks();
            }
            this.CreateSustainer();
        }


        // Token: 0x06002627 RID: 9767 RVA: 0x00122210 File Offset: 0x00120610
        public override void Tick()
        {
            if (base.Spawned)
            {
                HiveLike hive = (HiveLike)ThingMaker.MakeThing(hiveDef, null);
                this.sustainer.Maintain();
                Vector3 vector = base.Position.ToVector3Shifted();
                IntVec3 c;
                ResetStaticData();
                if (Rand.MTBEventOccurs(FilthSpawnMTB, 1f, 1.TicksToSeconds()) && CellFinder.TryFindRandomReachableCellNear(base.Position, base.Map, TunnelHiveLikeSpawner.FilthSpawnRadius, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), null, null, out c, 999999))
                {
                    FilthMaker.MakeFilth(c, base.Map, filthTypes.RandomElement<ThingDef>(), 1);
                }
                if (Rand.MTBEventOccurs(DustMoteSpawnMTB, 1f, 1.TicksToSeconds()))
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
                    this.Destroy(DestroyMode.Vanish);
                    if (this.spawnHive)
                    {
                        if (Def.strikespreexplode)
                        {
                            FireEvent(map, position);
                            EventDraw(map, position);
                        }
                        if (Def.explodesprespawn)
                        {
                            GenExplosion.DoExplosion(position, map, Def.blastradius, Def.damageDef, null, -1, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);

                        }
                        hive = (HiveLike)GenSpawn.Spawn(ThingMaker.MakeThing(hiveDef, null), position, map, WipeMode.Vanish);
                        hive.SetFaction(faction, null);
                        foreach (CompSpawnerLike compSpawner in hive.GetComps<CompSpawnerLike>())
                        {
                            if (compSpawner.PropsSpawner.thingToSpawn == ThingDefOf.InsectJelly)
                            {
                                compSpawner.TryDoSpawn();
                                break;
                            }
                        }
                    }
                    if (this.hivePoints > 0f)
                    {
                        this.hivePoints = Mathf.Max(this.hivePoints, pawnKinds.Min((PawnKindDef x) => x.combatPower));
                        float pointsLeft = this.hivePoints;
                        List<Pawn> list = new List<Pawn>();
                        int num = 0;
                        while (pointsLeft > 0f)
                        {
                            num++;
                            if (num > 1000)
                            {
                                Log.Error("Too many iterations.", false);
                                break;
                            }
                            IEnumerable<PawnKindDef> source = from x in pawnKinds
                                                              where x.combatPower <= pointsLeft
                                                              select x;
                            PawnKindDef pawnKindDef;
                            if (!source.TryRandomElement(out pawnKindDef))
                            {
                                break;
                            }
                            Pawn pawn = PawnGenerator.GeneratePawn(pawnKindDef, faction);
                            GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(position, map, 2, null), map, WipeMode.Vanish);
                            pawn.mindState.spawnedByInfestationThingComp = this.spawnedByInfestationThingComp;
                            list.Add(pawn);
                            pointsLeft -= pawnKindDef.combatPower;
                        }
                        if (list.Any<Pawn>())
                        {
                            LordMaker.MakeNewLord(faction, new LordJob_AssaultColony(faction, true, false, false, false, true), map, list);
                        }
                    }
                }
            }
        }

        // Token: 0x06002628 RID: 9768 RVA: 0x00122538 File Offset: 0x00120938
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

        // Token: 0x06002629 RID: 9769 RVA: 0x001225AC File Offset: 0x001209AC
        private void DrawDustPart(float initialAngle, float speedMultiplier, float scale)
        {
            float num = (Find.TickManager.TicksGame - this.secondarySpawnTick).TicksToSeconds();
            Vector3 pos = base.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.Filth);
            pos.y += 0.046875f * Rand.Range(0f, 1f);
            Color value = new Color(0.470588237f, 0.384313732f, 0.3254902f, 0.7f);
            TunnelHiveLikeSpawnerStatic.matPropertyBlock.SetColor(ShaderPropertyIDs.Color, value);
            Matrix4x4 matrix = Matrix4x4.TRS(pos, Quaternion.Euler(0f, initialAngle + speedMultiplier * num, 0f), Vector3.one * scale);
            Graphics.DrawMesh(MeshPool.plane10, matrix, TunnelHiveLikeSpawnerStatic.TunnelMaterial, 0, null, 0, TunnelHiveLikeSpawnerStatic.matPropertyBlock);
        }

        // Token: 0x0600262A RID: 9770 RVA: 0x0012266E File Offset: 0x00120A6E
        private void CreateSustainer()
        {
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                SoundDef tunnel = SoundDefOf.Tunnel;
                this.sustainer = tunnel.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
            });
        }

        // Token: 0x040015AA RID: 5546
        public bool active = true;

        // Token: 0x04001574 RID: 5492
        private int secondarySpawnTick;

        // Token: 0x04001575 RID: 5493
        public bool spawnHive = true;

        // Token: 0x04001576 RID: 5494
        public float hivePoints;

        // Token: 0x04001577 RID: 5495
        public bool spawnedByInfestationThingComp;

        // Token: 0x04001578 RID: 5496
        private Sustainer sustainer;

        // Token: 0x0400157A RID: 5498
        private readonly FloatRange ResultSpawnDelay = new FloatRange(12f, 16f);

        // Token: 0x0400157B RID: 5499
        [TweakValue("Gameplay", 0f, 1f)]
        private static float DustMoteSpawnMTB = 0.2f;

        // Token: 0x0400157C RID: 5500
        [TweakValue("Gameplay", 0f, 1f)]
        private static float FilthSpawnMTB = 0.3f;

        // Token: 0x0400157D RID: 5501
        [TweakValue("Gameplay", 0f, 10f)]
        private static float FilthSpawnRadius = 3f;

        // Token: 0x0400157F RID: 5503
        private static List<ThingDef> filthTypes = new List<ThingDef>();

        // Token: 0x04000C07 RID: 3079
        private IntVec3 strikeLoc = IntVec3.Invalid;

        // Token: 0x04000C08 RID: 3080
        private Mesh boltMesh;
    }

    [StaticConstructorOnStartup]
    public static class TunnelHiveLikeSpawnerStatic
    {
        // Token: 0x04000C09 RID: 3081
        public static readonly Material LightningMat = MatLoader.LoadMat("Weather/LightningBolt", -1);
        // Token: 0x04001579 RID: 5497
        public static MaterialPropertyBlock matPropertyBlock = new MaterialPropertyBlock();
        // Token: 0x0400157E RID: 5502
        public static readonly Material TunnelMaterial = MaterialPool.MatFrom("Things/Filth/Grainy/GrainyA", ShaderDatabase.Transparent);
    }
}
