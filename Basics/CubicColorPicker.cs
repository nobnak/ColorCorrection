using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorCorrection {
	
	public struct CubicColorPicker {
		public readonly int size;
		public readonly System.Func<int, int, int, Color> Getter;
		public readonly System.Action<int, int, int, Color> Setter;

		public CubicColorPicker(int size, 
			System.Func<int, int, int, Color> getter,
			System.Action<int, int, int, Color> setter) {

			this.size = size;
			this.Getter = getter;
			this.Setter = setter;
		}

		public Color this[int x, int y, int z] {
			get {
				return Getter(x, y, z);
			}
			set {
				Setter(x, y, z, value);
			}
		}

		public void SetIdentity() {
			var dx = 1f / (size - 1);
			for (var z = 0; z < size; z++)
				for (var y = 0; y < size; y++)
					for (var x = 0; x < size; x++)
						this[x,y,z] = new Color(x * dx, y * dx, z * dx, 1f);
		}
		public IEnumerable<Color> Sequence() {
			for (var z = 0; z < size; z++)
				for (var y = 0; y < size; y++)
					for (var x = 0; x < size; x++)
						yield return this[x, y, z];
		}
	}
}
