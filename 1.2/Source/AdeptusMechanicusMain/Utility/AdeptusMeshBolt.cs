using System;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x0200011C RID: 284
    [StaticConstructorOnStartup]
    public class AdeptusMeshBolt : Thing
    {
        // Token: 0x06000764 RID: 1892 RVA: 0x000674BD File Offset: 0x000656BD
        public AdeptusMeshBolt(IntVec3 hitThing, Vector3 origin, Material _mat)
        {
            this.hitThing = hitThing;
            this.origin = origin;
            this.mat = _mat;
        }

        // Token: 0x06000765 RID: 1893 RVA: 0x000674DC File Offset: 0x000656DC
        public void CreateBolt()
        {
            Vector3 vector;
            vector.x = (float)this.hitThing.x;
            vector.y = (float)this.hitThing.y;
            vector.z = (float)this.hitThing.z;
            this.direction = Quaternion.LookRotation((vector - this.origin).normalized);
            float distance = Vector3.Distance(this.origin, vector);
            this.boltMesh = AdeptusMeshMaker.NewBoltMesh(distance, 6f);
            Graphics.DrawMesh(this.boltMesh, this.origin, this.direction, this.mat, 0);
        }
        // Token: 0x06000765 RID: 1893 RVA: 0x000674DC File Offset: 0x000656DC
        public Mesh GetBolt(IntVec3 hitThing, Vector3 origin, Material _mat)
        {
            Vector3 vector;
            vector.x = (float)hitThing.x;
            vector.y = (float)hitThing.y;
            vector.z = (float)hitThing.z;
            this.direction = Quaternion.LookRotation((vector - origin).normalized);
            float distance = Vector3.Distance(origin, vector);
            this.boltMesh = AdeptusMeshMaker.NewBoltMesh(distance, 6f);
            return this.boltMesh;
        //    Graphics.DrawMesh(this.boltMesh, this.origin, this.direction, this.mat, 0);
        }

        // Token: 0x06000766 RID: 1894 RVA: 0x00067580 File Offset: 0x00065780
        public void CreateFadedBolt(int magnitude)
        {
            Vector3 vector;
            vector.x = (float)this.hitThing.x;
            vector.y = (float)this.hitThing.y;
            vector.z = (float)this.hitThing.z;
            this.direction = Quaternion.LookRotation((vector - this.origin).normalized);
            float distance = Vector3.Distance(this.origin, vector);
            this.boltMesh = AdeptusMeshMaker.NewBoltMesh(distance, 6f);
            Graphics.DrawMesh(this.boltMesh, this.origin, this.direction, FadedMaterialPool.FadedVersionOf(this.mat, (float)magnitude), 0);
        }
        
        private IntVec3 hitThing;

        private Vector3 origin;

        private Mesh boltMesh;

        private Quaternion direction;

        private Material mat;
    }
}
