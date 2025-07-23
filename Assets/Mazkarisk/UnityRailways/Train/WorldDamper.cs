using UnityEngine;

public class WorldDamper : MonoBehaviour {
	private Vector3 previousLinearVelocity = Vector3.zero;

	void FixedUpdate() {
		Rigidbody rigidbody = GetComponent<Rigidbody>();
		if (rigidbody != null) {
			Vector3 acceleration = rigidbody.linearVelocity - previousLinearVelocity;
			Vector3 adjustedAcceleration = acceleration * 0.2f;

			rigidbody.linearVelocity = previousLinearVelocity + adjustedAcceleration;

			previousLinearVelocity = rigidbody.linearVelocity;
		}
	}
}
