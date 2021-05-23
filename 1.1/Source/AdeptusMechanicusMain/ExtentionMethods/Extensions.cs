using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.ExtensionMethods
{
    public static class Extensions
    {
        public static IEnumerable<BodyPartRecord> GetNotMissingParts(this HediffSet set, BodyPartHeight height = BodyPartHeight.Undefined, BodyPartDepth depth = BodyPartDepth.Undefined, List<BodyPartTagDef> tags = null, BodyPartRecord partParent = null)
        {
            List<BodyPartRecord> allPartsList = set.pawn.def.race.body.AllParts;
            int num;
            for (int i = 0; i < allPartsList.Count; i = num + 1)
            {
                BodyPartRecord bodyPartRecord = allPartsList[i];
                if (!set.PartIsMissing(bodyPartRecord) && (height == BodyPartHeight.Undefined || bodyPartRecord.height == height) && (depth == BodyPartDepth.Undefined || bodyPartRecord.depth == depth) && (tags == null || bodyPartRecord.def.tags.Any(x=> tags.Contains(x))) && (partParent == null || bodyPartRecord.parent == partParent))
                {
                    yield return bodyPartRecord;
                }
                num = i;
            }
            yield break;
        }
        public static IEnumerable<BodyPartRecord> GetPartsWithTag(this BodyDef def, List<BodyPartTagDef> tags)
        {
            int num;
            for (int i = 0; i < def.AllParts.Count; i = num + 1)
            {
                BodyPartRecord bodyPartRecord = def.AllParts[i];
                for (int ii = 0; ii < tags.Count; ii++)
                {
                    BodyPartTagDef tag = tags[ii];
                    if (bodyPartRecord.def.tags.Contains(tag))
                    {
                        yield return bodyPartRecord;
                    }

                }
                num = i;
            }
            yield break;
        }

        public static object InvokeMethod(object obj, string methodName, params object[] methodParams)
        {
            var methodParamTypes = methodParams?.Select(p => p.GetType()).ToArray() ?? new Type[] { };
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
            MethodInfo method = null;
            var type = obj.GetType();
            while (method == null && type != null)
            {
                method = type.GetMethod(methodName, bindingFlags, Type.DefaultBinder, methodParamTypes, null);
                type = type.BaseType;
            }

            return method?.Invoke(obj, methodParams);
        }
    }
}
