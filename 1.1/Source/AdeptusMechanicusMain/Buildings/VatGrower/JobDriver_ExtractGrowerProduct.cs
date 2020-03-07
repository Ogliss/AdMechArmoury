using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
    /// <summary>
    /// Extracts the product out of a Grower.
    /// </summary>
    public class JobDriver_ExtractGrowerProduct : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (!pawn.CanReserve(TargetThingA))
            {
                return false;
            }

            return pawn.Reserve(TargetThingA, job, errorOnFailed: errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnDestroyedOrNull(TargetIndex.A);
            yield return Toils_Reserve.Reserve(TargetIndex.A);

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            yield return Toils_General.WaitWith(TargetIndex.A, 200, true);
            yield return new Toil()
            {
                initAction = delegate ()
                {
                    Building_GrowerBase grower = TargetThingA as Building_GrowerBase;
                    if (grower != null)
                    {
                        Pawn actor = GetActor();
                        Thing product = grower.ExtractProduct(actor);
                        if (!product.Spawned)
                        {
                            GenPlace.TryPlaceThing(product, grower.InteractionCell, grower.Map, ThingPlaceMode.Near);
                        }

                        if (product is Pawn)
                        {
                            EndJobWith(JobCondition.Succeeded);
                        }
                        else
                        {
                            //job.SetTarget(TargetIndex.B, product);
                            IntVec3 storeCell;
                            IHaulDestination haulDestination;
                            if (StoreUtility.TryFindBestBetterStorageFor(product, actor, product.Map, StoragePriority.Unstored, actor.Faction, out storeCell, out haulDestination, false))
                            {
                                if (storeCell.IsValid || haulDestination != null)
                                {
                                    actor.jobs.StartJob(HaulAIUtility.HaulToStorageJob(actor, product), JobCondition.Succeeded);
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
