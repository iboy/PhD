enum Shape {Cube = 0, Sphere = 1}
var shape = Shape.Cube;

function Start () {
	var line = new VectorLine ("Shape", XRayLineData.use.shapePoints[shape], XRayLineData.use.lineMaterial, XRayLineData.use.lineWidth);
	VectorManager.ObjectSetup (gameObject, line, Visibility.Always, Brightness.Normal);
}