using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    // Token: 0x02000020 RID: 32
    public class FloatingPawnExtension : DefModExtension
    {
        public bool isFloater = false;
        public bool canCrossWater = false;
        public bool useZOffset = false;
        public float zOffset = 0.0f;
        public bool useShadow = false;
        public float zOffsetShadow = 0.75f;
        public string shadow = "Things/Skyfaller/SkyfallerShadowCircle";
        public Vector2 shadowSize = Vector2.zero;

        public Vector2 ShadowSize(Pawn pawn)
        {
            if (shadowSize != Vector2.zero)
            {
                return shadowSize;
            }
            return pawn.Graphic.drawSize;
        }

        public void DrawDropSpotShadow(Pawn pawn, Vector3 __instance)
        {
            Material shadowMaterial = this.ShadowMaterial;
            if (shadowMaterial == null)
            {
                return;
            }
            CompFloating.DrawDropSpotShadow(__instance, pawn.Rotation, shadowMaterial, shadowSize, 150);
        }
        // Token: 0x06004F2F RID: 20271 RVA: 0x001AAD60 File Offset: 0x001A8F60
        public static void DrawDropSpotShadow(Vector3 center, Rot4 rot, Material material, Vector2 shadowSize, int ticksToImpact)
        {
            /*
            if (rot == Rot4.East || rot == Rot4.West)
            {
                Gen.Swap<float>(ref shadowSize.x, ref shadowSize.y);
            }
            */
            ticksToImpact = Mathf.Max(ticksToImpact, 0);
            Vector3 pos = center;
            pos.y = AltitudeLayer.Shadows.AltitudeFor();
            float num = 1f + (float)ticksToImpact / 100f;
            Vector3 s = new Vector3(shadowSize.x, 1f, shadowSize.y);
            //    Vector3 s = rot == Rot4.East || rot == Rot4.West ? new Vector3(shadowSize.y, 1f, shadowSize.x) : new Vector3(shadowSize.x, 1f, shadowSize.y);
            Color white = Color.white;
            if (ticksToImpact > 150)
            {
                white.a = Mathf.InverseLerp(200f, 150f, (float)ticksToImpact);
            }
            shadowPropertyBlock.SetColor(ShaderPropertyIDs.Color, white);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(pos, rot.AsQuat, s);
            Graphics.DrawMesh(MeshPool.plane10Back, matrix, material, 0, null, 0, shadowPropertyBlock);
        }
        private Material ShadowMaterial
        {
            get
            {
                if (this.cachedShadowMaterial == null && !shadow.NullOrEmpty())
                {
                    this.cachedShadowMaterial = MaterialPool.MatFrom(shadow, ShaderDatabase.Transparent);
                }
                return this.cachedShadowMaterial;
            }
        }
        private Material cachedShadowMaterial;
        private static MaterialPropertyBlock shadowPropertyBlock = new MaterialPropertyBlock();
    }
}
