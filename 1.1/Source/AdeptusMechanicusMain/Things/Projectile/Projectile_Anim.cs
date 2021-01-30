using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class Projectile_Anim : Bullet
    {
        private static int frameTicks = 5;
        private int TicksforFrame = frameTicks;
        public override void Tick()
        {
            base.Tick();
            checked
            {
                bool flag;
                this.TicksforFrame--;
                if (this.def.graphicData.graphicClass == typeof(Graphic_Cycle))
                {
                    flag = this.TicksforFrame == 0 && base.Map != null;
                    if (flag)
                    {
                        gfxint++;
                        if (gfxint >= subGraphics.Length)
                        {
                            gfxint = 0;
                        }
                        this.TicksforFrame = frameTicks;
                    }
                }
            }
        }
        
        public override Graphic Graphic
        {
            get
            {
            //    Log.Message(string.Format("Subgraphic used: {0}", gfxint));
                if (subGraphics!=null)
                {
                    return subGraphics[gfxint];
                }
                return base.Graphic;
            }
        }
        
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            Graphic_Cycle graphic = base.Graphic as Graphic_Cycle;
            if (graphic != null)
            {
                subGraphics = graphic.graphics;
            //    Log.Message(string.Format("Subgraphics: {0}", subGraphics.Length));
            }

        }

        public override void Draw()
        {

            Mesh mesh = MeshPool.GridPlane(this.def.graphicData.drawSize);
            Graphics.DrawMesh(mesh, this.DrawPos, this.ExactRotation, Graphic.MatSingle, 0);
            base.Comps_PostDraw();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.gfxint, "gfxint", -1);

        }

        Traverse traverse;
        Graphic[] subGraphics;
        public static FieldInfo subgraphics = typeof(Graphic_Flicker).GetField("subGraphics", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
        private int gfxint = 0;
    }
}
