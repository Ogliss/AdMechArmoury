﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Harmony;
using Verse.Sound;
using System.Reflection;
using UnityEngine;

namespace AdeptusMechanicus.Harmony
{
    [HarmonyPatch(typeof(PawnGenerator), "GenerateRandomAge")]
    public static class AM_PawnGenerator_GenerateRandomAge_test_Patch
    {
        [HarmonyPrefix]
        public static bool GeneratePawn_GenerateRandomAge_Postfix(Pawn pawn, PawnGenerationRequest request)
        {
            if (false)
            {
                if (request.FixedBiologicalAge != null && request.FixedChronologicalAge != null)
                {
                    float? fixedBiologicalAge = request.FixedBiologicalAge;
                    bool flag = fixedBiologicalAge != null;
                    float? fixedChronologicalAge = request.FixedChronologicalAge;
                    if ((flag & fixedChronologicalAge != null) && fixedBiologicalAge.GetValueOrDefault() > fixedChronologicalAge.GetValueOrDefault())
                    {
                        Log.Warning(string.Concat(new object[]
                        {
                        "Tried to generate age for pawn ",
                        pawn,
                        ", but pawn generation request demands biological age (",
                        request.FixedBiologicalAge,
                        ") to be greater than chronological age (",
                        request.FixedChronologicalAge,
                        ")."
                        }), false);
                    }
                }
                if (request.Newborn)
                {
                    pawn.ageTracker.AgeBiologicalTicks = 0L;
                }
                else if (request.FixedBiologicalAge != null)
                {
                    pawn.ageTracker.AgeBiologicalTicks = (long)(request.FixedBiologicalAge.Value * 3600000f);
                }
                else
                {
                    int num = 0;
                    float num2;
                    for (; ; )
                    {
                        if (pawn.RaceProps.ageGenerationCurve != null)
                        {
                            num2 = (float)Mathf.RoundToInt(Rand.ByCurve(pawn.RaceProps.ageGenerationCurve));
                        }
                        else if (pawn.RaceProps.IsMechanoid)
                        {
                            num2 = Rand.Range(0f, 2500f);
                        }
                        else
                        {
                            num2 = Rand.ByCurve(AM_PawnGenerator_GenerateRandomAge_test_Patch.DefaultAgeGenerationCurve) * pawn.RaceProps.lifeExpectancy;
                        }
                        num++;
                        if (num > 300)
                        {
                            break;
                        }
                        if (num2 <= (float)pawn.kindDef.maxGenerationAge && num2 >= (float)pawn.kindDef.minGenerationAge)
                        {
                            goto IL_1D4;
                        }
                    }
                    Log.Error("Tried 300 times to generate age for " + pawn, false);
                    IL_1D4:
                    pawn.ageTracker.AgeBiologicalTicks = (long)(num2 * 3600000f) + (long)Rand.Range(0, 3600000);
                }
                if (request.Newborn)
                {
                    pawn.ageTracker.AgeChronologicalTicks = 0L;
                }
                else if (request.FixedChronologicalAge != null)
                {
                    pawn.ageTracker.AgeChronologicalTicks = (long)(request.FixedChronologicalAge.Value * 3600000f);
                }
                else
                {
                    int num3;
                    if (request.CertainlyBeenInCryptosleep || Rand.Value < pawn.kindDef.backstoryCryptosleepCommonality)
                    {
                        float value = Rand.Value;
                        if (value < 0.7f)
                        {
                            num3 = Rand.Range(0, 100);
                        }
                        else if (value < 0.95f)
                        {
                            num3 = Rand.Range(100, 1000);
                        }
                        else
                        {
                            int max = GenDate.Year((long)GenTicks.TicksAbs, 0f) - 2026 - pawn.ageTracker.AgeBiologicalYears;
                            num3 = Rand.Range(1000, max);
                        }
                    }
                    else
                    {
                        num3 = 0;
                    }
                    int ticksAbs = GenTicks.TicksAbs;
                    long num4 = (long)ticksAbs - pawn.ageTracker.AgeBiologicalTicks;
                    num4 -= (long)num3 * 3600000L;
                    pawn.ageTracker.BirthAbsTicks = num4;
                }
                if (pawn.ageTracker.AgeBiologicalTicks > pawn.ageTracker.AgeChronologicalTicks)
                {
                    pawn.ageTracker.AgeChronologicalTicks = pawn.ageTracker.AgeBiologicalTicks;
                }
                return false;
            }
            return true;
        }
        
        // Token: 0x040034AC RID: 13484
        private static SimpleCurve DefaultAgeGenerationCurve = new SimpleCurve
        {
            {
                new CurvePoint(0.05f, 0f),
                true
            },
            {
                new CurvePoint(0.1f, 100f),
                true
            },
            {
                new CurvePoint(0.675f, 100f),
                true
            },
            {
                new CurvePoint(0.75f, 30f),
                true
            },
            {
                new CurvePoint(0.875f, 18f),
                true
            },
            {
                new CurvePoint(1f, 10f),
                true
            },
            {
                new CurvePoint(1.125f, 3f),
                true
            },
            {
                new CurvePoint(1.25f, 0f),
                true
            }
        };

    }

}
