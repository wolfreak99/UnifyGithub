/*************************
 * Original url: http://wiki.unity3d.com/index.php/RayCastWithoutPhysics
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/UtilityScripts/RayCastWithoutPhysics.cs
 * File based on original modification date of: 26 July 2014, at 07:54. 
 *
 * Author: Antony Stewart 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.UtilityScripts
{
    DescriptionRaycast using pixel color, moves a small colored square in raycast fashion at different distances towards your pixel until it finds the pixel objects Z buffer distance, and looks for colliders in that zone, takes about 15 frames for 0.03 distance precision, because one frame is required to change occluding square's distance and check color. Bounds checking code can be abit changed at the end for more precision. 
    Usage place on unity gameobject and click any colored object. 
    
    
    Moonraker.js // 
     
    #pragma strict
     
    /*
    -=Moonraker raycast without physics=-
    press middle mouse button to print distance and position of a triangle.
    Versionj 0.84...
    Works by occluding the pixel clicked using a flat object at different distances to estimate pixels distance and 3d position.
    Then the position can be compared to triangles on screen or object bounding boxes. code by ant stewart
    version 0.84 takes 15 frames , occluding shader Color is color.magenta, may change on different platforms,
    15 tests at 1000 meters is 1000 * 0.5 15 times precision = 0.03 meters.
    */
     
    var raylength : float = 1000; //this distnace devides by 2 at the end of every distance test  and homes in on the clicked pixel distance
     
    private var tex : Texture2D; // texture to read screen pixels at mouse position (slow needs a frame)
    private var mpos : Vector3;// mouse position
    private var scolor : Color; // color at mouse point, can change when is occluded
    private var testdistance : float; //current distance to place object with pink color at mouse point
     
    private var posGO : GameObject;
    private    var fov : float = 0.785398175;//Mathf.PI / 4.0;
    private var timez : float;
     var occludersize = 50.0;
    function Update () {
    if (  Input.GetMouseButtonDown(1)) {    moonrake(    );   }
     
    // print (Input.mousePosition);
    }
     
    function moonrake (){
    timez= Time.time;
        testdistance = camera.main.nearClipPlane + 0;
        mpos = Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay (mpos);
        var cube : GameObject  = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = ray.GetPoint (1000);
        cube.transform.LookAt(camera.main.transform,Vector3.up);
        Destroy (cube.renderer.material);
        cube.transform.localScale = Vector3(10, 10, 0.001);
     
        for (var k = 0; k < 15; k++)
        {
            //.. These 3 lines are only aesthetic to stop the pink square flashing on screen
            var worldSize :float = (2 * Mathf.Tan(fov / 2.0)) * testdistance;
            var size :float = 0.25f * worldSize;
            var screen = 50.0/Mathf.Max(Screen.width,Screen.height);
            cube.transform.localScale = Vector3(screen*size, screen*size, 0.0001); //flat square seen from front, should make it small depending on num pixels on screen
     
            cube.transform.position = ray.GetPoint (testdistance);
            getpixelcolor();
            yield  WaitForEndOfFrame();
            raylength *= 0.5;
            if(scolor == Color.magenta) testdistance += raylength;
            else testdistance -= raylength;
     
        }
     
        raylength = 1000; //reset distance variables for next test.
        testdistance = camera.main.nearClipPlane + 0;
     
     
     
     
        var dist = 10000.0;//far distance to narrow down
        var gameObjs : GameObject [ ] = FindObjectsOfType(GameObject) as GameObject [ ] ;//array all go's
     
        print ("the selected pixel was at position: "+ cube.transform.position);
        for ( k = 0; k < gameObjs.length; k++)
        {    
            if ( gameObjs[ k ].renderer != null  && gameObjs[ k ] != cube )
            {
                if ( gameObjs[ k ].renderer.bounds.Contains ( cube.transform.position) ) {posGO = gameObjs [ k ];}
            }    
        }
     
        if ( posGO ) // check if raycast has found any object and make it selected object
        {
            var lists4 = [ posGO.transform.root.gameObject ];
            Selection.objects = lists4;
            print ("there was an object.renderer.bounds that contains the pixel position, called: "+ posGO.name);
        }
        else print ("no gameobjects found with renderer.bounds.contains the pixel position ... ");
        // Destroy (posGO);
        Destroy (cube);
        print("time taken for raycast without physics: "+  (- timez + Time.time));
    }
     
    function getpixelcolor()
    {
        //make a 1x1 pixel texture of the pixel we want to check color of at mouse pos.
        var tex = new Texture2D(1, 1, TextureFormat.RGB24, false);
        yield  WaitForEndOfFrame();
        tex.ReadPixels(new Rect(mpos.x, mpos.y, mpos.x, mpos.y), 0, 0);
        tex.Apply();
        scolor = tex.GetPixel  ( 0, 0);
     
        Destroy (tex);
     
}
}
