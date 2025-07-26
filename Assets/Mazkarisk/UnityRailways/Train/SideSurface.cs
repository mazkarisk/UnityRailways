using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteAlways]
public class SideSurface : MonoBehaviour {

	/// <summary>
	/// 板厚(m単位)
	/// </summary>
	[SerializeField, Tooltip("板厚(m単位)"), Range(0.001f, 0.1f)]
	float plateThickness = 0.004f;

	/// <summary>
	/// 端面(板同士の接合部)のR加工半径(m単位)
	/// </summary>
	[SerializeField, Tooltip("端面(板同士の接合部)のR加工半径(m単位)"), Range(0.0001f, 0.1f)]
	float plateEndRound = 0.0005f;
	/// <summary>
	/// 側面(板同士の接合部以外)のR加工半径(m単位)
	/// </summary>
	[SerializeField, Tooltip("側面(板同士の接合部以外)のR加工半径(m単位)"), Range(0.0001f, 0.1f)]
	float plateSideRound = 0.001f;

	/// <summary>
	/// 全体の上下方向高さ(m単位)
	/// </summary>
	[SerializeField, Tooltip("全体の上下方向高さ(m単位)")]
	float overallHeight = 2.9f;
	/// <summary>
	/// 全体の前後方向長さ(m単位)
	/// </summary>
	[SerializeField, Tooltip("全体の前後方向長さ(m単位)")]
	float overallLength = 2f;

	/// <summary>
	/// 後方の開口部の前後方向の長さ(m単位)
	/// </summary>
	[SerializeField, Tooltip("後方の開口部の前後方向の長さ(m単位)")]
	float rearApertureLength = 0.75f;
	/// <summary>
	/// 後方の開口部の下部から下の高さ(m単位)
	/// </summary>
	[SerializeField, Tooltip("後方の開口部の下部から下の高さ(m単位)")]
	float rearApertureBottomHeight = 0.75f;
	/// <summary>
	/// 後方の開口部の上部から上の高さ(m単位)
	/// </summary>
	[SerializeField, Tooltip("後方の開口部の上部から上の高さ(m単位)")]
	float rearApertureTopHeight = 0.75f;

	/// <summary>
	/// 前方の開口部の前後方向の長さ(m単位)
	/// </summary>
	[SerializeField, Tooltip("前方の開口部の前後方向の長さ(m単位)")]
	float frontApertureLength = 0.75f;
	/// <summary>
	/// 前方の開口部の下部から下の高さ(m単位)
	/// </summary>
	[SerializeField, Tooltip("前方の開口部の下部から下の高さ(m単位)")]
	float frontApertureBottomHeight = 0.75f;
	/// <summary>
	/// 前方の開口部の上部から上の高さ(m単位)
	/// </summary>
	[SerializeField, Tooltip("前方の開口部の上部から上の高さ(m単位)")]
	float frontApertureTopHeight = 0.75f;

	private bool refreshMeshesRequired = true;
	private Mesh rearBottomMesh = null;
	private Mesh rearTopMesh = null;
	private Mesh midMesh = null;
	private Mesh frontBottomMesh = null;
	private Mesh frontTopMesh = null;

	void Start() {
		RefreshMeshes();
		refreshMeshesRequired = false;
	}

	void OnValidate() {
		refreshMeshesRequired = true;
	}

	void Update() {
		// 必要に応じてメッシュを初期化する
		if (refreshMeshesRequired) {
			RefreshMeshes();
			refreshMeshesRequired = false;
		}
	}

	private void RefreshMeshes() {
		rearBottomMesh = NegativeZEndMesh(plateThickness, rearApertureBottomHeight, rearApertureLength, plateEndRound, plateSideRound);
		transform.Find("RearBottomPart").GetComponent<MeshFilter>().mesh = rearBottomMesh;
		transform.Find("RearBottomPart").SetLocalPositionAndRotation(new Vector3(0, 0, rearApertureLength), Quaternion.identity);

		rearTopMesh = NegativeZEndMesh(plateThickness, rearApertureTopHeight, rearApertureLength, plateEndRound, plateSideRound);
		transform.Find("RearTopPart").GetComponent<MeshFilter>().mesh = rearTopMesh;
		transform.Find("RearTopPart").SetLocalPositionAndRotation(new Vector3(0, overallHeight - rearApertureTopHeight, rearApertureLength), Quaternion.identity);

		frontBottomMesh = PositiveZEndMesh(plateThickness, frontApertureBottomHeight, frontApertureLength, plateEndRound, plateSideRound);
		transform.Find("FrontBottomPart").GetComponent<MeshFilter>().mesh = frontBottomMesh;
		transform.Find("FrontBottomPart").SetLocalPositionAndRotation(new Vector3(0, 0, overallLength - frontApertureLength), Quaternion.identity);

		frontTopMesh = PositiveZEndMesh(plateThickness, frontApertureTopHeight, frontApertureLength, plateEndRound, plateSideRound);
		transform.Find("FrontTopPart").GetComponent<MeshFilter>().mesh = frontTopMesh;
		transform.Find("FrontTopPart").SetLocalPositionAndRotation(new Vector3(0, overallHeight - rearApertureTopHeight, overallLength - frontApertureLength), Quaternion.identity);

		midMesh = PillarMesh();
		transform.Find("MidPart").GetComponent<MeshFilter>().mesh = midMesh;
		transform.Find("MidPart").SetLocalPositionAndRotation(new Vector3(0, rearApertureBottomHeight, rearApertureLength), Quaternion.identity);
	}

	/// <summary>
	/// 板部材の-Z方向の端部のメッシュの作成
	/// </summary>
	/// <param name="thickness">板厚(m単位)</param>
	/// <param name="height">高さ(m単位)</param>
	/// <param name="length">長さ(m単位)</param>
	/// <param name="endRound">端面のR加工幅(m単位)</param>
	/// <param name="sideRound">端面以外のR加工幅(m単位)</param>
	/// <returns></returns>
	public static Mesh NegativeZEndMesh(float thickness, float height, float length, float endRound, float sideRound) {
		Mesh mesh = new Mesh();

		float angle = Mathf.PI / 4;
		float x0 = -thickness / 2;
		float x1 = -(thickness / 2 - sideRound);
		float x2 = thickness / 2 - sideRound;
		float x3 = thickness / 2;
		float y0 = 0;
		float y1 = sideRound;
		float y2 = height - sideRound;
		float y3 = height;
		float z0 = -length;
		float z1 = -(length - endRound);
		float z2 = 0;
		float z3 = thickness / 2 * Mathf.Sin(angle);

		// 頂点バッファの作成
		List<Vector3> vertices = ProcedualMesh.Get3x3GridVertices(x0, x1, x2, x3, y0, y1, y2, y3, z0, z1, z2, z3);

		// 法線バッファの作成
		List<Vector3> normals = ProcedualMesh.Get3x3GridNormals(vertices);

		// UVバッファの作成
		List<Vector2> uvs = ProcedualMesh.Get3x3GridUVs(vertices, Vector3.zero);

		// インデックスバッファの作成
		List<int> indices = ProcedualMesh.Get3x3GridIndices();

		// 頂点の位置の調整
		for (int i = 0; i < vertices.Count; i++) {
			Vector3 vertex = vertices[i];
			if (vertex.z == z3) {
				vertices[i] = new Vector3(vertex.x * Mathf.Cos(angle), vertex.y, vertex.z);
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

	/// <summary>
	/// 板部材の+Z方向の端部のメッシュの作成
	/// </summary>
	/// <param name="thickness">板厚(m単位)</param>
	/// <param name="height">高さ(m単位)</param>
	/// <param name="length">長さ(m単位)</param>
	/// <param name="endRound">端面のR加工幅(m単位)</param>
	/// <param name="sideRound">端面以外のR加工幅(m単位)</param>
	/// <returns></returns>
	public static Mesh PositiveZEndMesh(float thickness, float height, float length, float endRound, float sideRound) {
		Mesh mesh = new Mesh();

		float angle = Mathf.PI / 4;
		float x0 = -thickness / 2;
		float x1 = -(thickness / 2 - sideRound);
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
		List<Vector3> vertices = ProcedualMesh.Get3x3GridVertices(x0, x1, x2, x3, y0, y1, y2, y3, z0, z1, z2, z3);

		// 法線バッファの作成
		List<Vector3> normals = ProcedualMesh.Get3x3GridNormals(vertices);

		// UVバッファの作成
		List<Vector2> uvs = ProcedualMesh.Get3x3GridUVs(vertices, Vector3.zero);

		// インデックスバッファの作成
		List<int> indices = ProcedualMesh.Get3x3GridIndices();

		// 頂点の位置の調整
		for (int i = 0; i < vertices.Count; i++) {
			Vector3 vertex = vertices[i];
			if (vertex.z == z0) {
				vertices[i] = new Vector3(vertex.x * Mathf.Cos(angle), vertex.y, vertex.z);
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

	/// <summary>
	/// 板部材の柱部のメッシュの作成
	/// </summary>
	/// <returns></returns>
	public Mesh PillarMesh() {
		Mesh mesh = new Mesh();

		float angle = Mathf.PI / 4;
		float x0 = -plateThickness / 2;
		float x1 = -(plateThickness / 2 - plateSideRound);
		float x2 = plateThickness / 2 - plateSideRound;
		float x3 = plateThickness / 2;
		float y0 = -plateThickness / 2 * Mathf.Sin(angle);
		float y1 = 0;
		float y2 = overallHeight - rearApertureBottomHeight - rearApertureTopHeight;
		float y3 = overallHeight - rearApertureBottomHeight - rearApertureTopHeight + plateThickness / 2 * Mathf.Sin(angle);
		float z0 = 0;
		float z1 = plateSideRound;
		float z2 = overallLength - rearApertureLength - frontApertureLength - plateSideRound;
		float z3 = overallLength - rearApertureLength - frontApertureLength;

		// 頂点バッファの作成
		List<Vector3> vertices = ProcedualMesh.Get3x3GridVertices(x0, x1, x2, x3, y0, y1, y2, y3, z0, z1, z2, z3);

		// 法線バッファの作成
		List<Vector3> normals = ProcedualMesh.Get3x3GridNormals(vertices);

		// UVバッファの作成
		List<Vector2> uvs = ProcedualMesh.Get3x3GridUVs(vertices, Vector3.zero);

		// インデックスバッファの作成
		List<int> indices = ProcedualMesh.Get3x3GridIndices();

		// 頂点の位置の調整
		for (int i = 0; i < vertices.Count; i++) {
			Vector3 vertex = vertices[i];
			if (vertex.y == y0 || vertex.y == y3) {
				vertices[i] = new Vector3(vertex.x * Mathf.Cos(angle), vertex.y, vertex.z);
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
