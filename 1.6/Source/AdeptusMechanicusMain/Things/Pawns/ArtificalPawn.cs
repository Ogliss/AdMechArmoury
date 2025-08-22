using System;
using System.Collections.Generic;
using System.Linq;
using AdeptusMechanicus.ExtensionMethods;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{

    public class ArtificalPawn : Pawn
    {
        public new virtual Faction Faction
        {
            get { return base.factionInt; }
        }

        public virtual Pawn InnerPawn
        {
            get
            {
                return this;
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
        }
        
    }

}
