using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Verse
{
    /*
    // Token: 0x02000B9E RID: 2974
    public abstract class ResearchMod
    {
        // Token: 0x0600413D RID: 16701
        public abstract void Apply();
    }
    */

    public class ResearchMod_SetResearchToZero : ResearchMod
    {
        /// <summary>
        /// Defines a specific def. Leave this null for current def.
        /// </summary>
        public ResearchProjectDef def;
        public override void Apply()
        {
            var progress = (Dictionary<ResearchProjectDef, float>)typeof(ResearchManager).GetField("progress", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Find.ResearchManager);
            if (def == null)
                def = Find.ResearchManager.currentProj;

            progress[def] = 0f;
            //For testing and spamming purposes - No real effect on standard gameplay - AND CAUSING BUGS!
            //Find.ResearchManager.currentProj = null;
        }
    }
}
