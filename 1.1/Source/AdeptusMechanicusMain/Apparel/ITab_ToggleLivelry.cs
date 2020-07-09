using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class ITab_ToggleLivelry : ITab
    {
        public ITab_ToggleLivelry()
        {
            size = ToggleLivelryCardUtility.CardSize() + new Vector2(17f, 17f) * 2f;
        }

        public override bool IsVisible
        {
            get
            {
#pragma warning disable IDE0019 // Use pattern matching
                var selected = SelThing as ThingWithComps;
#pragma warning restore IDE0019 // Use pattern matching
                if (selected != null)
                {
                    var td = selected.GetComp<CompPauldronDrawer>();
                    if (td != null)
                    {
                        //Log.Message("ITab_isvisible");
                        labelKey = td.Props.labelKey; // defined by the Comp
                        return true;
                    }
                }
                return false;
            }
        }

        protected override void FillTab()
        {
            var selected = Find.Selector.SingleSelectedThing as ThingWithComps;
            var td = selected.GetComp<CompPauldronDrawer>();
            if (td == null) Log.Warning("selected thing has no CompPauldronDrawer for ITab_ToggleLivelry");
            labelKey = ((CompProperties_PauldronDrawer)td.props).labelKey; //"UM_TabToggleDef";//.Translate();
            if (labelKey == null) labelKey = "TOGGLEDEF";
            var rect = new Rect(17f, 17f, ToggleLivelryCardUtility.CardSize().x, ToggleLivelryCardUtility.CardSize().y);
            ToggleLivelryCardUtility.DrawCard(rect, selected);
        }
    }
}