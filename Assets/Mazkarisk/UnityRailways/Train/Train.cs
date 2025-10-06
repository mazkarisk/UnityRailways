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

		// �m�b�`����
		if (Input.GetKeyDown(KeyCode.DownArrow) && notch < 5) {
			notch++;
		}
		if (Input.GetKeyDown(KeyCode.UpArrow) && notch > -5) {
			notch--;
		}

		// ��i�؂�ւ�
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

		logText += "[��]:�����A[��]:�����A[�q]:��i�ؑ�\n";
		logText += "�m�b�` : " + notch + " / 5" + (backward ? " (��i)" : "") + "\n";
		//logText += "averagedLinearVelocity    [m/s] : " + averagedLinearVelocity.ToString() + "\n";
		//logText += "averagedAngularVelocity [rad/s] : " + averagedAngularVelocity.ToString() + "\n";
		//logText += "averagedFixedDeltaTime : " + averagedFixedDeltaTime + "\n";
		logText += "���x [km/h] : " + (averagedLinearVelocity.z * 3.6f).ToString("F1") + "\n";

		/*
		float curvature = 0f;
		if (averagedLinearVelocity.z != 0) {
			curvature = averagedAngularVelocity.y / averagedLinearVelocity.z;
		}
		logText += "�ȗ� [rad/m] : " + curvature.ToString("F4") + "\n";
		float radiusFromCurvature = 0f;
		if (curvature != 0) {
			radiusFromCurvature = 1f / curvature;
			if (Mathf.Abs(radiusFromCurvature) > 10000f) {
				radiusFromCurvature = 0f;
			}
		}
		logText += "�ȗ����a [m] : " + radiusFromCurvature.ToString("F0") + "\n";
		*/

		// ���O�̃e�L�X�g�X�^�C����ݒ�
		GUIStyle guiStyleBack = new GUIStyle();
		guiStyleBack.fontSize = 32;
		guiStyleBack.normal.textColor = Color.black;
		GUIStyle guiStyleFront = new GUIStyle();
		guiStyleFront.fontSize = 32;
		guiStyleFront.normal.textColor = Color.white;

		// ��ʏ�Ƀ��O�o��
		GUI.Label(new Rect(12, 12, Screen.width, Screen.height), logText, guiStyleBack);
		GUI.Label(new Rect(10, 10, Screen.width, Screen.height), logText, guiStyleFront);
	}
}
