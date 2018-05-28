using nobnak.Gist.Loader;
using System.IO;
using UnityEngine;

namespace ColorCorrection {
	[ExecuteInEditMode]
    public class ImageLUTLoader : LUTGenerator {
		[SerializeField]
		public Texture2D alternativeImage;
		[SerializeField]
		public ImageLoader loader;

		#region Unity
		protected override void Awake() {
			base.Awake();
			loader.Changed += UpdateLUT;
		}
		protected void Update() {
			loader.Validate();
		}
		protected override void OnDestroy() {
			loader.Dispose();
			base.OnDestroy();
        }
        #endregion

		protected void UpdateLUT (Texture2D tex) {
			Debug.Log("UpdateLUT");
			if (lut != null) {
				if (tex != null)
					lut.Convert(tex);
				else if (alternativeImage != null)
					lut.Convert(alternativeImage);
				else
					lut.SetDefault();

				NotifyOnUpdate();
			}
        }
    }
}
