// Original url: http://wiki.unity3d.com/index.php/CSVReader
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Networking/WWWScripts/CSVReader.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Networking.WWWScripts
{

Author: Dock : This is my first time adding a script the Unity wiki :) 


Contents [hide] 
1 Info 
2 Usage 
3 Known Issues 
4 CSVReader.cs 
5 ReadDemo.cs 
6 ReadDemo.js 
7 ExampleCSV.txt 

Info This is just a simple CSVReader for reading 'Comma Separated Value' text output. It should also work with Tab separated values. It uses Regex to do some of the splitting. 
Usage Just stick it in your plugins folder and you can access it from JS or C#. 
Known Issues This will choke on data where there's newline characters within the data. I'm not sure on the best way to fix this. 
CSVReader.cs /*
	CSVReader by Dock. (24/8/11)
	http://starfruitgames.com
 
	usage: 
	CSVReader.SplitCsvGrid(textString)
 
	returns a 2D string array. 
 
	Drag onto a gameobject for a demo of CSV parsing.
*/
 
using UnityEngine;
using System.Collections;
using System.Linq; 
 
public class CSVReader : MonoBehaviour 
{
	public TextAsset csvFile; 
	public void Start()
	{
		string[,] grid = SplitCsvGrid(csvFile.text);
		Debug.Log("size = " + (1+ grid.GetUpperBound(0)) + "," + (1 + grid.GetUpperBound(1))); 
 
		DebugOutputGrid(grid); 
	}
 
	// outputs the content of a 2D array, useful for checking the importer
	static public void DebugOutputGrid(string[,] grid)
	{
		string textOutput = ""; 
		for (int y = 0; y < grid.GetUpperBound(1); y++) {	
			for (int x = 0; x < grid.GetUpperBound(0); x++) {
 
				textOutput += grid[x,y]; 
				textOutput += "|"; 
			}
			textOutput += "\n"; 
		}
		Debug.Log(textOutput);
	}
 
	// splits a CSV file into a 2D string array
	static public string[,] SplitCsvGrid(string csvText)
	{
		string[] lines = csvText.Split("\n"[0]); 
 
		// finds the max width of row
		int width = 0; 
		for (int i = 0; i < lines.Length; i++)
		{
			string[] row = SplitCsvLine( lines[i] ); 
			width = Mathf.Max(width, row.Length); 
		}
 
		// creates new 2D string grid to output to
		string[,] outputGrid = new string[width + 1, lines.Length + 1]; 
		for (int y = 0; y < lines.Length; y++)
		{
			string[] row = SplitCsvLine( lines[y] ); 
			for (int x = 0; x < row.Length; x++) 
			{
				outputGrid[x,y] = row[x]; 
 
				// This line was to replace "" with " in my output. 
				// Include or edit it as you wish.
				outputGrid[x,y] = outputGrid[x,y].Replace("\"\"", "\"");
			}
		}
 
		return outputGrid; 
	}
 
	// splits a CSV row 
	static public string[] SplitCsvLine(string line)
	{
		return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
		@"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)", 
		System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
		select m.Groups[1].Value).ToArray();
	}
}ReadDemo.cs using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class ReadDemo : MonoBehaviour {
 
	public TextAsset csv; 
 
	void Start () {
		CSVReader.DebugOutputGrid( CSVReader.SplitCsvGrid(csv.text) ); 
	}
}ReadDemo.js var csv : TextAsset; 
 
function Start ()
{
	CSVReader.DebugOutputGrid( CSVReader.SplitCsvGrid(csv.text) ); 
}ExampleCSV.txt (has to be .txt for Unity to recognise it as a textasset) 
Flavours,Strawberry,Banana,Apple,Owl
Prices,0.34,0.30,0.60,34.0
Sadness,0,0,1,99
}
