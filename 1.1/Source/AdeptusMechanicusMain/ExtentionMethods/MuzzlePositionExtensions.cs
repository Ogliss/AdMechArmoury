using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdeptusMechanicus.Lasers;
using AdeptusMechanicus.settings;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.ExtensionMethods
{
    public static class MuzzlePositionExtensions
    {
        #region ThingExtensions
        public static bool Muzzle(this Thing thing, out float barrelLength, out float barrelOffset, out float bulletOffset, out ThingDef flareDef, out float flareSize, out ThingDef smokeDef, out float smokeSize)
        {
            bool result = false;
            IMuzzlePosition m;
            barrelLength = 0;
            barrelOffset = 0;
            bulletOffset = 0;
            flareDef = null;
            flareSize = 0;
            smokeDef = null;
            smokeSize = 0;
            string rstring = "\nbarrelLength: {0} barrelOffset: {1}\nflareDef: {2} flareSize: {3}\nsmokeDef: {4} smokeSize: {5}";
            StringBuilder builder = new StringBuilder();
            CompEquippable eq = thing.TryGetCompFast<CompEquippable>();
            if (eq != null)
            {
                if ((m = eq.PrimaryVerb?.verbProps as IMuzzlePosition) != null)
                {
                    result = true;
                    barrelLength = m.BarrelLength;
                    barrelOffset = m.BarrelOffset;
                    bulletOffset = m.BulletOffset;
                    flareDef ??= m.MuzzleFlareDef;
                    flareSize = m.MuzzleFlareSize > 0 ? m.MuzzleFlareSize : flareSize;
                    smokeDef ??= m.MuzzleSmokeDef;
                    smokeSize = m.MuzzleSmokeSize > 0 ? m.MuzzleSmokeSize : smokeSize;
                    builder.AppendLine("m = " + thing.Label + " verbProps" + string.Format(rstring, barrelLength, barrelOffset, flareDef, flareSize, smokeDef, smokeSize));
                }
                else
                {
                    if ((m = thing as IMuzzlePosition) != null)
                    {
                        result = true;
                        barrelLength = m.BarrelLength;
                        barrelOffset = m.BarrelOffset;
                        bulletOffset = m.BulletOffset;
                        flareDef ??= m.MuzzleFlareDef;
                        flareSize = m.MuzzleFlareSize > 0 ? m.MuzzleFlareSize : flareSize;
                        smokeDef ??= m.MuzzleSmokeDef;
                        smokeSize = m.MuzzleSmokeSize > 0 ? m.MuzzleSmokeSize : smokeSize;
                        builder.AppendLine("m = " + thing.Label + " IMuzzlePosition" + string.Format(rstring, barrelLength, barrelOffset, flareDef, flareSize, smokeDef, smokeSize));
                    }
                    if ((m = thing.def.GetModExtensionFast<BarrelOffsetExtension>()) != null)
                    {
                        result = true;
                        barrelLength = m.BarrelLength;
                        barrelOffset = m.BarrelOffset;
                        bulletOffset = m.BulletOffset;
                        flareDef ??= m.MuzzleFlareDef;
                        flareSize = m.MuzzleFlareSize > 0 ? m.MuzzleFlareSize : flareSize;
                        smokeDef ??= m.MuzzleSmokeDef;
                        smokeSize = m.MuzzleSmokeSize > 0 ? m.MuzzleSmokeSize : smokeSize;
                        builder.AppendLine("m = " + thing.Label + " BarrelOffsetExtension" + string.Format(rstring, barrelLength, barrelOffset, flareDef, flareSize, smokeDef, smokeSize));
                    }
                }
                if ((m = eq.PrimaryVerb?.GetProjectile()?.GetModExtensionFast<ProjectileVFX>()) != null)
                {
                    result = true;
                    barrelLength += m.BarrelLength;
                    barrelOffset += m.BarrelOffset;
                    bulletOffset += m.BulletOffset;
                    flareDef ??= m.MuzzleFlareDef;
                    flareSize = m.MuzzleFlareSize > 0 ? m.MuzzleFlareSize : flareSize;
                    smokeDef ??= m.MuzzleSmokeDef;
                    smokeSize = m.MuzzleSmokeSize > 0 ? m.MuzzleSmokeSize : smokeSize;
                    builder.AppendLine("m = " + thing.Label + " PrimaryVerb GetProjectile" + string.Format(rstring, barrelLength, barrelOffset, flareDef, flareSize, smokeDef, smokeSize));
                }
            }
            else
            {
                if ((m = thing.def.GetModExtensionFast<ProjectileVFX>()) != null)
                {
                    result = true;
                    barrelLength += m.BarrelLength;
                    barrelOffset += m.BarrelOffset;
                    bulletOffset += m.BulletOffset;
                    flareDef ??= m.MuzzleFlareDef;
                    flareSize = m.MuzzleFlareSize > 0 ? m.MuzzleFlareSize : flareSize;
                    smokeDef ??= m.MuzzleSmokeDef;
                    smokeSize = m.MuzzleSmokeSize > 0 ? m.MuzzleSmokeSize : smokeSize;
                    builder.AppendLine("m = " + thing.Label + " EffectProjectileExtension" + string.Format(rstring, barrelLength, barrelOffset, flareDef, flareSize, smokeDef, smokeSize));
                }
            }
            if (builder.Length > 0)
            {
            //    Log.Message(builder.ToString());
            }
            return result;
        }
        public static Vector3 MuzzlePositionFor(this Thing thing, Vector3 origin, float aimAngle)
        {
            Map map = thing.Map;
            if (thing.Muzzle(out float barrelLength, out float barrelOffset, out float bulletOffset, out ThingDef flareDef, out float flareSize, out ThingDef smokeDef, out float smokeSize))
            {
                float length = barrelLength + bulletOffset;
                float offset = -barrelOffset;
                IDrawnWeaponWithRotation gun = thing as IDrawnWeaponWithRotation;
                if (gun != null)
                {
                    aimAngle = (aimAngle + gun.RotationOffset) % 360;
                }
                if (aimAngle > 200f && aimAngle < 340f)
                {
                    offset = -offset;
                }
                offset *= (thing.def.graphicData.drawSize.magnitude / 2);
                length *= (thing.def.graphicData.drawSize.magnitude / 2);
                /*
                Log.Message(thing.Label + " graphicData drawSize: " + thing.def.graphicData.drawSize.magnitude + "  / 2 = " + thing.def.graphicData.drawSize.magnitude / 2);
                Log.Message(thing.Label + " graphic drawSize: " + thing.def.graphic.drawSize.magnitude + "  / 2 = " + thing.def.graphic.drawSize.magnitude / 2);
                */
                origin += new Vector3(0f + offset, 0f, 0.4f + length).RotatedBy(aimAngle);


                origin.y += 0.0367346928f;

            }
            return origin;
        }

        #endregion ThingExtensions

        #region VerbExtensions
        public static bool Muzzle(this Verb verb, out float barrelLength, out float barrelOffset, out ThingDef flareDef, out float flareSize, out ThingDef smokeDef, out float smokeSize)
        {
            bool result = false;
            IMuzzlePosition m;
            barrelLength = 0;
            barrelOffset = 0;
            flareDef = null;
            flareSize = 0;
            smokeDef = null;
            smokeSize = 0;
            string rstring = "\nbarrelLength: {0} barrelOffset: {1}\nflareDef: {2} flareSize: {3}\nsmokeDef: {4} smokeSize: {5}";
            StringBuilder builder = new StringBuilder();
            if (verb == null)
            {
                Log.Error("Cannot find Muzzle position, Verb is Null");
                return result;

            }
            if (verb.verbProps == null)
            {
                Log.Error("Cannot find Muzzle position, verbProps are null");
                return result;
            }
            if ((m = verb.verbProps as IMuzzlePosition) != null)
            {
            //    Log.Message("m = verbProps");
                result = true;
                barrelLength = m.BarrelLength;
                barrelOffset = m.BarrelOffset;
                flareDef ??= m.MuzzleFlareDef;
                flareSize = m.MuzzleFlareSize > 0 ? m.MuzzleFlareSize : flareSize;
                smokeDef ??= m.MuzzleSmokeDef;
                smokeSize = m.MuzzleSmokeSize > 0 ? m.MuzzleSmokeSize : smokeSize;
                builder.AppendLine("m = " + verb + " verbProps" + string.Format(rstring, barrelLength, barrelOffset, flareDef, flareSize, smokeDef, smokeSize));
            }
            else
            {
            //    Log.Message("m != verbProps");
                if ((m = verb.EquipmentSource as IMuzzlePosition) != null)
                {
                //    Log.Message("m = EquipmentSource");
                    result = true;
                    barrelLength = m.BarrelLength;
                    barrelOffset = m.BarrelOffset;
                    flareDef ??= m.MuzzleFlareDef;
                    flareSize = m.MuzzleFlareSize > 0 ? m.MuzzleFlareSize : flareSize;
                    smokeDef ??= m.MuzzleSmokeDef;
                    smokeSize = m.MuzzleSmokeSize > 0 ? m.MuzzleSmokeSize : smokeSize;
                    builder.AppendLine("m = " + verb + " IMuzzlePosition" + string.Format(rstring, barrelLength, barrelOffset, flareDef, flareSize, smokeDef, smokeSize));
                }
                if ((m = verb.EquipmentSource?.def.GetModExtensionFast<BarrelOffsetExtension>()) != null)
                {
                //    Log.Message("m = BarrelOffsetExtension");
                    result = true;
                    barrelLength = m.BarrelLength;
                    barrelOffset = m.BarrelOffset;
                    flareDef ??= m.MuzzleFlareDef;
                    flareSize = m.MuzzleFlareSize > 0 ? m.MuzzleFlareSize : flareSize;
                    smokeDef ??= m.MuzzleSmokeDef;
                    smokeSize = m.MuzzleSmokeSize > 0 ? m.MuzzleSmokeSize : smokeSize;
                    builder.AppendLine("m = " + verb + " BarrelOffsetExtension" + string.Format(rstring, barrelLength, barrelOffset, flareDef, flareSize, smokeDef, smokeSize));
                }
            }
            if ((m = verb.GetProjectile().GetModExtensionFast<ProjectileVFX>()) != null)
            {
            //    Log.Message("m = ProjectileVFX");
                result = true;
                barrelLength += m.BarrelLength;
                barrelOffset += m.BarrelOffset;
                flareDef ??= m.MuzzleFlareDef;
                flareSize = m.MuzzleFlareSize > 0 ? m.MuzzleFlareSize : flareSize;
                smokeDef ??= m.MuzzleSmokeDef;
                smokeSize = m.MuzzleSmokeSize > 0 ? m.MuzzleSmokeSize : smokeSize;
                builder.AppendLine("m = " + verb + " Verb GetProjectile" + string.Format(rstring, barrelLength, barrelOffset, flareDef, flareSize, smokeDef, smokeSize));
            }
            if (AMAMod.Dev)
            {
                if (m == null)
                {
                    Log.Message("No Muzzle found for verb: " + verb?.ToString() ?? "NULL");
                }
                if (builder.Length > 0)
                {
                    Log.Message(builder.ToString());
                }
            }
            return result;
        }

        public static Vector3 MuzzlePositionFor(this Verb verb, float aimAngle, bool throwMotes = false)
        {
            Vector3 origin = verb.CasterIsPawn ? verb.CasterPawn.Drawer.DrawPos : verb.Caster.DrawPos;
            if (verb == null)
            {
                Log.Error("Cannot find Muzzle position, Verb is Null");
                return origin;

            }
            if (verb.verbProps == null)
            {
                Log.Error("Cannot find Muzzle position, verbProps are null");
                return origin;
            }
            Map map = verb.Caster.Map;
            if (verb.Muzzle(out float barrelLength, out float barrelOffset, out ThingDef flareDef, out float flareSize, out ThingDef smokeDef, out float smokeSize))
            {
                float length = barrelLength;
                float offset = -barrelOffset;
                IDrawnWeaponWithRotation gun = verb.EquipmentSource as IDrawnWeaponWithRotation;
                if (gun != null)
                {
                    aimAngle = (aimAngle + gun.RotationOffset) % 360;
                }
                if (aimAngle > 200f && aimAngle < 340f)
                {
                    offset = -offset;
                }
                offset *= (verb.EquipmentSource.def.graphicData.drawSize.magnitude / 2);
                length *= (verb.EquipmentSource.def.graphicData.drawSize.magnitude / 2);

                origin += new Vector3(0f + offset, 0f, 0.4f + length).RotatedBy(aimAngle);
                origin.y += 0.0367346928f;

                if (throwMotes)
                {
                    if (flareDef != null && flareSize > 0)
                    {
                        AdeptusMoteMaker.MakeStaticMote(origin, map, flareDef, flareSize, null, Rand.Range(0, 360));
                    }
                    if (smokeDef != null && smokeSize > 0)
                    {
                        AdeptusMoteMaker.ThrowSmoke(origin, smokeSize, map, smokeDef, null, Rand.Range(0, 360));
                    }
                }
            }
            return origin;
        }
        #endregion VerbExtensions

    }
}
