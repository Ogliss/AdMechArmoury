using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x020002F0 RID: 752
	public class Graphic_Pauldron : Graphic
	{
		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x0600153C RID: 5436 RVA: 0x0007CB48 File Offset: 0x0007AD48
		public string GraphicPath
		{
			get
			{
				return this.path;
			}
		}

		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x0600153D RID: 5437 RVA: 0x0007CB50 File Offset: 0x0007AD50
		public override Material MatSingle
		{
			get
			{
				return this.MatSouth;
			}
		}

		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x0600153E RID: 5438 RVA: 0x0007CB58 File Offset: 0x0007AD58
		public override Material MatWest
		{
			get
			{
				return this.Mats[3];
			}
		}

		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x0600153F RID: 5439 RVA: 0x0007CB62 File Offset: 0x0007AD62
		public override Material MatSouth
		{
			get
			{
				return this.Mats[2];
			}
		}

		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x06001540 RID: 5440 RVA: 0x0007CB6C File Offset: 0x0007AD6C
		public override Material MatEast
		{
			get
			{
				return this.Mats[1];
			}
		}

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x06001541 RID: 5441 RVA: 0x0007CB76 File Offset: 0x0007AD76
		public override Material MatNorth
		{
			get
			{
				return this.Mats[0];
			}
		}

		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x06001542 RID: 5442 RVA: 0x0007CB80 File Offset: 0x0007AD80
		public override bool WestFlipped
		{
			get
			{
				return this.westFlipped;
			}
		}

		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x06001543 RID: 5443 RVA: 0x0007CB88 File Offset: 0x0007AD88
		public override bool EastFlipped
		{
			get
			{
				return this.eastFlipped;
			}
		}

		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x06001544 RID: 5444 RVA: 0x0007CB90 File Offset: 0x0007AD90
		public override bool ShouldDrawRotated
		{
			get
			{
				return (this.data == null || this.data.drawRotated) && (this.MatEast == this.MatNorth || this.MatWest == this.MatNorth);
			}
		}

		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x06001545 RID: 5445 RVA: 0x0007CBCF File Offset: 0x0007ADCF
		public override float DrawRotatedExtraAngleOffset
		{
			get
			{
				return this.drawRotatedExtraAngleOffset;
			}
		}

		// Token: 0x06001546 RID: 5446 RVA: 0x0007CBD8 File Offset: 0x0007ADD8
		public override void Init(GraphicRequest req)
		{
			this.data = req.graphicData;
			this.path = req.path;
			this.color = req.color;
			this.colorTwo = req.colorTwo;
			this.drawSize = req.drawSize;

			List<Texture2D>[] textures = new List<Texture2D>[this.mats.Length];
			List<Texture2D> list = (from x in ContentFinder<Texture2D>.GetAllInFolder(req.path)
									where !x.name.EndsWith("m") // &&  !x.name.Contains(Graphic_AdvancedMulti.NSuffix) && !x.name.Contains(Graphic_AdvancedMulti.SSuffix) && !x.name.Contains(Graphic_AdvancedMulti.ESuffix) && !x.name.Contains(Graphic_AdvancedMulti.WSuffix)
									orderby x.name
									select x).ToList<Texture2D>();
			if (list.NullOrEmpty<Texture2D>())
			{
				Log.Error("Collection cannot init: No textures found at path " + req.path, false);
			}
			textures[0].AddRange(list.FindAll(x => x.name.Contains("_north")));
			list.RemoveAll(x => textures[0].Contains(x));
			textures[1].AddRange(list.FindAll(x => x.name.Contains("_east")));
			list.RemoveAll(x => textures[1].Contains(x));
			textures[2].AddRange(list.FindAll(x => x.name.Contains("_south")));
			list.RemoveAll(x => textures[2].Contains(x));
			textures[3].AddRange(list.FindAll(x => x.name.Contains("_west")));
			list.RemoveAll(x => textures[3].Contains(x));
			Texture2D[] array = new Texture2D[this.mats.Length];
			array[0] = textures[0].First();
			array[1] = textures[1].First();
			array[2] = textures[2].First();
			array[3] = textures[3].First();
			if (array[0] == null)
			{
				if (array[2] != null)
				{
					array[0] = array[2];
					this.drawRotatedExtraAngleOffset = 180f;
				}
				else if (array[1] != null)
				{
					array[0] = array[1];
					this.drawRotatedExtraAngleOffset = -90f;
				}
				else if (array[3] != null)
				{
					array[0] = array[3];
					this.drawRotatedExtraAngleOffset = 90f;
				}
				else
				{
					array[0] = ContentFinder<Texture2D>.Get(req.path, false);
				}
			}
			if (array[0] == null)
			{
				Log.Error("Failed to find any textures at " + req.path + " while constructing " + this.ToStringSafe<Graphic_Pauldron>(), false);
				return;
			}
			if (array[2] == null)
			{
				array[2] = array[0];
			}
			if (array[1] == null)
			{
				if (array[3] != null)
				{
					array[1] = array[3];
					this.eastFlipped = base.DataAllowsFlip;
				}
				else
				{
					array[1] = array[0];
				}
			}
			if (array[3] == null)
			{
				if (array[1] != null)
				{
					array[3] = array[1];
					this.westFlipped = base.DataAllowsFlip;
				}
				else
				{
					array[3] = array[0];
				}
			}
			Texture2D[] array2 = new Texture2D[this.mats.Length];
			List<Texture2D>[] textures2 = new List<Texture2D>[this.mats.Length];
			if (req.shader.SupportsMaskTex())
			{
				List<Texture2D> list2 = (from x in ContentFinder<Texture2D>.GetAllInFolder(req.path)
										where x.name.EndsWith("m") // &&  !x.name.Contains(Graphic_AdvancedMulti.NSuffix) && !x.name.Contains(Graphic_AdvancedMulti.SSuffix) && !x.name.Contains(Graphic_AdvancedMulti.ESuffix) && !x.name.Contains(Graphic_AdvancedMulti.WSuffix)
										orderby x.name
										select x).ToList<Texture2D>();
				if (list2.NullOrEmpty<Texture2D>())
				{
					Log.Error("Collection cannot init: No textures found at path " + req.path, false);
				}
				textures[0].AddRange(list2.FindAll(x => x.name.Contains("_north")));
				list2.RemoveAll(x => textures[0].Contains(x));
				textures[1].AddRange(list2.FindAll(x => x.name.Contains("_east")));
				list2.RemoveAll(x => textures[1].Contains(x));
				textures[2].AddRange(list2.FindAll(x => x.name.Contains("_south")));
				list2.RemoveAll(x => textures[2].Contains(x));
				textures[3].AddRange(list2.FindAll(x => x.name.Contains("_west")));
				list2.RemoveAll(x => textures[3].Contains(x));
				array2[0] = textures2[0].First();
				array2[1] = textures2[1].First();
				array2[2] = textures2[2].First();
				array2[3] = textures2[3].First();
				if (array2[0] == null)
				{
					if (array2[2] != null)
					{
						array2[0] = array2[2];
					}
					else if (array2[1] != null)
					{
						array2[0] = array2[1];
					}
					else if (array2[3] != null)
					{
						array2[0] = array2[3];
					}
				}
				if (array2[2] == null)
				{
					array2[2] = array2[0];
				}
				if (array2[1] == null)
				{
					if (array2[3] != null)
					{
						array2[1] = array2[3];
					}
					else
					{
						array2[1] = array2[0];
					}
				}
				if (array2[3] == null)
				{
					if (array2[1] != null)
					{
						array2[3] = array2[1];
					}
					else
					{
						array2[3] = array2[0];
					}
				}
			}
			for (int i = 0; i < this.mats.Length; i++)
			{
				for (int ii = 0; ii < textures[i].Count; ii++)
				{

					array[i] = textures[i][ii];
					array2[i] = textures2[i][ii];
					MaterialRequest req2 = default(MaterialRequest);
					req2.mainTex = array[i];
					req2.shader = req.shader;
					req2.color = this.color;
					req2.colorTwo = this.colorTwo;
					req2.maskTex = array2[i];
					req2.shaderParameters = req.shaderParameters;
					this.mats[i].Add(MaterialPool.MatFrom(req2));
				}
			}
		}

		public override Material MatAt(Rot4 rot, Thing thing = null)
		{
			if (thing!=null)
			{
				SubGraphicFor(thing);
			}
			switch (rot.AsInt)
			{
				case 0:
					return this.MatNorth;
				case 1:
					return this.MatEast;
				case 2:
					return this.MatSouth;
				case 3:
					return this.MatWest;
				default:
					return BaseContent.BadMat;
			}
		}
		// Token: 0x06001547 RID: 5447 RVA: 0x0007CF33 File Offset: 0x0007B133
		public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
		{
			return GraphicDatabase.Get<Graphic_Multi>(this.path, newShader, this.drawSize, newColor, newColorTwo, this.data);
		}
		/*
		// Token: 0x06001564 RID: 5476 RVA: 0x0007CDD6 File Offset: 0x0007AFD6
		public override Material MatAt(Rot4 rot, Thing thing = null)
		{
			if (thing == null)
			{
				return this.MatSingle;
			}
			return this.MatSingleFor(thing);
		}
		// Token: 0x06001565 RID: 5477 RVA: 0x0007D26D File Offset: 0x0007B46D
		public override Material MatSingleFor(Thing thing)
		{
			if (thing == null)
			{
				return this.MatSingle;
			}
			return this.SubGraphicFor(thing).MatSingle;
		}
		
		*/
		// Token: 0x06001566 RID: 5478 RVA: 0x0007D285 File Offset: 0x0007B485
		public void SubGraphicFor(Thing thing)
		{
			CompPauldronDrawer comp = thing.TryGetComp<CompPauldronDrawer>();
			if (comp!=null)
			{
				if (!comp.ShoulderPadEntry.tagged.NullOrEmpty())
				{
					Mats[0] = mats[0].FirstOrFallback(x=> x.name.Contains(comp.ShoulderPadEntry.tagged), mats[0][0]);
					Mats[1] = mats[1].FirstOrFallback(x=> x.name.Contains(comp.ShoulderPadEntry.tagged), mats[1][0]);
					Mats[2] = mats[2].FirstOrFallback(x=> x.name.Contains(comp.ShoulderPadEntry.tagged), mats[2][0]);
					Mats[3] = mats[3].FirstOrFallback(x=> x.name.Contains(comp.ShoulderPadEntry.tagged), mats[3][0]);
				}
			}
		}

		// Token: 0x06001548 RID: 5448 RVA: 0x0007CF50 File Offset: 0x0007B150
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Multi(initPath=",
				this.path,
				", color=",
				this.color,
				", colorTwo=",
				this.colorTwo,
				")"
			});
		}

		// Token: 0x06001549 RID: 5449 RVA: 0x0007CFAD File Offset: 0x0007B1AD
		public override int GetHashCode()
		{
			return Gen.HashCombineStruct<Color>(Gen.HashCombineStruct<Color>(Gen.HashCombine<string>(0, this.path), this.color), this.colorTwo);
		}

		// Token: 0x04000DFF RID: 3583
		private List<Material>[] mats = new List<Material>[4];
		private Material[] Mats = new Material[4];

		// Token: 0x04000E00 RID: 3584
		private bool westFlipped;

		// Token: 0x04000E01 RID: 3585
		private bool eastFlipped;

		// Token: 0x04000E02 RID: 3586
		private float drawRotatedExtraAngleOffset;
	}
}
