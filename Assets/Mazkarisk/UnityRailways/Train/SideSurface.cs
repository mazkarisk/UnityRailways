using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteAlways]
public class SideSurface : MonoBehaviour {

	/// <summary>
	/// ��(m�P��)
	/// </summary>
	[SerializeField, Tooltip("��(m�P��)"), Range(0.001f, 0.1f)]
	float plateThickness = 0.004f;

	/// <summary>
	/// �[��(���m�̐ڍ���)��R���H���a(m�P��)
	/// </summary>
	[SerializeField, Tooltip("�[��(���m�̐ڍ���)��R���H���a(m�P��)"), Range(0.0001f, 0.1f)]
	float plateEndRound = 0.0005f;
	/// <summary>
	/// ����(���m�̐ڍ����ȊO)��R���H���a(m�P��)
	/// </summary>
	[SerializeField, Tooltip("����(���m�̐ڍ����ȊO)��R���H���a(m�P��)"), Range(0.0001f, 0.1f)]
	float plateSideRound = 0.001f;

	/// <summary>
	/// �S�̂̏㉺��������(m�P��)
	/// </summary>
	[SerializeField, Tooltip("�S�̂̏㉺��������(m�P��)")]
	float overallHeight = 2.9f;
	/// <summary>
	/// �S�̂̑O���������(m�P��)
	/// </summary>
	[SerializeField, Tooltip("�S�̂̑O���������(m�P��)")]
	float overallLength = 2f;

	/// <summary>
	/// ����̊J�����̑O������̒���(m�P��)
	/// </summary>
	[SerializeField, Tooltip("����̊J�����̑O������̒���(m�P��)")]
	float rearApertureLength = 0.75f;
	/// <summary>
	/// ����̊J�����̉������牺�̍���(m�P��)
	/// </summary>
	[SerializeField, Tooltip("����̊J�����̉������牺�̍���(m�P��)")]
	float rearApertureBottomHeight = 0.75f;
	/// <summary>
	/// ����̊J�����̏㕔�����̍���(m�P��)
	/// </summary>
	[SerializeField, Tooltip("����̊J�����̏㕔�����̍���(m�P��)")]
	float rearApertureTopHeight = 0.75f;
	/// <summary>
	/// ����̊J�����̊p�̊ۂ�(m�P��)
	/// </summary>
	[SerializeField, Tooltip("����̊J�����̊p�̊ۂ�(m�P��)"), Range(0.000001f, 1f)]
	float rearApertureRound = 0.1f;

	/// <summary>
	/// �O���̊J�����̑O������̒���(m�P��)
	/// </summary>
	[SerializeField, Tooltip("�O���̊J�����̑O������̒���(m�P��)")]
	float frontApertureLength = 0.75f;
	/// <summary>
	/// �O���̊J�����̉������牺�̍���(m�P��)
	/// </summary>
	[SerializeField, Tooltip("�O���̊J�����̉������牺�̍���(m�P��)")]
	float frontApertureBottomHeight = 0.75f;
	/// <summary>
	/// �O���̊J�����̏㕔�����̍���(m�P��)
	/// </summary>
	[SerializeField, Tooltip("�O���̊J�����̏㕔�����̍���(m�P��)")]
	float frontApertureTopHeight = 0.75f;
	/// <summary>
	/// �O���̊J�����̊p�̊ۂ�(m�P��)
	/// </summary>
	[SerializeField, Tooltip("�O���̊J�����̊p�̊ۂ�(m�P��)"), Range(0.000001f, 1f)]
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
		// �K�v�ɉ����ă��b�V��������������
		if (refreshMeshesRequired) {
			RefreshMeshes();
			refreshMeshesRequired = false;
		}
	}

	private void RefreshMeshes() {
		rearBottomMesh = NegativeZEndMesh(plateThickness, rearApertureBottomHeight, rearApertureLength - rearApertureRound, plateEndRound, plateSideRound);
		transform.Find("RearBottomPart").GetComponent<MeshFilter>().mesh = rearBottomMesh;
		transform.Find("RearBottomPart").localPosition = new Vector3(0, 0, rearApertureLength - rearApertureRound);

		rearTopMesh = NegativeZEndMesh(plateThickness, rearApertureTopHeight, rearApertureLength - rearApertureRound, plateEndRound, plateSideRound);
		transform.Find("RearTopPart").GetComponent<MeshFilter>().mesh = rearTopMesh;
		transform.Find("RearTopPart").localPosition = new Vector3(0, overallHeight - rearApertureTopHeight, rearApertureLength - rearApertureRound);

		midTopMesh = InversedTMesh();
		transform.Find("MidTopPart").GetComponent<MeshFilter>().mesh = midTopMesh;
		//transform.Find("MidTopPart").localPosition = new Vector3(0, rearApertureBottomHeight + rearApertureRound, rearApertureLength);

		midMesh = PillarMesh();
		transform.Find("MidPart").GetComponent<MeshFilter>().mesh = midMesh;
		transform.Find("MidPart").localPosition = new Vector3(0, rearApertureBottomHeight + rearApertureRound, rearApertureLength);

		midBottomMesh = TMesh();
		transform.Find("MidBottomPart").GetComponent<MeshFilter>().mesh = midBottomMesh;
		//transform.Find("MidBottomPart").localPosition = new Vector3(0, rearApertureBottomHeight + rearApertureRound, rearApertureLength);

		frontBottomMesh = PositiveZEndMesh(plateThickness, frontApertureBottomHeight, frontApertureLength - frontApertureRound, plateEndRound, plateSideRound);
		transform.Find("FrontBottomPart").GetComponent<MeshFilter>().mesh = frontBottomMesh;
		transform.Find("FrontBottomPart").localPosition = new Vector3(0, 0, overallLength - frontApertureLength + frontApertureRound);

		frontTopMesh = PositiveZEndMesh(plateThickness, frontApertureTopHeight, frontApertureLength - frontApertureRound, plateEndRound, plateSideRound); ;
		transform.Find("FrontTopPart").GetComponent<MeshFilter>().mesh = frontTopMesh;
		transform.Find("FrontTopPart").localPosition = new Vector3(0, overallHeight - frontApertureTopHeight, overallLength - frontApertureLength + frontApertureRound);
	}

	/// <summary>
	/// ���ނ�-Z�����̒[���̃��b�V���̍쐬
	/// </summary>
	/// <param name="thickness">��(m�P��)</param>
	/// <param name="height">����(m�P��)</param>
	/// <param name="length">����(m�P��)</param>
	/// <param name="endRound">�[�ʂ�R���H��(m�P��)</param>
	/// <param name="sideRound">�[�ʈȊO��R���H��(m�P��)</param>
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

		// ���_�o�b�t�@�̍쐬
		List<Vector3> vertices = ProcedualMesh.Get3x3GridVertices(x0, x1, x2, x3, y0, y1, y2, y3, z0, z1, z2, z3);

		// �@���o�b�t�@�̍쐬
		List<Vector3> normals = ProcedualMesh.Get3x3GridNormals(vertices);

		// UV�o�b�t�@�̍쐬
		List<Vector2> uvs = ProcedualMesh.Get3x3GridUVs(vertices, Vector3.zero);

		// �C���f�b�N�X�o�b�t�@�̍쐬
		List<int> indices = ProcedualMesh.Get3x3GridIndices();

		// ���_�̈ʒu�̒���
		for (int i = 0; i < vertices.Count; i++) {
			Vector3 vertex = vertices[i];
			if (vertex.z == z3) {
				vertices[i] = new Vector3(vertex.x * Mathf.Cos(angle), vertex.y, vertex.z);
			}
		}

		// ���b�V���̍쐬
		mesh.SetVertices(vertices);
		mesh.SetNormals(normals);
		mesh.SetUVs(0, uvs);
		mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		mesh.RecalculateBounds();
		mesh.RecalculateTangents();

		return mesh;
	}

	/// <summary>
	/// ���ނ�+Z�����̒[���̃��b�V���̍쐬
	/// </summary>
	/// <param name="thickness">��(m�P��)</param>
	/// <param name="height">����(m�P��)</param>
	/// <param name="length">����(m�P��)</param>
	/// <param name="endRound">�[�ʂ�R���H��(m�P��)</param>
	/// <param name="sideRound">�[�ʈȊO��R���H��(m�P��)</param>
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

		// ���_�o�b�t�@�̍쐬
		List<Vector3> vertices = ProcedualMesh.Get3x3GridVertices(x0, x1, x2, x3, y0, y1, y2, y3, z0, z1, z2, z3);

		// �@���o�b�t�@�̍쐬
		List<Vector3> normals = ProcedualMesh.Get3x3GridNormals(vertices);

		// UV�o�b�t�@�̍쐬
		List<Vector2> uvs = ProcedualMesh.Get3x3GridUVs(vertices, Vector3.zero);

		// �C���f�b�N�X�o�b�t�@�̍쐬
		List<int> indices = ProcedualMesh.Get3x3GridIndices();

		// ���_�̈ʒu�̒���
		for (int i = 0; i < vertices.Count; i++) {
			Vector3 vertex = vertices[i];
			if (vertex.z == z0) {
				vertices[i] = new Vector3(vertex.x * Mathf.Cos(angle), vertex.y, vertex.z);
			}
		}

		// ���b�V���̍쐬
		mesh.SetVertices(vertices);
		mesh.SetNormals(normals);
		mesh.SetUVs(0, uvs);
		mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		mesh.RecalculateBounds();
		mesh.RecalculateTangents();

		return mesh;
	}

	/// <summary>
	/// ���ނ̒����̃��b�V���̍쐬
	/// </summary>
	/// <returns></returns>
	private Mesh PillarMesh() {
		Mesh mesh = new Mesh();

		float angle = Mathf.PI / 4;
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

		// ���_�o�b�t�@�̍쐬
		List<Vector3> vertices = ProcedualMesh.Get3x3GridVertices(x0, x1, x2, x3, y0, y1, y2, y3, z0, z1, z2, z3);

		// �@���o�b�t�@�̍쐬
		List<Vector3> normals = ProcedualMesh.Get3x3GridNormals(vertices);

		// UV�o�b�t�@�̍쐬
		List<Vector2> uvs = ProcedualMesh.Get3x3GridUVs(vertices, Vector3.zero);

		// �C���f�b�N�X�o�b�t�@�̍쐬
		List<int> indices = ProcedualMesh.Get3x3GridIndices();

		// ���_�̈ʒu�̒���
		for (int i = 0; i < vertices.Count; i++) {
			Vector3 vertex = vertices[i];
			if (vertex.y == y0 || vertex.y == y3) {
				vertices[i] = new Vector3(vertex.x * Mathf.Cos(angle), vertex.y, vertex.z);
			}
		}

		// ���b�V���̍쐬
		mesh.SetVertices(vertices);
		mesh.SetNormals(normals);
		mesh.SetUVs(0, uvs);
		mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		mesh.RecalculateBounds();
		mesh.RecalculateTangents();

		return mesh;
	}

	/// <summary>
	/// ���ނ�"T"�����̃��b�V���̍쐬
	/// </summary>
	/// <returns></returns>
	private Mesh TMesh() {
		Mesh mesh = new Mesh();

		// TODO

		return mesh;
	}

	/// <summary>
	/// ���ނ�"��"�����̃��b�V���̍쐬
	/// </summary>
	/// <returns></returns>
	private Mesh InversedTMesh() {
		Mesh mesh = new Mesh();

		// TODO

		return mesh;
	}
}
