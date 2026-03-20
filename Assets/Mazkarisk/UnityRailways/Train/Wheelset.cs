using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Wheelset : MonoBehaviour {

	const int treadDivisions = 60;
	const int flangeDivisions = 60;

	public float wheelDiameter { get; set; } = 0.860f;
	public float wheelThickness { get; set; } = 0.125f;
	public float treadReferencePosition { get; set; } = 0.065f;
	public float backGauge { get; set; } = 0.990f;

	private bool refreshMeshesRequired = true;
	[SerializeField] public PhysicsMaterial physicsMaterial = null;
	[SerializeField] public Material colliderMaterial = null;

	public GameObject getGearCase() {
		return transform.Find("GearCase").gameObject;
	}
	public GameObject getAxelL() {
		return transform.Find("AxelL").gameObject;
	}
	public GameObject getWheelL() {
		return transform.Find("WheelL").gameObject;
	}
	public GameObject getAxelC() {
		return transform.Find("AxelC").gameObject;
	}
	public GameObject getWheelR() {
		return transform.Find("WheelR").gameObject;
	}
	public GameObject getAxelR() {
		return transform.Find("AxelR").gameObject;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		RefreshMeshes();
		refreshMeshesRequired = false;
		
		// 回転部品の角速度の上限を撤廃する
		getAxelL().GetComponent<Rigidbody>().maxAngularVelocity = float.MaxValue;
		getWheelL().GetComponent<Rigidbody>().maxAngularVelocity = float.MaxValue;
		getAxelC().GetComponent<Rigidbody>().maxAngularVelocity = float.MaxValue;
		getWheelR().GetComponent<Rigidbody>().maxAngularVelocity = float.MaxValue;
		getAxelR().GetComponent<Rigidbody>().maxAngularVelocity = float.MaxValue;
	}

	void OnValidate() {
		refreshMeshesRequired = true;
	}

	// Update is called once per frame
	void Update() {
		// 必要に応じてメッシュを初期化する
		if (refreshMeshesRequired) {
			RefreshMeshes();
			refreshMeshesRequired = false;
		}
	}

	void RefreshMeshes() {
		/*
		 *　｜　　　基準(0, 0)　　　　｜
		 *　〇　　　　　｜　　　　　　｜　　
		 *　：＼　　　　↓　　　　　　｜　　
		 *　〇…〇―――★――〇　　　〇
		 *　　　　　　　　　　└〇　　｜
		 *　　┌→+X　　　　　　└〇　〇
		 *　　↓　　　　　　　　　└〇┘
		 *　　+Y
		 */
		float wheelRadius = wheelDiameter / 2f;

		// コライダー形状
		Vector2[] treadCollider1Points = new Vector2[] {
			new Vector2(0.060f, 0.003f) + Vector2.up * wheelRadius,
			new Vector2(-0.060f, -0.003f) + Vector2.up * wheelRadius
		};
		Vector2[] treadCollider2Points = new Vector2[] {
			new Vector2(0.050f, 0.030f) + Vector2.up * wheelRadius,
			new Vector2(0.010f, -0.010f) + Vector2.up * wheelRadius
		};
		Vector2[] flangeColliderPoints = new Vector2[] {
			new Vector2(0.065f, 0.050f) + Vector2.up * wheelRadius,
			new Vector2(0.055f, 0.050f) + Vector2.up * wheelRadius,
			new Vector2(0.020f, -0.020f) + Vector2.up * wheelRadius
		};

		// Meshの生成
		Mesh treadCollider1Mesh = CreateCylinderMesh(treadCollider1Points, treadDivisions, false);
		Mesh treadRenderer1Mesh = CreateCylinderMesh(treadCollider1Points, treadDivisions, true);
		Mesh treadCollider2Mesh = CreateCylinderMesh(treadCollider2Points, treadDivisions, false);
		Mesh treadRenderer2Mesh = CreateCylinderMesh(treadCollider2Points, treadDivisions, true);
		Mesh flangeRenderer1Mesh = CreateCylinderMesh(flangeColliderPoints, flangeDivisions, true);
		Mesh flangeCollider1Mesh = CreateCylinderMesh(flangeColliderPoints, flangeDivisions, false);

		// Meshの設定
		transform.Find("WheelL").Find("TreadCollider1").GetComponent<MeshFilter>().mesh = treadRenderer1Mesh;
		transform.Find("WheelR").Find("TreadCollider1").GetComponent<MeshFilter>().mesh = treadRenderer1Mesh;
		transform.Find("WheelL").Find("TreadCollider1").GetComponent<MeshCollider>().sharedMesh = treadCollider1Mesh;
		transform.Find("WheelR").Find("TreadCollider1").GetComponent<MeshCollider>().sharedMesh = treadCollider1Mesh;

		transform.Find("WheelL").Find("TreadCollider2").GetComponent<MeshFilter>().mesh = treadRenderer2Mesh;
		transform.Find("WheelR").Find("TreadCollider2").GetComponent<MeshFilter>().mesh = treadRenderer2Mesh;
		transform.Find("WheelL").Find("TreadCollider2").GetComponent<MeshCollider>().sharedMesh = treadCollider2Mesh;
		transform.Find("WheelR").Find("TreadCollider2").GetComponent<MeshCollider>().sharedMesh = treadCollider2Mesh;

		transform.Find("WheelL").Find("FlangeCollider1").GetComponent<MeshFilter>().mesh = flangeRenderer1Mesh;
		transform.Find("WheelR").Find("FlangeCollider1").GetComponent<MeshFilter>().mesh = flangeRenderer1Mesh;
		transform.Find("WheelL").Find("FlangeCollider1").GetComponent<MeshCollider>().sharedMesh = flangeCollider1Mesh;
		transform.Find("WheelR").Find("FlangeCollider1").GetComponent<MeshCollider>().sharedMesh = flangeCollider1Mesh;
	}

	/// <summary>
	/// 衝突判定用の円筒形のメッシュを作成する。
	/// </summary>
	/// <remarks>
	/// 車輪のフランジ部とトレッド部とで共通で使用する。
	/// </remarks>
	/// <returns>車輪のフランジ部の衝突判定用メッシュ</returns>
	/// <param name="points">円筒を構成する点群</param>
	/// <param name="divisions">円筒の分割数</param>
	/// <param name="fillEnd">端面の処理。trueの場合のみ端面を作成する。表示用はtrue、コライダ用はfalseを設定する。</param>
	private static Mesh CreateCylinderMesh(Vector2[] points, int divisions, bool fillEnd) {

		// 頂点の設定
		List<Vector3> vertices = new List<Vector3>();
		for (int i = 0; i < points.Length; i++) {
			for (int j = 0; j < divisions; j++) {
				float theta = 2 * Mathf.PI / divisions * (j + 0.5f * (i % 2));
				vertices.Add(new Vector3(points[i].x, Mathf.Cos(theta) * points[i].y, Mathf.Sin(theta) * points[i].y));
			}
		}

		// インデックスの設定
		List<int> indices = new List<int>();
		for (int i = 0; i < points.Length - 1; i++) {
			for (int j = 0; j < divisions; j++) {
				indices.Add(divisions * i + j);
				indices.Add(divisions * i + divisions + (j + i % 2) % divisions);
				indices.Add(divisions * i + (j + 1) % divisions);
				indices.Add(divisions * i + (j + 1) % divisions);
				indices.Add(divisions * i + divisions + (j + i % 2) % divisions);
				indices.Add(divisions * i + divisions + (j + i % 2 + 1) % divisions);
			}
		}

		// fillEndフラグに応じて端面のメッシュも作成する
		if (fillEnd) {
			int indicesOffset = vertices.Count;
			vertices.AddRange(vertices.GetRange(0, divisions));
			vertices.AddRange(vertices.GetRange((points.Length - 1) * divisions, divisions));
			for (int j = 2; j < divisions; j++) {
				indices.Add(indicesOffset + 0);
				indices.Add(indicesOffset + j - 1);
				indices.Add(indicesOffset + j);
				indices.Add(indicesOffset + divisions + 0);
				indices.Add(indicesOffset + divisions + j);
				indices.Add(indicesOffset + divisions + j - 1);
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
