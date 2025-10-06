using UnityEngine;

public class Train : MonoBehaviour {

	int notch = 0;
	bool backward = false;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		Bogie bogieF = transform.Find("BogieF").GetComponent<Bogie>();
		Bogie bogieR = transform.Find("BogieR").GetComponent<Bogie>();

		// ノッチ操作
		if (Input.GetKeyDown(KeyCode.DownArrow) && notch < 5) {
			notch++;
		}
		if (Input.GetKeyDown(KeyCode.UpArrow) && notch > -5) {
			notch--;
		}

		// 後進切り替え
		if (Input.GetKeyDown(KeyCode.R)) {
			backward = !backward;
		}

		bogieF.notch = notch;
		bogieF.backward = backward;
		bogieR.notch = notch;
		bogieR.backward = backward;
	}

	void OnGUI() {
		string logText = "";

		Bogie bogieF = transform.Find("BogieF").GetComponent<Bogie>();
		Bogie bogieR = transform.Find("BogieR").GetComponent<Bogie>();

		Vector3 averagedLinearVelocity = (bogieF.averagedLinearVelocity + bogieR.averagedLinearVelocity) / 2f;
		Vector3 averagedAngularVelocity = (bogieF.averagedAngularVelocity + bogieR.averagedAngularVelocity) / 2f;
		float averagedFixedDeltaTime = (bogieF.averagedFixedDeltaTime + bogieR.averagedFixedDeltaTime) / 2f;

		logText += "[↓]:加速、[↑]:減速、[Ｒ]:後進切替\n";
		logText += "ノッチ : " + notch + " / 5" + (backward ? " (後進)" : "") + "\n";
		//logText += "averagedLinearVelocity    [m/s] : " + averagedLinearVelocity.ToString() + "\n";
		//logText += "averagedAngularVelocity [rad/s] : " + averagedAngularVelocity.ToString() + "\n";
		//logText += "averagedFixedDeltaTime : " + averagedFixedDeltaTime + "\n";
		logText += "速度 [km/h] : " + (averagedLinearVelocity.z * 3.6f).ToString("F1") + "\n";

		/*
		float curvature = 0f;
		if (averagedLinearVelocity.z != 0) {
			curvature = averagedAngularVelocity.y / averagedLinearVelocity.z;
		}
		logText += "曲率 [rad/m] : " + curvature.ToString("F4") + "\n";
		float radiusFromCurvature = 0f;
		if (curvature != 0) {
			radiusFromCurvature = 1f / curvature;
			if (Mathf.Abs(radiusFromCurvature) > 10000f) {
				radiusFromCurvature = 0f;
			}
		}
		logText += "曲率半径 [m] : " + radiusFromCurvature.ToString("F0") + "\n";
		*/

		// ログのテキストスタイルを設定
		GUIStyle guiStyleBack = new GUIStyle();
		guiStyleBack.fontSize = 32;
		guiStyleBack.normal.textColor = Color.black;
		GUIStyle guiStyleFront = new GUIStyle();
		guiStyleFront.fontSize = 32;
		guiStyleFront.normal.textColor = Color.white;

		// 画面上にログ出力
		GUI.Label(new Rect(12, 12, Screen.width, Screen.height), logText, guiStyleBack);
		GUI.Label(new Rect(10, 10, Screen.width, Screen.height), logText, guiStyleFront);
	}
}
