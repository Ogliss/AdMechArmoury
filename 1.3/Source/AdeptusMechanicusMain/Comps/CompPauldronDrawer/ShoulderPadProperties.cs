﻿using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public class ShoulderPadProperties
    {
        public ApparelAddonType shoulderPadType;
        public ShaderTypeDef shaderType;
        public bool bodyspecificTextures = true;
        public bool forceDynamicDraw = false;
        public bool? drawInBed;
        public string padTexPath;
        public string label;
        public int commonality;
        public string northtop;
        public string southtop;
        public string easttop;
        public string westtop;
        public bool UseFactionTextures = false;
        public bool UseFactionColors = true;
        public bool UseVariableTextures;
        public bool UsePrimaryColor = true;
        public bool UseSecondaryColorAsPrimary = false;
        public Color? overridePrimaryColor;
        public bool UseSecondaryColor = true;
        public bool UsePrimaryColorAsSecondary = false;
        public Color? overrideSecondaryColor;
        public int order = 1;
        public int sublayer = 0;
        public bool flipWest = false;
        public Vector3 NorthOffset = new Vector3();
        public Vector3 SouthOffset = new Vector3();
        public Vector3 EastOffset = new Vector3();
        public Vector3 WestOffset = new Vector3();
        public Vector2 size = new Vector2(1.5f, 1.5f);

        public PauldronTextureOption activeOption;
        public List<PauldronTextureOption> options = new List<PauldronTextureOption>();
        public PauldronTextureOption defaultOption = new PauldronTextureOption(null, "Blank");
    }

}
