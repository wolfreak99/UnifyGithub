// Original url: http://wiki.unity3d.com/index.php/LarsonScanner
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/GUI/HeadsUpDisplayScripts/LarsonScanner.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.GUI.HeadsUpDisplayScripts
{
Author: OPless 
Description A simple visual cue to show that the main loop isn't blocked. 
C Sharpusing UnityEngine;
using System.Collections.Generic;
 
public class HUDLarsonScanner : MonoBehaviour {
	public int larsonElements = 10;
	public float speed = 10;
	List<string> larsonStrings = new List<string>();
 
	// Use this for initialization
	void Start () 
	{
		int whiteSpace = larsonElements-1;
		for(int i=0; i< larsonElements; i++)
		{
			int left =   whiteSpace -i;    //  0  1  2  3  4 5
			int right=   whiteSpace -left; //  5  4  3  2  1 0
			string str = string.Format( "[{0}{1}{2}]", Str(left), '*', Str(right));
			larsonStrings.Add(str);
		}
		for(int i=larsonElements-1;i >= 0; i--)
		{
			larsonStrings.Add(larsonStrings[i]);
		}
	}
	string Str(int x)
	{
		if(x <= 0)
			return string.Empty;
		else
			return new string(' ',x);
	}
	// Update is called once per frame
	void Update () {
		float t= Time.realtimeSinceStartup * speed;
		this.guiText.text = larsonStrings[ ((int) t)% larsonStrings.Count];
	}
}
}
