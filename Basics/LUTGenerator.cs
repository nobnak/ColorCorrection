using UnityEngine;
using System.Collections;
using System.IO;
using nobnak.Gist;

namespace ColorCorrection {

	[ExecuteInEditMode]
    public abstract class LUTGenerator : MonoBehaviour {
        public LUTEvent LUTOnUpdate;

		protected LUT3D lut;
		protected Validator validator = new Validator();

		#region abstract
		protected abstract void UpdateLUT();
		#endregion

		#region Unity
		protected virtual void Awake() {
			lut = new LUT3D();

			validator.Validation += () => {
				UpdateLUT();
			};
			validator.Validated += () => {
				LUTOnUpdate.Invoke(lut);
			};
		}
		protected virtual void OnEnable() {
			validator.Invalidate();
		}
		protected virtual void Update() {
			validator.Validate();
        }
		protected virtual void OnValidate() {
			validator.Invalidate();
		}
		protected virtual void OnDestroy() {
			if (lut != null) {
				lut.Dispose();
				lut = null;
			}
		}
		#endregion

        [System.Serializable]
        public class LUTEvent : UnityEngine.Events.UnityEvent<LUT3D> {}
    }
}