using UnityEngine;

public class BrightnessControl : MonoBehaviour {

	RefInt objectNumber;

	public void Setup (VectorLine line) {
		if (line.lineColors == null) {
			Debug.LogError ("In order to use Brightness.Fog, the line \"" + line.vectorObject.name + "\" must contain segment colors");
			return;
		}
		objectNumber = new RefInt(0);
		VectorManager.use.CheckDistanceSetup (transform, line, line.lineColors[0], objectNumber);
		VectorManager.use.SetDistanceColor (objectNumber.i);
	}
	
	// Force the color to be set when becoming visible
	void OnBecameVisible () {
		VectorManager.use.oldDistances[objectNumber.i] = -1;
		VectorManager.use.SetDistanceColor (objectNumber.i);
		VectorManager.use.isVisible3[objectNumber.i] = true;
	}
	
	void OnBecameInvisible () {
		VectorManager.use.isVisible3[objectNumber.i] = false;
	}
	
	void OnDestroy () {
		if (VectorManager.use != null) {	// Don't try to use if playmode in the editor is stopped
			VectorManager.use.DistanceRemove (objectNumber.i);
		}
	}
}