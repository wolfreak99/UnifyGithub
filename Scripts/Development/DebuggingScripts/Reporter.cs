// Original url: http://wiki.unity3d.com/index.php/Reporter
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Development/DebuggingScripts/Reporter.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Development.DebuggingScripts
{

Author: Opless 
Description For support work (and testing alpha/beta products) you'll want to know what hardware your players are running on. After all, in the runtime still works on some cards that aren't supported, but you'll lose things like GUI etc. With the inclusion of the GUI classes in the first release of 2.0 I noticed that the GUI wasn't supported on the (rather decrepid) SiS graphics card - so this should help debugging other graphics problems. 
Don't worry about some System.Environment variables not being accessable in the web runtime. That's normal, and indeed that's what the try ... finally blocks are for. 
Finally if you have a FPS GUIText Counter like FramesPerSecond it'll read that, just make sure you set the gameobject "fps" in the inspecter appropriately. 
Reporter.cs Snarf some system information, and send it to our php script so we can get the data via email. 
using UnityEngine;
using System.Collections;
 
public class Reporter : MonoBehaviour {
 
	private float TS = 0;
	public  GameObject fps;
	public  float countdown = 5;
	public  float thankYouTextTimeOut = 60;
	public  string thankYouText ="Thanks!";
	public  string URL = "http://your-domain-here.com/Reporter.php";
 
	// Use this for initialization
	void Start () 
	{
		TS = Time.time + countdown;
	}
 
	// Update is called once per frame
	void Update () 
	{
		if(Time.time > TS && TS != -1)
		{
			WWWForm form = new WWWForm();
			TS = -1; // oneshot only
 
			try { form.AddField("FPS", fps.guiText.text.ToString()); } finally {} {  }
			try { form.AddField("Machine Name",System.Environment.MachineName); } finally {} {  }
			try { form.AddField("Operating System",System.Environment.OSVersion.ToString()); } finally {} {  }
			try { form.AddField("User Name",System.Environment.UserName); } finally {} {  }
			try { form.AddField(".NET Version",System.Environment.Version.ToString()); } finally {} {  }
			try { form.AddField("CPUCount",System.Environment.ProcessorCount); } finally {} {  }
			try { form.AddField("Command",System.Environment.CommandLine); } finally {} {  }
			try { form.AddField("CWD",System.Environment.CurrentDirectory); } finally {} {  }
			try { form.AddField("SYS",System.Environment.SystemDirectory); } finally {} {  }
			try { form.AddField("Runtime",Application.platform.ToString()); } finally {} {  }
			if(Application.isEditor)
			{
				try { form.AddField("Running In","editor"); } finally {} {  }
			}
			else
			{
				try { form.AddField("Running In","runtime"); } finally {} {  }
			}
			try { 
				form.AddField("URL",Application.absoluteURL);
				form.AddField("Src:",Application.absoluteURL);
				form.AddField("Unity:",Application.unityVersion);
				} finally {} {  }
			//try { data+= ""+System.Environment.+"\n";
			//Debug.Log(data);
			try {
				// This next section is Unity2.0 only, just comment/delete if you're still using unity 1.x
				form.AddField("graphicsMemorySize",SystemInfo.graphicsMemorySize);
				form.AddField("graphicsDeviceName",SystemInfo.graphicsDeviceName);
				form.AddField("graphicsDeviceVendor",SystemInfo.graphicsDeviceVendor);
				form.AddField("graphicsDeviceVersion",SystemInfo.graphicsDeviceVersion);
				//
				form.AddField("supportsShadows", SystemInfo.supportsShadows ? "supported" : "unsupported");
				form.AddField("supportsRenderTextures",SystemInfo.supportsRenderTextures ? "supported" : "unsupported");
				form.AddField("supportsImageEffects",SystemInfo.supportsImageEffects ? "supported" : "unsupported");
				} finally {} {  }
			WWW www = new WWW(URL, form);
			//yield www;
			this.guiText.text = thankYouText;
			Destroy(this.gameObject,thankYouTextTimeOut);
		}
 
	}
}

Reporter.php A very simple reporting script. It is left up to the end user to devise a way to push the data into a database, CRM system or some other method of storage. 
<?php
$data = "";
foreach($_REQUEST as $key=>$val)
{
	$data.="[".$key."] => [".$val."]\n";
}
$data.="\n\n\n";
foreach($_SERVER as $key=>$val)
{
	$data.="[".$key."] => [".$val."]\n";
}

mail("your-email-here@your-domain-here.com","REPORTER",$data);

?>
}
