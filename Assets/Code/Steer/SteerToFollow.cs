using UnityEngine;
using UnitySteer.Events ;

namespace UnitySteer.Base
{

[AddComponentMenu("UnitySteer/Steer/... to Follow")]
public class SteerToFollow : Steering
{
	
	/// <summary>
	/// Target transform
	/// </summary>
	[SerializeField]
	Transform _target;


	/// <summary>
	/// How far behind we should follow the target
	/// </summary>
	[SerializeField]
	Vector3 _distance;
	
	
	/// <summary>
	/// The target.
	/// </summary>
 	public Transform Target
	{
		get { return _target; }
		set
		{
			_target = value;
			ReportedArrival = false;
		}
	}



	
	
	public new void Start()
	{
		base.Start();
		
		if (Target == null)
		{
			Target = transform;
		}
	}
	
	
	protected override Vector3 CalculateForce()
	{
		var difference = Target.forward;
		difference.Scale(_distance);
		return Vehicle.GetSeekVector(Target.position - difference);
	}
}

}

