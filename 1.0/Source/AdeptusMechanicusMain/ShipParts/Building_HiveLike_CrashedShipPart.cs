using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace AdeptusMechanicus
{
    public class Building_HiveLike_CrashedShipPart : HiveLike
    { 
        public override void Tick()
        {
            base.Tick();
            this.age++;
        }
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.age, "age", 0, false);
        }

        public override void SpawnInitialPawns()
        {
        //    this.SpawnPawnsUntilPoints(InitialPawnsPoints);
            this.CalculateNextPawnSpawnTick();
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());
            if (stringBuilder.Length != 0)
            {
                stringBuilder.AppendLine();
            }
            stringBuilder.Append("AwokeDaysAgo".Translate(this.age.TicksToDays().ToString("F1")));
            return stringBuilder.ToString();
        }

        public int age;
    }
}
