using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AdeptusMechanicus
{
    public class ScenPart_StartingThing_Define : ScenPart_StartingThing_Defined
    {
        public ThingDef ThingDef
        {
            get
            {
                return this.thingDef;
            }
            set
            {
                this.thingDef = value;
            }
        }

        public int Count
        {
            get
            {
                return this.count;
            }
            set
            {
                this.count = value;
            }
        }

        public ThingDef ThingDefStuff
        {
            get
            {
                return this.stuff;
            }
            set
            {
                this.stuff = value;
            }
        }
        /*
        public override IEnumerable<Thing> PlayerStartingThings()
        {
            IEnumerable<Thing> things = base.PlayerStartingThings();
            if (AdeptusIntergrationUtil.enabled_CombatExtended && ThingDef.IsRangedWeapon)
            {
                things.AddItem(CEAmmo());
            }
            return things;
        }

        public Thing CEAmmo()
        {

            Thing thing = ThingMaker.MakeThing(this.thingDef, this.stuff);
            if (this.thingDef.Minifiable)
            {
                thing = thing.MakeMinified();
            }
            thing.stackCount = this.count;
            return thing;
        }
        */
    }
}
