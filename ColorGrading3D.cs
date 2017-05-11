using UnityEngine;
using System.Collections;
using System.IO;

namespace ColorCorrection {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
	public class ColorGrading3D : ColorGrading {
		
        LUT3D _lut;

		protected override void OnEnable() {
			base.OnEnable ();
            _lut = new LUT3D ();
        }
		protected override void OnDisable() {
			base.OnDisable ();
            _lut.Dispose ();
        }
		protected override void UpdateLUT (Texture2D lut) {
			_lut.Convert(lut);
        }
		protected override void SetProperty (Material mat) {
			UpdateKeyword ();
			_lut.SetProperty (mat);
		}
		protected override int GetPass () {
			return _lut.Pass;
		}

		void UpdateKeyword () {
			filterMat.shaderKeywords = null;
			filterMat.EnableKeyword (LUT3D.KW_LUT3D);
		}
    }
}