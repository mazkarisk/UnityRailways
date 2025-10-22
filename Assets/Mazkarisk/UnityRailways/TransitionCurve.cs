using System;
using System.Collections.Generic;
using UnityEngine;

namespace Geometry {

	/// <summary>
	/// 逓減曲線を表現する。
	/// </summary>
	public class TransitionCurve {

		/// <summary> 開始点の曲率[radians/m] </summary>
		public double startCurvature { get; }

		/// <summary> 終了点の曲率[radians/m] </summary>
		public double endCurvature { get; }

		/// <summary> 緩和曲線の長さ[m] </summary>
		public double length { get; }

		/// <summary> 使用する積分法 </summary>
		public QuadratureRules rule { get; }

		/// <summary> 分割数 </summary>
		public int division { get; }

		public List<Vector2d> points { get; private set; }
		public List<QuadraticBezierCurve2D> bezierCurves { get; private set; }

		/// <summary>
		/// コンストラクタ(積分法、分割数未指定)
		/// </summary>
		/// <param name="startCurvature">開始点の曲率[radians/m]</param>
		/// <param name="endCurvature">終了点の曲率[radians/m]</param>
		/// <param name="length">緩和曲線の長さ[m]</param>
		public TransitionCurve(double startCurvature, double endCurvature, double length)
			: this(startCurvature, endCurvature, length, QuadratureRules.Closed10, 256) { }

		/// <summary>
		/// コンストラクタ(積分法、分割数指定)
		/// </summary>
		/// <param name="startCurvature">開始点の曲率[radians/m]</param>
		/// <param name="endCurvature">終了点の曲率[radians/m]</param>
		/// <param name="length">緩和曲線の長さ[m]</param>
		/// <param name="rule">使用する積分法</param>
		/// <param name="division">分割数</param>
		public TransitionCurve(double startCurvature, double endCurvature, double length, QuadratureRules rule, int division) {

			this.startCurvature = startCurvature;
			this.endCurvature = endCurvature;
			this.length = length;
			this.rule = rule;
			this.division = division;

			points = new() { Vector2d.zero };
			for (int i = 0; i < division; i++) {
				double a = length * (i + 0) / division;
				double b = length * (i + 1) / division;
				Vector2d quadratured = Quadrature(a, b, rule);
				points.Add(points[i] + quadratured);
			}

			bezierCurves = new() { new QuadraticBezierCurve2D(points[0], points[1], (points[1] + points[2]) / 2) };
			for (int i = 2; i <= division - 2; i++) {
				bezierCurves.Add(new QuadraticBezierCurve2D((points[i - 1] + points[i]) / 2, points[i], (points[i] + points[i + 1]) / 2));
			}
			bezierCurves.Add(new QuadraticBezierCurve2D((points[division - 2] + points[division - 1]) / 2, points[division - 1], points[division]));
		}

		/// <summary>
		/// 曲線のある時刻の座標を、二次ベジェ曲線補間で取得する。
		/// </summary>
		/// <param name="t">時刻(0～1)</param>
		/// <returns>曲線上の座標</returns>
		public Vector2 GetPosition(float t) {
			t = t * division;
			if (t < 1.5) {
				return bezierCurves[0].P(t / 1.5f);
			} else if (t >= division - 1.5f) {
				return bezierCurves[division - 2].P((t - (division - 1.5f)) / 1.5f);
			} else {
				return bezierCurves[(int)(t - 1.5f) + 1].P(t - (int)(t - 1.5f) - 1.5f);
			}
		}

		/// <summary>
		/// 曲線のある時刻の曲率を取得する。
		/// </summary>
		/// <param name="t">時刻(0～1)</param>
		/// <returns>曲線の曲率</returns>
		public double GetCurvature(double t) {
			double s = length * t;
			return (endCurvature + startCurvature) / 2 - ((endCurvature - startCurvature) / 2 * Math.Cos(Math.PI * s / length));
		}

		/// <summary>
		/// 曲線のある時刻の角度を取得する。
		/// </summary>
		/// <param name="t">時刻(0～1)</param>
		/// <returns>曲線の角度</returns>
		public double GetAngle(double t) {
			double s = length * t;
			return (endCurvature + startCurvature) / 2 * s - ((endCurvature - startCurvature) / (2 * Math.PI) * Math.Sin(Math.PI * s / length) * length);
		}

		/// <summary>
		/// 特定区間について数値積分を実施する
		/// </summary>
		/// <param name="a">区間の始まり</param>
		/// <param name="b">区間の終わり</param>
		/// <param name="rule">積分法</param>
		/// <returns>x座標、y座標についてそれぞれ数値積分した結果</returns>
		/// <exception cref="Exception">積分法</exception>
		private Vector2d Quadrature(double a, double b, QuadratureRules rule) {
			Vector2d f0, f1, f2, f3, f4, f5, f6, f7, f8, f9, f10;

			switch (rule) {

				// 台形公式
				case QuadratureRules.Closed1:
					// TODO
					f0 = f(a + (b - a) * (0.0 / 1.0));
					f1 = f(a + (b - a) * (1.0 / 1.0));
					return ((b - a) / 2) * (f0 + f1);

				// シンプソンの公式
				case QuadratureRules.Closed2:
					f0 = f(a + (b - a) * (0.0 / 2.0));
					f1 = f(a + (b - a) * (1.0 / 2.0));
					f2 = f(a + (b - a) * (2.0 / 2.0));
					return ((b - a) / 6) * (f0 + 4 * f1 + f2);

				// シンプソンの3/8公式
				case QuadratureRules.Closed3:
					f0 = f(a + (b - a) * (0.0 / 3.0));
					f1 = f(a + (b - a) * (1.0 / 3.0));
					f2 = f(a + (b - a) * (2.0 / 3.0));
					f3 = f(a + (b - a) * (3.0 / 3.0));
					return ((b - a) / 8) * (f0 + f3 + 3 * (f1 + f2));

				// ブールの公式
				case QuadratureRules.Closed4:
					f0 = f(a + (b - a) * (0.0 / 4.0));
					f1 = f(a + (b - a) * (1.0 / 4.0));
					f2 = f(a + (b - a) * (2.0 / 4.0));
					f3 = f(a + (b - a) * (3.0 / 4.0));
					f4 = f(a + (b - a) * (4.0 / 4.0));
					return ((b - a) / 90) * (7 * (f0 + f4) + 32 * (f1 + f3) + 12 * f2);

				// 6次の閉じたニュートン・コーツの公式
				case QuadratureRules.Closed6:
					f0 = f(a + (b - a) * (0.0 / 6.0));
					f1 = f(a + (b - a) * (1.0 / 6.0));
					f2 = f(a + (b - a) * (2.0 / 6.0));
					f3 = f(a + (b - a) * (3.0 / 6.0));
					f4 = f(a + (b - a) * (4.0 / 6.0));
					f5 = f(a + (b - a) * (5.0 / 6.0));
					f6 = f(a + (b - a) * (6.0 / 6.0));
					return ((b - a) / 840) * (41 * (f0 + f6) + 216 * (f1 + f5) + 27 * (f2 + f4) + 272 * f3);

				// 8次の閉じたニュートン・コーツの公式
				case QuadratureRules.Closed8:
					f0 = f(a + (b - a) * (0.0 / 8.0));
					f1 = f(a + (b - a) * (1.0 / 8.0));
					f2 = f(a + (b - a) * (2.0 / 8.0));
					f3 = f(a + (b - a) * (3.0 / 8.0));
					f4 = f(a + (b - a) * (4.0 / 8.0));
					f5 = f(a + (b - a) * (5.0 / 8.0));
					f6 = f(a + (b - a) * (6.0 / 8.0));
					f7 = f(a + (b - a) * (7.0 / 8.0));
					f8 = f(a + (b - a) * (8.0 / 8.0));
					return ((b - a) / 28350) * (989 * (f0 + f8) + 5888 * (f1 + f7) - 928 * (f2 + f6) + 10496 * (f3 + f5) - 4540 * f4);

				// 10次の閉じたニュートン・コーツの公式
				case QuadratureRules.Closed10:
					f0 = f(a + (b - a) * (0.0 / 10.0));
					f1 = f(a + (b - a) * (1.0 / 10.0));
					f2 = f(a + (b - a) * (2.0 / 10.0));
					f3 = f(a + (b - a) * (3.0 / 10.0));
					f4 = f(a + (b - a) * (4.0 / 10.0));
					f5 = f(a + (b - a) * (5.0 / 10.0));
					f6 = f(a + (b - a) * (6.0 / 10.0));
					f7 = f(a + (b - a) * (7.0 / 10.0));
					f8 = f(a + (b - a) * (8.0 / 10.0));
					f9 = f(a + (b - a) * (9.0 / 10.0));
					f10 = f(a + (b - a) * (10.0 / 10.0));
					return ((b - a) / 598752) * (16067 * (f0 + f10) + 106300 * (f1 + f9) - 48525 * (f2 + f8) + 272400 * (f3 + f7) - 260550 * (f4 + f6) + 427368 * f5);

				// 中点則
				case QuadratureRules.Open0:
					f0 = f(a + (b - a) * (1.0 / 2.0));
					return (b - a) * f0;

				// 台形法
				case QuadratureRules.Open1:
					f0 = f(a + (b - a) * (1.0 / 3.0));
					f1 = f(a + (b - a) * (2.0 / 3.0));
					return ((b - a) / 2) * (f0 + f1);

				// ミルンの公式
				case QuadratureRules.Open2:
					f0 = f(a + (b - a) * (1.0 / 4.0));
					f1 = f(a + (b - a) * (2.0 / 4.0));
					f2 = f(a + (b - a) * (3.0 / 4.0));
					return ((b - a) / 3) * (2 * f0 - f1 + 2 * f2);

				default:
					throw new Exception();
			}
		}

		private Vector2d f(double s) {
			double angle = (endCurvature + startCurvature) / 2 * s - ((endCurvature - startCurvature) / (2 * Math.PI) * Math.Sin(Math.PI * s / length) * length);
			return new Vector2d(Math.Cos(angle), Math.Sin(angle));
		}

		public enum QuadratureRules {
			/// <summary> 1次の閉じたニュートン・コーツの公式(台形公式) </summary>
			Closed1,
			/// <summary> 2次の閉じたニュートン・コーツの公式(シンプソンの公式) </summary>
			Closed2,
			/// <summary> 3次の閉じたニュートン・コーツの公式(シンプソンの3/8公式) </summary>
			Closed3,
			/// <summary> 4次の閉じたニュートン・コーツの公式(ブールの公式) </summary>
			Closed4,
			/// <summary> 6次の閉じたニュートン・コーツの公式 </summary>
			Closed6,
			/// <summary> 8次の閉じたニュートン・コーツの公式 </summary>
			Closed8,
			/// <summary> 10次の閉じたニュートン・コーツの公式 </summary>
			Closed10,
			/// <summary> 0次の開いたニュートン・コーツの公式(中点則) </summary>
			Open0,
			/// <summary> 1次の開いたニュートン・コーツの公式(台形法) </summary>
			Open1,
			/// <summary> 2次の開いたニュートン・コーツの公式(ミルンの公式) </summary>
			Open2
		}

		public struct Vector2d {
			public double x { get; }
			public double y { get; }

			public Vector2d(double x, double y) {
				this.x = x;
				this.y = y;
			}
			public Vector2d(double x, double y, double l) {
				this.x = x;
				this.y = y;
			}

			static public Vector2d zero => new Vector2d(0, 0, 0);

			public static Vector2d operator +(Vector2d left, Vector2d right)
				=> new Vector2d(left.x + right.x, left.y + right.y);
			public static Vector2d operator -(Vector2d left, Vector2d right)
				=> new Vector2d(left.x - right.x, left.y - right.y);
			public static Vector2d operator *(Vector2d left, double right)
				=> new Vector2d(left.x * right, left.y * right);
			public static Vector2d operator /(Vector2d left, double right)
				=> new Vector2d(left.x / right, left.y / right);
			public static Vector2d operator *(double left, Vector2d right)
				=> new Vector2d(left * right.x, left * right.y);

			public static implicit operator Vector2(Vector2d vector2d) => new Vector2((float)vector2d.x, (float)vector2d.y);
		}
	}
}
