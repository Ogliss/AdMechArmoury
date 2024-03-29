﻿using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000020 RID: 32
    public class AnyPrerequisiteResearchExtension : DefModExtension
    {
        public List<string> reseachDefs = new List<string>();

        public string ResearchKeyPrefix = string.Empty;
        public string ResearchKey = string.Empty;
        public string ResearchKeyPostfix = string.Empty;

        public List<string> Exclude = new List<string>();
        public List<string> Include = new List<string>();

        public bool UseHidden = true;

        private List<ResearchProjectDef> requiredResearch;
        public List<ResearchProjectDef> RequiredResearch
        {
            get
            {
                if (requiredResearch == null)
                {
                    requiredResearch = new List<ResearchProjectDef>();
                    //    log.message("generating required Research");
                    requiredResearch = DefDatabase<ResearchProjectDef>.AllDefsListForReading.FindAll(x => 
                        (Include.NullOrEmpty() ? true : Include.Any(y => x.defName.Contains(y))) && 
                        (Exclude.NullOrEmpty() ? true : !Exclude.Any(y => x.defName.Contains(y))) && 
                        (ResearchKeyPrefix.NullOrEmpty() ? true : x.defName.StartsWith(ResearchKeyPrefix)) && 
                        (ResearchKeyPrefix.NullOrEmpty() ? true : x.defName.EndsWith(ResearchKeyPostfix)) &&
                        x.defName.Contains(ResearchKey)
                        );
                }
                return requiredResearch;
            }

        }


        public bool AnyRequiredResearchCompleted
        {
            get
            {
                foreach (var item in RequiredResearch)
                {
                    if (item.IsFinished) return true;
                }
                return RequiredResearch.NullOrEmpty();
            }
        }
    }
}
