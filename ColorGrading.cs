using UnityEngine;
using System.Collections;
using System.IO;

namespace ColorCorrection {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public abstract class ColorGrading : MonoBehaviour {
        public enum LoadTypeEnum { None = 0, ImageFile, Texture }

        [SerializeField]
        protected Data data;
        [SerializeField]
        protected Material filterMat;

        protected LoadTypeEnum current;
        protected Texture2D _lutImage;
        protected System.DateTime _lastUpdateTime;

        #region Unity
        protected virtual void OnEnable() {
            current = LoadTypeEnum.None;
            _lastUpdateTime = System.DateTime.MinValue;
        }
        protected virtual void OnDisable() {
            ReleaseImageTex();
        }
        protected virtual void OnRenderImage(RenderTexture src, RenderTexture dst) {
            if (filterMat == null || current == LoadTypeEnum.None) {
                Graphics.Blit (src, dst);
                return;
            }
            SetProperty (filterMat);
			Graphics.Blit (src, dst, filterMat, GetPass());
        }
        protected virtual void Update() {
            var now = System.DateTime.Now;
            var d = now - _lastUpdateTime;
            if (d.TotalSeconds > data.updateInterval) {
                _lastUpdateTime = now;
                UpdateLUT ();
            }
        }
        #endregion

        #region Static
        public static string GetSpecialFolder(System.Environment.SpecialFolder folder) {
            return System.Environment.GetFolderPath (folder);
        }
        #endregion

        protected abstract void SetProperty (Material mat);
        protected abstract int GetPass();
        protected abstract void UpdateLUT (Texture2D lut);

        protected virtual void UpdateLUT() {
            try {
                var path = GetImagePath();
                var next = (File.Exists (path) ? LoadTypeEnum.ImageFile : 
                    (data.alternativeImage != null ? LoadTypeEnum.Texture : LoadTypeEnum.None));

                switch (next) {
                case LoadTypeEnum.ImageFile:
                    var writeTime = File.GetLastWriteTime (path);
                    if (writeTime != _lastUpdateTime) {
                        Debug.LogFormat("Load image from {0}", path);
                        _lastUpdateTime = writeTime;
                        LoadImage (path);
                        UpdateLUT (_lutImage);
                    }
                    break;
                case LoadTypeEnum.Texture:
                    ReleaseImageTex();
                    if (current != LoadTypeEnum.Texture && data.alternativeImage != null)
                        UpdateLUT(data.alternativeImage);
                    break;
                default:
                    ReleaseImageTex();
                    break;
                }

                current = next;
            } catch (System.Exception e) {
                Debug.LogError (e);
            }
        }

        string GetImagePath() {
            return Path.Combine(GetSpecialFolder(System.Environment.SpecialFolder.MyDocuments), data.lutImageName);
        }
        void LoadImage (string imageFilePath) {
            CheckInitImageTex ();
            _lutImage.LoadImage (File.ReadAllBytes (imageFilePath));
        }
        void CheckInitImageTex () {
            if (_lutImage == null) {
                _lutImage = new Texture2D (2, 2, TextureFormat.ARGB32, false, true);
                _lutImage.filterMode = FilterMode.Bilinear;
                _lutImage.wrapMode = TextureWrapMode.Clamp;
                _lutImage.anisoLevel = 0;
            }
        }
        void ReleaseImageTex () {
            if (_lutImage != null)
                DestroyImmediate (_lutImage);
        }

        [System.Serializable]
        public class Data {
            public float updateInterval = 0.5f;
            public string lutImageName = "LUT.png";
            public Texture2D alternativeImage;
        }
    }
}
