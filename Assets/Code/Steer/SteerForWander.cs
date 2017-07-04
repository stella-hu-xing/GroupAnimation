using UnityEngine;
using UnitySteer;
using UnitySteer.Events;

/// <summary>
/// 修改版
/// Steers a vehicle to wander around
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for Wander")]
public class SteerForWander : Steering
{
	#region Private fields
	
	[SerializeField]
	float _wanderRadius=5.0f;
	[SerializeField]
	float _wanderDistance = 10;
	[SerializeField]
	float _wanderRange = 2;

    

    
	#endregion
	


	
	protected override Vector3 CalculateForce (){


		Vector3 center = Vehicle.Velocity.normalized;
		center *= _wanderDistance;
		Vector3 offset = Vector3.zero;
		offset = Random.insideUnitSphere * _wanderRange;
		offset.Normalize ();
		offset *= _wanderRadius;
		Vector3 Wforce = center + offset;

		return Wforce;

	}
	
}

