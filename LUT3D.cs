using UnityEngine;
using System.Collections;
using System.IO;

namespace ColorCorrection {
	public class LUT3D : System.IDisposable {
        public const int DEFAULT_LUT_DIM = 16;

        public const string PROP_SCALE = "_ColorGrading3D_Scale";
        public const string PROP_OFFSET = "_ColorGrading3D_Offset";
        public const string PROP_3DLUT = "_ColorGrading3D_Lut";

        public delegate Color ColorPickerInt(int x, int y, int z);
        public delegate Color ColorPickerNorm(float x, float y, float z);

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
            set { _3dcolors [IndexOf3D(x, y, z)] = value; }
            get { return _3dcolors[IndexOf3D(x, y, z)]; }
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
        public LUT3D Convert(ColorPickerNorm ColorPicker) {
            var dx = 1f / (_dim - 1);
            for (var z = 0; z < _dim; z++)
                for (var y = 0; y < _dim; y++)
                    for (var x = 0; x < _dim; x++)
                        this [x, y, z] = ColorPicker (x * dx, y * dx, z * dx);
            Apply ();
            return this;
        }
        public LUT3D Convert(ColorPickerInt Pixels) {
            for (var z = 0; z < _dim; z++)
                for (var y = 0; y < _dim; y++)
                    for (var x = 0; x < _dim; x++)
                        this [x, y, z] = Pixels (x, y, z); 
            Apply ();
            return this;
        }
        public LUT3D Convert(ColorPickerInt Pixels, int dim) {
            Reset (dim);
            return Convert (Pixels);
        }
        public LUT3D Convert(ColorPickerNorm ColorPicker, int dim) {
            Reset (dim);
            return Convert (ColorPicker);
        }
        public LUT3D Convert(Texture2D lutImage) {
            var dim = lutImage.height;
            Reset(dim);

            var pixels = lutImage.GetPixels ();
            return Convert ((x, y, z) => pixels [IndexOf2D(x, y, z, dim)], dim);
        }
        public void ConvertBack(Texture2D lutImage) {
            var height = _dim;
            var width = _dim * _dim;
            lutImage.Resize (width, height);

            var pixels = lutImage.GetPixels ();
            for (var z = 0; z < _dim; z++)
                for (var y = 0; y < _dim; y++)
                    for (var x = 0; x < _dim; x++)
                        pixels [IndexOf2D(x, y, z)] = this [x, y, z];
            lutImage.SetPixels (pixels);
            lutImage.Apply ();
        }

        public int IndexOf2D(int x, int y, int z) {
            return IndexOf2D (x, y, z, _dim);
        }
        public static int IndexOf2D(int x, int y, int z, int dim) {
            return x + (z + (dim - y - 1) * dim) * dim;
        }
        public int IndexOf3D(int x, int y, int z) {
            return IndexOf3D(x, y, z, _dim);
        }
        public static int IndexOf3D(int x, int y, int z, int dim) {
            return x + (y + z * dim) * dim;
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
            Convert (new ColorPickerNorm(ConverterBypass));
            Apply ();
            return this;
        }
        public LUT3D Apply () {
            _3dlut.SetPixels (_3dcolors);
            _3dlut.Apply (false);
            return this;
        }

        public static Color ConverterBypass(float x, float y, float z) {
            return new Color (x, y, z, 1f);
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