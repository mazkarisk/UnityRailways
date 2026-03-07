using Geometry;
using System.Collections.Generic;
using UnityEngine;

public class TrackUtility {

	/// <summary>
	/// バラスト道床を構成する頂点の配列。
	/// </summary>
	public static readonly List<Vector3> CrossSectionVertices = new List<Vector3> {

		// 右側
		new Vector3(2.000f, 0.000f),
		new Vector3(1.900f, 0.020f),
		new Vector3(1.800f, 0.050f),
		new Vector3(1.400f, 0.250f),
		new Vector3(1.300f, 0.290f),
		new Vector3(1.250f, 0.300f),
		new Vector3(1.200f, 0.290f),
		new Vector3(1.000f, 0.210f),
		new Vector3(0.900f, 0.200f),
		new Vector3(0.500f, 0.190f),
		// 左側
		new Vector3(-0.500f, 0.190f),
		new Vector3(-0.900f, 0.200f),
		new Vector3(-1.000f, 0.210f),
		new Vector3(-1.200f, 0.290f),
		new Vector3(-1.250f, 0.300f),
		new Vector3(-1.300f, 0.290f),
		new Vector3(-1.400f, 0.250f),
		new Vector3(-1.800f, 0.050f),
		new Vector3(-1.900f, 0.020f),
		new Vector3(-2.000f, 0.000f),
	};

	public static Mesh CreateTrackbedMesh(Path path, int division) {

		List<Vector3> vertices = new List<Vector3>(4);
		for (int i = 0; i < CrossSectionVertices.Count; i++) {
			vertices.AddRange(path.GetPositionArray(division + 1, 0, CrossSectionVertices[i].x, CrossSectionVertices[i].y));
		}

		List<int> indices = ProcedualMesh.GetGridIndices(division, CrossSectionVertices.Count -1, 0);

		List<Vector2> uvs = new List<Vector2>();
		for (int i = 0; i < vertices.Count; i++) {
			uvs.Add(new Vector2(vertices[i].x, vertices[i].z));
		}

		Mesh mesh = new Mesh();
		mesh.SetVertices(vertices);
		mesh.SetUVs(0, uvs);
		mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();

		return mesh;
	}
}
