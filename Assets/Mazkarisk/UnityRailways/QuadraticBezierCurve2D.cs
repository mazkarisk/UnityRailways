using UnityEngine;

namespace Geometry {

	/// <summary>
	/// 二次元平面上のベジェ曲線を表現する。
	/// </summary>
	public class QuadraticBezierCurve2D {

		readonly private Vector2 p0;
		readonly private Vector2 p1;
		readonly private Vector2 p2;

		public QuadraticBezierCurve2D(Vector2 p0, Vector2 p1, Vector2 p2) {
			this.p0 = p0;
			this.p1 = p1;
			this.p2 = p2;
		}

		public Vector2 P(float t) {
			return t * t * (p0 - 2.0f * p1 + p2) + 2.0f * t * (-p0 + p1) + p0;
		}
	}
}
