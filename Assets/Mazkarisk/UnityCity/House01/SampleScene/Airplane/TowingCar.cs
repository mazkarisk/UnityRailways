using UnityEngine;

public class TowingCar : MonoBehaviour {
	[SerializeField]
	public Vector3 Velocity;

	private void Start() {
		SpringJoint joint = GetComponent<SpringJoint>();
		joint.connectedBody.linearVelocity = Velocity;
	}

	void FixedUpdate() {
		Rigidbody rigidbody = GetComponent<Rigidbody>();
		rigidbody.position = rigidbody.position + Velocity * Time.fixedDeltaTime;
	}
}
