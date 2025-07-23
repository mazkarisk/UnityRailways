using UnityEngine;

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
	float rearApertureTopHeight = 0.75f;

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

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		Mesh rearBottomMesh = ProcedualMesh.PlateEnd(plateThickness, rearApertureBottomHeight, rearApertureLength, plateEndRound, plateSideRound);
		transform.Find("RearBottomPart").GetComponent<MeshFilter>().mesh = rearBottomMesh;
		transform.Find("RearBottomPart").SetLocalPositionAndRotation(new Vector3(0, 0, rearApertureLength), Quaternion.Euler(0, 180, 0));

		Mesh rearTopMesh = ProcedualMesh.PlateEnd(plateThickness, rearApertureTopHeight, rearApertureLength, plateEndRound, plateSideRound);
		transform.Find("RearTopPart").GetComponent<MeshFilter>().mesh = rearTopMesh;
		transform.Find("RearTopPart").SetLocalPositionAndRotation(new Vector3(0, overallHeight - rearApertureTopHeight, rearApertureLength), Quaternion.Euler(0, 180, 0));

		Mesh frontBottomMesh = ProcedualMesh.PlateEnd(plateThickness, frontApertureBottomHeight, frontApertureLength, plateEndRound, plateSideRound);
		transform.Find("FrontBottomPart").GetComponent<MeshFilter>().mesh = frontBottomMesh;
		transform.Find("FrontBottomPart").SetLocalPositionAndRotation(new Vector3(0, 0, overallLength - frontApertureLength), Quaternion.identity);

		Mesh frontTopMesh = ProcedualMesh.PlateEnd(plateThickness, frontApertureTopHeight, frontApertureLength, plateEndRound, plateSideRound);
		transform.Find("FrontTopPart").GetComponent<MeshFilter>().mesh = frontTopMesh;
		transform.Find("FrontTopPart").SetLocalPositionAndRotation(new Vector3(0, overallHeight - rearApertureTopHeight, overallLength - frontApertureLength), Quaternion.identity);

	}

	// Update is called once per frame
	void Update() {

	}

	private void OnDrawGizmosSelected() {

		Gizmos.color = new Color(1, 0, 0, 1);

		Mesh rearBottomMesh = ProcedualMesh.PlateEnd(plateThickness, rearApertureBottomHeight, rearApertureLength, plateEndRound, plateSideRound);
		Gizmos.DrawWireMesh(rearBottomMesh, transform.TransformPoint(Vector3.zero), transform.rotation, Vector3.one);
		Mesh rearTopMesh = ProcedualMesh.PlateEnd(plateThickness, rearApertureTopHeight, rearApertureLength, plateEndRound, plateSideRound);
		Gizmos.DrawWireMesh(rearTopMesh, transform.TransformPoint(new Vector3(0, overallHeight - rearApertureTopHeight, 0)), transform.rotation, Vector3.one);
	}
}
