using System.Collections.Generic;
using UnityEngine;

public class ProcedualMesh
{
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

		float angle = Mathf.PI / 6;
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

		// 頂点の法線バッファの作成と位置の調整
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

			if (vertex.z == z0) {
				vertices[i] = new Vector3(vertex.x * Mathf.Cos(angle), vertex.y, vertex.z);
			}
		}

		// インデックスバッファの作成
		List<int> indicesTemp = new List<int> {
			0, 4, 5, 5, 1, 0, 1, 5, 6, 6, 2, 1, 2, 6, 7, 7, 3, 2,
			4, 8, 9, 9, 5, 4, 5, 9, 10, 10, 6, 5, 6, 10, 11, 11, 7, 6,
			8, 12, 13, 13, 9, 8, 9, 13, 14, 14, 10, 9, 10, 14, 15, 15, 11, 10
		};
		List<int> indices = new List<int>();
		for (int i = 0; i < 6; i++) {
			for (int j = 0; j < indicesTemp.Count; j++) {
				indices.Add(indicesTemp[j] + i * 16);
			}
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
}
