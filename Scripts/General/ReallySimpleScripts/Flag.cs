// Original url: http://wiki.unity3d.com/index.php/Flag
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/General/ReallySimpleScripts/Flag.cs
// File based on original modification date of: 16 February 2012, at 04:55. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.ReallySimpleScripts
{
Author: Berenger 
Contents [hide] 
1 Description 
1.1 Usage 
1.2 Supported Operations 
2 Installation 
3 History 
4 Code (Flag.cs) 

Description Provide simple bitwise logical functions, turning a flag off and on, toggle and check. 
Usage In it's current form, Flag is object designed, but it could easily be completely static. You just have to add "static" to the function and pass the flag by reference. 
	f.TurnOn( 1 << 3 ); // 9
	f.TurnOn( 1 << 5 ); // 41
	f.TurnOff( 1 << 3 ); // 33
	f.TurnOn( 1 << 2 ); // 37
	f.Toggle( 1 << 2 ); // 33
	f.Toggle( 1 << 2 ); // 37
	print( f.Check( 1 << 5 ) ); // true
	print( f.Check( 1 << 6 ) ); // falseSupported Operations Following operations are supported. 
 
	//Add the mask
	public int TurnOn( int mask )
 
	//Remove the mask
	public int TurnOff( int mask )
 
	//Toggle the mask
	public int Toggle( int mask )
 
	//Check if mask is on
	public bool Check( int mask )Installation To be able to use Flag from all your classes (JavaScript, Boo and C#), it's best to put it into your "Standard Assets" folder. 
History 15 February 2012 
Initial release 
Code (Flag.cs) public class Flag
{		
	public Flag() : this( 0 ){}
	public Flag( int mask ){ m_Value = mask; }
 
	public int value{ get{ return m_Value; } }
	private int m_Value;
 
	//Add the mask to flags
	public int TurnOn( int mask ){
		return m_Value |= mask;
	}
 
	//Remove the mask from flags
	public int TurnOff( int mask ){
		return m_Value &= ~mask;
	}
 
	//Toggle the mask into flags
	public int Toggle( int mask ){
		return m_Value ^= mask;
	}
 
	//Check if mask is on
	public bool Check( int mask ){
		return ( m_Value & mask ) == mask;
	}
}
}
