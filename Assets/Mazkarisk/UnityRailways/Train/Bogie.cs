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

		// �L���[�Ɍv�������i�[
		Vector3 localLinearVelocity = transform.InverseTransformVector(rigidbody.linearVelocity);
		Vector3 localAngularVelocity = transform.InverseTransformVector(rigidbody.angularVelocity);
		bogieDataQueue.Enqueue(new BogieData(Time.fixedDeltaTime, localLinearVelocity, localAngularVelocity));

		// �L���[�̃T�C�Y�𐧌�����
		while (bogieDataQueue.Count > 100) {
			bogieDataQueue.Dequeue();
		}

		// �L���[���瑬�x�̕��ϒl���Z�o
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

		// �g���N�̐ݒ�
		Wheelset wheelsetF = goWheelsetF.GetComponent<Wheelset>();
		Wheelset wheelsetR = goWheelsetR.GetComponent<Wheelset>();
		HingeJoint hingeJointF = wheelsetF.getGearCase().GetComponent<HingeJoint>();
		HingeJoint hingeJointR = wheelsetR.getGearCase().GetComponent<HingeJoint>();

		if (notch == 0) {
			// �čs
			hingeJointF.useMotor = false;
			hingeJointR.useMotor = false;
		} else {
			JointMotor motor = new JointMotor();
			motor.freeSpin = false;
			motor.force = 2000f * Mathf.Abs(notch);
			if (notch > 0) {
				// �O�i
				motor.targetVelocity = -float.MaxValue * (backward ? -1 : 1); // �\���ɑ傫�Ȓl
				hingeJointF.motor = motor;
				motor.targetVelocity = -motor.targetVelocity;
				hingeJointR.motor = motor;
			} else {
				// �u���[�L
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
