using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SideSurface : MonoBehaviour {
	private Mesh mesh = null;

	/// <summary>
	/// ��(m�P��)
	/// </summary>
	[SerializeField, Tooltip("��(m�P��)")]
	float plateThickness = 0.004f;
	/// <summary>
	/// �[��(���m�̐ڍ���)��R���H���a(m�P��)
	/// </summary>
	[SerializeField, Tooltip("�[��(���m�̐ڍ���)��R���H���a(m�P��)")]
	float plateEndRound = 0.0005f;
	/// <summary>
	/// ����(���m�̐ڍ����ȊO)��R���H���a(m�P��)
	/// </summary>
	[SerializeField, Tooltip("����(���m�̐ڍ����ȊO)��R���H���a(m�P��)")]
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
	float rearApertureTopHeight = 1.75f;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		RefreshMesh();
		GetComponent<MeshFilter>().mesh = mesh;
	}

	// Update is called once per frame
	void Update() {

	}

	private void OnDrawGizmosSelected() {

		if (mesh == null) {
			RefreshMesh();
		}

		Gizmos.color = new Color(1, 0, 0, 1);
		Gizmos.DrawWireMesh(mesh, transform.position, transform.rotation, Vector3.one);
	}

	void RefreshMesh() {
		mesh = new Mesh();

		/*
		float frontApertureLength = 0.75f;
		float frontApertureBottomHeight = 0.75f;
		float frontApertureTopHeight = 1.75f;
		*/
		float angle = Mathf.PI / 6;
		float x0 = -plateThickness / 2;
		float x1 = -plateThickness / 2 + plateSideRound;
		float x2 = plateThickness / 2 - plateSideRound;
		float x3 = plateThickness / 2;
		float y0 = 0;
		float y1 = plateSideRound;
		float y2 = rearApertureBottomHeight - plateSideRound;
		float y3 = rearApertureBottomHeight;
		float z0 = 0;
		float z1 = plateEndRound;
		float z2 = rearApertureLength;
		float z3 = rearApertureLength + plateThickness / 2 * Mathf.Sin(angle);

		// ���_�o�b�t�@�̍쐬
		List<Vector3> vertices = new List<Vector3> {
			// -X��
			new Vector3(x0, y0, z3), new Vector3(x0, y0, z2), new Vector3(x0, y0, z1), new Vector3(x0, y0, z0),
			new Vector3(x0, y1, z3), new Vector3(x0, y1, z2), new Vector3(x0, y1, z1), new Vector3(x0, y1, z0),
			new Vector3(x0, y2, z3), new Vector3(x0, y2, z2), new Vector3(x0, y2, z1), new Vector3(x0, y2, z0),
			new Vector3(x0, y3, z3), new Vector3(x0, y3, z2), new Vector3(x0, y3, z1), new Vector3(x0, y3, z0),

			// +X��
			new Vector3(x3, y0, z0), new Vector3(x3, y0, z1), new Vector3(x3, y0, z2), new Vector3(x3, y0, z3),
			new Vector3(x3, y1, z0), new Vector3(x3, y1, z1), new Vector3(x3, y1, z2), new Vector3(x3, y1, z3),
			new Vector3(x3, y2, z0), new Vector3(x3, y2, z1), new Vector3(x3, y2, z2), new Vector3(x3, y2, z3),
			new Vector3(x3, y3, z0), new Vector3(x3, y3, z1), new Vector3(x3, y3, z2), new Vector3(x3, y3, z3),

			// -Y��
			new Vector3(x3, y0, z0), new Vector3(x2, y0, z0), new Vector3(x1, y0, z0), new Vector3(x0, y0, z0),
			new Vector3(x3, y0, z1), new Vector3(x2, y0, z1), new Vector3(x1, y0, z1), new Vector3(x0, y0, z1),
			new Vector3(x3, y0, z2), new Vector3(x2, y0, z2), new Vector3(x1, y0, z2), new Vector3(x0, y0, z2),
			new Vector3(x3, y0, z3), new Vector3(x2, y0, z3), new Vector3(x1, y0, z3), new Vector3(x0, y0, z3),

			// +Y��
			new Vector3(x0, y3, z0), new Vector3(x1, y3, z0), new Vector3(x2, y3, z0), new Vector3(x3, y3, z0),
			new Vector3(x0, y3, z1), new Vector3(x1, y3, z1), new Vector3(x2, y3, z1), new Vector3(x3, y3, z1),
			new Vector3(x0, y3, z2), new Vector3(x1, y3, z2), new Vector3(x2, y3, z2), new Vector3(x3, y3, z2),
			new Vector3(x0, y3, z3), new Vector3(x1, y3, z3), new Vector3(x2, y3, z3), new Vector3(x3, y3, z3),

			// -Z��
			new Vector3(x0, y0, z0), new Vector3(x1, y0, z0), new Vector3(x2, y0, z0), new Vector3(x3, y0, z0),
			new Vector3(x0, y1, z0), new Vector3(x1, y1, z0), new Vector3(x2, y1, z0), new Vector3(x3, y1, z0),
			new Vector3(x0, y2, z0), new Vector3(x1, y2, z0), new Vector3(x2, y2, z0), new Vector3(x3, y2, z0),
			new Vector3(x0, y3, z0), new Vector3(x1, y3, z0), new Vector3(x2, y3, z0), new Vector3(x3, y3, z0),

			// +Z��
			new Vector3(x3, y0, z3), new Vector3(x2, y0, z3), new Vector3(x1, y0, z3), new Vector3(x0, y0, z3),
			new Vector3(x3, y1, z3), new Vector3(x2, y1, z3), new Vector3(x1, y1, z3), new Vector3(x0, y1, z3),
			new Vector3(x3, y2, z3), new Vector3(x2, y2, z3), new Vector3(x1, y2, z3), new Vector3(x0, y2, z3),
			new Vector3(x3, y3, z3), new Vector3(x2, y3, z3), new Vector3(x1, y3, z3), new Vector3(x0, y3, z3)
		};

		// UV�o�b�t�@�̍쐬
		List<Vector2> uvs = new List<Vector2>();
		// -X��
		int k = 0;
		for (int i = 0; i < 16; i++, k++) {
			uvs.Add(new Vector2(vertices[k].z, vertices[k].y));
		}
		// +X��
		for (int i = 0; i < 16; i++, k++) {
			uvs.Add(new Vector2(vertices[k].z, vertices[k].y));
		}
		// -Y��
		for (int i = 0; i < 16; i++, k++) {
			uvs.Add(new Vector2(vertices[k].x, vertices[k].z));
		}
		// +Y��
		for (int i = 0; i < 16; i++, k++) {
			uvs.Add(new Vector2(vertices[k].x, vertices[k].z));
		}
		// -Z��
		for (int i = 0; i < 16; i++, k++) {
			uvs.Add(new Vector2(vertices[k].x, vertices[k].y));
		}
		// +Z��
		for (int i = 0; i < 16; i++, k++) {
			uvs.Add(new Vector2(vertices[k].x, vertices[k].y));
		}

		// ���_�̖@���o�b�t�@�̍쐬�ƈʒu�̒���
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

			if (vertex.z == z3) {
				vertices[i] = new Vector3(vertex.x * Mathf.Cos(angle), vertex.y, vertex.z);
			}
		}

		// �C���f�b�N�X�o�b�t�@�̍쐬
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

		// ���b�V���̍쐬
		mesh.SetVertices(vertices);
		mesh.SetNormals(normals);
		mesh.SetUVs(0, uvs);
		mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		mesh.RecalculateBounds();
		// mesh.RecalculateNormals();
		mesh.RecalculateTangents();
	}
}
