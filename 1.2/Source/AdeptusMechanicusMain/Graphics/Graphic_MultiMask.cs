using AdeptusMechanicus;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: AdeptusMechanicus.Graphic_MultiMask
    public class Graphic_MultiMask : Graphic_Multi
	{
		private Shader shader = ShaderTypeDefOf.CutoutComplex.Shader;
		private List<ShaderParameter> shaderParameters = new List<ShaderParameter>();
		public override void Init(GraphicRequest req)
		{
			ExtendedGraphicData extData = req.graphicData as ExtendedGraphicData;
            if (extData == null)
            {
				base.Init(req);
				return;
			}
			this.data = extData;
		//	Log.Message("Graphic_MultiMask Init");
			this.path = req.path;
			this.shaderParameters = req.shaderParameters;
			this.color = req.color;
			this.colorTwo = req.colorTwo;
			this.drawSize = req.drawSize;
			Texture2D[] array = new Texture2D[this.mats.Length];
			array[0] = ContentFinder<Texture2D>.Get(req.path + "_north", false);
			array[1] = ContentFinder<Texture2D>.Get(req.path + "_east", false);
			array[2] = ContentFinder<Texture2D>.Get(req.path + "_south", false);
			array[3] = ContentFinder<Texture2D>.Get(req.path + "_west", false);
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
				Log.Error("Failed to find any textures at " + req.path + " while constructing " + this.ToStringSafe<Graphic_MultiMask>(), false);
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
			string mask = extData.maskKey + extData.MaskSelector;
			if (shader.SupportsMaskTex())
			{
			//	Log.Message(path + mask + "_northm");
				array2[0] = ContentFinder<Texture2D>.Get(path + mask + "_northm", true);
				array2[1] = ContentFinder<Texture2D>.Get(path + mask + "_eastm", true);
				array2[2] = ContentFinder<Texture2D>.Get(path + mask + "_southm", true);
				array2[3] = ContentFinder<Texture2D>.Get(path + mask + "_westm", true);
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
			else
			{
			//	Log.Message("No Mask Support " + path + mask + "_northm");
			}
			for (int i = 0; i < this.mats.Length; i++)
			{
				MaterialRequest req2 = default(MaterialRequest);
				req2.mainTex = array[i];
				req2.shader = shader;
				req2.color = this.color;
				req2.colorTwo = this.colorTwo;
				req2.maskTex = array2[i];
				req2.shaderParameters = req.shaderParameters;
				this.mats[i] = MaterialPool.MatFrom(req2);
			}
		}

		// Token: 0x0600159E RID: 5534 RVA: 0x0007F177 File Offset: 0x0007D377
		public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
		{
		//	Log.Message("Graphic_MultiMask GetColoredVersion");
			Graphic_MultiMask graphic_ = GraphicDatabase.Get<Graphic_MultiMask>(this.path, newShader, this.drawSize, newColor, newColorTwo, this.data) as Graphic_MultiMask;
			
			return graphic_;
		}

		// Token: 0x0600159F RID: 5535 RVA: 0x0007F194 File Offset: 0x0007D394
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

		private void UpdateMats()
		{
			ExtendedGraphicData extData = this.data as ExtendedGraphicData;
		//	Log.Message("Graphic_MultiMask UpdateMats");
			Texture2D[] array = new Texture2D[this.mats.Length];
			array[0] = ContentFinder<Texture2D>.Get(path + "_north", false);
			array[1] = ContentFinder<Texture2D>.Get(path + "_east", false);
			array[2] = ContentFinder<Texture2D>.Get(path + "_south", false);
			array[3] = ContentFinder<Texture2D>.Get(path + "_west", false);
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
					array[0] = ContentFinder<Texture2D>.Get(path, false);
				}
			}
			if (array[0] == null)
			{
				Log.Error("Failed to find any textures at " + path + " while constructing " + this.ToStringSafe<Graphic_MultiMask>(), false);
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
			if (this.shader.SupportsMaskTex())
			{
				//	Log.Message(req.path + "_southm" + extData.maskKey + extData.MaskSelector);
				string mask = extData.maskKey + extData.MaskSelector;
				array2[0] = ContentFinder<Texture2D>.Get(path + mask + "_northm", true);
				array2[1] = ContentFinder<Texture2D>.Get(path + mask + "_eastm", true);
				array2[2] = ContentFinder<Texture2D>.Get(path + mask + "_southm", true);
				array2[3] = ContentFinder<Texture2D>.Get(path + mask + "_westm", true);
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
				this.mats[i].mainTexture = array[i];
				this.mats[i].color = color;
				this.mats[i].SetTexture(ShaderPropertyIDs.MaskTex, array2[i]);
				this.mats[i].SetColor(ShaderPropertyIDs.ColorTwo, colorTwo);
			}
		}
		// Token: 0x060015A0 RID: 5536 RVA: 0x0007F1F1 File Offset: 0x0007D3F1
		public override int GetHashCode()
		{
			return Gen.HashCombineStruct<Color>(Gen.HashCombineStruct<Color>(Gen.HashCombine<string>(0, this.path), this.color), this.colorTwo);
		}

		public new ExtendedGraphicData data;

		// Token: 0x04000E4B RID: 3659
		private Material[] mats = new Material[4];

		// Token: 0x04000E4C RID: 3660
		private bool westFlipped;

		// Token: 0x04000E4D RID: 3661
		private bool eastFlipped;

		// Token: 0x04000E4E RID: 3662
		private float drawRotatedExtraAngleOffset;
	}
}
