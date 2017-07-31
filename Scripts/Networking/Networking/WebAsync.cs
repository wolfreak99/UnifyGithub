// Original url: http://wiki.unity3d.com/index.php/WebAsync
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Networking/Networking/WebAsync.cs
// File based on original modification date of: 27 May 2013, at 23:24. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Networking.Networking
{
Author: Caue Rego (cawas) 
This is based on Ford's question which in turn is based on the MSDN library 
Description Uses threads and WebRequest to make HTTP calls. 
The initial idea was calling the HEAD to see if the URL exists. Now we also have a full "GetResponse" replacement. 
This is better than regular "WebRequest.GetResponse" because this does not lock up the system while waiting for an HTTP answer - and it even goes faster! 
Usage Easier to explain by example. Here, we use any given static method and "yield return StartCoroutine": 
	static public ThisClass Instance;
	void Awake () {
		Instance = GetComponent<ThisClass>();
	}
	static private IEnumerator CheckURL () {
		bool foundURL;
		string checkThisURL = "http://www.example.com/index.html";
		WebAsync webAsync = new WebAsync();
 
		yield return Instance.StartCoroutine( webAsync.CheckForMissingURL(checkThisURL) );
		Debug.Log("Does "+ checkThisURL  +" exist? "+ webAsync.isURLmissing);
	}We could have used it like this instead: 
		Instance.StartCoroutine( webAsync.CheckForMissingURL(checkThisURL) );
		while (! webAsync.isURLcheckingCompleted) yield return null;Now, for using the GetResponse method: 
	WebAsync webAsync = new WebAsync();
	void Update () {
		StartCoroutine( AreWeConnectedToInternet() );
	}
	private IEnumerator AreWeConnectedToInternet () {
		bool areWe;
		WebRequest requestAnyURL = HttpWebRequest.Create("http://www.example.com");
		requestAnyURL.Method = "HEAD";
 
		IEnumerator e = webAsync.GetResponse(requestAnyURL);
		while ( e.MoveNext() ) { yield return e.Current; }
 
		areWe = (webAsync.requestState.errorMessage == null);
 
		Debug.Log("Are we connected to the inter webs? "+ areWe);
	}WebAsync.cs using System;
using System.Net;
using System.Threading;
using System.Collections;
using UnityEngine;
 
/// <summary>
///  The RequestState class passes data across async calls.
/// </summary>
public class RequestState
{
	public WebRequest webRequest;
	public WebResponse webResponse;
	public string errorMessage;
 
	public RequestState ()
	{
		webRequest = null;
		webResponse = null;
		errorMessage = null;
	}
}
 
/// <summary>
/// Simplify getting web requests asynchronously
/// </summary>
public class WebAsync {
	const int TIMEOUT = 10; // seconds
 
	public bool isResponseCompleted = false;
	public RequestState requestState;
 
	public bool isURLcheckingCompleted = false;
	public bool isURLmissing = false;
 
	/// <summary>
	/// Updates the isURLmissing parameter.
	/// If the URL returns 404 or the connection is broken, it's missing. Else, we suppose it's fine.
	/// This should or can be used along with web async instance's isURLcheckingCompleted parameter
	/// inside a IEnumerator method capable of yield return for it, although it's mostly for clarity.
	/// Here's an usage example:
	/// 
	/// WebAsync webAsync = new WebAsync(); StartCoroutine( webAsync.CheckForMissingURL(url) );
	/// while (! webAsync.isURLcheckingCompleted) yield return null;
	/// bool result = webAsync.isURLmissing;
	/// 
	/// </summary>
	/// <param name='url'>
	/// A fully formated URL.
	/// </param>
	public IEnumerator CheckForMissingURL (string url) {
		isURLcheckingCompleted = false;
		isURLmissing = false;
 
		Uri httpSite = new Uri(url);
		WebRequest webRequest = WebRequest.Create(httpSite);
 
		// We need no more than HTTP's head
		webRequest.Method = "HEAD";
 
		// Get the request's reponse
		requestState = null;
 
		// Manually iterate IEnumerator, because Unity can't do it (and this does not inherit StartCoroutine from MonoBehaviour)
		IEnumerator e = GetResponse(webRequest);
		while (e.MoveNext()) yield return e.Current;
		while (! isResponseCompleted) yield return null; // this while is just to be sure and clear
 
		// Deal up with the results
		if (requestState.errorMessage != null) {
			if ( requestState.errorMessage.Contains("404") || requestState.errorMessage.Contains("NameResolutionFailure") ) {
				isURLmissing = true;
			} else {
				Debug.LogError("[WebAsync] Error trying to verify if URL '"+ url +"' exists: "+ requestState.errorMessage);
			}
		}
 
		isURLcheckingCompleted = true;
	}
 
	/// <summary>
	/// Equivalent of webRequest.GetResponse, but using our own RequestState.
	/// This can or should be used along with web async instance's isResponseCompleted parameter
	/// inside a IEnumerator method capable of yield return for it, although it's mostly for clarity.
	/// Here's an usage example:
	/// 
	/// WebAsync webAsync = new WebAsync(); StartCoroutine( webAsync.GetReseponse(webRequest) );
	/// while (! webAsync.isResponseCompleted) yield return null;
	/// RequestState result = webAsync.requestState;
	/// 
	/// </summary>
	/// <param name='webRequest'>
	/// A System.Net.WebRequest instanced var.
	/// </param>
	public IEnumerator GetResponse (WebRequest webRequest) {
		isResponseCompleted = false;
		requestState = new RequestState();
 
		// Put the request into the state object so it can be passed around
		requestState.webRequest = webRequest;
 
		// Do the actual async call here
		IAsyncResult asyncResult = (IAsyncResult) webRequest.BeginGetResponse(
			new AsyncCallback(RespCallback), requestState);
 
		// WebRequest timeout won't work in async calls, so we need this instead
		ThreadPool.RegisterWaitForSingleObject(
			asyncResult.AsyncWaitHandle,
			new WaitOrTimerCallback(ScanTimeoutCallback),
			requestState,
			(TIMEOUT *1000), // obviously because this is in miliseconds
			true
			);
 
		// Wait until the the call is completed
		while (!asyncResult.IsCompleted) { yield return null; }
 
		// Help debugging possibly unpredictable results
		if (requestState != null) {
			if (requestState.errorMessage != null) {
				// this is not an ERROR because there are at least 2 error messages that are expected: 404 and NameResolutionFailure - as can be seen on CheckForMissingURL
				Debug.Log("[WebAsync] Error message while getting response from request '"+ webRequest.RequestUri.ToString() +"': "+ requestState.errorMessage);
			}
		}
 
		isResponseCompleted = true;
	}
 
	private void RespCallback (IAsyncResult asyncResult)
	{
		WebRequest webRequest = requestState.webRequest;
 
		try {
			requestState.webResponse = webRequest.EndGetResponse(asyncResult);
		} catch (WebException webException) {
			requestState.errorMessage = "From callback, "+ webException.Message;
		}
	}
 
	private void ScanTimeoutCallback (object state, bool timedOut)
	{
		if (timedOut)  {
			RequestState requestState = (RequestState)state;
			if (requestState != null) 
				requestState.webRequest.Abort();
		} else {
			RegisteredWaitHandle registeredWaitHandle = (RegisteredWaitHandle)state;
			if (registeredWaitHandle != null)
				registeredWaitHandle.Unregister(null);
		}
	}
}
}
