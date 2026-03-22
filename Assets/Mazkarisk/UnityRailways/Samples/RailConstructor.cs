using Geometry;
using System.Collections.Generic;
using UnityEngine;

public class RailConstructor : MonoBehaviour {

	private const float RANDOMISE_MAGNITUDE = 0.000f;

	private const float RADIUS = 42.9f;
	private const float LENGTH = 24.2f;
	private List<TransitionCurve> transitionCurves = new List<TransitionCurve>(){
		new TransitionCurve(0, 0, LENGTH/2),
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
		new TransitionCurve(0, 0, LENGTH/2),
		new TransitionCurve(0, 0, LENGTH/2),
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
		new TransitionCurve(0, 0, LENGTH/2)
	};

	void Start() {

		Vector3 previousCurveEndPosition = Vector3.zero;
		float previousCurveEndAngle = 0f;

		// 軌道のプレハブを読み込む。
		GameObject trackPrefab = (GameObject)Resources.Load("Track");

		for (int i = 0; i < transitionCurves.Count; i++) {
			Quaternion rotator = Quaternion.Euler(0, Mathf.Rad2Deg * previousCurveEndAngle, 0);

			GameObject trackObject = Instantiate(trackPrefab, transform);
			trackObject.transform.localPosition = previousCurveEndPosition;
			trackObject.transform.localRotation = rotator;
			trackObject.transform.localScale = Vector3.one;
			trackObject.GetComponent<Track>().Initialize(new Path(transitionCurves[i]));

			Vector2 tempPosition = transitionCurves[i].GetPosition(1);
			previousCurveEndPosition += rotator * new Vector3(tempPosition.y, 0, tempPosition.x);
			previousCurveEndAngle += (float)transitionCurves[i].GetAngle(1);
		}

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
