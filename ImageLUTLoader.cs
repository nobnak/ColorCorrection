using nobnak.Gist;
using nobnak.Gist.Loader;
using System.IO;
using System.Text;
using UnityEngine;

namespace ColorCorrection {

	[ExecuteInEditMode]
    public class ImageLUTLoader : LUTGenerator {

		[SerializeField]
		protected bool gammaToLinear = false;
		[SerializeField]
		protected Texture2D alternativeImage;
		[SerializeField]
		protected ImageLoader loader;

		#region Unity
		protected override void Awake() {
			base.Awake();
			loader.Changed += v => validator.Invalidate();
		}
		protected override void Update() {
			base.Update();
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
					validator.Invalidate();
					alternativeImage = value;
				}
			}
		}
		#endregion

		protected override void UpdateLUT () {
			if (lut != null) {
				var buf = new StringBuilder("UpdateLUT : ");
				if (loader != null && loader.Target != null) {
					var tex = loader.Target;
					lut.Convert(tex, gammaToLinear);
					buf.AppendFormat(tex.name);
				} else if (alternativeImage != null) {
					lut.Convert(alternativeImage, gammaToLinear);
					buf.AppendFormat("Alternative {0}", alternativeImage.name);
				} else {
					lut.SetDefault();
					buf.AppendFormat("Reset");
				}
				Debug.Log(buf.ToString());
			}
        }
    }
}
