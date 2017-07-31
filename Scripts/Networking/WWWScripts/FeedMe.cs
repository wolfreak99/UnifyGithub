// Original url: http://wiki.unity3d.com/index.php/FeedMe
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Networking/WWWScripts/FeedMe.cs
// File based on original modification date of: 10 January 2012, at 20:45. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Networking.WWWScripts
{
Author: Corrupted Heart : I thought I should put some of my scripts on here since it provides a more centralized area for them than on a blog who knows where. 
Contents [hide] 
1 Info 
2 PHP Editor Script 
3 FeedMe.cs 
4 WriteFeed.cs 
5 XML Structure 

Info FeedMe is a simple XML displayer that utilizes TinyXmlReader to parse the document which is downloaded from a server. The current version (0.7) package offers the read and write scripts as prefabs for easy entry into a game. 
 
PHP Editor Script I was going to post the php script however it seems to be causes errors for the wiki so you can find the script at my blog: http://www.corruptedheart.co.cc/2010/07/serve-and-feed-me.html 
FeedMe.cs For reading the feeds into your app. 
//<author>Garth "Corrupted Heart" de Wet</author>
//<email>mydeathofme[at]gmail[dot]com</email>
//<summary>Contains methods in order to get feeds from an xml document located on a server</summary>
//<license>Creative Commons:
//http://creativecommons.org/licenses/by/3.0/
//FeedMe by Garth "Corrupted Heart" de Wet is licensed under a Creative Commons Attribution 3.0 Unported License.
//Permissions beyond the scope of this license may be available at www.CorruptedHeart.co.cc
//</license>
using UnityEngine;
using System.Collections;
 
public class FeedMe : MonoBehaviour
{
#region Variables
#region URLS
	// The URL to where the feed is located
	public string getFeedURL = "http://localhost/feed.xml";
#endregion
	// A Struc of what every feed is made up of
	private struct feedThis
	{
    	public string Title;
    	public string Message;
		public string Date;
		public string Author;
	}
#region Feed Variables
	// Should the feed only update once per game play
	public bool updateOnce = false;
	// How long till the next update
	public int minutesToUpdate = 60;
	// This is used to make sure that the text loaded doesn't fill more than the array
	private int feedLength = 9;
	// This is the array of a struc of variables used to hold the feeds
	private feedThis[] theFeed = new feedThis[9];
	// When the feed is updated the time is set here plus the minutes to update
	private float timeSinceLastUpdate = 0;
	// if the feed must be updated
	private bool updateFeed = true;
	// if the feed is updating
	private bool updating = false;
	// if there is no errors from the WWW connection
	private bool errorFree = true;
	// used to measure the number of elements within the array
	private int i = 0;
#endregion
 
#region GUI
	// Skin for gui
	public GUISkin mySkin;
	// Keeps position of the scroll position
	private Vector2 scrollPosition;
	// The size and position of the feed window - The first two fields are the only ones required
	public Rect feedWinRect = new Rect(0,30,250,50);
	// the actual width of the feed window
	public int winWidth = 250;
	// the size of the feed button
	public int feedBtnWidth = 190;
	// the size of the reload button
	public int reloadBtnWidth = 30;
	// whether the feed window is being displayed or not
	private bool showFeed = false;
	// The icon to display on the feed button
	public Texture2D feedIcon;
	// The icon to display on the reload button
	public Texture2D reloadIcon;
	private GUIContent reloadBtn;
#endregion
#endregion
#region Standard Methods
	void Start()
	{
		// This adds a bit of error checking to see if there is a icon attached to the reloadIcon variable
		// and if there is none then it changes the button size to accomodate the extra width of the words reload
		// and fills the GUIContent with the words reload
		if(reloadIcon)
		{
			reloadBtn = new GUIContent(reloadIcon);
		}
		else
		{
			reloadBtn = new GUIContent("Reload");
			reloadBtnWidth = 50;
		}
		// Starts the getFeed Coroutine
		StartCoroutine(getFeed());
	}
 
	void Update()
	{
		// If it is set to update only once then it skips the next piece
		if(!updateOnce)
		{
			// Checks whether the feed needs to be updated
			if((Time.time >= timeSinceLastUpdate) && (!updateFeed))
			{
				// Resets the vars
				updateFeed = true;
				i = 0;
				// Starts the getFeed Coroutine
				StartCoroutine(getFeed());
			}
		}
	}
 
	void OnGUI()
	{
		// Creates the GUIContent for the button because it needs to be created for the error checking to work
		// correctly
		GUIContent feedBtn = new GUIContent("Feed: ");
		// creates an empty string for the feed button title
		string Feed = "";
		// If there was a skin set then set it else leave it as default
		if(mySkin != null)
		{
			GUI.skin = mySkin;
		}
		//To show that the feed is updating
		if(updating)
		{
			Feed =  "Is updating...";
		}
		// if the feed isn't updating show the latest feed title as button caption
		else
		{
			Feed = theFeed[0].Title;
		}
		// If there is an icon in the feedIcon var then it fills the GUIContent created above with the feed string and 
		// icon. Otherwise it creates a string representation of feed
		if(feedIcon != null)
		{	// Sets an icon and the feed string to a GUIContent
			feedBtn = new GUIContent("   " + Feed, feedIcon);
		}
		else
		{
			feedBtn = new GUIContent("Feed: " + Feed);
		}
		// Button for showing the field
		if(GUI.Button(new Rect(0,0,feedBtnWidth, 30),feedBtn))
		{
			// On click it simply switches the feed window on and off
			showFeed = !showFeed;
		}		
		// Shows the feed window based upon the value of showFeed
		if(showFeed)
		{
			feedWinRect = GUILayout.Window(1, feedWinRect, theFeedWindow, "Feeds", GUILayout.Width(winWidth));
		}
		// An update now button
		if(GUI.Button(new Rect(feedBtnWidth, 0, reloadBtnWidth, 30), reloadBtn))
		{
			// Resets the vars
			updateFeed = true;
			i = 0;
			// Starts the getFeed Coroutine
			StartCoroutine(getFeed());
		}
	}
#endregion
#region Custom Functions
	IEnumerator getFeed()
	{
		// if the feed must be updated run the function
		if(updateFeed)
		{
			// Sets updating to true
			updating = true;
			// Gets the data from the url set in the inspector
			WWWForm form = new WWWForm();
			form.AddField("number", 1221);
    		WWW feedPost = new WWW(getFeedURL, form);
			// Returns control to rest of program until it is finished downloading
    		yield return feedPost;
			// if there are no errors runs through the function
			if(feedPost.error == null)
			{
				// Creates a instance of TinyXmlReader to loop through the xml document
				// loops through the document and extracts data from each <Feed></Feed>
				// For my use, it extracts the title, date, author and message.
				// it stores this in an array of struc type declared above
				TinyXmlReader reader = new TinyXmlReader(feedPost.data);
				while(reader.Read("Feeds") && (i < feedLength))
				{
					while(reader.Read("Feed"))
					{
						if(reader.tagName == "Title" && reader.isOpeningTag)
						{
							theFeed[i].Title = reader.content;
						}
						if(reader.tagName == "Date" && reader.isOpeningTag)
						{
							theFeed[i].Date = reader.content;
						}
						if(reader.tagName == "Author" && reader.isOpeningTag)
						{
							theFeed[i].Author = reader.content;
						}
						if(reader.tagName == "Message" && reader.isOpeningTag)
						{
							theFeed[i].Message = reader.content;
						}
					}
					// Increments the counter variable for the array
					i++;
				}
			}
			// if there was an error
			else
			{
				// sets all the error messages
				errorFree = false;
				theFeed[0].Title = "Error connecting to Feed...";
				theFeed[0].Message = feedPost.error;
				// Logs an error
				Debug.LogError(feedPost.error);
			}
			// sets the values of the variables to there off state
			updateFeed = false;
			updating = false;
			// sets the last update time
			timeSinceLastUpdate = Time.time + (60 * minutesToUpdate);
			// Destroys the feedPost...
			feedPost.Dispose();
		}
	}
#endregion
#region GUI Functions
	// The feed window GUI
	void theFeedWindow(int WindowID)
	{
		// Creates a empty string to hold all the feeds
		string Feeds = "";
		// Starts a scroll view
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(winWidth), GUILayout.Height(400));
		// if there are no errors, shows the feed as is
		if(errorFree)
		{
			// loops through the array adding the feed data to the feed string
			for(int x=0;x<=i-1;x++)
			{
				Feeds = Feeds + theFeed[x].Title + " - " + theFeed[x].Date + "\n"
						+ "Author: " + theFeed[x].Author + "\n" + theFeed[x].Message + "\n\n";
			}
		}
		// if there is an error sets the feed to just 0 and not a loop
		else
		{
			Feeds = theFeed[0].Title + "\n" + theFeed[0].Message;
		}
		// Creates a label from the feed string
		GUILayout.Label(Feeds);
		// ends the view
		GUILayout.EndScrollView();
	}
#endregion
}WriteFeed.cs For writing feeds from within your app. 
//<author>Garth "Corrupted Heart" de Wet</author>
//<email>mydeathofme[at]gmail[dot]com</email>
//<summary>Contains methods in order to get feeds from an xml document located on a server</summary>
//<license>Creative Commons:
//http://creativecommons.org/licenses/by/3.0/
//FeedMe by Garth "Corrupted Heart" de Wet is licensed under a Creative Commons Attribution 3.0 Unported License.
//Permissions beyond the scope of this license may be available at www.CorruptedHeart.co.cc
//</license>
using UnityEngine;
using System.Collections;
 
public class WriteFeed : MonoBehaviour
{
#region Variables
#region URLS
	// The URL to where the feed is located
	public string NewFeedURL = "http://localhost/create.php";
#endregion
	// A Struc of what every feed is made up of
	private struct FeedThis
	{
    	public string Title;
    	public string Message;
		public string Author;
	}
#region Feed Variables
	// This is the array of a struc of variables used to hold the feeds
	private FeedThis NewFeed = new FeedThis();
	// if the feed must be updated
	private bool Updated = false;
	// if the feed is updating
	private bool Updating = false;
	// if there is no errors from the WWW connection
	private bool ErrorFree = true;
	private string FeedBack = "";
#endregion
#region GUI
	// Skin for gui
	public GUISkin MySkin;
	// Keeps position of the scroll position
	private Vector2 ScrollPosition;
	// The size and position of the feed window - The first two fields are the only ones required
	public Rect FeedWinRect = new Rect(270,30,250,400);
	// The size and position of the feed window - The first two fields are the only ones required
	public Rect FeedBtnWidth = new Rect(220,0,35,30);
	// The width of the window
	public int FeedWidth = 250;
	// The height of the window
	public int FeedHeight = 400;
	// whether the feed window is being displayed or not
	private bool ShowFeed = false;
	// The icon to display on the feed button
	public Texture2D PostFeedIcon;
#endregion
#endregion
#region Standard Methods
	void Start()
	{
		// Setting the feed to empty in order to allow the input to work
		NewFeed.Author = "";
		NewFeed.Message = "";
		NewFeed.Title = "";
	}
 
	void OnGUI()
	{
		// If there was a skin set then set it else leave it as default
		if(MySkin != null)
		{
			GUI.skin = MySkin;
		}
		// Button for showing the field
		if(GUI.Button(FeedBtnWidth,PostFeedIcon))
		{
			// On click it simply switches the feed window on and off
			ShowFeed = !ShowFeed;
		}
		// Shows the feed window based upon the value of showFeed
		if(ShowFeed)
		{
			// creates the window
			FeedWinRect = GUILayout.Window(2, FeedWinRect, theFeedWindow, "New Feed", GUILayout.Width(FeedWidth));
		}
	}
#endregion
#region Custom Functions
	IEnumerator PostFeed()
	{
		// Sets updating to true
		Updating = true;
 
		// Creates a WWWForm in order to fill in the details that will be submitted to the php script
		WWWForm theFeed = new WWWForm();
		// Adds the Title
		theFeed.AddField("title", NewFeed.Title);
		// Adds the message
		theFeed.AddField("message", NewFeed.Message);
		// Adds the name
		theFeed.AddField("name", NewFeed.Author);
 
		// Sends the form and gets the reply from the server
   		WWW feedPost = new WWW(NewFeedURL, theFeed);
		// Returns control to rest of program until it is finished downloading
   		yield return feedPost;
		// if there are no errors runs through the function
		if(feedPost.error == null)
		{
			FeedBack = feedPost.data;
			Updated = true;
		}
		// if there was an error
		else
		{
			// sets all the error messages
			ErrorFree = false;
			// Logs an error
			Debug.LogError(feedPost.error);
			FeedBack = feedPost.error;
		}
		// sets the values of the variables to there off state
		Updating = false;
		// Destroys the feedPost...
		feedPost.Dispose();
	}
#endregion
#region GUI Functions
	// The feed window GUI
	void theFeedWindow(int WindowID)
	{
		// Starts a scroll view
		ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.Width(FeedWidth), GUILayout.Height(FeedHeight));
		// if it is not error free create a label with the error code
		if(!ErrorFree)
		{
			GUILayout.Label("Error: " + FeedBack);
		}
		// if it was updated then clear the inputs and put a label with the answer from the server
		if(Updated)
		{
			NewFeed.Author = "";
			NewFeed.Message = "";
			NewFeed.Title = "";
			Updated = false;
		}
		// if it is currently updating then display that in a label or else display the input area
		if(Updating)
		{
			GUILayout.Label("Feed is being updated...");
		}
		else
		{
			GUILayout.Label(FeedBack);
			GUILayout.Label("Enter Your Name:");
			NewFeed.Author = GUILayout.TextField(NewFeed.Author);
			GUILayout.Label("Enter The Title:");
			NewFeed.Title = GUILayout.TextField(NewFeed.Title);
			GUILayout.Label("Enter Your Message:");
			NewFeed.Message = GUILayout.TextArea(NewFeed.Message);
 
			if(GUILayout.Button("Submit"))
			{
				Updated = false;
				StartCoroutine(PostFeed());
			}
 
			if(GUILayout.Button("Reset"))
			{
				NewFeed.Author = "";
				NewFeed.Message = "";
				NewFeed.Title = "";
			}
		}
		// ends the view
		GUILayout.EndScrollView();
	}
#endregion
}XML Structure <xml><Feeds>     <Feed>          <Title></Title>          <Date></Date>          <Author></Author>          <Message></Message>     </Feed> </Feeds></xml> 
}
