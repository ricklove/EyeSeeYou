//var amplitude = 1.0;
var LT : Light;
var LightOn = true;
var LightTXT = "Light Off";

function OnGUI() {
	if (GUI.Button(Rect(Screen.width/2-50,10,100,50),LightTXT)){
			if(LightOn){
			LightOn = false;
			LightTXT = "Light On";
			LT.light.enabled = false;			
			}
			else{
			LightOn = true;
			LightTXT = "Light Off";
			LT.light.enabled = true;
			
			}
	}
}

