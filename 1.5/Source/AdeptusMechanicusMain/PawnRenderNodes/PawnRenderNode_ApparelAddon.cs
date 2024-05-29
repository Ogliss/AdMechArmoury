using AdeptusMechanicus.settings;
using AdeptusMechanicus.Utility;
using HugsLib.Utils;
using RimWorld;
using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static HarmonyLib.Code;
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace AdeptusMechanicus
{
    public class PawnRenderNodeProperties_ApparelAddon : PawnRenderNodeProperties
    {


    }
    public class PawnRenderNodeWorker_Apparel_BodyAddon : PawnRenderNodeWorker_Apparel_Body
    {
        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            PawnRenderNode_ApparelAddon addon = node as PawnRenderNode_ApparelAddon;
            return addon.entry.CheckPauldronRotation(parms.facing) && base.CanDrawNow(node, parms);
        }

    }
    public class PawnRenderNodeWorker_Apparel_HeadAddon : PawnRenderNodeWorker_Apparel_Head
    {
        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            PawnRenderNode_ApparelAddon addon = node as PawnRenderNode_ApparelAddon;
            return addon.entry.CheckPauldronRotation(parms.facing) && base.CanDrawNow(node, parms);
        }

    }
    // AdeptusMechanicus.PawnRenderNode_ApparelAddon
    public class PawnRenderNode_ApparelAddon : PawnRenderNode_Apparel
    {
        public PawnRenderNode_ApparelAddon(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree, Apparel apparel, ShoulderPadEntry entry) : base(pawn, props, tree, apparel)
        {
        //    if (AMAMod.Dev) Log.Message($"Mew PawnRenderNode_ApparelAddon: {pawn} {apparel} {entry}");
            this.apparel = apparel;
            this.entry = entry;
            this.pawn = pawn;
        }
        public override void EnsureMaterialsInitialized()
        {
            ApparelGraphicRecord apparelGraphicRecord;
            if (this.graphic == null && ApparelAddonGraphicRecordGetter.TryGetGraphicApparelAddon(this.entry, this.pawn, out apparelGraphicRecord))
            {
                this.graphic = apparelGraphicRecord.graphic;
            //    if (AMAMod.Dev) Log.Message($"PawnRenderNode_ApparelAddon Graphic for: {pawn} {apparel} {entry} PATH= {this.graphic.path}");
            }
        }
        Pawn pawn;
        public ShoulderPadEntry entry;
    }
    // AdeptusMechanicus.PawnRenderNode_ApparelExtra
    public class PawnRenderNode_ApparelExtra : PawnRenderNode_Apparel
    {
        public PawnRenderNode_ApparelExtra(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree, Apparel apparel, CompApparelExtraPartDrawer entry) : base(pawn, props, tree, apparel)
        {
        //    if (AMAMod.Dev) Log.Message($"Mew PawnRenderNode_ApparelAddon: {pawn} {apparel} {entry}");
            this.apparel = apparel;
            this.entry = entry;
            this.pawn = pawn;
        }
        
        public override void EnsureMaterialsInitialized()
        {
            if (this.graphic == null && entry.Graphic != null)
            {
                this.graphic = entry.Graphic;
            //    if (AMAMod.Dev) Log.Message($"PawnRenderNode_ApparelAddon Graphic for: {pawn} {apparel} {entry} PATH= {this.graphic.path}");
            }
        }
        Pawn pawn;
        public CompApparelExtraPartDrawer entry;
    }
}
