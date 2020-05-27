﻿using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x0200024E RID: 590
    public class HediffCompProperties_MoveThroughCover : HediffCompProperties
    {
        // Token: 0x06000AB1 RID: 2737 RVA: 0x0005598C File Offset: 0x00053D8C
        public HediffCompProperties_MoveThroughCover()
        {
            this.compClass = typeof(HediffComp_MoveThroughCover);
        }
        public bool MoveThroughAll = false;
        public List<string> TerrainTagsList = new List<string>();
        public List<string> BuildingClassList = new List<string>();
    }
    // Token: 0x02000C69 RID: 3177
    public class HediffComp_MoveThroughCover : HediffComp
    {
        public new HediffCompProperties_MoveThroughCover Props
        {
            get
            {
                return (HediffCompProperties_MoveThroughCover)this.props;
            }
        }
        public Map Map
        {
            get
            {
                return Pawn.Map;
            }
        }
        public IntVec3 Position
        {
            get
            {
                return Map==null ? Pawn.PositionHeld : Pawn.Position;
            }
        }

        public TerrainDef CurTerrain
        {
            get
            {
                return Position.GetTerrain(Map);
            }
        }

        public Building CurrBuilding
        {
            get
            {
                return Position.GetFirstBuilding(Map);
            }
        }

        public bool Active
        {
            get
            {
                bool result = false;
                if (!Props.TerrainTagsList.NullOrEmpty())
                {
                    foreach (string tag in Props.TerrainTagsList)
                    {
                        if (CurTerrain.HasTag(tag))
                        {
                            result = true;
                            break;
                        }
                    }
                }
                if (!Props.BuildingClassList.NullOrEmpty())
                {
                    foreach (string tag in Props.BuildingClassList)
                    {
                        if (CurrBuilding.def.thingClass.FullName.Contains(tag))
                        {
                            result = true;
                            break;
                        }
                    }
                }
                if (Props.MoveThroughAll)
                {
                    result = true;
                }
            //    Log.Warning(string.Format("{0}",result));
                return result;
            }
        }
    }
}
