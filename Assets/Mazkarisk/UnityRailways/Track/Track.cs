using Geometry;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Track : MonoBehaviour {

	/// <summary>軌間(m)</summary>
	const float Gauge = 1.067f;

	/// <summary>レール頭部の幅(m)</summary>
	const float RailHeadWidth = 0.064f;

	/// <summary>レール継目の隙間(m)</summary>
	const float RailEdgeClearance = 0.010f;

	/// <summary>枕木の奥行き(m)</summary>
	const float SleeperDepth = 0.200f;

	/// <summary>枕木の最大間隔(m)</summary>
	const float MaxSleeperInterval = 25f / 40f;

	bool requireInitialize = true;
	Path path = null;

	Rail leftRailComponent;
	Rail rightRailComponent;
	GameObject[] railSleeperObjects;

	void Start() {
		if (requireInitialize) {
			Initialize();
		}
	}

	void Update() {

	}

	private void OnDrawGizmosSelected() {
		const float offset = Gauge * 0.5f + RailHeadWidth * 0.5f;

		Gizmos.color = new Color(0, 0, 1, 1);

		if (path != null) {
			int div = 10;
			Vector3 previousPositionL = path.GetPosition(0) + path.GetLeftDirection(0) * offset;
			Vector3 previousPositionC = path.GetPosition(0);
			Vector3 previousPositionR = path.GetPosition(0) + path.GetRightDirection(0) * offset;
			for (int i = 1; i <= div; i++) {
				float distance = path.GetOverallLength() * (i / (float)div);
				Vector3 positionL = path.GetPosition(distance) + path.GetLeftDirection(distance) * offset;
				Vector3 positionC = path.GetPosition(distance);
				Vector3 positionR = path.GetPosition(distance) + path.GetRightDirection(distance) * offset;
				Gizmos.DrawLine(transform.TransformPoint(previousPositionL), transform.TransformPoint(positionL));
				Gizmos.DrawLine(transform.TransformPoint(previousPositionC), transform.TransformPoint(positionC));
				Gizmos.DrawLine(transform.TransformPoint(previousPositionR), transform.TransformPoint(positionR));

				previousPositionL = positionL;
				previousPositionC = positionC;
				previousPositionR = positionR;
			}

		}
		if (railSleeperObjects != null) {
			for (int i = 0; i < railSleeperObjects.Length; i++) {
				Gizmos.DrawSphere(railSleeperObjects[i].transform.position, 0.200f);
			}
		}
	}

	public void Initialize() {
		int numPoint = 1234;
		Vector3[] positions = new Vector3[numPoint];
		for (int i = 0; i < numPoint; i++) {
			float rate = i / (float)(numPoint - 1);
			positions[i] = Vector3.forward * 25f * rate;
			positions[i] += Vector3.right * rate * rate * 10f;
		}

		path = new Path(positions, new Vector3[] { });
		Initialize(path);
		SetKinematic(false);
	}

	public void Initialize(Path path) {
		requireInitialize = false;
		this.path = path;

		const float offset = Gauge * 0.5f + RailHeadWidth * 0.5f;

		// 枕木のプレハブを読み込む。
		GameObject sleeperPrefab = (GameObject)Resources.Load("RailSleeper");

		// レールのプレハブを読み込む。
		GameObject railPrefab = (GameObject)Resources.Load("Rail");

		int railChunkCount = 50;

		// レールをインスタンス化する。
		GameObject leftRailObject = Instantiate(railPrefab);
		leftRailObject.transform.parent = transform;
		leftRailObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		leftRailComponent = leftRailObject.GetComponent<Rail>();
		leftRailComponent.Initialize(path.GetPositionArray(railChunkCount + 1, RailEdgeClearance * 0.5f, -offset, 0.260f));

		GameObject rightRailObject = Instantiate(railPrefab);
		rightRailObject.transform.parent = transform;
		rightRailObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		rightRailComponent = rightRailObject.GetComponent<Rail>();
		rightRailComponent.Initialize(path.GetPositionArray(railChunkCount + 1, RailEdgeClearance * 0.5f, offset, 0.260f));

		// 枕木の個数を算出。
		int sleepersCount = (int)Mathf.Ceil((path.GetOverallLength() - SleeperDepth - RailEdgeClearance) / MaxSleeperInterval) + 1;

		// 枕木をインスタンス化。
		railSleeperObjects = new GameObject[sleepersCount];
		float sleeperInterval = (path.GetOverallLength() - SleeperDepth - RailEdgeClearance) / (sleepersCount - 1);
		for (int i = 0; i < sleepersCount; i++) {
			float distance = RailEdgeClearance * 0.5f + SleeperDepth * 0.5f + sleeperInterval * i;

			railSleeperObjects[i] = Instantiate(sleeperPrefab);
			railSleeperObjects[i].transform.parent = transform;
			railSleeperObjects[i].transform.localPosition = path.GetPosition(distance) + Vector3.up * 0.100f;
			railSleeperObjects[i].transform.localRotation = path.GetLookRotation(distance);
			railSleeperObjects[i].transform.localScale = Vector3.one;

			// 枕木のジョイントを取得する。上面左→右の順で配列に格納される。
			ConfigurableJoint[] joints = railSleeperObjects[i].GetComponents<ConfigurableJoint>();

			// 枕木と接続
			int railChunkIndex = (int)((distance / path.GetOverallLength()) * railChunkCount);
			joints[0].connectedBody = leftRailComponent.GetRailChunkObject(railChunkIndex).GetComponent<Rigidbody>();
			joints[1].connectedBody = rightRailComponent.GetRailChunkObject(railChunkIndex).GetComponent<Rigidbody>();
		}

		// 道床のメッシュを作成。分割数は枕木の数を流用。
		Mesh mesh = TrackUtility.CreateTrackbedMesh(path, sleepersCount);
		Transform trackbedTransform = transform.Find("Trackbed");
		trackbedTransform.GetComponent<MeshFilter>().mesh = mesh;
	}

	public void SetKinematic(bool isKinematic) {
		leftRailComponent.SetKinematic(isKinematic);
		rightRailComponent.SetKinematic(isKinematic);
		for (int i = 0; i < railSleeperObjects.Length; i++) {
			railSleeperObjects[i].GetComponent<Rigidbody>().isKinematic = isKinematic;
		}
	}
}
