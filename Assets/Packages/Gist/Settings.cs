using UnityEngine;
using System.Collections;
using System.IO;

namespace Gist {
        
    public abstract class Settings : MonoBehaviour {
        [SerializeField]
        public enum PathTypeEnum { StreamingAssets = 0, MyDocuments }
        public enum ModeEnum { Normal = 0, GUI = 1, End = 2 }
        public const float WINDOW_WIDTH = 300f;

        public PathTypeEnum pathType;
        public ModeEnum mode;
        public string dataPath;
        public KeyCode toggleKey = KeyCode.None;
    }
    public abstract class Settings<T> : Settings {
        public T data;

        Rect _window;

        protected virtual void OnEnable() {
            Load ();
        }
        protected virtual void OnDisable() {
            Save();
        }
        protected virtual void Update() {
            if (Input.GetKeyDown (toggleKey)) {
                mode = (ModeEnum)(((int)mode + 1) % (int)ModeEnum.End);
                if (mode == ModeEnum.Normal)
                    Save ();
            }
        }
        protected virtual void OnGUI() {
            if (mode == ModeEnum.GUI)
                _window = GUILayout.Window (GetInstanceID(), _window, Window, this.name, GUILayout.MinWidth (WINDOW_WIDTH));
        }

        #region GUI
        protected virtual void DrawGUI() {
        }
        void Window(int id) {
            DrawGUI ();
            GUI.DragWindow ();
        }
        #endregion

        #region Save/Load
        protected virtual void Load() {
            string path;
            if (!DataPath (out path))
                return;

            try {
                data = JsonUtility.FromJson<T>(File.ReadAllText(path));
            } catch (System.Exception e) {
                Debug.Log (e);
            }
        }
        protected virtual void Save() {
            string path;
            if (!DataPath (out path))
                return;

            try {
                File.WriteAllText(path, JsonUtility.ToJson(data, true));
            } catch (System.Exception e) {
                Debug.Log (e);
            }
        }
        protected virtual bool DataPath(out string path) {
            path = MakePath (pathType, dataPath);
            return !string.IsNullOrEmpty (dataPath);
        }

        public static string MakePath(PathTypeEnum pathType, string filename) {
            var dir = Application.streamingAssetsPath;
            switch (pathType) {
            case PathTypeEnum.MyDocuments:
                dir = System.Environment.GetFolderPath (System.Environment.SpecialFolder.MyDocuments);
                break;
            }
            return Path.Combine (dir, filename);
        }
        #endregion
    }
}
