using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000020 RID: 32
    public class SpawbWithAnimalsExtension : DefModExtension
    {
        public float animalChance = 0.5f;
        public IntRange animalCount = new IntRange(1,5);
        public float animalPoints = -1f;
        public List<PawnGenOption> kindDefs = new List<PawnGenOption>();
    }
}
 