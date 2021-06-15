using AdeptusMechanicus.ExtensionMethods;
using AdeptusMechanicus.settings;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	public class CompProperties_FactionColorable : CompProperties
    {
		public string Key = string.Empty;

		public CompProperties_FactionColorable()
		{
			compClass = typeof(CompColorableTwoFaction);
		}
	}

    public class CompColorableTwoFaction : CompColorableTwo
	{
		public CompProperties_FactionColorable Props => this.props as CompProperties_FactionColorable;
		public override Color Color
		{
			get
			{
                //	Log.Message(this.parent.LabelCap + " CompColorableTwo color active: " + active);
                if (this.FactionActive && this.ActiveFaction)
				{
					return this.factioncolor;
				}
				return base.Color;
			}
			set
			{
				if (value == this.color)
				{
					return;
				}
				this.active = true;
				this.color = value;
				//	Log.Message(this.parent.LabelCap + " CompColorableTwo color set: " + value);
				this.parent.Notify_ColorChanged();
			}
		}

		public override Color ColorTwo
		{
			get
			{
				if (this.FactionActiveTwo && this.ActiveFaction)
				{
					return this.factioncolorTwo;
				}
				return base.ColorTwo;
			}
			set
			{
				if (value == this.colorTwo)
				{
					return;
				}
				this.ActiveTwo = true;
				this.colorTwo = value;
				//	Log.Message(this.parent.LabelCap + " CompColorableTwo colorTwo set: " + value);
				this.parent.Notify_ColorChanged();
			}
		}

		public virtual bool FactionActive
		{
			get
			{
				return this.factionactive;
			}
			set
			{
				this.factionactive = value;
			}
		}

		public virtual bool FactionActiveTwo
		{
			get
			{
				return this.factionactiveTwo;
			}
			set
			{

				this.factionactiveTwo = value;
			}
		}

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
                    if (ActiveFaction)
                    {

                    }
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
								FactionDefExtension Extension = faction?.GetModExtensionFast<FactionDefExtension>() ?? null;
                                if (Extension != null)
								{
								//	Log.Message("chached faction null, checking " + faction.LabelCap + "for FactionDefExtension");
									if (Extension.factionColor.HasValue)
									{
										this.factionactive = true;
										this.factioncolor = Extension.factionColor.Value;
									}
                                    else
									{
										this.factionactive = false;
										this.factioncolor = Color.white;
									}
									if (Extension.factionColorTwo.HasValue)
									{
										this.factionactiveTwo = true;
										this.factioncolorTwo = Extension.factionColorTwo.Value;
									}
                                    else
									{
										this.factionactiveTwo = false;
										this.factioncolorTwo = Color.white;
									}
								}
							}

                        }
                    }
                }
                else
                {
				//	if (ActiveFaction) Log.Message("chached faction " + faction.LabelCap +" Faction Colour Active: " +ActiveFaction + " Colour: "+FactionActive+ " " +factioncolor +" ColourTwo: " + FactionActiveTwo + " " + factioncolorTwo);
				}
				return faction;
			}
            set
            {
				faction = value;
                if (faction != null)
				{
					FactionDefExtension Extension = faction.GetModExtensionFast<FactionDefExtension>();
					if (Extension != null)
					{
						if (Extension.factionColor.HasValue)
						{
							this.factioncolor = Extension.factionColor.Value;
							this.factionactive = true;
						}
                        else
                        {
							this.FactionActive = false;
						}
						if (Extension.factionColorTwo.HasValue)
						{
							this.factioncolorTwo = Extension.factionColorTwo.Value;
							this.factionactiveTwo = true;
						}
                        else
						{
							this.FactionActiveTwo = false;
						}
						this.ActiveFaction = true;
						this.active = true;
					//	Log.Message("set faction " + faction.LabelCap + " Colour: " + factioncolor + " ColourTwo: " + factioncolorTwo);
						return;
					}
				}
				this.ActiveFaction = false;
				this.FactionActive = false;
				this.FactionActiveTwo = false;

			}
        }
		public Apparel Apparel => this.parent as Apparel;
		public FactionDefExtension Extension => FactionDef?.GetModExtensionFast<FactionDefExtension>() ?? null;

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
					FactionDefExtension e = f.GetModExtensionFast<FactionDefExtension>();
					if (e.factionColor.HasValue || e.factionColorTwo.HasValue || !e.factionMaskTag.NullOrEmpty() || !e.factionTextureTag.NullOrEmpty())
					{
						factionsC.Add(f);
					}
				}
				return factionsC;
			}
		}

        public override void PostExposeData()
		{
			Scribe_Defs.Look<FactionDef>(ref this.faction, "faction");
			Scribe_Values.Look<bool>(ref this.activeFaction, "activeFaction");
			if ((Scribe.mode == LoadSaveMode.Saving && this.factionactive) || Scribe.mode != LoadSaveMode.Saving)
			{
				Scribe_Values.Look<Color>(ref this.factioncolor, "factioncolor", default, false);
				Scribe_Values.Look<bool>(ref this.factionactive, "factioncolorActive", false, false);
			}
			if ((Scribe.mode == LoadSaveMode.Saving && this.factionactiveTwo) || Scribe.mode != LoadSaveMode.Saving)
			{
				Scribe_Values.Look<Color>(ref this.factioncolorTwo, "factioncolorTwo", default, false);
				Scribe_Values.Look<bool>(ref this.factionactiveTwo, "factioncolorActiveTwo", false, false);
			}
			base.PostExposeData();
		}

		// Token: 0x06001746 RID: 5958 RVA: 0x00085618 File Offset: 0x00083818
		public override void PostSplitOff(Thing piece)
		{
			base.PostSplitOff(piece);
			if (this.factionactive)
			{
				if (this.factioncolor != null && this.factioncolor != Color.white)
				{
					piece.SetColor(this.factioncolor, true);
				}
				if (this.factioncolorTwo != null && this.factioncolorTwo != Color.white)
				{
					piece.SetColorTwo(this.factioncolorTwo, true);
				}
			}
		}

        public override string CompInspectStringExtra()
        {
            if (AMAMod.Dev)
            {
				string s = "Faction Colour comp.";
				s += "\ncolor: " + color + " Active: " + active + ".";
				s += "\ncolorTwo: " + colorTwo + " Active: " + activeTwo + ".";
				s += "\nfaction: " + (faction?.LabelCap.ToString() ?? "Null") + " activeFaction: " + activeFaction + ".";
				s += "\nfactioncolor: " + factioncolor + " Active: " + factionactive + ".";
				s += "\nfactioncolorTwo: " + factioncolorTwo + " Active: " + factionactiveTwo + ".";
				return s;
			}
            return base.CompInspectStringExtra();
        }

        protected FactionDef faction;
		protected bool activeFaction;

		// Token: 0x04000EA1 RID: 3745
		protected Color factioncolor = Color.white;
		protected Color factioncolorTwo = Color.white;

		// Token: 0x04000EA2 RID: 3746
		protected bool factionactive;
		protected bool factionactiveTwo;
	}

}
