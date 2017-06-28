// ©2010 Starscene Software. All rights reserved. Redistribution without permission not allowed.
import UnityEngine.GUILayout;
import System.IO;

class Vector3Pair extends System.ValueType {	// struct
	var p1 : Vector3;
	var p2 : Vector3;

	function Vector3Pair (point1 : Vector3, point2 : Vector3) {
		p1 = point1;
		p2 = point2;
	}
}

class LineMaker extends ScriptableWizard {
	var useCsharp = false;
	var objectTransform : Transform;
	var pointObjects : GameObject[];
	var lines : Array;
	var linePoints : Array;
	static var pointScale = .3;
	static var lineScale = .15;
	static var oldPointScale = .3;
	static var oldLineScale = .15;
	var message = "";
	var linePositions : Array;
	var lineRotations : Array;
	var objectMaterial : Material;
	var originalMaterial : Material;
	var pointMaterial : Material;
	var lineMaterial : Material;
	var objectMesh : Mesh;
	var initialized = false;
	var focusTemp = false;
	var idx : int;
	var endianDiff1 : int;
	var endianDiff2 : int;
	var canUseVector2 = true;
	var floatSuffix = "";
	var newPrefix = "";
	
	@MenuItem ("Assets/Line Maker... %l")
	static function CreateWizard () {
		var object = Selection.activeObject as GameObject;
		if (!object) {
			EditorUtility.DisplayDialog("No object selected", "Please select an object", "Cancel");
			return;
		}
		var mf = object.GetComponentInChildren(MeshFilter) as MeshFilter;
		if (!mf) {
			EditorUtility.DisplayDialog("No MeshFilter present", "Object must have a MeshFilter component", "Cancel");
			return;
		}
		var objectMesh = mf.sharedMesh as Mesh;
		if (!objectMesh) {
			EditorUtility.DisplayDialog("No mesh present", "Object must have a mesh", "Cancel");
			return;
		}
		if (objectMesh.vertexCount > 500) {
			EditorUtility.DisplayDialog("Too many vertices", "Please select a low-poly object", "Cancel");
			return;
		}
		
		ScriptableWizard.DisplayWizard("Line Maker", LineMaker);
	}
	
	// Initialize this way so we don't have to use static vars, so multiple wizards can be used at once with different objects
	function OnWizardUpdate () {
		if (!initialized) {
			Initialize();
		}
	}
	
	function Initialize () {
		System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
		var object : GameObject = Selection.activeObject;
		objectMesh = (object.GetComponentInChildren(MeshFilter) as MeshFilter).sharedMesh;	
		originalMaterial = object.GetComponentInChildren(Renderer).sharedMaterial;
		objectMaterial = new Material(Shader.Find("Transparent/Diffuse"));
		objectMaterial.color.a = .8;
		object.GetComponentInChildren(Renderer).material = objectMaterial;
		pointMaterial = new Material(Shader.Find("VertexLit"));
		pointMaterial.color = Color.blue;
		pointMaterial.SetColor("_Emission", Color.blue);
		pointMaterial.SetColor("_SpecColor", Color.blue);
		lineMaterial = new Material(Shader.Find("VertexLit"));
		lineMaterial.color = Color.green;
		lineMaterial.SetColor("_Emission", Color.green);
		lineMaterial.SetColor("_SpecColor", Color.green);
		
		var meshVertices = objectMesh.vertices;
		// Remove duplicate vertices
		var meshVerticesArray = new Array(meshVertices);
		for (i = 0; i < meshVerticesArray.length-1; i++) {
			var j = i+1;
			while (j < meshVerticesArray.length) {
				if (meshVerticesArray[i] == meshVerticesArray[j]) {
					meshVerticesArray.RemoveAt(j);
				}
				else {
					j++;
				}
			}
		}
		meshVertices = meshVerticesArray.ToBuiltin(Vector3);
		
		// See if z is substantially different on any points, in which case the option to generate a Vector2 array will not be available
		var zCoord = System.Math.Round(meshVertices[0].z, 3);
		for (i = 1; i < meshVertices.Length; i++) {
			if (!Mathf.Approximately(System.Math.Round(meshVertices[i].z, 3), zCoord)) {
				canUseVector2 = false;
				break;
			}
		}

		// Create the blue point sphere widgets
		objectTransform = object.transform;
		var objectMatrix = objectTransform.localToWorldMatrix;
		pointObjects = new GameObject[meshVertices.Length];
		for (i = 0; i < pointObjects.Length; i++) {
			pointObjects[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			pointObjects[i].transform.position = objectMatrix.MultiplyPoint3x4(meshVertices[i]);
			pointObjects[i].transform.parent = objectTransform;
			pointObjects[i].transform.localScale = Vector3(pointScale, pointScale, pointScale);
			pointObjects[i].renderer.sharedMaterial = pointMaterial;
		}
		
		lines = new Array();
		linePoints = new Array();
		linePositions = new Array();
		lineRotations = new Array();
		endianDiff1 = System.BitConverter.IsLittleEndian? 0 : 3;
		endianDiff2 = System.BitConverter.IsLittleEndian? 0 : 1;
		Selection.objects = new UnityEngine.Object[0];	// Deselect object so it's not in the way as much
		if (useCsharp) {
			floatSuffix = "f";
			newPrefix = "new ";
		}
		initialized = true;
	}
	
	function OnFocus () {
		for (i = 0; i < lines.length; i++) {
			var line : GameObject = lines[i];
			// Make sure line segment is where it's supposed to be in case it was moved
			if (line) {
				line.transform.position = linePositions[i];
				line.transform.rotation = lineRotations[i];
			}
			// But if it's null, then the user must have trashed it, so delete the line
			else {
				DeleteLine(i);
			}
		}
	}

	new function OnGUI () {
		Label("Object: \"" + objectTransform.name + "\"");
		Space(10);
	
		BeginHorizontal();
			Label("Point size: ", Width(60));
			pointScale = HorizontalSlider(pointScale, .05, 1.0);
			if (pointScale != oldPointScale) {
				SetPointScale();
			}
		EndHorizontal();
		BeginHorizontal();
			Label("Line size: ", Width(60));
			lineScale = HorizontalSlider(lineScale, .025, .5);
			if (lineScale != oldLineScale) {
				SetLineScale();
			}
		EndHorizontal();
		Space(10);
	
		BeginHorizontal();
			var buttonWidth = Screen.width/2 - 6;
			if (Button("Connect All Points", Width(buttonWidth))) {
				message = "";
				if (objectMesh.triangles.Length == 0) {
					message = "No triangles exist...must connect points manually";
				}
				else {
					DeleteAllLines();
					ConnectAllPoints();
				}
			}
			if (Button("Delete All Line Segments", Width(buttonWidth))) {
				message = "";
				DeleteAllLines();
			}
		EndHorizontal();
		Space(5);
	
		BeginHorizontal();
			if (Button("Make Line Segment", Width(buttonWidth))) {
				message = "";
				selectionIDs = Selection.instanceIDs;
				if (selectionIDs.Length == 2) {
					if (CheckPointID(selectionIDs[0]) && CheckPointID(selectionIDs[1])) {
						CreateLine();
					}
					else {
						message = "Must select vertex points from this object";
					}
				}
				else {
					message = "Must have two points selected";
				}
			}
			if (Button("Delete Line Segments", Width(buttonWidth))) {
				message = "";
				selectionIDs = Selection.instanceIDs;
				if (selectionIDs.Length == 0) {
					message = "Must select line segment(s) to delete";
				}
				for (i = 0; i < selectionIDs.Length; i++) {
					var lineNumber = CheckLineID(selectionIDs[i]);
					if (lineNumber != -1) {
						DeleteLine (lineNumber);
					}
					else {
						message = "Only lines from this object can be deleted";
					}
				}
			}
		EndHorizontal();
		Space(10);

		Label(message);
		Space(10);
		
		if (canUseVector2) {
			BeginHorizontal();
		}
		if (Button("Generate Complete Line")) {
			message = "";
			ComputeLine(true);
		}
		if (canUseVector2) {
			if (Button("Vector2", Width(100))) {
				ComputeLine(false);
			}
			EndHorizontal();
		}
		Space(2);
		
		if (canUseVector2) {
			BeginHorizontal();
		}
		if (Button("Write Complete Line to File")) {
			WriteLineToFile(true);
		}
		if (canUseVector2) {
			if (Button("Vector2", Width(100))) {
				WriteLineToFile(false);
			}
			EndHorizontal();
		}
	}
	
	function CheckPointID (thisID : int) : boolean {
		for (i = 0; i < pointObjects.Length; i++) {
			if (pointObjects[i].GetInstanceID() == thisID) {
				return true;
			}
		}
		return false;
	}

	function CheckLineID (thisID : int) : int {
		for (i = 0; i < lines.length; i++) {
			if ((lines[i] as GameObject).GetInstanceID() == thisID) {
				return i;
			}
		}
		return -1;
	}
	
	function CreateLine () {
		var selectedObjects = Selection.gameObjects;
		Line (selectedObjects[0].transform.position, selectedObjects[1].transform.position);
	}
	
	function Line (p1 : Vector3, p2 : Vector3) {
		// Make a cube midway between the two points, scaled and rotated so it connects them
		var line = GameObject.CreatePrimitive(PrimitiveType.Cube);
		line.transform.position = Vector3( (p1.x + p2.x)/2, (p1.y + p2.y)/2, (p1.z + p2.z)/2 );
		line.transform.localScale = Vector3(lineScale, lineScale, Vector3.Distance(p1, p2));
		line.transform.LookAt(p1);
		line.transform.parent = objectTransform;
		line.renderer.sharedMaterial = lineMaterial;
		lines.Add(line);
		linePoints.Add(Vector3Pair(p1, p2));
		linePositions.Add(line.transform.position);
		lineRotations.Add(line.transform.rotation);
	}
	
	function DeleteLine (lineID : int) {
		message = "";
		var thisLine = lines[lineID];
		lines.RemoveAt(lineID);
		linePoints.RemoveAt(lineID);
		linePositions.RemoveAt(lineID);
		lineRotations.RemoveAt(lineID);
		if (thisLine) DestroyImmediate(thisLine);
	}
	
	function ConnectAllPoints () {
		var meshTris = objectMesh.triangles;
		var meshVertices = objectMesh.vertices;
		var objectMatrix = objectTransform.localToWorldMatrix;
		var pairs = new Hashtable();
		
		for (i = 0; i < meshTris.Length; i += 3) {
			var p1 = meshVertices[meshTris[i]];
			var p2 = meshVertices[meshTris[i+1]];
			CheckPoints(pairs, p1, p2, objectMatrix);
			
			p1 = meshVertices[meshTris[i+1]];
			p2 = meshVertices[meshTris[i+2]];
			CheckPoints(pairs, p1, p2, objectMatrix);
			
			p1 = meshVertices[meshTris[i+2]];
			p2 = meshVertices[meshTris[i]];
			CheckPoints(pairs, p1, p2, objectMatrix);
		}
	}

	function CheckPoints (pairs : Hashtable, p1 : Vector3, p2 : Vector3, objectMatrix : Matrix4x4) {
		// Only add a line if the two points haven't been connected yet, so there are no duplicate lines
		var pair1 = Vector3Pair(p1, p2);
		var pair2 = Vector3Pair(p2, p1);
		if (pairs[pair1] == null && pairs[pair2] == null) {
			pairs[pair1] = true;
			pairs[pair2] = true;
			Line (objectMatrix.MultiplyPoint3x4(p1), objectMatrix.MultiplyPoint3x4(p2));
		}
	}
	
	function DeleteAllLines () {
		if (!lines) return;
		
		for (var line : GameObject in lines) {
			DestroyImmediate(line);
		}
		lines = new Array();
		linePoints = new Array();
		linePositions = new Array();
		lineRotations = new Array();
	}
	
	function ComputeLine (doVector3 : boolean) {
		if (lines.length < 1) {
			message = "No lines present";
			return;
		}

		var output = "";
		for (i = 0; i < linePoints.length; i++) {
			var v : Vector3Pair = linePoints[i];
			var p1 = Vector3Round(objectTransform.InverseTransformPoint(v.p1));
			var p2 = Vector3Round(objectTransform.InverseTransformPoint(v.p2));
			if (doVector3) {
				output += newPrefix + "Vector3(" + p1.x + floatSuffix + ", " + p1.y + floatSuffix + ", " + p1.z + floatSuffix + "), "
						+ newPrefix + "Vector3(" + p2.x + floatSuffix + ", " + p2.y + floatSuffix + ", " + p2.z + floatSuffix + ")";
			}
			else {
				output += newPrefix + "Vector2(" + p1.x + floatSuffix + ", " + p1.y + floatSuffix + "), "
						+ newPrefix + "Vector2(" + p2.x + floatSuffix + ", " + p2.y + floatSuffix + ")";
			}
			if (i < linePoints.length-1) {
				output += ", ";
			}
		}
		EditorGUIUtility.systemCopyBuffer = output;
		message = "Vector line sent to copy buffer. Please paste into a script now.";
	}
	
	function Vector3Round (p : Vector3) : Vector3 {
		// Round to 3 digits after the decimal so we don't get annoying and unparseable floating point values like -5E-05
		return Vector3(System.Math.Round(p.x, 3), System.Math.Round(p.y, 3), System.Math.Round(p.z, 3));
	}
	
	function WriteLineToFile (doVector3 : boolean) {
		if (lines.length < 1) {
			message = "No lines present";
			return;
		}

		var path = EditorUtility.SaveFilePanelInProject("Save " + (doVector3? "Vector3" : "Vector2") + " Line", objectTransform.name+"Vector.bytes", "bytes", 
														"Please enter a file name for the line data");
		if (path == "") return;

		var fileBytes = new byte[(doVector3? linePoints.length*24 : linePoints.length*16)];
    	idx = 0;
		for (i = 0; i < linePoints.length; i++) {
			var v : Vector3Pair = linePoints[i];
			var p = objectTransform.InverseTransformPoint(v.p1);
			ConvertFloatToBytes (p.x, fileBytes);
			ConvertFloatToBytes (p.y, fileBytes);
			if (doVector3) {
				ConvertFloatToBytes (p.z, fileBytes);
			}
			p = objectTransform.InverseTransformPoint(v.p2);
			ConvertFloatToBytes (p.x, fileBytes);
			ConvertFloatToBytes (p.y, fileBytes);
			if (doVector3) {
				ConvertFloatToBytes (p.z, fileBytes);
			}
		}

		try {
			File.WriteAllBytes(path, fileBytes);
			AssetDatabase.Refresh();
		}
		catch (err) {
			message = err.Message;
			return;
		}
		message = "File written successfully";
	}
	
	function ConvertFloatToBytes (f : float, bytes : byte[]) {
		var floatBytes = System.BitConverter.GetBytes(f);
		bytes[idx++] = floatBytes[    endianDiff1];
		bytes[idx++] = floatBytes[1 + endianDiff2];
		bytes[idx++] = floatBytes[2 - endianDiff2];
		bytes[idx++] = floatBytes[3 - endianDiff1];
	}
	
	function SetPointScale () {
		oldPointScale = pointScale;
		for (po in pointObjects) {
			po.transform.localScale = Vector3(pointScale, pointScale, pointScale);
		}
	}

	function SetLineScale () {
		oldLineScale = lineScale;
		for (var line : GameObject in lines) {
			line.transform.localScale = Vector3(lineScale, lineScale, line.transform.localScale.z);
		}
	}

	new function OnDestroy () {
		for (po in pointObjects) {
			DestroyImmediate(po);
		}
		DeleteAllLines();
		objectTransform.GetComponentInChildren(Renderer).material = originalMaterial;
		DestroyImmediate(objectMaterial);
		DestroyImmediate(pointMaterial);
		DestroyImmediate(lineMaterial);
	}
}