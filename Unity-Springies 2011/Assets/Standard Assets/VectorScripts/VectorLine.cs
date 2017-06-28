// Version 1.3
// ©2011 Starscene Software. All rights reserved. Redistribution of source code without permission not allowed.

using UnityEngine;

public enum LineType {Continuous, Discrete}
public enum Joins {Fill, Open, None}

public class VectorMaterial {
	public static Material defaultLineMaterial;
}

public class VectorLine {
	public Vector2[] points2;
	public Vector3[] points3;
	public float lineWidth;
	public float[] lineWidths;
	public float capLength;
	public int lineDepth;
	public GameObject vectorObject;
	public MeshFilter meshFilter;
	public Mesh mesh;
	public Vector3[] lineVertices;
	public Vector2[] lineUVs;
	public Color[] lineColors;
	public bool smoothWidth = false;
	bool m_continuousLine;
	bool m_fillJoins;
	bool m_isPoints;
	
	public bool continuousLine {
		get {return m_continuousLine;}
	}	
	public bool fillJoins {
		get {return m_fillJoins;}
	}
	public bool isPoints {
		get {return m_isPoints;}
	}
	
	// Vector3 constructors
	public VectorLine (string lineName, Vector3[] linePoints, Material lineMaterial, float width) {
		points3 = linePoints;
		SetupMesh (ref lineName, lineMaterial, null, ref width, 0.0f, 0, LineType.Discrete, Joins.Open, false, false);
	}
	public VectorLine (string lineName, Vector3[] linePoints, Color color, Material lineMaterial, float width) {
		points3 = linePoints;
		Color[] colors = SetColor(color, LineType.Discrete, linePoints.Length, false);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, 0.0f, 0, LineType.Discrete, Joins.Open, false, false);
	}
	public VectorLine (string lineName, Vector3[] linePoints, Color[] colors, Material lineMaterial, float width) {
		points3 = linePoints;
		SetupMesh (ref lineName, lineMaterial, colors, ref width, 0.0f, 0, LineType.Discrete, Joins.Open, false, false);
	}
	public VectorLine (string lineName, Vector3[] linePoints, Material lineMaterial, float width, int depth, LineType lineType, Joins joins) {
		points3 = linePoints;
		SetupMesh (ref lineName, lineMaterial, null, ref width, 0.0f, depth, lineType, joins, false, false);
	}
	public VectorLine (string lineName, Vector3[] linePoints, Color[] colors, Material lineMaterial, float width, int depth, LineType lineType, Joins joins) {
		points3 = linePoints;
		SetupMesh (ref lineName, lineMaterial, colors, ref width, 0.0f, depth, lineType, joins, false, false);
	}
	public VectorLine (string lineName, Vector3[] linePoints, Color color, Material lineMaterial, float width, int depth, LineType lineType, Joins joins) {
		points3 = linePoints;
		Color[] colors = SetColor(color, lineType, linePoints.Length, false);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, 0.0f, depth, lineType, joins, false, false);
	}
	
	// Vector2 constructors
	public VectorLine (string lineName, Vector2[] linePoints, Material lineMaterial, float width) {
		points2 = linePoints;
		SetupMesh (ref lineName, lineMaterial, null, ref width, 0.0f, 0, LineType.Discrete, Joins.Open, true, false);
	}	
	public VectorLine (string lineName, Vector2[] linePoints, Material lineMaterial, float width, float cap) {
		points2 = linePoints;
		SetupMesh (ref lineName, lineMaterial, null, ref width, cap, 0, LineType.Discrete, Joins.Open, true, false);
	}
	public VectorLine (string lineName, Vector2[] linePoints, Color color, Material lineMaterial, float width) {
		points2 = linePoints;
		Color[] colors = SetColor(color, LineType.Discrete, linePoints.Length, false);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, 0.0f, 0, LineType.Discrete, Joins.Open, true, false);
	}
	public VectorLine (string lineName, Vector2[] linePoints, Color color, Material lineMaterial, float width, float cap) {
		points2 = linePoints;
		Color[] colors = SetColor(color, LineType.Discrete, linePoints.Length, false);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, cap, 0, LineType.Discrete, Joins.Open, true, false);
	}
	public VectorLine (string lineName, Vector2[] linePoints, Color[] colors, Material lineMaterial, float width) {
		points2 = linePoints;
		SetupMesh (ref lineName, lineMaterial, colors, ref width, 0.0f, 0, LineType.Discrete, Joins.Open, true, false);
	}
	public VectorLine (string lineName, Vector2[] linePoints, Color[] colors, Material lineMaterial, float width, float cap) {
		points2 = linePoints;
		SetupMesh (ref lineName, lineMaterial, colors, ref width, cap, 0, LineType.Discrete, Joins.Open, true, false);
	}
	public VectorLine (string lineName, Vector2[] linePoints, Material lineMaterial, float width, float cap, int depth, LineType lineType, Joins joins) {
		points2 = linePoints;
		SetupMesh (ref lineName, lineMaterial, null, ref width, cap, depth, lineType, joins, true, false);
	}
	public VectorLine (string lineName, Vector2[] linePoints, Color[] colors, Material lineMaterial, float width, float cap, int depth, LineType lineType, Joins joins) {
		points2 = linePoints;
		SetupMesh (ref lineName, lineMaterial, colors, ref width, cap, depth, lineType, joins, true, false);
	}
	public VectorLine (string lineName, Vector2[] linePoints, Color color, Material lineMaterial, float width, float cap, int depth, LineType lineType, Joins joins) {
		points2 = linePoints;
		Color[] colors = SetColor(color, lineType, linePoints.Length, false);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, cap, depth, lineType, joins, true, false);
	}
	
	// Points constructors
	protected VectorLine (bool usePoints, string lineName, Vector2[] linePoints, Material lineMaterial, float width) {
		points2 = linePoints;
		SetupMesh (ref lineName, lineMaterial, null, ref width, 0.0f, 0, LineType.Continuous, Joins.Open, true, true);
	}
	protected VectorLine (bool usePoints, string lineName, Vector2[] linePoints, Color[] colors, Material lineMaterial, float width) {
		points2 = linePoints;
		SetupMesh (ref lineName, lineMaterial, colors, ref width, 0.0f, 0, LineType.Continuous, Joins.Open, true, true);
	}
	protected VectorLine (bool usePoints, string lineName, Vector2[] linePoints, Color color, Material lineMaterial, float width) {
		points2 = linePoints;
		Color[] colors = SetColor(color, LineType.Continuous, linePoints.Length, true);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, 0.0f, 0, LineType.Continuous, Joins.Open, true, true);
	}
	protected VectorLine (bool usePoints, string lineName, Vector2[] linePoints, Material lineMaterial, float width, int depth) {
		points2 = linePoints;
		SetupMesh (ref lineName, lineMaterial, null, ref width, 0.0f, depth, LineType.Continuous, Joins.Open, true, true);
	}
	protected VectorLine (bool usePoints, string lineName, Vector2[] linePoints, Color[] colors, Material lineMaterial, float width, int depth) {
		points2 = linePoints;
		SetupMesh (ref lineName, lineMaterial, colors, ref width, 0.0f, depth, LineType.Continuous, Joins.Open, true, true);
	}
	protected VectorLine (bool usePoints, string lineName, Vector2[] linePoints, Color color, Material lineMaterial, float width, int depth) {
		points2 = linePoints;
		Color[] colors = SetColor(color, LineType.Continuous, linePoints.Length, true);
		SetupMesh (ref lineName, lineMaterial, colors, ref width, 0.0f, depth, LineType.Continuous, Joins.Open, true, true);
	}
	
	Color[] SetColor (Color color, LineType lineType, int size, bool usePoints) {
		if (!usePoints) {
			size = lineType == LineType.Continuous? size-1 : size/2;
		}
		Color[] colors = new Color[size];
		for (int i = 0; i < size; i++) {
			colors[i] = color;
		}
		return colors;
	}

	protected void SetupMesh (ref string lineName, Material useMaterial, Color[] colors, ref float width, float cap, int depth, LineType lineType, Joins joins, bool use2Dlines, bool usePoints) {
		m_fillJoins = (joins == Joins.Fill? true : false);
		m_continuousLine = (lineType == LineType.Continuous? true : false);
		if (m_fillJoins && !continuousLine) {
			Debug.LogError("VectorLine: Must use LineType.Continuous if using Joins.Fill for \"" + lineName + "\"");
			return;
		}
		
		int pointsLength = use2Dlines? points2.Length : points3.Length;
		if ( (use2Dlines && points2 == null) || (!use2Dlines && points3 == null) ) {
			Debug.LogError("VectorLine: the points array is null for \"" + lineName + "\"");
			return;
		}
		if (!usePoints && pointsLength < 2) {
			Debug.LogError("The points array must contain at least two points");
			return;
		}
		if (!continuousLine && pointsLength%2 != 0) {
			Debug.LogError("VectorLine: Must have an even points array length for \"" + lineName + "\" when using LineType.Discrete");
			return;
		}
		
		
		lineWidth = width;
		lineWidths = new float[1];
		lineWidths[0] = lineWidth * .5f;
		lineDepth = Mathf.Clamp(depth, 0, 100);
		capLength = cap;
		m_isPoints = usePoints;
		bool useSegmentColors = (colors != null? true : false);
		
		if (!usePoints) {
			if (continuousLine) {
				if (useSegmentColors && colors.Length != pointsLength-1) {
					Debug.LogWarning("VectorLine: Length of color array for \"" + lineName + "\" must be length of points array minus one...disabling segment colors");
					useSegmentColors = false;
				}
			}
			else {
				if (useSegmentColors && colors.Length != pointsLength/2) {
					Debug.LogWarning("VectorLine: Length of color array for \"" + lineName + "\" must be exactly half the length of points array...disabling segment colors");
					useSegmentColors = false;
				}
			}
		}
		else {
			if (useSegmentColors && colors.Length != pointsLength) {
				Debug.LogWarning("VectorLine: Length of color array for \"" + lineName + "\" must be the same length as the points array...disabling segment colors");
				useSegmentColors = false;
			}
		}
		
		if (useMaterial == null) {
			if (VectorMaterial.defaultLineMaterial == null) {
				VectorMaterial.defaultLineMaterial = new Material("Shader \"Vertex Colors/Alpha\" {SubShader {Cull Off ZWrite On Blend SrcAlpha OneMinusSrcAlpha Pass {BindChannels {Bind \"Color\", color Bind \"Vertex\", vertex}}}}");
			}
			useMaterial = VectorMaterial.defaultLineMaterial;
		}
	
		mesh = new Mesh();
		mesh.name = lineName;
		vectorObject = new GameObject("Vector "+lineName, typeof(MeshRenderer));
		vectorObject.layer = Vector.vectorLayer;
		meshFilter = (MeshFilter)vectorObject.AddComponent(typeof(MeshFilter));
		vectorObject.renderer.material = useMaterial;
		meshFilter.mesh = mesh;		
		BuildMesh (pointsLength, use2Dlines, useSegmentColors, colors);
	}
	
	public void Resize (Vector3[] linePoints) {
		if (points2 != null) {
			Debug.LogError ("Must supply a Vector2 array instead of a Vector3 array for this line");
			return;
		}
		points3 = linePoints;
		RebuildMesh (false, linePoints.Length);
	}

	public void Resize (Vector2[] linePoints) {
		if (points3 != null) {
			Debug.LogError ("Must supply a Vector3 array instead of a Vector2 array for this line");
			return;
		}
		points2 = linePoints;
		RebuildMesh (true, linePoints.Length);
	}
	
	public void Resize (int newSize) {
		if (points2 != null) {
			points2 = new Vector2[newSize];
		}
		else {
			points3 = new Vector3[newSize];
		}
		RebuildMesh (points2 != null, newSize);
	}
	
	void RebuildMesh (bool use2Dlines, int pointsLength) {
		GameObject.Destroy(mesh);
		mesh = new Mesh();
		meshFilter.mesh = mesh;
		Color[] colors = null;
		if (lineColors != null) {
			colors = SetColor (lineColors[0], continuousLine? LineType.Continuous : LineType.Discrete, pointsLength, isPoints);
		}
		if (lineWidths.Length > 1) {
			lineWidths = new float[pointsLength];
			float thisWidth = lineWidth * .5f;
			for (int i = 0; i < pointsLength; i++) {
				lineWidths[i] = thisWidth;
			}
		}
		BuildMesh (pointsLength, use2Dlines, colors != null, colors);
	}
	
	void BuildMesh (int pointsLength, bool use2Dlines, bool useSegmentColors, Color[] colors) {
		int vertLength = 0;
		int triLength = 0;
		bool addPoint = false;
		
		if (continuousLine) {
			if (!m_isPoints) {
				vertLength = (pointsLength-1)*4;
				triLength = (pointsLength-1)*6;
			}
			else {
				vertLength = (pointsLength)*4;
				triLength = (pointsLength)*6;
			}
			if (fillJoins) {
				vertLength += pointsLength-1;
				triLength += (pointsLength-2)*6;
				// Add another join fill if the first point equals the last point (like with a square)
				if ( (use2Dlines && points2[0] == points2[points2.Length-1]) || (!use2Dlines && points3[0] == points3[points3.Length-1]) ) {
					triLength += 6;
					addPoint = true;
				}
			}
		}
		else {
			vertLength = pointsLength*2;
			triLength = pointsLength/2 * 6;
		}
		if (vertLength > 65534) {
			Debug.LogError("VectorLine: exceeded maximum vertex count of 65534 for \"" + vectorObject.name + "\"...use fewer points");
			return;
		}
		
		lineVertices = new Vector3[vertLength];
		int[] newTriangles = new int[triLength];
		lineUVs = new Vector2[vertLength];
	
		int idx = 0;
		int vIdx = 0;
		int end = 0;
		if (!m_isPoints) {
			end = continuousLine? pointsLength-1 : pointsLength/2;
		}
		else {
			end = pointsLength;
		}
		
		for (int i = 0; i < end; i++) {	
			lineUVs[idx]   = new Vector2(0.0f, 1.0f);
			lineUVs[idx+1] = new Vector2(0.0f, 0.0f);
			lineUVs[idx+2] = new Vector2(1.0f, 1.0f);
			lineUVs[idx+3] = new Vector2(1.0f, 0.0f);
			idx += 4;
		}
		if (m_fillJoins) {
			vIdx = idx;
			for (int i = 1; i < end; i++) {
				lineUVs[idx++] = new Vector2(.5f, .5f);
			}
		}
		
		if (useSegmentColors) {
			lineColors = new Color[vertLength];
			idx = 0;
			for (int i = 0; i < colors.Length; i++) {
				lineColors[idx]   = colors[i];
				lineColors[idx+1] = colors[i];
				lineColors[idx+2] = colors[i];
				lineColors[idx+3] = colors[i];
				idx += 4;
			}
			if (m_fillJoins) {
				for (int i = 1; i < colors.Length; i++) {
					lineColors[idx++] = colors[i];
				}
				if (addPoint) {
					lineColors[idx] = colors[colors.Length-1];
				}
			}
		}
		
		idx = 0;
		if (!m_isPoints) {
			end = continuousLine? (pointsLength-1)*4 : pointsLength*2;
		}
		else {
			end = pointsLength*4;			
		}
		for (int i = 0; i < end; i += 4) {
			newTriangles[idx]   = i;
			newTriangles[idx+1] = i+2;
			newTriangles[idx+2] = i+1;
	
			newTriangles[idx+3] = i+2;
			newTriangles[idx+4] = i+3;
			newTriangles[idx+5] = i+1;
			idx += 6;
		}
		
		if (m_fillJoins) {
			end -= 2;
			int i = 0;
			for (i = 2; i < end; i += 4) {
				newTriangles[idx]   = i+1;
				newTriangles[idx+1] = i+3;
				newTriangles[idx+2] = vIdx;
		
				newTriangles[idx+3] = vIdx++;
				newTriangles[idx+4] = i;
				newTriangles[idx+5] = i+2;
				idx += 6;	
			}
			if (addPoint) {
				newTriangles[idx]   = 1;
				newTriangles[idx+1] = i+1;
				newTriangles[idx+2] = vIdx;
		
				newTriangles[idx+3] = vIdx;
				newTriangles[idx+4] = 0;
				newTriangles[idx+5] = i;
			}
		}

		mesh.vertices = lineVertices;
		mesh.uv = lineUVs;
		if (useSegmentColors) {
			mesh.colors = lineColors;
		}
		mesh.triangles = newTriangles;
	}
}

public class VectorPoints : VectorLine {
	public VectorPoints (string name, Vector2[] points, Material material, float width) : base (true, name, points, material, width) {
	}
	public VectorPoints (string name, Vector2[] points, Color[] colors, Material material, float width) : base (true, name, points, colors, material, width) {
	}
	public VectorPoints (string name, Vector2[] points, Color color, Material material, float width) : base (true, name, points, color, material, width) {
	}
	public VectorPoints (string name, Vector2[] points, Material material, float width, int depth) : base (true, name, points, material, width, depth) {
	}
	public VectorPoints (string name, Vector2[] points, Color[] colors, Material material, float width, int depth) : base (true, name, points, colors, material, width, depth) {
	}
	public VectorPoints (string name, Vector2[] points, Color color, Material material, float width, int depth) : base (true, name, points, color, material, width, depth) {
	}
}