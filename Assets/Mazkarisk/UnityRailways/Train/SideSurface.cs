using UnityEngine;

public class SideSurface : MonoBehaviour {
	private Mesh mesh = null;

	/// <summary>
	/// 板厚(m単位)
	/// </summary>
	[SerializeField, Tooltip("板厚(m単位)")]
	float plateThickness = 0.004f;
	/// <summary>
	/// 端面(板同士の接合部)のR加工半径(m単位)
	/// </summary>
	[SerializeField, Tooltip("端面(板同士の接合部)のR加工半径(m単位)")]
	float plateEndRound = 0.0005f;
	/// <summary>
	/// 側面(板同士の接合部以外)のR加工半径(m単位)
	/// </summary>
	[SerializeField, Tooltip("側面(板同士の接合部以外)のR加工半径(m単位)")]
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
