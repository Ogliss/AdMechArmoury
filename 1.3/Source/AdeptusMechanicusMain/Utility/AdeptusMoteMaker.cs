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
        // AdeptusFleckMaker.ThrowDustPuff
        /*
        public static void ThrowDustPuff(Vector3 loc, Map map, float scale, ThingDef def = null, Color? color = null)
        {
            if (!loc.ShouldSpawnMotesAt(map))
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(def ?? AdeptusThingDefOf.OG_Mote_DustPuff, null);
            moteThrown.Scale = 1.9f * scale;
            moteThrown.rotationRate = (float)Rand.Range(-60, 60);
            moteThrown.exactPosition = loc;
            moteThrown.SetVelocity((float)Rand.Range(0, 360), Rand.Range(0.6f, 0.75f));
            if (color.HasValue)
            {
                moteThrown.instanceColor = color.Value;
                //    moteThrown.instanceColor = new Color(0.368f, 0f, 1f);
            }

            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
        }
        public static void ThrowSmoke(Vector3 loc, float size, Map map, ThingDef def = null , Color? color = null, float? exactRotation = null)
        {
            if (!GenView.ShouldSpawnMotesAt(loc, map))
            {
                return;
            }
            Rand.PushState();
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(def ?? AdeptusThingDefOf.OG_Mote_Smoke, null);
            moteThrown.Scale = Rand.Range(1.5f, 2.5f) * size;
            moteThrown.rotationRate = Rand.Range(-30f, 30f);
            moteThrown.exactPosition = loc;
            moteThrown.SetVelocity((float)Rand.Range(30, 40), Rand.Range(0.5f, 0.7f));
            if (color.HasValue)
            {
                moteThrown.instanceColor = color.Value;
                //    moteThrown.instanceColor = new Color(0.368f, 0f, 1f);
            }
            if (exactRotation.HasValue)
            {
                moteThrown.exactRotation += exactRotation.Value;
            }
            Rand.PopState();
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
        }
        */
        public static void ThrowExplosionCell(IntVec3 cell, Map map, ThingDef moteDef, Color? color = null)
        {
            if (!cell.ShouldSpawnMotesAt(map))
            {
                return;
            }
            Mote moteThrown = (Mote)ThingMaker.MakeThing(moteDef, null);
            moteThrown.exactRotation = (float)(90 * Rand.RangeInclusive(0, 3));
            moteThrown.exactPosition = cell.ToVector3Shifted();
            if (color.HasValue)
            {
                moteThrown.instanceColor = color.Value;
                //    moteThrown.instanceColor = new Color(0.368f, 0f, 1f);
            }
            GenSpawn.Spawn(moteThrown, cell, map, WipeMode.Vanish);
            if (Rand.Value < 0.7f)
            {
                AdeptusFleckMaker.ThrowDustPuff(cell.ToVector3Shifted(), map, 1.2f, null, color);
            }
        }
        public static Mote MakeStaticMote(Vector3 loc, Map map, ThingDef moteDef, float scale = 1f, Color? color = null, float? exactRotation = null)
        {
            if (!loc.ShouldSpawnMotesAt(map))
            {
                return null;
            }
            Mote moteThrown = (Mote)ThingMaker.MakeThing(moteDef, null);
            moteThrown.exactPosition = loc;
            moteThrown.Scale = scale;
            if (color.HasValue)
            {
                moteThrown.instanceColor = color.Value;
            //    moteThrown.instanceColor = new Color(0.368f, 0f, 1f);
            }
            if (exactRotation.HasValue)
            {
                moteThrown.exactRotation += exactRotation.Value;
            }
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
            return moteThrown;
        }
        public static void ThrowLightningGlow(Vector3 loc, Map map, float size, Color? color = null)
        {
            if (!loc.ToIntVec3().InBounds(map))
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(AdeptusThingDefOf.OG_Mote_LightningGlow, null);
            moteThrown.Scale = Rand.Range(4f, 6f) * size;
            moteThrown.rotationRate = Rand.Range(-3f, 3f);
            moteThrown.exactPosition = loc + size * new Vector3(Rand.Value - 0.5f, 0f, Rand.Value - 0.5f);
            moteThrown.SetVelocity((float)Rand.Range(0, 360), 1.2f);
            if (color.HasValue)
            {
                moteThrown.instanceColor = color.Value;
                //    moteThrown.instanceColor = new Color(0.368f, 0f, 1f);
            }
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
        }
        /*
        public static void ThrowToxicGas(Vector3 loc, Map map, float size, Color? color = null)
        {
            if (!loc.ToIntVec3().InBounds(map))
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(AdeptusThingDefOf.OG_Mote_Smoke, null);
            moteThrown.Scale = Rand.Range(1.5f, 2.5f) * size;
            moteThrown.rotationRate = Rand.Range(-30f, 30f);
            moteThrown.exactPosition = loc;
            moteThrown.SetVelocity((float)Rand.Range(30, 40), Rand.Range(0.5f, 0.7f));
            if (color.HasValue)
            {
                moteThrown.instanceColor = color.Value;
                //    moteThrown.instanceColor = new Color(0.368f, 0f, 1f);
            }
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
        }

        public static void ThrowToxicSmoke(Vector3 loc, Map map, Color? color = null)
        {
            if (!loc.ShouldSpawnMotesAt(map))
            {
                return;
            }
            //moteThrown.instanceColor = new ColorInt(43, 56, 54).ToColor; // to investigate
            //moteThrown.Scale = Rand.Range(2.5f, 3.9f); to investigate
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(AdeptusThingDefOf.OG_Mote_Smoke, null);
            moteThrown.Scale = Rand.Range(0.5f, 0.9f);
            moteThrown.rotationRate = Rand.Range(-30f, 30f);
            moteThrown.exactPosition = loc;
            moteThrown.airTimeLeft = Rand.Range(0.1f, 0.4f);
            moteThrown.Speed = 0.3f;
            moteThrown.SetVelocity((float)Rand.Range(-20, 20), Rand.Range(0.5f, 0.7f));
            if (color.HasValue)
            {
                moteThrown.instanceColor = color.Value;
                //    moteThrown.instanceColor = new Color(0f, 0.0862f, 0.094117f);
            }
            GenSpawn.Spawn(moteThrown, IntVec3Utility.ToIntVec3(loc), map, WipeMode.Vanish);
        }
        public static void ThrowToxicPostExplosionSmoke(Vector3 loc, Map map, float size, Color? color = null)
        {
            if (!loc.ShouldSpawnMotesAt(map))
            {
                return;
            }
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(AdeptusThingDefOf.OG_Mote_Smoke, null);
            moteThrown.Scale = Rand.Range(1.5f, 2.5f) * size;
            moteThrown.rotationRate = Rand.Range(-30f, 30f);
            moteThrown.exactPosition = loc;
            moteThrown.SetVelocity((float)Rand.Range(30, 40), Rand.Range(0.5f, 0.7f));
            if (color.HasValue)
            {
                moteThrown.instanceColor = color.Value;
                //    moteThrown.instanceColor = new Color(0.368f, 0f, 1f);
            }
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);

        }
        */
    }
}
