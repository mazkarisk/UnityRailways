using UnityEngine;

public class CameraController : MonoBehaviour {

	/// <summary>
	/// 注視点となるオブジェクト(nullも可)
	/// </summary>
	[SerializeField, TooltipAttribute("注視点となるオブジェクト(nullも可)")]
	private GameObject lookAtObject = null;

	private Vector3 lookAtPosition = Vector3.up * 0.0f;
	private Vector3 lookAtPositionSpeed;
	private float phi, theta, radius;
	private float phiSpeed, thetaSpeed, radiusSpeed;

	private Vector3 previousMousePosition;

	private float gizmoVisibleTime;
	private const float GIZMO_VISIBLE_TIME_MAX = 1f;

	// Use this for initialization
	void Start() {
		lookAtPositionSpeed = Vector3.zero;
		phiSpeed = 0f;
		thetaSpeed = 0f;
		radiusSpeed = 0f;

		phi = 45f;
		theta = 60f * Mathf.Deg2Rad;
		radius = (transform.position - lookAtPosition).magnitude;

		gizmoVisibleTime = 0f;
	}

	// Update is called once per frame
	void Update() {

		// 移動
		float inputX = (Input.GetKey(KeyCode.D) ? 1f : 0f) - (Input.GetKey(KeyCode.A) ? 1f : 0f);
		float inputY = (Input.GetKey(KeyCode.E) ? 1f : 0f) - (Input.GetKey(KeyCode.Q) ? 1f : 0f);
		float inputZ = (Input.GetKey(KeyCode.W) ? 1f : 0f) - (Input.GetKey(KeyCode.S) ? 1f : 0f);
		if (inputX != 0 || inputY != 0 || inputZ != 0) {
			gizmoVisibleTime = GIZMO_VISIBLE_TIME_MAX;
		}
		lookAtPositionSpeed += Quaternion.Euler(0f, -phi * Mathf.Rad2Deg - 90f, 0f) * (new Vector3(inputX, inputY, inputZ) * radius * Time.deltaTime * 10f);
		lookAtPosition += lookAtPositionSpeed * Time.deltaTime;
		lookAtPositionSpeed *= 0.8f;

		// 回転
		if (Input.GetMouseButton(1)) {
			Vector3 mouseVelocity = (Input.mousePosition - previousMousePosition);
			thetaSpeed += mouseVelocity.y * 0.1f;
			phiSpeed += mouseVelocity.x * -0.1f;
			//gizmoVisibleTime = GIZMO_VISIBLE_TIME_MAX;
		}
		phi += phiSpeed * Time.deltaTime;
		theta += thetaSpeed * Time.deltaTime;
		phiSpeed *= 0.8f;
		thetaSpeed *= 0.8f;

		// 縦回転の制限
		const float deltaAngle = 0.001f;
		if (theta < deltaAngle) theta = deltaAngle;
		if (theta > Mathf.PI - deltaAngle) theta = Mathf.PI - deltaAngle;

		// ズーム
		if (Input.mouseScrollDelta.y != 0f) {
			radiusSpeed += Input.mouseScrollDelta.y * -2f;
			gizmoVisibleTime = GIZMO_VISIBLE_TIME_MAX;
		}
		radius = Mathf.Exp(Mathf.Log(radius) + radiusSpeed * Time.deltaTime);
		radiusSpeed *= 0.8f;

		// カメラ位置・角度の更新
		Vector3 cameraLocalPosition = new Vector3(radius * Mathf.Cos(phi) * Mathf.Sin(theta), radius * Mathf.Cos(theta), radius * Mathf.Sin(phi) * Mathf.Sin(theta));
		if (lookAtObject != null) {
			transform.position = lookAtObject.transform.TransformPoint(cameraLocalPosition + lookAtPosition);
			transform.rotation = lookAtObject.transform.rotation * Quaternion.LookRotation(-cameraLocalPosition);
		} else {
			transform.position = cameraLocalPosition + lookAtPosition;
			transform.rotation = Quaternion.LookRotation(-cameraLocalPosition);
		}

		previousMousePosition = Input.mousePosition;
		gizmoVisibleTime -= Time.deltaTime;
	}

	private void OnDrawGizmos() {

		// 回転の中心位置を描画
		float a = Mathf.Clamp01(gizmoVisibleTime * 2f);
		Gizmos.color = new Color(1f, 0.92f, 0.016f, a);
		if (lookAtObject != null) {
			Gizmos.DrawWireSphere(lookAtObject.transform.TransformPoint(lookAtPosition), 0.1f);
		} else {
			Gizmos.DrawWireSphere(lookAtPosition, 0.1f);
		}
	}

}

