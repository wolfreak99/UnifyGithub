// Original url: http://wiki.unity3d.com/index.php/LevelUp
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/ReallySimpleScripts/LevelUp.cs
// File based on original modification date of: 23 July 2016, at 18:47. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.ReallySimpleScripts
{
Author: Divinux 
Contents [hide] 
1 Description 
2 Usage 
3 C# Experience.cs 
4 JavaScript Experience.js 
5 Credit 
6 History 

Description Provides the basic formula for leveling up characters based on experience points. 
Usage Just drop the script onto your character or a manager GameObject. Now, how you implement it depends on your game. Just call GainExp(int) from whatever object/action you want to reward your player for. Dying enemy, found item, destroyed building, you get the idea. 
Once the player has accumulated enough experience, LvlUp() is called automatically. Add whatever cool stuff should happen on level up here. You know, trumpet sounds, sparkly particles, increase player strength,... 
Oh, and don't forget to add a juicy UI experience bar! Just in case, the progress bar should have a width of x *(vCurrExp/vExpLeft), with x being the maximum width of the progress bar. 
C# Experience.cs using UnityEngine;
using System.Collections;
 
public class Experience : MonoBehaviour 
{
 
    //current level
    public int vLevel = 1;
    //current exp amount
    public int vCurrExp = 0;
    //exp amount needed for lvl 1
    public int vExpBase = 10;
    //exp amount left to next levelup
    public int vExpLeft = 10;
    //modifier that increases needed exp each level
    public float vExpMod = 1.15f;
 
    //vvvvvv USED FOR TESTING FEEL FREE TO DELETE
    void Update () 
    {
        if (Input.GetButtonDown("Jump"))
        {
            GainExp(3);
        }
    }
    //^^^^^^ USED FOR TESTING FEEL FREE TO DELETE
 
    //leveling methods
    public void GainExp(int e)
    {
        vCurrExp += e;
        if(vCurrExp >= vExpLeft)
        {
            LvlUp();
        }
    }
    void LvlUp()
    {
        vCurrExp -= vExpLeft;
        vLevel++;
        float t = Mathf.Pow(vExpMod, vLevel);
        vExpLeft = (int)Mathf.Floor(vExpBase * t);
    }
}JavaScript Experience.js import UnityEngine;
import System.Collections;
 
 
 
//current level
public var vLevel: int = 1;
//current exp amount
public var vCurrExp: int = 0;
//exp amount needed for lvl 1
public var vExpBase: int = 10;
//exp amount left to next levelup
public var vExpLeft: int = 10;
//modifier that increases needed exp each level
public var vExpMod: float = 1.15f;
 
//vvvvvv USED FOR TESTING FEEL FREE TO DELETE
function Update () 
{
    if (Input.GetButtonDown("Jump"))
    {
        GainExp(3);
    }
}
//^^^^^^ USED FOR TESTING FEEL FREE TO DELETE
 
//leveling methods
public function GainExp(e:int)
{
    vCurrExp += e;
    if(vCurrExp >= vExpLeft)
    {
        LvlUp();
    }
}
function LvlUp()
{
    vCurrExp -= vExpLeft;
    vLevel++;
    var t:float = Mathf.Pow(vExpMod, vLevel);
    vExpLeft = Mathf.FloorToInt(vExpBase * t);
}Credit I can't find the original forum even with google unfortunately. But I saved myself the crucial bit of the thread in a text file: 
The general formula for the jump in *experience needed* between levels was E=A(B^C), E being experience needed for this level, 
A the experience needed at level one, B the percentage increase you want (such as 1.2 for 20%), and C being whatever level you're at now.So thanks for that, random internet guy. 
History 15 July 2016 - Release 
}
