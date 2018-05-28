using UnityEngine;
using System.Collections;
using System.IO;
using nobnak.Gist;

namespace ColorCorrection {
    public abstract class LUTGenerator : MonoBehaviour {
        public LUTEvent LUTOnUpdate;

		protected LUT3D lut;

        #region Unity
		protected virtual void Awake() {
			lut = new LUT3D();
		}
		protected virtual void OnEnable() {
            NotifyOnUpdate ();
        }
		protected virtual void OnDestroy() {
			if (lut != null) {
				lut.Dispose();
				lut = null;
			}
		}
		#endregion

		protected virtual void NotifyOnUpdate() {
            LUTOnUpdate.Invoke (lut);
        }

        [System.Serializable]
        public class LUTEvent : UnityEngine.Events.UnityEvent<LUT3D> {}
    }
}