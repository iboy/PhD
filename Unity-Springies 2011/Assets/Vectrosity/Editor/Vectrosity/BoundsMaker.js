@MenuItem ("Assets/Make Invisible Bounds Mesh...")

static function ExportNormalmap () {
	var object : GameObject = Selection.activeObject as GameObject;
	if (object == null) {
		EditorUtility.DisplayDialog("No object selected", "Please select an object.", "Cancel");
		return;
	}
	if (!object.renderer) {
		EditorUtility.DisplayDialog("No renderer present", "Object must have a renderer.", "Cancel");
		return;
	}
	
	var path = EditorUtility.SaveFilePanelInProject("Save Mesh", object.name+"_bounds.asset", "asset", "");
	if (path == "") return;
	
	var originalPos = object.transform.position;
	var originalRotation = object.transform.rotation;
	var originalScale = object.transform.localScale;
	object.transform.position = Vector3.zero;
	object.transform.rotation = Quaternion.identity;
	object.transform.localScale = Vector3.one;

	var bounds = object.renderer.bounds;
	var mesh = new Mesh();
	var vertices = new Vector3[8];
	vertices[0] = bounds.center + Vector3(-bounds.extents.x,  bounds.extents.y,  bounds.extents.z);
	vertices[1] = bounds.center + Vector3( bounds.extents.x,  bounds.extents.y,  bounds.extents.z);
	vertices[2] = bounds.center + Vector3(-bounds.extents.x,  bounds.extents.y, -bounds.extents.z);
	vertices[3] = bounds.center + Vector3( bounds.extents.x,  bounds.extents.y, -bounds.extents.z);
	vertices[4] = bounds.center + Vector3(-bounds.extents.x, -bounds.extents.y,  bounds.extents.z);
	vertices[5] = bounds.center + Vector3( bounds.extents.x, -bounds.extents.y,  bounds.extents.z);
	vertices[6] = bounds.center + Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
	vertices[7] = bounds.center + Vector3( bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
	mesh.vertices = vertices;
	
	object.transform.position = originalPos;
	object.transform.rotation = originalRotation;
	object.transform.localScale = originalScale;
	
	AssetDatabase.CreateAsset(mesh, path);
	AssetDatabase.Refresh();
}