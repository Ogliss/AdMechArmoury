using RimWorld;
using System;
using Verse;

namespace AbilityUser
{
    /*
    // Token: 0x0200000F RID: 15
    public class GenericCompAbilityItem : CompAbilityUser
    {
        // Token: 0x06000055 RID: 85 RVA: 0x00003E3C File Offset: 0x0000203C
        public override bool TryTransformPawn()
        {
            return true;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            TryTransformPawn();
        }
        public override void CompTick()
        {
            base.CompTick();
            if (this.parent.Faction==Faction.OfPlayer)
            {
                if (this.AbilityData.AllPowers.Count == 0)
                {
                    Pawn p = parent as Pawn;
                    foreach (Apparel Ap in p.apparel.WornApparel)
                    {
                        CompAbilityItem comp = Ap.TryGetComp<AbilityUser.CompAbilityItem>();
                        if (comp != null)
                        {
                            foreach (AbilityDef Pa in comp.Props.Abilities)
                            {
                                if (!this.AbilityData.TemporaryApparelPowers.Any(x => x.Def == Pa))
                                {
                                    this.AddApparelAbility(Pa);
                                }
                            }
                        }
                    }
                    foreach (var Eq in p.equipment.AllEquipmentListForReading)
                    {
                        CompAbilityItem comp = Eq.TryGetComp<AbilityUser.CompAbilityItem>();
                        if (comp != null)
                        {
                            foreach (AbilityDef Pa in comp.Props.Abilities)
                            {
                                if (!this.AbilityData.TemporaryWeaponPowers.Any(x=> x.Def==Pa))
                                {
                                    this.AddWeaponAbility(Pa);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    */
}