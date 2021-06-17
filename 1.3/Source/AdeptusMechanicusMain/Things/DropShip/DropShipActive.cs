using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    // Token: 0x02001596 RID: 5526
    public class DropShipActive : ActiveDropPod, IActiveDropPod, IThingHolder
	{
		public Thing Dropship => this.contents.innerContainer.FirstOrFallback(x=> x.TryGetCompFast<CompDropship>()!=null);
		public CompDropship Cargo => Dropship?.TryGetCompFast<CompDropship>();
		/*
		public new ActiveDropPodInfo Contents
		{
			get
			{
				return ((ActiveDropPod)this.innerContainer[0]).Contents;
			}
			set
			{
				((ActiveDropPod)this.innerContainer[0]).Contents = value;
			}
		}
		*/
		// Token: 0x0600792A RID: 31018 RVA: 0x00239AC8 File Offset: 0x00237CC8
		public new void PodOpen()
		{
			Map map = base.Map;
			if (this.contents.despawnPodBeforeSpawningThing)
			{
				this.DeSpawn(DestroyMode.Vanish);
			}
			if (Dropship != null)
			{
				this.contents.innerContainer.Remove(Dropship);
			}
			else return;
			GenSpawn.Spawn(Dropship, base.Position, map, this.contents.setRotation.Value, this.contents.spawnWipeMode.Value, false);
			for (int i = this.contents.innerContainer.Count - 1; i >= 0; i--)
			{
				Thing thing = this.contents.innerContainer[i];
				if (Dropship.TryGetCompFast<CompTransporter>() !=null )
				{
					CompDropship transporter = Dropship.TryGetCompFast<CompDropship>();
					transporter.Transporter.innerContainer.TryAddOrTransfer(thing);
				}

			}
			this.contents.innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
		//	SoundDefOf.DropPod_Open.PlayOneShot(new TargetInfo(base.Position, map, false));
			this.Destroy(DestroyMode.Vanish);
		}

		// Token: 0x04004DE3 RID: 19939
		public new int age;

	}
}
