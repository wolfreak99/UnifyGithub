// Original url: http://wiki.unity3d.com/index.php/ModularConstruction
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/ReallySimpleScripts/ModularConstruction.cs
// File based on original modification date of: 13 July 2016, at 03:18. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.ReallySimpleScripts
{
Author: Divinux 
Contents [hide] 
1 Description 
2 Usage 
3 Code ModularConstruction.cs 
4 History 

Description This is a really basic version of a modular building system. Rust and Unturned use this kind of thing for houses and such, but it can also be used for weapons, corridors, spaceships, vehicles, anything that you can provide "building blocks" for. 
 
Usage Set up the manager object 
Create an empty GameObject and call it BuildingManager or similar. Attach this script to it. 
Set the size of the Prefabslist to however many modules you have. 
Set up a module 
Create an empty GameObject. Give it a descriptive tag like "wall". Also name it "WallModule" or similar. 
Add a model as a child to it. 
Add a collider as a child as well. 
Add a new sphere as a child. Tag it "handle". You can set this tag in the inspector of this script. 
Make sure the sphere you just created has a collider, then tick the "Is Trigger" box. 
Disable the sphere's mesh renderer. 
Position this sphere where you want objects to connect. Your prefab may have multiple connections. 
Make a prefab for this module. 
Set up a preview 
Just keep using the object you just created and make the following changes: 
Add a transparent material to the mesh, to make it look like a ghost preview. 
Disable the collider. 
On all your handle children, disable the collider and enable the mesh renderer. 
Name the object "WallPreview" or similar and make a prefab for it. 
Add references to the manager 
Now go back to the inspector of your BuildingManager and just drag your prefabs into the list "Prefabs". 
You're done! 
Code ModularConstruction.cs using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class ModularConstruction : MonoBehaviour 
{
 //distance at which objects can connect
    public float Range = 50f;
    //distance at which previews hover
    public float HoverDist = 10f;
    //tag for the position markers
    public string Tag = "handle";
    //currently selected part in the list
    //change this from wherever to have the player select an item
    public int Selected = -1;
    //list of all prefabs
    public List<Part> Prefabs = new List<Part>();
 
    //private stuffs
    RaycastHit hit;
    GameObject Preview;
    //list of all handles the prefab has
    List<GameObject> Handles = new List<GameObject>();
    //currently active handle
    int SelectedHandle = 0;
    //just a position holder
    Vector3 p;
 
    //a single object class, 
    //the actual part that gets instantiated,
    //and the preview model for it
    [System.Serializable]
    public class Part
    {
        public GameObject Prefab;
        public GameObject Preview;
    }
 
    void Update () 
    {
        //check for player selection
        //only needed for testing, feel free to delete
        CheckSelection();
        //if any item is "active"
        if(Selected != -1)
        {
            //check if anything was hit
            if(Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3((Screen.width / 2), (Screen.height / 2), 0)), out hit, Range))
            {
                //enable preview
                EnableSelected();
                //check if hit object is a positioning handle
                if(hit.transform.tag == Tag)
                {
                    //find correct position
                    p = Handles[SelectedHandle].transform.position - Preview.transform.position;
                    //move preview into position
                    Preview.transform.position = hit.transform.position - p;
                }
            }
            //if not, just spawn it at default distance
            else
            {
                EnableSelected();
                Preview.transform.position = Camera.main.transform.position+(Camera.main.transform.forward*HoverDist);
            }
            // rotating
            AwaitRotation();
            // toggling the active handle
            AwaitHandleSwitch();
            //actually place the object
            AwaitPlacement();
        }
        else if(Preview != null)
        {
            Destroy(Preview);
            Handles.Clear();
        }
 
    }
    //enables the selected preview
    void EnableSelected()
    {
        if(Preview == null)
        {
            //instantiate if no preview is present
            Preview = Instantiate(Prefabs[Selected].Preview,Camera.main.transform.position+(Camera.main.transform.forward*HoverDist), new Quaternion(0,0,0,0)) as GameObject;
            FindHandles(Preview);
        }
        else if(Preview.transform.tag != Prefabs[Selected].Preview.transform.tag)
        {
            //if wrong preview is present, 
            //delete, then instantiate correct one
            Destroy(Preview);
            Preview = Instantiate(Prefabs[Selected].Preview,Camera.main.transform.position+(Camera.main.transform.forward*HoverDist), new Quaternion(0,0,0,0)) as GameObject;
            FindHandles(Preview);
        }
        else
        {
            //else just move the preview
            Preview.transform.position = Camera.main.transform.position+(Camera.main.transform.forward*HoverDist);
        }
    }
    //waits for the player to confirm placing the prefab 
    void AwaitPlacement()
    {
        if(Input.GetMouseButtonUp(0))
        {
            Instantiate(Prefabs[Selected].Prefab, Preview.transform.position, Preview.transform.rotation);
        }
    }
    //waits for player to rotate the object
    void AwaitRotation()
    {
        if(Input.GetKeyUp(KeyCode.Q))
        {
            Preview.transform.Rotate(90,0,0,Space.World);
        }
        if(Input.GetKeyUp(KeyCode.E))
        {
            Preview.transform.Rotate(0,90,0,Space.World);
        }
        if(Input.GetKeyUp(KeyCode.R))
        {
            Preview.transform.Rotate(0,0,90,Space.World);
        }
    }
    //waits for player to toggle the active handle
    void AwaitHandleSwitch()
    {
        if(Input.GetKeyUp(KeyCode.T))
        {
            if(Handles.Count > 0)
            {
                SelectedHandle++;
                if(SelectedHandle >= Handles.Count)
                {
                    SelectedHandle = 0;
                }
            }
        }
    }
    //Finds all handles the prefab has 
    void FindHandles(GameObject g)
    {
        Handles.Clear();
        foreach(Transform c in Preview.transform)
        {
            if(c.tag == Tag)
            {
                Handles.Add(c.gameObject);
            }
        }
    }
    //PLACEHOLDER METHOD FOR SELECTING
    //REPLACE WITH WHATEVER
    //WAY YOU HAVE FOR THE PLAYER
    //TO SELECT ITEMS
    void CheckSelection()
    {
        if(Input.GetKeyDown ("1"))
        {
            if(Selected != 0)
            {Selected = 0;}
            else
            {Selected = -1;}
            SelectedHandle = 0;
        }
        else if(Input.GetKeyDown ("2"))
        {
            if(Selected != 1)
            {Selected = 1;}
            else
            {Selected = -1;}
            SelectedHandle = 0;
        }
    }
}History 12 March 2016 - Release 
}
