using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using nobnak.Gist;
using nobnak.Gist.ObjectExt;
using nobnak.Gist.Extensions.Texture2DExt;

namespace ColorCorrection {
    [ExecuteInEditMode]
    public class ImageLUTSaver : MonoBehaviour {
        public System.Environment.SpecialFolder folderEnum = System.Environment.SpecialFolder.MyDocuments;
        public string filename = "LUT.png";

        protected Texture2D buffer;

        #region Unity
        void OnDisable() {
			buffer.Destroy();
        }
        #endregion

        public void Save(LUT3D lut) {
            if (lut == null)
                return;

            var tex = GetBuffer ();
            lut.ConvertBack (tex);
            File.WriteAllBytes (GetPath (), tex.EncodeToPNG ());
        }

        public string GetFolder() {
            return System.Environment.GetFolderPath (folderEnum);
        }
        public string GetPath() {
            return Path.Combine (GetFolder (), filename);
        }
        public Texture2D GetBuffer() {
            if (buffer == null)
                buffer = buffer.Create(4, 4, TextureFormat.ARGB32, false, true);
            return buffer;
        }
    }
}
