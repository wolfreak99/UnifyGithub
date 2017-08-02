/*************************
 * Original url: http://wiki.unity3d.com/index.php/CopyVariables
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/CopyVariables.cs
 * File based on original modification date of: 21 October 2012, at 10:32. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    DescriptionThis script matches variables from 1 GameObject from one hierarchy to the other. 
    UsagePlace this script in Assets/Editor and select "easyvar" in the Window menu. Specify the name of the script, and the source and destination GameObject, and hit 'copy variables' to execute. NOTE: Save your scene before running this script since it will overwrite all variables their value. 
    UnityScript - EasyVar .js/*
    	description
    	this script copys the variables from script A on gameObject 1 to script A on gameObject 2
     
    	v1.00 by Hannes Delbeke 20/10/2012
     
    	feel free to edit as much as you want, would be cool to add changes up here in the comments
     
    	interesting links that helped me creating this
    	http://msdn.microsoft.com/en-us/library/system.reflection.membertypes.aspx
    	http://answers.unity3d.com/questions/207095/construct-variable-name-from-strings.html
    */
     
     
    class EasyVar extends EditorWindow {
     
    	var arr = new Array ();
    	 var ReadGameObject : GameObject ;
    	 var WriteGameObject : GameObject ;
    	 var ScriptName : String = "test1" ;
     
    	 var readString : String = "Input : ";
     
        @MenuItem ("Window/EasyVar")
        public static function ShowWindow () 
        {   	
        	//Show existing window instance. If one doesn't exist, make one
            EditorWindow.GetWindow (EasyVar);
        }    
     
        function OnInspectorGUI()
        {
        }
        function OnGUI () {  
            ScriptName = GUILayout.TextField (ScriptName, 25);
     
            if (GUILayout.Button(readString+ReadGameObject))
            {
                GetReadGameObject();
            }
            if (GUILayout.Button(readString+WriteGameObject))
            {
                GetWriteGameObject();
            }
            if (GUILayout.Button("Copy Variables"))
            {
                PasteVariables();
            }
        }
        function GetReadGameObject()
        {
    		ReadGameObject = Selection.activeGameObject;
        }
        function GetWriteGameObject()
        {
    		WriteGameObject = Selection.activeGameObject;
        }
        function PasteVariables()
        {		
    		if(WriteGameObject)
    		var script2 = WriteGameObject.GetComponent(ScriptName) ;
    		else
    		Debug.Log("ERROR please assign a write gameobject");
     
    		if(ReadGameObject)
    		var script = ReadGameObject.GetComponent(ScriptName);
    		else
    		Debug.Log("ERROR please assign a read gameobject");
     
    		if(script && script2) //if both gameobejcts have the same script, and they exist
    	  	for(var mi  in script.GetType().GetMembers())
           	{
    	       if(System.String.Format("{0}",mi.MemberType) == "Field" )
    			{
    				 Debug.Log(System.String.Format("{0} = {1}", mi.Name, mi.MemberType));
    				 arr.Push (mi);
    				 var P = script.GetType();
    				 var FI = P.GetField(mi.Name);
    				Debug.Log("" + FI);
    				var tmp  = FI.GetValue(script);
    				Debug.Log("" + tmp);
     
    				P = script2.GetType();
    				FI= P.GetField(mi.Name);
    				FI.SetValue(script2,tmp);
    			}
    		}
    	}
    }
}
