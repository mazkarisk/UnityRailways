using System.Collections.Generic;
using UnityEngine;

public class Bogie : MonoBehaviour {
	[SerializeField]
	PhysicsMaterial physicsMaterial = null;
	[SerializeField]
	Material colliderMaterial = null;

	[SerializeField]
	float targetVelocityInKph = 100f;

	[SerializeField]
	GameObject goJournalBoxFL;
	[SerializeField]
	GameObject goJournalBoxFR;
	[SerializeField]
	GameObject goJournalBoxRL;
	[SerializeField]
	GameObject goJournalBoxRR;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		float wheelDiameter = 0.860f;

		GameObject goF = new GameObject();
		goF.name = "—ÖŽ²F";
		goF.transform.parent = transform;
		goF.transform.localPosition = new Vector3(0f, wheelDiameter / 2f, 2.1f / 2f);
		goF.transform.localRotation = Quaternion.identity;
		goF.transform.localScale = Vector3.one;
		Wheelset wheelsetF = goF.AddComponent<Wheelset>();
		wheelsetF.physicsMaterial = physicsMaterial;
		wheelsetF.colliderMaterial = colliderMaterial;

		GameObject goR = new GameObject();
		goR.name = "—ÖŽ²R";
		goR.transform.parent = transform;
		goR.transform.localPosition = new Vector3(0f, wheelDiameter / 2f, -2.1f / 2f);
		goR.transform.localRotation = Quaternion.identity;
		goR.transform.localScale = Vector3.one;
		Wheelset wheelsetR = goR.AddComponent<Wheelset>();
		wheelsetR.physicsMaterial = physicsMaterial;
		wheelsetR.colliderMaterial = colliderMaterial;

		JointWheelAndJournalBox(wheelsetF.getAxelL(), goJournalBoxFL, Vector3.zero, new Vector3(-(1.640f - 0.560f) / 2f, 0, 0));
		JointWheelAndJournalBox(wheelsetF.getAxelR(), goJournalBoxFR, Vector3.zero, new Vector3((1.640f - 0.560f) / 2f, 0, 0));
		JointWheelAndJournalBox(wheelsetR.getAxelL(), goJournalBoxRL, Vector3.zero, new Vector3(-(1.640f - 0.560f) / 2f, 0, 0));
		JointWheelAndJournalBox(wheelsetR.getAxelR(), goJournalBoxRR, Vector3.zero, new Vector3((1.640f - 0.560f) / 2f, 0, 0));

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
	Queue<BogieData> bogieDataQueue = new Queue<BogieData>();

	Vector3 averagedAngularVelocity = Vector3.zero;
	Vector3 averagedLinearVelocity = Vector3.zero;
	float averagedFixedDeltaTime = 1 / 60f;

	// Update is called once per frame
	void FixedUpdate() {
		Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();

		float ratio = averagedFixedDeltaTime * 10;
		averagedAngularVelocity = (averagedAngularVelocity * (ratio - Time.fixedDeltaTime) + transform.InverseTransformVector(rigidbody.angularVelocity) * Time.fixedDeltaTime) / ratio;
		averagedLinearVelocity = (averagedLinearVelocity * (ratio - Time.fixedDeltaTime) + transform.InverseTransformVector(rigidbody.linearVelocity) * Time.fixedDeltaTime) / ratio;
		averagedFixedDeltaTime = (averagedFixedDeltaTime * (ratio - Time.fixedDeltaTime) + Time.fixedDeltaTime * Time.fixedDeltaTime) / ratio;

		/*
		bogieDataQueue.Enqueue(new BogieData(Time.fixedDeltaTime, transform.InverseTransformVector(rigidbody.angularVelocity), transform.InverseTransformVector(rigidbody.linearVelocity)));
		foreach (var bogieData in bogieDataQueue) {
			bogieData.;
		}
		*/
		float targetVelocityInMps = targetVelocityInKph / 3.6f;
		float hoge = (targetVelocityInMps + 0.5f) - averagedLinearVelocity.z;
		if (hoge < 0) {
			hoge = 0;
		} else if (hoge > 1) {
			hoge = 1;
		} else {
			hoge = 1 - hoge * hoge;
		}

		rigidbody.AddForce(transform.forward * 50f * hoge, ForceMode.Acceleration);

	}

	// TODO
	private GUIStyle style = new GUIStyle();
	private GUIStyleState styleState = new GUIStyleState();
	void OnGUI() {
		int y = 0;
		style.fontStyle = FontStyle.Normal;
		style.fontSize = 20;
		styleState.textColor = Color.white;
		style.normal = styleState;

		GUI.Label(new Rect(10, y += 20, 500, 20), "averagedLinearVelocity    [m/s] : " + averagedLinearVelocity.ToString(), style);
		GUI.Label(new Rect(10, y += 20, 500, 20), "averagedAngularVelocity [rad/s] : " + averagedAngularVelocity.ToString(), style);
		GUI.Label(new Rect(10, y += 20, 500, 20), "averagedFixedDeltaTime : " + averagedFixedDeltaTime, style);
		GUI.Label(new Rect(10, y += 20, 500, 20), "‘¬“x [km/h] : " + averagedLinearVelocity.z * 3.6f, style);

		if (averagedLinearVelocity.z != 0) {
			float curvature = averagedAngularVelocity.y / averagedLinearVelocity.z;
			GUI.Label(new Rect(10, y += 20, 500, 20), "‹È—¦ [rad/m] : " + curvature, style);

			if (curvature != 0) {
				GUI.Label(new Rect(10, y += 20, 500, 20), "‹È—¦”¼Œa [m] : " + (1f / curvature), style);
			}
		}
	}

	ConfigurableJoint JointWheelAndJournalBox(GameObject wheel, GameObject journalBox, Vector3 anchor, Vector3 connectedAnchor) {
		SoftJointLimitSpring spring = new SoftJointLimitSpring();
		spring.spring = 100000000f;
		spring.damper = 10000000f;
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

}
