using UnityEngine;
using UnitySteer.Events;

[AddComponentMenu("UnitySteer/Steer/... for Seek")]
public class SteerForSeek : Steering
{
	
	enum  TargetCategory{ point=1,vehi=2};
	
	[SerializeField]
	TargetCategory tc;

	[SerializeField]
	Vector3 _targetPoint = Vector3.zero;
	[SerializeField]
	Vehicle _targetvehi;
	

	
	
	/// <summary>
	/// The target point.
	/// </summary>
	public Vector3 TargetPoint
	{
		get { return _targetPoint; }
		set
		{
			_targetPoint = value;
			ReportedArrival = false;
		}
	}
	public Vehicle TargetVehi
	{
		get { return _targetvehi; }
		set
		{
			_targetvehi = value;
			ReportedArrival = false;
		}
	}
	


	
	
	
	public new void Start()
	{
		base.Start();
		
		if (TargetPoint == Vector3.zero)
		{
			TargetPoint = transform.position;
		}
	}
	
	/// <summary>
	/// 计算力
	/// </returns>
	protected override Vector3 CalculateForce()
	{
		if (tc == TargetCategory.point) {
						return Vehicle.GetSeekVector (TargetPoint);
				} else 
						return Vehicle.GetSeekVector (TargetVehi.Position);
		
	}
}
