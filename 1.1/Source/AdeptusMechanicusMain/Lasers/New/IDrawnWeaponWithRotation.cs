using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdeptusMechanicus.Lasers
{
    interface IDrawnWeaponWithRotation
    {
        float RotationOffset
        {
            get;
            set;
        }
    }
}
