var lineMaterial : Material;
var textureScale = 4.0;
private var selectionLine : VectorLine;
private var originalPos : Vector2;

function Start () {
	selectionLine = new VectorLine("Selection", new Vector2[5], Color.white, lineMaterial, 4.0, 0.0, 0, LineType.Continuous, Joins.Open);
}

function OnGUI () {
	GUI.Label(Rect(10, 10, 300, 25), "Click & drag to make a selection box");
}

function Update () {
	if (Input.GetMouseButtonDown(0)) {
		originalPos = Input.mousePosition;
	}
	if (Input.GetMouseButton(0)) {
		Vector.MakeRectInLine (selectionLine, originalPos, Input.mousePosition);
		Vector.DrawLine (selectionLine);
	}
	
	Vector.SetTextureScale (selectionLine, textureScale, -Time.time*2.0 % 1);
}