using AdeptusMechanicus.ExtensionMethods;
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
    [StaticConstructorOnStartup]
    public static class PawnRenderUtility
    {
        static PawnRenderUtility()
        {
            foreach (ThingDef item in DefDatabase<ThingDef>.AllDefsListForReading.Where(x=> x.IsApparel))
            {
                if (!pauldrons.Contains(item) && item.HasComp(typeof(CompPauldronDrawer)))
                {
                    pauldrons.Add(item);
                }
                if (!extras.Contains(item) && item.HasComp(typeof(CompApparelExtraPartDrawer)))
                {
                    extras.Add(item);
                }
            }
        }

        public static Material OverrideMaterialIfNeeded(Material original, Pawn pawn)
        {
            Material baseMat = pawn.IsInvisible() ? InvisibilityMatPool.GetInvisibleMat(original) : original;
            return baseMat;//  pawn.Drawer.renderer.graphics.flasher.GetDamagedMat(baseMat);
        }

        public static void AlienRacesPatch(Pawn pawn, Rot4 bodyFacing, out Vector2 size, bool portrait)
        {
            AlienRace.ThingDef_AlienRace alienDef = pawn.def as AlienRace.ThingDef_AlienRace;
            Vector2 d;
            if (alienDef != null)
            {
                AlienRace.AlienPartGenerator.AlienComp comp = pawn.TryGetCompFast<AlienRace.AlienPartGenerator.AlienComp>();
                if (comp != null)
                {
                    //    d = (portrait ? comp.alienPortraitGraphics.bodySet.MeshAt(bodyFacing).bounds.size : comp.alienGraphics.bodySet.MeshAt(bodyFacing).bounds.size);
                    d = (portrait ? comp.customPortraitDrawSize : comp.customDrawSize);

                    size = new Vector2(d.x * 1.5f, d.y * 1.5f);
                    return;
                }
            }
            size = new Vector2(1.5f, 1.5f);
            return;
        }
        public static Mesh GetPawnMesh(bool portrait, Pawn pawn, Rot4 facing, bool wantsBody)
        {

            if (AdeptusIntergrationUtility.enabled_AlienRaces)
            {
                if (!wantsBody)
                {
                    return HumanlikeMeshPoolUtility.GetHumanlikeHeadSetForPawn(pawn).MeshAt(facing);
                }
                return HumanlikeMeshPoolUtility.GetHumanlikeBodySetForPawn(pawn).MeshAt(facing);
            }
            if (!wantsBody)
            {
                return MeshPool.humanlikeHeadSet.MeshAt(facing);
            }
            return MeshPool.humanlikeBodySet.MeshAt(facing);
        }
        /*
         * broken by changes in har
        public static Mesh GetAlienPawnMesh(bool portrait, Pawn pawn, Rot4 facing, bool wantsBody)
        {

            AlienRace.AlienPartGenerator.AlienComp comp = pawn.GetComp<AlienRace.AlienPartGenerator.AlienComp>();
            if (comp == null)
            {
                if (!wantsBody)
                {
                    return MeshPool.humanlikeHeadSet.MeshAt(facing);
                }
                return MeshPool.humanlikeBodySet.MeshAt(facing);
            }
            else if (!portrait)
            {
                if (!wantsBody)
                {
                    return comp.alienHeadGraphics.headSet.MeshAt(facing);
                }
                return comp.alienGraphics.bodySet.MeshAt(facing);
            }
            else
            {
                if (!wantsBody)
                {
                    return comp.alienPortraitHeadGraphics.headSet.MeshAt(facing);
                }
                return comp.alienPortraitGraphics.bodySet.MeshAt(facing);
            }
        }
        */
        private static List<ThingDef> pauldrons = new List<ThingDef>();
        public static List<ThingDef> Pauldrons
        {
            get
            {
                return pauldrons;
            }
        }

        private static List<ThingDef> extras = new List<ThingDef>();
        public static List<ThingDef> Extras
        {
            get
            {
                return extras;
            }
        }

        public static bool CompositeApparel(Thing thing)
        {
            return CompositeApparel(thing.def);
        }
        public static bool CompositeApparel(ThingDef thing)
        {
            if ((!PawnRenderUtility.Pauldrons.NullOrEmpty() && PawnRenderUtility.Pauldrons.Contains(thing)) || (!PawnRenderUtility.Extras.NullOrEmpty() && PawnRenderUtility.Extras.Contains(thing)))
            {
                return true;
            }
            return false;
        }
    }
}
