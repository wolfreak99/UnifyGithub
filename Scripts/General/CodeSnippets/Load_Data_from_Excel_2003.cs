/*************************
 * Original url: http://wiki.unity3d.com/index.php/Load_Data_from_Excel_2003
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/CodeSnippets/Load_Data_from_Excel_2003.cs
 * File based on original modification date of: 9 January 2013, at 18:05. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.CodeSnippets
{
    Reading Excel Files and Sheets into Unity Sometimes you want the ability to load information from a spreadsheet into Unity, some rules you need to understand first. You need to ensure that your sheet is laid out like a table, such that the first row is the column headers, you want to avoid special characters and also keep the headers simple with no spaces. The next row will help determine the data type for the data being read into Unity. So the data in the cells need to match the data type you are reading for that entire column. If the column is to hold integers, make sure the first entry is an integer, not a letter. 
    There are 2 files you need to locate in Unity to copy into your assets folders. 
    First, in your windows explorer, browse to the following folder: 
    C:\Program Files\Unity\Editor\Data\Mono\lib\mono\2.0 
    Locate and copy the following 2 files to your project Assets folder: 
    System.Data.dll 
    System.EnterpriseServices.dll 
    Once these two files are in your assets folder, you are ready to begin. Create a CSharp asset, rename it to EXCELREADER and make sure the file name is also EXCELREADER.cs, then take the following code block and paste it in so we can you to read in a simple excel spreadsheet. The Excel workbook in this example is simply called Book1.xls and is located in the Assets folder of your project. The excel spreadsheet has the following layout: 
    A B C 
    1 X Y Z 
    2 1 2 1 
    3 2 3 2 
    4 3 4 3 
    
    
    
    using UnityEngine;
    using System.Collections;
    using System; 
    using System.Data; 
    using System.Data.Odbc; 
     
    public class EXCELREADER : MonoBehaviour {
     
    	// Use this for initialization
    	void Start () {
    		readXLS(Application.dataPath + "/Book1.xls");
    	}
     
    	// Update is called once per frame
    	void Update () {
     
    	}
     
    	void readXLS( string filetoread)
    	{
    		// Must be saved as excel 2003 workbook, not 2007, mono issue really
    		string con = "Driver={Microsoft Excel Driver (*.xls)}; DriverId=790; Dbq="+filetoread+";";
    		Debug.Log(con);
    		string yourQuery = "SELECT * FROM [Sheet1$]"; 
    		// our odbc connector 
    		OdbcConnection oCon = new OdbcConnection(con); 
    		// our command object 
    		OdbcCommand oCmd = new OdbcCommand(yourQuery, oCon);
    		// table to hold the data 
    		DataTable dtYourData = new DataTable("YourData"); 
    		// open the connection 
    		oCon.Open(); 
    		// lets use a datareader to fill that table! 
    		OdbcDataReader rData = oCmd.ExecuteReader(); 
    		// now lets blast that into the table by sheer man power! 
    		dtYourData.Load(rData); 
    		// close that reader! 
    		rData.Close(); 
    		// close your connection to the spreadsheet! 
    		oCon.Close(); 
    		// wow look at us go now! we are on a roll!!!!! 
    		// lets now see if our table has the spreadsheet data in it, shall we? 
     
    		if(dtYourData.Rows.Count > 0) 
    		{ 
    			// do something with the data here 
    			// but how do I do this you ask??? good question! 
    			for (int i = 0; i < dtYourData.Rows.Count; i++) 
    			{ 
    				// for giggles, lets see the column name then the data for that column! 
                    Debug.Log(dtYourData.Columns[0].ColumnName + " : " + dtYourData.Rows[i][dtYourData.Columns[0].ColumnName].ToString() + 
                        "  |  " + dtYourData.Columns[1].ColumnName + " : " + dtYourData.Rows[i][dtYourData.Columns[1].ColumnName].ToString() + 
                        "  |  " + dtYourData.Columns[2].ColumnName + " : " + dtYourData.Rows[i][dtYourData.Columns[2].ColumnName].ToString()); 
    			} 
    		} 
    	}
    }Now, when you run the code, your debug log will show the output for each row having the values in their perspective columns. 
}
