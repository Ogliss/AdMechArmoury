using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public static class AMConstants
    {
        //   public static int AMSeed = 454385387;
        public static int AMSeed = 454385387;
        public static readonly string ModPrefix = "AM_";

        public static Color YautjaCloakColor = new Color(0.25f, 0.25f, 0.25f, 0.0001f);
        public static int CloakNoiseTex = Shader.PropertyToID("_NoiseTex");

        public static PawnGraphicSet Invisiblegraphics(Pawn pawn)
        {
            PawnGraphicSet graphics = new PawnGraphicSet_Invisible(pawn)
            {
                nakedGraphic = new Graphic_Invisible(),
                rottingGraphic = new Graphic_Invisible(),
                packGraphic = new Graphic_Invisible(),
                headGraphic = new Graphic_Invisible(),
                desiccatedHeadGraphic = new Graphic_Invisible(),
                skullGraphic = new Graphic_Invisible(),
                headStumpGraphic = new Graphic_Invisible(),
                desiccatedHeadStumpGraphic = new Graphic_Invisible(),
                hairGraphic = new Graphic_Invisible()
            };
            return graphics;
        }
    }
}
