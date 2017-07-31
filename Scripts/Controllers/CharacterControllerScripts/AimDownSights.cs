// Original url: http://wiki.unity3d.com/index.php/AimDownSights
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CharacterControllerScripts/AimDownSights.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
Author: NickAVV 
DescriptionThis script will make a generic first person shooter seem more modern by allowing you to hold a key to aim down the sights.
While the key is held it slows down moving and turning speeds and smoothly adjusts the x and y position of the gun as well as the field of view of the camera. 
UsagePlace your gun model as a child of the Main Camera in the default FPSWalker. It's position should be x(0.5), y(-0.4), and whatever z position looks best. 
You may want to adjust the scale of your model to whatever looks best in this position as well. 
Set up an input button named "Sights". You can map this to whatever key or mouse button you want. Mine is set to left shift. 
Attach the following script to the First Person Controller. 
Drag your gun model (child of the main camera) into the slot for "Gun" on the script. 
Script var gun : Transform;
 var nextPos = 0.0;
 var nextField = 40.0;
 var nextPos2 = -0.2;
 var dampVelocity = 0.4;
 var dampVelocity2 = 0.4;
 var dampVelocity3 = 0.4;
 
 function Update () {
    var newPos = Mathf.SmoothDamp(gun.transform.localPosition.x, nextPos, dampVelocity, .3);
    var newField = Mathf.SmoothDamp(Camera.main.fieldOfView, nextField, dampVelocity2, .3);
    var newPos2 = Mathf.SmoothDamp(gun.transform.localPosition.y, nextPos2, dampVelocity3, .3);
 
    gun.transform.localPosition.x = newPos;
    gun.transform.localPosition.y = newPos2;
    Camera.main.fieldOfView = newField;
 
    if (Input.GetButton("Fire2")) {
        //adjust viewpoint and gun position
        nextField = 40.0;
        nextPos = 0.0;
        nextPos2 = -0.2;
 
        //slow down turning and movement speed
        GetComponent(FPSWalker).speed = 1.5;
        GetComponent("MouseLook").sensitivityX = 2;
        camera.main.GetComponent("MouseLook").sensitivityX = 2;
        camera.main.GetComponent("MouseLook").sensitivityY = 2;
    } else {
        //adjust viewpoint and gun position
        nextField = 60.0;
        nextPos = 0.5;
        nextPos2 = -0.4;
 
        //speed up turning and movement speed
        GetComponent(FPSWalker).speed = 6;
        GetComponent("MouseLook").sensitivityX = 6;
        camera.main.GetComponent("MouseLook").sensitivityX = 6;
        camera.main.GetComponent("MouseLook").sensitivityY = 6;
    }
 }
}
