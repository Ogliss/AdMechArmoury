using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	public class CompProperties_FactionColorable : CompProperties
    {

    }

    // Token: 0x0200031C RID: 796
    public class CompColorableTwoFaction : CompColorableTwo
	{
		public virtual bool ActiveFaction
        {
            get
            {
				return activeFaction;
            }
            set
            {
				activeFaction = value;
            }
        }

		public FactionDef FactionDef
        {
            get
            {
                if (faction == null && ActiveFaction)
                {
				//	Log.Message("chached faction null, checking apparel");
                    if (Apparel != null)
					{
					//	Log.Message("chached faction null, checking wearer");
						if (Apparel.Wearer != null)
						{
						//	Log.Message("chached faction null, checking wearer faction");
							if (Apparel.Wearer.Faction != null && Apparel.Wearer.Faction != Faction.OfPlayer)
                            {
								faction = Apparel.Wearer.Faction.def;
							//	Log.Message("chached faction null, checking " + faction.LabelCap + "for FactionDefExtension");
								FactionDefExtension Extension = faction?.GetModExtension<FactionDefExtension>() ?? null;
                                if (Extension != null)
								{
								//	Log.Message("chached faction null, checking " + faction.LabelCap + "for FactionDefExtension");
									if (Extension.factionColor.HasValue)
									{
										this.Color = Extension.factionColor.Value;
									}
									if (Extension.factionColorTwo.HasValue)
									{
										this.ColorTwo = Extension.factionColorTwo.Value;
									}
								}
							}

                        }
                    }
                }
                else
                {
					if (ActiveFaction) Log.Message("chached faction " + faction.LabelCap);
				}
				return faction;
			}
            set
            {
				faction = value;
                if (faction != null)
				{
					FactionDefExtension Extension = faction?.GetModExtension<FactionDefExtension>() ?? null;
					if (Extension != null)
					{
						if (Extension.factionColor.HasValue)
						{
							this.Color = Extension.factionColor.Value;
						}
						if (Extension.factionColorTwo.HasValue)
						{
							this.ColorTwo = Extension.factionColorTwo.Value;
						}
					}
					this.ActiveFaction = true;
				}
                else
                {
					this.ActiveFaction = false;
					this.Active = false;
					this.ActiveTwo = false;
                }

			}
        }
		public Apparel Apparel => this.parent as Apparel;
		public FactionDefExtension Extension => FactionDef?.GetModExtension<FactionDefExtension>() ?? null;

		public List<FactionDef> ColouredDefs
        {
            get
            {

				List<FactionDef> factions = new List<FactionDef>();
				for (int i = 0; i < DefDatabase<FactionDef>.AllDefsListForReading.Count; i++)
				{
					if (DefDatabase<FactionDef>.AllDefsListForReading[i].HasModExtension<FactionDefExtension>())
					{
						factions.Add(DefDatabase<FactionDef>.AllDefsListForReading[i]);
					}
				}

				List<FactionDef> factionsC = new List<FactionDef>();
				for (int i = 0; i < factions.Count; i++)
				{
					FactionDef f = factions[i];
					FactionDefExtension e = f.GetModExtension<FactionDefExtension>();
					if (e.factionColor.HasValue || e.factionColorTwo.HasValue || !e.factionMaskTag.NullOrEmpty() || !e.factionTextureTag.NullOrEmpty())
					{
						factionsC.Add(f);
					}
				}
				return factionsC;
			}
		}
		public override void Initialize(CompProperties props)
		{
			this.props = props;
			/*
            if (Apparel != null)
			{
				Log.Message(this.parent + " is Apparel");
				if (Apparel.Wearer != null)
				{
					Log.Message(this.parent + " is Worn Apparel");
					if (Apparel.Wearer.Faction != null)
					{
						Log.Message(this.parent + " Wearer Faction is "+ Apparel.Wearer.Faction.def.defName);
						if (Apparel.Wearer.Faction != RimWorld.Faction.OfPlayer)
                        {
							faction = Apparel.Wearer.Faction.def;
							Log.Message(this.parent + " FactionDef set to Wearer faction " + this.faction.defName);

						}
                    }
                }
            }
			if (FactionDef != null)
			{
				FactionDefExtension extension = FactionDef.GetModExtension<FactionDefExtension>();
				if (extension != null)
				{;
					if (extension.factionColor.HasValue)
					{
						this.Color = extension.factionColor.Value;
						Log.Message(this.parent + " Primary Color set to factionColor " + this.Color);
					}
					if (extension.factionColorTwo.HasValue)
					{
						this.ColorTwo = extension.factionColorTwo.Value;
						Log.Message(this.parent + " Secondry Color set to factionColorTwo " + this.ColorTwo);
					}
				}
			}
			else
			if (this.parent.def.colorGenerator != null && (this.parent.Stuff == null || this.parent.Stuff.stuffProps.allowColorGenerators))
			{
                if (!this.active)
				{
					this.Color = this.parent.def.colorGenerator.NewRandomizedColor();
					this.active = true;
					Log.Message(this.parent + " Primary Color set to NewRandomizedColor " + this.Color);
				}
                if (!this.activeTwo)
				{
					this.ColorTwo = this.parent.def.colorGenerator.NewRandomizedColor();
					this.activeTwo = true;
					Log.Message(this.parent + " Secondry Color set to NewRandomizedColor " + this.ColorTwo);
				}
			}
			*/
		}

		// Token: 0x06001745 RID: 5957 RVA: 0x000855C8 File Offset: 0x000837C8
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Defs.Look<FactionDef>(ref this.faction, "faction");
			Scribe_Values.Look<bool>(ref this.activeFaction, "activeFaction");
		}
		protected FactionDef faction;
		protected bool activeFaction;
	}

}
