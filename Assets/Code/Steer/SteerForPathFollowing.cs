using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SteerForPathFollowing : Steering {

	//[SerializeField]
	//Vector3 Point1;

	
	private int _pathIndex = 0;
	private double _pathThreshold = 5;
	private bool _loop=true;

	IList<Vector3> way=new List<Vector3>(5);

	public IList<Vector3> makePath(){
		way = new List<Vector3> (5);
		way.Add(new Vector3(50,0,50));
		way.Add (new Vector3 (25, 0, 75));
		way.Add (new Vector3 (75, 0, 75));
		way.Add (new Vector3 (25, 0, 25));
		way.Add (new Vector3 (75, 0, 25));
		
		return way;
	}



	protected override Vector3 CalculateForce ()
	{
		way = makePath ();
		if (way.Count==0)
		{
			return Vector3.zero;
		}
		Vector3 wayPoint = way[_pathIndex];
		if (wayPoint==null)
		{
			return Vector3.zero;;
		}
		if ((Vehicle.Position-wayPoint).magnitude<_pathThreshold)
		{
			if (_pathIndex>=way.Count-1)
			{
				if (_loop)
				{
					_pathIndex = 0;
				}
				
			}
			else
			{
				_pathIndex++;
			}
		}
		if (_pathIndex>=way.Count-1&&!_loop)
		{
		return 	Vehicle.GetArriveVector(wayPoint,Vehicle.Decelerate.fast);
		}
		else
		{
		return 	Vehicle.GetSeekVector(wayPoint);
		}
		
		}
}
