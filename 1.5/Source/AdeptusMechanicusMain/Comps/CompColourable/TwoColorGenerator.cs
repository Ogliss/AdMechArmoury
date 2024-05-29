using System;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000081 RID: 129
    public abstract class TwoColorGenerator : ColorGenerator
	{
		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x060004B8 RID: 1208 RVA: 0x00017CFD File Offset: 0x00015EFD
		public virtual Color ExemplaryColorTwo
		{
			get
			{
				Rand.PushState(764543439);
				Color result = this.NewRandomizedColorTwo();
				Rand.PopState();
				return result;
			}
		}

		// Token: 0x060004B9 RID: 1209
		public abstract Color NewRandomizedColorTwo();
	}
}
