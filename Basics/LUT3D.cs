using UnityEngine;
using System.Collections;
using System.IO;
using nobnak.Gist.ObjectExt;
using System.Linq;
using System.Collections.Generic;

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

		public LUT3D() : this(DEFAULT_LUT_DIM) {
		}
        public LUT3D(int dim) {
            Reset (dim);
        }

		#region public
		public LUT3D Reset(int dim) {
            if (_3dlut == null || _dim != dim) {
                _dim = dim;
				_3dlut.Destroy();
                _3dlut = Create3DLutTex (dim);
            }
            SetDefault ();
            return this;
        }

        #region Convert 
        public LUT3D Convert(Texture2D lutImage) {
			var w = lutImage.width;
            var h = lutImage.height;
			var cubeSize = Mathf.RoundToInt(Mathf.Pow(w * h, 1f / 3));
			if (cubeSize <= 0 || cubeSize * cubeSize * cubeSize != w * h) {
				Debug.LogWarningFormat(
					"Cannot convert image : size={0}x{1} estimated dim={2}",
					w, h, cubeSize);
				return this;
			}
			Reset(cubeSize);

			var indexer = new TiledCubeIndexer(cubeSize, w / cubeSize);
			var inputs = lutImage.GetPixels();
			var outputs = IterateCubicIndex(_dim).Select(i => inputs[indexer[i]]).ToArray();
			Apply(outputs);
			return this;
		}
		public LUT3D SetDefault() {
			var dx = 1f / (_dim - 1);
			var identity = IterateCubicIndex(_dim)
				.Select(v => new Color(v.x * dx, v.y * dx, v.z * dx, 1f))
				.ToArray();
			Apply(identity);
			return this;
		}
		#endregion

		#region IDisposable implementation
		public void Dispose() {
			_3dlut.Destroy();
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

        public LUT3D Apply (Color[] pixels) {
            _3dlut.SetPixels (pixels);
            _3dlut.Apply (false);
            return this;
        }
		#endregion

		#region static
		public static Color ConverterBypass(float x, float y, float z) {
            return new Color (x, y, z, 1f);
        }
        public static Texture3D Create3DLutTex (int dim) {
            var tex3d = new Texture3D (dim, dim, dim, TextureFormat.ARGB32, false);
			tex3d.wrapMode = TextureWrapMode.Clamp;
            tex3d.filterMode = FilterMode.Bilinear;
			tex3d.anisoLevel = 0;
            return tex3d;
		}
		public static IEnumerable<Vector3Int> IterateCubicIndex(int size) {
			for (var z = 0; z < size; z++)
				for (var y = 0; y < size; y++)
					for (var x = 0; x < size; x++)
						yield return new Vector3Int(x, y, z);
		}
		#endregion
	}
}