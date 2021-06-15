using Verse;
using RimWorld;
using AdeptusMechanicus;
using UnityEngine;
using OgsCompActivatableEffect;

namespace AdeptusMechanicus
{
    public class CompProperties_PowerWeaponActivatableEffect : CompProperties_AlwaysActivatableEffect
    {
        public CompProperties_PowerWeaponActivatableEffect() => this.compClass = typeof(CompPowerWeaponActivatableEffect);
    }

    public class CompPowerWeaponActivatableEffect : CompAlwaysActivatableEffect
    {
        public override void PostDraw()
        {

        }
    }
}