using System.Collections.Generic;
using UnityEngine;

public class WorldDamper : MonoBehaviour {
	private Vector3 previousVelocity = Vector3.zero;
	private Queue<Vector3> velocityQueue = new Queue<Vector3>();

	void FixedUpdate() {
		SmoothenAcceleration();
	}

	void SmoothenAcceleration() {
		Rigidbody rigidbody = GetComponent<Rigidbody>();
		if (rigidbody == null) {
			return;
		}

		// 速度、加速度を求める。
		Vector3 velocity = rigidbody.linearVelocity;
		Vector3 acceleration = (velocity - previousVelocity) / Time.fixedDeltaTime;

		// 過去の呼び出し数回分の速度を保存しておく。
		velocityQueue.Enqueue(velocity);
		while (velocityQueue.Count > 10) {
			velocityQueue.Dequeue();
		}

		// 速度が遅い場合は処理を終了する。
		if (velocity.sqrMagnitude < 1) {
			return;
		}

		// 過去の呼び出し数回分の速度の平均値を求める。
		var sumVelocity = Vector3.zero;
		var velocityArray = velocityQueue.ToArray();
		for (int i = 0; i < velocityArray.Length; i++) {
			sumVelocity += velocityArray[i];
		}
		Vector3 averageVelocity = sumVelocity / velocityArray.Length;

		// 上で求めた平均速度方向以外の加速度成分を弱める。
		Quaternion rotator = Quaternion.FromToRotation(averageVelocity.normalized, Vector3.forward);
		Vector3 rotatedAcceleration = rotator * acceleration;
		Vector3 adjustedAcceleration = new Vector3(rotatedAcceleration.x * 0.5f, rotatedAcceleration.y * 0.5f, rotatedAcceleration.z);
		adjustedAcceleration = Quaternion.Inverse(rotator) * adjustedAcceleration;

		// 速度を適用する。
		acceleration = adjustedAcceleration;
		velocity = previousVelocity + acceleration * Time.fixedDeltaTime;
		rigidbody.linearVelocity = velocity;

		// 次の呼び出しに備えて速度を保存しておく。
		previousVelocity = rigidbody.linearVelocity;
	}
}
