using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace AbilityUser
{
    // Token: 0x02000023 RID: 35
    public class VerbProperties_AbilityOG : VerbProperties_Ability
    {
        public Reliability reliability = Reliability.NA;
        public bool logging = false;
        public bool rapidfire = false;
        public bool canJam = false;
        public bool overheat = false;
        public bool canDamageWeapon = false;
        public bool criticaloverheatExplosion = false;
        public bool isSecondry = false;
        public float extraWeaponDamage = 0f;
        public float overheatsOn = 0f;
        public int pelletCount = 1;
        public int coolDown = 0;
        public int lastShotTick = 0;


        public override string ToString()
        {
            string str;
            if (!this.label.NullOrEmpty())
            {
                str = this.label;
            }
            else
            {
                str = string.Concat(new object[]
                {
                    "range=",
                    this.range,
                    ", projectile=",
                    (this.defaultProjectile == null) ? "null" : this.defaultProjectile.defName,
                    ", reliability=",
                    this.reliability.ToString()
                });
            }
            return "VerbProperties(" + str + ")";
        }
    }

    public enum Reliability : short
    {
        UR = 80,
        ST = 55,
        VR = 30,
        NA = 0
    }

}
