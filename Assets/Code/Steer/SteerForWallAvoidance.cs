using UnityEngine;
using UnitySteer;
using System.Collections;
using UnitySteer.Events;
using System.Linq;

public class SteerForWallAvoidance : Steering {


	#region Structs
	public struct PathIntersection
	{
		bool _intersect;
		float _distance;
		DetectableObject _obstacle;
		
		public bool Intersect 
		{ 
			get { return _intersect; }
			set { _intersect = value; }
		}
		
		public float Distance 
		{ 
			get { return _distance; }
			set { _distance = value; }
		}
		
		
		public DetectableObject Obstacle 
		{ 
			get { return _obstacle; } 
			set { _obstacle = value; }
		}
		
		public PathIntersection(DetectableObject obstacle)
		{
			_obstacle = obstacle;
			_intersect = false;
			_distance = float.MaxValue;
		}
	};	
	#endregion
	
	#region Private fields
	[SerializeField]
	float _avoidanceForceFactor = 0.75f;

	//距离障碍物的最短时间，防止离得太近；
	[SerializeField]
	float _minTimeToCollision = 2;

	#endregion
	
	
	public override bool IsPostProcess 
	{ 
		get { return true; }
	}
	
	
	#region Public properties

	public float AvoidanceForceFactor {
		get {
			return this._avoidanceForceFactor;
		}
		set {
			_avoidanceForceFactor = value;
		}
	}
	
	/// <summary>
	/// Minimum time to collision to consider
	/// </summary>
	public float MinTimeToCollision {
		get {
			return this._minTimeToCollision;
		}
		set {
			_minTimeToCollision = value;
		}
	}
	#endregion
	

	protected override Vector3 CalculateForce()
	{
		Vector3 avoidance = Vector3.zero;
		if (Vehicle.Radar.Obstacles == null || !Vehicle.Radar.Obstacles.Any())
		{
			return avoidance;
		}
		
		PathIntersection nearest = new PathIntersection(null);
		/*
		 * While we could just calculate movement as (Velocity * predictionTime) 
		 * and save ourselves the substraction, this allows other vehicles to
		 * override PredictFuturePosition for their own ends.
		 */
		Vector3 futurePosition = Vehicle.PredictFutureDesiredPosition(_minTimeToCollision);
		Vector3 movement = futurePosition - Vehicle.Position;
		
		
		
		// test all obstacles for intersection with my forward axis,
		// select the one whose point of intersection is nearest
		Profiler.BeginSample("Find nearest intersection");
		foreach (var o in Vehicle.Radar.Obstacles)
		{
			var obj = o as DetectableObject;
			PathIntersection next = FindNextIntersection(Vehicle.Position, futurePosition, obj);
			if (!nearest.Intersect ||
			    (next.Intersect &&
			 next.Distance < nearest.Distance))
			{
				nearest = next;
			}
		}
		Profiler.EndSample();
		
		
		// when a nearest intersection was found
		Profiler.BeginSample("Calculate avoidance");
		if (nearest.Intersect &&
		    nearest.Distance < movement.magnitude)
		{
		
			
			// compute avoidance steering force: take offset from obstacle to me,
			// take the component of that which is lateral (perpendicular to my
			// movement direction),  add a bit of forward component
			Vector3 offset = Vehicle.Position - nearest.Obstacle.Position;
			Vector3 moveDirection = movement.normalized;
			avoidance =	 OpenSteerUtility.perpendicularComponent(offset, moveDirection);
			
			avoidance.Normalize();
			
			#if ANNOTATE_AVOIDOBSTACLES
			Debug.DrawLine(Vehicle.Position, Vehicle.Position + avoidance, Color.white);
			#endif
			
			avoidance += moveDirection * Vehicle.MaxForce * _avoidanceForceFactor;
			
			#if ANNOTATE_AVOIDOBSTACLES
			Debug.DrawLine(Vehicle.Position, Vehicle.Position + avoidance, Color.yellow);
			#endif
		}
		Profiler.EndSample();
		
		return avoidance;
	}

	public PathIntersection FindNextIntersection(Vector3 vehiclePosition, 
	                                                       Vector3 futureVehiclePosition, DetectableObject obstacle) {
		var intersection = new PathIntersection(obstacle);
		
		float combinedRadius = Vehicle.ScaledRadius + obstacle.ScaledRadius;
		var movement = futureVehiclePosition - vehiclePosition;
		var direction = movement.normalized;
		
		var vehicleToObstacle = obstacle.Position - vehiclePosition;
		
		// this is the length of vehicleToObstacle projected onto direction
		float projectionLength = Vector3.Dot(direction, vehicleToObstacle);
		
		// if the projected obstacle center lies further away than our movement + both radius, we're not going to collide
		if (projectionLength > movement.magnitude + combinedRadius) {
			//print("no collision - 1");
			return intersection;
		}
		
		// the foot of the perpendicular
		var projectedObstacleCenter = vehiclePosition + projectionLength * direction;
		
		// distance of the obstacle to the pathe the vehicle is going to take
		float obstacleDistanceToPath = (obstacle.Position - projectedObstacleCenter).magnitude;
		//print("obstacleDistanceToPath: " + obstacleDistanceToPath);
		
		// if the obstacle is further away from the movement, than both radius, there's no collision
		if (obstacleDistanceToPath > combinedRadius) {
			//print("no collision - 2");
			return intersection;
		}
		// if the projected obstacle center lies opposite to the movement direction (aka "behind")
		if (projectionLength < 0) {
			// behind and further away than both radius -> no collision (we already passed)
			if (vehicleToObstacle.magnitude > combinedRadius)
				return intersection;
			
			var intersectionPoint = projectedObstacleCenter - direction ;
			intersection.Intersect = true;
			intersection.Distance = (intersectionPoint - vehiclePosition).magnitude;
			return intersection;
		}
		
		// calculate both intersection points
		var intersectionPoint1 = projectedObstacleCenter - direction ;
		var intersectionPoint2 = projectedObstacleCenter + direction ;
		
		// pick the closest one
		float intersectionPoint1Distance = (intersectionPoint1 - vehiclePosition).magnitude;
		float intersectionPoint2Distance = (intersectionPoint2 - vehiclePosition).magnitude;
		
		intersection.Intersect = true;
		intersection.Distance = Mathf.Min(intersectionPoint1Distance, intersectionPoint2Distance);
		
		return intersection;
		


		}
}
