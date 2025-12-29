using System.Collections.Generic;
using UnityEngine;

public class RailUtility {

	/// <summary>
	/// レール断面を構成する頂点の配列。
	/// </summary>
	public static readonly List<Vector3> CrossSectionVertices = new List<Vector3> {
		// 頭頂面
		new Vector3(-0.020f, 0.140f), new Vector3(-0.007f, 0.141f), new Vector3(0.007f, 0.141f), new Vector3(0.020f, 0.140f), 
		// 頭部右側
		new Vector3(0.026f, 0.138f), new Vector3(0.030f, 0.134f), new Vector3(0.032f, 0.128f), new Vector3(0.032f, 0.119f), 
		// 右側面
		new Vector3(0.032f, 0.110f), new Vector3(0.020f, 0.105f), new Vector3(0.008f, 0.100f),
		new Vector3(0.008f, 0.060f), new Vector3(0.008f, 0.020f), new Vector3(0.034f, 0.015f), 
		// 底部
		new Vector3(0.060f, 0.010f), new Vector3(0.060f, 0.005f), new Vector3(0.060f, 0.000f), new Vector3(0.055f, 0.000f),
		new Vector3(-0.055f, 0.000f), new Vector3(-0.060f, 0.000f), new Vector3(-0.060f, 0.005f), new Vector3(-0.060f, 0.010f), 
		// 左側面
		new Vector3(-0.034f, 0.015f), new Vector3(-0.008f, 0.020f), new Vector3(-0.008f, 0.060f),
		new Vector3(-0.008f, 0.100f), new Vector3(-0.020f, 0.105f), new Vector3(-0.032f, 0.110f), 
		// 頭部左側
		new Vector3(-0.032f, 0.119f), new Vector3(-0.032f, 0.128f), new Vector3(-0.030f, 0.134f), new Vector3(-0.026f, 0.138f), 
		// 頭頂面(再掲)
		new Vector3(-0.020f, 0.140f)
	};

	/// <summary>
	/// レール断面のメッシュのインデックス。
	/// </summary>
	public static readonly List<int> CrossSectionIndices = new List<int> {
		// 頭部
		27, 28, 26, 28, 29, 26, 29, 30, 26, 30, 31, 26, 31, 0, 26,
		0, 1, 25, 0, 25, 26, 1, 2, 10, 1, 10, 25, 2, 3, 9, 2, 9, 10,
		3, 4, 9, 4, 5, 9, 5, 6, 9, 6, 7, 9, 7, 8, 9, 
		// ウェブ部
		25, 10, 11, 25, 11, 24, 24, 11, 12, 24, 12, 23, 
		// 底部
		23, 12, 13, 23, 13, 22, 22, 13, 14, 22, 14, 21,
		21, 14, 15, 21, 15, 20, 20, 15, 17, 20, 17, 18,
		15, 16, 17, 18, 19, 20
	};

	/// <summary>レール断面の頂点数のうち、レール頭頂面の頂点数。</summary>
	public const int TopVerticesCount = 4;

	/// <summary>レール断面の頂点数のうち、レール側面の頂点数。</summary>
	public const int SideVerticesCount = 30;

	public static Mesh CreateTopMesh(GameObject[] railChunkObjects, Transform railTransform, int meshDivision) {
		List<Vector3> vertices = new List<Vector3>();
		for (int i = 0; i <= meshDivision; i++) {
			float t = (float)i / meshDivision * railChunkObjects.Length;
			vertices.AddRange(TransformPoints(CrossSectionVertices.GetRange(0, TopVerticesCount), railChunkObjects, railTransform, t));
		}
		List<int> indices = ProcedualMesh.GetGridIndices(TopVerticesCount - 1, meshDivision, 0);

		Mesh mesh = new Mesh();
		mesh.SetVertices(vertices);
		mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();
		return mesh;
	}

	public static Mesh CreateSideMesh(GameObject[] railChunkObjects, Transform railTransform, int meshDivision) {
		System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

		// 側面・底面のメッシュ
		List<Vector3> vertices = new List<Vector3>();
		stopwatch.Restart();
		for (int i = 0; i <= meshDivision; i++) {
			float t = (float)i / meshDivision * railChunkObjects.Length;
			vertices.AddRange(TransformPoints(CrossSectionVertices.GetRange(TopVerticesCount - 1, SideVerticesCount), railChunkObjects, railTransform, t));
		}
		List<int> indices = ProcedualMesh.GetGridIndices(SideVerticesCount - 1, meshDivision, 0);
		stopwatch.Stop();
		Debug.Log("メッシュ作成時間 : " + stopwatch.Elapsed.TotalMilliseconds + " ms");

		// 端面(始点側)のメッシュ
		int offset1 = vertices.Count;
		vertices.AddRange(TransformPoints(CrossSectionVertices.GetRange(0, CrossSectionVertices.Count - 1), railChunkObjects, railTransform, 0f));
		for (int i = 0; i < CrossSectionIndices.Count; i++) {
			indices.Add(CrossSectionIndices[i] + offset1);
		}

		// 端面(終点側)のメッシュ
		int offset2 = vertices.Count;
		vertices.AddRange(TransformPoints(CrossSectionVertices.GetRange(0, CrossSectionVertices.Count - 1), railChunkObjects, railTransform, railChunkObjects.Length));
		for (int i = 0; i < CrossSectionIndices.Count; i++) {
			// 終点側は裏返す
			int i2 = i;
			if (i % 3 == 1) i2++;
			if (i % 3 == 2) i2--;

			indices.Add(CrossSectionIndices[i2] + offset2);
		}

		Mesh mesh = new Mesh();
		mesh.SetVertices(vertices);
		mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();

		return mesh;
	}

	/// <summary>
	/// Transformの重みづけ関数。
	/// </summary>
	/// <param name="x">入力値</param>
	/// <returns>Transformの重み。(-1 <= x <= 2)の範囲を二次関数で滑らかに結ぶ。</returns>
	private static float TransformBlendWeight(float x) {
		if (x <= -1) {
			return 0;
		} else if (x <= 0) {
			return 0.5f * (x + 1) * (x + 1);
		} else if (x <= 1) {
			return 0.75f - (x - 0.5f) * (x - 0.5f);
		} else if (x <= 2) {
			return 0.5f * (x - 2) * (x - 2);
		} else {
			return 0;
		}
	}

	private static List<Vector3> TransformPoints(List<Vector3> original, GameObject[] railChunkObjects, Transform railTransform, float t) {

		int tIntegerPart = Mathf.FloorToInt(t);
		float tFractionalPart = t - tIntegerPart;

		int i0 = tIntegerPart - 1;
		int i1 = tIntegerPart + 0;
		int i2 = tIntegerPart + 1;

		float t0 = tFractionalPart + 1;
		float t1 = tFractionalPart + 0;
		float t2 = tFractionalPart - 1;

		while (i0 < 0) {
			i0++;
			t0--;
		}
		while (i1 >= railChunkObjects.Length) {
			i1--;
			t1++;
		}
		while (i2 >= railChunkObjects.Length) {
			i2--;
			t2++;
		}

		float weight0 = TransformBlendWeight(tFractionalPart + 1);
		float weight1 = TransformBlendWeight(tFractionalPart + 0);
		float weight2 = TransformBlendWeight(tFractionalPart - 1);

		List<Vector3> points = new List<Vector3>();
		for (int j = 0; j < original.Count; j++) {
			Vector3 point0 = railChunkObjects[i0].transform.TransformPoint(new Vector3(original[j].x, original[j].y, t0));
			Vector3 point1 = railChunkObjects[i1].transform.TransformPoint(new Vector3(original[j].x, original[j].y, t1));
			Vector3 point2 = railChunkObjects[i2].transform.TransformPoint(new Vector3(original[j].x, original[j].y, t2));
			points.Add(railTransform.InverseTransformPoint(point0 * weight0 + point1 * weight1 + point2 * weight2));
		}

		return points;
	}
}
