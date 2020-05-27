using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace AdeptusMechanicus
{
    // Token: 0x0200011B RID: 283
    [StaticConstructorOnStartup]
    public static class AM_MeshMaker
    {
        // Token: 0x0600075F RID: 1887 RVA: 0x000670C4 File Offset: 0x000652C4
        public static Mesh NewBoltMesh(float distance, float amplitude)
        {
            AM_MeshMaker.lightningTop = new Vector2(Rand.Range(-0.2f, 0.2f), distance);
            AM_MeshMaker.MakeVerticesBase();
            bool flag = amplitude > 0f;
            if (flag)
            {
                AM_MeshMaker.PeturbVerticesRandomly(amplitude);
            }
            AM_MeshMaker.DoubleVertices();
            return AM_MeshMaker.MeshFromVerts();
        }

        // Token: 0x06000760 RID: 1888 RVA: 0x00067118 File Offset: 0x00065318
        private static void MakeVerticesBase()
        {
            int num = (int)Math.Ceiling((double)((Vector2.zero - AM_MeshMaker.lightningTop).magnitude / 0.25f));
            Vector2 b = AM_MeshMaker.lightningTop / (float)num;
            AM_MeshMaker.verts2D = new List<Vector2>();
            Vector2 vector = Vector2.zero;
            for (int i = 0; i < num; i++)
            {
                AM_MeshMaker.verts2D.Add(vector);
                vector += b;
            }
        }

        // Token: 0x06000761 RID: 1889 RVA: 0x00067194 File Offset: 0x00065394
        private static void PeturbVerticesRandomly(float amplitude)
        {
            Perlin perlin = new Perlin(0.0070000002160668373, 2.0, 0.5, 6, Rand.Range(0, int.MaxValue), QualityMode.High);
            List<Vector2> list = AM_MeshMaker.verts2D.ListFullCopy<Vector2>();
            AM_MeshMaker.verts2D.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                float d = amplitude * (float)perlin.GetValue((double)i, 0.0, 0.0);
                Vector2 item = list[i] + d * Vector2.right;
                AM_MeshMaker.verts2D.Add(item);
            }
        }

        // Token: 0x06000762 RID: 1890 RVA: 0x00067244 File Offset: 0x00065444
        private static void DoubleVertices()
        {
            List<Vector2> list = AM_MeshMaker.verts2D.ListFullCopy<Vector2>();
            Vector3 vector = default(Vector3);
            Vector2 a = default(Vector2);
            AM_MeshMaker.verts2D.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                bool flag = i <= list.Count - 2;
                bool flag2 = flag;
                if (flag2)
                {
                    vector = Quaternion.AngleAxis(90f, Vector3.up) * (list[i] - list[i + 1]);
                    a = new Vector2(vector.y, vector.z);
                    a.Normalize();
                }
                Vector2 item = list[i] - 1f * a;
                Vector2 item2 = list[i] + 1f * a;
                AM_MeshMaker.verts2D.Add(item);
                AM_MeshMaker.verts2D.Add(item2);
            }
        }

        // Token: 0x06000763 RID: 1891 RVA: 0x00067348 File Offset: 0x00065548
        private static Mesh MeshFromVerts()
        {
            Vector3[] array = new Vector3[AM_MeshMaker.verts2D.Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new Vector3(AM_MeshMaker.verts2D[i].x, 0f, AM_MeshMaker.verts2D[i].y);
            }
            float num = 0f;
            Vector2[] array2 = new Vector2[AM_MeshMaker.verts2D.Count];
            for (int j = 0; j < AM_MeshMaker.verts2D.Count; j += 2)
            {
                array2[j] = new Vector2(0f, num);
                array2[j + 1] = new Vector2(1f, num);
                num += 0.04f;
            }
            int[] array3 = new int[AM_MeshMaker.verts2D.Count * 3];
            for (int k = 0; k < AM_MeshMaker.verts2D.Count - 2; k += 2)
            {
                int num2 = k * 3;
                array3[num2] = k;
                array3[num2 + 1] = k + 1;
                array3[num2 + 2] = k + 2;
                array3[num2 + 3] = k + 2;
                array3[num2 + 4] = k + 1;
                array3[num2 + 5] = k + 3;
            }
            return new Mesh
            {
                vertices = array,
                uv = array2,
                triangles = array3,
                name = "MeshFromVerts()"
            };
        }

        // Token: 0x040006B0 RID: 1712
        private static List<Vector2> verts2D;

        // Token: 0x040006B1 RID: 1713
        private static Vector2 lightningTop;
    }
}
