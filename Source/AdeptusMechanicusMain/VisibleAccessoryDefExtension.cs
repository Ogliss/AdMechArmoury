using System;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000020 RID: 32
    public class VisibleAccessoryDefExtension : DefModExtension
    {
        // Token: 0x06000059 RID: 89 RVA: 0x000059D8 File Offset: 0x00003BD8
        public void Validate()
        {
            bool flag = this.validated;
            if (!flag)
            {
                bool flag2 = this.order < 1;
                if (flag2)
                {
                    Log.Error("CE detected VisibleAccessoryDefExtension with order lower than 1, viable values are 1-4. Clamping to 1", false);
                    this.order = 1;
                }
                else
                {
                    bool flag3 = this.order > 4;
                    if (flag3)
                    {
                        Log.Error("CE detected VisibleAccessoryDefExtension with order higher than 4, viable values are 1-4. Clamping to 4", false);
                        this.order = 4;
                    }
                }
                this.validated = true;
            }
        }

        // Token: 0x0400006C RID: 108
        public int order = 1;
        public int sublayer = 0;
        public bool northtop = false;
        public float NorthOffset = 0f;
        public float SouthOffset = 0f;
        public float EastOffset = 0f;
        public float WestOffset = 0f;
        // Token: 0x0400006D RID: 109
        private bool validated = false;
    }
}
