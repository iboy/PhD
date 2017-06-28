// Version 1.3
// ©2011 Starscene Software. All rights reserved. Redistribution without permission not allowed.

using UnityEngine;

public class Vector : MonoBehaviour {
	static Camera cam;
	static Transform camTransform;
	static Camera cam3D;
	public static Vector3 oldPosition;
	public static Vector3 oldRotation;
	public static int vectorLayer = 31;
	static float zDist;
	static bool useOrthoCam;
	const float cutoff = .15f;
	static bool error = false;
	
	public static void LogError (string errorString) {
		Debug.LogError(errorString);
		error = true;
	}
	
	public static Vector3 CamTransformPosition () {
		return camTransform.position;
	}
	
	public static Vector3 CamTransformEulerAngles () {
		return camTransform.eulerAngles;
	}
	
	public static bool CamTransformExists () {
		if (camTransform) return true;
		return false;
	}

	public static void SetCamera () {
		SetCamera (CameraClearFlags.Depth, false);
	}
	
	public static void SetCamera (bool useOrtho) {
		SetCamera (CameraClearFlags.Depth, useOrtho);
	}
	
	public static void SetCamera (CameraClearFlags clearFlags) {
		SetCamera (clearFlags, false);
	}
	
	public static void SetCamera (CameraClearFlags clearFlags, bool useOrtho) {
		if (Camera.main == null) {
			LogError("Vector.SetCamera: no camera tagged \"Main Camera\" found");
			return;
		}
		SetCamera (Camera.main, clearFlags, useOrtho);
	}
	
	public static void SetCamera (Camera thisCamera) {
		SetCamera (thisCamera, CameraClearFlags.Depth, false);
	}
	
	public static void SetCamera (Camera thisCamera, bool useOrtho) {
		SetCamera (thisCamera, CameraClearFlags.Depth, useOrtho);
	}
	
	public static void SetCamera (Camera thisCamera, CameraClearFlags clearFlags) {
		SetCamera (thisCamera, clearFlags, false);
	}
	
	public static void SetCamera (Camera thisCamera, CameraClearFlags clearFlags, bool useOrtho) {
		if (!cam) {
			cam = new GameObject("VectorCam", typeof(Camera)).camera;
			DontDestroyOnLoad(cam);
		}
		cam.depth = thisCamera.depth+1;
		cam.clearFlags = clearFlags;
		cam.orthographic = useOrtho;
		useOrthoCam = useOrtho;
		if (useOrtho) {
			cam.orthographicSize = Screen.height/2;
			cam.farClipPlane = 101.1f;
			cam.nearClipPlane = .9f;
		}
		else {
			cam.fieldOfView = 90.0f;
			cam.farClipPlane = Screen.height/2 + .0101f;
			cam.nearClipPlane = Screen.height/2 - .0001f;
		}
		cam.transform.position = new Vector3(Screen.width/2 - .5f, Screen.height/2 - .5f, 0.0f);
		cam.transform.eulerAngles = Vector3.zero;
		cam.cullingMask = 1 << vectorLayer;
		cam.backgroundColor = thisCamera.backgroundColor;
		
		thisCamera.cullingMask = thisCamera.cullingMask & (-1 ^ (1 << vectorLayer));
		camTransform = thisCamera.transform;
		cam3D = thisCamera;
		oldPosition = camTransform.position + Vector3.one;
		oldRotation = camTransform.eulerAngles + Vector3.one;
	}
	
	public static void SetCamera3D () {
		if (Camera.main == null) {
			LogError("Vector.SetCamera3D: no camera tagged \"Main Camera\" found");
			return;
		}
		SetCamera3D (Camera.main);
	}
	
	public static void SetCamera3D (Camera thisCamera) {
		camTransform = thisCamera.transform;
		cam3D = thisCamera;
		oldPosition = camTransform.position + Vector3.one;
		oldRotation = camTransform.eulerAngles + Vector3.one;
	}
	
	public static void SetVectorCamDepth (int depth) {
		cam.depth = depth;
	}
	
	static int GetPointsLength (VectorLine vectorLine) {
		return vectorLine.points2 != null? vectorLine.points2.Length : vectorLine.points3.Length;
	}

	static string[] functionNames = {"Vector: SetColors: Length of color", "Vector: SetColorsSmooth: Length of color", "Vector: SetWidths: Length of line widths"};
	enum FunctionName {SetColors, SetColorsSmooth, SetWidths}
	
	static bool WrongArrayLength (VectorLine vectorLine, int arrayLength, FunctionName functionName) {
		int pointsLength = GetPointsLength (vectorLine);
		if (vectorLine.continuousLine) {
			if (arrayLength != pointsLength-1) {
				LogError(functionNames[(int)functionName] + " array for " + vectorLine.vectorObject.name + " must be length of points array minus one for a continuous line (one entry per line segment)");
				return true;
			}
		}
		else if (arrayLength != pointsLength/2) {
			LogError(functionNames[(int)functionName] + " array in " + vectorLine.vectorObject.name + " must be exactly half the length of points array for a discrete line (one entry per line segment)");
			return true;
		}
		return false;
	}
	
	static void NoVertexColorsError (VectorLine vectorLine, string functionName) {
		LogError(functionName + ": the line \"" + vectorLine.vectorObject.name + "\" has no vertex colors. To use this function, lines must be declared with a color or color array.");
	}
	
	public static void SetColor (VectorLine vectorLine, Color color) {
		if (vectorLine.lineColors == null) {
			NoVertexColorsError (vectorLine, "Vector: SetColor");
			return;
		}
		int end = vectorLine.lineColors.Length;
		for (int i = 0; i < end; i++) {
			vectorLine.lineColors[i] = color;
		}
		vectorLine.mesh.colors = vectorLine.lineColors;
	}

	public static void SetColors (VectorLine vectorLine, Color[] lineColors) {
		if (vectorLine.lineColors == null) {
			NoVertexColorsError (vectorLine, "Vector: SetColors");
			return;
		}
		if (!vectorLine.isPoints) {
			if (WrongArrayLength (vectorLine, lineColors.Length, FunctionName.SetColors)) {
				return;
			}
		}
		else if (lineColors.Length != vectorLine.points2.Length) {
			LogError("Vector: SetColors: Length of lineColors array in " + vectorLine.vectorObject.name + " must be same length as points array");
			return;
		}
		
		int idx = 0;
		int end = lineColors.Length;
		for (int i = 0; i < end; i++) {
			vectorLine.lineColors[idx] = lineColors[i];
			vectorLine.lineColors[idx+1] = lineColors[i];
			vectorLine.lineColors[idx+2] = lineColors[i];
			vectorLine.lineColors[idx+3] = lineColors[i];
			idx += 4;
		}
		if (vectorLine.fillJoins) {
			for (int i = 1; i < end; i++) {
				vectorLine.lineColors[idx++] = lineColors[i];
			}
		}
		vectorLine.mesh.colors = vectorLine.lineColors;
	}
	
	public static void SetColorsSmooth (VectorLine vectorLine, Color[] lineColors) {
		if (vectorLine.isPoints) {
			LogError ("Vector: SetColorsSmooth must be used with a line rather than points");
			return;
		}
		if (vectorLine.lineColors.Length == 0) {
			NoVertexColorsError (vectorLine, "Vector: SetColorsSmooth");
			return;
		}
		if (WrongArrayLength (vectorLine, lineColors.Length, FunctionName.SetColorsSmooth)) {
			return;
		}
		
		int idx = 0;
		int end = lineColors.Length;
		vectorLine.lineColors[idx++] = lineColors[0];
		vectorLine.lineColors[idx++] = lineColors[0];
		vectorLine.lineColors[idx++] = lineColors[0];
		vectorLine.lineColors[idx++] = lineColors[0];
		for (int i = 1; i < end; i++) {
			vectorLine.lineColors[idx] = lineColors[i-1];
			vectorLine.lineColors[idx+1] = lineColors[i-1];
			vectorLine.lineColors[idx+2] = lineColors[i];
			vectorLine.lineColors[idx+3] = lineColors[i];
			idx += 4;
		}
		if (vectorLine.fillJoins) {
			for (int i = 1; i < end; i++) {
				vectorLine.lineColors[idx++] = lineColors[i];
			}
		}
		vectorLine.mesh.colors = vectorLine.lineColors;
	}
	
	public static void SetWidths (VectorLine vectorLine, float[] lineWidths) {
		if (lineWidths == null) {
			LogError("Vector: SetWidths: line widths array must not be null");
			return;
		}
		if (vectorLine.isPoints) {
			if (lineWidths.Length != vectorLine.points2.Length) {
				LogError("Vector: SetWidths: line widths array must be the same length as the points array for \"" + vectorLine.vectorObject.name + "\"");
				return;
			}
		}
		else if (WrongArrayLength (vectorLine, lineWidths.Length, FunctionName.SetWidths)) {
			return;
		}
		
		int end = lineWidths.Length;
		vectorLine.lineWidths = new float[end];
		for (int i = 0; i < end; i++) {
			vectorLine.lineWidths[i] = lineWidths[i] * .5f;
		}
	}
	
	static Material lineMaterial;
	static float lineWidth;
	static int lineDepth;
	static float capLength;
	static Color lineColor;
	static LineType lineType;
	static Joins joins;
	static bool set = false;
	static Vector3 v1 = Vector3.zero;
	static Vector3 v2 = Vector3.zero;
	
	public static void SetLineParameters (Color color, Material mat, float width, float cap, int depth, LineType thisLineType, Joins thisJoins) {
		lineColor = color;
		lineMaterial = mat;
		lineWidth = width;
		lineDepth = depth;
		capLength = cap;
		lineType = thisLineType;
		joins = thisJoins;
		set = true;
	}
	
	public static VectorLine MakeLine (string name, Vector3[] points, Color[] colors) {
		if (!set) {
			LogError("Vector: Must call SetLineParameters before using MakeLine with these parameters");
			return null;
		}
		return new VectorLine(name, points, colors, lineMaterial, lineWidth, lineDepth, lineType, joins);
	}

	public static VectorLine MakeLine (string name, Vector2[] points, Color[] colors) {
		if (!set) {
			LogError("Vector: Must call SetLineParameters before using MakeLine with these parameters");
			return null;
		}
		return new VectorLine(name, points, colors, lineMaterial, lineWidth, capLength, lineDepth, lineType, joins);
	}

	public static VectorLine MakeLine (string name, Vector3[] points, Color color) {
		if (!set) {
			LogError("Vector: Must call SetLineParameters before using MakeLine with these parameters");
			return null;
		}
		return new VectorLine(name, points, color, lineMaterial, lineWidth, lineDepth, lineType, joins);
	}

	public static VectorLine MakeLine (string name, Vector2[] points, Color color) {
		if (!set) {
			LogError("Vector: Must call SetLineParameters before using MakeLine with these parameters");
			return null;
		}
		return new VectorLine(name, points, color, lineMaterial, lineWidth, capLength, lineDepth, lineType, joins);
	}

	public static VectorLine MakeLine (string name, Vector3[] points) {
		if (!set) {
			LogError("Vector: Must call SetLineParameters before using MakeLine with these parameters");
			return null;
		}
		return new VectorLine(name, points, lineColor, lineMaterial, lineWidth, lineDepth, lineType, joins);
	}

	public static VectorLine MakeLine (string name, Vector2[] points) {
		if (!set) {
			LogError("Vector: Must call SetLineParameters before using MakeLine with these parameters");
			return null;
		}
		return new VectorLine(name, points, lineColor, lineMaterial, lineWidth, capLength, lineDepth, lineType, joins);
	}

	public static VectorLine SetLine (Color color, params Vector2[] points) {
		if (points.Length < 2) {
			LogError("Vector.SetLine needs at least two points");
			return null;
		}
		VectorLine line = new VectorLine("Line", points, color, null, 1.0f, 0.0f, 0, LineType.Continuous, Joins.Open);
		DrawLine(line);
		return line;
	}
	
	public static VectorLine SetLine (Color color, params Vector3[] points) {
		if (points.Length < 2) {
			LogError("Vector.SetLine needs at least two points");
			return null;
		}
		VectorLine line = new VectorLine("Line", points, color, null, 1.0f, 0, LineType.Continuous, Joins.Open);
		DrawLine(line);
		return line;
	}
	
	public static VectorLine SetLine3D (Color color, params Vector3[] points) {
		if (points.Length < 2) {
			LogError("Vector.SetLine3D needs at least two points");
			return null;
		}
		VectorLine line = new VectorLine("Line", points, color, null, 1.0f, 0, LineType.Continuous, Joins.Open);
		DrawLine3D(line);
		return line;
	}
	
	public static void DrawLine (VectorLine vectorLine) {
		DrawLine(vectorLine, null, false);
	}
	
	public static void DrawLine (VectorLine vectorLine, Transform thisTransform) {
		DrawLine(vectorLine, thisTransform, true);
	}
	
	public static void DrawLine (VectorLine vectorLine, Transform thisTransform, bool useTransformMatrix) {
		if (error) return;
		if (!cam) {
			SetCamera();
			if (!cam) {	// If that didn't work (no camera tagged "Main Camera")
				LogError("Vector.DrawLine: You must call SetCamera before calling DrawLine for " + vectorLine.vectorObject.name);
				return;
			}
		}
	
		Matrix4x4 thisMatrix = useTransformMatrix? thisTransform.localToWorldMatrix : Matrix4x4.identity;
		zDist = useOrthoCam? 101-vectorLine.lineDepth : Screen.height/2 + ((100.0f - vectorLine.lineDepth) * .0001f);
		int idx = 0;
		int end = 0;
		if (vectorLine.points2 != null) {
			end = vectorLine.continuousLine? vectorLine.points2.Length-1 : vectorLine.points2.Length;
			Line2D (vectorLine, end, ref idx, thisMatrix, useTransformMatrix);
		}
		else {
			end = vectorLine.continuousLine? vectorLine.points3.Length-1 : vectorLine.points3.Length;
			if (vectorLine.continuousLine) {
				LineContinuous (vectorLine, end, ref idx, thisMatrix, useTransformMatrix);
			}
			else {
				LineDiscrete (vectorLine, end, ref idx, thisMatrix, useTransformMatrix);
			}
		}
	
		if (vectorLine.fillJoins) {
			end++;
			if (vectorLine.points2 != null) {
				FillContinuous2D (vectorLine, end, ref idx, thisMatrix, useTransformMatrix);
			}
			else {
				FillContinuous (vectorLine, end, ref idx, thisMatrix, useTransformMatrix);
			}
		}
		
		vectorLine.mesh.vertices = vectorLine.lineVertices;
		vectorLine.mesh.RecalculateBounds();
	}
	
	public static void DrawLine3D (VectorLine vectorLine) {
		DrawLine3D (vectorLine, null, false);
	}
	
	public static void DrawLine3D (VectorLine vectorLine, Transform thisTransform) {
		DrawLine3D (vectorLine, thisTransform, true);
	}
	
	public static void DrawLine3D (VectorLine vectorLine, Transform thisTransform, bool useTransformMatrix) {
		if (error) return;
		if (!cam3D) {
			SetCamera3D();
			if (!cam3D) {
				LogError("Vector.DrawLine3D: You must call SetCamera or SetCamera3D before calling DrawLine3D for " + vectorLine.vectorObject.name);
				return;
			}
		}
		if (vectorLine.points3 == null) {
			LogError("Vector: DrawLine3D can only be used with a Vector3 array, which " + vectorLine.vectorObject.name + " doesn't have");
			return;
		}
		
		Matrix4x4 thisMatrix = useTransformMatrix? thisTransform.localToWorldMatrix : Matrix4x4.identity;
		int end = vectorLine.continuousLine? vectorLine.points3.Length-1 : vectorLine.points3.Length;
		int add = vectorLine.continuousLine? 1 : 2;
		int idx = 0;
		int widthIdx = 0;
		int widthIdxAdd = vectorLine.lineWidths.Length == 1? 0 : 1;
		Vector3 pos1 = Vector3.zero;
		Vector3 pos2 = Vector3.zero;
		
		for (int i = 0; i < end; i += add) {
			if (useTransformMatrix) {
				pos1 = cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(vectorLine.points3[i]));
				pos2 = cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(vectorLine.points3[i+1]));
			}
			else {
				pos1 = cam3D.WorldToScreenPoint(vectorLine.points3[i]);
				pos2 = cam3D.WorldToScreenPoint(vectorLine.points3[i+1]);					
			}
			
			v1.x = pos2.y; v1.y = pos1.x;
			v2.x = pos1.y; v2.y = pos2.x;
			Vector3 line = (v1 - v2).normalized;
			Vector3 perpendicular = line * vectorLine.lineWidths[widthIdx];
			vectorLine.lineVertices[idx]   = cam3D.ScreenToWorldPoint(pos1 - perpendicular);
			vectorLine.lineVertices[idx+1] = cam3D.ScreenToWorldPoint(pos1 + perpendicular);
			if (vectorLine.smoothWidth && i < end-add) {
				perpendicular = line * vectorLine.lineWidths[widthIdx+1];
			}
			vectorLine.lineVertices[idx+2] = cam3D.ScreenToWorldPoint(pos2 - perpendicular);
			vectorLine.lineVertices[idx+3] = cam3D.ScreenToWorldPoint(pos2 + perpendicular);
			idx += 4;
			widthIdx += widthIdxAdd;
		}
		
		vectorLine.mesh.vertices = vectorLine.lineVertices;
		vectorLine.mesh.RecalculateBounds();
	}
	
	public static void SetLayer (VectorLine vectorLine, int layer) {
		vectorLine.vectorObject.layer = layer;
	}
	
	static void Line2D (VectorLine vectorLine, int end, ref int idx, Matrix4x4 thisMatrix, bool doTransform) {
		Vector3 p1 = new Vector3(0.0f, 0.0f, 0.0f);
		Vector3 p2 = new Vector3(0.0f, 0.0f, 0.0f);
		int add = vectorLine.continuousLine? 1 : 2;
		int widthIdx = 0;
		int widthIdxAdd = vectorLine.lineWidths.Length == 1? 0 : 1;
		if (vectorLine.capLength == 0.0f) {
			for (int i = 0; i < end; i += add) {
				if (doTransform) {
					p1 = thisMatrix.MultiplyPoint3x4(vectorLine.points2[i]);
					p2 = thisMatrix.MultiplyPoint3x4(vectorLine.points2[i+1]);
				}
				else {
					p1 = vectorLine.points2[i];
					p2 = vectorLine.points2[i+1];					
				}
				p1.z = zDist;
				if (p1.x == p2.x && p1.y == p2.y) {Skip (vectorLine, ref idx, ref p1); continue;}
				p2.z = zDist;
				
				v1.x = p2.y; v1.y = p1.x;
				v2.x = p1.y; v2.y = p2.x;
				Vector3 perpendicular = v1 - v2;
				float normalizedDistance = ( 1.0f / Mathf.Sqrt((perpendicular.x * perpendicular.x) + (perpendicular.y * perpendicular.y)) );
				perpendicular *= normalizedDistance * vectorLine.lineWidths[widthIdx];
				vectorLine.lineVertices[idx]   = p1 - perpendicular;
				vectorLine.lineVertices[idx+1] = p1 + perpendicular;
				if (vectorLine.smoothWidth && i < end-add) {
					perpendicular = v1 - v2;
					perpendicular *= normalizedDistance * vectorLine.lineWidths[widthIdx+1];
				}
				vectorLine.lineVertices[idx+2] = p2 - perpendicular;
				vectorLine.lineVertices[idx+3] = p2 + perpendicular;
				idx += 4;
				widthIdx += widthIdxAdd;
			}
		}
		else {
			for (int i = 0; i < end; i += add) {
				if (doTransform) {
					p1 = thisMatrix.MultiplyPoint3x4(vectorLine.points2[i]);
					p2 = thisMatrix.MultiplyPoint3x4(vectorLine.points2[i+1]);
				}
				else {
					p1 = vectorLine.points2[i];
					p2 = vectorLine.points2[i+1];					
				}
				p1.z = zDist;
				if (p1.x == p2.x && p1.y == p2.y) {Skip (vectorLine, ref idx, ref p1); continue;}
				p2.z = zDist;
				
				Vector3 line = p2 - p1;
				line *= ( 1.0f / Mathf.Sqrt((line.x * line.x) + (line.y * line.y)) );
				p1 -= line * vectorLine.capLength;
				p2 += line * vectorLine.capLength;

				v1.x = line.y; v1.y = -line.x;
				line = v1 * vectorLine.lineWidths[widthIdx];
				vectorLine.lineVertices[idx]   = p1 - line;
				vectorLine.lineVertices[idx+1] = p1 + line;
				if (vectorLine.smoothWidth && i < end-add) {
					line = v1 * vectorLine.lineWidths[widthIdx+1];
				}
				vectorLine.lineVertices[idx+2] = p2 - line;
				vectorLine.lineVertices[idx+3] = p2 + line;
				idx += 4;
				widthIdx += widthIdxAdd;
			}
		}
	}
	
	static void LineContinuous (VectorLine vectorLine, int end, ref int idx, Matrix4x4 thisMatrix, bool doTransform) {
		Vector3 pos2 = doTransform? cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(vectorLine.points3[0])) :
									cam3D.WorldToScreenPoint(vectorLine.points3[0]);
		pos2.z = pos2.z < cutoff? -zDist : zDist;
		int widthIdx = 0;
		int widthIdxAdd = vectorLine.lineWidths.Length == 1? 0 : 1;
		for (int i = 0; i < end; i++) {
			Vector3 pos1 = pos2;
			pos2 = doTransform? cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(vectorLine.points3[i+1])) :
								cam3D.WorldToScreenPoint(vectorLine.points3[i+1]);
			if (pos1.x == pos2.x && pos1.y == pos2.y) {Skip (vectorLine, ref idx, ref pos1); continue;}
			pos2.z = pos2.z < cutoff? -zDist : zDist;
			
			v1.x = pos2.y; v1.y = pos1.x;
			v2.x = pos1.y; v2.y = pos2.x;		
			Vector3 perpendicular = v1 - v2;
			float normalizedDistance = 1.0f / Mathf.Sqrt((perpendicular.x * perpendicular.x) + (perpendicular.y * perpendicular.y));
			perpendicular *= normalizedDistance * vectorLine.lineWidths[widthIdx];
			vectorLine.lineVertices[idx]   = pos1 - perpendicular;
			vectorLine.lineVertices[idx+1] = pos1 + perpendicular;
			if (vectorLine.smoothWidth && i < end-1) {
				perpendicular = v1 - v2;
				perpendicular *= normalizedDistance * vectorLine.lineWidths[widthIdx+1];
			}
			vectorLine.lineVertices[idx+2] = pos2 - perpendicular;
			vectorLine.lineVertices[idx+3] = pos2 + perpendicular;
			idx += 4;
			widthIdx += widthIdxAdd;
		}
	}

	static void LineDiscrete (VectorLine vectorLine, int end, ref int idx, Matrix4x4 thisMatrix, bool doTransform) {
		Vector3 pos1 = Vector3.zero;
		Vector3 pos2 = Vector3.zero;
		int widthIdx = 0;
		int widthIdxAdd = vectorLine.lineWidths.Length == 1? 0 : 1;
		for (int i = 0; i < end; i += 2) {
			if (doTransform) {
				pos1 = cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(vectorLine.points3[i]));
				pos2 = cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(vectorLine.points3[i+1]));
			}
			else {
				pos1 = cam3D.WorldToScreenPoint(vectorLine.points3[i]);
				pos2 = cam3D.WorldToScreenPoint(vectorLine.points3[i+1]);				
			}
			pos1.z = pos1.z < cutoff? -zDist : zDist;
			if (pos1.x == pos2.x && pos1.y == pos2.y) {Skip (vectorLine, ref idx, ref pos1); continue;}
			pos2.z = pos2.z < cutoff? -zDist : zDist;
			
			v1.x = pos2.y; v1.y = pos1.x;
			v2.x = pos1.y; v2.y = pos2.x;		
			Vector3 perpendicular = v1 - v2;
			float normalizedDistance = 1.0f / Mathf.Sqrt((perpendicular.x * perpendicular.x) + (perpendicular.y * perpendicular.y));
			perpendicular *= normalizedDistance * vectorLine.lineWidths[widthIdx];
			vectorLine.lineVertices[idx]   = pos1 - perpendicular;
			vectorLine.lineVertices[idx+1] = pos1 + perpendicular;
			if (vectorLine.smoothWidth && i < end-2) {
				perpendicular = v1 - v2;
				perpendicular *= normalizedDistance * vectorLine.lineWidths[widthIdx+1];				
			}
			vectorLine.lineVertices[idx+2] = pos2 - perpendicular;
			vectorLine.lineVertices[idx+3] = pos2 + perpendicular;
			idx += 4;
			widthIdx += widthIdxAdd;
		}
	}
	
	static void Skip (VectorLine vectorLine, ref int idx, ref Vector3 pos) {
		vectorLine.lineVertices[idx] = vectorLine.lineVertices[idx+1] = vectorLine.lineVertices[idx+2] = vectorLine.lineVertices[idx+3] = pos;
		idx += 4;
	}
	
	static void FillContinuous2D (VectorLine vectorLine, int end, ref int idx, Matrix4x4 thisMatrix, bool doTransform) {
		for (int i = 1; i < end; i++) {
			Vector3 p1 = doTransform? thisMatrix.MultiplyPoint3x4(vectorLine.points2[i]) :
									  (Vector3)vectorLine.points2[i];
			p1.z = zDist;
			vectorLine.lineVertices[idx++] = p1;
		}
	}

	static void FillContinuous (VectorLine vectorLine, int end, ref int idx, Matrix4x4 thisMatrix, bool doTransform) {
		for (int i = 1; i < end; i++) {
			Vector3 pos1 = doTransform? cam3D.WorldToScreenPoint(thisMatrix.MultiplyPoint3x4(vectorLine.points3[i])) :
										cam3D.WorldToScreenPoint(vectorLine.points3[i]);
			pos1.z = pos1.z < cutoff? -zDist : zDist;
			vectorLine.lineVertices[idx++] = pos1;
		}
	}
	
	public static void SetTextureScale (VectorLine vectorLine, float textureScale) {
		SetTextureScale (vectorLine, textureScale, 0.0f);
	}
	
	public static void SetTextureScale (VectorLine vectorLine, float textureScale, float offset) {
		int pointsLength = GetPointsLength (vectorLine);
		int end = vectorLine.continuousLine? pointsLength-1 : pointsLength;
		int add = vectorLine.continuousLine? 1 : 2;
		int idx = 0;
		float thisScale = 1.0f / textureScale;
		
		if (vectorLine.points2 != null) {
			for (int i = 0; i < end; i += add) {
				float xPos = thisScale / (vectorLine.lineWidth / (vectorLine.points2[i] - vectorLine.points2[i+1]).magnitude);
				vectorLine.lineUVs[idx++].x = offset;
				vectorLine.lineUVs[idx++].x = offset;
				vectorLine.lineUVs[idx++].x = xPos + offset;
				vectorLine.lineUVs[idx++].x = xPos + offset;
				offset = (offset + xPos) % 1;
			}		
		}
		else {
			for (int i = 0; i < end; i += add) {
				Vector2 p1 = cam3D.WorldToScreenPoint(vectorLine.points3[i]);
				Vector2 p2 = cam3D.WorldToScreenPoint(vectorLine.points3[i+1]);
				float xPos = thisScale / (vectorLine.lineWidth / (p1 - p2).magnitude);
				vectorLine.lineUVs[idx++].x = offset;
				vectorLine.lineUVs[idx++].x = offset;
				vectorLine.lineUVs[idx++].x = xPos + offset;
				vectorLine.lineUVs[idx++].x = xPos + offset;
				offset = (offset + xPos) % 1;
			}
		}
		
		vectorLine.mesh.uv = vectorLine.lineUVs;
	}

	public static void ResetTextureScale (VectorLine vectorLine) {
		int pointsLength = GetPointsLength (vectorLine);
		int end = vectorLine.continuousLine? pointsLength-1 : pointsLength;
		int add = vectorLine.continuousLine? 1 : 2;
		int idx = 0;
		
		for (int i = 0; i < end; i += add) {
			vectorLine.lineUVs[idx++].x = 0.0f;
			vectorLine.lineUVs[idx++].x = 0.0f;
			vectorLine.lineUVs[idx++].x = 1.0f;
			vectorLine.lineUVs[idx++].x = 1.0f;
		}
		
		vectorLine.mesh.uv = vectorLine.lineUVs;
	}
	
	public static void DrawPoints (VectorPoints vectorLine) {
		DrawPoints(vectorLine, null, false);
	}
	
	public static void DrawPoints (VectorPoints vectorLine, Transform thisTransform) {
		DrawPoints(vectorLine, thisTransform, true);
	}
	
	public static void DrawPoints (VectorPoints vectorLine, Transform thisTransform, bool useTransformMatrix) {
		if (error) return;
		if (!cam) {
			SetCamera();
			if (!cam) {
				LogError("Vector.DrawPoints: You must call SetCamera before calling DrawPoints");
				return;
			}
		}
	
		Matrix4x4 thisMatrix = useTransformMatrix? thisTransform.localToWorldMatrix : Matrix4x4.identity;
		zDist = useOrthoCam? 101-vectorLine.lineDepth : Screen.height/2 + ((100.0f - vectorLine.lineDepth) * .0001f);

		int end = vectorLine.points2.Length;
		int idx = 0;
		int widthIdx = 0;
		int widthIdxAdd = vectorLine.lineWidths.Length == 1? 0 : 1;

		for (int i = 0; i < end; i++) {
			Vector3 pos1 = useTransformMatrix? thisMatrix.MultiplyPoint3x4(vectorLine.points2[i]) :
											   (Vector3)vectorLine.points2[i];
			pos1.z = zDist;
			v1.x = v1.y = v2.y = vectorLine.lineWidths[widthIdx];
			v2.x = -vectorLine.lineWidths[widthIdx];

			vectorLine.lineVertices[idx]   = pos1 + v2;
			vectorLine.lineVertices[idx+1] = pos1 - v1;
			vectorLine.lineVertices[idx+2] = pos1 + v1;
			vectorLine.lineVertices[idx+3] = pos1 - v2;
			idx += 4;
			widthIdx += widthIdxAdd;
		}
		
		vectorLine.mesh.vertices = vectorLine.lineVertices;
		vectorLine.mesh.RecalculateBounds();
	}
	
	public static void SetDepth (VectorLine thisLine, int depth) {
		depth = Mathf.Clamp(depth, 0, 100);
		thisLine.lineDepth = depth;
	}
	
	public static void SetDepth (Transform thisTransform, int depth) {
		depth = Mathf.Clamp(depth, 0, 100);
		thisTransform.position = new Vector3(thisTransform.position.x,
											 thisTransform.position.y,
											 useOrthoCam? 101-depth : Screen.height/2 + ((100.0f - depth) * .0001f));		
	}
	
	static int endianDiff1;
	static int endianDiff2;
	static byte[] byteBlock;
	
	public static Vector3[] BytesToVector3Array (byte[] lineBytes) {
		if (lineBytes.Length % 12 != 0) {
			LogError("Vector.BytesToVector3Array: Incorrect input byte length...must be a multiple of 12");
			return new Vector3[0];
		}
		
		SetupByteBlock();
		Vector3[] points = new Vector3[lineBytes.Length/12];
		int idx = 0;
		for (int i = 0; i < lineBytes.Length; i += 12) {
			points[idx++] = new Vector3( ConvertToFloat (lineBytes, i),
										 ConvertToFloat (lineBytes, i+4),
										 ConvertToFloat (lineBytes, i+8) );
		}
		return points;
	}
	
	public static Vector2[] BytesToVector2Array (byte[] lineBytes) {
		if (lineBytes.Length % 8 != 0) {
			LogError("Vector.BytesToVector2Array: Incorrect input byte length...must be a multiple of 8");
			return new Vector2[0];
		}
		
		SetupByteBlock();
		Vector2[] points = new Vector2[lineBytes.Length/8];
		int idx = 0;
		for (int i = 0; i < lineBytes.Length; i += 8) {
			points[idx++] = new Vector2( ConvertToFloat (lineBytes, i),
										 ConvertToFloat (lineBytes, i+4));
		}
		return points;
	}
	
	static void SetupByteBlock () {
		if (byteBlock == null) {byteBlock = new byte[4];}
		if (System.BitConverter.IsLittleEndian) {endianDiff1 = 0; endianDiff2 = 0;}
		else {endianDiff1 = 3; endianDiff2 = 1;}	
	}
	
	// Unfortunately we can't just use System.BitConverter.ToSingle as-is...we need a function to handle both big-endian and little-endian systems
	static float ConvertToFloat (byte[] bytes, int i) {
		byteBlock[    endianDiff1] = bytes[i];
		byteBlock[1 + endianDiff2] = bytes[i+1];
		byteBlock[2 - endianDiff2] = bytes[i+2];
		byteBlock[3 - endianDiff1] = bytes[i+3];
		return System.BitConverter.ToSingle(byteBlock, 0);
	}
	
	public static void DestroyLine (VectorLine line) {
		if (line != null) {
			Destroy(line.vectorObject);
			Destroy(line.mesh);
			Destroy(line.meshFilter);
			line = null;
		}
	}

	public static void DestroyObject (VectorLine line, GameObject go) {
		DestroyLine(line);
		if (go != null) {
			Destroy(go);
		}
	}
	
	public static void MakeRectInLine (VectorLine line, Rect rect) {
		MakeRectInLine (line, new Vector2(rect.x, rect.y), new Vector2(rect.x+rect.width, rect.y-rect.height), 0);
	}

	public static void MakeRectInLine (VectorLine line, Rect rect, int index) {
		MakeRectInLine (line, new Vector2(rect.x, rect.y), new Vector2(rect.x+rect.width, rect.y-rect.height), index);
	}

	public static void MakeRectInLine (VectorLine line, Vector3 topLeft, Vector3 bottomRight) {
		MakeRectInLine (line, topLeft, bottomRight, 0);
	}

	public static void MakeRectInLine (VectorLine line, Vector3 topLeft, Vector3 bottomRight, int index) {
		int linePoints = (line.points2 == null)? line.points3.Length : line.points2.Length;

		if (line.continuousLine) {
			if (index + 5 > linePoints) {
				if (index == 0) {
					LogError("Vector: MakeRectInLine: The length of the array for continuous lines needs to be at least 5 for " + line.vectorObject.name);
					return;
				}
				LogError("Vector: Calling MakeRectInLine with an index of " + index + " would exceed the length of the Vector2 array for " + line.vectorObject.name);
				return;
			}
			if (line.points2 == null) {
				line.points3[index  ] = new Vector3(topLeft.x,     topLeft.y, 	  topLeft.z);
				line.points3[index+1] = new Vector3(bottomRight.x, topLeft.y, 	  topLeft.z);
				line.points3[index+2] = new Vector3(bottomRight.x, bottomRight.y, bottomRight.z);
				line.points3[index+3] = new Vector3(topLeft.x,	   bottomRight.y, bottomRight.z);
				line.points3[index+4] = new Vector3(topLeft.x,     topLeft.y, 	  topLeft.z);
			}
			else {
				line.points2[index  ] = new Vector2(topLeft.x,     topLeft.y);
				line.points2[index+1] = new Vector2(bottomRight.x, topLeft.y);
				line.points2[index+2] = new Vector2(bottomRight.x, bottomRight.y);
				line.points2[index+3] = new Vector2(topLeft.x,	   bottomRight.y);
				line.points2[index+4] = new Vector2(topLeft.x,     topLeft.y);
			}
		}
		
		else {
			if (index + 8 > linePoints) {
				if (index == 0) {
					LogError("Vector: MakeRectInLine: The length of the array for discrete lines needs to be at least 8 for " + line.vectorObject.name);
					return;
				}
				LogError("Vector: Calling MakeRectInLine with an index of " + index + " would exceed the length of the Vector2 array for " + line.vectorObject.name);
				return;
			}
			if (line.points2 == null) {
				line.points3[index  ] = new Vector3(topLeft.x,     topLeft.y,	  topLeft.z);
				line.points3[index+1] = new Vector3(bottomRight.x, topLeft.y, 	  topLeft.z);
				line.points3[index+2] = new Vector3(topLeft.x,     bottomRight.y, bottomRight.z);
				line.points3[index+3] = new Vector3(bottomRight.x, bottomRight.y, bottomRight.z);
				line.points3[index+4] = new Vector3(topLeft.x,     topLeft.y, 	  topLeft.z);
				line.points3[index+5] = new Vector3(topLeft.x,     bottomRight.y, bottomRight.z);
				line.points3[index+6] = new Vector3(bottomRight.x, topLeft.y, 	  topLeft.z);
				line.points3[index+7] = new Vector3(bottomRight.x, bottomRight.y, bottomRight.z);			
			}
			else {
				line.points2[index  ] = new Vector2(topLeft.x,     topLeft.y);
				line.points2[index+1] = new Vector2(bottomRight.x, topLeft.y);
				line.points2[index+2] = new Vector2(topLeft.x,     bottomRight.y);
				line.points2[index+3] = new Vector2(bottomRight.x, bottomRight.y);
				line.points2[index+4] = new Vector2(topLeft.x,     topLeft.y);
				line.points2[index+5] = new Vector2(topLeft.x,     bottomRight.y);
				line.points2[index+6] = new Vector2(bottomRight.x, topLeft.y);
				line.points2[index+7] = new Vector2(bottomRight.x, bottomRight.y);
			}
		}
	}
	
	public static void MakeCircleInLine (VectorLine line, Vector3 origin, float radius, int segments) {
		MakeEllipseInLine (line, origin, radius, radius, segments, 0.0f, 0);
	}

	public static void MakeCircleInLine (VectorLine line, Vector3 origin, float radius, int segments, float pointRotation) {
		MakeEllipseInLine (line, origin, radius, radius, segments, pointRotation, 0);
	}

	public static void MakeCircleInLine (VectorLine line, Vector3 origin, float radius, int segments, int index) {
		MakeEllipseInLine (line, origin, radius, radius, segments, 0.0f, index);
	}

	public static void MakeCircleInLine (VectorLine line, Vector3 origin, float radius, int segments, float pointRotation, int index) {
		MakeEllipseInLine (line, origin, radius, radius, segments, pointRotation, index);
	}

	public static void MakeEllipseInLine (VectorLine line, Vector3 origin, float xRadius, float yRadius, int segments) {
		MakeEllipseInLine (line, origin, xRadius, yRadius, segments, 0.0f, 0);
	}
	
	public static void MakeEllipseInLine (VectorLine line, Vector3 origin, float xRadius, float yRadius, int segments, int index) {
		MakeEllipseInLine (line, origin, xRadius, yRadius, segments, 0.0f, index);
	}

	public static void MakeEllipseInLine (VectorLine line, Vector3 origin, float xRadius, float yRadius, int segments, float pointRotation) {
		MakeEllipseInLine (line, origin, xRadius, yRadius, segments, pointRotation, 0);
	}
	
	public static void MakeEllipseInLine (VectorLine line, Vector3 origin, float xRadius, float yRadius, int segments, float pointRotation, int index) {
		if (segments < 3) {
			LogError("Vector: MakeEllipseInLine needs at least 3 segments");
			return;
		}
		
		int linePoints = (line.points2 == null)? line.points3.Length : line.points2.Length;
		float radians = 360.0f / segments*Mathf.Deg2Rad;
		float p = -pointRotation*Mathf.Deg2Rad;
		
		if (line.continuousLine) {
			if (index + (segments+1) > linePoints) {
				if (index == 0) {
					LogError("Vector: MakeEllipseInLine: The length of the points array for continuous lines needs to be at least the number of ellipse segments plus one for " + line.vectorObject.name);
					return;
				}
				LogError("Vector: Calling MakeEllipseInLine with an index of " + index + " would exceed the length of the vector array for " + line.vectorObject.name);
				return;
			}
			int i = 0;
			if (line.points2 == null) {
				for (i = 0; i < segments; i++) {
					line.points3[index+i] = origin + new Vector3(Mathf.Cos(p)*xRadius, Mathf.Sin(p)*yRadius, 0.0f);
					p += radians;
				}
				line.points3[index+i] = line.points3[index+(i-segments)];			
			}
			else {
				Vector2 v2Origin = origin;
				for (i = 0; i < segments; i++) {
					line.points2[index+i] = v2Origin + new Vector2(.5f + Mathf.Cos(p)*xRadius, .5f + Mathf.Sin(p)*yRadius);
					p += radians;
				}
				line.points2[index+i] = line.points2[index+(i-segments)];
			}
		}
		
		else {
			if (index + segments*2 > linePoints) {
				if (index == 0) {
					LogError("Vector: MakeEllipseInLine: The length of the point array for discrete lines needs to be at least twice the number of ellipse segments for " + line.vectorObject.name);
					return;
				}
				LogError("Vector: Calling MakeEllipseInLine with an index of " + index + " would exceed the length of the vector array for " + line.vectorObject.name);
				return;
			}
			if (line.points2 == null) {
				for (int i = 0; i < segments*2; i++) {
					line.points3[index+i] = origin + new Vector3(Mathf.Cos(p)*xRadius, Mathf.Sin(p)*yRadius, 0.0f);
					p += radians;
					i++;
					line.points3[index+i] = origin + new Vector3(Mathf.Cos(p)*xRadius, Mathf.Sin(p)*yRadius, 0.0f);
				}
			}
			else {
				Vector2 v2Origin = origin;
				for (int i = 0; i < segments*2; i++) {
					line.points2[index+i] = v2Origin + new Vector2(.5f + Mathf.Cos(p)*xRadius, .5f + Mathf.Sin(p)*yRadius);
					p += radians;
					i++;
					line.points2[index+i] = v2Origin + new Vector2(.5f + Mathf.Cos(p)*xRadius, .5f + Mathf.Sin(p)*yRadius);
				}
			}
		}
	}
	
	public static void MakeCurveInLine (VectorLine line, Vector2[] curvePoints, int segments) {
		MakeCurveInLine (line, curvePoints, segments, 0);
	}

	public static void MakeCurveInLine (VectorLine line, Vector2[] curvePoints, int segments, int index) {
		if (curvePoints.Length != 4) {
			LogError("Vector: MakeCurveInLine needs exactly 4 points in the curve points array");
			return;
		}
		MakeCurveInLine (line, curvePoints[0], curvePoints[1], curvePoints[2], curvePoints[3], segments, index);
	}
	
	public static void MakeCurveInLine (VectorLine line, Vector3[] curvePoints, int segments) {
		MakeCurveInLine (line, curvePoints, segments, 0);
	}
	
	public static void MakeCurveInLine (VectorLine line, Vector3[] curvePoints, int segments, int index) {
		if (curvePoints.Length != 4) {
			LogError("Vector: MakeCurveInLine needs exactly 4 points in the curve points array");
			return;
		}
		MakeCurveInLine (line, curvePoints[0], curvePoints[1], curvePoints[2], curvePoints[3], segments, index);
	}
	
	public static void MakeCurveInLine (VectorLine line, Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2, int segments) {
		MakeCurveInLine (line, anchor1, control1, anchor2, control2, segments, 0);
	}
	
	public static void MakeCurveInLine (VectorLine line, Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2, int segments, int index) {
		if (segments < 2) {
			LogError("Vector: MakeCurveInLine needs at least 2 segments");
			return;
		}
		int linePoints = (line.points2 == null)? line.points3.Length : line.points2.Length;
		
		if (line.continuousLine) {
			if (index + (segments+1) > linePoints) {
				if (index == 0) {
					LogError("Vector: MakeCurveInLine: The length of the array for continuous lines needs to be at least the number of segments plus one for " + line.vectorObject.name);
					return;
				}
				LogError("Vector: Calling MakeCurveInLine with an index of " + index + " would exceed the length of the Vector array for " + line.vectorObject.name);
				return;
			}
			if (line.points2 != null) {
				for (int i = 0; i < segments+1; i++) {
					line.points2[index+i] = GetBezierPoint (anchor1, control1, anchor2, control2, (float)i/segments);
				}
			}
			else {
				for (int i = 0; i < segments+1; i++) {
					line.points3[index+i] = GetBezierPoint3D (anchor1, control1, anchor2, control2, (float)i/segments);
				}
			}
		}
		
		else {
			if (index + segments*2 > linePoints) {
				if (index == 0) {
					LogError("Vector: MakeCurveInLine: The length of the array for discrete lines needs to be at least twice the number of segments for " + line.vectorObject.name);
					return;
				}
				LogError("Vector: Calling MakeCurveInLine with an index of " + index + " would exceed the length of the Vector array for " + line.vectorObject.name);
				return;
			}
			int idx = 0;
			if (line.points2 != null) {
				for (int i = 0; i < segments; i++) {
					line.points2[index + idx++] = GetBezierPoint (anchor1, control1, anchor2, control2, (float)i/segments);
					line.points2[index + idx++] = GetBezierPoint (anchor1, control1, anchor2, control2, (float)(i+1)/segments);
				}
			}
			else {
				for (int i = 0; i < segments; i++) {
					line.points3[index + idx++] = GetBezierPoint3D (anchor1, control1, anchor2, control2, (float)i/segments);
					line.points3[index + idx++] = GetBezierPoint3D (anchor1, control1, anchor2, control2, (float)(i+1)/segments);
				}
			}
		}
	}
	
	private static Vector2 GetBezierPoint (Vector2 anchor1, Vector2 control1, Vector2 anchor2, Vector2 control2, float t) {
		float cx = 3 * (control1.x - anchor1.x);
		float bx = 3 * (control2.x - control1.x) - cx;
		float ax = anchor2.x - anchor1.x - cx - bx;
		float cy = 3 * (control1.y - anchor1.y);
		float by = 3 * (control2.y - control1.y) - cy;
		float ay = anchor2.y - anchor1.y - cy - by;
		
		return new Vector2( (ax * (t*t*t)) + (bx * (t*t)) + (cx * t) + anchor1.x,
						    (ay * (t*t*t)) + (by * (t*t)) + (cy * t) + anchor1.y );
	}

	private static Vector3 GetBezierPoint3D (Vector3 anchor1, Vector3 control1, Vector3 anchor2, Vector3 control2, float t) {
		float cx = 3 * (control1.x - anchor1.x);
		float bx = 3 * (control2.x - control1.x) - cx;
		float ax = anchor2.x - anchor1.x - cx - bx;
		float cy = 3 * (control1.y - anchor1.y);
		float by = 3 * (control2.y - control1.y) - cy;
		float ay = anchor2.y - anchor1.y - cy - by;
		float cz = 3 * (control1.z - anchor1.z);
		float bz = 3 * (control2.z - control1.z) - cz;
		float az = anchor2.z - anchor1.z - cz - bz;
		
		return new Vector3( (ax * (t*t*t)) + (bx * (t*t)) + (cx * t) + anchor1.x,
							(ay * (t*t*t)) + (by * (t*t)) + (cy * t) + anchor1.y,
							(az * (t*t*t)) + (bz * (t*t)) + (cz * t) + anchor1.z );
	}
	
	public static void MakeTextInLine (VectorLine line, string text, Vector2 startPos, float size) {
		MakeTextInLine (line, text, startPos, size, 1.0f, 1.5f, true);
	}
	
	public static void MakeTextInLine (VectorLine line, string text, Vector2 startPos, float size, bool uppercaseOnly) {
		MakeTextInLine (line, text, startPos, size, 1.0f, 1.5f, uppercaseOnly);
	}
	
	public static void MakeTextInLine (VectorLine line, string text, Vector2 startPos, float size, float charSpacing, float lineSpacing) {
		MakeTextInLine (line, text, startPos, size, charSpacing, lineSpacing, true);
	}
	
	public static void MakeTextInLine (VectorLine line, string text, Vector2 startPos, float size, float charSpacing, float lineSpacing, bool uppercaseOnly) {
		if (line.continuousLine) {
			LogError ("Vector: MakeTextInLine can only be used with a discrete line");
			return;
		}
		Vector2 scaleVector = new Vector2(size, size);
		int idx = 0;
		int pointsLength = 0;
		
		while (true) {
			pointsLength = GetPointsLength (line);
			idx = 0;
			float charPos = 0.0f;
			float linePos = 0.0f;
			int charNum = 0;
			bool addChar = false;
			Vector2[] useArray = VectorChar.data[0];

			for (int i = 0; i < text.Length; i++) {
				charNum = System.Convert.ToInt32(text[i]);
				addChar = false;
				if (charNum < 0 || charNum > VectorChar.numberOfCharacters) {
					LogError ("Vector.MakeTextInLine: Character " + charNum + " out of range");
					return;
				}
				// Newline
				else if (charNum == 10) {
					linePos -= lineSpacing;
					charPos = 0.0f;
				}
				// Space
				else if (charNum == 32) {
					charPos += charSpacing;
				}
				// Character
				else {
					if (uppercaseOnly && charNum >= 97 && charNum <= 122) {
						charNum -= 32;
					}
					useArray = VectorChar.data[charNum];
					addChar = useArray != null;
				}
				if (addChar) {
					// See if it would exceed array length...if so, keep going so we can add up the total needed
					if (idx + useArray.Length > pointsLength) {
						idx += useArray.Length;
					}
					else {
						if (line.points2 != null) {
							for (int j = 0; j < useArray.Length; j++) {
								line.points2[idx] = useArray[j] + new Vector2(charPos, linePos);
								line.points2[idx] = Vector2.Scale(line.points2[idx], scaleVector);
								line.points2[idx++] += startPos;
							}
						}
						else {
							for (int j = 0; j < useArray.Length; j++) {
								line.points3[idx] = (Vector3)useArray[j] + new Vector3(charPos, linePos, 0.0f);
								line.points3[idx] = Vector2.Scale(line.points3[idx], scaleVector);
								line.points3[idx++] += (Vector3)startPos;
							}
						}
					}
					charPos += charSpacing;
				}
			}
			if (idx > pointsLength) {
				System.Array.Resize(ref line.points3, idx);
				line.Resize(idx);
			}
			else {
				break;
			}
		}
		// Zero out any unused space in the array, in case there was any previous data
		if (idx < pointsLength) {
			ZeroPointsInLine (line, idx);
		}
	}

	public static void ZeroPointsInLine (VectorLine line) {
		ZeroPointsInLine (line, 0);
	}

	public static void ZeroPointsInLine (VectorLine line, int index) {
		if (line.points2 != null) {
			if (index >= line.points2.Length) {
				LogError("Vector: index out of range for " + line.vectorObject.name + " when calling ZeroPointsInLine. Index: " + index + ", array length: " + line.points2.Length);
				return;
			}
			for (int i = index; i < line.points2.Length; i++) {
				line.points2[i] = new Vector2(0.0f, 0.0f);
			}
		}
		else {
			if (index >= line.points3.Length) {
				LogError("Vector: index out of range for " + line.vectorObject.name + " when calling ZeroPointsInLine. Index: " + index + ", array length: " + line.points3.Length);
				return;
			}
			for (int i = index; i < line.points3.Length; i++) {
				line.points3[i] = new Vector3(0.0f, 0.0f, 0.0f);
			}
		}
	}
	
	public static void EndCap (VectorLine line) {
		int pointsLength = GetPointsLength (line);
		if (pointsLength < 3) {
			LogError("Vector: EndCap needs at least 3 points for end caps to work");
			return;
		}
		
		line.lineUVs[0] = new Vector2(0.0f, 0.5f);	// 1 is already set at 0,0
		line.lineUVs[2] = new Vector2(0.5f, 0.5f);
		line.lineUVs[3] = new Vector2(0.5f, 0.0f);
		
		int i = 0;
		for (i = 4; i < line.lineUVs.Length-4; i += 4) {
			line.lineUVs[i+1] = new Vector2(0.0f, 0.5f);	// 0 is already set at 0,1
			line.lineUVs[i+2] = new Vector2(0.5f, 1.0f);
			line.lineUVs[i+3] = new Vector2(0.5f, 0.5f);
		}
		
		line.lineUVs[i] = new Vector2(0.5f, 0.5f);
		line.lineUVs[i+1] = new Vector2(0.5f, 0.0f);
		line.lineUVs[i+2] = new Vector2(1.0f, 0.5f);	// i+3 is already set at 1,0
		
		line.mesh.uv = line.lineUVs;
	}
}