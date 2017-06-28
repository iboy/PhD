// *******NOTE*******
// This may not work with Unity 2.6.1!
// A bug in TextAsset.bytes sometimes results in corrupted data.
// It will always work in Unity iPhone and Unity 3.
// See Simple3D.js for another way of doing this that doesn't use TextAsset.bytes.

var vectorCube : TextAsset;

function Start () {
	// Make a Vector3 array from the data stored in the vectorCube text asset
	// Try using different assets from the Vectors folder for different shapes (the collider will still be a cube though!)
	var cubePoints = Vector.BytesToVector3Array (vectorCube.bytes);
	
	// Make a line using the above points and material, with a width of 2 pixels
	var line = new VectorLine("Line", cubePoints, null, 2.0);
	
	// Make this transform have the vector line object defined above, where it's always drawn every frame and doesn't use Brightness.Fog
	// This object is a rigidbody, so the vector object will do exactly what this object does
	VectorManager.ObjectSetup (gameObject, line, Visibility.Always, Brightness.Normal);
}