using System.Collections.Generic;
using UnityEngine;

public class RailConstructor : MonoBehaviour {

	[SerializeField] GameObject rail;

	private const float RANDOMISE_MAGNITUDE = 0.000f;

	private const float RADIUS = 150;
	private List<TransitionCurve> transitionCurves = new List<TransitionCurve>(){
		new TransitionCurve(0, 0, RADIUS/2),
		new TransitionCurve(0, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (0 + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (1.0 / RADIUS + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (1.0 / RADIUS + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (1.0 / RADIUS + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (1.0 / RADIUS + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (1.0 / RADIUS + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (1.0 / RADIUS + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (1.0 / RADIUS + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (1.0 / RADIUS + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (1.0 / RADIUS + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (1.0 / RADIUS + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 0, (2 * Mathf.PI / 12f / (0 + 1.0 / RADIUS))),
		new TransitionCurve(0, 0, RADIUS/2),
		new TransitionCurve(0, 0, RADIUS/2),
		new TransitionCurve(0, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (0 + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (1.0 / RADIUS + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (1.0 / RADIUS + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (1.0 / RADIUS + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (1.0 / RADIUS + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (1.0 / RADIUS + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (1.0 / RADIUS + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (1.0 / RADIUS + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (1.0 / RADIUS + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (1.0 / RADIUS + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 1.0 / RADIUS, (2 * Mathf.PI / 12f / (1.0 / RADIUS + 1.0 / RADIUS))),
		new TransitionCurve(1.0 / RADIUS, 0, (2 * Mathf.PI / 12f / (0 + 1.0 / RADIUS))),
		new TransitionCurve(0, 0, RADIUS/2)
	};

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {

		// レールの中心線の位置・角度
		Vector3 previousCurveEndPosition = Vector3.zero;
		float previousCurveEndAngle = 0f;

		// 左右レールの位置
		Vector3 fromL, fromR, toL, toR;
		fromL = previousCurveEndPosition + Vector3.left * (1.067f + 0.065f) * 0.5f;
		fromR = previousCurveEndPosition + Vector3.right * (1.067f + 0.065f) * 0.5f;

		for (int i = 0; i < transitionCurves.Count; i++) {
			Vector3 from, to;
			from = previousCurveEndPosition;

			for (int j = 1; j <= 50; j++) {
				float t = j / 50.0f;
				Vector2 curvePosition = transitionCurves[i].GetPosition(t);
				double curveAngle = transitionCurves[i].GetAngle(t);
				double curveCurvature = transitionCurves[i].GetCurvature(t);
				to = Quaternion.Euler(0, previousCurveEndAngle * Mathf.Rad2Deg, 0) * new Vector3(curvePosition.y, 0, curvePosition.x) + previousCurveEndPosition;

				float angle = previousCurveEndAngle + (float)curveAngle;
				toL = to + Quaternion.Euler(0, angle * Mathf.Rad2Deg, 0) * Vector3.left * (1.067f + 0.065f) * 0.5f;
				toR = to + Quaternion.Euler(0, angle * Mathf.Rad2Deg, 0) * Vector3.right * (1.067f + 0.065f) * 0.5f;

				// カントの設定
				/*
				if (curveCurvature > 0) {
					toL += Vector3.up * (float)curveCurvature * 7.5f;
				}
				if (curveCurvature < 0) {
					toR += Vector3.up * -(float)curveCurvature * 7.5f;
				}
				*/

				GameObject goL = CreateRail(fromL, toL);
				GameObject goR = CreateRail(fromR, toR);
				from = to;
				fromL = toL;
				fromR = toR;
			}
			previousCurveEndPosition = from;
			previousCurveEndAngle += (float)transitionCurves[i].GetAngle(1);
		}

	}

	// Update is called once per frame
	void Update() {

	}

	private GameObject CreateRail(Vector3 from, Vector3 to) {
		GameObject go = Instantiate(rail, transform);
		Rail railComponent = go.GetComponentInChildren<Rail>();
		railComponent.SetLength((to - from).magnitude);

		go.transform.localPosition = from;
		go.transform.localRotation = Quaternion.LookRotation(to - from, Vector3.up);

		return go;
	}

	private void OnDrawGizmos() {
		Gizmos.color = new Color(0, 0, 1, 1);
		Vector3 previousCurveEndPosition = Vector3.zero;
		float previousCurveEndAngle = 0f;
		for (int i = 0; i < transitionCurves.Count; i++) {
			Vector3 from, to;
			var transitionCurve = transitionCurves[i];
			from = previousCurveEndPosition;
			for (int j = 1; j <= 100; j++) {
				float t = j / 100.0f;
				Vector2 curvePosition = transitionCurve.GetPosition(t);
				to = Quaternion.Euler(0, previousCurveEndAngle * Mathf.Rad2Deg, 0) * new Vector3(curvePosition.y, 0, curvePosition.x) + previousCurveEndPosition;
				Gizmos.DrawLine(from, to);
				from = to;
			}
			previousCurveEndPosition = from;
			previousCurveEndAngle += (float)transitionCurve.GetAngle(1);
		}
	}
}
