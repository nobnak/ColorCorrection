using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorCorrection;
using Gist.DataUI;

namespace ColorCorrection {
    
    [ExecuteInEditMode]
    public class  CurveLUTGeneratorGUI : CurveLUTGenerator {
        public const float LABEL_WIDTH = 100f;

        [SerializeField]
        KeyCode toggleKey;
        [SerializeField]
        bool enabledGUI;

        Rect window = new Rect (10f, 10f, 300f, 300f);

        TextFloat curveWhite;
        TextFloat curveBlack;
        TextFloat curveHighlight;
        TextFloat curveShadow;

        TextFloat hsvHue;
        TextFloat hsvSaturation;
        TextFloat hsvValue;

        #region Unity
        protected override void OnEnable () {
            base.OnEnable ();
            Load ();
        }
        protected virtual void Update() {
            if (Input.GetKeyDown (toggleKey))
                enabledGUI = !enabledGUI;
        }
        protected virtual void OnGUI() {
            if (enabledGUI)
                window = GUILayout.Window (GetInstanceID (), window, Window, name);
        }
        #endregion

        void Window(int id) {
            GUI.changed = false;
            GUILayout.BeginVertical ();

            GUILayout.Label ("Curve");

            Item ("- White", curveWhite);
            Item ("- Black", curveBlack);
            Item ("- Highlight", curveHighlight);
            Item ("- Shadow", curveShadow);

            GUILayout.Label ("HSV");
            Item ("- Hue", hsvHue);
            Item ("- Saturation", hsvSaturation);
            Item ("- Value", hsvValue);

            if (GUILayout.Button ("Reset")) {
                master.Clear ();
                hsv.Clear ();
                UpdateAll ();
                Load ();
            }

            GUILayout.EndVertical ();
            GUI.DragWindow ();

            if (GUI.changed) {
                Save ();
                UpdateAll ();
            }
        }

        void Item (string label, TextFloat textFloat) {
            GUILayout.BeginHorizontal ();
            GUILayout.Label (label, GUILayout.Width (LABEL_WIDTH));
            textFloat.StrValue = GUILayout.TextField (textFloat.StrValue);
            GUILayout.EndHorizontal ();
        }

        void Save () {
            master.white = curveWhite.Value;
            master.black = curveBlack.Value;
            master.highlight = curveHighlight.Value;
            master.shadow = curveShadow.Value;
            hsv.hue = hsvHue.Value;
            hsv.saturation = hsvSaturation.Value;
            hsv.value = hsvValue.Value;
        }
        void Load () {
            LoadOrInitTextFloat (ref curveWhite, master.white);
            LoadOrInitTextFloat(ref curveBlack, master.black);
            LoadOrInitTextFloat(ref curveHighlight, master.highlight);
            LoadOrInitTextFloat(ref curveShadow, master.shadow);
            LoadOrInitTextFloat(ref hsvHue, hsv.hue);
            LoadOrInitTextFloat(ref hsvSaturation, hsv.saturation);
            LoadOrInitTextFloat(ref hsvValue, hsv.value);
        }

        void LoadOrInitTextFloat (ref TextFloat text, float v) {
            if (text == null)
                text = new TextFloat (v);
            else
                text.Value = v;
        }
    }
}
