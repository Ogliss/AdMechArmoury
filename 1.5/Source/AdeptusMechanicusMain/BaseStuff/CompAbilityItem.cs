using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class CompAbilityItem : CompWearable
    {
        public List<PawnAbility> Abilities = new List<PawnAbility>()
            ; // should these exist or only in CompAbilityUser.temporaryWeaponPowers?

        public CompAbilityUser AbilityUserTarget = null;

        private AbilityData abilityData;
        private Graphic Overlay;

        public CompProperties_AbilityItem Props => (CompProperties_AbilityItem) props;

        public CompProperties_AbilityUser PropsAU => (CompProperties_AbilityUser)props;
        public void GetOverlayGraphic()
        {
            // Cool effect if you uncomment. Places this graffic behind the item.
            // this.Overlay = GraphicDatabase.Get<Graphic_Single>("UI/Glow_Corrupt", ShaderDatabase.MetaOverlay, Vector2.one, Color.white);
        }

        public virtual AbilityData AbilityData
        {
            get
            {
                if (abilityData == null)
                    abilityData = new AbilityData(this);
                return abilityData;
            }
        }
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (parent.def.tickerType == TickerType.Never)
                parent.def.tickerType = TickerType.Rare;
            base.PostSpawnSetup(respawningAfterLoad);

            GetOverlayGraphic();
            Find.TickManager.RegisterAllTickabilityFor(parent);
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            if (Overlay == null) Log.Message("NoOverlay");
            if (Overlay != null)
            {
                var drawPos = parent.DrawPos;
                drawPos.y = Altitudes.AltitudeFor(AltitudeLayer.MoteOverhead);
                var s = new Vector3(2.0f, 2.0f, 2.0f);
                var matrix = default(Matrix4x4);
                matrix.SetTRS(drawPos, Quaternion.AngleAxis(0, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, Overlay.MatSingle, 0);
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosWorn()
        {
            for (var i = 0; i < AbilityData?.AllPowers.Count; i++)
            {
                var ability = AbilityData?.AllPowers[i];
                if (ability.ShouldShowGizmo())
                    yield return ability.GetGizmo();
            }
            base.CompGetGizmosWorn();
        }
        //        public override void CompTick() { }
        //        public override void CompTickRare() { }

        public override void PostExposeData()
        {
            base.PostExposeData();
        }

        public override string GetDescriptionPart()
        {
            var str = string.Empty;

            if (Props.Abilities.Count == 1)
                str += "Item Ability:";
            else if (Props.Abilities.Count > 1)
                str += "Item Abilities:";

            foreach (var pa in Props.Abilities)
            {
                str += "\n\n";
                str += pa.label.CapitalizeFirst() + " - ";
                str += pa.GetDescription();
            }
            return str;
        }
    }
}