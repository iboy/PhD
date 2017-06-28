// ©2011 Starscene Software. All rights reserved. Redistribution of source code without permission not allowed.

using UnityEngine;
using System.Collections;

public class VisibilityControlStatic : MonoBehaviour {
	
	RefInt m_objectNumber;
	public bool destroyed = false;
	VectorLine vectorLine;
	
	public RefInt objectNumber {
		get {return m_objectNumber;}
	}
	
	public void Setup (VectorLine line) {
		if (!VectorManager.use) {
			Debug.LogError("The VectorManager script must be attached to an object in the scene");
			return;
		}
		// Adjust points to this position, so the line doesn't have to be updated with the transform of this object
		// We make a new array since each line must therefore be a unique instance, not a reference to the original set of Vector3s
		var thisPoints = new Vector3[line.points3.Length];
		var thisMatrix = transform.localToWorldMatrix;
		for (int i = 0; i < thisPoints.Length; i++) {
			thisPoints[i] = thisMatrix.MultiplyPoint3x4(line.points3[i]);
		}
		line.points3 = thisPoints;
		vectorLine = line;
		VectorManager.use.VisibilityStaticSetup (line, out m_objectNumber);
		StartCoroutine(WaitCheck());
	}
	
	IEnumerator WaitCheck () {
		// Ensure that the line is drawn once even if the camera isn't moving
		// Otherwise this object would be invisible until the camera moves
		// However, the camera might not have been set up yet, so wait a frame and turn off if necessary
		if (VectorManager.useDrawLine3D) {
			Vector.DrawLine3D(VectorManager.use.vectorLines[m_objectNumber.i]);
		}
		else {
			Vector.DrawLine(VectorManager.use.vectorLines[m_objectNumber.i]);
		}
		
		yield return null;
		if (!renderer.isVisible) {
			VectorManager.use.isVisible[m_objectNumber.i] = false;
			VectorManager.use.vectorLines[m_objectNumber.i].vectorObject.renderer.enabled = false;
		}
	}
	
	void OnBecameVisible () {
		VectorManager.use.isVisible[m_objectNumber.i] = true;
		VectorManager.use.vectorLines[m_objectNumber.i].vectorObject.renderer.enabled = true;
		
		// Draw line now, otherwise's there's a 1-frame delay before the line is actually drawn in the next LateUpdate
		if (VectorManager.useDrawLine3D) {
			Vector.DrawLine3D(VectorManager.use.vectorLines[m_objectNumber.i]);
		}
		else {
			Vector.DrawLine(VectorManager.use.vectorLines[m_objectNumber.i]);
		}
	}
	
	void OnBecameInvisible () {
		if (destroyed) return;

		VectorManager.use.isVisible[m_objectNumber.i] = false;
		VectorManager.use.vectorLines[m_objectNumber.i].vectorObject.renderer.enabled = false;
	}
	
	void OnDestroy () {
		VectorManager.DestroyObject (vectorLine, gameObject);
	}
}