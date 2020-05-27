using System;

namespace AdeptusMechanicus
{
    public class TargetAoEProperties
    {
        public bool friendlyFire = false;
        public int maxTargets = 3;
        public int range;
        public bool showRangeOnSelect = true;
        public bool startsFromCaster = true;
        public Type targetClass;
    }
}