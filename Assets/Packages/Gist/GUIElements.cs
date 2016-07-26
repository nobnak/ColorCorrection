using UnityEngine;
using System.Collections;

namespace Gist {

    public static class GUIElements {
        public abstract class ElementNumberBase<T> {
            protected bool _changed = false;
            protected string _text;
            protected T _value;

            public ElementNumberBase() : this(default(T)) {}
            public ElementNumberBase(T initialValue) {
                Value = initialValue;
            }

            public string Text {
                get {
                    return _text;
                }
                set {
                    _changed = true;
                    _text = value;
                }
            }
            public T Value {
                get {
                    if (_changed) {
                        _changed = false;
                        TryParse (_text, out _value);
                    }
                    return _value;
                }
                set {
                    _value = value;
                    _text = value.ToString ();
                }
            }

            public abstract bool TryParse(string text, out T value);
        }

        public class ElementEnum<T> : ElementNumberBase<T> {
            public override bool TryParse(string text, out T value) {
                value = (T) System.Enum.Parse(typeof(T), text);
                return System.Enum.IsDefined (typeof(T), value);
            }
        }
        public class ElementInt : ElementNumberBase<int> {
            public override bool TryParse(string text, out int value) {
                return int.TryParse(text, out value);
            }
        }
        public class ElementFloat : ElementNumberBase<float> {
            public override bool TryParse (string text, out float value) {
                return float.TryParse(text, out value);
            }
        }
    }
}
