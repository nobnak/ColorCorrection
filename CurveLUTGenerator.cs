using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorCorrection;

namespace ColorCorrection {
    [ExecuteInEditMode]
    public class CurveLUTGenerator : LUTGenerator {
        [SerializeField]
        ToneCurve master;
        [SerializeField]
        ToneCurve red;
        [SerializeField]
        ToneCurve green;
        [SerializeField]
        ToneCurve blue;

        protected override void OnEnable() {
            base.OnEnable ();
            UpdateCurves ();
            UpdateLUT ();
        }
        protected void OnValidate() {
            UpdateCurves ();
            UpdateLUT ();
        }

        void UpdateLUT () {
            if (lut != null) {
                lut.Convert ((float x, float y, float z) => {
                    var v = Mathf.Max(x, Mathf.Max(y, z));
                    var m = master.Evaluate(v) / v;
                    return new Color(
                        m * red.Evaluate(x),
                        m * green.Evaluate(y),
                        m * blue.Evaluate(z));
                });
                NotifyOnUpdate ();
            }
        }

        void UpdateCurves () {
            master.Update ();
            red.Update ();
            green.Update ();
            blue.Update ();
        }
    }
}
