using UnityEngine;
using UnitySteer.Events ;

/// <summary>
/// 插入中间
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for Interpose")]
public class SteerForInterpose : Steering
{
	public Vehicle A;
	public Vehicle B;

	/// <summary>
	/// 计算力
	/// </returns>
	protected override Vector3 CalculateForce()
	{
		Vector3 Midpoint = (A.Position + B.Position) / 2;

		//预测未来位置
	/*	float Timetoreach2 = (this.transform.position - Midpoint).sqrMagnitude;
		float Timeto = Mathf.Sqrt (Timetoreach2) * 20;
		Vector3 Apos = A.Position + A.Velocity * Timeto;
		Vector3 BPos = B.Position + B.Velocity * Timeto;
		Midpoint = (Apos + BPos) / 2;
*/
		return Vehicle.GetArriveVector (Midpoint, Vehicle.Decelerate.fast);
	}
}
