using UnityEngine;
using System.Collections;
using System.IO;

namespace ColorCorrection {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class ColorGrading : MonoBehaviour {
        public const int DEFAULT_LUT_DIM = 16;
        public const float UPDATE_INTERVAL = 0.5f;

        public enum DataPathEnum { StreamingAssets = 0, Application }
        public enum DebugModeEnum { Disabled = 0, Pass }

        public DataPathEnum dataPath;
        public DebugModeEnum debugMode;
        public string propertyPrefix = "_ColorGrading";
        public string lutImageName = "ColorGrading.png";
        public Material filterMat;

        LUT3D _lut;
        Texture2D _lutImage;
        System.DateTime _lastUpdateTime;

        void OnEnable() {
            _lastUpdateTime = System.DateTime.MinValue;
            _lut = new LUT3D (DEFAULT_LUT_DIM, propertyPrefix);
        }
        void OnDisable() {
            ReleaseImageTex();
            _lut.Dispose ();
        }
        void OnRenderImage(RenderTexture src, RenderTexture dst) {
            if (filterMat == null || _lutImage == null || debugMode == DebugModeEnum.Pass) {
                Graphics.Blit (src, dst);
                return;
            }

            _lut.SetProperty (filterMat);
            Graphics.Blit (src, dst, filterMat, _lut.Pass);
        }
        void Update() {
            var now = System.DateTime.Now;
            var d = now - _lastUpdateTime;
            if (d.TotalSeconds > UPDATE_INTERVAL) {
                _lastUpdateTime = now;
                UpdateLUT ();
            }
        }

        public string GetPath() {
            switch (dataPath) {
            case DataPathEnum.Application:
                return System.Environment.CurrentDirectory + "/" + lutImageName;
            default:
                return Application.streamingAssetsPath + "/" + lutImageName;
            }
        }

        void UpdateLUT() {
            try {
                var path = GetPath ();
                if (!File.Exists (path)) {
                    //Debug.LogFormat ("LUT image not found at {0}", path);
                    ReleaseImageTex();
                    return;
                }

                var writeTime = File.GetLastWriteTime (path);
                if (writeTime != _lastUpdateTime) {
                    _lastUpdateTime = writeTime;

                    if (_lutImage == null)
                        _lutImage = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                    _lutImage.LoadImage(File.ReadAllBytes(path));
                    _lut.Convert(_lutImage);
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