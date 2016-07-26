using UnityEngine;
using System.Collections;

namespace ColorCorrection {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Renderer))]
    public class AspectFitting : MonoBehaviour {
        public float size = 1f;

        Material _mat;
    	
    	void OnEnable () {
            _mat = GetComponent<Renderer> ().sharedMaterial;
    	}
    	
    	void Update () {
            if (_mat == null)
                return;
            var tex = _mat.mainTexture;
            if (tex == null)
                return;

            var aspect = (float)tex.width / tex.height;
            transform.localScale = new Vector3 (size * aspect, size, 1f);
    	}
    }
}