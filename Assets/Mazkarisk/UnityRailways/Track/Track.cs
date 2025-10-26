using Geometry;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour {

	/// <summary>�O��(m)</summary>
	const float Gauge = 1.067f;

	/// <summary>���[�������̕�(m)</summary>
	const float RailHeadWidth = 0.064f;

	/// <summary>���؂̉��s��(m)</summary>
	const float SleeperDepth = 0.200f;

	/// <summary>���؂̍ő�Ԋu(m)</summary>
	const float MaxSleeperInterval = 25f / 40f;

	bool requireInitialize = true;
	Path path = null;

	GameObject[] railObjectsLeft;
	GameObject[] railObjectsRight;
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

		// ���؂̃v���n�u��ǂݍ��ށB
		GameObject sleeperPrefab = (GameObject)Resources.Load("RailSleeper");

		// ���[���̃v���n�u��ǂݍ��ށB
		GameObject railChunkPrefab = (GameObject)Resources.Load("RailChunk");

		int railChunkCount = 50;

		// ���[�����C���X�^���X���B
		railObjectsLeft = new GameObject[railChunkCount];
		railObjectsRight = new GameObject[railChunkCount];
		InstantiateRail(railObjectsLeft, railChunkPrefab, path.GetPositionArray(railChunkCount + 1, -offset, 0.200f));
		InstantiateRail(railObjectsRight, railChunkPrefab, path.GetPositionArray(railChunkCount + 1, offset, 0.200f));

		// ���؂̌����Z�o�B
		int sleepersCount = (int)Mathf.Ceil((path.GetOverallLength() - SleeperDepth) / MaxSleeperInterval) + 1;

		// ���؂��C���X�^���X���B
		railSleeperObjects = new GameObject[sleepersCount];
		float sleeperInterval = (path.GetOverallLength() - SleeperDepth) / (sleepersCount - 1);
		for (int i = 0; i < sleepersCount; i++) {
			float distance = SleeperDepth * 0.5f + sleeperInterval * i;

			railSleeperObjects[i] = Instantiate(sleeperPrefab);
			railSleeperObjects[i].transform.parent = transform;
			railSleeperObjects[i].transform.localPosition = path.GetPosition(distance);
			railSleeperObjects[i].transform.localRotation = path.GetLookRotation(distance);
			railSleeperObjects[i].transform.localScale = Vector3.one;

			// ���؂̃W���C���g���擾����B��ʍ����E�̏��Ŕz��Ɋi�[�����B
			ConfigurableJoint[] joints = railSleeperObjects[i].GetComponents<ConfigurableJoint>();

			// ���؂Ɛڑ�
			int railChunkIndex = (int)((distance / path.GetOverallLength()) * railChunkCount);
			joints[0].connectedBody = railObjectsLeft[railChunkIndex].GetComponent<Rigidbody>();
			joints[1].connectedBody = railObjectsRight[railChunkIndex].GetComponent<Rigidbody>();
		}

	}

	private void InstantiateRail(GameObject[] railObjects, GameObject railChunkPrefab, Vector3[] positionArray) {
		for (int i = 0; i < positionArray.Length - 1; i++) {
			Vector3 diff = positionArray[i + 1] - positionArray[i];

			// ���[���`�����N���C���X�^���X�����A�ݒ���s���B
			railObjects[i] = Instantiate(railChunkPrefab, transform);
			railObjects[i].name = "RailChunk" + i;
			railObjects[i].transform.localPosition = positionArray[i];
			railObjects[i].transform.localRotation = Quaternion.LookRotation(diff);
			railObjects[i].transform.localScale = new Vector3(1, 1, diff.magnitude);
			railObjects[i].GetComponent<Rigidbody>().mass = 40f * diff.magnitude;

			// �Ō���̃��[���Ȃ玩�g��Joint�͎g�p���Ȃ��̂ō폜����B
			if (i >= positionArray.Length - 2) {
				DestroyImmediate(railObjects[i].GetComponent<ConfigurableJoint>());
			}

			// �擪�ȊO�̏ꍇ�A��O�̃��[����Joint�Ɏ��g��Rigidbody��ݒ肷��B
			if (i > 0) {
				ConfigurableJoint joint = railObjects[i - 1].GetComponent<ConfigurableJoint>();
				joint.connectedBody = railObjects[i].GetComponent<Rigidbody>();
			}
		}
	}

	public void SetKinematic(bool isKinematic) {
		for (int i = 0; i < railObjectsLeft.Length; i++) {
			railObjectsLeft[i].GetComponent<Rigidbody>().isKinematic = isKinematic;
		}
		for (int i = 0; i < railObjectsRight.Length; i++) {
			railObjectsRight[i].GetComponent<Rigidbody>().isKinematic = isKinematic;
		}
		for (int i = 0; i < railSleeperObjects.Length; i++) {
			railSleeperObjects[i].GetComponent<Rigidbody>().isKinematic = isKinematic;
		}
	}
}
