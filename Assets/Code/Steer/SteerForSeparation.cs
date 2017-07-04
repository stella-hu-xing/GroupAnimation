using UnityEngine;

/// <summary>
/// Steers a vehicle to keep separate from neighbors
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for Separation")]
public class SteerForSeparation : SteerForNeighbors
{
	#region Methods
	protected override Vector3 CalculateNeighborContribution(Vehicle other)
	{
		// 
		Vector3 offset = other.Position - Vehicle.Position;
		float distanceSquared = Vector3.Dot (offset, offset);
		Vector3 steering = (offset / -distanceSquared);	
		return steering;
	}
	#endregion
}

