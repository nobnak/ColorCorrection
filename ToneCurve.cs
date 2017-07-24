using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ToneCurve {
    public const float EPSILON = 1e-4f;
    public const float TIME_DARK = 1f / 4;
    public const float TIME_HIGHLIGHT = 3f / 4;

    public float white = 1f;
    public float highlight = 0f;
    public float dark = 0f;
    public float black = 0f;

    [SerializeField]
    protected AnimationCurve curve;
    protected Keyframe[] keys;

    public ToneCurve() {
        Init ();
    }

    public void Init() {
        keys = new Keyframe[4];
        keys [0] = new Keyframe (0f, 0f);
        keys [1] = new Keyframe (TIME_DARK, TIME_DARK);
        keys [2] = new Keyframe (TIME_HIGHLIGHT, TIME_HIGHLIGHT);
        keys [3] = new Keyframe (1f, 1f);
        curve = new AnimationCurve (keys);
        curve.preWrapMode = WrapMode.ClampForever;
        curve.postWrapMode = WrapMode.ClampForever;
        Smooth ();
    }
    public void Update() {
        dark = Mathf.Clamp (dark, -1f, 1f);
        highlight = Mathf.Clamp (highlight, -1f, 1f);

        keys [1].value = (1f + dark) * TIME_DARK;
        keys [2].value = (1f - highlight) * TIME_HIGHLIGHT + highlight;
        Smooth ();
    }
    public float Evaluate(float t) {
        return (white - black) * curve.Evaluate (t) + black;
    }
    public void Smooth() {
        #if true
        var a = keys [0];
        var b = keys [1];
        var c = keys [2];
        var d = keys [3];

        a.inTangent = a.outTangent = Gradient (a, b);
        b.inTangent = b.outTangent = Gradient (a, b);
        c.inTangent = c.outTangent = Gradient (c, d);
        d.inTangent = d.outTangent = Gradient (c, d);

        keys [0] = a;
        keys [1] = b;
        keys [2] = c;
        keys [3] = d;

        curve.keys = keys;
        #else
        for (var i = 0; i < keys.Length; i++) {
            UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, i, UnityEditor.AnimationUtility.TangentMode.Auto);
            UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, i, UnityEditor.AnimationUtility.TangentMode.Auto);
        }
        #endif

        Debug.LogFormat ("Tangent {0} to {1}", keys[0].outTangent, keys[3].inTangent);
    }
    public float Gradient(Keyframe a, Keyframe b) {
        var dx = b.time - a.time;
        var dy = b.value - a.value;
        return dy / dx;
    }
}
