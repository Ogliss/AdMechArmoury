using RimWorld;
using System;

namespace AdeptusMechanicus
{
    class StatPart_Overheat : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            if (req.HasThing && (req.Thing.GetType() == Type.GetType("AdeptusMechanicus.ThingDef_GunOH")))
            {
                ThingDef_GunOH gun = (ThingDef_GunOH)req.Thing;
                string overheatString;
                float jamsOn;
                GetOverheat(gun, out overheatString, out jamsOn);
                this.parentStat.formatString = overheatString;
                switch (overheatString)
                {
                    case "Unreliable":
                        this.parentStat.description = "This gun is unreliable in combat and can jam easily.";
                        break;
                    case "Standard":
                        this.parentStat.description = "This gun is reliable in combat but can occationally jam.";
                        break;
                    case "Very Reliable":
                        this.parentStat.description = "This gun is very reliable in combat and tends not to jam.";
                        break;
                    case "Extremely Reliable":
                        this.parentStat.description = "This gun is extremely reliable in combat and tends not to jam.";
                        break;
                    default:
                        return;
                }
                val *= jamsOn;
            }
        }
        
        public override string ExplanationPart(StatRequest req)
        {
            string result = string.Empty;
            //if (req.HasThing && (req.Thing.GetType() == Type.GetType("Cyberpunk.ThingDef_GunCP")))
            //{
            //    ThingDef_GunCP gun = (ThingDef_GunCP)req.Thing;
            //    string reliabilityString;
            //    float jamsOn;
            //    GetReliability(gun, out reliabilityString, out jamsOn);
            //    reliabilityString = gun.reliability.ToString();
            //    switch (gun.reliability)
            //    {
            //        case Reliability.UR:
            //            reliabilityString = "Unreliable";
            //            break;
            //        case Reliability.ST:
            //            reliabilityString = "Standard";
            //            break;
            //        case Reliability.VR:
            //            reliabilityString = "Very Reliable";
            //            break;
            //        default:
            //            reliabilityString = "Standard";
            //            break;
            //    }
            //    result = string.Format("Design Reliability: {0}\r\n\r\n", reliabilityString);
            //    return result;
            //}
            
            return result;
        }

        public static void GetOverheat(ThingDef_GunOH gun, out string rel, out float jamsOn)
        {
            rel = string.Empty;
            jamsOn = JamChance(gun);
            if (jamsOn < 0.25)
                rel = "Extremely Reliable";
            else if (jamsOn < 0.5)
                rel = "Very Reliable";
            else if (jamsOn < 1)
                rel = "Standard";
            else
                rel = "Unreliable";
        }

        /// <summary>
        /// Calculates the chance that the gun will jam
        /// </summary>
        /// <param name="gun">The gun object</param>
        /// <returns>floating point number representing the jam chance</returns>
        public static float JamChance(ThingDef_GunOH gun)
        {
            float result = 0f;
            switch (gun.overheat)
            {
                case Overheat.UR:
                    result = 80f;
                    break;
                case Overheat.ST:
                    result = 55f;
                    break;
                case Overheat.VR:
                    result = 30f;
                    break;
                default:
                    return 0;
            }
            result += GetQualityFactor(gun);
            result = result * 100 / gun.HitPoints / 100;
            result = (float)(Math.Truncate((double)result * 100.0) / 100.0);
            return result;
        }

        /// <summary>
        /// Returns a factor to scale quality. If the ownerEquipment doesn't have a CompQuality it will return a factor of 0.
        /// </summary>
        /// <returns>Quality-based scale factor</returns>
        public static int GetQualityFactor(ThingDef_GunOH gun)
        {
            QualityCategory qc;
            if (gun.TryGetQuality(out qc))
            {
                switch (qc)
                {
                    case QualityCategory.Awful:
                        return 15;
                    case QualityCategory.Shoddy:
                        return 10;
                    case QualityCategory.Poor:
                        return 5;
                    case QualityCategory.Normal:
                        return 0;
                    case QualityCategory.Good:
                        return -5;
                    case QualityCategory.Excellent:
                        return -10;
                    case QualityCategory.Superior:
                        return -15;
                    case QualityCategory.Masterwork:
                        return -20;
                    case QualityCategory.Legendary:
                        return -25;
                }
            }
            return 0;
        }
    }
}
