// Original url: http://wiki.unity3d.com/index.php/SQLite
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/GeneralConcepts/SQLite.cs
// File based on original modification date of: 5 April 2014, at 01:21. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.GeneralConcepts
{
SQLite is a convenient way of implementing a simple database in Unity. Rather than a full-blown client-server based implementation of SQL, SQLite uses a single local file to store the database, and provides access to that file via standard SQL commands. 
Advantages of SQLite: 
Easy setup in Javascript 
Easy to view database structure and contents with the free SQLite Browser 
Maintains state over sessions (since it's a local file) 
Disadvantages of SQLite: 
Since it's using a local file for the database, it is NOT possible to use it for Web Player applications 
It's a bit finicky to get set up with in C# 
(Advanced) It's doesn't guarantee domain integrity - not usually a problem, since Unity's single threaded nature makes it probably impossible to write two things at the same time, but you might have issues if you're trying to write to your database from a program outside of Unity at the same time. 
So how do you set it up, and how do you start working with SQLite in Unity? For now, this guide only focuses on Javascript, since it's easier to set up. And as long as your database-access functions return acceptable types, you can access all the Javascript functions from C# or Boo so long as you put your database scripts in the 'Plugins' or 'Standard Assets' folder of your project, and don't try to access them by scripts that are earlier in the compiler order 




Contents [hide] 
1 JavaScript 
1.1 dbAccess.js 
1.2 ScriptThatUsesTheDatabase.js 
1.3 Troubleshooting 

JavaScript Here are the specific steps to getting SQLite set up in your project. 
Download SQLite - you'll want the ZIP file with the DLL inside that's in the Precompiled Binaries for Windows section. 
Important Copy sqlite3.dll into your into your project's Plugins folder (make a folder called Plugins if you don't have one). 
You won't get a warning if you don't do this, and your project will run fine in the editor, however, it will fail to work when you actually build your project, and will only provide information about this in the log file. 
This will give you a License Error if you're using Unity Indie, but it doesn't seem to have an effect on the actual play in the editor, nor does it seem to effect the ability to build stand-alone versions. 
Alternately, you can leave it out of your project entirely, but when you build your application, you'll need to include a copy of sqlite3.dll in the same directory as the .exe in order for it to work. 
In your project, add in the dbAccess.js file below. 
You should be good to go! An example using the database is also included - ScriptThatUsesTheDatabase.js - It is a GUI script, so attach it to your main camera. 
The commands run by the dbAccess class are IDbCommand commands, and those commands return an IDataReader object. For more information on using those interfaces, the references are here: IDbCommand and IDataReader 
dbAccess.js #pragma strict
 
/*  Javascript class for accessing SQLite objects.  
     To use it, you need to make sure you COPY Mono.Data.SQLiteClient.dll from wherever it lives in your Unity directory
     to your project's Assets folder
     Originally created by dklompmaker in 2009
     http://forum.unity3d.com/threads/28500-SQLite-Class-Easier-Database-Stuff    
     Modified 2011 by Alan Chatham
     Modified 2014 by Shinsuke Sugita           */
import System.Data;  // we import our  data class
import Mono.Data.Sqlite; // we import sqlite
import System.Collections.Generic;
 
class dbAccess {
    // variables for basic query access
    private var connection : String;
    private var dbcon : IDbConnection;
    private var dbcmd : IDbCommand;
    private var reader : IDataReader;
 
    function OpenDB(p : String) {
    connection = "URI=file:" + p; // we set the connection to our database
    dbcon = new SqliteConnection(connection);
    dbcon.Open();
    }
 
    function BasicQuery(q : String, r : boolean):IDataReader{ // run a baic Sqlite query
        dbcmd = dbcon.CreateCommand(); // create empty command
        dbcmd.CommandText = q; // fill the command
        reader = dbcmd.ExecuteReader(); // execute command which returns a reader
        if(r) { // if we want to return the reader
            return reader; // return the reader
        }
    }
 
    // This returns a 2 dimensional ArrayList with all the
    //  data from the table requested
    function ReadFullTable(tableName : String) {
        var query : String;
        query = "SELECT * FROM " + tableName;
        dbcmd = dbcon.CreateCommand();
        dbcmd.CommandText = query; 
        reader = dbcmd.ExecuteReader();
        var readArray = new ArrayList();
        while(reader.Read()) { 
            var lineArray = new ArrayList();
            for (var i:int = 0; i < reader.FieldCount; i++)
                lineArray.Add(reader.GetValue(i)); // This reads the entries in a row
            readArray.Add(lineArray); // This makes an array of all the rows
        }
        return readArray; // return matches
    }
 
    // This function deletes all the data in the given table.  Forever.  WATCH OUT! Use sparingly, if at all
    function DeleteTableContents(tableName : String) {
    var query : String;
    query = "DELETE FROM " + tableName;
    dbcmd = dbcon.CreateCommand();
    dbcmd.CommandText = query; 
    reader = dbcmd.ExecuteReader();
    }
 
    function CreateTable(name : String, col : Array, colType : Array) { // Create a table, name, column array, column type array
        var query : String;
        query  = "CREATE TABLE " + name + "(" + col[0] + " " + colType[0];
        for(var i=1; i<col.length; i++) {
            query += ", " + col[i] + " " + colType[i];
        }
        query += ")";
        dbcmd = dbcon.CreateCommand(); // create empty command
        dbcmd.CommandText = query; // fill the command
        reader = dbcmd.ExecuteReader(); // execute command which returns a reader
 
    }
 
    function InsertIntoSingle(tableName : String, colName : String, value : String) { // single insert 
        var query : String;
        query = "INSERT INTO " + tableName + "(" + colName + ") " + "VALUES (" + value + ")";
        dbcmd = dbcon.CreateCommand(); // create empty command
        dbcmd.CommandText = query; // fill the command
        reader = dbcmd.ExecuteReader(); // execute command which returns a reader
    }
 
    function InsertIntoSpecific(tableName : String, col : Array, values : Array) { // Specific insert with col and values
        var query : String;
        query = "INSERT INTO " + tableName + "(" + col[0];
        for(var i=1; i<col.length; i++) {
            query += ", " + col[i];
        }
        query += ") VALUES (" + values[0];
        for(i=1; i<values.length; i++) {
            query += ", " + values[i];
        }
        query += ")";
        dbcmd = dbcon.CreateCommand();
        dbcmd.CommandText = query; 
        reader = dbcmd.ExecuteReader();
    }
 
    function InsertInto(tableName : String, values : Array) { // basic Insert with just values
        var query : String;
        query = "INSERT INTO " + tableName + " VALUES (" + values[0];
        for(var i=1; i<values.length; i++) {
            query += ", " + values[i];
        }
        query += ")";
        dbcmd = dbcon.CreateCommand();
        dbcmd.CommandText = query; 
        reader = dbcmd.ExecuteReader(); 
    }
 
    // This function reads a single column
    //  wCol is the WHERE column, wPar is the operator you want to use to compare with, 
    //  and wValue is the value you want to compare against.
    //  Ex. - SingleSelectWhere("puppies", "breed", "earType", "=", "floppy")
    //  returns an array of matches from the command: SELECT breed FROM puppies WHERE earType = floppy;
    //function SingleSelectWhere(tableName : String, itemToSelect : String, wCol : String, wPar : String, wValue : String):Array { // Selects a single Item
    function SingleSelectWhere(tableName : String, itemToSelect : String, wCol : String, wPar : String, wValue : String):List.<String>{ // Selects a single Item
        var query : String;
        query = "SELECT " + itemToSelect + " FROM " + tableName + " WHERE " + wCol + wPar + wValue; 
        dbcmd = dbcon.CreateCommand();
        dbcmd.CommandText = query; 
        reader = dbcmd.ExecuteReader();
        //var readArray = new Array();
        var readArray:List.<String> = new List.<String>();
        while(reader.Read()) { 
            //readArray.Push(reader.GetString(0)); // Fill array with all matches
            var japanese:String = reader.GetString(0);
            Debug.Log(japanese);
            readArray.Add(japanese); // Fill array with all matches
            var url:String = reader.GetString(1);
            Debug.Log(url);
            readArray.Add(url); // Fill array with all matches
        }
        return readArray; // return matches
    }
 
    function CloseDB() {
        reader.Close(); // clean everything up
        reader = null; 
        dbcmd.Dispose(); 
        dbcmd = null; 
        dbcon.Close(); 
        dbcon = null; 
    }
}ScriptThatUsesTheDatabase.js #pragma strict
 
/*  Script for testing out SQLite in Javascript
          2011 - Alan Chatham
          Released into the public domain
 
        This script is a GUI script - attach it to your main camera.
        It creates/opens a SQLite database, and with the GUI you can read and write to it.
                                        */
 
// This is the file path of the database file we want to use
// Right now, it'll load TestDB.sqdb in the project's root folder.
// If one doesn't exist, it will be automatically created.
public var DatabaseName : String = "TestDB.sqdb";
 
// This is the name of the table we want to use
public var TableName : String = "TestTable";
var db : dbAccess;
 
function Start() {
    // Give ourselves a dbAccess object to work with, and open it
    db = new dbAccess();
    db.OpenDB(DatabaseName);
    // Let's make sure we've got a table to work with as well!
    var tableName = TableName;
    var columnNames = new Array("firstName","lastName");
    var columnValues = new Array("text","text");
    try {
        db.CreateTable(tableName,columnNames,columnValues);
    }
    catch(e) {// Do nothing - our table was already created
        //- we don't care about the error, we just don't want to see it
    }
}
 
// These variables just hold info to display in our GUI
var firstName : String = "First Name";
var lastName : String = "Last Name"; 
var DatabaseEntryStringWidth = 100;
var scrollPosition : Vector2;
var databaseData : ArrayList = new ArrayList();
 
// This GUI provides us with a way to enter data into our database
//  as well as a way to view it
function OnGUI() {
    GUI.Box(Rect (25,25,Screen.width - 50, Screen.height - 50),""); 
    GUILayout.BeginArea(Rect(50, 50, Screen.width - 100, Screen.height - 100));
    // This first block allows us to enter new entries into our table
        GUILayout.BeginHorizontal();
            firstName = GUILayout.TextField(firstName, GUILayout.Width (DatabaseEntryStringWidth));
            lastName = GUILayout.TextField(lastName, GUILayout.Width (DatabaseEntryStringWidth));
        GUILayout.EndHorizontal();
 
        if (GUILayout.Button("Add to database")) {
            // Insert the data
            InsertRow(firstName,lastName);
            // And update the readout of the database
            databaseData = ReadFullTable();
        }
        // This second block gives us a button that will display/refresh the contents of our database
        GUILayout.BeginHorizontal();
            if (GUILayout.Button ("Read Database")) 
                databaseData = ReadFullTable();
            if (GUILayout.Button("Clear"))
                databaseData.Clear();
        GUILayout.EndHorizontal();
 
        GUILayout.Label("Database Contents");
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(100));
            for (var line : ArrayList in databaseData) {
                GUILayout.BeginHorizontal();
                for (var s in line) {
                    GUILayout.Label(s.ToString(), GUILayout.Width(DatabaseEntryStringWidth));
                }
                GUILayout.EndHorizontal();
            }
 
        GUILayout.EndScrollView();
        if (GUILayout.Button("Delete All Data")) {
            DeleteTableContents();
            databaseData = ReadFullTable();
        }
 
    GUILayout.EndArea();
}
 
// Wrapper function for inserting our specific entries into our specific database and table for this file
function InsertRow(firstName:String, lastName:String) {
    var values = new Array(("'"+firstName+"'"),("'"+lastName+"'"));
    db.InsertInto(TableName, values);
}
 
// Wrapper function, so we only mess with our table.
function ReadFullTable() {
    return db.ReadFullTable(TableName);
}
 
// Another wrapper function...
function DeleteTableContents() {
    db.DeleteTableContents(TableName);
}Troubleshooting Everything works fine in the editor, but when I make a build, my SQL stuff doesn't work. 
For whatever reason, the sqlite3.dll file needs to be in the Plugins directory of your project. 
}
