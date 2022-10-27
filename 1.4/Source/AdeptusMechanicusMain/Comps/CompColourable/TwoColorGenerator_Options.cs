using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	// AdeptusMechanicus.TwoColorGenerator_Options
	public class TwoColorGenerator_Options : ColorGenerator_Options
	{
		public override Color ExemplaryColor
		{
			get
			{
				ColorOption colorOption = null;
				for (int i = 0; i < this.options.Count; i++)
				{
					if (colorOption == null || this.options[i].weight > colorOption.weight)
					{
						colorOption = this.options[i];
					}
				}
				if (colorOption == null)
				{
					return Color.white;
				}
				if (colorOption.only.a >= 0f)
				{
					return colorOption.only;
				}
				return new Color((colorOption.min.r + colorOption.max.r) / 2f, (colorOption.min.g + colorOption.max.g) / 2f, (colorOption.min.b + colorOption.max.b) / 2f, (colorOption.min.a + colorOption.max.a) / 2f);
			}
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x00017EF9 File Offset: 0x000160F9
		public override Color NewRandomizedColor()
		{
			return this.options.RandomElementByWeight((ColorOption pi) => pi.weight).RandomizedColor();
		}

		public virtual Color ExemplaryColorTwo
		{
			get
			{
				ColorOption colorOption = null;
				for (int i = 0; i < this.optionsTwo.Count; i++)
				{
					if (colorOption == null || this.optionsTwo[i].weight > colorOption.weight)
					{
						colorOption = this.optionsTwo[i];
					}
				}
				if (colorOption == null)
				{
					return Color.white;
				}
				if (colorOption.only.a >= 0f)
				{
					return colorOption.only;
				}
				return new Color((colorOption.min.r + colorOption.max.r) / 2f, (colorOption.min.g + colorOption.max.g) / 2f, (colorOption.min.b + colorOption.max.b) / 2f, (colorOption.min.a + colorOption.max.a) / 2f);
			}
		}

		public virtual Color NewRandomizedColorTwo()
		{
			return this.optionsTwo.RandomElementByWeight((ColorOption pi) => pi.weight).RandomizedColor();
		}

		public List<ColorOption> optionsTwo = new List<ColorOption>();
		public List<ColorPair> optionsPairs = new List<ColorPair>();

		public struct ColorPair
        {
			public float weight;
			public ColorOption colorOne;
			public ColorOption colorTwo;
        }
	}

}
