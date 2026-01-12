using UnityEngine;

public class House01SampleSceneController : MonoBehaviour {

	/// <summary>街灯のプレハブを設定する。</summary>
	[SerializeField]
	GameObject streetLightPrefab;

	/// <summary>天球のTransform。</summary>
	[SerializeField]
	Transform skyTransform;

	/// <summary>MainCameraのTransform。</summary>
	[SerializeField]
	Transform cameraTransform;

	float radius;

	void Start() {
		// 街灯を並べる。
		for (int z = 0; z <= 7; z++) {
			for (int x = 0; x <= 0; x++) {
				GameObject instantiatedStreetLight = Instantiate(streetLightPrefab, transform);
				instantiatedStreetLight.transform.localPosition = new Vector3(x * 16, 0, z * 12);
			}
		}

		radius = cameraTransform.localPosition.magnitude;
	}
	void Update() {
		// 天球を回す。
		float skyAngle = -80 - Time.time * 3;
		skyTransform.localRotation = Quaternion.Euler(23.4f, 0f, skyAngle);

		// カメラを回す。
		const float timeA = 12.83f;
		const float timeB = 73.95f;
		float angle = -135f;
		if (Time.time >= timeA && Time.time <= timeB) {
			float t = (Time.time - timeA) / (timeB - timeA);
			angle = angle + 360f * (1 - Mathf.Cos(t * Mathf.PI));
		}
		//cameraTransform.localEulerAngles = new Vector3(25, angle, 0);
		cameraTransform.localPosition = new Vector3(radius * Mathf.Cos(-angle * Mathf.Deg2Rad), 10f, radius * Mathf.Sin(-angle * Mathf.Deg2Rad));
		cameraTransform.localRotation = Quaternion.LookRotation(-cameraTransform.localPosition);

	}
}
