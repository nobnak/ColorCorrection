using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ToneCurve {
    public const float EPSILON = 1e-4f;

    public float white = 1f;
    public float black = 0f;

    [SerializeField]
    protected AnimationCurve curve;
    protected Keyframe[] keys;

    public ToneCurve() {
        Init ();
    }

    public void Init() {
        keys = new Keyframe[2];
        keys [0] = new Keyframe (0f, 0f);
        keys [1] = new Keyframe (1f, 1f);
        curve = new AnimationCurve (keys);
        curve.preWrapMode = WrapMode.ClampForever;
        curve.postWrapMode = WrapMode.ClampForever;
        Smooth ();
    }
    public void Update() {
    }
    public float Evaluate(float t) {
        return (white - black) * curve.Evaluate (t) + black;
    }
    public void Smooth() {
    }
    public float Gradient(Keyframe a, Keyframe b) {
        var dx = b.time - a.time;
        var dy = b.value - a.value;
        return dy / dx;
    }
}
