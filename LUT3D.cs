using UnityEngine;
using System.Collections;
using System.IO;

namespace ColorCorrection {
	public class LUT3D : System.IDisposable {
        public const int DEFAULT_LUT_DIM = 16;

        public const string PROP_SCALE = "_ColorGrading3D_Scale";
        public const string PROP_OFFSET = "_ColorGrading3D_Offset";
        public const string PROP_3DLUT = "_ColorGrading3D_Lut";

        int _dim;
        string _prefix;
        Texture3D _3dlut;
        Color[] _3dcolors;

		public LUT3D() : this(DEFAULT_LUT_DIM) {
		}
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
                Release3DLutTex (_3dlut);
                _3dlut = Create3DLutTex (dim);
                _3dcolors = _3dlut.GetPixels ();
            }
            SetDefault ();
            return this;
        }
        #region Convert 
        public LUT3D Convert(System.Func<float, float, float, Color> ColorPicker) {
            var dx = 1f / (_dim - 1);
            for (var z = 0; z < _dim; z++)
                for (var y = 0; y < _dim; y++)
                    for (var x = 0; x < _dim; x++)
                        this [x, y, z] = ColorPicker (x * dx, y * dx, z * dx);
            Apply ();
            return this;
        }
        public LUT3D Convert(System.Func<int, int, int, Color> Pixels) {
            for (var z = 0; z < _dim; z++)
                for (var y = 0; y < _dim; y++)
                    for (var x = 0; x < _dim; x++)
                        this [x, y, z] = Pixels (x, y, z); 
            Apply ();
            return this;
        }
        public LUT3D Convert(System.Func<int, int, int, Color> Pixels, int dim) {
            Reset (dim);
            return Convert (Pixels);
        }
        public LUT3D Convert(System.Func<float, float, float, Color> ColorPicker, int dim) {
            Reset (dim);
            return Convert (ColorPicker);
        }
        public LUT3D Convert(Texture2D lutImage) {
            var dim = lutImage.height;
            var pixels = lutImage.GetPixels ();
            return Convert ((x, y, z) => pixels [x + (z + (dim - y - 1) * dim) * dim], dim);
        }
        #endregion

        public virtual void SetProperty(Material mat) {
            mat.SetFloat (PROP_SCALE, Scale);
            mat.SetFloat (PROP_OFFSET, Offset);
            mat.SetTexture (PROP_3DLUT, _3dlut);
        }
        public virtual void SetProperty(MaterialPropertyBlock block) {
            block.SetFloat (PROP_SCALE, Scale);
            block.SetFloat (PROP_OFFSET, Offset);
            block.SetTexture (PROP_3DLUT, _3dlut);
        }

        public float Scale { get { return (_dim - 1f) / _dim; } }
        public float Offset { get { return 1f / (2f * _dim); } }
        public Texture Texture { get { return _3dlut; } }

        public LUT3D SetDefault() {
            Convert ((float x, float y, float z) => new Color (x, y, z, 1f));
            Apply ();
            return this;
        }
        public LUT3D Apply () {
            _3dlut.SetPixels (_3dcolors);
            _3dlut.Apply (false);
            return this;
        }

        static Texture3D Create3DLutTex (int dim) {
            var tex3d = new Texture3D (dim, dim, dim, TextureFormat.ARGB32, false);
			tex3d.wrapMode = TextureWrapMode.Clamp;
            tex3d.filterMode = FilterMode.Bilinear;
			tex3d.anisoLevel = 0;
            return tex3d;
        }
        static void Release3DLutTex (Object obj) {
            GameObject.DestroyImmediate (obj);
        }

        #region IDisposable implementation
        public void Dispose () {
            Release3DLutTex (_3dlut);
            _3dcolors = null;
        }
        #endregion
    }
}