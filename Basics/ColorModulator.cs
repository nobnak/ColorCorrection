using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorCorrection {
    
    [System.Serializable]
    public class ColorModulator {
        [Range(-1f, 1f)]
        public float hue = 0f;
        [Range(-1f, 1f)]
        public float saturation = 0f;
        [Range(-1f, 1f)]
        public float value = 0f;

        public Color Evaluate(Color c) {
            float h, s, v;
            Color.RGBToHSV (c, out h, out s, out v);
            h = Mathf.Repeat (h + hue, 1f);
            s = Mathf.Clamp01 (s + saturation);
            v = Mathf.Clamp01 (v + value);
            return Color.HSVToRGB (h, s, v);
        }

        public void Clear() {
            hue = 0f;
            saturation = 0f;
            value = 0f;
        }
    }
}