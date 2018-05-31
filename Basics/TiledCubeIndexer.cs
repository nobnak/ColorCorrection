using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorCorrection {

	public struct TiledCubeIndexer {
		public readonly int cubeSize;
		public readonly int tileWidth;
		public readonly int tileHeight;

		public TiledCubeIndexer(int cubeSize, int tileWidth) {
			this.cubeSize = cubeSize;
			this.tileWidth = tileWidth;
			this.tileHeight = cubeSize / tileWidth;
		}

		public int this[int x, int y, int z] {
			get {
				var zAlongHeight = z / tileWidth;
				var zAlongWidth = z - tileWidth * zAlongHeight;

				var texZAlongHeight = (tileHeight - 1) - zAlongHeight;
				var zOffset = (
					tileWidth * ((texZAlongHeight + 1) * cubeSize - 1)
					+ zAlongWidth
					) * cubeSize;
				var yOffset = -y * tileWidth * cubeSize;
				return x + yOffset + zOffset;
			}
		}
		public int this[Vector3Int p] {
			get {
				return this[p.x, p.y, p.z];
			}
		}
	}
}
