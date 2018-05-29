using nobnak.Gist.Loader;
using System.IO;
using System.Text;
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
			if (lut != null) {
				var buf = new StringBuilder("UpdateLUT : ");
				if (tex != null) {
					lut.Convert(tex);
					buf.AppendFormat(tex.name);
				} else if (alternativeImage != null) {
					lut.Convert(alternativeImage);
					buf.AppendFormat("Alternative {0}", alternativeImage.name);
				} else {
					lut.SetDefault();
					buf.AppendFormat("Reset");
				}
				Debug.Log(buf.ToString());

				NotifyOnUpdate();
			}
        }
    }
}
