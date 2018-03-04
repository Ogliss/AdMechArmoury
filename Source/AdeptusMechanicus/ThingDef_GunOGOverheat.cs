using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
	public class ThingDef_GunOGOverheat : ThingWithComps
	{
		public Overheatable overheatchance
        {
			get
			{
				foreach (VerbProperties current in this.def.Verbs)
				{
					if (current.GetType() == Type.GetType("AdeptusMechanicus.VerbPropertiesOGOverheat"))
					{
						return ((VerbPropertiesOGOverheat)current).overheatchance;
					}
				}
				return Overheatable.NA;
			}
		}

		public override string GetInspectString()
		{
			string arg_21_0 = base.GetInspectString();
			string arg;
			float num;
            StatPart_Overheatable.GetOverheatable(this, out arg, out num);
			return arg_21_0 + string.Format("\r\nOverheatable: {0}\r\nChance of jam: {1}%", arg, num);
		}

		public override void ExposeData()
		{
			base.ExposeData();
			string text;
			float num;
            StatPart_Overheatable.GetOverheatable(this, out text, out num);
			Scribe_Values.Look<string>(ref text, "overheatchance", "NA", false);
		}
	}
}
