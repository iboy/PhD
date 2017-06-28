// See Simple3D 2.js for another way of doing this that uses TextAsset.bytes instead.
// Warning: TextAsset.bytes is bugged in Unity 2.6.1. The method below always works
// (although it is a bit ugly!).

function Start () {
	// Make a Vector3 array that contains points for a cube that's 1 unit in size
	var cubePoints = [Vector3(-0.5, -0.5, 0.5), Vector3(0.5, -0.5, 0.5), Vector3(-0.5, 0.5, 0.5), Vector3(-0.5, -0.5, 0.5), Vector3(0.5, -0.5, 0.5), Vector3(0.5, 0.5, 0.5), Vector3(0.5, 0.5, 0.5), Vector3(-0.5, 0.5, 0.5), Vector3(-0.5, 0.5, -0.5), Vector3(-0.5, 0.5, 0.5), Vector3(0.5, 0.5, 0.5), Vector3(0.5, 0.5, -0.5), Vector3(0.5, 0.5, -0.5), Vector3(-0.5, 0.5, -0.5), Vector3(-0.5, -0.5, -0.5), Vector3(-0.5, 0.5, -0.5), Vector3(0.5, 0.5, -0.5), Vector3(0.5, -0.5, -0.5), Vector3(0.5, -0.5, -0.5), Vector3(-0.5, -0.5, -0.5), Vector3(-0.5, -0.5, 0.5), Vector3(-0.5, -0.5, -0.5), Vector3(0.5, -0.5, -0.5), Vector3(0.5, -0.5, 0.5)];
	
	// Make a line using the above points and material, with a width of 2 pixels
	var line = new VectorLine("Line", cubePoints, null, 2.0);
	
	// Make this transform have the vector line object defined above, where it's always drawn every frame and doesn't use Brightness.Fog
	// This object is a rigidbody, so the vector object will do exactly what this object does
	VectorManager.ObjectSetup (gameObject, line, Visibility.Always, Brightness.Normal);
}