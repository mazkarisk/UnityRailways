using UnityEngine;

public class Detonator : MonoBehaviour {
	bool detonated = false;

	void Update() {
		if (!detonated && transform.position.z > -10) {
			GetComponent<ParticleSystem>().Play();
			detonated = true;
		}
	}
}
