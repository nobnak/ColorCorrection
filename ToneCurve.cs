using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ToneCurve {
    public const float EPSILON = 1e-4f;
    public const float TIME_SHADOW = 1f / 5;
    public const float TIME_HIGHLIGHT = 4f / 5;
    public static readonly float SHIFT = Mathf.Min (TIME_SHADOW, 0.5f - TIME_SHADOW);

    public float white = 1f;
    public float highlight = 0f;
    public float shadow = 0f;
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
        keys [1] = new Keyframe (TIME_SHADOW, TIME_SHADOW);
        keys [2] = new Keyframe (TIME_HIGHLIGHT, TIME_HIGHLIGHT);
        keys [3] = new Keyframe (1f, 1f);
        curve = new AnimationCurve (keys);
        curve.preWrapMode = WrapMode.ClampForever;
        curve.postWrapMode = WrapMode.ClampForever;
        Smooth ();
    }
    public void Update() {
        shadow = Mathf.Clamp (shadow, -1f, 1f);
        highlight = Mathf.Clamp (highlight, -1f, 1f);

        var s = keys [1];
        var h = keys [2];
        s.value = TIME_SHADOW + shadow * SHIFT;
        s.time = TIME_SHADOW + 0.2f * shadow * SHIFT;
        h.value = TIME_HIGHLIGHT + highlight * SHIFT;
        h.time = TIME_HIGHLIGHT + 0.2f * highlight * SHIFT;
        keys [1] = s;
        keys [2] = h;
        Smooth ();
    }
    public float Evaluate(float t) {
        return (white - black) * curve.Evaluate (t) + black;
    }
    public void Smooth() {
        var a = keys [0];
        var b = keys [1];
        var c = keys [2];
        var d = keys [3];

        var c0 = Gradient (a, b);
        var c1 = Gradient (b, c);
        var c2 = Gradient (c, d);

        a.inTangent = a.outTangent = c0;
        b.inTangent = b.outTangent = Mathf.Lerp (c0, c1, (b.value - a.value) / (c.value - a.value));
        c.inTangent = c.outTangent = Mathf.Lerp (c1, c2, (c.value - b.value) / (d.value - b.value));
        d.inTangent = d.outTangent = c2;

        keys [0] = a;
        keys [1] = b;
        keys [2] = c;
        keys [3] = d;

        curve.keys = keys;
    }
    public float Gradient(Keyframe a, Keyframe b) {
        var dx = b.time - a.time;
        var dy = b.value - a.value;
        return dy / dx;
    }
}
