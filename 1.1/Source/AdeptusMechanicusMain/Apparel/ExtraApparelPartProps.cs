﻿using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public class ExtraApparelPartProps
    {
        public GraphicData graphicData;
        public BodyTypeDef forcedBodyType = null;
        public bool OnHead;
        public bool UseBodytypeOffsets;
        public bool UseHeadOffsets;
        public int commonality;
        public Vector2 drawSize = new Vector2(1, 1);

        public Apparel apparel;
        public ShoulderPadType shoulderPadType;
        public bool bodyspecificTextures = true;
        public string texPath;
        public string label;
        public bool hidesHair = false;
        public bool hidesHead = false;
        public bool hidesBody = false;
        public bool northtop = false;
        public bool UseFactionTextures = false;
        public bool UseFactionColors = false;
        public bool UseVariableTextures;
        public bool UsePrimaryColor = true;
        public bool UseSecondaryColorAsPrimary = false;
        public Color? overridePrimaryColor;
        public bool UseSecondaryColor = true;
        public bool UsePrimaryColorAsSecondary = false;
        public Color? overrideSecondaryColor;
        public int order = 1;
        public int sublayer = 0;
        public Vector3 NorthOffset = new Vector3();
        public Vector3 SouthOffset = new Vector3();
        public Vector3 EastOffset = new Vector3();
        public Vector3 WestOffset = new Vector3();
        public Vector2 size = new Vector2(1.5f, 1.5f);

        public PauldronTextureOption activeOption;
        public List<PauldronTextureOption> options = new List<PauldronTextureOption>();
        public PauldronTextureOption defaultOption = new PauldronTextureOption(null, "Blank");


        public Graphic Graphic(Apparel thing)
        {
            if (thing.Wearer == null)
            {
                graphic = null;
            }
            else
            if (graphic == null)
            {
                graphic = graphicData.GraphicColoredFor(thing);
            }
            return graphic;
        }

        public GraphicMeshSet MeshSet(Pawn pawn)
        {
            if (apparel?.Wearer == null)
            {
                return new GraphicMeshSet(1.5f, 1.5f);
            }
            if (meshSet == null)
            {
                meshSet = new GraphicMeshSet(size.x, size.y);
            }
            return meshSet;
        }

        private Graphic graphic;
        private GraphicMeshSet meshSet;
    }

}
