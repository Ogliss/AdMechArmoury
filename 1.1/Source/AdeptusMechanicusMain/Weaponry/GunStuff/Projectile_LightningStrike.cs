using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000003 RID: 3
    public class Projectile_LightningStrike : Bullet
    {
        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);
            base.Map.weatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrike(base.Map, base.Position));
        }
    }

}
