// Version 1.3
// ©2011 Starscene Software. All rights reserved. Redistribution of source code without permission not allowed.

using UnityEngine;
using System.Collections.Generic;

public enum Visibility {Dynamic, Static, Always, NotControlled, None}
public enum Brightness {Fog, Normal, None}

public class VectorManager : MonoBehaviour {

	public static float minBrightnessDistance = 500.0f;
	public static float maxBrightnessDistance = 250.0f;
	static int brightnessLevels = 32;
	static float distanceCheckFrequency = .2f;
	static Color fogColor;
	public static bool useDrawLine3D = false;

	static VectorManager s_Use = null;

	public static VectorManager use {
		get {
			if (s_Use == null) {
				s_Use = FindObjectOfType(typeof (VectorManager)) as VectorManager;
			}
			
			return s_Use;
		}
	}

	void OnApplicationQuit() {
		s_Use = null;
	}
	
	public static void SetBrightnessParameters (float min, float max, int levels, float frequency, Color color) {
		// Since we're using sqrMagnitude for speed (instead of Vector3.Distance), we need the squared distances
		minBrightnessDistance = min * min;
		maxBrightnessDistance = max * max;
		brightnessLevels = levels;
		distanceCheckFrequency = frequency;
		fogColor = color;
	}
	
	public static float GetBrightnessValue (Vector3 pos) {
		return Mathf.InverseLerp(minBrightnessDistance, maxBrightnessDistance, (pos - Vector.CamTransformPosition()).sqrMagnitude);
	}
	
	public static void ObjectSetup (GameObject go, VectorLine line, Visibility visibility, Brightness brightness) {
		if (visibility == Visibility.Dynamic) {
			VisibilityControl vc = go.AddComponent(typeof(VisibilityControl)) as VisibilityControl;
			vc.Setup(line);
		}
		else if (visibility == Visibility.Static) {
			VisibilityControlStatic vcs = go.AddComponent(typeof(VisibilityControlStatic)) as VisibilityControlStatic;
			vcs.Setup(line);
		}
		else if (visibility == Visibility.Always) {
			VisibilityControlAlways vca = go.AddComponent(typeof(VisibilityControlAlways)) as VisibilityControlAlways;
			vca.Setup(line);
		}
		if (brightness == Brightness.Fog) {
			BrightnessControl bc = go.AddComponent(typeof(BrightnessControl)) as BrightnessControl;
			bc.Setup(line);
		}
		if (visibility >= Visibility.NotControlled && brightness >= Brightness.Normal) {
			Debug.LogWarning ("VectorManager: Nothing to do for " + line.vectorObject.name + " in ObjectSetup");
		}
	}
	
	void LateUpdate () {
		if (!Vector.CamTransformExists()) return;
			
		if (Vector.oldPosition != Vector.CamTransformPosition() || Vector.oldRotation != Vector.CamTransformEulerAngles()) {
			// Only redraw static objects if camera is moving
			for (int i = 0; i < arrayCount; i++) {
				if (isVisible[i]) {
					if (useDrawLine3D) {
						Vector.DrawLine3D (vectorLines[i]);
					}
					else {
						Vector.DrawLine (vectorLines[i]);
					}
				}
			}
		}

		Vector.oldPosition = Vector.CamTransformPosition();
		Vector.oldRotation = Vector.CamTransformEulerAngles();
		
		// Always redraw dynamic objects
		for (int i = 0; i < arrayCount2; i++) {
			if (isVisible2[i]) {
				if (useDrawLine3D) {
					Vector.DrawLine3D (vectorLines2[i], transforms[i]);
				}
				else {
					Vector.DrawLine (vectorLines2[i], transforms[i]);
				}
			}
		}
	}
	
	// It's quite a bit simpler just to have each VisibilityControlStatic script do its own check...however, running a lot of LateUpdate instances
	// is a fair bit slower than just running a centralized instance that checks all objects in a loop.
	// Hence it's worth the bother of tracking some Lists.
	[HideInInspector]
	public List<bool> isVisible;
	public List<VectorLine> vectorLines;
	List<RefInt> objectNumbers;
	int arrayCount = 0;
	bool visibilityRunning = false;
	
	public void VisibilityStaticSetup (VectorLine line, out RefInt objectNum) {
		if (!visibilityRunning) {
			isVisible = new List<bool>();
			vectorLines = new List<VectorLine>();
			objectNumbers = new List<RefInt>();
			visibilityRunning = true;
		}
		isVisible.Add(true);
		vectorLines.Add(line);
		objectNum = new RefInt(arrayCount++); 
		objectNumbers.Add(objectNum);
	}
	
	public void VisibilityStaticRemove (int objectNumber) {
		if (objectNumber >= isVisible.Count) {
			Debug.LogError ("VectorManager: object number exceeds array length in VisibilityStaticRemove");
			return;
		}
		
		for (int i = objectNumber+1; i < arrayCount; i++) {
			objectNumbers[i].i--;
		}
		isVisible.RemoveAt(objectNumber);
		vectorLines.RemoveAt(objectNumber);
		objectNumbers.RemoveAt(objectNumber);
		arrayCount--;
	}

	// Same as above, for VisibilityControl.  Kind of repetitious, but it was the fastest and least complicated way I could figure.
	[HideInInspector]
	public List<bool> isVisible2;
	public List<VectorLine> vectorLines2;
	List<RefInt> objectNumbers2;
	[HideInInspector]
	public List<Transform> transforms;
	int arrayCount2 = 0;
	bool visibilityRunning2 = false;
	
	public void VisibilitySetup (Transform thisTransform, VectorLine line, out RefInt objectNum) {
		if (!visibilityRunning2) {
			isVisible2 = new List<bool>();
			vectorLines2 = new List<VectorLine>();
			objectNumbers2 = new List<RefInt>();
			transforms = new List<Transform>();
			visibilityRunning2 = true;
		}
		isVisible2.Add(true);
		vectorLines2.Add(line);
		transforms.Add(thisTransform);
		objectNum = new RefInt(arrayCount2++); 
		objectNumbers2.Add(objectNum);
	}
	
	public void VisibilityRemove (int objectNumber) {
		if (objectNumber >= isVisible2.Count) {
			Debug.LogError ("VectorManager: object number exceeds array length in VisibilityRemove");
			return;
		}
		
		for (int i = objectNumber+1; i < arrayCount2; i++) {
			objectNumbers2[i].i--;
		}
		isVisible2.RemoveAt(objectNumber);
		transforms.RemoveAt(objectNumber);
		vectorLines2.RemoveAt(objectNumber);
		objectNumbers2.RemoveAt(objectNumber);
		arrayCount2--;
	}
	
	// Same as above...better to have one CheckDistance instance here instead of using InvokeRepeating for every object
	
	[HideInInspector]
	public List<bool> isVisible3;
	List<Transform> transforms3;
	public List<VectorLine> vectorLines3;
	[HideInInspector]
	public List<int> oldDistances;
	List<Color> colors;
	List<RefInt> objectNumbers3;
	int arrayCount3 = 0;
	bool checkRunning = false;
	
	public void CheckDistanceSetup (Transform thisTransform, VectorLine line, Color color, RefInt objectNum) {
		if (!checkRunning) {
			isVisible3 = new List<bool>();
			transforms3 = new List<Transform>();
			vectorLines3 = new List<VectorLine>();
			oldDistances = new List<int>();
			colors = new List<Color>();
			objectNumbers3 = new List<RefInt>();
			InvokeRepeating("CheckDistance", .01f, distanceCheckFrequency);
			checkRunning = true;
		}
		isVisible3.Add(true);
		transforms3.Add(thisTransform);
		vectorLines3.Add(line);
		oldDistances.Add(-1);
		colors.Add(color);
		objectNum.i = arrayCount3++;
		objectNumbers3.Add(objectNum);
	}
	
	public void DistanceRemove (int objectNumber) {
		if (objectNumber >= isVisible3.Count) {
			Debug.LogError ("VectorManager: object number exceeds array length in DistanceRemove");
			return;
		}
		
		for (int i = objectNumber+1; i < arrayCount3; i++) {
			objectNumbers3[i].i--;
		}
		isVisible3.RemoveAt(objectNumber);
		transforms3.RemoveAt(objectNumber);
		vectorLines3.RemoveAt(objectNumber);
		oldDistances.RemoveAt(objectNumber);
		colors.RemoveAt(objectNumber);
		objectNumbers3.RemoveAt(objectNumber);
		arrayCount3--;
	}
	
	void CheckDistance () {
		for (int i = 0; i < arrayCount3; i++) {
			if (isVisible3[i]) {
				SetDistanceColor(i);
			}	
		}
	}
	
	// This makes the color darker the farther away from the camera.  Just like fog.  However, fog won't work for vector objects,
	// hence we have to go through the effort of duplicating the effects.  But this also presents the opportunity to simulate limited
	// brightness levels being available, and the brightness only changes from minDistance to maxDistance units away and is constant otherwise.
	// (This also happens to make it a bit faster as a bonus, since Vector.SetColor isn't called as often.)
	// And vector objects can ignore the "fog" by just not having this script attached.
	// Currently it uses only one color per object, which it takes from line.segmentColors[0]
	
	public void SetDistanceColor (int i) {
		float thisDistance = GetBrightnessValue(transforms3[i].position);
		int intDistance = (int)(thisDistance * brightnessLevels);
		if (intDistance != oldDistances[i]) {
			Vector.SetColor(vectorLines3[i], Color.Lerp(fogColor, colors[i], thisDistance));
		}
		oldDistances[i] = intDistance;
	}
	
	public static void DestroyObject (VectorLine line, GameObject go) {
		int objectNumber = 0;
		bool destroyed = false;
		bool staticControl = false;
		
		VisibilityControl vc = go.GetComponent(typeof(VisibilityControl)) as VisibilityControl;
		if (vc != null) {
			objectNumber = vc.objectNumber.i;
			destroyed = vc.destroyed;
			vc.destroyed = true;
		}
		else {
			VisibilityControlStatic vcs = go.GetComponent(typeof(VisibilityControlStatic)) as VisibilityControlStatic;
			if (vcs != null) {
				objectNumber = vcs.objectNumber.i;
				destroyed = vcs.destroyed;
				vcs.destroyed = true;
				staticControl = true;
			}
			else {
				VisibilityControlAlways vca = go.GetComponent(typeof(VisibilityControlAlways)) as VisibilityControlAlways;
				if (vca != null) {
					objectNumber = vca.objectNumber.i;
					destroyed = vca.destroyed;
					vca.destroyed = true;
				}
				else {
					Debug.LogError ("VectorManager: DestroyObject: no visibility scripts seem to be attached to the object " + go.name);
					return;
				}
			}
		}
		// Prevent multiple calls during the same frame from messing things up, also don't try to use if playmode in the editor is stopped
		if (destroyed || VectorManager.use == null) return;
				
		if (staticControl) {
			VectorManager.use.VisibilityStaticRemove (objectNumber);
		}
		else {
			VectorManager.use.VisibilityRemove (objectNumber);
		}
		Vector.DestroyObject (line, go);
	}
}