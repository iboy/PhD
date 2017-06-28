// ©2011 Starscene Software. All rights reserved. Redistribution of source code without permission not allowed.

using UnityEngine;

public class VisibilityControlAlways : MonoBehaviour {

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
		VectorManager.use.isVisible2[m_objectNumber.i] = true;
		if (VectorManager.useDrawLine3D) {
			Vector.DrawLine3D(VectorManager.use.vectorLines2[m_objectNumber.i], VectorManager.use.transforms[m_objectNumber.i]);
		}
		else {
			Vector.DrawLine(VectorManager.use.vectorLines2[m_objectNumber.i], VectorManager.use.transforms[m_objectNumber.i]);
		}
		vectorLine = line;
	}

	void OnDestroy () {
		if (!destroyed) {
			VectorManager.DestroyObject (vectorLine, gameObject);
		}
	}
}