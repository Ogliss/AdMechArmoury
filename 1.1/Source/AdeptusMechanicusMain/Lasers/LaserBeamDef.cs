﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class LaserBeamDecoration
    {
        public ThingDef mote;
        public float spacing = 1.0f;
        public float initialOffset = 0;
        public float speed = 1.0f;
        public float speedJitter;
        public float speedJitterOffset;

    }

    public class LaserBeamDef : ThingDef
    {
        public float capSize = 1.0f;
        public float capOverlap = 1.1f / 64;

        public int lifetime = 30;
        public int flickerFrameTime = 5;
        public float impulse = 4.0f;

        public float beamWidth = 1.0f;
        public float shieldDamageMultiplier = 0.5f;
        public float seam = -1f;
        
        public float causefireChance = -1f;
        public bool canExplode = false;
        

        public bool LightningBeam = false;
        public float LightningVariance = 3f;
        public bool StaticLightning = true;
        public int ArcCount = 1;

        public List<LaserBeamDecoration> decorations;

        public EffecterDef explosionEffect;
        public EffecterDef hitLivingEffect;
        public ThingDef beamGraphic;

        public List<string> textures;
        public List<Material> materials = new List<Material> ();

        public float AddHediffChance = 0.00f; //The default chance of adding a hediff.
        public HediffDef HediffToAdd = null;
        public bool CanResistHediff = false; //The default chance of adding a hediff.
        public float ResistHediffChance = 0.00f; //The default chance of adding a hediff.
        public StatDef ResistHediffStat = null; //The default chance of adding a hediff.

        void CreateGraphics()
        {
            if (this.graphicData.graphicClass == typeof(Graphic_Random) || this.graphicData.graphicClass == typeof(Graphic_Flicker))
            {
                for (int i = 0; i < textures.Count; i++)
                {
                    List<Texture2D> list = (from x in ContentFinder<Texture2D>.GetAllInFolder(textures[i])
                                            where !x.name.EndsWith(Graphic_Single.MaskSuffix)
                                            orderby x.name
                                            select x).ToList<Texture2D>();
                    if (list.NullOrEmpty<Texture2D>())
                    {
                        Log.Error("Collection cannot init: No textures found at path " + textures[i], false);
                    }
                    for (int ii = 0; ii < list.Count; ii++)
                    {

                        materials.Add(MaterialPool.MatFrom(textures[i] + "/" + list[ii].name, ShaderDatabase.TransparentPostLight));
                    }
                }
            }
            else
            {
                for (int i = 0; i < textures.Count; i++)
                {
                    materials.Add(MaterialPool.MatFrom(textures[i], ShaderDatabase.TransparentPostLight));
                }
            }
        }

        public Material GetBeamMaterial(int index)
        {
            if (materials.Count == 0 && textures.Count != 0)
                CreateGraphics();

            if (materials.Count == 0) {
                return null;
            }

            if (index >= materials.Count || index < 0)
                index = 0;

            return materials[index];
        }

        public bool IsWeakToShields
        {
            get { return shieldDamageMultiplier < 1f; }
        }

    }
}
