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
	/// 後方の開口部の角の丸み(m単位)
	/// </summary>
	[SerializeField, Tooltip("後方の開口部の角の丸み(m単位)"), Range(0.0001f, 1f)]
	float rearApertureRound = 0.1f;

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
	/// <summary>
	/// 前方の開口部の角の丸み(m単位)
	/// </summary>
	[SerializeField, Tooltip("前方の開口部の角の丸み(m単位)"), Range(0.0001f, 1f)]
	float frontApertureRound = 0.1f;

	private bool refreshMeshesRequired = true;
	private Mesh rearTopMesh = null;
	private Mesh rearBottomMesh = null;
	private Mesh midTopMesh = null;
	private Mesh midMesh = null;
	private Mesh midBottomMesh = null;
	private Mesh frontTopMesh = null;
	private Mesh frontBottomMesh = null;

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
		// 後方のメッシュ
		rearBottomMesh = NegativeZEndMesh(plateThickness, rearApertureBottomHeight, rearApertureLength - rearApertureRound, plateEndRound, plateSideRound);
		transform.Find("RearBottomPart").GetComponent<MeshFilter>().mesh = rearBottomMesh;
		transform.Find("RearBottomPart").localPosition = new Vector3(0, 0, rearApertureLength - rearApertureRound);

		rearTopMesh = NegativeZEndMesh(plateThickness, rearApertureTopHeight, rearApertureLength - rearApertureRound, plateEndRound, plateSideRound);
		transform.Find("RearTopPart").GetComponent<MeshFilter>().mesh = rearTopMesh;
		transform.Find("RearTopPart").localPosition = new Vector3(0, overallHeight - rearApertureTopHeight, rearApertureLength - rearApertureRound);

		// 中央部のメッシュ
		midTopMesh = ProcedualMesh.InversedTMesh(
			plateThickness,
			overallLength - rearApertureLength - frontApertureLength + rearApertureRound + frontApertureRound,
			Mathf.Max(rearApertureTopHeight + rearApertureRound, frontApertureTopHeight + frontApertureRound),
			rearApertureRound,
			rearApertureRound + Mathf.Max(rearApertureTopHeight - frontApertureTopHeight, 0),
			frontApertureRound,
			frontApertureRound + Mathf.Max(frontApertureTopHeight - rearApertureTopHeight, 0)
			); ;
		transform.Find("MidTopPart").GetComponent<MeshFilter>().mesh = midTopMesh;
		transform.Find("MidTopPart").localPosition = new Vector3(0, overallHeight, rearApertureLength + rearApertureRound + (overallLength - rearApertureLength - rearApertureRound - frontApertureLength - frontApertureRound) / 2);
		transform.Find("MidTopPart").localRotation = Quaternion.Euler(180, 90, 0);

		midMesh = PillarMesh();
		transform.Find("MidPart").GetComponent<MeshFilter>().mesh = midMesh;
		transform.Find("MidPart").localPosition = new Vector3(0, 0, rearApertureLength);

		midBottomMesh = ProcedualMesh.InversedTMesh(
			plateThickness,
			overallLength - rearApertureLength - frontApertureLength + rearApertureRound + frontApertureRound,
			Mathf.Max(rearApertureBottomHeight + rearApertureRound, frontApertureBottomHeight + frontApertureRound),
			rearApertureRound,
			rearApertureRound + Mathf.Max(rearApertureBottomHeight - frontApertureBottomHeight, 0),
			frontApertureRound,
			frontApertureRound + Mathf.Max(frontApertureBottomHeight - rearApertureBottomHeight, 0)
			);
		transform.Find("MidBottomPart").GetComponent<MeshFilter>().mesh = midBottomMesh;
		transform.Find("MidBottomPart").localPosition = new Vector3(0, 0, rearApertureLength + rearApertureRound + (overallLength - rearApertureLength - rearApertureRound - frontApertureLength - frontApertureRound) / 2);
		transform.Find("MidBottomPart").localRotation = Quaternion.Euler(0, 90, 0);

		// 前方のメッシュ
		frontBottomMesh = PositiveZEndMesh(plateThickness, frontApertureBottomHeight, frontApertureLength - frontApertureRound, plateEndRound, plateSideRound);
		transform.Find("FrontBottomPart").GetComponent<MeshFilter>().mesh = frontBottomMesh;
		transform.Find("FrontBottomPart").localPosition = new Vector3(0, 0, overallLength - frontApertureLength + frontApertureRound);

		frontTopMesh = PositiveZEndMesh(plateThickness, frontApertureTopHeight, frontApertureLength - frontApertureRound, plateEndRound, plateSideRound); ;
		transform.Find("FrontTopPart").GetComponent<MeshFilter>().mesh = frontTopMesh;
		transform.Find("FrontTopPart").localPosition = new Vector3(0, overallHeight - frontApertureTopHeight, overallLength - frontApertureLength + frontApertureRound);
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
		float angle = Mathf.PI / 6;
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
		Mesh mesh = new Mesh();
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
		float angle = Mathf.PI / 6;
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
		Mesh mesh = new Mesh();
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
	private Mesh PillarMesh() {
		float angle = Mathf.PI / 6;
		float pillarLength = overallLength - rearApertureLength - frontApertureLength;

		float x0 = -plateThickness / 2;
		float x1 = -(plateThickness / 2 - plateSideRound);
		float x2 = plateThickness / 2 - plateSideRound;
		float x3 = plateThickness / 2;
		float y0 = -plateThickness / 2 * Mathf.Sin(angle);
		float y1 = 0;
		float y2 = overallHeight - rearApertureBottomHeight - rearApertureTopHeight - rearApertureRound * 2;
		float y3 = overallHeight - rearApertureBottomHeight - rearApertureTopHeight - rearApertureRound * 2 + plateThickness / 2 * Mathf.Sin(angle);
		float z0 = 0;
		float z1 = plateSideRound;
		float z2 = overallLength - rearApertureLength - frontApertureLength - plateSideRound;
		float z3 = overallLength - rearApertureLength - frontApertureLength;

		// float y10 = rearApertureBottomHeight + rearApertureRound;
		// float y13 = frontApertureBottomHeight + frontApertureRound;
		// float y20 = overallHeight - rearApertureTopHeight - rearApertureRound;
		// float y23 = overallHeight - frontApertureTopHeight - frontApertureRound;
		float y10 = Mathf.Max(rearApertureBottomHeight + rearApertureRound, frontApertureBottomHeight + frontApertureRound);
		float y13 = y10;
		float y20 = Mathf.Min(overallHeight - rearApertureTopHeight - rearApertureRound, overallHeight - frontApertureTopHeight - frontApertureRound);
		float y23 = y20;
		float y00 = y10 - plateThickness / 2 * Mathf.Sin(angle);
		float y03 = y13 - plateThickness / 2 * Mathf.Sin(angle);
		float y30 = y20 + plateThickness / 2 * Mathf.Sin(angle);
		float y33 = y23 + plateThickness / 2 * Mathf.Sin(angle);

		float y01 = Mathf.Lerp(y00, y03, z1 / pillarLength);
		float y02 = Mathf.Lerp(y00, y03, z2 / pillarLength);
		float y11 = Mathf.Lerp(y10, y13, z1 / pillarLength);
		float y12 = Mathf.Lerp(y10, y13, z2 / pillarLength);
		float y21 = Mathf.Lerp(y20, y23, z1 / pillarLength);
		float y22 = Mathf.Lerp(y20, y23, z2 / pillarLength);
		float y31 = Mathf.Lerp(y30, y33, z1 / pillarLength);
		float y32 = Mathf.Lerp(y30, y33, z2 / pillarLength);

		// 頂点バッファの作成
		List<Vector3> vertices = new List<Vector3> {
			// -X面
			new Vector3(x0, y03, z3), new Vector3(x0, y02, z2), new Vector3(x0, y01, z1), new Vector3(x0, y00, z0),
			new Vector3(x0, y13, z3), new Vector3(x0, y12, z2), new Vector3(x0, y11, z1), new Vector3(x0, y10, z0),
			new Vector3(x0, y23, z3), new Vector3(x0, y22, z2), new Vector3(x0, y21, z1), new Vector3(x0, y20, z0),
			new Vector3(x0, y33, z3), new Vector3(x0, y32, z2), new Vector3(x0, y31, z1), new Vector3(x0, y30, z0),

			// +X面
			new Vector3(x3, y00, z0), new Vector3(x3, y01, z1), new Vector3(x3, y02, z2), new Vector3(x3, y03, z3),
			new Vector3(x3, y10, z0), new Vector3(x3, y11, z1), new Vector3(x3, y12, z2), new Vector3(x3, y13, z3),
			new Vector3(x3, y20, z0), new Vector3(x3, y21, z1), new Vector3(x3, y22, z2), new Vector3(x3, y23, z3),
			new Vector3(x3, y30, z0), new Vector3(x3, y31, z1), new Vector3(x3, y32, z2), new Vector3(x3, y33, z3),

			// -Y面
			new Vector3(x3, y00, z0), new Vector3(x2, y00, z0), new Vector3(x1, y00, z0), new Vector3(x0, y00, z0),
			new Vector3(x3, y01, z1), new Vector3(x2, y01, z1), new Vector3(x1, y01, z1), new Vector3(x0, y01, z1),
			new Vector3(x3, y02, z2), new Vector3(x2, y02, z2), new Vector3(x1, y02, z2), new Vector3(x0, y02, z2),
			new Vector3(x3, y03, z3), new Vector3(x2, y03, z3), new Vector3(x1, y03, z3), new Vector3(x0, y03, z3),

			// +Y面
			new Vector3(x0, y30, z0), new Vector3(x1, y30, z0), new Vector3(x2, y30, z0), new Vector3(x3, y30, z0),
			new Vector3(x0, y31, z1), new Vector3(x1, y31, z1), new Vector3(x2, y31, z1), new Vector3(x3, y31, z1),
			new Vector3(x0, y32, z2), new Vector3(x1, y32, z2), new Vector3(x2, y32, z2), new Vector3(x3, y32, z2),
			new Vector3(x0, y33, z3), new Vector3(x1, y33, z3), new Vector3(x2, y33, z3), new Vector3(x3, y33, z3),

			// -Z面
			new Vector3(x0, y00, z0), new Vector3(x1, y00, z0), new Vector3(x2, y00, z0), new Vector3(x3, y00, z0),
			new Vector3(x0, y10, z0), new Vector3(x1, y10, z0), new Vector3(x2, y10, z0), new Vector3(x3, y10, z0),
			new Vector3(x0, y20, z0), new Vector3(x1, y20, z0), new Vector3(x2, y20, z0), new Vector3(x3, y20, z0),
			new Vector3(x0, y30, z0), new Vector3(x1, y30, z0), new Vector3(x2, y30, z0), new Vector3(x3, y30, z0),

			// +Z面
			new Vector3(x3, y03, z3), new Vector3(x2, y03, z3), new Vector3(x1, y03, z3), new Vector3(x0, y03, z3),
			new Vector3(x3, y13, z3), new Vector3(x2, y13, z3), new Vector3(x1, y13, z3), new Vector3(x0, y13, z3),
			new Vector3(x3, y23, z3), new Vector3(x2, y23, z3), new Vector3(x1, y23, z3), new Vector3(x0, y23, z3),
			new Vector3(x3, y33, z3), new Vector3(x2, y33, z3), new Vector3(x1, y33, z3), new Vector3(x0, y33, z3)
		};

		// 法線バッファの作成
		List<Vector3> normals = ProcedualMesh.Get3x3GridNormals(vertices);

		// UVバッファの作成
		List<Vector2> uvs = ProcedualMesh.Get3x3GridUVs(vertices, Vector3.zero);

		// インデックスバッファの作成
		List<int> indices = ProcedualMesh.Get3x3GridIndices();

		// 頂点の位置の調整
		int[] modifyTarget = new int[] {
			0, 1, 2, 3,
			12, 13, 14, 15,
			16, 17, 18, 19,
			28, 29, 30, 31,

			32, 33, 34, 35,
			36, 37, 38, 39,
			40, 41, 42, 43,
			44, 45, 46, 47,

			48, 49, 50, 51,
			52, 53, 54, 55,
			56, 57, 58, 59,
			60, 61, 62, 63,

			64, 65, 66, 67,
			76, 77, 78, 79,
			80, 81, 82, 83,
			92, 93, 94, 95
		};
		for (int i = 0; i < modifyTarget.Length; i++) {
			Vector3 vertex = vertices[modifyTarget[i]];
			vertices[modifyTarget[i]] = new Vector3(vertex.x * Mathf.Cos(angle), vertex.y, vertex.z);
		}

		// メッシュの作成
		Mesh mesh = new Mesh();
		mesh.SetVertices(vertices);
		mesh.SetNormals(normals);
		mesh.SetUVs(0, uvs);
		mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		mesh.RecalculateBounds();
		mesh.RecalculateTangents();

		return mesh;
	}
}
