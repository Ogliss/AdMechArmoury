﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public class AbilityButtons
    {
        public static readonly Texture2D EmptyTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
        public static readonly Texture2D FullTex = SolidColorMaterials.NewSolidColorTexture(0.5f, 0.5f, 0.5f, 0.6f);
    }
}
