// ©2011 Starscene Software. All rights reserved. Redistribution of source code without permission not allowed.

using UnityEngine;

public class VisibilityControl : MonoBehaviour {

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
		VectorManager.use.VisibilitySetup (transform, line, out m_objectNumber);
		vectorLine = line;
	}

	void OnBecameVisible () {
		VectorManager.use.isVisible2[m_objectNumber.i] = true;
		VectorManager.use.vectorLines2[m_objectNumber.i].vectorObject.renderer.enabled = true;
		
		// Draw line now, otherwise's there's a 1-frame delay before the line is actually drawn in the next LateUpdate
		if (VectorManager.useDrawLine3D) {
			Vector.DrawLine3D(VectorManager.use.vectorLines2[m_objectNumber.i], VectorManager.use.transforms[m_objectNumber.i]);
		}
		else {
			Vector.DrawLine(VectorManager.use.vectorLines2[m_objectNumber.i], VectorManager.use.transforms[m_objectNumber.i]);
		}
	}
	
	void OnBecameInvisible () {
		if (destroyed) return;
		
		VectorManager.use.isVisible2[m_objectNumber.i] = false;
		VectorManager.use.vectorLines2[m_objectNumber.i].vectorObject.renderer.enabled = false;
	}
	
	void OnDestroy () {
		VectorManager.DestroyObject (vectorLine, gameObject);
	}
}