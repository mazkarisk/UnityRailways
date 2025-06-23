using UnityEditor.Rendering;
using UnityEngine;

public class RailConstructor : MonoBehaviour {

	[SerializeField] GameObject rail;

	private const float RANDOMISE_MAGNITUDE = 0.000f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		int numDivision = 360 * 2;

		float centerRadius = 300f;

		numDivision = (int)(2 * Mathf.PI * centerRadius / 5f);

		float cant = 0f;
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

}
