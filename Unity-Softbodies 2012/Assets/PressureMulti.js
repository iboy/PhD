public var hSliderValue : float = 1.0;
public var vSliderValue : float = -5.4458;

public var myTarget1 : Transform;
public var myTarget2 : Transform;
public var myTarget3 : Transform;
public var myFloor : Transform;
public var toggleLights : boolean = true;


function awake () {
  
  vSliderValue = -5.4458;

}

function Update () {

 //target.Translate(0, 2, 0);
 if (myTarget1) {
	myTarget1.GetComponent(InteractiveCloth).pressure = hSliderValue;
    
    myTarget1.GetComponent(InteractiveCloth).randomAcceleration = Vector3(0, 10, 0);
    
    }
    
     if (myTarget2) {
	myTarget2.GetComponent(InteractiveCloth).pressure = hSliderValue;
    
    myTarget2.GetComponent(InteractiveCloth).randomAcceleration = Vector3(0, 10, 0);
    
    }
    
     if (myTarget3) {
	myTarget3.GetComponent(InteractiveCloth).pressure = hSliderValue;
    
    myTarget3.GetComponent(InteractiveCloth).randomAcceleration = Vector3(0, 10, 0);
    
    }
    
    myFloor.position.y = vSliderValue;
    
  }


function OnGUI () {
  

	hSliderValue = GUI.HorizontalSlider (Rect (25, 25, 100, 30), hSliderValue, 0, 42.0);

                  vSliderValue = GUI.HorizontalSlider (Rect (130, 25, 100, 30), vSliderValue, -11.76, 10.0);

  
  
if (GUI.Button(Rect(25,50,220,30),"000 - Basic mesh"))
  Application.LoadLevel(0);	
if (GUI.Button(Rect(25,80,220,30),"001 - Soft body"))
  Application.LoadLevel(1);
if (GUI.Button(Rect(25,110,220,30),"002 - Internal controllers"))
  Application.LoadLevel(2);
if (GUI.Button(Rect(25,140,220,30),"003 - Controllers - attached"))
  Application.LoadLevel(3);
if (GUI.Button(Rect(25,170,220,30),"004 - No interaction"))
  Application.LoadLevel(4);
if (GUI.Button(Rect(25,200,220,30),"005 - Two way interaction"))
  Application.LoadLevel(5);
if (GUI.Button(Rect(25,230,220,30),"006 - Controllers with physics"))
  Application.LoadLevel(6);
if (GUI.Button(Rect(25,260,220,30),"007 - Metaballs"))
  Application.LoadLevel(7);
if (GUI.Button(Rect(25,290,220,30),"008 - Rodded softbody"))
  Application.LoadLevel(8);


}


function myAddForce () {
	
	
	
}