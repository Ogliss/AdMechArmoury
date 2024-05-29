using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000002 RID: 2
    public class TrailThrower
    {
        public static void ThrowSprayTrail(Vector3 loc, Map map, Vector3 origin, Vector3 destination, FleckDef fleck = null, float size = 1.5f, int rotationRate = 240, float projectieSpeed = 0, Color? color = null)
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
            TrailThrower.ThrowSprayTrail(loc, map, forward, backward, fleck, size, rotationRate, projectieSpeed, color);
        }

        public static void ThrowSprayTrail(Vector3 loc, Map map, float angle, float angleR, FleckDef fleck = null, float size = 1.5f, int rotationRate = 240, float projectieSpeed = 0, Color? color = null)
        {
            if (fleck != null)
            {
                Rand.PushState();
                Vector3 sloc = loc;// + Quaternion.AngleAxis(Rand.Range(-10, 10) + angle, Vector3.up) * Vector3.back * Rand.Range(0, projectieSpeed);
                AdeptusFleckMaker.Thrown(sloc, map, fleck, size, color, Rand.Range(0, 360), rotationRate, null, Rand.Range(0, projectieSpeed), (float)Rand.Range(-15, 15) + angle);
                Rand.PopState();
            }
        }

    }
}
