using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace AdeptusMechanicus
{
    public class Apparel_Colour : Apparel
    {
        public override void PostMake()
        {
            base.PostMake();
            this.Notify_ColorChanged();
        }

        public Color drawColor = Color.white;
        public override Color DrawColor
        {
            get
            {
                bool selected = Find.Selector.SingleSelectedThing == Wearer && false;
                CompColorable comp = base.GetComp<CompColorable>();
                if (comp != null && comp.Active)
                {
                    if (selected)
                    {
                //        Log.Message(string.Format("Apparel_Colour return {1}'s comp.Color {0}", comp.Color, this.def.label));
                    }
                    return comp.Color;
                }
                else if (this.def.colorGenerator != null && (this.Stuff == null || this.Stuff.stuffProps.allowColorGenerators))
                {
                    if (drawColor == Color.white)
                    {
                        drawColor = this.def.colorGenerator.NewRandomizedColor();
                        if (selected)
                        {
                    //        Log.Message(string.Format("Apparel_Colour return {1}'s drawColor {0}", drawColor, this.def.label));
                        }
                        return drawColor;
                    }
                    base.DrawColor = this.def.colorGenerator.NewRandomizedColor();
                    if (selected)
                    {
                //        Log.Message(string.Format("Apparel_Colour return {1}'s base.DrawColor {0}", base.DrawColor, this.def.label));
                    }
                    return base.DrawColor;
                }
                else if ((this.Stuff != null && this.Stuff.stuffProps.color != null))
                {
                    if (selected)
                    {
                //        Log.Message(string.Format("Apparel_Colour return {1}'s stuffProps.color {0}", this.Stuff.stuffProps.color, this.def.label));
                    }
                    drawColor = this.Stuff.stuffProps.color;
                    return this.Stuff.stuffProps.color;
                }
                if (selected)
                {
            //        Log.Message(string.Format("Apparel_Colour return {1}'s base.DrawColor {0}", base.DrawColor, this.def.label));
                }
                return base.DrawColor;
            }
            set
            {
                this.SetColor(value, true);
            }
        }

        public override void DrawWornExtras()
        {
            base.DrawWornExtras();
            CompPauldronDrawer comp = base.GetComp<CompPauldronDrawer>();
            if (comp != null)
            {
                comp.PostDraw();
            }
        }
    }
}