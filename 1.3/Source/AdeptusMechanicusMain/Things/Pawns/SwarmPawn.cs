using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdeptusMechanicus.ExtensionMethods;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    class SwarmPawn : Pawn
    {
        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);
        }
        public override void ExposeData()
        {
            base.ExposeData();
        }

    }
}
