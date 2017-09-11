using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorCorrection;
using Gist.DataUI;
using DataUI.Settings;

namespace ColorCorrection {
    
    [ExecuteInEditMode]
    public class  CurveLUTGeneratorGUI : CurveLUTGenerator {
        public const float LABEL_WIDTH = 100f;

        [SerializeField]
        AbstractSettingsUI.SettingsCore settings;
        [SerializeField]
        Data data;

        [System.Serializable]
        public class Data {
            public float curveWhite;
            public float curveBlack;
            public float curveHighlight;
            public float curveShadow;

            public float hsvHue;
            public float hsvSaturation;
            public float hsvValue;

            public bool bypass;
        }

        #region Unity
        protected override void OnEnable () {
            base.OnEnable ();
            settings.OnDataChange.AddListener (() => {
                Load ();
                UpdateAll ();
            });
            settings.OnEnable (data);
        }
        protected virtual void Update() {
            settings.Update (data);
        }
        protected virtual void OnGUI() {
            if (settings.mode == AbstractSettingsUI.SettingsCore.ModeEnum.GUI)
                settings.OnGUI (this);
        }
        protected virtual void OnDisable() {
            settings.OnDataChange.RemoveAllListeners();
        }
        #endregion

        void Load () {
            master.white = data.curveWhite;
            master.black = data.curveBlack;
            master.highlight = data.curveHighlight;
            master.shadow = data.curveShadow;
            hsv.hue = data.hsvHue;
            hsv.saturation = data.hsvSaturation;
            hsv.value = data.hsvValue;
            bypass = data.bypass;
        }
    }
}
