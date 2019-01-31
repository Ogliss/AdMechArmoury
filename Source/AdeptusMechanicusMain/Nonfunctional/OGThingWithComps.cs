using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;

namespace Verse
{
    // Token: 0x02000E51 RID: 3665
    public class OGThingWithComps : Thing
    {
        // Token: 0x17000D8B RID: 3467
        // (get) Token: 0x0600537E RID: 21374 RVA: 0x0010D4B8 File Offset: 0x0010B8B8
        public List<ThingComp> AllComps
        {
            get
            {
                if (this.comps == null)
                {
                    return OGThingWithComps.EmptyCompsList;
                }
                return this.comps;
            }
        }

        // Token: 0x17000D8C RID: 3468
        // (get) Token: 0x0600537F RID: 21375 RVA: 0x0010D4D4 File Offset: 0x0010B8D4
        // (set) Token: 0x06005380 RID: 21376 RVA: 0x0010D506 File Offset: 0x0010B906
        public override Color DrawColor
        {
            get
            {
                CompColorable comp = this.GetComp<CompColorable>();
                if (comp != null && comp.Active)
                {
                    return comp.Color;
                }
                return base.DrawColor;
            }
            set
            {
                this.SetColor(value, true);
            }
        }

        // Token: 0x17000D8D RID: 3469
        // (get) Token: 0x06005381 RID: 21377 RVA: 0x0010D510 File Offset: 0x0010B910
        public override string LabelNoCount
        {
            get
            {
                string text = base.LabelNoCount;
                if (this.comps != null)
                {
                    int i = 0;
                    int count = this.comps.Count;
                    while (i < count)
                    {
                        text = this.comps[i].TransformLabel(text);
                        i++;
                    }
                }
                return text;
            }
        }

        // Token: 0x17000D8E RID: 3470
        // (get) Token: 0x06005382 RID: 21378 RVA: 0x0010D564 File Offset: 0x0010B964
        public override string DescriptionFlavor
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(base.DescriptionFlavor);
                if (this.comps != null)
                {
                    for (int i = 0; i < this.comps.Count; i++)
                    {
                        string descriptionPart = this.comps[i].GetDescriptionPart();
                        if (!descriptionPart.NullOrEmpty())
                        {
                            if (stringBuilder.Length > 0)
                            {
                                stringBuilder.AppendLine();
                                stringBuilder.AppendLine();
                            }
                            stringBuilder.Append(descriptionPart);
                        }
                    }
                }
                return stringBuilder.ToString();
            }
        }

        // Token: 0x06005383 RID: 21379 RVA: 0x0010D5F0 File Offset: 0x0010B9F0
        public override void PostMake()
        {
            base.PostMake();
            this.InitializeComps();
        }

        // Token: 0x06005384 RID: 21380 RVA: 0x0010D600 File Offset: 0x0010BA00
        public T GetComp<T>() where T : ThingComp
        {
            if (this.comps != null)
            {
                int i = 0;
                int count = this.comps.Count;
                while (i < count)
                {
                    T t = this.comps[i] as T;
                    if (t != null)
                    {
                        return t;
                    }
                    i++;
                }
            }
            return (T)((object)null);
        }

        // Token: 0x06005385 RID: 21381 RVA: 0x0010D660 File Offset: 0x0010BA60
        public IEnumerable<T> GetComps<T>() where T : ThingComp
        {
            if (this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    T cT = this.comps[i] as T;
                    if (cT != null)
                    {
                        yield return cT;
                    }
                }
            }
            yield break;
        }

        // Token: 0x06005386 RID: 21382 RVA: 0x0010D684 File Offset: 0x0010BA84
        public ThingComp GetCompByDef(CompProperties def)
        {
            if (this.comps != null)
            {
                int i = 0;
                int count = this.comps.Count;
                while (i < count)
                {
                    if (this.comps[i].props == def)
                    {
                        return this.comps[i];
                    }
                    i++;
                }
            }
            return null;
        }

        // Token: 0x06005387 RID: 21383 RVA: 0x0010D6E0 File Offset: 0x0010BAE0
        public void InitializeComps()
        {
            if (this.def.comps.Any<CompProperties>())
            {
                this.comps = new List<ThingComp>();
                for (int i = 0; i < this.def.comps.Count; i++)
                {
                    ThingComp thingComp = (ThingComp)Activator.CreateInstance(this.def.comps[i].compClass);
                    thingComp.parent = this;
                    this.comps.Add(thingComp);
                    thingComp.Initialize(this.def.comps[i]);
                }
            }
        }

        // Token: 0x06005388 RID: 21384 RVA: 0x0010D77C File Offset: 0x0010BB7C
        public override string GetCustomLabelNoCount(bool includeHp = true)
        {
            string text = base.GetCustomLabelNoCount(includeHp);
            if (this.comps != null)
            {
                int i = 0;
                int count = this.comps.Count;
                while (i < count)
                {
                    text = this.comps[i].TransformLabel(text);
                    i++;
                }
            }
            return text;
        }

        // Token: 0x06005389 RID: 21385 RVA: 0x0010D7D0 File Offset: 0x0010BBD0
        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                this.InitializeComps();
            }
            if (this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].PostExposeData();
                }
            }
        }

        // Token: 0x0600538A RID: 21386 RVA: 0x0010D82C File Offset: 0x0010BC2C
        public void BroadcastCompSignal(string signal)
        {
            this.ReceiveCompSignal(signal);
            if (this.comps != null)
            {
                int i = 0;
                int count = this.comps.Count;
                while (i < count)
                {
                    this.comps[i].ReceiveCompSignal(signal);
                    i++;
                }
            }
        }

        // Token: 0x0600538B RID: 21387 RVA: 0x0010D87B File Offset: 0x0010BC7B
        protected virtual void ReceiveCompSignal(string signal)
        {
        }

        // Token: 0x0600538C RID: 21388 RVA: 0x0010D880 File Offset: 0x0010BC80
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].PostSpawnSetup(respawningAfterLoad);
                }
            }
        }

        // Token: 0x0600538D RID: 21389 RVA: 0x0010D8D0 File Offset: 0x0010BCD0
        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            Map map = base.Map;
            base.DeSpawn(mode);
            if (this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].PostDeSpawn(map);
                }
            }
        }

        // Token: 0x0600538E RID: 21390 RVA: 0x0010D924 File Offset: 0x0010BD24
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            Map map = base.Map;
            base.Destroy(mode);
            if (this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].PostDestroy(mode, map);
                }
            }
        }

        // Token: 0x0600538F RID: 21391 RVA: 0x0010D97C File Offset: 0x0010BD7C
        public override void Tick()
        {
            if (this.comps != null)
            {
                int i = 0;
                int count = this.comps.Count;
                while (i < count)
                {
                    this.comps[i].CompTick();
                    i++;
                }
            }
        }

        // Token: 0x06005390 RID: 21392 RVA: 0x0010D9C4 File Offset: 0x0010BDC4
        public override void TickRare()
        {
            if (this.comps != null)
            {
                int i = 0;
                int count = this.comps.Count;
                while (i < count)
                {
                    this.comps[i].CompTickRare();
                    i++;
                }
            }
        }

        // Token: 0x06005391 RID: 21393 RVA: 0x0010DA0C File Offset: 0x0010BE0C
        public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            base.PreApplyDamage(ref dinfo, out absorbed);
            if (absorbed)
            {
                return;
            }
            if (this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].PostPreApplyDamage(dinfo, out absorbed);
                    if (absorbed)
                    {
                        return;
                    }
                }
            }
        }

        // Token: 0x06005392 RID: 21394 RVA: 0x0010DA70 File Offset: 0x0010BE70
        public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostApplyDamage(dinfo, totalDamageDealt);
            if (this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].PostPostApplyDamage(dinfo, totalDamageDealt);
                }
            }
        }

        // Token: 0x06005393 RID: 21395 RVA: 0x0010DABF File Offset: 0x0010BEBF
        public override void Draw()
        {
            base.Draw();
            this.Comps_PostDraw();
        }

        // Token: 0x06005394 RID: 21396 RVA: 0x0010DAD0 File Offset: 0x0010BED0
        protected void Comps_PostDraw()
        {
            if (this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].PostDraw();
                }
            }
        }

        // Token: 0x06005395 RID: 21397 RVA: 0x0010DB18 File Offset: 0x0010BF18
        public override void DrawExtraSelectionOverlays()
        {
            base.DrawExtraSelectionOverlays();
            if (this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].PostDrawExtraSelectionOverlays();
                }
            }
        }

        // Token: 0x06005396 RID: 21398 RVA: 0x0010DB64 File Offset: 0x0010BF64
        public override void Print(SectionLayer layer)
        {
            base.Print(layer);
            if (this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].PostPrintOnto(layer);
                }
            }
        }

        // Token: 0x06005397 RID: 21399 RVA: 0x0010DBB4 File Offset: 0x0010BFB4
        public virtual void PrintForPowerGrid(SectionLayer layer)
        {
            if (this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].CompPrintForPowerGrid(layer);
                }
            }
        }

        // Token: 0x06005398 RID: 21400 RVA: 0x0010DBFC File Offset: 0x0010BFFC
        public override IEnumerable<Gizmo> GetGizmos()
        {
            string logroll = string.Format("getting gizmos OGThingWithComps");
            Log.Message(logroll);
            if (this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    foreach (Gizmo com in this.comps[i].CompGetGizmosExtra())
                    {
                        yield return com;
                    }
                }
            }
            yield break;
        }

        // Token: 0x06005399 RID: 21401 RVA: 0x0010DC20 File Offset: 0x0010C020
        public override bool TryAbsorbStack(Thing other, bool respectStackLimit)
        {
            if (!this.CanStackWith(other))
            {
                return false;
            }
            int count = ThingUtility.TryAbsorbStackNumToTake(this, other, respectStackLimit);
            if (this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].PreAbsorbStack(other, count);
                }
            }
            return base.TryAbsorbStack(other, respectStackLimit);
        }

        // Token: 0x0600539A RID: 21402 RVA: 0x0010DC88 File Offset: 0x0010C088
        public override Thing SplitOff(int count)
        {
            Thing thing = base.SplitOff(count);
            if (thing != null && this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].PostSplitOff(thing);
                }
            }
            return thing;
        }

        // Token: 0x0600539B RID: 21403 RVA: 0x0010DCE0 File Offset: 0x0010C0E0
        public override bool CanStackWith(Thing other)
        {
            if (!base.CanStackWith(other))
            {
                return false;
            }
            if (this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    if (!this.comps[i].AllowStackWith(other))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // Token: 0x0600539C RID: 21404 RVA: 0x0010DD3C File Offset: 0x0010C13C
        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());
            string text = this.InspectStringPartsFromComps();
            if (!text.NullOrEmpty())
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.AppendLine();
                }
                stringBuilder.Append(text);
            }
            return stringBuilder.ToString();
        }

        // Token: 0x0600539D RID: 21405 RVA: 0x0010DD90 File Offset: 0x0010C190
        protected string InspectStringPartsFromComps()
        {
            if (this.comps == null)
            {
                return null;
            }
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < this.comps.Count; i++)
            {
                string text = this.comps[i].CompInspectStringExtra();
                if (!text.NullOrEmpty())
                {
                    if (Prefs.DevMode && char.IsWhiteSpace(text[text.Length - 1]))
                    {
                        Log.ErrorOnce(this.comps[i].GetType() + " CompInspectStringExtra ended with whitespace: " + text, 25612, false);
                        text = text.TrimEndNewlines();
                    }
                    if (stringBuilder.Length != 0)
                    {
                        stringBuilder.AppendLine();
                    }
                    stringBuilder.Append(text);
                }
            }
            return stringBuilder.ToString();
        }

        // Token: 0x0600539E RID: 21406 RVA: 0x0010DE5C File Offset: 0x0010C25C
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (FloatMenuOption o in base.GetFloatMenuOptions(selPawn))
            {
                yield return o;
            }
            if (this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    foreach (FloatMenuOption o2 in this.comps[i].CompFloatMenuOptions(selPawn))
                    {
                        yield return o2;
                    }
                }
            }
            yield break;
        }

        // Token: 0x0600539F RID: 21407 RVA: 0x0010DE88 File Offset: 0x0010C288
        public override void PreTraded(TradeAction action, Pawn playerNegotiator, ITrader trader)
        {
            if (this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].PrePreTraded(action, playerNegotiator, trader);
                }
            }
            base.PreTraded(action, playerNegotiator, trader);
        }

        // Token: 0x060053A0 RID: 21408 RVA: 0x0010DEDC File Offset: 0x0010C2DC
        public override void PostGeneratedForTrader(TraderKindDef trader, int forTile, Faction forFaction)
        {
            base.PostGeneratedForTrader(trader, forTile, forFaction);
            if (this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].PostPostGeneratedForTrader(trader, forTile, forFaction);
                }
            }
        }

        // Token: 0x060053A1 RID: 21409 RVA: 0x0010DF30 File Offset: 0x0010C330
        protected override void PostIngested(Pawn ingester)
        {
            base.PostIngested(ingester);
            if (this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].PostIngested(ingester);
                }
            }
        }

        // Token: 0x060053A2 RID: 21410 RVA: 0x0010DF80 File Offset: 0x0010C380
        public override void Notify_SignalReceived(Signal signal)
        {
            base.Notify_SignalReceived(signal);
            if (this.comps != null)
            {
                for (int i = 0; i < this.comps.Count; i++)
                {
                    this.comps[i].Notify_SignalReceived(signal);
                }
            }
        }

        // Token: 0x04003721 RID: 14113
        private List<ThingComp> comps;

        // Token: 0x04003722 RID: 14114
        private static readonly List<ThingComp> EmptyCompsList = new List<ThingComp>();

        public static implicit operator ThingWithComps(OGThingWithComps v)
        {
            throw new NotImplementedException();
        }
    }
}
