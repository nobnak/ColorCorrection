using UnityEngine;
using System.Collections;
using System.IO;
using Gist;

namespace ColorCorrection {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class CameraGrading : MonoBehaviour {
        [SerializeField]
        protected Shader gradingShader;

        protected Material gradingMat;

        public LUT3D Lut { get; set; }

        #region Unity
        protected virtual void OnEnable() {
            gradingMat = new Material (gradingShader);
        }
        protected virtual void OnDisable() {
            Release (gradingMat);
        }
        protected virtual void OnRenderImage(RenderTexture src, RenderTexture dst) {
            if (Lut != null) {
                Lut.SetProperty (gradingMat);
                Graphics.Blit (src, dst, gradingMat);
            } else {
                Graphics.Blit (src, dst);
            }
        }
        #endregion

        #region Static
        public static void Release(Object obj) {
            if (Application.isPlaying)
                Destroy (obj);
            else
                DestroyImmediate (obj);
        }
        #endregion
    }
}
