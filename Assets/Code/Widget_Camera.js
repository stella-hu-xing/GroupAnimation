//Widget_Camera.js: A script to control the camera and make it smoothly follow widget.
// The object we want to follow and look at . 
var target : Transform;

// The distance for X and Z for the camera to stay from the target
var distance = 10.0;
// the distance in Y for the camera to stay from the target
var height = 5.0;

// Speed controls for the camera - how fast it catches up to the moving object
var heightDamping = 2.0;
var rotationDamping = 3.0;
var distanceDampingX = 0.5;
var distanceDampingZ = 0.2;

//The camera controls for looking at the target
var camSpeed = 2.0;
var smoothed = true;


function LateUpdate () {
	// Check to make sure a target has been assigned in Inspector
	if (!target)
		return;
	
	// Calculate the current rotation angles, positions, and where we want the camera to end up
	wantedRotationAngle = target.eulerAngles.y;
	wantedHeight = target.position.y + height;
	wantedDistanceZ = target.position.z - distance;
	wantedDistanceX = target.position.x - distance;
	
	currentRotationAngle = transform.eulerAngles.y;
	currentHeight = transform.position.y;
	currentDistanceZ = transform.position.z;
	currentDistanceX = transform.position.x;
	

	// Damp the rotation around the y-axis
	currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
	
	// Damp the distance
	currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
	currentDistanceZ = Mathf.Lerp(currentDistanceZ, wantedDistanceZ, distanceDampingZ * Time.deltaTime);
	currentDistanceX = Mathf.Lerp(currentDistanceX, wantedDistanceX, distanceDampingX * Time.deltaTime);

	// Convert the angle into a rotation
	currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
	
	// Set the new position of the camera
	transform.position -= currentRotation * Vector3.forward * distance ;
	transform.position.x = currentDistanceX;
	transform.position.z = currentDistanceZ; 
	transform.position.y = currentHeight;
	
	// Make sure the camera is always looking at the target
	LookAtMe();
}


function LookAtMe(){
		//check  whether we want the camera to be smoothed or not - can be changed in the Inspector
		if(smoothed)
		 {
			//Find the new rotation value based upon the target and camera's current position.  Then interpolate
			//smoothly between the two using the specified speed setting
			var camRotation = Quaternion.LookRotation(target.position - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, camRotation, Time.deltaTime * camSpeed);
		}
		//This default will flatly move with the targeted object
		else{
			transform.LookAt(target);
		}
	}

@script AddComponentMenu("Player/Smooth Follow Camera")