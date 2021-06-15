using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    public class StatPart_Reliability : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            if (req.HasThing && req.Thing.TryGetCompFast<CompWeapon_GunSpecialRules>()!=null)
            {
                CompWeapon_GunSpecialRules gun = req.Thing.TryGetCompFast<CompWeapon_GunSpecialRules>();
                string reliabilityString;
                float jamsOn;
                GetReliability(gun, out reliabilityString, out jamsOn);
                this.parentStat.formatString = reliabilityString;
                switch (reliabilityString)
                {
                    case "Unreliable":
                        this.parentStat.description = "This gun is unreliable in combat and can fail easily.";
                        break;
                    case "Standard":
                        this.parentStat.description = "This gun is reliable in combat but can occationally fail.";
                        break;
                    case "Very Reliable":
                        this.parentStat.description = "This gun is very reliable in combat and tends not to fail.";
                        break;
                    case "Extremely Reliable":
                        this.parentStat.description = "This gun is extremely reliable in combat and tends not to fail.";
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

        public static void GetReliability(IAdvancedVerb verbEntry, Thing gun, out string rel, out float jamsOn)
        {
            rel = string.Empty;
            jamsOn = JamChance(verbEntry, gun);
            if (jamsOn < 0.25)
                rel = "Extremely Reliable";
            else if (jamsOn < 0.5)
                rel = "Very Reliable";
            else if (jamsOn < 1)
                rel = "Standard";
            else
                rel = "Unreliable";
        }
        public static void GetReliability(CompWeapon_GunSpecialRules gun, out string rel, out float jamsOn)
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
        /// <param name="verbEntry">The gun object</param>
        /// <returns>floating point number representing the jam chance</returns>
        public static float JamChance(Reliability verbEntry, Thing gun)
        {
            float result = 0f;
            switch (verbEntry)
            {
                case Reliability.UR:
                    result = 50f;
                    break;
                case Reliability.ST:
                    result = 30f;
                    break;
                case Reliability.VR:
                    result = 10f;
                    break;
                default:
                    return 0;
            }
            if (gun != null)
            {
                result += GetQualityFactor(gun);
                result = result * 100 / gun.HitPoints / 100;
            }
            result = (float)(Math.Truncate((double)result * 100.0) / 100.0);
            return result;
        }
        public static float JamChance(IAdvancedVerb verbEntry, Thing gun)
        {
            float result = 0f;
            switch (verbEntry.Reliability)
            {
                case Reliability.UR:
                    result = 50f;
                    break;
                case Reliability.ST:
                    result = 30f;
                    break;
                case Reliability.VR:
                    result = 10f;
                    break;
                default:
                    return 0;
            }
            if (gun != null)
            {
                result += GetQualityFactor(gun);
                result = result * 100 / gun.HitPoints / 100;
            }
            result = (float)(Math.Truncate((double)result * 100.0) / 100.0);
            return result;
        }
        public static float JamChance(GunVerbEntry verbEntry, Thing gun)
        {
            float result = 0f;
            switch (verbEntry.reliability)
            {
                case Reliability.UR:
                    result = 50f;
                    break;
                case Reliability.ST:
                    result = 30f;
                    break;
                case Reliability.VR:
                    result = 10f;
                    break;
                default:
                    return 0;
            }
            if (gun != null)
            {
                result += GetQualityFactor(gun);
                result = result * 100 / gun.HitPoints / 100;
            }
            result = (float)(Math.Truncate((double)result * 100.0) / 100.0);
            return result;
        }
        public static float JamChance(CompWeapon_GunSpecialRules gun)
        {
            float result = 0f;
            switch (gun.Reliability)
            {
                case Reliability.UR:
                    result = 30f;
                    break;
                case Reliability.ST:
                    result = 15f;
                    break;
                case Reliability.VR:
                    result = 7.5f;
                    break;
                default:
                    return 0;
            }
            //    Log.Message(string.Format("result {0}", result));
            result += GetQualityFactor(gun.parent);
            //    Log.Message(string.Format("result {0}", result));
            result = result * 100 / gun.parent.HitPoints / 100;
            //    Log.Message(string.Format("result {0}", result));
            result = (float)(Math.Truncate((double)result * 100.0) / 100.0);
            //    Log.Message(string.Format("result {0}", result));
            return result;
        }
        /// <summary>
        /// Returns a factor to scale quality. If the ownerEquipment doesn't have a CompQuality it will return a factor of 0.
        /// </summary>
        /// <returns>Quality-based scale factor</returns>
        public static int GetQualityFactor(Thing gun)
        {
            QualityCategory qc;
            if (gun.TryGetQuality(out qc))
            {
                switch (qc)
                {
                    case QualityCategory.Awful:
                        return 35;
                    case QualityCategory.Poor:
                        return 25;
                    case QualityCategory.Normal:
                        return 0;
                    case QualityCategory.Good:
                        return -5;
                    case QualityCategory.Excellent:
                        return -20;
                    case QualityCategory.Masterwork:
                        return -35;
                    case QualityCategory.Legendary:
                        return -50;
                }
            }
            return 0;
        }
        
    }
}
