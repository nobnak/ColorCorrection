using nobnak.Gist.Loader;
using System.IO;
using System.Text;
using UnityEngine;

namespace ColorCorrection {
	[ExecuteInEditMode]
    public class ImageLUTLoader : LUTGenerator {
		[SerializeField]
		protected Texture2D alternativeImage;
		[SerializeField]
		protected ImageLoader loader;

		#region Unity
		protected override void Awake() {
			base.Awake();
			loader.Changed += ListenLoaderOnChanged;
		}
		protected void Update() {
			loader.Validate();
		}
		protected override void OnDestroy() {
			loader.Dispose();
			base.OnDestroy();
        }
		#endregion

		#region public
		public virtual Texture2D AlternativeImage {
			get { return alternativeImage; }
			set {
				if (alternativeImage != value) {
					alternativeImage = value;
					UpdateLUT();
				}
			}
		}
		#endregion

		protected void ListenLoaderOnChanged(Texture2D tex) {
			UpdateLUT();
		}
		protected void UpdateLUT () {
			if (lut != null) {
				var buf = new StringBuilder("UpdateLUT : ");
				if (loader != null && loader.Target != null) {
					var tex = loader.Target;
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
