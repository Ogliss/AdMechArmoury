using RimWorld;
using System;
using System.Collections.Generic;
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

    // Token: 0x0200031C RID: 796
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
				if (!this.active)
				{
					return this.parent.def.graphicData.color;
				}
				return this.color;
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
				//	Log.Message(this.parent.LabelCap + " CompColorableTwo colorTwo active: " + ActiveTwo);
				if (this.FactionActiveTwo && this.ActiveFaction)
				{
					return this.factioncolorTwo;
				}
				return base.ColorTwo;
				if (!this.activeTwo)
				{
					return this.parent.def.graphicData.colorTwo;
				}
				return this.colorTwo;
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

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x06001743 RID: 5955 RVA: 0x00085558 File Offset: 0x00083758
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
								FactionDefExtension Extension = faction?.GetModExtension<FactionDefExtension>() ?? null;
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
					if (ActiveFaction) Log.Message("chached faction " + faction.LabelCap +" Faction Colour Active: " +ActiveFaction + " Colour: "+FactionActive+ " " +factioncolor +" ColourTwo: " + FactionActiveTwo + " " + factioncolorTwo);
				}
				return faction;
			}
            set
            {
				faction = value;
                if (faction != null)
				{
					FactionDefExtension Extension = faction.GetModExtension<FactionDefExtension>();
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
						Log.Message("set faction " + faction.LabelCap + " Colour: " + factioncolor + " ColourTwo: " + factioncolorTwo);
						return;
					}
				}
				this.ActiveFaction = false;
				this.FactionActive = false;
				this.FactionActiveTwo = false;

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

        /*
		public override void Initialize(CompProperties props)
		{
			this.props = props;
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
			base.Initialize(props);
		}
		*/

        // Token: 0x06001745 RID: 5957 RVA: 0x000855C8 File Offset: 0x000837C8
        public override void PostExposeData()
		{
			Scribe_Defs.Look<FactionDef>(ref this.faction, "faction");
			Scribe_Values.Look<bool>(ref this.activeFaction, "activeFaction");
			if ((Scribe.mode == LoadSaveMode.Saving && this.factionactive) || Scribe.mode != LoadSaveMode.Saving)
			{
				Scribe_Values.Look<Color>(ref this.factioncolor, "factioncolor", default(Color), false);
				Scribe_Values.Look<bool>(ref this.factionactive, "factioncolorActive", false, false);
			}
			if ((Scribe.mode == LoadSaveMode.Saving && this.factionactiveTwo) || Scribe.mode != LoadSaveMode.Saving)
			{
				Scribe_Values.Look<Color>(ref this.factioncolorTwo, "factioncolorTwo", default(Color), false);
				Scribe_Values.Look<bool>(ref this.factionactiveTwo, "factioncolorActiveTwo", false, false);
			}
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
