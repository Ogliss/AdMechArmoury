using System;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x02000029 RID: 41
	public class DropShipTraveling : TravelingTransportPods
	{
		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000138 RID: 312 RVA: 0x0000D3C8 File Offset: 0x0000B5C8
		private Material DropShipMaterial
		{
			get
			{
				bool flag = this.flyingThing == null;
				Material result;
				if (flag)
				{
					result = this.Material;
				}
				else
				{
					bool flag2 = this.material == null;
					if (flag2)
					{
						string text = this.flyingThing.def.graphicData.texPath;
						GraphicRequest graphicRequest = new GraphicRequest(this.flyingThing.def.graphicData.graphicClass, this.flyingThing.def.graphicData.texPath, ShaderTypeDefOf.Cutout.Shader, this.flyingThing.def.graphic.drawSize, Color.white, Color.white, this.flyingThing.def.graphicData, 0, null);
						bool flag3 = graphicRequest.graphicClass == typeof(Graphic_Multi);
						if (flag3)
						{
							text += "_north";
						}
						this.material = MaterialPool.MatFrom(text, ShaderDatabase.WorldOverlayTransparentLit, WorldMaterials.WorldObjectRenderQueue);
					}
					result = ((ExpandableWorldObjectsUtility.TransitionPct > 0f) ? this.flyingThing.Graphic.MatNorth : this.material);
				}
				return result;
			}
		}

		// Token: 0x06000139 RID: 313 RVA: 0x0000D4E9 File Offset: 0x0000B6E9
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look<Thing>(ref this.flyingThing, "flyingThing", false);
		}


		/*
		// Token: 0x0600013A RID: 314 RVA: 0x0000D508 File Offset: 0x0000B708
		public override void Draw()
		{
			bool flag = !SRTSMod.mod.settings.dynamicWorldDrawingSRTS;
			if (flag)
			{
				base.Draw();
			}
			else
			{
				bool flag2 = !this.HiddenBehindTerrainNow();
				if (flag2)
				{
					float averageTileSize = Find.WorldGrid.averageTileSize;
					float transitionPct = ExpandableWorldObjectsUtility.TransitionPct;
					bool flag3 = this.transitionSize < 1f;
					if (flag3)
					{
						this.transitionSize += 0.015f * (float)Find.TickManager.CurTimeSpeed;
					}
					float num = (1f + transitionPct * Find.WorldCameraDriver.AltitudePercent * 35f) * this.transitionSize;
					bool flag4 = this.directionFacing == default(Vector3);
					if (flag4)
					{
						this.InitializeFacing();
					}
					Vector3 normalized = this.DrawPos.normalized;
					Quaternion q = Quaternion.LookRotation(Vector3.Cross(normalized, this.directionFacing), normalized) * Quaternion.Euler(0f, 90f, 0f);
					Vector3 s = new Vector3(averageTileSize * 0.7f * num, 5f, averageTileSize * 0.7f * num);
					Matrix4x4 matrix = default(Matrix4x4);
					matrix.SetTRS(this.DrawPos + normalized * 0.015f, q, s);
					int worldLayer = WorldCameraManager.WorldLayer;
					Graphics.DrawMesh(MeshPool.plane10, matrix, this.DropShipMaterial, worldLayer);
				}
			}
		}
		*/

		// Token: 0x0600013B RID: 315 RVA: 0x0000D678 File Offset: 0x0000B878
		private void InitializeFacing()
		{
			Vector3 normalized = Find.WorldGrid.GetTileCenter(this.destinationTile).normalized;
			this.directionFacing = (this.DrawPos - normalized).normalized;
		}

		// Token: 0x040000B6 RID: 182
		public Thing flyingThing;

		// Token: 0x040000B7 RID: 183
		private Material material;

		// Token: 0x040000B8 RID: 184
		private const float ExpandingResize = 35f;

		// Token: 0x040000B9 RID: 185
		private const float TransitionTakeoff = 0.015f;

		// Token: 0x040000BA RID: 186
		private float transitionSize = 0f;

		// Token: 0x040000BB RID: 187
		private Vector3 directionFacing;
	}
}
