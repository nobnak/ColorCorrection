using UnityEngine;
using System.Collections;
using System.IO;

namespace ColorCorrection {
    [RequireComponent(typeof(Camera))]
    public class ColorGrading : MonoBehaviour {
        public const string PROP_SCALE = "_ColorGrading_Scale";
        public const string PROP_OFFSET = "_ColorGrading_Offset";
        public const string PROP_3DLUT = "_ColorGrading_3DLut";
        public const int DEFAULT_LUT_DIM = 16;
        public const float UPDATE_INTERVAL = 0.5f;

        public enum DataPathEnum { StreamingAssets = 0, Application }
        public enum DebugModeEnum { Disabled = 0, Pass }

        public DataPathEnum dataPath;
        public DebugModeEnum debugMode;
        public string image3dlut = "ColorGrading.png";

        public Material filterMat;

        Texture3D _3dlut;
        Texture2D _imageTex;
        Color[] _3dcolors;
        System.DateTime _lastUpdateTime;
        IEnumerator _updateWork;
        float _nextUpdateTime;

        void OnEnable() {
            _updateWork = null;
            _lastUpdateTime = System.DateTime.MinValue;
            _nextUpdateTime = 0f;
            InitLUT ();
        }
        void Update() {
            if (_updateWork != null) {
                if (!_updateWork.MoveNext ())
                    _updateWork = null;
            } else if (_nextUpdateTime < Time.timeSinceLevelLoad) {
                _nextUpdateTime = Time.timeSinceLevelLoad + UPDATE_INTERVAL;
                _updateWork = UpdateLUT ();
            }
            
        }
        void OnDisable() {
            Release3DLutTex ();
            ReleaseImageTex();
        }
        void OnRenderImage(RenderTexture src, RenderTexture dst) {
            if (_3dlut == null || filterMat == null || debugMode == DebugModeEnum.Pass) {
                Graphics.Blit (src, dst);
                return;
            }

            var dim = _3dlut.width;
            var pass = QualitySettings.activeColorSpace == ColorSpace.Linear ? 1 : 0;
            filterMat.SetFloat (PROP_SCALE, (float)(dim - 1) / dim);
            filterMat.SetFloat (PROP_OFFSET, 1f / (2f * dim));
            filterMat.SetTexture (PROP_3DLUT, _3dlut);
            Graphics.Blit (src, dst, filterMat, pass);
        }

        public void InitLUT() {
            var dim = DEFAULT_LUT_DIM;
            if (_3dlut != null)
                Release3DLutTex ();
            Create3DLutTex(dim);

            var dc = 1f / (dim - 1);
            for (var z = 0; z < dim; z++) {
                for (var y = 0; y < dim; y++) {
                    for (var x = 0; x < dim; x++) {
                        var i = x + (y + z * dim) * dim;
                        _3dcolors [i] = new Color (x * dc, y * dc, z * dc, 1f);
                    }
                }
            }
            _3dlut.SetPixels (_3dcolors);
            _3dlut.Apply (false);
        }
        public IEnumerator UpdateLUT() {
            var path = GetPath ();
            if (!File.Exists (path)) {
                Debug.LogFormat ("LUT image not found at {0}", path);
                yield break;
            }

            var writeTime = File.GetLastWriteTime (path);
            if (writeTime != _lastUpdateTime) {
                _lastUpdateTime = writeTime;

                var www = new WWW ("file://" + path);
                yield return www;
                if (!string.IsNullOrEmpty(www.error)) {
                    Debug.LogError (www.error);
                    yield break;
                }

                ReleaseImageTex();
                _imageTex = www.texture;
                Convert (_imageTex);
                www.Dispose ();
            }
        }
        public void Convert(Texture2D lutImage) {
            var dim = lutImage.height;
            if (_3dlut.width != dim) {
                Release3DLutTex ();
                Create3DLutTex (dim);
            }

            var cimg = lutImage.GetPixels ();
            for (var z = 0; z < dim; z++) {
                for (var y = 0; y < dim; y++) {
                    for (var x = 0; x < dim; x++) {
                        _3dcolors [x + (y + z * dim) * dim] = cimg [x + (z + (dim - y - 1) * dim) * dim];
                    }
                }
            }
            _3dlut.SetPixels (_3dcolors);
            _3dlut.Apply (false);
        }
        public string GetPath() {
            switch (dataPath) {
            case DataPathEnum.Application:
                return System.Environment.CurrentDirectory + "/" + image3dlut;
            default:
                return Application.streamingAssetsPath + "/" + image3dlut;
            }
        }

        void Release3DLutTex () {
            if (_3dlut != null) {
                DestroyImmediate (_3dlut);
                _3dlut = null;
            }
        }

        void Create3DLutTex (int dim) {
            _3dlut = new Texture3D (dim, dim, dim, TextureFormat.ARGB32, false);
            _3dcolors = _3dlut.GetPixels ();
        }
        void ReleaseImageTex () {
            if (_imageTex != null)
                DestroyImmediate (_imageTex);
        }
    }
}