using UnityEngine;
using System.Collections;
using System.IO;
using nobnak.Gist;

namespace ColorCorrection {
    public abstract class LUTGenerator : MonoBehaviour {
        public LUTEvent LUTOnUpdate;

        protected LUT3D lut;

        #region Unity
        protected virtual void OnEnable() {
            lut = new LUT3D ();
            NotifyOnUpdate ();
        }
        protected virtual void OnDisable() {
            if (lut != null) {
                lut.Dispose ();
                lut = null;
                NotifyOnUpdate ();
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