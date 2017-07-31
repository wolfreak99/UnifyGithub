// Original url: http://wiki.unity3d.com/index.php/Sender
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Networking/Unity1xNetworkingScripts/Sender.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Networking.Unity1xNetworkingScripts
{
Sender ScriptA sender sends data via a Server. This script was implemented so that the code could be modified in one place. It sends the root object name, the name of the object it's attached to, and whatever is passed in as the data variable. A script could simply access the Server directly and bypass the Sender. 
Codeprivate var server : Server = null;
 
function Start(){
	var controller = GameObject.FindWithTag("GameController");
	if(controller!=null)server = controller.GetComponent("Server");
}
 
function Send(data){
	if(server==null) {
		var controller = GameObject.FindWithTag("GameController");
		if(controller!=null)server = controller.GetComponent("Server");
	}
	if(server!=null && server.Connected()){
		var sendString = gameObject.transform.root.name + "," + gameObject.name + "," + data + "\n";
		server.PutMessage(sendString);
	}
}
}
