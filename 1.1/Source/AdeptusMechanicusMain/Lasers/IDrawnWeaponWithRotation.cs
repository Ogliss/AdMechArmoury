using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdeptusMechanicus
{
    public interface IDrawnWeaponWithRotation
    {
        float RotationOffset
        {
            get;
            set;
        }
    }
}
