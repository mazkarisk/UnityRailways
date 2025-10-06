using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Wheelset : MonoBehaviour {

	const int treadDivisions = 80;
	const int flangeDivisions = 80;

	public float wheelDiameter { get; set; } = 0.860f;
	public float wheelThickness { get; set; } = 0.125f;
	public float treadReferencePosition { get; set; } = 0.065f;
	public float backGauge { get; set; } = 0.990f;

	const float treadBevel = 0.005f;
	const float treadSlope = 1f / 20f;
	const float flangeHeight = 0.030f;
	const float flangeRadius = 0.010f;
	const float flangeInsideAngle = 82;
	const float flangeOutsideAngle = 65;
	const float treadFrangeDistance = 0.010f;
	const float colliderExtension = 0.01f;

	private bool refreshMeshesRequired = true;
	[SerializeField] public PhysicsMaterial physicsMaterial = null;
	[SerializeField] public Material colliderMaterial = null;

	public GameObject getGearCase() {
		return transform.Find("GearCase").gameObject;
	}
	public GameObject getAxelL() {
		return transform.Find("AxelL").gameObject;
	}
	public GameObject getAxelC() {
		return transform.Find("AxelC").gameObject;
	}
	public GameObject getAxelR() {
		return transform.Find("AxelR").gameObject;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		RefreshMeshes();
		refreshMeshesRequired = false;
	}

	void OnValidate() {
		refreshMeshesRequired = true;
	}

	// Update is called once per frame
	void Update() {
		// �K�v�ɉ����ă��b�V��������������
		if (refreshMeshesRequired) {
			RefreshMeshes();
			refreshMeshesRequired = false;
		}
	}

	void RefreshMeshes() {
		/*
		 *�@�b�@�@�@�(0, 0)�@�@�@�@�@�b
		 *�@�@�@�@�@�@�@�b�@�@�@�@�@�@�@�b�@�@
		 *�@�F�_�@�@�@�@���@�@�@�@�@�@�@�b�@�@
		 *�@�A�c�B�\�\�\���\�\�C�@�@�@�@�I
		 *�@�@�@�@�@�@�@�@�@�@�@�D�@�@�@/
		 *�@�@����+X�@�@�@�@�@�@�@�E�G�H
		 *�@�@���@�@�@�@�@�@�@�@�@���F��
		 *�@�@+Y
		 */
		float wheelRadius = wheelDiameter / 2f;
		Vector2 point1;
		Vector2 point2;
		Vector2 point3;
		Vector2 point4;
		Vector2 point5;
		Vector2 point6;
		Vector2 point7;
		Vector2 point8;
		Vector2 point9;
		Vector2 point10;

		float flangeInsideSlope = Mathf.Tan(flangeInsideAngle * Mathf.Deg2Rad);
		float flangeOutsideSlope = Mathf.Tan(flangeOutsideAngle * Mathf.Deg2Rad);

		// ���ʊO��
		point1.x = treadReferencePosition - wheelThickness;
		point2.x = treadReferencePosition - wheelThickness;
		point3.x = treadReferencePosition - wheelThickness + treadBevel;
		point3.y = point3.x * treadSlope;
		point2.y = point2.x * treadSlope;
		point1.y = point3.y - treadBevel;

		// �t�����W��
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

		// ����-�t�����W�O�ʂ̌�_
		point4.x = (point6.x * flangeOutsideSlope - point6.y) / (flangeOutsideSlope - treadSlope);
		point4.y = point4.x * treadSlope;

		// �R���C�_�[�`��
		float tan1 = Mathf.Tan(flangeInsideAngle * Mathf.Deg2Rad);
		float tan2 = Mathf.Tan((180 - flangeInsideAngle) / 2 * Mathf.Deg2Rad);
		float tan3 = Mathf.Tan((180 - flangeOutsideAngle) / 2 * Mathf.Deg2Rad);
		Vector2[] treadCollider1Points = new Vector2[] {
			point4+(point4 - point2).normalized  * colliderExtension + Vector2.up * wheelRadius,
			point2 + Vector2.up * wheelRadius
		};
		Vector2[] treadCollider2Points = new Vector2[] {
			point5 + Vector2.up * wheelRadius,
			(Vector2.zero + point4) / 2 + Vector2.up * wheelRadius
		};
		Vector2[] flangeColliderPoints = new Vector2[] {
			point10 + Vector2.up * (flangeHeight + wheelRadius),
			new Vector2(point10.x - flangeHeight / tan1 - flangeRadius * (1 / tan2 + 1 / tan3), wheelRadius + flangeHeight),
			point4 + (point4 - point6).normalized * colliderExtension + Vector2.up * wheelRadius
		};

		// Mesh�̐���
		Mesh treadCollider1Mesh = CreateCylinderMesh(treadCollider1Points, treadDivisions, false);
		Mesh treadRenderer1Mesh = CreateCylinderMesh(treadCollider1Points, treadDivisions, true);
		Mesh treadCollider2Mesh = CreateCylinderMesh(treadCollider2Points, treadDivisions, false);
		Mesh treadRenderer2Mesh = CreateCylinderMesh(treadCollider2Points, treadDivisions, true);
		Mesh flangeRenderer1Mesh = CreateCylinderMesh(flangeColliderPoints, flangeDivisions, true);
		Mesh flangeCollider1Mesh = CreateCylinderMesh(flangeColliderPoints, flangeDivisions, false);

		// Mesh�̐ݒ�
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
	/// �Փ˔���p�̉~���`�̃��b�V�����쐬����B
	/// </summary>
	/// <remarks>
	/// �ԗւ̃t�����W���ƃg���b�h���Ƃŋ��ʂŎg�p����B
	/// </remarks>
	/// <returns>�ԗւ̃t�����W���̏Փ˔���p���b�V��</returns>
	/// <param name="points">�~�����\������_�Q</param>
	/// <param name="divisions">�~���̕�����</param>
	/// <param name="fillEnd">�[�ʂ̏����Btrue�̏ꍇ�̂ݒ[�ʂ��쐬����B�\���p��true�A�R���C�_�p��false��ݒ肷��B</param>
	private static Mesh CreateCylinderMesh(Vector2[] points, int divisions, bool fillEnd) {

		// ���_�̐ݒ�
		List<Vector3> vertices = new List<Vector3>();
		for (int i = 0; i < points.Length; i++) {
			for (int j = 0; j < divisions; j++) {
				float theta = 2 * Mathf.PI / divisions * (j + 0.5f * (i % 2));
				vertices.Add(new Vector3(points[i].x, Mathf.Cos(theta) * points[i].y, Mathf.Sin(theta) * points[i].y));
			}
		}

		// �C���f�b�N�X�̐ݒ�
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

		// fillEnd�t���O�ɉ����Ē[�ʂ̃��b�V�����쐬����
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

		// Mesh�쐬
		Mesh mesh = new Mesh();
		mesh.SetVertices(vertices);
		mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();

		return mesh;
	}
}
