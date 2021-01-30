using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public static class AdeptusMoteMaker
    {
        public static void ThrowDustPuff(Vector3 loc, Map map, float scale)
        {
            if (!loc.ShouldSpawnMotesAt(map))
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDefOf.Mote_DustPuff, null);
            moteThrown.Scale = 1.9f * scale;
            moteThrown.rotationRate = (float)Rand.Range(-60, 60);
            moteThrown.exactPosition = loc;
            moteThrown.SetVelocity((float)Rand.Range(0, 360), Rand.Range(0.6f, 0.75f));
            moteThrown.instanceColor = new Color(0.368f, 0f, 1f);

            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
        }

        public static void ThrowExplosionCell(IntVec3 cell, Map map, ThingDef moteDef, Color color)
        {
            if (!cell.ShouldSpawnMotesAt(map))
            {
                return;
            }
            Mote moteThrown = (Mote)ThingMaker.MakeThing(moteDef, null);
            moteThrown.exactRotation = (float)(90 * Rand.RangeInclusive(0, 3));
            moteThrown.exactPosition = cell.ToVector3Shifted();
            moteThrown.instanceColor = new Color(0.368f, 0f, 1f);
            GenSpawn.Spawn(moteThrown, cell, map, WipeMode.Vanish);
            if (Rand.Value < 0.7f)
            {
                AdeptusMoteMaker.ThrowDustPuff(cell.ToVector3Shifted(), map, 1.2f);
            }
        }
        public static Mote MakeStaticMote(Vector3 loc, Map map, ThingDef moteDef, float scale = 1f)
        {
            if (!loc.ShouldSpawnMotesAt(map))
            {
                return null;
            }
            Mote moteThrown = (Mote)ThingMaker.MakeThing(moteDef, null);
            moteThrown.exactPosition = loc;
            moteThrown.Scale = scale;
            moteThrown.instanceColor = new Color(0.368f, 0f, 1f);
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
            return moteThrown;
        }
        public static void ThrowLightningGlow(Vector3 loc, Map map, float size)
        {
            if (!loc.ToIntVec3().InBounds(map))
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDefOf.Mote_LightningGlow, null);
            moteThrown.Scale = Rand.Range(4f, 6f) * size;
            moteThrown.rotationRate = Rand.Range(-3f, 3f);
            moteThrown.exactPosition = loc + size * new Vector3(Rand.Value - 0.5f, 0f, Rand.Value - 0.5f);
            moteThrown.SetVelocity((float)Rand.Range(0, 360), 1.2f);
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
        }
        public static void ThrowToxicGas(Vector3 loc, Map map, float size)
        {
            if (!loc.ToIntVec3().InBounds(map))
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDefOf.Mote_Smoke, null);
            moteThrown.Scale = Rand.Range(1.5f, 2.5f) * size;
            moteThrown.rotationRate = Rand.Range(-30f, 30f);
            moteThrown.exactPosition = loc;
            moteThrown.SetVelocity((float)Rand.Range(30, 40), Rand.Range(0.5f, 0.7f));
            moteThrown.instanceColor = new Color(0.368f, 0f, 1f);
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
        }
        public static void ThrowToxicSmoke(Vector3 loc, Map map)
        {
            if (!loc.ShouldSpawnMotesAt(map))
            {
                return;
            }
            //moteThrown.instanceColor = new ColorInt(43, 56, 54).ToColor; // to investigate
            //moteThrown.Scale = Rand.Range(2.5f, 3.9f); to investigate
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDefOf.Mote_Smoke, null);
            moteThrown.Scale = Rand.Range(0.5f, 0.9f);
            moteThrown.rotationRate = Rand.Range(-30f, 30f);
            moteThrown.exactPosition = loc;
            moteThrown.airTimeLeft = Rand.Range(0.1f, 0.4f);
            moteThrown.Speed = 0.3f;
            moteThrown.SetVelocity((float)Rand.Range(-20, 20), Rand.Range(0.5f, 0.7f));
            moteThrown.instanceColor = new Color(0f, 0.0862f, 0.094117f);
            GenSpawn.Spawn(moteThrown, IntVec3Utility.ToIntVec3(loc), map, WipeMode.Vanish);
        }

        public static void ThrowToxicPostExplosionSmoke(Vector3 loc, Map map, float size)
        {
            if (!loc.ShouldSpawnMotesAt(map))
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDefOf.Mote_Smoke, null);
            moteThrown.Scale = Rand.Range(1.5f, 2.5f) * size;
            moteThrown.rotationRate = Rand.Range(-30f, 30f);
            moteThrown.exactPosition = loc;
            moteThrown.SetVelocity((float)Rand.Range(30, 40), Rand.Range(0.5f, 0.7f));
            moteThrown.instanceColor = new Color(0.368f, 0f, 1f);
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);

        }

        public static void ThrowLightningBolt(Vector3 loc, Map map)
        {
            if (!loc.ShouldSpawnMotesAt(map))
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDef.Named("Mote_BlastEMP"), null);
            moteThrown.Scale = Rand.Range(0.5f, 1f);
            moteThrown.rotationRate = Rand.Range(-12f, 12f);
            moteThrown.exactPosition = loc;
            GenSpawn.Spawn(moteThrown, IntVec3Utility.ToIntVec3(loc), map, 0);
        }
        public static void ThrowEMPMicroSparks(Vector3 loc, Map map)
        {
            if (!loc.ShouldSpawnMotesAt(map))
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDef.Named("EMP_Sparks"), null);
            moteThrown.Scale = Rand.Range(5f, 8f);
            moteThrown.rotationRate = Rand.Range(-12f, 12f);
            moteThrown.exactPosition = loc;
            moteThrown.exactPosition -= new Vector3(0.5f, 0f, 0.5f);
            moteThrown.exactPosition += new Vector3(Rand.Value, 0f, Rand.Value);
            moteThrown.SetVelocity((float)Rand.Range(35, 45), 1.2f);
            GenSpawn.Spawn(moteThrown, IntVec3Utility.ToIntVec3(loc), map, 0);
        }

        public static void ThrowEMPLightningGlow(Vector3 loc, Map map, float size)
        {
            if (!loc.ShouldSpawnMotesAt(map))
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDef.Named("EMPGlow"), null);
            moteThrown.Scale = Rand.Range(6f, 8f) * size;
            moteThrown.rotationRate = Rand.Range(-3f, 3f);
            moteThrown.exactPosition = loc + size * new Vector3(Rand.Value - 0.5f, 0f, Rand.Value - 0.5f);
            moteThrown.SetVelocity((float)Rand.Range(0, 360), 1.2f);
            GenSpawn.Spawn(moteThrown, IntVec3Utility.ToIntVec3(loc), map, 0);
        }

        public static void ThrowEMPSmoke(Vector3 loc, Map map, float size)
        {
            if (!GenView.ShouldSpawnMotesAt(loc, map))
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(ThingDef.Named("Mote_EMPSmoke"), null);
            moteThrown.Scale = Rand.Range(1.5f, 2.5f) * size;
            moteThrown.rotationRate = Rand.Range(-30f, 30f);
            moteThrown.exactPosition = loc;
            moteThrown.SetVelocity((float)Rand.Range(30, 40), Rand.Range(0.5f, 0.7f));
            GenSpawn.Spawn(moteThrown, IntVec3Utility.ToIntVec3(loc), map, 0);
        }
    }
}
