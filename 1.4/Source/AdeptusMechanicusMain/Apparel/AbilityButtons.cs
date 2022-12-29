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
    public class AbilityButtons
    {
        public static readonly Texture2D EmptyTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
        public static readonly Texture2D FullTex = SolidColorMaterials.NewSolidColorTexture(0.5f, 0.5f, 0.5f, 0.6f);
    }
    public class ColorGenerator_Single : ColorGenerator
    {
        // Token: 0x060005FA RID: 1530 RVA: 0x000208B4 File Offset: 0x0001EAB4
        public override Color NewRandomizedColor()
        {
            return this.color;
        }

        // Token: 0x04000377 RID: 887
        public Color color;
    }
}
