using HarmonyLib;
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

        public List<PauldronTextureOption> options = new List<PauldronTextureOption>();
        public PauldronTextureOption defaultOption = new PauldronTextureOption(null, "Blank");


        public bool DrawAtRot(Rot4 bodyFacing)
        {
            if (shoulderPadType == ApparelAddonType.Left && bodyFacing == Rot4.East)
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.Right && bodyFacing == Rot4.West)
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.SouthOnly && bodyFacing != Rot4.South)
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.NotSouth && bodyFacing == Rot4.South)
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.NorthOnly && bodyFacing != Rot4.North)
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.NotNorth && bodyFacing == Rot4.North)
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.NorthSouth && (bodyFacing != Rot4.North && bodyFacing != Rot4.South))
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.EastOnly && bodyFacing != Rot4.East)
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.NotEast && bodyFacing == Rot4.East)
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.WestOnly && bodyFacing != Rot4.West)
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.NotWest && bodyFacing == Rot4.West)
            {
                return false;
            }
            if (shoulderPadType == ApparelAddonType.EastWest && (bodyFacing != Rot4.East && bodyFacing != Rot4.West))
            {
                return false;
            }
            return true;
        }
    }

}
