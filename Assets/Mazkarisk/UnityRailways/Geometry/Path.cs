using UnityEngine;

namespace Geometry {

	/// <summary>
	/// �Ȑ�(�����܂�)���A�������ꂽ�����̓_�̍��W�̏W���ŕ\������B
	/// </summary>
	public class Path {

		private readonly Vector3[] positions;
		private readonly Vector3[] upDirection;
		private readonly float[] distances;
		private readonly float averageInterval;

		/// <summary>
		/// �R���X�g���N�^�B
		/// </summary>
		/// <param name="positions">�Ȑ���̓_�̍��W�̔z��B�Ⴆ�ΐ��H�̐��`�Ȃ琔mm�`��cm�P�ʂȂǁA�\���ɍׂ����Ԋu�ł��邱�Ƃ�z�肵�Ă���B</param>
		/// <param name="upDirection">�Ȑ���̓_�ɂ����������x�N�g���̔z��B�����Ő��K�����邽�߁A���K������Ă���K�v�͂Ȃ��B</param>
		public Path(Vector3[] positions, Vector3[] upDirection) {
			this.positions = positions;

			this.upDirection = new Vector3[positions.Length];
			if (upDirection != null && upDirection.Length == positions.Length) {
				// ������x�N�g�����������w�肳��Ă���ꍇ�A���K�����Ċi�[����B
				for (int i = 0; i < upDirection.Length; i++) {
					this.upDirection[i] = upDirection[i].normalized;
				}
			} else {
				// ������x�N�g�������w��ł���ꍇ�A���̒l(�^�����)�Ŗ��߂�B
				for (int i = 0; i < this.upDirection.Length; i++) {
					this.upDirection[i] = Vector3.up;
				}
			}

			// �n�_����̗݌v�������v�Z���ۑ����Ă����B
			distances = new float[this.positions.Length];
			distances[0] = 0; // �n�_�̗݌v������0�Ƃ���B
			for (int i = 1; i < this.positions.Length; i++) {
				distances[i] = (this.positions[i] - this.positions[i - 1]).magnitude + distances[i - 1];
			}

			// �_�Ԃ̕��ϊԊu�����߂�B
			averageInterval = distances[distances.Length - 1] / distances.Length;
		}

		/****************/
		/* ��{���\�b�h */
		/****************/

		/// <summary>
		/// �p�X�S�̂̒�����ԋp����B���ƂȂ����Ȑ��Ɋ֌W�Ȃ��A�_���m���q�����̒����̍��v���Ԃ����B
		/// </summary>
		public float GetOverallLength() {
			return distances[distances.Length - 1];
		}

		/// <summary>
		/// �p�X����w�肳�ꂽ�������i�񂾓_�ɂ��āA���̍��W���擾����B�_�Ԃ͐��`��Ԃ����B
		/// </summary>
		public Vector3 GetPositionByDistance(float distance) {
			// �w�肳�ꂽ������0�ȉ��Ȃ�A�n�_��Ԃ��B
			if (distance <= 0) {
				return positions[0];
			}

			for (int i = 1; i < distances.Length; i++) {
				// �w�肳�ꂽ���������_�܂ł̗݌v�����ȉ��Ȃ�A���_�ʒu����`��Ԃ��ĕԂ��B
				if (distance <= distances[i]) {
					float blendRate = (distance - distances[i - 1]) / (distances[i] - distances[i - 1]);
					return positions[i] * blendRate + positions[i - 1] * (1 - blendRate);
				}
			}

			// �w�肳�ꂽ�������S�̂̒�����蒷���Ȃ�A�I�_��Ԃ��B
			return positions[positions.Length - 1];
		}

		/// <summary>
		/// �p�X����w�肳�ꂽ�������i�񂾓_�ɂ��āA���̌������擾����B�ԋp�l�͐��K������Ă���B
		/// </summary>
		public Vector3 GetDirectionByDistance(float distance) {
			// �w�肳�ꂽ�������p�X�͈͓̔��Ɏ��߂�B
			distance = Mathf.Clamp(distance, 0, distances[distances.Length - 1]);

			Vector3 point0 = GetPositionByDistance(distance - averageInterval);
			Vector3 point1 = GetPositionByDistance(distance + averageInterval);
			return (point1 - point0).normalized;
		}

		/// <summary>
		/// �p�X����w�肳�ꂽ�������i�񂾓_�ɂ��āA���̏�������擾����B�ԋp�l�͐��K������Ă���B
		/// </summary>
		public Vector3 GetUpDirectionByDistance(float distance) {
			// �w�肳�ꂽ������0�ȉ��Ȃ�A�n�_��Ԃ��B
			if (distance <= 0) {
				return upDirection[0].normalized;
			}

			for (int i = 1; i < distances.Length; i++) {
				// �w�肳�ꂽ���������_�܂ł̗݌v�����ȉ��Ȃ�A���_�ʒu����`��Ԃ��ĕԂ��B
				if (distance <= distances[i]) {
					float blendRate = (distance - distances[i - 1]) / (distances[i] - distances[i - 1]);
					return (upDirection[i] * blendRate + upDirection[i - 1] * (1 - blendRate)).normalized;
				}
			}

			// �w�肳�ꂽ�������S�̂̒����ȏ�Ȃ�A�I�_��Ԃ��B
			return upDirection[upDirection.Length - 1].normalized;
		}

		/// <summary>
		/// �p�X����w�肳�ꂽ�������i�񂾓_�ɂ��āA���̍��������擾����B�ԋp�l�͐��K������Ă���B
		/// </summary>
		public Vector3 GetLeftDirectionByDistance(float distance) {
			return Vector3.Cross(GetDirectionByDistance(distance), GetUpDirectionByDistance(distance)).normalized;
		}

		/// <summary>
		/// �p�X����w�肳�ꂽ�������i�񂾓_�ɂ��āA���̉E�������擾����B�ԋp�l�͐��K������Ă���B
		/// </summary>
		public Vector3 GetRightDirectionByDistance(float distance) {
			return -GetLeftDirectionByDistance(distance);
		}

		/****************/
		/* �֗����\�b�h */
		/****************/

		/// <summary>
		/// �p�X����萔�ɕ����������_�̔z����쐬����B�E������𐳂Ƃ����A�������̃I�t�Z�b�g�ʂ��w��ł���B
		/// </summary>
		/// <param name="pointCount">�n�_�ƏI�_���܂ށA���_�̑����B</param>
		/// <param name="offset">�E������𐳂Ƃ����A�������̃I�t�Z�b�g�ʁB</param>
		/// <returns>���_�̔z��B</returns>
		public Vector3[] GetPositionArray(int pointCount, float offset) {
			Vector3[] result = new Vector3[pointCount];

			for (int i = 0; i < pointCount; i++) {
				float distance = GetOverallLength() * ((float)i / (pointCount - 1));
				result[i] = GetPositionByDistance(distance) + GetRightDirectionByDistance(distance) * offset;
			}

			return result;
		}
	}
}
