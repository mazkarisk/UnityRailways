using System.Collections.Generic;
using UnityEngine;

public class Bogie : MonoBehaviour {
	[SerializeField]
	GameObject goJournalBoxFL;
	[SerializeField]
	GameObject goJournalBoxFR;
	[SerializeField]
	GameObject goJournalBoxRL;
	[SerializeField]
	GameObject goJournalBoxRR;
	[SerializeField]
	GameObject goWheelsetF;
	[SerializeField]
	GameObject goWheelsetR;

	Queue<BogieData> bogieDataQueue = new Queue<BogieData>();
	public Vector3 averagedAngularVelocity { get; private set; } = Vector3.zero;
	public Vector3 averagedLinearVelocity { get; private set; } = Vector3.zero;
	public float averagedFixedDeltaTime { get; private set; } = 1 / 60f;
	public float torque { get; set; } = 0f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		Wheelset wheelsetF = goWheelsetF.GetComponent<Wheelset>();
		Wheelset wheelsetR = goWheelsetR.GetComponent<Wheelset>();
		JointWheelAndJournalBox(wheelsetF.getAxelL(), goJournalBoxFL, Vector3.zero, new Vector3(-(1.640f - 0.560f) / 2f, 0, 0));
		JointWheelAndJournalBox(wheelsetF.getAxelR(), goJournalBoxFR, Vector3.zero, new Vector3((1.640f - 0.560f) / 2f, 0, 0));
		JointWheelAndJournalBox(wheelsetR.getAxelL(), goJournalBoxRL, Vector3.zero, new Vector3(-(1.640f - 0.560f) / 2f, 0, 0));
		JointWheelAndJournalBox(wheelsetR.getAxelR(), goJournalBoxRR, Vector3.zero, new Vector3((1.640f - 0.560f) / 2f, 0, 0));
	}

	// Update is called once per frame
	void FixedUpdate() {
		Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();

		float ratio = averagedFixedDeltaTime * 10;
		averagedAngularVelocity = (averagedAngularVelocity * (ratio - Time.fixedDeltaTime) + transform.InverseTransformVector(rigidbody.angularVelocity) * Time.fixedDeltaTime) / ratio;
		averagedLinearVelocity = (averagedLinearVelocity * (ratio - Time.fixedDeltaTime) + transform.InverseTransformVector(rigidbody.linearVelocity) * Time.fixedDeltaTime) / ratio;
		averagedFixedDeltaTime = (averagedFixedDeltaTime * (ratio - Time.fixedDeltaTime) + Time.fixedDeltaTime * Time.fixedDeltaTime) / ratio;

		Wheelset wheelsetF = goWheelsetF.GetComponent<Wheelset>();
		Wheelset wheelsetR = goWheelsetR.GetComponent<Wheelset>();
		Rigidbody axelRigidbodyF = wheelsetF.getAxelC().GetComponent<Rigidbody>();
		Rigidbody axelRigidbodyR = wheelsetR.getAxelC().GetComponent<Rigidbody>();
		axelRigidbodyF.maxAngularVelocity = float.PositiveInfinity;
		axelRigidbodyR.maxAngularVelocity = float.PositiveInfinity;
		axelRigidbodyF.AddRelativeTorque(new Vector3(torque, 0, 0), ForceMode.Force);
		axelRigidbodyR.AddRelativeTorque(new Vector3(torque, 0, 0), ForceMode.Force);
	}

	ConfigurableJoint JointWheelAndJournalBox(GameObject wheel, GameObject journalBox, Vector3 anchor, Vector3 connectedAnchor) {
		SoftJointLimitSpring spring = new SoftJointLimitSpring();
		spring.spring = 1000000f;
		spring.damper = 100000f;
		SoftJointLimit limit = new SoftJointLimit();
		limit.limit = 0.000001f;

		ConfigurableJoint joint = journalBox.AddComponent<ConfigurableJoint>();

		joint.connectedBody = wheel.GetComponent<Rigidbody>();
		joint.autoConfigureConnectedAnchor = true;   // TODO
		joint.anchor = anchor;
		joint.connectedAnchor = connectedAnchor;
		joint.massScale = 1f;
		joint.connectedMassScale = 1f;   // TODO
		joint.xMotion = ConfigurableJointMotion.Limited;   // TODO
		joint.yMotion = ConfigurableJointMotion.Locked;   // TODO
		joint.zMotion = ConfigurableJointMotion.Locked;   // TODO
		joint.angularXMotion = ConfigurableJointMotion.Free;
		joint.angularYMotion = ConfigurableJointMotion.Limited;    // TODO
		joint.angularZMotion = ConfigurableJointMotion.Limited;    // TODO
		joint.linearLimit = limit;
		joint.angularYLimit = limit;
		joint.angularZLimit = limit;
		joint.linearLimitSpring = spring;
		joint.angularYZLimitSpring = spring;

		return joint;
	}

	// TODO
	struct BogieData {
		public readonly float fixedDeltaTime { get; }
		public readonly Vector3 localAngularVelocity { get; }
		public readonly Vector3 localLinearVelocity { get; }
		public BogieData(float fixedDeltaTime, Vector3 localAngularVelocity, Vector3 localLinearVelocity) {
			this.fixedDeltaTime = fixedDeltaTime;
			this.localAngularVelocity = localAngularVelocity;
			this.localLinearVelocity = localLinearVelocity;
		}
	}
}
