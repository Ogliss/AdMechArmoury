using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.GlowerProjectileExtension
    public class GlowerProjectileExtension : DefModExtension
    {
        public bool useGraphicColor = false;
        public bool useGraphicColorTwo = false;
        public string glowMoteDef = string.Empty;
        public float glowMoteSize = 1f;
        public int glowMoteInterval = 30;

        public float GlowMoteSize => glowMoteSize;
        public int GlowMoteInterval => glowMoteInterval;
        public GraphicData glowGrahphicData;
        private Graphic _glowGrahphic;
        public ThingDef GlowMoteDef => _GlowMoteDef ??= (glowMoteDef.NullOrEmpty() ? null : DefDatabase<ThingDef>.GetNamed(glowMoteDef));
        private ThingDef _GlowMoteDef;

        public void Glow(Thing glower, Quaternion ExactRotation)
        {
            if (glowGrahphicData == null)
            {
                Material material = GlowMoteDef.graphic.MatSingle;
                if (useGraphicColor)
                {
                    material.color = glower.DrawColor;
                }
                else
                if (useGraphicColorTwo)
                {
                    material.color = glower.DrawColorTwo;
                }

                Mesh mesh2 = MeshPool.GridPlane(GlowMoteDef.graphicData.drawSize * GlowMoteSize);
                Graphics.DrawMesh(mesh2, glower.DrawPos, ExactRotation, material, 0);
            }
            else
            {
                if (_glowGrahphic == null)
                {
                    _glowGrahphic = glowGrahphicData.GraphicColoredFor(glower);
                }
                if (useGraphicColor)
                {
                    _glowGrahphic.color = glower.DrawColor;
                }
                else
                if (useGraphicColorTwo)
                {
                    _glowGrahphic.color = glower.DrawColorTwo;
                }

                Mesh mesh2 = MeshPool.GridPlane(glower.Graphic.drawSize * glowGrahphicData.drawSize);
                Graphics.DrawMesh(mesh2, glower.DrawPos, ExactRotation, _glowGrahphic.MatSingle, 0);
            }
        }
        public void Glow(Material mat, Vector3 pos, Quaternion ExactRotation)
        {

            Material glow = GlowMoteDef.graphic.MatSingle;
            if (useGraphicColor)
            {
                glow.color = mat.color;
            }
            else
            if (useGraphicColorTwo)
            {
                glow.SetColor(ShaderPropertyIDs.ColorTwo, mat.GetColorTwo());
            }
            Mesh mesh = MeshPool.GridPlane(GlowMoteDef.graphicData.drawSize * GlowMoteSize);
            Graphics.DrawMesh(mesh, pos, ExactRotation, glow, 0);

        }
    }

}
