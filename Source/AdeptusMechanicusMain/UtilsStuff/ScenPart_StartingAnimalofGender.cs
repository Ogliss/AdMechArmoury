using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
    // Token: 0x0200066B RID: 1643
    public class ScenPart_StartingAnimalofGender : ScenPart
    {
        // Token: 0x06002238 RID: 8760 RVA: 0x00101BAC File Offset: 0x000FFFAC
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look<PawnKindDef>(ref this.animalKind, "animalKind");
            Scribe_Values.Look<int>(ref this.count, "count", 0, false);
            Scribe_Values.Look<float>(ref this.bondToRandomPlayerPawnChance, "bondToRandomPlayerPawnChance", 0f, false);
        }

        // Token: 0x06002239 RID: 8761 RVA: 0x00101BEC File Offset: 0x000FFFEC
        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight * 2f);
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(scenPartRect.TopHalf());
            listing_Standard.ColumnWidth = scenPartRect.width;
            listing_Standard.TextFieldNumeric<int>(ref this.count, ref this.countBuf, 1f, 1E+09f);
            listing_Standard.End();
            if (Widgets.ButtonText(scenPartRect.BottomHalf(), this.CurrentAnimalLabel().CapitalizeFirst(), true, false, true))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                list.Add(new FloatMenuOption("RandomPet".Translate().CapitalizeFirst(), delegate ()
                {
                    this.animalKind = null;
                }, MenuOptionPriority.Default, null, null, 0f, null, null));
                foreach (PawnKindDef localKind2 in this.PossibleAnimals())
                {
                    PawnKindDef localKind = localKind2;
                    list.Add(new FloatMenuOption(localKind.LabelCap, delegate ()
                    {
                        this.animalKind = localKind;
                    }, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }
        }

        // Token: 0x0600223A RID: 8762 RVA: 0x00101D40 File Offset: 0x00100140
        private IEnumerable<PawnKindDef> PossibleAnimals()
        {
            return from td in DefDatabase<PawnKindDef>.AllDefs
                   where td.RaceProps.Animal
                   select td;
        }

        // Token: 0x0600223B RID: 8763 RVA: 0x00101D69 File Offset: 0x00100169
        private IEnumerable<PawnKindDef> RandomPets()
        {
            return from td in this.PossibleAnimals()
                   where td.RaceProps.petness > 0f
                   select td;
        }

        // Token: 0x0600223C RID: 8764 RVA: 0x00101D93 File Offset: 0x00100193
        private string CurrentAnimalLabel()
        {
            return (this.animalKind == null) ? "RandomPet".Translate() : this.animalKind.label;
        }

        // Token: 0x0600223D RID: 8765 RVA: 0x00101DBA File Offset: 0x001001BA
        public override string Summary(Scenario scen)
        {
            return ScenSummaryList.SummaryWithList(scen, "PlayerStartsWith", ScenPart_StartingThing_Defined.PlayerStartWithIntro);
        }

        // Token: 0x0600223E RID: 8766 RVA: 0x00101DCC File Offset: 0x001001CC
        public override IEnumerable<string> GetSummaryListEntries(string tag)
        {
            if (tag == "PlayerStartsWith")
            {
                yield return this.CurrentAnimalLabel().CapitalizeFirst() + " x" + this.count;
            }
            yield break;
        }

        // Token: 0x0600223F RID: 8767 RVA: 0x00101DF8 File Offset: 0x001001F8
        public override void Randomize()
        {
            if (Rand.Value < 0.5f)
            {
                this.animalKind = null;
            }
            else
            {
                this.animalKind = this.PossibleAnimals().RandomElement<PawnKindDef>();
            }
            this.count = ScenPart_StartingAnimalofGender.PetCountChances.RandomElementByWeight((Pair<int, float> pa) => pa.Second).First;
            this.bondToRandomPlayerPawnChance = 0f;
        }

        // Token: 0x06002240 RID: 8768 RVA: 0x00101E74 File Offset: 0x00100274
        public override bool TryMerge(ScenPart other)
        {
            ScenPart_StartingAnimalofGender scenPart_StartingAnimal = other as ScenPart_StartingAnimalofGender;
            if (scenPart_StartingAnimal != null && scenPart_StartingAnimal.animalKind == this.animalKind)
            {
                this.count += scenPart_StartingAnimal.count;
                return true;
            }
            return false;
        }

        // Token: 0x06002241 RID: 8769 RVA: 0x00101EB8 File Offset: 0x001002B8
        public override IEnumerable<Thing> PlayerStartingThings()
        {
            for (int i = 0; i < this.count; i++)
            {
                PawnKindDef kind;
                if (this.animalKind != null)
                {
                    kind = this.animalKind;
                }
                else
                {
                    kind = this.RandomPets().RandomElementByWeight((PawnKindDef td) => td.RaceProps.petness);
                }
                Pawn animal = PawnGenerator.GeneratePawn(kind, Faction.OfPlayer);
                if (animal.Name == null || animal.Name.Numerical)
                {
                    animal.Name = PawnBioAndNameGenerator.GeneratePawnName(animal, NameStyle.Full, null);
                }
                if (Rand.Value < this.bondToRandomPlayerPawnChance && animal.training.CanAssignToTrain(TrainableDefOf.Obedience).Accepted)
                {
                    Pawn pawn = (from p in Find.GameInitData.startingAndOptionalPawns.Take(Find.GameInitData.startingPawnCount)
                                 where TrainableUtility.CanBeMaster(p, animal, false) && !p.story.traits.HasTrait(TraitDefOf.Psychopath)
                                 select p).RandomElementWithFallback(null);
                    if (pawn != null)
                    {
                        animal.training.Train(TrainableDefOf.Obedience, null, true);
                        animal.training.SetWantedRecursive(TrainableDefOf.Obedience, true);
                        pawn.relations.AddDirectRelation(PawnRelationDefOf.Bond, animal);
                        animal.playerSettings.Master = pawn;
                    }
                }
                if (gender != Gender.None)
                {
                    animal.gender = gender;
                }
                yield return animal;
            }
            yield break;
        }

        private Gender gender = Gender.None;

        // Token: 0x040013B4 RID: 5044
        private PawnKindDef animalKind;

        // Token: 0x040013B5 RID: 5045
        private int count = 1;

        // Token: 0x040013B6 RID: 5046
        private float bondToRandomPlayerPawnChance = 0.5f;

        // Token: 0x040013B7 RID: 5047
        private string countBuf;

        // Token: 0x040013B8 RID: 5048
        private static readonly List<Pair<int, float>> PetCountChances = new List<Pair<int, float>>
        {
            new Pair<int, float>(1, 20f),
            new Pair<int, float>(2, 10f),
            new Pair<int, float>(3, 5f),
            new Pair<int, float>(4, 3f),
            new Pair<int, float>(5, 1f),
            new Pair<int, float>(6, 1f),
            new Pair<int, float>(7, 1f),
            new Pair<int, float>(8, 1f),
            new Pair<int, float>(9, 1f),
            new Pair<int, float>(10, 0.1f),
            new Pair<int, float>(11, 0.1f),
            new Pair<int, float>(12, 0.1f),
            new Pair<int, float>(13, 0.1f),
            new Pair<int, float>(14, 0.1f)
        };
    }
}
