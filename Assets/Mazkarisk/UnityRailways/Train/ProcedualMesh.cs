using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProcedualMesh {

	/// <summary>
	/// 板部材の"凸"字部のメッシュの作成
	/// </summary>
	/// <remarks>
	/// 以下の図のようなメッシュを作成する。原点はTop部上面の中心から垂直に下した線と底面が交差する場所であり、必ずしも底面の中心ではない。
	/// <code>
	/// 　　┏━┓　　　　　<br/>
	/// 　　┃Ｔ┃　+Y +Z 　<br/>
	/// ┏━┛　┃　↑↗ 　　<br/>
	/// ┃　│＼┃　└→+X　<br/>
	/// ┃Ｌ│　┗━┓　　　<br/>
	/// ┃　│　│Ｒ┃　　　<br/>
	/// ┗━━┿━━┛　　　<br/>
	/// 　　　↑原点
	/// </code>
	/// </remarks>
	/// <returns></returns>
	public static Mesh InversedTMesh(float thickness, float overallWidth, float overallHeight, float leftNotchWidth, float leftNotchHeight, float rightNotchWidth, float rightNotchHeight) {
		float zn = -thickness / 2;
		float zp = thickness / 2;

		float x0 = -(leftNotchWidth + (overallWidth - leftNotchWidth - rightNotchWidth) / 2);
		float x1 = -(overallWidth - leftNotchWidth - rightNotchWidth) / 2;
		float x2 = (overallWidth - leftNotchWidth - rightNotchWidth) / 2;
		float x3 = (rightNotchWidth + (overallWidth - leftNotchWidth - rightNotchWidth) / 2);

		float yLeftTop = overallHeight - leftNotchHeight;
		float yRightTop = overallHeight - rightNotchHeight;

		List<Vector3> vertices = new List<Vector3> {
			// -X面(Left部)
			new Vector3(x0, yLeftTop, zp), new Vector3(x0, yLeftTop, zn), new Vector3(x0, 0, zp), new Vector3(x0, 0, zn),
			// -X面(Top部)
			new Vector3(x1, overallHeight, zp), new Vector3(x1, overallHeight, zn), new Vector3(x1, yLeftTop, zp), new Vector3(x1, yLeftTop, zn),
			
			// +X面(Top部)
			new Vector3(x2, overallHeight, zn), new Vector3(x2, overallHeight, zp), new Vector3(x2, yRightTop, zn), new Vector3(x2, yRightTop, zp),
			// +X面(Right部)
			new Vector3(x3, yRightTop, zn), new Vector3(x3, yRightTop, zp), new Vector3(x3, 0, zn), new Vector3(x3, 0, zp),
			
			// -Y面
			new Vector3(x3, 0, zn), new Vector3(x3, 0, zp),
			new Vector3(x2, 0, zn), new Vector3(x2, 0, zp),
			new Vector3(x1, 0, zn), new Vector3(x1, 0, zp),
			new Vector3(x0, 0, zn), new Vector3(x0, 0, zp),
			
			// +Y面(Left部)
			new Vector3(x0, yLeftTop, zn), new Vector3(x0, yLeftTop, zp),
			new Vector3(x1, yLeftTop, zn), new Vector3(x1, yLeftTop, zp), 
			// +Y面(Top部)
			new Vector3(x1, overallHeight, zn), new Vector3(x1, overallHeight, zp),
			new Vector3(x2, overallHeight, zn), new Vector3(x2, overallHeight, zp),
			// +Y面(Right部)
			new Vector3(x2, yRightTop, zn), new Vector3(x2, yRightTop, zp),
			new Vector3(x3, yRightTop, zn), new Vector3(x3, yRightTop, zp), 
			
			// -Z面
			new Vector3(x1, overallHeight, zn), new Vector3(x2, overallHeight, zn),
			new Vector3(x0, yLeftTop, zn), new Vector3(x1, yLeftTop, zn), new Vector3(x2, yRightTop, zn), new Vector3(x3, yRightTop, zn),
			new Vector3(x0, 0, zn), new Vector3(x1, 0, zn), new Vector3(x2, 0, zn), new Vector3(x3, 0, zn),
			
			// +Z面
			new Vector3(x2, overallHeight, zp), new Vector3(x1, overallHeight, zp),
			new Vector3(x3, yRightTop, zp), new Vector3(x2, yRightTop, zp), new Vector3(x1, yLeftTop, zp), new Vector3(x0, yLeftTop, zp),
			new Vector3(x3, 0, zp), new Vector3(x2, 0, zp), new Vector3(x1, 0, zp), new Vector3(x0, 0, zp)
		};

		List<Vector3> normals = new List<Vector3> {
			// -X面
			Vector3.left, Vector3.left, Vector3.left, Vector3.left,
			Vector3.left, Vector3.left, Vector3.left, Vector3.left,
			// +X面
			Vector3.right, Vector3.right, Vector3.right, Vector3.right,
			Vector3.right, Vector3.right, Vector3.right, Vector3.right,
			// -Y面
			Vector3.down, Vector3.down, Vector3.down, Vector3.down,
			Vector3.down, Vector3.down, Vector3.down, Vector3.down,
			// +Y面
			Vector3.up, Vector3.up, Vector3.up, Vector3.up,
			Vector3.up, Vector3.up, Vector3.up, Vector3.up,
			Vector3.up, Vector3.up, Vector3.up, Vector3.up,
			// -Z面
			Vector3.back, Vector3.back,
			Vector3.back, Vector3.back, Vector3.back, Vector3.back,
			Vector3.back, Vector3.back, Vector3.back, Vector3.back,
			// +Z面
			Vector3.forward, Vector3.forward,
			Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward,
			Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward
		};

		List<int> indices = new List<int> {
			// -X面
			0, 1, 2, 2, 1, 3,
			4, 5, 6, 6, 5, 7,
			// +X面
			8, 9, 10, 10, 9, 11,
			12, 13, 14, 14, 13, 15,
			// -Y面
			16, 17, 18, 18, 17, 19,
			18, 19, 20, 20, 19, 21,
			20, 21, 22, 22, 21, 23,
			// +Y面
			24, 25, 26, 26, 25, 27,
			28, 29, 30, 30, 29, 31,
			32, 33, 34, 34, 33, 35,
			// -Z面
			36, 37, 39, 39, 37, 40,
			38, 39, 42, 42, 39, 43,
			39, 40, 43, 43, 40, 44,
			40, 41, 44, 44, 41, 45,
			// +Z面
			46, 47, 49, 49, 47, 50,
			48, 49, 52, 52, 49, 53,
			49, 50, 53, 53, 50, 54,
			50, 51, 54, 54, 51, 55
		};

		// メッシュの作成
		Mesh mesh = new Mesh();
		mesh.SetVertices(vertices);
		mesh.SetNormals(normals);
		mesh.SetUVs(0, PlanarMapUVs(vertices, normals, Vector3.zero));
		//mesh.SetUVs(0, uvs);
		mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		mesh.RecalculateBounds();
		mesh.RecalculateTangents();

		return mesh;
	}

	/// <summary>
	/// 3×3の格子状に整列したメッシュで構成された直方体の頂点バッファを作成する。
	/// </summary>
	/// <returns>作成した頂点バッファ</returns>
	public static List<Vector3> Get3x3GridVertices(float x0, float x1, float x2, float x3, float y0, float y1, float y2, float y3, float z0, float z1, float z2, float z3) {
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
	public static List<Vector3> Get3x3GridNormals(List<Vector3> vertices) {

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
	/// 3×3の格子状に整列したメッシュで構成された直方体のUVバッファを作成する。
	/// </summary>
	/// <returns>作成したUVバッファ</returns>
	public static List<Vector2> Get3x3GridUVs(List<Vector3> vertices, Vector3 offset) {
		List<Vector2> uvs = new List<Vector2>();

		// -X面
		int k = 0;
		for (int i = 0; i < 16; i++, k++) {
			Vector3 vertex = vertices[k] + offset;
			uvs.Add(new Vector2(-vertices[k].z, vertices[k].y));
		}
		// +X面
		for (int i = 0; i < 16; i++, k++) {
			Vector3 vertex = vertices[k] + offset;
			uvs.Add(new Vector2(vertices[k].z, vertices[k].y));
		}
		// -Y面
		for (int i = 0; i < 16; i++, k++) {
			Vector3 vertex = vertices[k] + offset;
			uvs.Add(new Vector2(-vertices[k].x, vertices[k].z));
		}
		// +Y面
		for (int i = 0; i < 16; i++, k++) {
			Vector3 vertex = vertices[k] + offset;
			uvs.Add(new Vector2(-vertices[k].x, -vertices[k].z));
		}
		// -Z面
		for (int i = 0; i < 16; i++, k++) {
			Vector3 vertex = vertices[k] + offset;
			uvs.Add(new Vector2(-vertices[k].x, -vertices[k].y));
		}
		// +Z面
		for (int i = 0; i < 16; i++, k++) {
			Vector3 vertex = vertices[k] + offset;
			uvs.Add(new Vector2(-vertices[k].x, vertices[k].y));
		}

		return uvs;
	}

	/// <summary>
	/// 3×3の格子状に整列したメッシュで構成された直方体のインデックスバッファを作成する。
	/// </summary>
	/// <returns>作成したインデックスバッファ</returns>
	public static List<int> Get3x3GridIndices() {
		List<int> indices = new List<int>();

		for (int i = 0; i < 6; i++) {
			indices.AddRange(GetGridIndices(3, 3, i * 16));
		}

		return indices;
	}

	/// <summary>
	/// 格子状に整列したメッシュのインデックスバッファを作成する。
	/// </summary>
	/// <param name="gridSizeX">格子の横サイズ(四角形の数)</param>
	/// <param name="gridSizeY">格子の縦サイズ(四角形の数)</param>
	/// <param name="offset">インデックスのオフセット</param>
	/// <returns>作成したインデックスバッファ</returns>
	public static List<int> GetGridIndices(int gridSizeX, int gridSizeY, int offset) {
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

	/// <summary>
	/// 頂点と法線の情報からPlanar MappingでUVバッファを作成する。
	/// </summary>
	/// <returns>作成したUVバッファ</returns>
	public static List<Vector2> PlanarMapUVs(List<Vector3> vertices, List<Vector3> normals, Vector3 offset) {
		List<Vector2> uvs = new List<Vector2>();

		for (int i = 0; i < normals.Count; i++) {
			Vector3 vertex = vertices[i] + offset;
			Vector3 normal = normals[i];
			float absX = Mathf.Abs(normal.x);
			float absY = Mathf.Abs(normal.y);
			float absZ = Mathf.Abs(normal.z);
			if (absX > absY && absX > absZ) {
				if (normal.x < 0) {
					// -X面
					uvs.Add(new Vector2(-vertices[i].z, vertices[i].y));
				} else {
					// +X面
					uvs.Add(new Vector2(vertices[i].z, vertices[i].y));
				}
			} else if (absY > absZ) {
				if (normal.y < 0) {
					// -Y面
					uvs.Add(new Vector2(-vertices[i].x, vertices[i].z));
				} else {
					// +Y面
					uvs.Add(new Vector2(-vertices[i].x, -vertices[i].z));
				}
			} else {
				if (normal.z < 0) {
					// -Z面
					uvs.Add(new Vector2(-vertices[i].x, -vertices[i].y));
				} else {
					// +Z面
					uvs.Add(new Vector2(-vertices[i].x, vertices[i].y));
				}
			}
		}

		return uvs;
	}

	public static Mesh MergeMeshes(params Mesh[] meshes) {
		Mesh mergedMesh = null;
		for (int i = 0; i < meshes.Length; i++) {
			mergedMesh = MergeTwoMeshes(mergedMesh, meshes[i]);
		}
		return mergedMesh;
	}

	private static Mesh MergeTwoMeshes(Mesh a, Mesh b) {

		// nullや空だった場合の処理
		bool aIsEmpty = a == null || a.vertices.Length == 0;
		bool bIsEmpty = b == null || b.vertices.Length == 0;
		if (aIsEmpty && bIsEmpty) {
			return null;
		} else if (aIsEmpty) {
			return b;
		} else if (bIsEmpty) {
			return a;
		}

		// 頂点のマージ
		List<Vector3> vertices = new List<Vector3>();
		vertices.AddRange(a.vertices);
		vertices.AddRange(b.vertices);

		// 法線のマージ
		List<Vector3> normals = new List<Vector3>();
		vertices.AddRange(a.normals);
		vertices.AddRange(b.normals);

		// UVのマージ
		List<Vector2> uv = new List<Vector2>();
		uv.AddRange(a.uv);
		uv.AddRange(b.uv);

		// インデックスのマージ
		List<int> indices = new List<int>();
		for (int i = 0; i < a.GetIndices(0).Length; i++) {
			indices.Add(a.GetIndices(0)[i]);
		}
		for (int i = 0; i < b.GetIndices(0).Length; i++) {
			indices.Add(b.GetIndices(0)[i] + a.vertices.Length);
		}

		// メッシュの作成
		Mesh mesh = new Mesh();
		mesh.SetVertices(vertices);
		mesh.SetNormals(normals);
		mesh.SetUVs(0, uv);
		mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		mesh.RecalculateBounds();
		mesh.RecalculateTangents();

		return mesh;
	}
}
