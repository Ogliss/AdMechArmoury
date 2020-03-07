using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;
using AdeptusMechanicus;

namespace AdeptusMechanicus
{
    /// <summary>
    /// Building for growing things like organs. Requires constant maintence in order to not botch the crafting. Dirty rooms increase maintence drain even more.
    /// </summary>
    public class Building_VatGrower : Building_GrowerBase, IMaintainableGrower
    {
        static Building_VatGrower()
        {
            cleanlinessCurve.Add(-5.0f, 5.00f);
            cleanlinessCurve.Add(-2.0f, 1.75f);
            cleanlinessCurve.Add(0.0f, 1.0f);
            cleanlinessCurve.Add(0.4f, 0.35f);
            cleanlinessCurve.Add(2.0f, 0.1f);
        }

        public static SimpleCurve cleanlinessCurve = new SimpleCurve();

        /// <summary>
        /// Current active recipe being crafted.
        /// </summary>
        public GrowerRecipeDef activeRecipe;

        public override int TicksNeededToCraft => activeRecipe?.craftingTime ?? 0;

        /// <summary>
        /// From 0.0 to 1.0. If the maintence is below 50% there is a chance for failure.
        /// </summary>
        public float scientistMaintence;

        /// <summary>
        /// From 0.0 to 1.0. If the maintence is below 50% there is a chance for failure.
        /// </summary>
        public float doctorMaintence;

        public float RoomCleanliness
        {
            get
            {
                Room room = this.GetRoom(RegionType.Set_Passable);
                if (room != null)
                {
                    return room.GetStat(RoomStatDefOf.Cleanliness);
                }

                return 0f;
            }
        }

        private VatGrowerProperties vatGrowerPropsInt;

        public VatGrowerProperties VatGrowerProps
        {
            get
            {
                if (vatGrowerPropsInt == null)
                {
                    vatGrowerPropsInt = def.GetModExtension<VatGrowerProperties>();

                    //Fallback; Is defaults.
                    if (vatGrowerPropsInt == null)
                    {
                        vatGrowerPropsInt = new VatGrowerProperties();
                    }
                }

                return vatGrowerPropsInt;
            }
        }

        public float ScientistMaintence { get => scientistMaintence; set => scientistMaintence = value; }

        public float DoctorMaintence { get => doctorMaintence; set => doctorMaintence = value; }

        public Building_VatGrower() : base()
        {

        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Defs.Look(ref activeRecipe, "activeRecipe");
            Scribe_Values.Look(ref scientistMaintence, "scientistMaintence");
            Scribe_Values.Look(ref doctorMaintence, "doctorMaintence");
        }

        public override string GetInspectString()
        {
            if (!(ParentHolder is Map))
            {
                return null;
            }

            StringBuilder builder = new StringBuilder(base.GetInspectString());

            if (status == CrafterStatus.Crafting)
            {
                builder.AppendLine();
                builder.AppendLine("QE_VatGrowerScientistMaintence".Translate() + ": " + scientistMaintence.ToStringPercent());
                builder.AppendLine("QE_VatGrowerDoctorMaintence".Translate() + ": " + doctorMaintence.ToStringPercent());
            }

            return builder.ToString().TrimEndNewlines();
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            //Draw bottom graphic
            Vector3 drawAltitude = drawLoc;
            if (VatGrowerProps.bottomGraphic != null)
            {
                VatGrowerProps.bottomGraphic.Graphic.Draw(drawAltitude, Rotation, this);
            }

            //Draw product
            drawAltitude += new Vector3(0f, 0.005f, 0f);
            if ((status == CrafterStatus.Crafting || status == CrafterStatus.Finished) && activeRecipe != null && activeRecipe.productGraphic != null)
            {
                Material material = activeRecipe.productGraphic.Graphic.MatSingle;

                float scale = (0.2f + (CraftingProgressPercent * 0.8f)) * VatGrowerProps.productScaleModifier;
                Vector3 scaleVector = new Vector3(scale, 1f, scale);
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(drawAltitude + VatGrowerProps.productOffset, Quaternion.AngleAxis(0f, Vector3.up), scaleVector);

                Graphics.DrawMesh(MeshPool.plane10, matrix, material, 0);
            }

            //Draw top graphic
            if (VatGrowerProps.topGraphic != null)
            {
                drawAltitude += new Vector3(0f, 0.005f, 0f);
                VatGrowerProps.topGraphic.Graphic.Draw(drawAltitude, Rotation, this);
            }

            //Draw top detail graphic
            if (VatGrowerProps.topDetailGraphic != null && (PowerTrader?.PowerOn ?? false))
            {
                drawAltitude += new Vector3(0f, 0.005f, 0f);
                VatGrowerProps.topDetailGraphic.Graphic.Draw(drawAltitude, Rotation, this);
            }

            //Draw glow graphic
            if ((status == CrafterStatus.Crafting || status == CrafterStatus.Finished) && VatGrowerProps.glowGraphic != null && (PowerTrader?.PowerOn ?? false))
            {
                drawAltitude += new Vector3(0f, 0.005f, 0f);
                VatGrowerProps.glowGraphic.Graphic.Draw(drawAltitude, Rotation, this);
            }
        }

        public override void Notify_CraftingStarted()
        {
            innerContainer.ClearAndDestroyContents();
        }

        public override void Notify_CraftingFinished()
        {
            Messages.Message("QE_MessageGrowingDone".Translate(activeRecipe.productDef.LabelCap), new LookTargets(this), MessageTypeDefOf.PositiveEvent, false);
        }

        public override void Tick_Crafting()
        {
            base.Tick_Crafting();

            //Deduct maintence, fail if any of them go below 0%.
            float powerModifier = 1f;
            if (PowerTrader != null && !PowerTrader.PowerOn)
            {
                powerModifier = 10f;
            }
            float cleanlinessModifer = cleanlinessCurve.Evaluate(RoomCleanliness);
            float decayRate = 0.00003f * cleanlinessModifer * powerModifier;

            scientistMaintence -= decayRate;
            doctorMaintence -= decayRate;

            if (scientistMaintence < 0f || doctorMaintence < 0f)
            {
                //Fail the craft, waste all products.
                Reset();
            }
        }

        public override Thing ExtractProduct(Pawn pawn)
        {
            Thing product = ThingMaker.MakeThing(activeRecipe.productDef);
            product.stackCount = activeRecipe.productAmount;

            if (status == CrafterStatus.Finished)
            {
                CraftingFinished();
            }

            return product;
        }

        public void StartCraftingRecipe(GrowerRecipeDef recipeDef)
        {
            //Setup recipe order
            orderProcessor.Reset();
            IngredientUtility.FillOrderProcessorFromVatGrowerRecipe(orderProcessor, recipeDef);
            orderProcessor.Notify_ContentsChanged();

            //Initialize maintence
            scientistMaintence = 0.25f;
            doctorMaintence = 0.25f;

            activeRecipe = recipeDef;
            status = CrafterStatus.Filling;
        }

        public override void Notify_ThingLostInOrderProcessor()
        {
            StopCrafting();
        }

        public void StopCrafting()
        {
            craftingProgress = 0;
            orderProcessor.Reset();

            status = CrafterStatus.Idle;
            activeRecipe = null;
            if (innerContainer.Count > 0)
            {
                innerContainer.TryDropAll(InteractionCell, Map, ThingPlaceMode.Near);
            }
        }

        public override string TransformStatusLabel(string label)
        {
            string recipeLabel = activeRecipe?.LabelCap ?? "no recipe";

            if (status == CrafterStatus.Filling || status == CrafterStatus.Finished)
            {
                return label + " " + recipeLabel.CapitalizeFirst();
            }
            if (status == CrafterStatus.Crafting)
            {
                return label + " " + recipeLabel.CapitalizeFirst() + " (" + CraftingProgressPercent.ToStringPercent() + ")";
            }

            return base.TransformStatusLabel(label);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }

            if (status == CrafterStatus.Idle)
            {
                yield return new Command_Action()
                {
                    defaultLabel = "QE_VatGrowerStartCraftingGizmoLabel".Translate(),
                    defaultDesc = "QE_VatGrowerStartCraftingGizmoDescription".Translate(),
                    icon = ContentFinder<Texture2D>.Get("Things/Item/Health/HealthItem", true),
                    order = -100,
                    action = delegate ()
                    {
                        List<FloatMenuOption> options = new List<FloatMenuOption>();

                        foreach (GrowerRecipeDef recipeDef in DefDatabase<GrowerRecipeDef>.AllDefs.OrderBy(def => def.orderID))
                        {
                            bool disabled = false;
                            if (recipeDef.requiredResearch != null && !recipeDef.requiredResearch.IsFinished)
                            {
                                disabled = true;
                            }

                            string label = null;
                            if (disabled)
                            {
                                label = "QE_VatGrowerStartCraftingFloatMenuDisabled".Translate(recipeDef.LabelCap, recipeDef.requiredResearch.LabelCap);
                            }
                            else
                            {
                                label = recipeDef.LabelCap;
                            }

                            FloatMenuOption option = new FloatMenuOption(label, delegate ()
                            {
                                StartCraftingRecipe(recipeDef);
                            });

                            option.Disabled = disabled;

                            options.Add(option);
                        }

                        if (options.Count > 0)
                        {
                            Find.WindowStack.Add(new FloatMenu(options));
                        }
                    }
                };
            }
            else
            {
                if (status != CrafterStatus.Finished)
                {
                    yield return new Command_Action()
                    {
                        defaultLabel = "QE_VatGrowerStopCraftingGizmoLabel".Translate(),
                        defaultDesc = "QE_VatGrowerStopCraftingGizmoDescription".Translate(),
                        icon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel", true),
                        order = -100,
                        action = delegate ()
                        {
                            StopCrafting();
                        }
                    };
                }
            }
        }
    }
}
