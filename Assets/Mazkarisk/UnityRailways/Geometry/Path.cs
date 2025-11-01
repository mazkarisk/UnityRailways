using UnityEngine;

namespace Geometry {

	/// <summary>
	/// 曲線(直線含む)を、分割された複数の点の座標の集合で表現する。
	/// </summary>
	public class Path {

		private readonly Vector3[] positions;
		private readonly Vector3[] upDirections;
		private readonly float[] distances;
		private readonly float averageInterval;

		/// <summary>
		/// コンストラクタ。
		/// </summary>
		/// <param name="positions">曲線上の点の座標の配列。例えば線路の線形なら数mm～数cm単位など、十分に細かく、かつほぼ等間隔であることを想定している。</param>
		/// <param name="upDirections">曲線上の点における上方向ベクトルの配列。内部で正規化するため、正規化されている必要はない。</param>
		public Path(Vector3[] positions, Vector3[] upDirections) {
			this.positions = positions;

			this.upDirections = new Vector3[positions.Length];
			if (upDirections != null && upDirections.Length == positions.Length) {
				// 上方向ベクトルが正しく指定されている場合、正規化して格納する。
				for (int i = 0; i < upDirections.Length; i++) {
					this.upDirections[i] = upDirections[i].normalized;
				}
			} else {
				// 上方向ベクトルが未指定である場合、仮の値(真上方向)で埋める。
				for (int i = 0; i < this.upDirections.Length; i++) {
					this.upDirections[i] = Vector3.up;
				}
			}

			// 始点からの累計距離を計算し保存しておく。
			distances = new float[this.positions.Length];
			distances[0] = 0; // 始点の累計距離は0とする。
			for (int i = 1; i < this.positions.Length; i++) {
				distances[i] = (this.positions[i] - this.positions[i - 1]).magnitude + distances[i - 1];
			}

			// 点間の平均間隔を求める。
			averageInterval = distances[distances.Length - 1] / distances.Length;
		}

		/// <summary>
		/// TransitionCurveからPathを作成するコンストラクタ。
		/// </summary>
		/// <param name="original">元となるTransitionCurveオブジェクト。</param>
		public Path(TransitionCurve original) {

			positions = new Vector3[original.division + 1];
			for (int i = 0; i < positions.Length; i++) {
				float t = (float)i / original.division;
				Vector2 tempPosition = original.GetPosition(t);
				positions[i] = new Vector3(tempPosition.y, 0, tempPosition.x);
			}

			upDirections = new Vector3[original.division + 1];
			for (int i = 0; i < upDirections.Length; i++) {
				upDirections[i] = Vector3.up; // TODO 良い感じにする
			}

			// 始点からの累計距離を計算し保存しておく。
			distances = new float[positions.Length];
			distances[0] = 0; // 始点の累計距離は0とする。
			for (int i = 1; i < positions.Length; i++) {
				distances[i] = (positions[i] - positions[i - 1]).magnitude + distances[i - 1];
			}

			// 点間の平均間隔を求める。
			averageInterval = distances[distances.Length - 1] / distances.Length;
		}

		/****************/
		/* 基本メソッド */
		/****************/

		/// <summary>
		/// パス全体の長さを返却する。元となった曲線に関係なく、点同士を繋ぐ線の長さの合計が返される。
		/// </summary>
		public float GetOverallLength() {
			return distances[distances.Length - 1];
		}

		/// <summary>
		/// パス上を指定された長さ分進んだ点について、その座標を取得する。点間は線形補間される。
		/// </summary>
		public Vector3 GetPosition(float distance) {
			// 指定された距離が0以下なら、始点を返す。
			if (distance <= 0) {
				return positions[0];
			}

			for (int i = 1; i < distances.Length; i++) {
				// 指定された距離が頂点までの累計距離以下なら、頂点位置を線形補間して返す。
				if (distance <= distances[i]) {
					float blendRate = (distance - distances[i - 1]) / (distances[i] - distances[i - 1]);
					return positions[i] * blendRate + positions[i - 1] * (1 - blendRate);
				}
			}

			// 指定された距離が全体の長さより長いなら、終点を返す。
			return positions[positions.Length - 1];
		}

		/// <summary>
		/// パス上を指定された長さ分進んだ点について、その向きを取得する。返却値は正規化されている。
		/// </summary>
		public Vector3 GetDirection(float distance) {
			// 指定された距離をパスの範囲内に収める。
			distance = Mathf.Clamp(distance, 0, distances[distances.Length - 1]);

			Vector3 point0 = GetPosition(distance - averageInterval);
			Vector3 point1 = GetPosition(distance + averageInterval);
			return (point1 - point0).normalized;
		}

		/// <summary>
		/// パス上を指定された長さ分進んだ点について、その上方向を取得する。返却値は正規化されている。
		/// </summary>
		public Vector3 GetUpDirection(float distance) {
			// 指定された距離が0以下なら、始点を返す。
			if (distance <= 0) {
				return upDirections[0].normalized;
			}

			for (int i = 1; i < distances.Length; i++) {
				// 指定された距離が頂点までの累計距離以下なら、頂点位置を線形補間して返す。
				if (distance <= distances[i]) {
					float blendRate = (distance - distances[i - 1]) / (distances[i] - distances[i - 1]);
					return (upDirections[i] * blendRate + upDirections[i - 1] * (1 - blendRate)).normalized;
				}
			}

			// 指定された距離が全体の長さ以上なら、終点を返す。
			return upDirections[upDirections.Length - 1].normalized;
		}

		/// <summary>
		/// パス上を指定された長さ分進んだ点について、その左方向を取得する。返却値は正規化されている。
		/// </summary>
		public Vector3 GetLeftDirection(float distance) {
			return Vector3.Cross(GetDirection(distance), GetUpDirection(distance)).normalized;
		}

		/// <summary>
		/// パス上を指定された長さ分進んだ点について、その右方向を取得する。返却値は正規化されている。
		/// </summary>
		public Vector3 GetRightDirection(float distance) {
			return -GetLeftDirection(distance);
		}

		/****************/
		/* 便利メソッド */
		/****************/

		/// <summary>
		/// パスを一定数に分割した頂点の配列を作成する。右手方向を正とした、横方向のオフセット量も指定できる。
		/// </summary>
		/// <param name="pointCount">始点と終点を含む、頂点の総数。</param>
		/// <param name="rightOffset">右手方向を正とした、横方向のオフセット量。</param>
		/// <param name="upOffset">上方向を正とした、上下方向のオフセット量。</param>
		/// <returns>頂点の配列。</returns>
		public Vector3[] GetPositionArray(int pointCount, float rightOffset, float upOffset) {
			Vector3[] result = new Vector3[pointCount];

			for (int i = 0; i < pointCount; i++) {
				float distance = GetOverallLength() * ((float)i / (pointCount - 1));
				result[i] = GetPosition(distance) + GetRightDirection(distance) * rightOffset + GetUpDirection(distance) * upOffset;
			}

			return result;
		}

		/// <summary>
		/// パス上を指定された長さ分進んだ点について、その地点のパスの移動方向を向く回転を表すQuaternionを作成する。
		/// </summary>
		/// <param name="distance">始点からの、点間を結ぶ線上を移動した累計距離。</param>
		/// <returns>パスの移動方向を向く回転を表すQuaternion</returns>
		public Quaternion GetLookRotation(float distance) {
			return Quaternion.LookRotation(GetDirection(distance), GetUpDirection(distance));
		}
	}
}
