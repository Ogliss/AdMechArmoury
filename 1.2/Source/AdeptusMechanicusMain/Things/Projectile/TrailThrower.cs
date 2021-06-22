using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000002 RID: 2
    public class TrailThrower
    {
        public static void ThrowSmokeTrail(Vector3 loc, float size, Map map, string DefName, Thing attachTo = null)
        {
            if (!GenView.ShouldSpawnMotesAt(loc, map))
            {
                return;
            }
            ThingDef moteDef = ThingDef.Named(DefName);
            Rand.PushState();
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(moteDef, null);
            moteThrown.Scale = Rand.Range(2f, 3f) * size;
            moteThrown.exactPosition = loc;
            moteThrown.rotationRate = Rand.Range(-0.5f, 0.5f);
            moteThrown.SetVelocity((float)Rand.Range(30, 40), Rand.Range(0.008f, 0.012f));
            Rand.PopState();
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
            return;
        }

        public static void ThrowSprayTrail(Vector3 loc, Map map, Vector3 origin, Vector3 destination, string defname = null, float size = 1.5f, int rotationRate = 240, float projectieSpeed = 0, Color? color = null)
        {
            if (map == null)
            {
                return;
            }
            if (!loc.InBounds(map))
            {
                return;
            }
            if (!GenView.ShouldSpawnMotesAt(loc, map))
            {
                return;
            }
            float forward = 0f;
            if ((destination - origin).MagnitudeHorizontalSquared() > 0.001f)
            {
                forward = (destination - origin).AngleFlat();
            }
            float backward = 0f;
            if ((origin - destination).MagnitudeHorizontalSquared() > 0.001f)
            {
                backward = (origin - destination).AngleFlat();
            }
            TrailThrower.ThrowSprayTrail(loc, map, forward, backward, defname, size, rotationRate, projectieSpeed, color);
        }

        public static void ThrowSprayTrail(Vector3 loc, Map map, float angle, float angleR, string defname = null, float size = 1.5f, int rotationRate = 240, float projectieSpeed = 0, Color? color = null)
        {
            ThingDef def = defname.NullOrEmpty() ? null : DefDatabase<ThingDef>.GetNamedSilentFail(defname);
            MoteThrown moteThrown = TrailThrower.NewBaseAirPuff(def, size, rotationRate);
            moteThrown.exactPosition = loc;
            moteThrown.solidTimeOverride = 0;
            moteThrown.exactPosition += Quaternion.AngleAxis(Rand.Range(-10, 10) + angle, Vector3.up) * Vector3.back * Rand.Range(0, projectieSpeed);
            //moteThrown.exactPosition += new Vector3(Rand.Range(-0.05f, 0.05f), 0f, Rand.Range(-0.05f, 0.05f));
            moteThrown.exactRotation = Rand.Range(0, 360);
            if (color.HasValue)
            {
                moteThrown.instanceColor = color.Value;
            }
            moteThrown.SetVelocity((float)Rand.Range(-15, 15) + angle, Rand.Range(1.2f, 1.5f) + (-(projectieSpeed / 2)) );
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
        }
        private static MoteThrown NewBaseAirPuff(ThingDef def = null, float size = 1.5f, int rotationRate = 240)
        {
            MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(def ?? ThingDefOf.Mote_AirPuff, null);
            moteThrown.Scale = size;
            moteThrown.rotationRate = (float)Rand.RangeInclusive(-rotationRate, rotationRate);
            return moteThrown;
        }

        public static void ThrowAirPuffUp(Vector3 loc, Map map)
        {
            if (map == null)
            {
                return;
            }
            if (!loc.InBounds(map))
            {
                return;
            }
            if (!GenView.ShouldSpawnMotesAt(loc, map))
            {
                return;
            }
            MoteThrown moteThrown = TrailThrower.NewBaseAirPuff();
            moteThrown.exactPosition = loc;
            moteThrown.exactPosition += new Vector3(Rand.Range(-0.02f, 0.02f), 0f, Rand.Range(-0.02f, 0.02f));
            moteThrown.SetVelocity((float)Rand.Range(-45, 45), Rand.Range(1.2f, 1.5f));
            GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map, WipeMode.Vanish);
        }

    }
}
