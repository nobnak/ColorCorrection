using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorCorrection;

namespace ColorCorrection {
    
    [ExecuteInEditMode]
    public class CurveLUTGenerator : LUTGenerator {
        public ToneCurve master;
        public ColorModulator hsv;

        protected override void OnEnable() {
            base.OnEnable ();
            UpdateAll ();
        }
        protected virtual void OnValidate() {
            UpdateAll ();
        }

        public virtual void UpdateLUT () {
            if (lut != null) {
                lut.Convert ((float x, float y, float z) => {
                    var v = Mathf.Max(x, Mathf.Max(y, z));
                    var m = master.Evaluate(v) / v;
                    var c = new Color(
                        m * x,
                        m * y,
                        m * z);
                    return hsv.Evaluate(c);
                });
                NotifyOnUpdate ();
            }
        }

        public virtual void UpdateAll () {
            UpdateCurves ();
            UpdateLUT ();
        }

        public virtual void UpdateCurves () {
            master.Update ();
        }
    }
}
