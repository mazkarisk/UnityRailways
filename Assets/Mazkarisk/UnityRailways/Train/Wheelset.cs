using System.Collections.Generic;
using UnityEngine;

public class Wheelset : MonoBehaviour {
	public PhysicsMaterial physicsMaterial { get; set; } = null;
	public Material colliderMaterial { get; set; } = null;

	const int treadDivisions = 60;
	const int flangeDivisions = 60;

	const float colliderExtension = 0.01f; //TODO

	public float wheelDiameter { get; set; } = 0.860f;
	public float wheelThickness { get; set; } = 0.125f;
	public float treadReferencePosition { get; set; } = 0.065f;
	public float backGauge { get; set; } = 0.990f;


	float treadBevel = 0.005f;
	float treadSlope = 1f / 10f; // TODO
	float flangeHeight = 0.030f;
	float flangeRadius = 0.010f;
	float flangeInsideAngle = 82;
	float flangeOutsideAngle = 65;
	float treadFrangeDistance = 0.010f;

	/*
	 *　｜　　　基準(0, 0)　　　　　｜
	 *　①　　　　　｜　　　　　　　｜　　
	 *　：＼　　　　↓　　　　　　　｜　　
	 *　②…③―――★――④　　　　⑩
	 *　　　　　　　　　　　⑤　　　/
	 *　　┌→+X　　　　　　　⑥⑧⑨
	 *　　↓　　　　　　　　　└⑦┘
	 *　　+Y
	 */
	private float wheelRadius;
	private Vector2 point1;
	private Vector2 point2;
	private Vector2 point3;
	private Vector2 point4;
	private Vector2 point5;
	private Vector2 point6;
	private Vector2 point7;
	private Vector2 point8;
	private Vector2 point9;
	private Vector2 point10;
	Vector2[][] treadColliderPoints = null;
	Vector2[][] flangeColliderPoints = null;

	private GameObject goWheelL = null;
	private GameObject goWheelR = null;
	private GameObject goAxelL = null;
	private GameObject goAxelC = null;
	private GameObject goAxelR = null;

	public GameObject getWheelL() {
		if (goWheelL == null) {
			Initialize();
		}
		return goWheelL;
	}
	public GameObject getWheelR() {
		if (goWheelR == null) {
			Initialize();
		}
		return goWheelR;
	}
	public GameObject getAxelL() {
		if (goAxelL == null) {
			Initialize();
		}
		return goAxelL;
	}
	public GameObject getAxelC() {
		if (goAxelC == null) {
			Initialize();
		}
		return goAxelC;
	}
	public GameObject getAxelR() {
		if (goAxelR == null) {
			Initialize();
		}
		return goAxelR;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		Initialize();
	}

	// Update is called once per frame
	void Update() {

	}

	void Initialize() {
		CalculatePoints();

		//Mesh[] treadColliderMesh = new Mesh[] { CreateCylinderMesh(treadColliderPoints[0], treadDivisions, false), CreateCylinderMesh(treadColliderPoints[1], treadDivisions, false), CreateCylinderMesh(treadColliderPoints[2], treadDivisions, false) };
		Mesh[] treadColliderMesh = new Mesh[] { CreateCylinderMesh(treadColliderPoints[0], treadDivisions, false) };
		Mesh[] flangeColliderMesh = new Mesh[] { CreateCylinderMesh(flangeColliderPoints[0], flangeDivisions, false) };
		//Mesh[] treadRendererMesh = new Mesh[] { CreateCylinderMesh(treadColliderPoints[0], treadDivisions, true), CreateCylinderMesh(treadColliderPoints[1], treadDivisions, true), CreateCylinderMesh(treadColliderPoints[2], treadDivisions, true) };
		Mesh[] treadRendererMesh = new Mesh[] { CreateCylinderMesh(treadColliderPoints[0], treadDivisions, true) };
		Mesh[] flangeRendererMesh = new Mesh[] { CreateCylinderMesh(flangeColliderPoints[0], flangeDivisions, true) };

		if (goWheelL == null) {
			goWheelL = CreateWheelObject("WheelL", transform, treadColliderMesh, flangeColliderMesh, treadRendererMesh, flangeRendererMesh, physicsMaterial, colliderMaterial);
			goWheelL.transform.localPosition = new Vector3(-(backGauge / 2f + treadReferencePosition), 0f, 0f);
			goWheelL.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		}

		if (goWheelR == null) {
			goWheelR = CreateWheelObject("WheelR", transform, treadColliderMesh, flangeColliderMesh, treadRendererMesh, flangeRendererMesh, physicsMaterial, colliderMaterial);
			goWheelR.transform.localPosition = new Vector3(backGauge / 2f + treadReferencePosition, 0f, 0f);
			goWheelR.transform.localEulerAngles = new Vector3(360f / (treadDivisions * 2f), 180f, 0f);
		}

		if (goAxelL == null) {
			float length = 0.4f;
			goAxelL = CreateAxelObject("AxelL", transform, Vector3.left * (backGauge * 0.5f + wheelThickness + length * 0.5f), Quaternion.identity, new Vector3(length, 0.1f, 0.1f), physicsMaterial, colliderMaterial);
			JointWheelAndAxel(goWheelL, goAxelL, Vector3.left * (wheelThickness - treadReferencePosition), Vector3.right * 0.5f);
		}
		if (goAxelC == null) {
			goAxelC = CreateAxelObject("AxelC", transform, Vector3.zero, Quaternion.identity, new Vector3(backGauge, 0.1f, 0.1f), physicsMaterial, colliderMaterial);
			JointWheelAndAxel(goWheelL, goAxelC, Vector3.right * treadReferencePosition, Vector3.left * 0.5f);
			JointWheelAndAxel(goWheelR, goAxelC, Vector3.right * treadReferencePosition, Vector3.right * 0.5f);
		}
		if (goAxelR == null) {
			float length = 0.4f;
			goAxelR = CreateAxelObject("AxelR", transform, Vector3.right * (backGauge * 0.5f + wheelThickness + length * 0.5f), Quaternion.identity, new Vector3(length, 0.1f, 0.1f), physicsMaterial, colliderMaterial);
			JointWheelAndAxel(goWheelR, goAxelR, Vector3.left * (wheelThickness - treadReferencePosition), Vector3.left * 0.5f);
		}
	}

	private void CalculatePoints() {
		wheelRadius = wheelDiameter / 2f;
		float flangeInsideSlope = Mathf.Tan(flangeInsideAngle * Mathf.Deg2Rad);
		float flangeOutsideSlope = Mathf.Tan(flangeOutsideAngle * Mathf.Deg2Rad);

		// 踏面外側
		point1.x = treadReferencePosition - wheelThickness;
		point2.x = treadReferencePosition - wheelThickness;
		point3.x = treadReferencePosition - wheelThickness + treadBevel;
		point3.y = point3.x * treadSlope;
		point2.y = point2.x * treadSlope;
		point1.y = point3.y - treadBevel;

		// フランジ部
		point10 = new Vector2(treadReferencePosition, 0);
		point9.y = flangeHeight - flangeRadius * Mathf.Cos(flangeInsideAngle * Mathf.Deg2Rad);
		point9.x = point10.x - point9.y / flangeInsideSlope;
		point8.y = flangeHeight - flangeRadius;
		point8.x = point9.x - flangeRadius * Mathf.Sin(flangeInsideAngle * Mathf.Deg2Rad);
		point7 = new Vector2(point8.x, flangeHeight);
		point6.y = point8.y + flangeRadius * Mathf.Sin(flangeOutsideAngle * Mathf.Deg2Rad);
		point6.x = point8.x - flangeRadius * Mathf.Cos(flangeOutsideAngle * Mathf.Deg2Rad);
		point5.y = treadFrangeDistance;
		point5.x = point6.x - (point6.y - point5.y) / flangeOutsideSlope;

		// 踏面-フランジ外面
		point4.x = (point6.x * flangeOutsideSlope - point6.y) / (flangeOutsideSlope - treadSlope);
		point4.y = point4.x * treadSlope;

		// (参考)導出過程
		// 踏面とフランジ外面の直線は以下の式で表せる。
		// 　踏面の式 : y = treadSlope * x ※切片は0。
		// 　フランジ外面の式：y = flangeOutsideSlope * x - point6.x * flangeOutsideSlope + point6.y
		// 2式から交点を求める。
		// 　treadSlope * x = flangeOutsideSlope * x - point6.x * flangeOutsideSlope + point6.y
		// 　flangeOutsideSlope * x - treadSlope * x = point6.x * flangeOutsideSlope - point6.y
		// 　(flangeOutsideSlope - treadSlope) * x = point6.x * flangeOutsideSlope - point6.y
		// 　x = (point6.x * flangeOutsideSlope - point6.y) / (flangeOutsideSlope - treadSlope)

		// コライダー形状
		float tan1 = Mathf.Tan(flangeInsideAngle * Mathf.Deg2Rad);
		float tan2 = Mathf.Tan((180 - flangeInsideAngle) / 2 * Mathf.Deg2Rad);
		float tan3 = Mathf.Tan((180 - flangeOutsideAngle) / 2 * Mathf.Deg2Rad);
		treadColliderPoints = new Vector2[][] {
			new Vector2[] {
				point4+(point4 - point2).normalized  * colliderExtension + Vector2.up * wheelRadius,
				point2 + Vector2.up * wheelRadius
			}/*,
			new Vector2[] {
				((Vector2.zero + point4) / 2 + point4 + point5) / 3 + Vector2.up * wheelRadius,
				(Vector2.zero + point4) / 2 + Vector2.up * wheelRadius
			},
			new Vector2[] {
				point5 + Vector2.up * wheelRadius,
				((Vector2.zero + point4) / 2 + point4 + point5) / 3 + Vector2.up * wheelRadius
			}*/
		};
		flangeColliderPoints = new Vector2[][] {
			/*
			new Vector2[] {
				point10 + Vector2.up * wheelRadius,
				new Vector2(point10.x - flangeHeight / tan1, wheelRadius + flangeHeight),
				new Vector2(point10.x - flangeHeight / tan1 - flangeRadius * (1 / tan2 + 1 / tan3), wheelRadius + flangeHeight),
				point4 + (point4 - point6).normalized * colliderExtension + Vector2.up * wheelRadius
			}
			*/
			new Vector2[] {
				point10 + Vector2.up * (flangeHeight + wheelRadius),
				new Vector2(point10.x - flangeHeight / tan1 - flangeRadius * (1 / tan2 + 1 / tan3), wheelRadius + flangeHeight),
				point4 + (point4 - point6).normalized * colliderExtension + Vector2.up * wheelRadius
			}
		};
	}

	// TODO ドキュメント整備
	private static GameObject CreateWheelObject(string name, Transform parent, Mesh[] treadColliderMesh, Mesh[] flangeColliderMesh, Mesh[] treadRendererMesh, Mesh[] flangeRendererMesh, PhysicsMaterial physicsMaterial, Material colliderMaterial) {

		GameObject wheelObject = new GameObject(name);
		wheelObject.transform.parent = parent;
		wheelObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

		for (int i = 0; i < treadRendererMesh.Length; i++) {
			GameObject treadObject = new GameObject(name + "_TreadCollider" + (i + 1));
			treadObject.transform.parent = wheelObject.transform;
			treadObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			MeshRenderer treadRenderer = treadObject.AddComponent<MeshRenderer>();
			treadRenderer.sharedMaterial = colliderMaterial;
			MeshFilter treadFilter = treadObject.AddComponent<MeshFilter>();
			treadFilter.mesh = treadRendererMesh[i];
			MeshCollider treadCollider = treadObject.AddComponent<MeshCollider>();
			treadCollider.convex = true;
			treadCollider.sharedMesh = treadColliderMesh[i];
			treadCollider.material = physicsMaterial;
		}

		for (int i = 0; i < flangeRendererMesh.Length; i++) {
			GameObject flangeObject = new GameObject(name + "_FlangeCollider" + (i + 1));
			flangeObject.transform.parent = wheelObject.transform;
			flangeObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			MeshRenderer flangeRenderer = flangeObject.AddComponent<MeshRenderer>();
			flangeRenderer.sharedMaterial = colliderMaterial;
			MeshFilter flangeFilter = flangeObject.AddComponent<MeshFilter>();
			flangeFilter.mesh = flangeRendererMesh[i];
			MeshCollider flangeCollider = flangeObject.AddComponent<MeshCollider>();
			flangeCollider.convex = true;
			flangeCollider.sharedMesh = flangeColliderMesh[i];
			flangeCollider.material = physicsMaterial;
		}

		Rigidbody rigidbody = wheelObject.AddComponent<Rigidbody>();
		rigidbody.mass = 200; // TODO
		rigidbody.linearDamping = 0f;
		rigidbody.angularDamping = 0f;  // TODO いる？

		return wheelObject;
	}

	// TODO ドキュメント整備
	private static GameObject CreateAxelObject(string name, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, PhysicsMaterial physicsMaterial, Material colliderMaterial) {
		GameObject axelObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		axelObject.name = name;
		axelObject.transform.parent = parent;
		axelObject.transform.localPosition = localPosition;
		axelObject.transform.localRotation = localRotation;
		axelObject.transform.localScale = localScale;
		//axelObject.GetComponent<MeshRenderer>().enabled = false;
		axelObject.GetComponent<BoxCollider>().material = physicsMaterial;

		MeshRenderer axelRenderer = axelObject.GetComponent<MeshRenderer>();
		axelRenderer.sharedMaterial = colliderMaterial;

		Rigidbody axelRigidbody = axelObject.AddComponent<Rigidbody>();
		axelRigidbody.mass = 200;

		return axelObject;
	}

	// TODO ドキュメント整備
	ConfigurableJoint JointWheelAndAxel(GameObject wheelObject, GameObject axelObject, Vector3 anchor, Vector3 connectedAnchor) {
		SoftJointLimitSpring spring = new SoftJointLimitSpring();
		spring.spring = 1000000f;
		spring.damper = 1000000f;
		SoftJointLimit limit = new SoftJointLimit();
		limit.limit = 0.000001f;

		ConfigurableJoint joint = wheelObject.AddComponent<ConfigurableJoint>();
		joint.autoConfigureConnectedAnchor = false;
		joint.enableCollision = false;
		joint.connectedBody = axelObject.GetComponent<Rigidbody>();
		joint.anchor = anchor;
		joint.connectedAnchor = connectedAnchor;
		joint.xMotion = ConfigurableJointMotion.Limited;   // TODO
		joint.yMotion = ConfigurableJointMotion.Locked;   // TODO
		joint.zMotion = ConfigurableJointMotion.Locked;   // TODO
		joint.angularXMotion = ConfigurableJointMotion.Locked;// TODO
		joint.angularYMotion = ConfigurableJointMotion.Locked;    // TODO
		joint.angularZMotion = ConfigurableJointMotion.Locked;    // TODO
		joint.linearLimit = limit;
		joint.highAngularXLimit = limit;
		joint.lowAngularXLimit = limit;
		joint.angularYLimit = limit;
		joint.angularZLimit = limit;
		joint.linearLimitSpring = spring;
		joint.angularXLimitSpring = spring;
		joint.angularYZLimitSpring = spring;

		return joint;
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
