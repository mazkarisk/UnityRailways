using System;
using System.Collections.Generic;
using UnityEngine;

public class TransitionCurve {

	private const int START_DIVISION = 1;
	private const int MAX_DIVISION = 65536;

	/// <summary> 開始点の曲率[radians/m] </summary>
	public double startCurvature { get; }

	/// <summary> 終了点の曲率[radians/m] </summary>
	public double endCurvature { get; }

	/// <summary> 緩和曲線の長さ[m] </summary>
	public double length { get; }

	public List<List<double>> x { get; private set; }
	public List<List<double>> y { get; private set; }

	public TransitionCurve(double startCurvature, double endCurvature, double length) {
		this.startCurvature = startCurvature;
		this.endCurvature = endCurvature;
		this.length = length;

		Quadrature(QuadratureRules.Closed10);

		int index = 1;
		for (int division = START_DIVISION * 2; division <= MAX_DIVISION; division *= 2) {
			double x0 = x[index - 1][x[index - 1].Count - 1];
			double y0 = y[index - 1][y[index - 1].Count - 1];
			double x1 = x[index - 0][x[index - 0].Count - 1];
			double y1 = y[index - 0][y[index - 0].Count - 1];
			double diff = Math.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0));
			double diffRate = diff / length;
			Debug.Log(division + " : " + diffRate);
			index++;
		}
	}

	private void Quadrature(QuadratureRules rule) {
		x = new();
		y = new();
		for (int division = START_DIVISION; division <= MAX_DIVISION; division *= 2) {
			List<double> tempX = new();
			List<double> tempY = new();

			tempX.Add(0);
			tempY.Add(0);

			for (int i = 0; i < division; i++) {
				double a = length * (i + 0) / division;
				double b = length * (i + 1) / division;
				Vector3d quadratured = Quadrature(a, b, rule);

				tempX.Add(tempX[i] + quadratured.x);
				tempY.Add(tempY[i] + quadratured.y);
			}
			x.Add(tempX);
			y.Add(tempY);
		}
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
	private Vector3d Quadrature(double a, double b, QuadratureRules rule) {
		Vector3d f0, f1, f2, f3, f4, f5, f6, f7, f8, f9, f10;

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
				return ((b - a) / 8) * (f0 + f3 + 3 * (f1 +f2) );

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

	private Vector3d f(double s) {
		double angle = endCurvature / 2 * (s - length / Math.PI * Math.Sin(Math.PI * s / length));
		return new Vector3d(Math.Cos(angle), Math.Sin(angle));
	}

	private struct Vector3d {
		public double x { get; }
		public double y { get; }
		public double l { get; }

		public Vector3d(double x, double y) {
			this.x = x;
			this.y = y;
			l = Math.Sqrt(x * x + y * y);
		}
		public Vector3d(double x, double y, double l) {
			this.x = x;
			this.y = y;
			this.l = l;
		}

		public static Vector3d operator +(Vector3d left, Vector3d right)
			=> new Vector3d(left.x + right.x, left.y + right.y, left.l + right.l);
		public static Vector3d operator -(Vector3d left, Vector3d right)
			=> new Vector3d(left.x - right.x, left.y - right.y, left.l - right.l);
		public static Vector3d operator *(Vector3d left, double right)
			=> new Vector3d(left.x * right, left.y * right, left.l * right);
		public static Vector3d operator *(double left, Vector3d right)
			=> new Vector3d(left * right.x, left * right.y, left * right.l);
	}
}


