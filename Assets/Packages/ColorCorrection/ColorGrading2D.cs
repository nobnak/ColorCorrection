using UnityEngine;
using System.Collections;
using System.IO;

namespace ColorCorrection {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
	public class ColorGrading2D : ColorGrading {
		public const int PASS = 0;

		public string propLut = "_Lut";

		protected override void SetProperty (Material mat) {
			UpdateKeyword ();
			mat.SetTexture (propLut, _lutImage);
		}
		protected override int GetPass () {
			return PASS;
		}
		protected override void PostUpateLUT (Texture2D lut) {
		}

		void UpdateKeyword () {
			filterMat.shaderKeywords = null;
			filterMat.EnableKeyword (LUT3D.KW_LUT2D);
		}
    }
}