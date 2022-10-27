using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.Projectile_Fire
    public class Projectile_Grower : Projectile_Anim
    {
        public override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing, blockedByShield);
        }

        public override void Tick()
        {
            base.Tick();

            distancetotravel = launcher.Position.DistanceTo(usedTarget.Cell);
            distancetraveled = launcher.Position.DistanceTo(this.Position);
            traveled = (distancetraveled / distancetotravel);
            pos = this.Position;
        }

        public override Quaternion ExactRotation
        {
            get
            {
                var forward = destination - origin;
                forward.y = 0;
                return Quaternion.LookRotation(forward);
            }
        }

        public override void Draw()
        {
            Mesh mesh = MeshPool.GridPlane(this.def.graphicData.drawSize * traveled);
            Graphics.DrawMesh(mesh, this.DrawPos, this.ExactRotation, Graphic.MatSingle, 0);
            base.Comps_PostDraw();
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
        }
        private float distancetotravel = 0;
        private float distancetraveled = 0;
        private float traveled = 0;
        private int age = 0;
        private IntVec3 pos = IntVec3.Invalid;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref distancetotravel, "distancetotravel");
            Scribe_Values.Look(ref distancetraveled, "distancetraveled");
            Scribe_Values.Look(ref age, "age");
        }
    }
}
