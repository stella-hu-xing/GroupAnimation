using UnityEngine;
using System.Collections;

public class SteerForOffsetPursuit : Steering {

	/// <summary>
	/// Target transform
	/// </summary>
	[SerializeField]
	Vehicle _target;

	[SerializeField]
	Vector3 _offset;

	/// <summary>
	/// The target.
	/// </summary>
	public Vehicle Target
	{
		get { return _target; }
		set
		{
			_target = value;
			ReportedArrival = false;
		}
	}


	// Use this for initialization
	public new void Start(){
		base.Start();
		if (Target == null)
		{
			Target = Vehicle;
		}
	}
	
	protected override Vector3 CalculateForce()
	{
		Vector3 offsetPos = Target.Position - _offset;
		Vector3 toOffset = offsetPos - Vehicle.Position;

		float LookTime=toOffset.magnitude/(Vehicle.MaxSpeed+Target.Speed);

		return Vehicle.GetArriveVector (offsetPos + Target.Velocity * LookTime, Vehicle.Decelerate.fast);
		
	}
}
