using Verse;
using RimWorld;
using AdeptusMechanicus;
using UnityEngine;
using System.Collections.Generic;
using OgsCompActivatableEffect;
using AdvancedGraphics;

namespace AdeptusMechanicus
{
    public class CompProperties_AlwaysActivatableEffect : CompProperties_ActivatableEffect
    {
        public CompProperties_AlwaysActivatableEffect() => this.compClass = typeof(CompAlwaysActivatableEffect);
    }

    public class CompAlwaysActivatableEffect : CompActivatableEffect
    {

        private Graphic graphicInt;
        private OgsCompActivatableEffect.CompActivatableEffect.State currentState = OgsCompActivatableEffect.CompActivatableEffect.State.Deactivated;

        public bool PowerWeapon => parent.def.tools.Any(x => x.capacities.Any(y => y.defName.Contains("OG_PowerWeapon_")));
        public bool RendingWeapon => parent.def.tools.Any(x => x.capacities.Any(y => y.defName.Contains("OG_RendingWeapon_")));
        public bool ForceWeapon => parent.def.tools.Any(x => x.capacities.Any(y => y.defName.Contains("OG_ForceWeapon_")));
        public bool Witchblade => parent.def.tools.Any(x => x.capacities.Any(y => y.defName.Contains("OG_WitchbladeWeapon_")));
        public override bool CanActivate() => GetPawn != null && GetPawn.Spawned && GetPawn.Map != null;

        private CompAdvancedGraphic advancedGraphic;
        public CompAdvancedGraphic AdvancedGraphic;

        private string texPath;
        public string TexPath
        {
            get
            {
                if (texPath.NullOrEmpty())
                {
                    string tex = this.Props.graphicData.texPath;
                    if (tex.NullOrEmpty())
                    {
                        tex = this.parent.Graphic.path;
                        if (this.parent.TryGetComp<CompAdvancedGraphic>() != null && this.parent.TryGetComp<CompAdvancedGraphic>() is CompAdvancedGraphic graphic)
                        {
                            tex = graphic.current.path;
                        }
                        if (tex.NullOrEmpty())
                        {
                            tex = this.parent.def.graphicData.texPath;
                        }
                    }
                    texPath = tex + "_Glow";
                }
                return texPath;
            }
        }

        private GraphicData intGraphicData;
        private GraphicData GraphicData
        {
            get
            {
                if (intGraphicData == null)
                {
                    intGraphicData = new GraphicData();
                    intGraphicData.CopyFrom(this.Props.graphicData);
                    intGraphicData.texPath = this.TexPath;
                }
                return intGraphicData;
            }
        }

        public override Graphic Graphic
        {
            get
            {
                bool flag = this.graphicInt == null;
                if (flag)
                {
                    bool flag2 = this.Props.graphicData == null;
                    if (flag2)
                    {
                        Log.ErrorOnce(this.parent.def + " has no SecondLayer graphicData but we are trying to access it.", 764532, false);
                        return BaseContent.BadGraphic;
                    }
                    Color newColor = (GraphicData.color == Color.white) ? this.parent.DrawColor : GraphicData.color;
                    Color newColorTwo = (GraphicData.colorTwo == Color.white) ? this.parent.DrawColorTwo : GraphicData.colorTwo;
                    Shader shader = (GraphicData.shaderType == null) ? this.parent.Graphic.Shader : GraphicData.shaderType.Shader;
                    this.graphicInt = GraphicData.Graphic.GetColoredVersion(shader, newColor, newColorTwo);
                    this.graphicInt = this.PostGraphicEffects(this.graphicInt);
                }
                return this.graphicInt;
            }
            set
            {
                this.graphicInt = value;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            if (GetPawn!=null && !GetPawn.IsColonist)
            {
                this.currentState = CompActivatableEffect.State.Activated;
            }
        }

        public override void Activate()
        {
            base.Activate();
        }
        
        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override string CompInspectStringExtra()
        {
            string str = "Special Rules:";
            string str2 = string.Empty;
            if (RendingWeapon)
            {
                str2 = str2.NullOrEmpty() ? str + " Rending Weapon" : str + ", Rending Weapon";
            }
            if (PowerWeapon)
            {
                str2 = str2.NullOrEmpty() ? str + " Power Weapon": str + ", Power Weapon";
            }
            if (ForceWeapon)
            {
                str2 = str2.NullOrEmpty() ? str + "Force Weapon" : str + ", Force Weapon";
            }
            if (Witchblade)
            {
                str2 = str2.NullOrEmpty() ? str + " Witchblade" : str + ", Witchblade";
            }
            return str2.NullOrEmpty() ? null : str+str2;
        }

        public override string GetDescriptionPart()
        {

            string str = string.Empty;
            CompWeapon_MeleeSpecialRules m = parent.TryGetComp<CompWeapon_MeleeSpecialRules>();
            CompEquippable c = parent.GetComp<CompEquippable>();
            if (RendingWeapon)
            {
                if (m != null)
                {
                    List<Tool> list = parent.def.tools.FindAll(x => x.capacities.Any(y => y.defName.Contains("OG_RendingWeapon_")));
                    List<string> listl = new List<string>();
                    list.ForEach(x => listl.Add(x.label));
                    str = str + string.Format("\n Rending: has a {0} chance to ignore all armour", m.RendingChance);
                }
            }
            if (PowerWeapon)
            {
                List<Tool> list = parent.def.tools.FindAll(x => x.capacities.Any(y => y.defName.Contains("OG_PowerWeapon_")));
                List<string> listl = new List<string>();
                list.ForEach(x => listl.Add(x.label));
                str = str + string.Format("\n Power Weapon: Attacks made by the following Tools Ignore all armour:\n{0} ", listl.ToCommaList());
            }
            if (ForceWeapon)
            {
                if (m != null)
                {
                    List<Tool> list = parent.def.tools.FindAll(x => x.capacities.Any(y => y.defName.Contains("OG_ForceWeapon_")));
                    List<string> listl = new List<string>();
                    list.ForEach(x => listl.Add(x.label));
                    str = str + string.Format("\n Force Weapon: Attacks made by the following Tools can cause Force Attacks if the wielder is a Psyker:\n{0}", listl.ToCommaList(), m.ForceWeaponKillChance);
                }
            }

            if (Witchblade)
            {
                if (m != null)
                {
                    List<Tool> list = parent.def.tools.FindAll(x => x.capacities.Any(y => y.defName.Contains("OG_WitchbladeWeapon_")));
                    List<string> listl = new List<string>();
                    list.ForEach(x => listl.Add(x.label));
                    str = str + string.Format("\n Witchblade: Attacks made by the following Tools can cause increased damage if the wielder is a Psyker:\n{0}", listl.ToCommaList());
                }
            }

            return str;
        }
    }
}