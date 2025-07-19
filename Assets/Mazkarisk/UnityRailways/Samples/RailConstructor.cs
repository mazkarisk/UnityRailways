using System.Collections.Generic;
using UnityEngine;

public class RailConstructor : MonoBehaviour {

	[SerializeField] GameObject rail;

	private const float RANDOMISE_MAGNITUDE = 0.000f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		int numDivision = 360 * 5;

		float centerRadius = 1000f;

		float cant = 0.1f;
		float radius = centerRadius + (1.067f + 0.065f) * 0.5f;// - (Mathf.Sqrt(1.067f * 1.067f + cant * cant) - 1.067f);	// TODO ŒvŽZˆá‚¤
		for (int i = 0; i < numDivision; i++) {
			float x1 = Mathf.Cos(2f * Mathf.PI * ((float)i / (float)numDivision)) * radius + centerRadius;
			float z1 = Mathf.Sin(2f * Mathf.PI * ((float)i / (float)numDivision)) * radius;
			float x2 = Mathf.Cos(2f * Mathf.PI * ((float)(i + 1) / (float)numDivision)) * radius + centerRadius;
			float z2 = Mathf.Sin(2f * Mathf.PI * ((float)(i + 1) / (float)numDivision)) * radius;
			Vector3 from = new Vector3(x1, cant, z1);
			Vector3 to = new Vector3(x2, cant, z2);
			GameObject go = CreateRail(from + Random.insideUnitSphere * RANDOMISE_MAGNITUDE, to + Random.insideUnitSphere * RANDOMISE_MAGNITUDE);
		}
		radius = centerRadius - (1.067f + 0.065f) * 0.5f;
		for (int i = 0; i < numDivision; i++) {
			float x1 = Mathf.Cos(2f * Mathf.PI * ((float)i / (float)numDivision)) * radius + centerRadius;
			float z1 = Mathf.Sin(2f * Mathf.PI * ((float)i / (float)numDivision)) * radius;
			float x2 = Mathf.Cos(2f * Mathf.PI * ((float)(i + 1) / (float)numDivision)) * radius + centerRadius;
			float z2 = Mathf.Sin(2f * Mathf.PI * ((float)(i + 1) / (float)numDivision)) * radius;
			Vector3 from = new Vector3(x1, 0, z1);
			Vector3 to = new Vector3(x2, 0, z2);
			GameObject go = CreateRail(from + Random.insideUnitSphere * RANDOMISE_MAGNITUDE, to + Random.insideUnitSphere * RANDOMISE_MAGNITUDE);
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
		//go.transform.localScale = new Vector3(1, 1, (to - from).magnitude);

		/*
		CapsuleCollider[] sc = go.GetComponentsInChildren<CapsuleCollider>();
		for (int i = 0; i < sc.Length; i++) {
			sc[i].height = (1 + 0.026f / (to - from).magnitude) * 2f;
		}
		*/
		return go;
	}

	/*
	TransitionCurve tc = new TransitionCurve(0, 1.0 / 11, 120);
	private void OnDrawGizmos() {

		for (int i = 0; i < tc.x.Count; i++) {
			List<double> tempX = tc.x[i];
			List<double> tempY = tc.y[i];

			Gizmos.color = new Color(1, 0, 0, 1);
			Vector3 from, to;

			from = new Vector3((float)tc.x[i][0], (float)tc.y[i][0], i * 0.1f);
			for (int j = 1; j < tc.x[i].Count; j++) {
				if (tc.x[i].Count > 16 && j % (tc.x[i].Count / 16) > 0) {
					continue;
				}
				to = new Vector3((float)tc.x[i][j], (float)tc.y[i][j], i * 0.1f);
				Gizmos.DrawLine(from, to);
				from = to;
			}
		}
	}
	*/
}
