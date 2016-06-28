using UnityEngine;
using System.Collections;
using System.IO;

namespace ColorCorrection {
    public class LUT3D : System.IDisposable {
        public const int PASS_GAMMA = 0;
        public const int PASS_LINEAR = 1;

        public const string PROP_SCALE = "_ColorGrading_Scale";
        public const string PROP_OFFSET = "_ColorGrading_Offset";
        public const string PROP_3DLUT = "_ColorGrading_Lut3D";

        int _dim;
        string _prefix;
        Texture3D _3dlut;
        Color[] _3dcolors;

        public LUT3D(int dim) {
            Reset (dim);
        }

        public Color this[int x, int y, int z] {
            set {
                _3dcolors [x + (y + z * _dim) * _dim] = value;
            }
        }
        public LUT3D Reset(int dim) {
            if (_3dlut == null || _dim != dim) {
                _dim = dim;
                Release3DLutTex ();
                Create3DLutTex (dim);
            }
            return this;
        }
        public LUT3D Convert(Texture2D lutImage) {
            Reset (lutImage.height);
            var cimg = lutImage.GetPixels ();
            for (var z = 0; z < _dim; z++)
                for (var y = 0; y < _dim; y++)
                    for (var x = 0; x < _dim; x++)
                        this[x, y, z] = cimg [x + (z + (_dim - y - 1) * _dim) * _dim];
            Apply ();
            return this;
        }
        public LUT3D SetProperty(Material mat) {
            mat.SetFloat (PROP_SCALE, (float)(_dim - 1) / _dim);
            mat.SetFloat (PROP_OFFSET, 1f / (2f * _dim));
            mat.SetTexture (PROP_3DLUT, _3dlut);
            return this;
        }
        public LUT3D SetProperty(MaterialPropertyBlock block) {
            block.SetFloat (PROP_SCALE, (float)(_dim - 1) / _dim);
            block.SetFloat (PROP_OFFSET, 1f / (2f * _dim));
            block.SetTexture (PROP_3DLUT, _3dlut);
            return this;
        }
        public int Pass {
            get { return QualitySettings.activeColorSpace == ColorSpace.Linear ? PASS_LINEAR : PASS_GAMMA; }
        }
        public LUT3D SetDefault() {
            var dc = 1f / (_dim - 1);
            for (var z = 0; z < _dim; z++)
                for (var y = 0; y < _dim; y++)
                    for (var x = 0; x < _dim; x++)
                        this[x, y, z] = new Color (x * dc, y * dc, z * dc, 1f);
            Apply ();
            return this;
        }
        public LUT3D Apply () {
            _3dlut.SetPixels (_3dcolors);
            _3dlut.Apply (false);
            return this;
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