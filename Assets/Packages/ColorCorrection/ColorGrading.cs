using UnityEngine;
using System.Collections;
using System.IO;

namespace ColorCorrection {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public abstract class ColorGrading : MonoBehaviour {
        public enum DataPathEnum { StreamingAssets = 0, MyDocuments }
        public enum DebugModeEnum { Disabled = 0, Pass }

		public float updateInterval = 0.5f;
        public DataPathEnum dataPath;
        public DebugModeEnum debugMode;
        public string lutImageName = "ColorGrading.png";
        public Material filterMat;

        protected Texture2D _lutImage;
        protected System.DateTime _lastUpdateTime;

		protected abstract void SetProperty (Material mat);
		protected abstract int GetPass();
		protected abstract void PostUpateLUT (Texture2D lut);

        protected virtual void OnEnable() {
            _lastUpdateTime = System.DateTime.MinValue;
        }
        protected virtual void OnDisable() {
            ReleaseImageTex();
        }
        protected virtual void OnRenderImage(RenderTexture src, RenderTexture dst) {
            if (filterMat == null || _lutImage == null || debugMode == DebugModeEnum.Pass) {
                Graphics.Blit (src, dst);
                return;
            }
            SetProperty (filterMat);
			Graphics.Blit (src, dst, filterMat, GetPass());
        }
        protected virtual void Update() {
            var now = System.DateTime.Now;
            var d = now - _lastUpdateTime;
            if (d.TotalSeconds > updateInterval) {
                _lastUpdateTime = now;
                UpdateLUT ();
            }
        }

        protected virtual string GetPath() {
			switch (dataPath) {
			case DataPathEnum.MyDocuments:
				return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), lutImageName);
			default:
				return Path.Combine(Application.streamingAssetsPath, lutImageName);
			}
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
                        _lutImage = new Texture2D(2, 2, TextureFormat.ARGB32, false);
						_lutImage.filterMode = FilterMode.Bilinear;
						_lutImage.wrapMode = TextureWrapMode.Clamp;
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
    }
}