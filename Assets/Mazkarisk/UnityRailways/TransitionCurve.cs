using System;
using System.Collections.Generic;
using UnityEngine;

namespace Geometry {

	/// <summary>
	/// �����Ȑ���\������B
	/// </summary>
	public class TransitionCurve {

		/// <summary> �J�n�_�̋ȗ�[radians/m] </summary>
		public double startCurvature { get; }

		/// <summary> �I���_�̋ȗ�[radians/m] </summary>
		public double endCurvature { get; }

		/// <summary> �ɘa�Ȑ��̒���[m] </summary>
		public double length { get; }

		/// <summary> �g�p����ϕ��@ </summary>
		public QuadratureRules rule { get; }

		/// <summary> ������ </summary>
		public int division { get; }

		public List<Vector2d> points { get; private set; }
		public List<QuadraticBezierCurve2D> bezierCurves { get; private set; }

		/// <summary>
		/// �R���X�g���N�^(�ϕ��@�A���������w��)
		/// </summary>
		/// <param name="startCurvature">�J�n�_�̋ȗ�[radians/m]</param>
		/// <param name="endCurvature">�I���_�̋ȗ�[radians/m]</param>
		/// <param name="length">�ɘa�Ȑ��̒���[m]</param>
		public TransitionCurve(double startCurvature, double endCurvature, double length)
			: this(startCurvature, endCurvature, length, QuadratureRules.Closed10, 256) { }

		/// <summary>
		/// �R���X�g���N�^(�ϕ��@�A�������w��)
		/// </summary>
		/// <param name="startCurvature">�J�n�_�̋ȗ�[radians/m]</param>
		/// <param name="endCurvature">�I���_�̋ȗ�[radians/m]</param>
		/// <param name="length">�ɘa�Ȑ��̒���[m]</param>
		/// <param name="rule">�g�p����ϕ��@</param>
		/// <param name="division">������</param>
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
		/// �Ȑ��̂��鎞���̍��W���A�񎟃x�W�F�Ȑ���ԂŎ擾����B
		/// </summary>
		/// <param name="t">����(0�`1)</param>
		/// <returns>�Ȑ���̍��W</returns>
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
		/// �Ȑ��̂��鎞���̋ȗ����擾����B
		/// </summary>
		/// <param name="t">����(0�`1)</param>
		/// <returns>�Ȑ��̋ȗ�</returns>
		public double GetCurvature(double t) {
			double s = length * t;
			return (endCurvature + startCurvature) / 2 - ((endCurvature - startCurvature) / 2 * Math.Cos(Math.PI * s / length));
		}

		/// <summary>
		/// �Ȑ��̂��鎞���̊p�x���擾����B
		/// </summary>
		/// <param name="t">����(0�`1)</param>
		/// <returns>�Ȑ��̊p�x</returns>
		public double GetAngle(double t) {
			double s = length * t;
			return (endCurvature + startCurvature) / 2 * s - ((endCurvature - startCurvature) / (2 * Math.PI) * Math.Sin(Math.PI * s / length) * length);
		}

		/// <summary>
		/// �����Ԃɂ��Đ��l�ϕ������{����
		/// </summary>
		/// <param name="a">��Ԃ̎n�܂�</param>
		/// <param name="b">��Ԃ̏I���</param>
		/// <param name="rule">�ϕ��@</param>
		/// <returns>x���W�Ay���W�ɂ��Ă��ꂼ�ꐔ�l�ϕ���������</returns>
		/// <exception cref="Exception">�ϕ��@</exception>
		private Vector2d Quadrature(double a, double b, QuadratureRules rule) {
			Vector2d f0, f1, f2, f3, f4, f5, f6, f7, f8, f9, f10;

			switch (rule) {

				// ��`����
				case QuadratureRules.Closed1:
					// TODO
					f0 = f(a + (b - a) * (0.0 / 1.0));
					f1 = f(a + (b - a) * (1.0 / 1.0));
					return ((b - a) / 2) * (f0 + f1);

				// �V���v�\���̌���
				case QuadratureRules.Closed2:
					f0 = f(a + (b - a) * (0.0 / 2.0));
					f1 = f(a + (b - a) * (1.0 / 2.0));
					f2 = f(a + (b - a) * (2.0 / 2.0));
					return ((b - a) / 6) * (f0 + 4 * f1 + f2);

				// �V���v�\����3/8����
				case QuadratureRules.Closed3:
					f0 = f(a + (b - a) * (0.0 / 3.0));
					f1 = f(a + (b - a) * (1.0 / 3.0));
					f2 = f(a + (b - a) * (2.0 / 3.0));
					f3 = f(a + (b - a) * (3.0 / 3.0));
					return ((b - a) / 8) * (f0 + f3 + 3 * (f1 + f2));

				// �u�[���̌���
				case QuadratureRules.Closed4:
					f0 = f(a + (b - a) * (0.0 / 4.0));
					f1 = f(a + (b - a) * (1.0 / 4.0));
					f2 = f(a + (b - a) * (2.0 / 4.0));
					f3 = f(a + (b - a) * (3.0 / 4.0));
					f4 = f(a + (b - a) * (4.0 / 4.0));
					return ((b - a) / 90) * (7 * (f0 + f4) + 32 * (f1 + f3) + 12 * f2);

				// 6���̕����j���[�g���E�R�[�c�̌���
				case QuadratureRules.Closed6:
					f0 = f(a + (b - a) * (0.0 / 6.0));
					f1 = f(a + (b - a) * (1.0 / 6.0));
					f2 = f(a + (b - a) * (2.0 / 6.0));
					f3 = f(a + (b - a) * (3.0 / 6.0));
					f4 = f(a + (b - a) * (4.0 / 6.0));
					f5 = f(a + (b - a) * (5.0 / 6.0));
					f6 = f(a + (b - a) * (6.0 / 6.0));
					return ((b - a) / 840) * (41 * (f0 + f6) + 216 * (f1 + f5) + 27 * (f2 + f4) + 272 * f3);

				// 8���̕����j���[�g���E�R�[�c�̌���
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

				// 10���̕����j���[�g���E�R�[�c�̌���
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

				// ���_��
				case QuadratureRules.Open0:
					f0 = f(a + (b - a) * (1.0 / 2.0));
					return (b - a) * f0;

				// ��`�@
				case QuadratureRules.Open1:
					f0 = f(a + (b - a) * (1.0 / 3.0));
					f1 = f(a + (b - a) * (2.0 / 3.0));
					return ((b - a) / 2) * (f0 + f1);

				// �~�����̌���
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
			/// <summary> 1���̕����j���[�g���E�R�[�c�̌���(��`����) </summary>
			Closed1,
			/// <summary> 2���̕����j���[�g���E�R�[�c�̌���(�V���v�\���̌���) </summary>
			Closed2,
			/// <summary> 3���̕����j���[�g���E�R�[�c�̌���(�V���v�\����3/8����) </summary>
			Closed3,
			/// <summary> 4���̕����j���[�g���E�R�[�c�̌���(�u�[���̌���) </summary>
			Closed4,
			/// <summary> 6���̕����j���[�g���E�R�[�c�̌��� </summary>
			Closed6,
			/// <summary> 8���̕����j���[�g���E�R�[�c�̌��� </summary>
			Closed8,
			/// <summary> 10���̕����j���[�g���E�R�[�c�̌��� </summary>
			Closed10,
			/// <summary> 0���̊J�����j���[�g���E�R�[�c�̌���(���_��) </summary>
			Open0,
			/// <summary> 1���̊J�����j���[�g���E�R�[�c�̌���(��`�@) </summary>
			Open1,
			/// <summary> 2���̊J�����j���[�g���E�R�[�c�̌���(�~�����̌���) </summary>
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
