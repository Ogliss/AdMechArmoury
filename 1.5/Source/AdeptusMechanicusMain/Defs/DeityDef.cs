using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.DeityDef
    public class DeityDef : Def
	{
		public Gender gender;
		public string iconPath;
		public MemeDef relatedMeme;
		public Texture2D icon;
		public IdeoFoundation_Deity.Deity Deity()
		{
			return new IdeoFoundation_Deity.Deity()
			{
				name = this.label,
				iconPath = this.iconPath,
				type = this.description,
				gender = this.gender,
				relatedMeme = this.relatedMeme
			};
		}
	}

}
