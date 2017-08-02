/*************************
 * Original url: http://wiki.unity3d.com/index.php/Easy_MySQL_Submission
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Networking/WWWScripts/Easy_MySQL_Submission.cs
 * File based on original modification date of: 20 July 2012, at 18:39. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Networking.WWWScripts
{
    This is a highly modified version of the Server Side Highscores , only adapted to perform general SQL queries , feel free to improve upon it ! First write create this C# script NOTE: You need to add a hash for security reasons , it can be found in the Server Side Highscores script , odds are Ill probably add it to this a bit latter , but I'm a bit tired right now ... It can go on any active game object , but I suggest making a separate game object for this ( Call it SQL Control ) . Edit the PHP script if you need to send data , I'm hoping I can save another person a near descent into madness getting this to work .. 
    In the inspector urlA has to be the location of the PHP script on your websever . This doesn't cover the setting up of your database , but that's covered in the first part of the Server Side Highscroes tutorial . 
    using UnityEngine;
    using System.Collections;
    public class CleanSql : MonoBehaviour {
    public string urlA ;
    public string Answer = "A" ; 
    public string QuestionNumber = "1" ;
    public string GotResult ;    
    public bool GetOnStart = true  ;
    	void New () {
     
            string url = urlA ;
     
            WWWForm form = new WWWForm();
            form.AddField("var1", Answer);
            form.AddField("var2", QuestionNumber);
             form.AddField("tempKey", "TempKey"); // USE A HASH , this is just a temp work around 
            WWW www = new WWW(url, form);
     
            StartCoroutine(WaitForRequest(www));
        }
     
    void Start ()
    	{    // we want to get it on start ?
    Debug.Log("Did you add a Hash check ?" ) ; // just a warning to add the hash in here ....		
    if( GetOnStart == true ) 
    		{
    		New ();	
    		}
     
    	}
        IEnumerator WaitForRequest(WWW www)
        {
            yield return www;
     
            // check for errors
            if (www.error == null)
            {
                Debug.Log("WWW Ok!: " + www.text);
            	After ();
    		// store this info as a string var, then you can get at it from anywhere...
                    GotResult = www.text ;
    } 
     
    		else 
     
    		{
                Debug.Log("WWW Error: "+ www.error);
            }    
        }    
     
     
     
     void After () {
            string url = urlA;
            WWW www = new WWW(url);
            StartCoroutine(WaitForRequestb(www));
        }
     
        IEnumerator WaitForRequestb(WWW www)
        {
            yield return www;
     
            // check for errors
            if (www.error == null)
            {
               // Debug.Log("WWW Ok!: " + www.text);
            } else {
                Debug.Log("WWW Error: "+ www.error);
            }    
        }
    }
    
    And here's the PHP script you need to add on your websever , as long as you can reach the URL it SHOULDN'T matter where you put it 
    <?php 
            $db = mysql_connect('host', 'user', 'pass') or die('Could not connect: ' . mysql_error()); 
            mysql_select_db('databasename') or die('Could not select database');
     
    	$getString = mysql_real_escape_string($_GET['getString'], $db);
        $gotNumber = mysql_real_escape_string($_GET['gotNumber'], $db);
        $hash = $_GET['hash'];
    	//$juststring = "box" ;
    $var1     = mysql_real_escape_string($_POST['var1']);	
    $var2     = mysql_real_escape_string($_POST['var2']);
    //SELECT * FROM table_name
     $TempKey = mysql_real_escape_string($_GET['tempKey'], $db);
    	$real_hash = md5($gotString . $secretKey); 
     $secretKey="mySecretKeyb";
     $limit = $_POST['getString'];
    //$real_hash = md5($gotString . $secretKey); 
    $locString = "SELECT  A  FROM Quiz1 WHERE Question =  " . $var2 ;
    //Compare() ; 
    //if($real_hash == $hash) {
    if ( $TempKey == "TempKey") // VERY CRUDE WORK AROUND , USE A HASH FOR REAL
    {
    $result = mysql_query("SELECT  *  FROM Quiz1 WHERE Question =  " .$var2);
    }
    // Check result
    // This shows the actual query sent to MySQL, and the error. Useful for debugging.
    //if (!$result) {
      //  $message  = 'Invalid query: ' . mysql_error() . "\n";
        //$message .= 'Whole query: ' . $query;
        //die($message);
    //}
     
    // Use result
    // Attempting to print $result won't allow access to information in the resource
    // One of the mysql result functions must be used
    // See also mysql_result(), mysql_fetch_array(), mysql_fetch_row(), etc.
    //echo $var1 ;
    while ($row = mysql_fetch_assoc($result)) {
      echo $row[$var1];
     
    }
    mysql_free_result($result);
     
     
     ?>
}
