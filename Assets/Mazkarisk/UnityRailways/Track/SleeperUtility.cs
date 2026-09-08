using UnityEngine;

public class SleeperUtility {

	/// <summary>
	/// 枕木のメッシュを作成する。
	/// </summary>
	/// <remarks>
	/// 原点は底面の中央となる。テクスチャ座標については、以下のような展開図をUV空間に設定する。
	/// <code>
	/// 　　・─・　　　　<br />
	/// 　　│右│　　　　<br />
	/// ・─・─・─・─・<br />
	/// │　│　│　│　│<br />
	/// │前│上│後│底│<br />
	/// │　│　│　│　│<br />
	/// ・─・─・─・─・<br />
	/// 　　│左│　　　　<br />
	/// 　　・─・　　　　<br />
	/// </code>
	/// </remarks>
	/// <param name="size">枕木のサイズ</param>
	/// <param name="uvOffset">UV座標のオフセット値</param>
	/// <returns>枕木のMesh。</returns>
	public static Mesh CreateMesh(Vector3 size, Vector2 uvOffset) {

		// 簡単にするため、各種サイズを短い名称の変数に格納しておく。
		float x = size.x;
		float y = size.y;
		float z = size.z;
		float hx = size.x / 2;
		float hy = size.y / 2;
		float hz = size.z / 2;

		// 頂点位置を設定する。
		Vector3[] vertices = new Vector3[] {
			new Vector3(-hx, 0, hz), new Vector3(hx, 0, hz), new Vector3(-hx, y, hz), new Vector3(hx, y, hz), // 前面
			new Vector3(-hx, y, hz), new Vector3(hx, y, hz), new Vector3(-hx, y, -hz), new Vector3(hx, y, -hz), // 上面
			new Vector3(-hx, y, -hz), new Vector3(hx, y, -hz), new Vector3(-hx, 0, -hz), new Vector3(hx, 0, -hz), // 後面
			new Vector3(-hx, 0, -hz), new Vector3(hx, 0, -hz), new Vector3(-hx, 0, hz), new Vector3(hx, 0, hz), // 底面
			new Vector3(-hx, y, hz), new Vector3(-hx, y, -hz), new Vector3(-hx, 0, hz), new Vector3(-hx, 0, -hz), // 左面
			new Vector3(hx, y, -hz), new Vector3(hx, y, hz), new Vector3(hx, 0, -hz), new Vector3(hx, 0, hz), // 右面
		};

		// UV座標を設定する。実寸法のメートル値を半分にしてUV座標の計算に使用する。
		Vector2[] uvs = new Vector2[] {
			new Vector2(-hy, 0), new Vector2(-hy, hx), new Vector2(0, 0), new Vector2(0, hx), // 前面
			new Vector2(0, 0), new Vector2(0, hx), new Vector2(hz, 0), new Vector2(hz, hx), // 上面
			new Vector2(hz, 0), new Vector2(hz, hx), new Vector2(hz + hy, 0), new Vector2(hz + hy, hx), // 後面
			new Vector2(hz + hy, 0), new Vector2(hz + hy, hx), new Vector2(hz + hy + hz, 0), new Vector2(hz + hy + hz, hx), // 底面
			new Vector2(0, 0), new Vector2(hz, 0), new Vector2(0, -hy), new Vector2(hz, -hy), // 左面
			new Vector2(hz, hx), new Vector2(0, hx), new Vector2(hz, hx + hy), new Vector2(0, hx + hy), // 右面
		};

		// UV座標にオフセット値を適用する。
		for (int i = 0; i < uvs.Length; i++) {
			uvs[i] = uvs[i] + uvOffset;
		}

		// インデックスを設定する。
		int[] indices = new int[] {
			0, 1, 2, 2, 1, 3, // 前面
			4, 5, 6, 6, 5, 7, // 上面
			8, 9, 10, 10, 9, 11, // 後面
			12, 13, 14, 14, 13, 15, // 底面
			16, 17, 18, 18, 17, 19, // 左面
			20, 21, 22, 22, 21, 23, // 右面
		};

		Mesh mesh = new Mesh();
		mesh.SetVertices(vertices);
		mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		mesh.SetUVs(0, uvs);
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();

		return mesh;
	}
}
