// For touchscreen devices -- draw a line with your finger

var lineMaterial : Material;
var maxPoints = 300;
var lineWidth = 4.0;
var minPixelMove = 5;	// Must move at least this many pixels per sample for a new segment to be recorded
private var linePoints : Vector2[];
private var line : VectorLine;
private var touch : Touch;
private var lineIndex = 0;
private var previousPosition : Vector2;
private var sqrMinPixelMove : int;
private var canDraw = false;

function Start () {
	if (maxPoints%2 != 0) maxPoints++;	// No odd numbers
	linePoints = new Vector2[maxPoints];
	line = new VectorLine("DrawnLine", linePoints, lineMaterial, lineWidth);
	sqrMinPixelMove = minPixelMove*minPixelMove;
}

function Update () {
	if (Input.touchCount > 0) {
		touch = Input.GetTouch(0);
		if (touch.phase == TouchPhase.Began) {
			Vector.ZeroPointsInLine(line);
			previousPosition = touch.position;
			lineIndex = 0;
			Vector.DrawLine(line);
			canDraw = true;
		}
		else if (touch.phase == TouchPhase.Moved && (touch.position - previousPosition).sqrMagnitude > sqrMinPixelMove && canDraw) {
			linePoints[lineIndex++] = previousPosition;
			linePoints[lineIndex++] = touch.position;
			previousPosition = touch.position;
			if (lineIndex >= maxPoints) canDraw = false;
			Vector.DrawLine(line);
		}
	}
}