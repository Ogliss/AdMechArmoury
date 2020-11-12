using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.ColorGeneratorTwo_Options
    public class TwoColorGenerator_Options : ColorGenerator_Options
	{
		public Color ExemplaryColorTwo
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

		public Color NewRandomizedColorTwo()
		{
			return this.optionsTwo.RandomElementByWeight((ColorOption pi) => pi.weight).RandomizedColor();
		}

		public List<ColorOption> optionsTwo = new List<ColorOption>();
	}
}
