using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(IdeoFoundation_Deity), "GenerateNewDeity")]
    public static class IdeoFoundation_Deity_GenerateNewDeity_FactionSpecifc_Patch
    {
        public static IdeoFoundation_Deity.Deity Postfix(IdeoFoundation_Deity.Deity __result, IdeoFoundation_Deity __instance)
        {
            IdeoFoundation_Deity.Deity deity = __result;
            if (__instance.ideo.culture is CultureDef culture)
            {
                if (culture.deities != null)
                {
                    bool possible = !culture.deities.possibleDeities.NullOrEmpty();
                    bool required = !culture.deities.requiredDeities.NullOrEmpty();
                    if (culture != null)
                    {
                        int max = culture.deities.max;
                        if (__instance.deities.Count >= max)
                        {
                            return __result;
                        }
                        StringBuilder st = new StringBuilder($"Adding Deity : required:{required} : possible:{required}");
                        if (required)
                        {
                            st.AppendLine($"Required: {culture.deities.requiredDeities.Count}");
                            List<DeityDef> requiredDefs = new List<DeityDef>();
                            foreach (var def in culture.deities.requiredDeities)
                            {
                                if (!__instance.deities.Any(x => x.name == def.label) && !requiredDefs.Contains(def))
                                {
                                    st.AppendLine($"    {def}");
                                    requiredDefs.Add(def);
                                }
                            }
                            if (!requiredDefs.NullOrEmpty())
                            {
                                deity = requiredDefs.RandomElement().Deity();
                                st.AppendLine($"using requiredDeity {deity.name}");
                                DeityUtility.FillDeity(__instance, deity);
                                Log.Message(st.ToString());
                                return deity;
                            }
                        }
                        if (possible)
                        {
                            st.AppendLine($"Optional: {culture.deities.possibleDeities.Count}");
                            List<DeityDef> possibleDefs = new List<DeityDef>();
                            foreach (var def in culture.deities.possibleDeities)
                            {
                                if (!__instance.deities.Any(x => x.name == def.label) && !possibleDefs.Contains(def))
                                {
                                    st.AppendLine($"    {def}");
                                    possibleDefs.Add(def);
                                }
                            }
                            if (!possibleDefs.NullOrEmpty())
                            {
                                deity = possibleDefs.RandomElement().Deity();
                                st.AppendLine($"using possibleDeity {deity.name}");
                                DeityUtility.FillDeity(__instance, deity);
                                Log.Message(st.ToString());
                                return deity;
                            }
                        }
                    }

                }
                if (__instance.ideo.culture.defName.StartsWith("OG_"))
                {
                    List<DeityDef> usedDefs = new List<DeityDef>();
                    if (__instance.ideo.culture.defName.Contains("Mechanicus"))
                    {
                        if (!__instance.deities.Any(x => x.name == DeityUtility.Omnissiah.name))
                        {
                            deity = DeityUtility.Omnissiah.cloneDeity();
                            DeityUtility.FillDeity(__instance, deity);
                            return deity;
                        }
                    }
                    if (__instance.ideo.culture.defName.Contains("Imperial"))
                    {
                        if (!__instance.deities.Any(x => x.name == DeityUtility.Emperor.name))
                        {
                            deity = DeityUtility.Emperor.cloneDeity();
                            DeityUtility.FillDeity(__instance, deity);
                            return deity;
                        }
                    }
                    if (__instance.ideo.culture.defName.Contains("Greenskin") || __instance.ideo.culture.defName.Contains("Orkoid"))
                    {
                        if (!__instance.deities.Any(x => x.name == DeityUtility.Gork.name))
                        {
                            deity = DeityUtility.Gork.cloneDeity();
                            DeityUtility.FillDeity(__instance, deity);
                            return deity;

                        }
                        if (!__instance.deities.Any(x => x.name == DeityUtility.Mork.name))
                        {
                            deity = DeityUtility.Mork.cloneDeity();
                            DeityUtility.FillDeity(__instance, deity);
                            return deity;
                        }
                    }
                    if (__instance.ideo.culture.defName.Contains("Kroot"))
                    {
                        if (!__instance.deities.Any(x => x.name == DeityUtility.Vawk.name))
                        {
                            deity = DeityUtility.Vawk.cloneDeity();
                            DeityUtility.FillDeity(__instance, deity);
                            return deity;
                        }
                        if (!__instance.deities.Any(x => x.name == DeityUtility.Gmork.name))
                        {
                            deity = DeityUtility.Gmork.cloneDeity();
                            DeityUtility.FillDeity(__instance, deity);
                            return deity;
                        }
                    }
                    if (__instance.ideo.culture.defName.Contains("Aeldari"))
                    {
                        if (!__instance.deities.Any(x => x.name == DeityUtility.Khaine.name))
                        {
                            deity = DeityUtility.Khaine.cloneDeity();
                            DeityUtility.FillDeity(__instance, deity);
                            return deity;

                        }
                        if (!__instance.deities.Any(x => x.name == DeityUtility.Ynnead.name))
                        {
                            deity = DeityUtility.Ynnead.cloneDeity();
                            DeityUtility.FillDeity(__instance, deity);
                            return deity;
                        }
                        if (!__instance.deities.Any(x => x.name == DeityUtility.Cegorach.name))
                        {
                            deity = DeityUtility.Cegorach.cloneDeity();
                            DeityUtility.FillDeity(__instance, deity);
                            return deity;
                        }
                    }
                }
            }
            return __result;
        }
    }
}
