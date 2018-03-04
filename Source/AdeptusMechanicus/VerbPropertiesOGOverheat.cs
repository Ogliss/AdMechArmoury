using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
	public class VerbPropertiesOGOverheat : VerbProperties
	{
		public Overheatable overheatchance;

		public override string ToString()
		{
			string str;
			if (!this.label.NullOrEmpty())
			{
				str = this.label;
			}
			else
			{
				str = string.Concat(new object[]
				{
					"range=",
					this.range,
					", projectile=",
					(this.defaultProjectile == null) ? "null" : this.defaultProjectile.defName,
                    ", overheatchance=",
					this.overheatchance.ToString()
				});
			}
			return "VerbProperties(" + str + ")";
		}
	}
}
