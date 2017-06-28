private var selectionLine : VectorLine;
private var originalPos : Vector2;
private var lineColors : Color[];

function Start () {
	lineColors = new Color[4];
	selectionLine = new VectorLine("Selection", new Vector2[5], Color.white, null, 3.0, 1.5, 0, LineType.Continuous, Joins.Open);
}

function OnGUI () {
	GUI.Label(Rect(10, 10, 300, 25), "Click & drag to make a selection box");
}

function Update () {
	if (Input.GetMouseButtonDown(0)) {
		StopCoroutine ("CycleColor");
		Vector.SetColor (selectionLine, Color.white);
		originalPos = Input.mousePosition;
	}
	if (Input.GetMouseButton(0)) {
		Vector.MakeRectInLine (selectionLine, originalPos, Input.mousePosition);
		Vector.DrawLine (selectionLine);
	}
	if (Input.GetMouseButtonUp(0)) {
		StartCoroutine ("CycleColor");
	}
}

function CycleColor () {
	while (true) {
		for (i = 0; i < 4; i++) {
			lineColors[i] = Color.Lerp (Color.yellow, Color.red, Mathf.PingPong((Time.time+i*.25)*3.0, 1.0));
		}
		Vector.SetColors (selectionLine, lineColors);
		yield;
	}
}