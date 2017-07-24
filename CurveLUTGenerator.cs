using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorCorrection;

namespace ColorCorrection {
    [ExecuteInEditMode]
    public class CurveLUTGenerator : LUTGenerator {
        [SerializeField]
        AnimationCurve master = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField]
        AnimationCurve red = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField]
        AnimationCurve green = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField]
        AnimationCurve blue = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        protected override void OnEnable() {
            base.OnEnable ();
            UpdateLUT ();
        }
        protected void OnValidate() {
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
    }
}
