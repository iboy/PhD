// This is the same thing as the Simple3D script, except it draws the line in "real" 3D space, so it can be occluded by other 3D objects and so on.
// In this demo scene, it normally looks exactly the same as Simple3D, but try turning the renderer on for the cube to see the difference.
// Basically there are two additional lines which accomplish this: VectorManager.SetDrawLine3D and Vector.SetLayer.

function Start () {
	// Make a Vector3 array that contains points for a cube that's 1 unit in size
	var cubePoints = [Vector3(-0.5, -0.5, 0.5), Vector3(0.5, -0.5, 0.5), Vector3(-0.5, 0.5, 0.5), Vector3(-0.5, -0.5, 0.5), Vector3(0.5, -0.5, 0.5), Vector3(0.5, 0.5, 0.5), Vector3(0.5, 0.5, 0.5), Vector3(-0.5, 0.5, 0.5), Vector3(-0.5, 0.5, -0.5), Vector3(-0.5, 0.5, 0.5), Vector3(0.5, 0.5, 0.5), Vector3(0.5, 0.5, -0.5), Vector3(0.5, 0.5, -0.5), Vector3(-0.5, 0.5, -0.5), Vector3(-0.5, -0.5, -0.5), Vector3(-0.5, 0.5, -0.5), Vector3(0.5, 0.5, -0.5), Vector3(0.5, -0.5, -0.5), Vector3(0.5, -0.5, -0.5), Vector3(-0.5, -0.5, -0.5), Vector3(-0.5, -0.5, 0.5), Vector3(-0.5, -0.5, -0.5), Vector3(0.5, -0.5, -0.5), Vector3(0.5, -0.5, 0.5)];
	
	// Make a line using the above points and material, with a width of 2 pixels
	var line = new VectorLine("Line", cubePoints, null, 2.0);
	
	// We'll use DrawLine3D, so we'll also use SetCamera3D
	// This isn't really necessary, but it avoids creating the VectorCam object, which is a small but useful optimization,
	// as long as you don't need to draw 2D lines
	Vector.SetCamera3D();
	
	// Make this transform have the vector line object defined above, where it's always drawn every frame and doesn't use Brightness.Fog
	// This object is a rigidbody, so the vector object will do exactly what this object does
	VectorManager.useDrawLine3D = true;
	VectorManager.ObjectSetup (gameObject, line, Visibility.Always, Brightness.Normal);
}