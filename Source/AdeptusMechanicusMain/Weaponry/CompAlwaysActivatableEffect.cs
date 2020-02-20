using Verse;
using RimWorld;
using CompActivatableEffect;
using UnityEngine;
using System.Collections.Generic;

namespace AdeptusMechanicus
{
    public class CompProperties_AlwaysActivatableEffect : CompActivatableEffect.CompProperties_ActivatableEffect
    {
        public CompProperties_AlwaysActivatableEffect() => this.compClass = typeof(CompAlwaysActivatableEffect);
    }

    public class CompAlwaysActivatableEffect : CompActivatableEffect.CompActivatableEffect
    {

        private Graphic graphicInt;
        private CompActivatableEffect.CompActivatableEffect.State currentState = CompActivatableEffect.CompActivatableEffect.State.Deactivated;

        public bool PowerWeapon => parent.def.tools.Any(x => x.capacities.Any(y => y.defName.Contains("OG_PowerWeapon_")));
        public bool RendingWeapon => parent.def.tools.Any(x => x.capacities.Any(y => y.defName.Contains("OG_RendingWeapon_")));
        public bool ForceWeapon => parent.def.tools.Any(x => x.capacities.Any(y => y.defName.Contains("OG_ForceWeapon_")));
        public override bool CanActivate() => GetPawn != null && GetPawn.Spawned && GetPawn.Map != null;
        
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
                    if (this.parent.TryGetComp<CompAdvancedGraphic>()!=null && this.parent.TryGetComp<CompAdvancedGraphic>() is CompAdvancedGraphic graphic)
                    {
                        this.Props.graphicData.texPath = graphic.current.path + "_Glow";
                    }
                    this.Props.graphicData.texPath = this.Props.graphicData.texPath == null ? this.parent.def.graphicData.texPath + "_Glow" : this.Props.graphicData.texPath;
                    this.Props.graphicData.texPath = this.Props.graphicData.texPath == null ? this.parent.def.graphicData.texPath + "Glow" : this.Props.graphicData.texPath;
                    Color newColor = (this.Props.graphicData.color == Color.white) ? this.parent.DrawColor : this.Props.graphicData.color;
                    Color newColorTwo = (this.Props.graphicData.colorTwo == Color.white) ? this.parent.DrawColorTwo : this.Props.graphicData.colorTwo;
                    Shader shader = (this.Props.graphicData.shaderType == null) ? this.parent.Graphic.Shader : this.Props.graphicData.shaderType.Shader;
                    this.graphicInt = this.Props.graphicData.Graphic.GetColoredVersion(shader, newColor, newColorTwo);
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
            if (!GetPawn.IsColonist)
            {
                this.currentState = CompActivatableEffect.CompActivatableEffect.State.Activated;
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
                str2 = str2.NullOrEmpty() ? str2 + " Rending Weapon" : str2 + ", Rending Weapon";
            }
            if (PowerWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " Power Weapon": str2 + ", Power Weapon";
            }
            if (ForceWeapon)
            {
                str2 = str2.NullOrEmpty() ? str2 + " Force Weapon" : str2 + ", Force Weapon";
            }
            return str2.NullOrEmpty() ? null : str+str2;
        }

        public override string GetDescriptionPart()
        {

            string str = string.Empty;
            CompEquippable c = parent.GetComp<CompEquippable>();
            if (RendingWeapon)
            {
                List<Tool> list = parent.def.tools.FindAll(x => x.capacities.Any(y => y.defName.Contains("OG_RendingWeapon_")));
                List<string> listl = new List<string>();
                list.ForEach(x => listl.Add(x.label));
                CompWeapon_MeleeSpecialRules m = parent.GetComp<CompWeapon_MeleeSpecialRules>();
                str = str + string.Format("\n Rending: has a {0} chance to ignore all armour", m.RendingChance);
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
                CompWeapon_MeleeSpecialRules m = parent.GetComp<CompWeapon_MeleeSpecialRules>();
                List<Tool> list = parent.def.tools.FindAll(x => x.capacities.Any(y => y.defName.Contains("OG_ForceWeapon_")));
                List<string> listl = new List<string>();
                list.ForEach(x => listl.Add(x.label));
                str = str + string.Format("\n Force Weapon: Attacks made by the fallowing Tools can cause Force Attacks if the wielder is a Psyker:\n{0}", listl.ToCommaList(), m.ForceWeaponKillChance);
            }
            return str;
        }
    }
}