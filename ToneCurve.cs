using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorCorrection {
        
    [System.Serializable]
    public class ToneCurve {
        public const float WHITE_POINT = 1f;
        public const float BLACK_POINT = 0f;
        public const float EPSILON = 1e-4f;
        public const float CONTROL_HEIGHT = 1f / 2;
        public const int SUBDIVITIONS = 10;

        [Range(-1f, 1f)]
        public float white = 0f;
        [Range(-1f, 1f)]
        public float black = 0f;
        [Range(-1f, 1f)]
        public float highlight = 0f;
        [Range(-1f, 1f)]
        public float shadow = 0f;

        [SerializeField]
        AnimationCurve curve;

        Keyframe[] keys;

        public ToneCurve() {
            Init ();
        }

        public void Init() {
            var dt = 1f / SUBDIVITIONS;
            keys = new Keyframe[SUBDIVITIONS + 1];
            for (var i = 0; i < keys.Length; i++) {
                var v = i * dt;
                var key = new Keyframe (v, v);
                key.inTangent = 1f;
                key.outTangent = 1f;
                keys [i] = key;
            }
            curve = new AnimationCurve (keys);
        }
        public void Update() {
            var dt = 1f / (keys.Length - 1);
            var f = CreateCurveFunc ();
            for (var i = 0; i < keys.Length; i++) {
                var v = f (i * dt);
                var k = keys [i];
                k.time = v.x;
                k.value = v.y;
                keys [i] = k;
            }
            keys = Smooth (keys);
            curve.keys = keys;
        }
        public void Clear() {
            white = 0f;
            black = 0f;
            highlight = 0f;
            shadow = 0f;
        }
        public float Evaluate(float t) {
            return Exposure (curve.Evaluate(t));
        }

        public float White { get { return white + WHITE_POINT; } }
        public float Black { get { return black + BLACK_POINT; } }
        public float Exposure(float v) {
            return (White - Black) * v + Black;
        }

        System.Func<float, Vector2> CreateCurveFunc() {
            var rad45 = 45f * Mathf.Deg2Rad;
            var radOpen = 45f * Mathf.Deg2Rad;
            var b1 = CONTROL_HEIGHT * new Vector2 (
                         Mathf.Cos (rad45 + shadow * radOpen),
                         Mathf.Sin (rad45 + shadow * radOpen));
            var b2 = Vector2.one - CONTROL_HEIGHT * new Vector2 (
                         Mathf.Cos (rad45 - highlight * radOpen),
                         Mathf.Sin (rad45 - highlight * radOpen));
            var b3 = Vector2.one;
            return t => {
                var tdual = 1f - t;
                return (3f * t * tdual * tdual) * b1 
                    + (3f * t * t * tdual) * b2
                    + (t * t * t) * b3;
            };
        }

        static Keyframe[] Smooth(Keyframe[] keys) {
            var limit = keys.Length - 1;
            for (var i = 0; i < keys.Length; i++) {
                var iprev = Mathf.Max (0, i - 1);
                var inext = Mathf.Min (limit, i + 1);
                var k0 = keys [iprev];
                var k1 = keys [inext];
                var k = keys [i];
                k.inTangent = k.outTangent = (k1.value - k0.value) / (k1.time - k0.time);
                keys [i] = k;
            }
            return keys;
        }
    }
}
