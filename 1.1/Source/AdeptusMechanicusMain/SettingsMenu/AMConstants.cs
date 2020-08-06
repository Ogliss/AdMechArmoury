﻿using System;
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
        public static int AvPSeed = 454385387;
        public static readonly string ModPrefix = "AvP_";

        public static Color YautjaCloakColor = new Color(0.25f, 0.25f, 0.25f, 0.0001f);
        public static int CloakNoiseTex = Shader.PropertyToID("_NoiseTex");

        public static PawnGraphicSet Invisiblegraphics(Pawn pawn)
        {
            PawnGraphicSet graphics = new PawnGraphicSet_Invisible(pawn)
            {
                nakedGraphic = new Graphic_Invisible(),
                rottingGraphic = null,
                packGraphic = null,
                headGraphic = new Graphic_Invisible(),
                desiccatedHeadGraphic = null,
                skullGraphic = null,
                headStumpGraphic = null,
                desiccatedHeadStumpGraphic = null,
                hairGraphic = new Graphic_Invisible()
            };
            return graphics;
        }
    }
}
