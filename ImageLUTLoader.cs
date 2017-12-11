using UnityEngine;
using System.Collections;
using System.IO;
using nobnak.Gist;

namespace ColorCorrection {
    [ExecuteInEditMode]
    public class ImageLUTLoader : LUTGenerator {
        public enum LoadTypeEnum { None = 0, ImageFile, Texture }

        [SerializeField]
        protected Data data;

        protected LoadTypeEnum current;
        protected ImageLoader loader;
        protected System.DateTime _lastUpdateTime;

        #region Unity
        protected override void OnEnable() {
            base.OnEnable ();

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
        protected virtual void Update() {
            var now = System.DateTime.Now;
            if ((now - _lastUpdateTime).TotalSeconds > data.updateInterval) {
                _lastUpdateTime = now;

                var path = GetImagePath();
                loader.Update(path);

                var next = (loader.Image != null ? LoadTypeEnum.Texture :
                    (data.alternativeImage != null ? LoadTypeEnum.ImageFile : LoadTypeEnum.None));
                switch (next) {
                case LoadTypeEnum.ImageFile:
                    UpdateLUT (data.alternativeImage);
                    break;
                case LoadTypeEnum.None:
                    if (current != LoadTypeEnum.None)
                        lut.SetDefault ();
                    break;
                }
                current = next;
            }
        }
        protected override void OnDisable() {
            loader.Dispose ();
            base.OnDisable ();
        }
        #endregion

		protected void UpdateLUT (Texture2D tex) {
			lut.Convert(tex);
            NotifyOnUpdate ();
        }

        public static string GetSpecialFolder(System.Environment.SpecialFolder folder) {
            return System.Environment.GetFolderPath (folder);
        }
        protected string GetImagePath() {
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
