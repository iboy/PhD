﻿
public var toggleLights : boolean = true;

public var myTarget : Transform;


function awake () {
  
  //vSliderValue = -5.4458;

}

function OnGUI () {  
  
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

