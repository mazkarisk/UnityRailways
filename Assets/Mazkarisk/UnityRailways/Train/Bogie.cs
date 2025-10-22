using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Bogie : MonoBehaviour {
	[SerializeField]
	GameObject goWheelsetF;
	[SerializeField]
	GameObject goWheelsetR;

	Queue<BogieData> bogieDataQueue = new Queue<BogieData>();
	public float averagedFixedDeltaTime { get; private set; } = 1 / 60f;
	public Vector3 averagedLinearVelocity { get; private set; } = Vector3.zero;
	public Vector3 averagedAngularVelocity { get; private set; } = Vector3.zero;

	public int notch { get; set; } = 0;
	public bool backward { get; set; } = false;

	void FixedUpdate() {
		Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();

		// キューに計測情報を格納
		Vector3 localLinearVelocity = transform.InverseTransformVector(rigidbody.linearVelocity);
		Vector3 localAngularVelocity = transform.InverseTransformVector(rigidbody.angularVelocity);
		bogieDataQueue.Enqueue(new BogieData(Time.fixedDeltaTime, localLinearVelocity, localAngularVelocity));

		// キューのサイズを制限する
		while (bogieDataQueue.Count > 100) {
			bogieDataQueue.Dequeue();
		}

		// キューから速度の平均値を算出
		float sumFixedDeltaTime = 0f;
		Vector3 sumLinearVelocity = Vector3.zero;
		Vector3 sumAngularVelocity = Vector3.zero;
		foreach (var bogieData in bogieDataQueue) {
			sumFixedDeltaTime += bogieData.fixedDeltaTime;
			sumLinearVelocity += bogieData.localLinearVelocity * bogieData.fixedDeltaTime;
			sumAngularVelocity += bogieData.localAngularVelocity * bogieData.fixedDeltaTime;
		}
		averagedFixedDeltaTime = sumFixedDeltaTime / bogieDataQueue.Count;
		averagedAngularVelocity = sumAngularVelocity / sumFixedDeltaTime;
		averagedLinearVelocity = sumLinearVelocity / sumFixedDeltaTime;

		// トルクの設定
		Wheelset wheelsetF = goWheelsetF.GetComponent<Wheelset>();
		Wheelset wheelsetR = goWheelsetR.GetComponent<Wheelset>();
		HingeJoint hingeJointF = wheelsetF.getGearCase().GetComponent<HingeJoint>();
		HingeJoint hingeJointR = wheelsetR.getGearCase().GetComponent<HingeJoint>();

		if (notch == 0) {
			// 惰行
			hingeJointF.useMotor = false;
			hingeJointR.useMotor = false;
		} else {
			JointMotor motor = new JointMotor();
			motor.freeSpin = false;
			motor.force = 2000f * Mathf.Abs(notch);
			if (notch > 0) {
				// 前進
				motor.targetVelocity = -float.MaxValue * (backward ? -1 : 1); // 十分に大きな値
				hingeJointF.motor = motor;
				motor.targetVelocity = -motor.targetVelocity;
				hingeJointR.motor = motor;
			} else {
				// ブレーキ
				motor.targetVelocity = 0f;
				hingeJointF.motor = motor;
				hingeJointR.motor = motor;
			}
			hingeJointF.useMotor = true;
			hingeJointR.useMotor = true;
		}
	}

	private struct BogieData {
		public readonly float fixedDeltaTime { get; }
		public readonly Vector3 localLinearVelocity { get; }
		public readonly Vector3 localAngularVelocity { get; }
		public BogieData(float fixedDeltaTime, Vector3 localLinearVelocity, Vector3 localAngularVelocity) {
			this.fixedDeltaTime = fixedDeltaTime;
			this.localLinearVelocity = localLinearVelocity;
			this.localAngularVelocity = localAngularVelocity;
		}
	}
}
