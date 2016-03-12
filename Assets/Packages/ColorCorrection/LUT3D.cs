using UnityEngine;
using System.Collections;
using System.IO;

namespace ColorCorrection {
    public class LUT3D : System.IDisposable {
        public const int PASS_GAMMA = 0;
        public const int PASS_LINEAR = 1;

        public const string PROP_SCALE = "_Scale";
        public const string PROP_OFFSET = "_Offset";
        public const string PROP_3DLUT = "_Lut3D";

        int _dim;
        string _prefix;
        Texture3D _3dlut;
        Color[] _3dcolors;

        public LUT3D(int dim, string prefix) {
            Reset (dim);
            this._prefix = prefix;
        }

        public Color this[int x, int y, int z] {
            set {
                _3dcolors [x + (y + z * _dim) * _dim] = value;
            }
        }
        public void Reset(int dim) {
            if (_3dlut == null || _dim != dim) {
                _dim = dim;
                Release3DLutTex ();
                Create3DLutTex (dim);
            }
        }
        public void Convert(Texture2D lutImage) {
            Reset (lutImage.height);
            var cimg = lutImage.GetPixels ();
            for (var z = 0; z < _dim; z++)
                for (var y = 0; y < _dim; y++)
                    for (var x = 0; x < _dim; x++)
                        this[x, y, z] = cimg [x + (z + (_dim - y - 1) * _dim) * _dim];
            Apply ();
        }
        public void SetProperty(Material mat) {
            mat.SetFloat (_prefix + PROP_SCALE, (float)(_dim - 1) / _dim);
            mat.SetFloat (_prefix + PROP_OFFSET, 1f / (2f * _dim));
            mat.SetTexture (_prefix + PROP_3DLUT, _3dlut);
            Debug.LogFormat ("3D LUT exists ? {0}", _3dlut != null);
        }
        public int Pass {
            get { return QualitySettings.activeColorSpace == ColorSpace.Linear ? PASS_LINEAR : PASS_GAMMA; }
        }
        public void SetDefault() {
            var dc = 1f / (_dim - 1);
            for (var z = 0; z < _dim; z++)
                for (var y = 0; y < _dim; y++)
                    for (var x = 0; x < _dim; x++)
                        this[x, y, z] = new Color (x * dc, y * dc, z * dc, 1f);
            Apply ();
        }
        public void Apply () {
            _3dlut.SetPixels (_3dcolors);
            _3dlut.Apply (false);
        }

        void Create3DLutTex (int dim) {
            _3dlut = new Texture3D (dim, dim, dim, TextureFormat.ARGB32, false);
            _3dlut.filterMode = FilterMode.Bilinear;
            _3dcolors = _3dlut.GetPixels ();
        }
        void Release3DLutTex () {
            if (_3dlut != null) {
                GameObject.DestroyImmediate (_3dlut);
                _3dlut = null;
            }
        }

        #region IDisposable implementation
        public void Dispose () {
            Release3DLutTex ();
            _3dcolors = null;
        }
        #endregion
    }
}