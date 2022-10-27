using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdeptusMechanicus.Lasers
{
    public interface IDrawnWeaponWithRotation
    {
        public float RotationOffset
        {
            get;
            set;
        }
    }
}
