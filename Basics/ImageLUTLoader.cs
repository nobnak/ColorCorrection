using nobnak.Gist.Loader;
using System.IO;
using UnityEngine;

namespace ColorCorrection {
	[ExecuteInEditMode]
    public class ImageLUTLoader : LUTGenerator {
        [SerializeField]
        protected Data data;

        #region Unity
        protected override void OnEnable() {
            base.OnEnable ();

			data.loader.Changed += UpdateLUT;
			UpdateLUT(data.loader.Target);
		}
		protected void OnValidate() {
			if (data != null && data.loader != null)
				UpdateLUT(data.loader.Target);
			else
				UpdateLUT(null);
		}
		protected override void OnDisable() {
			data.loader.Changed -= UpdateLUT;
			UpdateLUT(null);
            base.OnDisable ();
        }
        #endregion

		protected void UpdateLUT (Texture2D tex) {
			if (lut != null) {
				if (tex != null)
					lut.Convert(tex);
				else if (data.alternativeImage != null)
					lut.Convert(data.alternativeImage);
				else
					lut.SetDefault();

				NotifyOnUpdate();
			}
        }

        [System.Serializable]
        public class Data {
            public Texture2D alternativeImage;
			[SerializeField]
			public ImageLoader loader;
		}
    }
}
