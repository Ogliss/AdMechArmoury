using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace AnimatedProjectile
{
    public class Projectile_Anim : Projectile
    {
        private int frameTicks => this.def.HasModExtension<AnimatedProjectileExtension>()? this.def.GetModExtension<AnimatedProjectileExtension>().ticksPerFrame : 5;

        public override void Tick()
        {
            base.Tick();
            checked
            {
                bool flag;
                if (this.def.graphicData.graphicClass == typeof(Graphic_Flicker))
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
                this.TicksforFrame--;
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
            if (this.def.graphicData.graphicClass == typeof(Graphic_Flicker))
            {
                traverse = Traverse.Create(base.Graphic);
                subGraphics = (Graphic[])subgraphics.GetValue(base.Graphic);
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
        private int TicksforFrame = 5;
    }
}
