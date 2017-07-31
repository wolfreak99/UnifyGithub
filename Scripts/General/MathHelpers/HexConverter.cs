// Original url: http://wiki.unity3d.com/index.php/HexConverter
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/MathHelpers/HexConverter.cs
// File based on original modification date of: 17 November 2014, at 08:34. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.MathHelpers
{
JavaScriptfunction GetHex (decimal : int) : String
Converts int between 0 and 15 to hexidecimal character.
function HexToInt (hexChar : char) : int
Converts single hexadecimal character (0..F) to the corresponding int.
function RGBToHex (color : Color) : String
Convert a Unity color to hexadecimal. NOTE: this function assumes that color values are in the 0..1 range. Alpha values are ignored.
function HexToRGB (color : String) : Color
Convert standard web-formatted (hexadecimal) color (000000..FFFFFF) to RGB color with values in 0..1 range.

Author: Danny Lawrence 
function GetHex (decimal : int) {
	alpha = "0123456789ABCDEF";
	out = "" + alpha[decimal];
	return out;
};
 
function HexToInt (hexChar : char) {
	var hex : String = "" + hexChar;
	switch (hex) {
		case "0": return 0;
		case "1": return 1;
		case "2": return 2;
		case "3": return 3;
		case "4": return 4;
		case "5": return 5;
		case "6": return 6;
		case "7": return 7;
		case "8": return 8;
		case "9": return 9;
		case "A": return 10;
		case "B": return 11;
		case "C": return 12;
		case "D": return 13;
		case "E": return 14;
		case "F": return 15;
	}
};
 
function RGBToHex (color : Color) {
   red = color.r * 255;
   green = color.g * 255;
   blue = color.b * 255;
 
   a = GetHex(Mathf.Floor(red / 16));
   b = GetHex(Mathf.Round(red % 16));
   c = GetHex(Mathf.Floor(green / 16));
   d = GetHex(Mathf.Round(green % 16));
   e = GetHex(Mathf.Floor(blue / 16));
   f = GetHex(Mathf.Round(blue % 16));
 
   z = a + b + c + d + e + f;
 
   return z;
};
 
function HexToRGB (color : String) {
	red = (HexToInt(color[1]) + HexToInt(color[0]) * 16.000) / 255;
	green = (HexToInt(color[3]) + HexToInt(color[2]) * 16.000) / 255;
	blue = (HexToInt(color[5]) + HexToInt(color[4]) * 16.000) / 255;
	var finalColor = new Color();
	finalColor.r = red;
	finalColor.g = green;
	finalColor.b = blue;
	finalColor.a = 1;
	return finalColor;
};JavaScript ReduxAuthor: yoyo 
Note that the approach suggested below by mvi can also be applied in JavaScript, for example: 
function HexToColor(hex : String) : Color
{
    var r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
    var g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
    var b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
    return new Color32(r,g,b, 255);
}

C#Author: mvi 
// Note that Color32 and Color implictly convert to each other. You may pass a Color object to this method without first casting it.
string ColorToHex(Color32 color)
{
	string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
	return hex;
}
 
Color HexToColor(string hex)
{
	byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
	byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
	byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
	return new Color32(r,g,b, 255);
}
}
