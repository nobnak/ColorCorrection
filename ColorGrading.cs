using UnityEngine;
using System.Collections;
using System.IO;
using Gist;

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
        protected ImageLoader loader;
        protected System.DateTime _lastUpdateTime;

        #region Unity
        protected virtual void OnEnable() {
            loader = new ImageLoader (TextureFormat.ARGB32, false, true);
            loader.TextureCreate += (obj) => {
                obj.filterMode = FilterMode.Bilinear;
                obj.wrapMode = TextureWrapMode.Clamp;
                obj.anisoLevel = 0;
            };
            loader.TextureUpdate += (obj) => {
                Debug.Log("Update Texture");
                UpdateLUT(obj);
            };

            current = LoadTypeEnum.None;
            _lastUpdateTime = System.DateTime.MinValue;
        }
        protected virtual void OnDisable() {
            loader.Dispose ();
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
            if ((now - _lastUpdateTime).TotalSeconds > data.updateInterval) {
                _lastUpdateTime = now;

                var path = GetImagePath();
                loader.Update(path);

                current = (loader.Image != null ? LoadTypeEnum.Texture :
                    (data.alternativeImage != null ? LoadTypeEnum.ImageFile : LoadTypeEnum.None));
                switch (current) {
                case LoadTypeEnum.ImageFile:
                    UpdateLUT (data.alternativeImage);
                    break;
                }
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

        string GetImagePath() {
            return Path.Combine(GetSpecialFolder(System.Environment.SpecialFolder.MyDocuments), data.lutImageName);
        }

        [System.Serializable]
        public class Data {
            public float updateInterval = 0.5f;
            public string lutImageName = "LUT.png";
            public Texture2D alternativeImage;
        }
    }
}
