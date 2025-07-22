using System.Collections.Generic;
using UnityEngine;

public class ProcedualMesh {

	/// <summary>
	/// 板部材の端部のメッシュの作成
	/// </summary>
	/// <param name="thickness">板厚(m単位)</param>
	/// <param name="height">高さ(m単位)</param>
	/// <param name="length">長さ(m単位)</param>
	/// <param name="endRound">端面のR加工幅(m単位)</param>
	/// <param name="sideRound">端面以外のR加工幅(m単位)</param>
	/// <returns></returns>
	public static Mesh PlateEnd(float thickness, float height, float length, float endRound, float sideRound) {
		Mesh mesh = new Mesh();

		float angle = Mathf.PI / 4;
		float x0 = -thickness / 2;
		float x1 = -thickness / 2 + sideRound;
		float x2 = thickness / 2 - sideRound;
		float x3 = thickness / 2;
		float y0 = 0;
		float y1 = sideRound;
		float y2 = height - sideRound;
		float y3 = height;
		float z0 = -thickness / 2 * Mathf.Sin(angle);
		float z1 = 0;
		float z2 = length - endRound;
		float z3 = length;

		// 頂点バッファの作成
		List<Vector3> vertices = Get3x3GridVertices(x0, x1, x2, x3, y0, y1, y2, y3, z0, z1, z2, z3);

		// UVバッファの作成
		List<Vector2> uvs = new List<Vector2>();
		// -X面
		int k = 0;
		for (int i = 0; i < 16; i++, k++) {
			uvs.Add(new Vector2(vertices[k].z, vertices[k].y));
		}
		// +X面
		for (int i = 0; i < 16; i++, k++) {
			uvs.Add(new Vector2(vertices[k].z, vertices[k].y));
		}
		// -Y面
		for (int i = 0; i < 16; i++, k++) {
			uvs.Add(new Vector2(vertices[k].x, vertices[k].z));
		}
		// +Y面
		for (int i = 0; i < 16; i++, k++) {
			uvs.Add(new Vector2(vertices[k].x, vertices[k].z));
		}
		// -Z面
		for (int i = 0; i < 16; i++, k++) {
			uvs.Add(new Vector2(vertices[k].x, vertices[k].y));
		}
		// +Z面
		for (int i = 0; i < 16; i++, k++) {
			uvs.Add(new Vector2(vertices[k].x, vertices[k].y));
		}

		// 頂点の法線バッファの作成
		List<Vector3> normals = Get3x3GridNormals(vertices);

		// 頂点の位置の調整
		for (int i = 0; i < vertices.Count; i++) {
			Vector3 vertex = vertices[i];
			if (vertex.z == z0) {
				vertices[i] = new Vector3(vertex.x * Mathf.Cos(angle), vertex.y, vertex.z);
			}
		}

		// インデックスバッファの作成
		List<int> indices = new List<int>();
		for (int i = 0; i < 6; i++) {
			indices.AddRange(GetGridIndices(3, 3, i * 16));
		}

		// メッシュの作成
		mesh.SetVertices(vertices);
		mesh.SetNormals(normals);
		mesh.SetUVs(0, uvs);
		mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		mesh.RecalculateBounds();
		mesh.RecalculateTangents();

		return mesh;
	}

	/// <summary>
	/// 3×3の格子状に整列したメッシュで構成された直方体の頂点バッファを作成する。
	/// </summary>
	/// <returns>作成した頂点バッファ</returns>
	private static List<Vector3> Get3x3GridVertices(float x0, float x1, float x2, float x3, float y0, float y1, float y2, float y3, float z0, float z1, float z2, float z3) {
		List<Vector3> vertices = new List<Vector3> {
			// -X面
			new Vector3(x0, y0, z3), new Vector3(x0, y0, z2), new Vector3(x0, y0, z1), new Vector3(x0, y0, z0),
			new Vector3(x0, y1, z3), new Vector3(x0, y1, z2), new Vector3(x0, y1, z1), new Vector3(x0, y1, z0),
			new Vector3(x0, y2, z3), new Vector3(x0, y2, z2), new Vector3(x0, y2, z1), new Vector3(x0, y2, z0),
			new Vector3(x0, y3, z3), new Vector3(x0, y3, z2), new Vector3(x0, y3, z1), new Vector3(x0, y3, z0),

			// +X面
			new Vector3(x3, y0, z0), new Vector3(x3, y0, z1), new Vector3(x3, y0, z2), new Vector3(x3, y0, z3),
			new Vector3(x3, y1, z0), new Vector3(x3, y1, z1), new Vector3(x3, y1, z2), new Vector3(x3, y1, z3),
			new Vector3(x3, y2, z0), new Vector3(x3, y2, z1), new Vector3(x3, y2, z2), new Vector3(x3, y2, z3),
			new Vector3(x3, y3, z0), new Vector3(x3, y3, z1), new Vector3(x3, y3, z2), new Vector3(x3, y3, z3),

			// -Y面
			new Vector3(x3, y0, z0), new Vector3(x2, y0, z0), new Vector3(x1, y0, z0), new Vector3(x0, y0, z0),
			new Vector3(x3, y0, z1), new Vector3(x2, y0, z1), new Vector3(x1, y0, z1), new Vector3(x0, y0, z1),
			new Vector3(x3, y0, z2), new Vector3(x2, y0, z2), new Vector3(x1, y0, z2), new Vector3(x0, y0, z2),
			new Vector3(x3, y0, z3), new Vector3(x2, y0, z3), new Vector3(x1, y0, z3), new Vector3(x0, y0, z3),

			// +Y面
			new Vector3(x0, y3, z0), new Vector3(x1, y3, z0), new Vector3(x2, y3, z0), new Vector3(x3, y3, z0),
			new Vector3(x0, y3, z1), new Vector3(x1, y3, z1), new Vector3(x2, y3, z1), new Vector3(x3, y3, z1),
			new Vector3(x0, y3, z2), new Vector3(x1, y3, z2), new Vector3(x2, y3, z2), new Vector3(x3, y3, z2),
			new Vector3(x0, y3, z3), new Vector3(x1, y3, z3), new Vector3(x2, y3, z3), new Vector3(x3, y3, z3),

			// -Z面
			new Vector3(x0, y0, z0), new Vector3(x1, y0, z0), new Vector3(x2, y0, z0), new Vector3(x3, y0, z0),
			new Vector3(x0, y1, z0), new Vector3(x1, y1, z0), new Vector3(x2, y1, z0), new Vector3(x3, y1, z0),
			new Vector3(x0, y2, z0), new Vector3(x1, y2, z0), new Vector3(x2, y2, z0), new Vector3(x3, y2, z0),
			new Vector3(x0, y3, z0), new Vector3(x1, y3, z0), new Vector3(x2, y3, z0), new Vector3(x3, y3, z0),

			// +Z面
			new Vector3(x3, y0, z3), new Vector3(x2, y0, z3), new Vector3(x1, y0, z3), new Vector3(x0, y0, z3),
			new Vector3(x3, y1, z3), new Vector3(x2, y1, z3), new Vector3(x1, y1, z3), new Vector3(x0, y1, z3),
			new Vector3(x3, y2, z3), new Vector3(x2, y2, z3), new Vector3(x1, y2, z3), new Vector3(x0, y2, z3),
			new Vector3(x3, y3, z3), new Vector3(x2, y3, z3), new Vector3(x1, y3, z3), new Vector3(x0, y3, z3)
		};
		return vertices;
	}

	/// <summary>
	/// 3×3の格子状に整列したメッシュで構成された直方体の法線バッファを作成する。
	/// </summary>
	/// <returns>作成した頂点バッファ</returns>
	private static List<Vector3> Get3x3GridNormals(List<Vector3> vertices) {

		// 各座標の最小値・最大値の算出
		float x0 = float.PositiveInfinity;
		float x3 = float.NegativeInfinity;
		float y0 = float.PositiveInfinity;
		float y3 = float.NegativeInfinity;
		float z0 = float.PositiveInfinity;
		float z3 = float.NegativeInfinity;
		for (int i = 0; i < vertices.Count; i++) {
			Vector3 vertex = vertices[i];
			if (vertex.x < x0) x0 = vertex.x;
			if (vertex.x > x3) x3 = vertex.x;
			if (vertex.y < y0) y0 = vertex.y;
			if (vertex.y > y3) y3 = vertex.y;
			if (vertex.z < z0) z0 = vertex.z;
			if (vertex.z > z3) z3 = vertex.z;
		}

		// 各座標の最小値・最大値と一致するか判定して法線を作成
		List<Vector3> normals = new List<Vector3>();
		for (int i = 0; i < vertices.Count; i++) {
			Vector3 vertex = vertices[i];
			Vector3 normal = Vector3.zero;
			if (vertex.x == x0) {
				normal.x = -1;
			} else if (vertex.x == x3) {
				normal.x = 1;
			}
			if (vertex.y == y0) {
				normal.y = -1;
			} else if (vertex.y == y3) {
				normal.y = 1;
			}
			if (vertex.z == z0) {
				normal.z = -1;
			} else if (vertex.z == z3) {
				normal.z = 1;
			}
			normals.Add(normal.normalized);
		}

		return normals;
	}

	/// <summary>
	/// 格子状に整列したメッシュのインデックスバッファを作成する。
	/// </summary>
	/// <param name="gridSizeX">格子の横サイズ(四角形の数)</param>
	/// <param name="gridSizeY">格子の縦サイズ(四角形の数)</param>
	/// <param name="offset">インデックスのオフセット</param>
	/// <returns>作成したインデックスバッファ</returns>
	private static List<int> GetGridIndices(int gridSizeX, int gridSizeY, int offset) {
		List<int> indices = new List<int>();

		for (int y = 0; y < gridSizeY; y++) {
			for (int x = 0; x < gridSizeX; x++) {
				int offsetOfThisGrid = y * (gridSizeX + 1) + x + offset;
				List<int> indicesTemp = new List<int> {
					offsetOfThisGrid + 0                  ,
					offsetOfThisGrid + 0 + (gridSizeX + 1),
					offsetOfThisGrid + 1 + (gridSizeX + 1),
					offsetOfThisGrid + 1 + (gridSizeX + 1),
					offsetOfThisGrid + 1                  ,
					offsetOfThisGrid + 0
				};
				indices.AddRange(indicesTemp);
			}
		}

		return indices;
	}

}
