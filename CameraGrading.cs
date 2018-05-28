using UnityEngine;
using System.Collections;
using System.IO;
using nobnak.Gist;
using nobnak.Gist.ObjectExt;

namespace ColorCorrection {
    
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class CameraGrading : MonoBehaviour {
		public const string MATERIAL_NAME = "ColorGrading";

		protected Material colorGradingMat;

        public LUT3D Lut { get; set; }

        #region Unity
        protected virtual void OnEnable() {
			colorGradingMat = Resources.Load<Material>(MATERIAL_NAME);
        }
        protected virtual void OnDisable() {
			Resources.UnloadAsset(colorGradingMat);
        }
        protected virtual void OnRenderImage(RenderTexture src, RenderTexture dst) {
            if (Lut != null) {
                Lut.SetProperty (colorGradingMat);
                Graphics.Blit (src, dst, colorGradingMat);
            } else {
                Graphics.Blit (src, dst);
            }
        }
        #endregion
    }
}
