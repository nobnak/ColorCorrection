using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorCorrection;

namespace ColorCorrection {
    
    [ExecuteInEditMode]
    public class CurveLUTGenerator : LUTGenerator {
        public ToneCurve master;
        public ColorModulator hsv;
        public bool bypass = false;

        protected override void OnEnable() {
            base.OnEnable ();
            UpdateAll ();
        }
        protected virtual void OnValidate() {
            UpdateAll ();
        }

        public virtual void UpdateLUT () {
            if (lut != null) {
                var conv = (bypass ? 
                    new LUT3D.ColorPickerNorm (LUT3D.ConverterBypass) : 
                    new LUT3D.ColorPickerNorm (Converter));
                lut.Convert (conv);
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

        protected virtual Color Converter(float x, float y, float z) {
            var v = Mathf.Max(x, Mathf.Max(y, z));
            var m = master.Evaluate(v) / v;
            var c = new Color(
                m * x,
                m * y,
                m * z);
            return hsv.Evaluate(c);
        }
    }
}
