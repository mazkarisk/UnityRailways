using UnityEngine;

public class Rail : MonoBehaviour {

	private GameObject[] railChunkObjects;
	private MeshFilter topMeshFilter;
	private MeshFilter sideMeshFilter;

	/// <summary>レール全体の初期化時の長さ。</summary>
	private float length;

	private void Start() {
		UpdateMesh(100);
	}

	public void UpdateMesh(int meshDivision) {
		// レール頭頂面のメッシュを更新する。
		if (topMeshFilter == null) {
			topMeshFilter = transform.Find("TopMesh").GetComponent<MeshFilter>();
		}
		topMeshFilter.mesh = RailUtility.CreateTopMesh(railChunkObjects, transform, length, meshDivision);

		// レール側面のメッシュを更新する。
		if (sideMeshFilter == null) {
			sideMeshFilter = transform.Find("SideMesh").GetComponent<MeshFilter>();
		}
		sideMeshFilter.mesh = RailUtility.CreateSideMesh(railChunkObjects, transform, length, meshDivision);
	}

	/// <summary>
	/// RailChunkを取得する。
	/// </summary>
	/// <param name="index">取得したいRailChunkのインデックス。</param>
	/// <returns>引数indexで指定したRailChunkのGameObject。</returns>
	public GameObject GetRailChunkObject(int index) {
		return railChunkObjects[index];
	}

	/// <summary>
	/// レールのGameObjectなどを初期化する。
	/// </summary>
	/// <param name="positionArray">レールが通る座標(ローカル座標系)の配列。</param>
	public void Initialize(Vector3[] positionArray) {
		length = 0;

		// レールのプレハブを読み込む。
		GameObject railChunkPrefab = (GameObject)Resources.Load("RailChunk");

		Transform railChunksTransform = transform.Find("RailChunks");
		railChunkObjects = new GameObject[positionArray.Length - 1];
		for (int i = 0; i < positionArray.Length - 1; i++) {
			Vector3 diff = positionArray[i + 1] - positionArray[i];
			length += diff.magnitude;

			// レールをインスタンス化し、設定を行う。
			railChunkObjects[i] = Instantiate(railChunkPrefab, railChunksTransform);
			railChunkObjects[i].name = "RailChunk" + i;
			railChunkObjects[i].transform.localPosition = positionArray[i];
			railChunkObjects[i].transform.localRotation = Quaternion.LookRotation(diff);
			railChunkObjects[i].transform.localScale = new Vector3(1, 1, diff.magnitude);
			railChunkObjects[i].GetComponent<Rigidbody>().mass = 40f * diff.magnitude;

			// 最後尾のレールなら自身のJointは使用しないので削除する。
			if (i >= positionArray.Length - 2) {
				DestroyImmediate(railChunkObjects[i].GetComponent<ConfigurableJoint>());
			}

			// 先頭以外の場合、一つ前のレールのJointに自身のRigidbodyを設定する。
			if (i > 0) {
				ConfigurableJoint joint = railChunkObjects[i - 1].GetComponent<ConfigurableJoint>();
				joint.connectedBody = railChunkObjects[i].GetComponent<Rigidbody>();
			}
		}
	}

	/// <summary>
	/// 各RailChunkのisKinematic属性を一括設定する。
	/// </summary>
	/// <param name="isKinematic">変更後のisKinematic。</param>
	public void SetKinematic(bool isKinematic) {
		for (int i = 0; i < railChunkObjects.Length; i++) {
			railChunkObjects[i].GetComponent<Rigidbody>().isKinematic = isKinematic;
		}
	}
}
