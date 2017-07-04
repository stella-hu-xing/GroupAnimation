//#define DEBUG_DRAWNEIGHBORS
using UnityEngine;
using UnitySteer;
using UnitySteer.Events;


/// <summary>
///有群组行为的智能体的相关力计算
///三种行为的权重 
/// 
/// public float separationRadius =   5;
/// public float separationAngle  = 135;
/// public float separationWeight =  12;
/// 
/// public float alignmentRadius =    7.5f;
/// public float alignmentAngle  =   45;
/// public float alignmentWeight =    8;
/// 
/// public float cohesionRadius  =    9;
/// public float cohesionAngle   =   99;
/// public float cohesionWeight  =    8;   
/// </remarks>
[AddComponentMenu("UnitySteer/Steer/... for Neighbors")]
public abstract class SteerForNeighbors : Steering
{
	#region Private properties
	[SerializeField]
	float _minRadius = 3f;
	[SerializeField]
	float _maxRadius = 7.5f;
	[SerializeField]
	float _angleCos = 0.7f;	
	[SerializeField]
	LayerMask _layersChecked;
	
	#endregion
	
	
	#region Public properties
	/// <summary>
	/// 最大角

	public float AngleCos {
		get {
			return this._angleCos;
		}
		set {
			_angleCos = Mathf.Clamp(value, -1.0f, 1.0f);
		}
	}
	
	/// <summary>
	/// Degree accessor for the angle
	/// </summary>
	/// <remarks>The cosine is actually used in calculations for performance reasons</remarks>
    public float AngleDegrees
    {
        get
        {
            return OpenSteerUtility.DegreesFromCos(_angleCos);;
        }
        set
        {
            _angleCos = OpenSteerUtility.CosFromDegrees(value);
        }
    }	
	
	/// <summary>
	/// Indicates the vehicles on which layers are evaluated on this behavior
	/// </summary>	
	public LayerMask LayersChecked {
		get {
			return this._layersChecked;
		}
		set {
			_layersChecked = value;
		}
	}

	/// <summary>
	/// 最小角
	/// </summary>
	public float MinRadius {
		get {
			return this._minRadius;
		}
		set {
			_minRadius = value;
		}
	}	
	
	/// <summary>
	/// Maximum neighborhood radius
	/// </summary>
	public float MaxRadius {
		get {
			return this._maxRadius;
		}
		set {
			_maxRadius = value;
		}
	}		
	#endregion	
	
	
	#region Methods
	protected override Vector3 CalculateForce ()
	{
		// steering accumulator and count of neighbors, both initially zero
		Vector3 steering = Vector3.zero;
		int neighbors = 0;
		
		
        for (int i = 0; i < Vehicle.Radar.Vehicles.Count; i++) {
            var other  = Vehicle.Radar.Vehicles[i];
			//目标对象不为空，层检测对象也不为空，并且周围有邻居
			if (!other.GameObject.Equals(null) &&(1 << other.GameObject.layer & LayersChecked) != 0 &&Vehicle.IsInNeighborhood(other, MinRadius, MaxRadius, AngleCos)) 
			{
				#if DEBUG_DRAWNEIGHBORS
				Debug.DrawLine(Vehicle.Position, other.Position, Color.magenta);
				#endif
				steering += CalculateNeighborContribution(other);				
				neighbors++;
			}
		};


		// divide by neighbors, then normalize to pure direction
		if (neighbors > 0) {
			steering = (steering / (float)neighbors);
		
		//	steering.Normalize();
		}
		
		return steering;
	}
	
	protected abstract Vector3 CalculateNeighborContribution(Vehicle other);
	#endregion
	
}
