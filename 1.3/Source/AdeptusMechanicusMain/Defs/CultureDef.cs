using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;
using Verse.Grammar;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.CultureDef
    public class CultureDef : RimWorld.CultureDef
	{

		public DeityProperties deities = new DeityProperties();
		public List<RoleRenamer> rolesNames = new List<RoleRenamer>();
		public List<RitualRenamer> ritualNames = new List<RitualRenamer>();
		public List<string> preferredApparelTags = new List<string>();
		public RulePack generalRules;
		public class DeityProperties
        {
			public bool randomizeOrder = false;
			public int max = -1;
			public int min = 0;
			public List<DeityDef> requiredDeities = new List<DeityDef>();
			public List<DeityDef> possibleDeities = new List<DeityDef>();
		}

	}
	public class RoleRenamer
	{
		public PreceptDef role;
		public RulePackDef rulePack;
	}
	public class RitualRenamer
	{
		public PreceptDef ritual;
		public RulePackDef rulePack;
	}

}
