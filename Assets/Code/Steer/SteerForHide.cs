using UnityEngine;
using System.Collections;

public class SteerForHide : Steering {

	[SerializeField]
	Vehicle itarget;
     [SerializeField]
	float MaxDis=20;

	private Vector3 GetHidePosition(Vector3 Obspos,float Obsradius,Vehicle target){
		float distanceFrBound = 30.0f;
		float DistAway = Obsradius + distanceFrBound;
		Vector3 ToOb = (Obspos - target.Position).normalized;
		return (ToOb * DistAway) + Obspos;
		}

	protected override Vector3 CalculateForce ()
	{
		float DistToClost=MaxDis;
		Vector3 BestHidePos=Vector3.zero;
		foreach(DetectableObject de in Vehicle.Radar.Obstacles){
			Vector3 HidingPos=GetHidePosition(de.Position,de.Radius,itarget);
			float dist=Vector3.Distance(HidingPos,Vehicle.Position);
			dist=Mathf.Sqrt(dist);
			if(dist<DistToClost){
				DistToClost=dist;
				BestHidePos=HidingPos;
			}
		}
		if (DistToClost == MaxDis) {
			return Vehicle.GetArriveVector(-itarget.Position,Vehicle.Decelerate.fast);
		}
		return Vehicle.GetArriveVector(BestHidePos,Vehicle.Decelerate.fast);
	}
}
