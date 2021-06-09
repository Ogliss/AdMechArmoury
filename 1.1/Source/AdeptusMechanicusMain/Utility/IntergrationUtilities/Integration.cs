using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public abstract class Integration
    {
        public virtual string PackageID()
        {
            return "";
        }
        public bool IsActive => ModsConfig.ActiveModsInLoadOrder.Any((ModMetaData m) => m.PackageIdPlayerFacing == PackageID());


        public virtual void DrawSettings(ref Rect rect) { }
        public virtual void ScribeSettings()
        {
        }
    }
}
