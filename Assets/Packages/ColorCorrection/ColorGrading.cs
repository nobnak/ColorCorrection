using UnityEngine;
using System.Collections;
using System.IO;
using Gist;

namespace ColorCorrection {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public abstract class ColorGrading : Settings<ColorGrading.Data> {
        public Material filterMat;

        protected Texture2D _lutImage;
        protected System.DateTime _lastUpdateTime;

		protected abstract void SetProperty (Material mat);
		protected abstract int GetPass();
		protected abstract void PostUpateLUT (Texture2D lut);

        protected override void OnEnable() {
            base.OnEnable ();
            _lastUpdateTime = System.DateTime.MinValue;
        }
        protected override void OnDisable() {
            ReleaseImageTex();
            base.OnDisable ();
        }
        protected virtual void OnRenderImage(RenderTexture src, RenderTexture dst) {
            if (filterMat == null || _lutImage == null || data.debugMode == Data.DebugModeEnum.Pass) {
                Graphics.Blit (src, dst);
                return;
            }
            SetProperty (filterMat);
			Graphics.Blit (src, dst, filterMat, GetPass());
        }
        protected override void Update() {
            base.Update ();
            var now = System.DateTime.Now;
            var d = now - _lastUpdateTime;
            if (d.TotalSeconds > data.updateInterval) {
                _lastUpdateTime = now;
                UpdateLUT ();
            }
        }
        protected override void DrawGUI () {
            data.lutImageName = GUILayout.TextField (data.lutImageName);
        }

        protected virtual string GetPath() {
            return MakePath (data.dataPath, data.lutImageName);
        }
        protected virtual void UpdateLUT() {
            try {
                var path = GetPath ();
                if (!File.Exists (path)) {
                    ReleaseImageTex();
                    return;
                }

                var writeTime = File.GetLastWriteTime (path);
                if (writeTime != _lastUpdateTime) {
                    _lastUpdateTime = writeTime;

					if (_lutImage == null) {
						_lutImage = new Texture2D(2, 2, TextureFormat.ARGB32, false, true);
						_lutImage.filterMode = FilterMode.Bilinear;
						_lutImage.wrapMode = TextureWrapMode.Clamp;
						_lutImage.anisoLevel = 0;
					}
                    _lutImage.LoadImage(File.ReadAllBytes(path));
					PostUpateLUT (_lutImage);
                }
            } catch (System.Exception e) {
                Debug.LogError (e);
            }
        }
        void ReleaseImageTex () {
            if (_lutImage != null)
                DestroyImmediate (_lutImage);
        }

        [System.Serializable]
        public class Data {
            public enum DebugModeEnum { Normal = 0, Pass }

            public float updateInterval = 0.5f;
            public Settings.PathTypeEnum dataPath;
            public DebugModeEnum debugMode;
            public string lutImageName = "ColorGrading.png";
        }
    }
}
