using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus
{
    public class ThingDef_GunOG : ThingWithComps, IBeamColorThing, IDrawnWeaponWithRotation
    {

        new public LaserGunDef def => base.def as LaserGunDef ?? LaserGunDef.defaultObj;

        public int BeamColor
        {
            get { return LaserColor.IndexBasedOnThingQuality(beamColorIndex, this); }
            set { beamColorIndex = value; }
        }

        int ticksPreviously = 0;
        public float RotationOffset
        {
            get
            {
                int ticks = Find.TickManager.TicksGame;
                UpdateRotationOffset(ticks - ticksPreviously);
                ticksPreviously = ticks;

                return rotationOffset;
            }
            set
            {
                rotationOffset = value;
                rotationSpeed = 0;
            }
        }

        public Reliability reliability
        {
            get
            {
                if (!this.AllComps.NullOrEmpty())
                {
                    if (GetComp<CompWeapon_GunSpecialRules>() != null)
                    {
                        if (GetComp<CompWeapon_GunSpecialRules>() is CompWeapon_GunSpecialRules GunExt)
                        {
                            return GunExt.reliability;
                        }
                    }
                }
                return Reliability.NA;
            }
        }
        /*
        public override void Tick()
        {
            base.Tick();
            CompWargearWeaponSecondry comp = base.GetComp<CompWargearWeaponSecondry>();
            if (comp != null)
            {
                comp.CompTick();
            }
        }
        */
        public override string GetInspectString()
        {
            string result = base.GetInspectString();
            if (GetComp<CompWeapon_GunSpecialRules>()!=null)
            {
                CompWeapon_GunSpecialRules specialRules = GetComp<CompWeapon_GunSpecialRules>();
                if (specialRules.GetsHot || specialRules.Jams)
                {
                    string reliabilityString;
                    float jamsOn;
                    StatPart_Reliability.GetReliability(GetComp<CompWeapon_GunSpecialRules>(), out reliabilityString, out jamsOn);
                    string cause = specialRules.GetsHot ? "Overheat" : "Jam";
                    float chance = specialRules.GetsHot ? jamsOn / 10 : jamsOn / 100;
                    result += string.Format("\r\nReliability: {0}\r\n" + cause + " chance: {1}", reliabilityString, chance.ToStringPercent());
                }
            }
            return result;
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn pawn)
        {
            foreach (FloatMenuOption o in base.GetFloatMenuOptions(pawn))
            {
                if (o != null) yield return o;
            }

            if (!def.supportsColors) yield break;
            /*
            foreach (FloatMenuOption o in LaserColor.GetChangeBeamColorFloatMenuOptions(this, pawn))
            {
                if (o != null) yield return o;
            }
            */
            yield break;
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref beamColorIndex, "beamColorIndex", -1, false);
            if (GetComp<CompWeapon_GunSpecialRules>() != null)
            {
                CompWeapon_GunSpecialRules specialRules = GetComp<CompWeapon_GunSpecialRules>();
                if (specialRules.GetsHot || specialRules.Jams)
                {
                    string reliabilityString;
                    float jamsOn;
                    StatPart_Reliability.GetReliability(GetComp<CompWeapon_GunSpecialRules>(), out reliabilityString, out jamsOn);

                    Scribe_Values.Look<string>(ref reliabilityString, "reliability", "NA", false);
                }
            }
            if (GetComp<CompWeapon_GunSpecialRules>() != null)
            {
                CompWeapon_GunSpecialRules specialRules = GetComp<CompWeapon_GunSpecialRules>();
                if (specialRules.GetsHot || specialRules.Jams)
                {
                    string reliabilityString;
                    float jamsOn;
                    StatPart_Reliability.GetReliability(GetComp<CompWeapon_GunSpecialRules>(), out reliabilityString, out jamsOn);

                    Scribe_Values.Look<string>(ref reliabilityString, "reliability", "NA", false);
                }
            }
        }
        void UpdateRotationOffset(int ticks)
        {
            if (rotationOffset == 0) return;
            if (ticks <= 0) return;
            if (ticks > 30) ticks = 30;

            if (rotationOffset > 0)
            {
                rotationOffset -= rotationSpeed;
                if (rotationOffset < 0) rotationOffset = 0;
            }
            else if (rotationOffset < 0)
            {
                rotationOffset += rotationSpeed;
                if (rotationOffset > 0) rotationOffset = 0;
            }

            rotationSpeed += ticks * 0.01f;
        }

        private int beamColorIndex = -1;
        private float rotationSpeed = 0;
        private float rotationOffset = 0;
    }
}
