using System.Collections.Generic;
using UnityEngine;

public class Rail : MonoBehaviour {
	[SerializeField]
	GameObject modelsObject;
	[SerializeField]
	GameObject collidersObject;
	[SerializeField]
	GameObject headObject;

	public void SetLength(float length) {
		modelsObject.transform.localScale = new Vector3(1, 1, length);

		CapsuleCollider[] capsuleColliders = collidersObject.GetComponentsInChildren<CapsuleCollider>();
		BoxCollider[] boxColliders = collidersObject.GetComponentsInChildren<BoxCollider>();

		/*
		for (int i = 0; i < capsuleColliders.Length; i++) {
			capsuleColliders[i].height = length + capsuleColliders[i].radius * 2;
			capsuleColliders[i].center = capsuleColliders[i].center + Vector3.forward * length * 0.5f;
		}
		for (int i = 0; i < boxColliders.Length; i++) {
			boxColliders[i].size = boxColliders[i].size + Vector3.forward * length;
			boxColliders[i].center = boxColliders[i].center + Vector3.forward * length * 0.5f;
		}
		*/
		var headMesh = CreateHeadMesh(length, 2);
		MeshCollider headMeshCollider = headObject.GetComponentInChildren<MeshCollider>();
		headMeshCollider.sharedMesh = headMesh;
		MeshFilter headMeshFilter = headObject.GetComponentInChildren<MeshFilter>();
		headMeshFilter.sharedMesh = headMesh;
	}

	/// <summary>
	/// レールの頭部分の衝突判定用メッシュを作成する。
	/// </summary>
	/// <returns>レールの頭部分の衝突判定用メッシュオブジェクト</returns>
	/// <param name="length">レールの長さ[m]</param>
	/// <param name="divisions">角の丸め部分の分割数</param>
	private static Mesh CreateHeadMesh(float length, int divisions) {
		const float overallHeight = 0.140f;
		const float topWidth = 0.064f;
		const float topHeight = 0.040f;
		const float topUpperRadius = 0.013f;
		const float topLowerBevel = 0.010f;
		const float webWidth = 0.016f;

		const float taper = 1f / 100f;
		float taperLength = length / 2f;

		// 基準断面の中心座標
		Vector2 taperedCenter = new Vector2(0, overallHeight - topHeight / 2);

		// 基準断面の座標を作成
		List<Vector2> typicalSectionPoints = new List<Vector2>();
		typicalSectionPoints.Add(new Vector2(topWidth / 2 - topUpperRadius, overallHeight));
		for (int i = 0; i < divisions; i++) {
			float theta = 0.5f * Mathf.PI * (i + 1) / (divisions + 1);
			float x = topWidth / 2 - topUpperRadius + Mathf.Sin(theta) * topUpperRadius;
			float y = overallHeight - topUpperRadius + Mathf.Cos(theta) * topUpperRadius;
			typicalSectionPoints.Add(new Vector2(x, y));
		}
		typicalSectionPoints.Add(new Vector2(topWidth / 2, overallHeight - topUpperRadius));
		typicalSectionPoints.Add(new Vector2(topWidth / 2, overallHeight - topHeight + topLowerBevel));
		typicalSectionPoints.Add(new Vector2(webWidth / 2, overallHeight - topHeight));
		// 基準断面のもう半分をミラーリングで作成
		for (int i = typicalSectionPoints.Count - 1; i >= 0; i--) {
			typicalSectionPoints.Add(new Vector2(-typicalSectionPoints[i].x, typicalSectionPoints[i].y));
		}

		// 基準断面を元に、テーパー部分の座標を作成
		List<Vector2> taperedSectionPoints = new List<Vector2>();
		Vector2 scale = new Vector2((topWidth - taper * taperLength * 2) / topWidth, (topHeight - taper * taperLength * 2) / topHeight);
		for (int i = 0; i < typicalSectionPoints.Count; i++) {
			Vector2 taperedSectionPoint = typicalSectionPoints[i];
			taperedSectionPoint -= taperedCenter;
			taperedSectionPoint = new Vector2(taperedSectionPoint.x * scale.x, taperedSectionPoint.y * scale.y);
			taperedSectionPoint += taperedCenter;
			taperedSectionPoints.Add(taperedSectionPoint);
		}

		// 基準断面を元に頂点を設定
		List<Vector3> vertices = new List<Vector3>();
		for (int i = 0; i < taperedSectionPoints.Count; i++) {
			vertices.Add(new Vector3(taperedSectionPoints[i].x, taperedSectionPoints[i].y, -taperLength));
		}
		for (int i = 0; i < typicalSectionPoints.Count; i++) {
			vertices.Add(new Vector3(typicalSectionPoints[i].x, typicalSectionPoints[i].y, 0));
		}
		for (int i = 0; i < typicalSectionPoints.Count; i++) {
			vertices.Add(new Vector3(typicalSectionPoints[i].x, typicalSectionPoints[i].y, length));
		}
		for (int i = 0; i < taperedSectionPoints.Count; i++) {
			vertices.Add(new Vector3(taperedSectionPoints[i].x, taperedSectionPoints[i].y, length + taperLength));
		}

		// インデックスの設定
		int count = typicalSectionPoints.Count;
		List<int> indices = new List<int>();
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < count; j++) {
				indices.Add(count * i + j);
				indices.Add(count * i + count + j);
				indices.Add(count * i + (j + 1) % count);
				indices.Add(count * i + (j + 1) % count);
				indices.Add(count * i + count + j);
				indices.Add(count * i + count + (j + 1) % count);
			}
		}

		// Mesh作成
		Mesh mesh = new Mesh();
		mesh.SetVertices(vertices);
		mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();

		return mesh;
	}
}
