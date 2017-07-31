// Original url: http://wiki.unity3d.com/index.php/Angle
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/MathHelpers/Angle.cs
// File based on original modification date of: 10 January 2012, at 20:57. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.General.MathHelpers
{
Author: Adrian 
Contents [hide] 
1 Description 
1.1 Usage 
1.2 Supported Operations 
1.3 Methods 
2 Installation 
3 History 
4 Code (Angle.boo) 

Description This is a helper class that greatly simplifies working with euler angles. It allows to calculate with angles using regular operators, compare angles with each other and automatically wraps angles. The class supports both degree and radian and will try guess the type when calculating or comparing with float values. 
Usage Angle supports both degrees and radians. When creating an Angle, you can specify the type of the Angle or use Angle's default mode. 
// Create an Angle with default type (Degree)
var a = Angle(50);
// Specify Angle type
var a = Angle(2.34, AngleMode.Radian);
// Change the default mode
Angle.defaultMode = AngleMode.RadianAngles automatically convert their type so it's possible to mix radians and degrees. 
var a = Angle(90, AngleMode.Degree);
var b = Angle(Mathf.PI / 2, AngleMode.Radian);
var c = a + b;
Debug.Log(c.Deg + " / " + c.Rad); // Prints: 180 / 3.1415...It's possible to mix regular float values with Angles. Float values will be assumed to be the same type as the Angle they interact with. 
var a = Angle(90); // Angle in degree
var b = a - 180; // Assumes 180 to be same type (degree)
Debug.Log(b) // Prints: 270 degWhen comparing angles, wrapping will be considered. The circle is split into two parts and an Angle is less if it's inside the left-hand part and bigger if it's inside the right-hand part. Smaller and bigger than, will neither match the same nor the opposite Angle, smaller and equal or bigger and equal with, both math the same and the opposite Angle. 
var a = Angle(180);
Debug.Log(90 < a); // True
Debug.Log(270 < a); // False
Debug.Log(0 < a); // False
Debug.Log(0 <= a); // TrueSupported Operations Following operations are supported. 
var a = Angle(90);
var b = Angle(90);
var c;
 
// Addition
c = a + b; // c == Angle(180)
c = a + 90; // c == Angle(180)
c = 90 + a; // c == Angle(180)
 
// Subtraction
c = a - b; // c == Angle(0)
c = a - 90; // c == Angle(0)
c = 90 - a; // c == Angle(0)
 
// Multiplication
c = a * 2; // c == Angle(180)
c = 2 * a; // c == Angle(180)
// Note: a * b is not supported since it would be of type deg^2 or rad^2. Use a * b.Deg if needed.
 
// Division
c = a / b; // c == 1
c = a / 90; // c == Angle(1)
c = 90 / a; // c == 1 (90 is assumed to be Angle(90))
 
// Modulus
c = a % b; // c == 0
c = a % 90; // c == Angle(0)
c = 90 % a; // c == 0 (90 is assumed to be Angle(90))
 
// Equality
c = (a == b); // c == true
c = (a == 90); // c == true
c = (90 == a); // c == true
 
// Inequality
c = (a != b); // c == false
c = (a != 90); // c == false
c = (90 != a); // c == false
 
// Less than
c = (a < b); // c == false
c = (a < 90); // c == false
c = (90 < a); // c == false
 
// Less than or equal
c = (a <= b); // c == true
c = (a <= 90); // c == true
c = (90 <= a); // c == true
 
// Bigger than
c = (a > b); // c == false
c = (a > 90); // c == false
c = (90 > a); // c == false
 
// Bigger than or equal
c = (a >= b); // c == true
c = (a >= 90); // c == true
c = (90 >= a); // c == true
 
// Complement (~)
c = ~a; // c == Angle(270)Methods Angle objects support following operations: 
// Create Angle with default type
var a = Angle(90);
// Create Angle with given type
a = Angle(90, AngleMode.Degree);
// Create empty Angle with type
a = Angle(AngleMode.Degree);
// Clone Angle
a = Angle(a);
 
// Get and set current Angle's mode (converts the angle)
Debug.Log(a.Mode);
a.Mode = AngleMode.Radian;
 
// Get and set angle as degrees or radians
Debug.Log(a.Deg + " / " + a.Rad);
a.Deg = 90;
a.Rad = Mathf.PI / 2;
 
// Calculations (makes no copy)
a.Add(a);
a.Add(Math.PI);
a.Subtract(a);
a.Subtract(Math.PI);
a.Multiply(2);
a.Divide(2);
a.Reverse();
 
// Difference to another Angle
a.Difference(a);
 
// Comparison
var b = Angle(180);
a.Equals(b);
a.Opposite(b);
a.LessThan(b);
a.LessThanOrEqual(b);
a.GreaterThan(b);
a.GreaterThanOrEqual(b);Installation To be able to use Angle from all your classes (JavaScript, Boo and C#), it's best to put it into your "Standard Assets/Scripts" folder. 
History 4. May 2008 
Fixed a bug in Angle.Diffrence(Angle), now wraps correctly and never exceeds 180 degrees. 
13. April 2008 
Initial release 
Code (Angle.boo) import UnityEngine
 
# Angle can be in Degree or Radian mode
enum AngleMode:
    Degree
    Radian
 
class Angle: 
 
    # Default mode
    public static defaultMode as AngleMode = AngleMode.Degree
 
    # Radian or Degree mode?
    _mode as AngleMode = AngleMode.Degree
 
    Mode as AngleMode:
        get:
            return _mode
        set:
            _angle = GetAs(value)
            _mode = value
 
    static names as Hash = {AngleMode.Degree: "deg", AngleMode.Radian: "rad"}
 
    # ---------------------------------------- #
 
    # Angle value
    _angle as double
 
    # Get and set angle as radians
    Rad as double:
        get:
            return GetAs(AngleMode.Radian)
        set:
            SetAs(value,AngleMode.Radian)
 
    # Get and set angle as degrees
    Deg as double:
        get:
            return GetAs(AngleMode.Degree)
        set:
            SetAs(value,AngleMode.Degree)
 
    # ---------------------------------------- #
 
    # Make a new angle with default mode
    def constructor(ang as double):
        SetAs(ang, Angle.defaultMode)
 
    # Make a new angle with given mode
    def constructor(ang as double, m as AngleMode):
        _mode = m
        SetAs(ang, m)
 
    # Create zero Angle with given mode
    def constructor(m as AngleMode):
        _mode = m
 
    def constructor(a as Angle):
        _mode = a.Mode
        _angle = a.GetAs(_mode)
 
    # ---------------------------------------- #
 
    # Get angle as radian or dergee
    def GetAs(m as AngleMode):
        wrap()
        if m == _mode:
            return _angle
        elif m == AngleMode.Degree:
            return _angle * Mathf.Rad2Deg
        else:
            return _angle * Mathf.Deg2Rad
 
    # Set angle as radian or degree
    def SetAs(value as double, m as AngleMode):
        if m == _mode:
            _angle = value
        elif m == AngleMode.Degree:
            _angle = value * Mathf.Deg2Rad
        else:
            _angle = value * Mathf.Rad2Deg
 
    # Covert to string with type
    def ToString():
        wrap()
        return _angle.ToString() + " " + names[_mode]
 
    # ---------------------------------------- #
 
    # Wrap the angle around for < 0 and > 2*Half()
    private def wrap():
        if _angle < 0:
            _angle += 2*Half()
        elif _angle >= 2*Half():
            _angle = _angle % (2*Half())
 
    # Convert an Angle to local mode
    private def ConvertToLocal(a as Angle):
        return a.GetAs(_mode)
 
    # Get half angle based on mode
    private def Half() as double:
        if (_mode == AngleMode.Degree):
            return 180
        else:
            return Mathf.PI
 
    # ---------------------------------------- #
 
    def Add(ang as Angle):
        _angle += ConvertToLocal(ang)
 
    def Add(ang as double):
        _angle += ang
 
    def Subtract(ang as Angle):
        _angle -= ConvertToLocal(ang)
 
    def Subtract(ang as double):
        _angle -= ang
 
    def Multiply(ang as double):
        _angle *= ang
 
    def Divide(ang as double):
        _angle /= ang
 
    def Reverse():
        _angle += Half()
 
    def Difference(ang as Angle):
        diff = Mathf.Abs(cast(single,(_angle - ConvertToLocal(ang))))
        if (diff > 180):
            diff = 360 - diff
        return Angle(diff)
 
    # ---------------------------------------- #
 
    def Equals(ang as Angle):
        wrap()
        return _angle == ConvertToLocal(ang)
 
    def Opposite(ang as Angle):
        return ((_angle + Half() == ConvertToLocal(ang)) or (_angle == ConvertToLocal(ang) + Half()))
 
    def LessThan(big as Angle):
        return not GreaterThan(big) and not Equals(big) and not Opposite(big)
 
    def LessThanOrEqual(small as Angle):
        #Debug.Log(_angle + " <= " + small)
        return not GreaterThan(small)
 
    def GreaterThan(other as Angle):
        #Debug.Log(_angle + " > " + other)
        wrap()
        small = ConvertToLocal(other)
        if ((_angle == small)
                or
            (_angle >= Half() and (small > _angle or small <= _angle - Half()))
                or
            (_angle < Half() and (small > _angle and small <= _angle + Half()))):
            return false
        return true
 
    def GreaterThanOrEqual(big as Angle):
        return not LessThan(big)
 
    # ---------------------------------------- #
 
    # Addition (+)
    static def op_Addition(ang1 as Angle, ang2 as Angle):
        ang1 = Angle(ang1)
        ang1.Add(ang2)
        return ang1
 
    static def op_Addition(ang1 as Angle, ang2 as double):
        return op_Addition(ang1,Angle(ang2,ang1.Mode))
 
    static def op_Addition(ang1 as double, ang2 as Angle):
        return op_Addition(Angle(ang1,ang2.Mode),ang2)
 
    # Subtraction (-)
    static def op_Subtraction(ang1 as Angle, ang2 as Angle):
        ang1 = Angle(ang1)
        ang1.Subtract(ang2)
        return ang1
 
    static def op_Subtraction(ang1 as Angle, ang2 as double):
        return op_Subtraction(ang1,Angle(ang2,ang1.Mode))
 
    static def op_Subtraction(ang1 as double, ang2 as Angle):
        return op_Subtraction(Angle(ang1,ang2.Mode),ang2)
 
    # Multiplication (*)
    static def op_Multiply(ang1 as Angle, mult as double):
        ang1 = Angle(ang1)
        ang1.Multiply(mult)
        return ang1
 
    static def op_Multiply(mult as double, ang as Angle):
        return op_Multiply(ang, mult)
 
    # Division (/)
    static def op_Division(ang1 as Angle, div as double):
        ang1 = Angle(ang1)
        ang1.Divide(div)
        return ang1
 
    static def op_Division(ang1 as double, div as Angle):
        return op_Division(Angle(ang1, div.Mode), div)
 
    static def op_Division(ang1 as Angle, ang2 as Angle):
        return ang1.GetAs(AngleMode.Radian) / ang2.GetAs(AngleMode.Radian)
 
    # Modulus (%)
    static def op_Modulus(ang1 as Angle, ang2 as Angle):
        return (ang1.GetAs(ang1.Mode) % ang2.GetAs(ang1.Mode))
 
    static def op_Modulus(ang1 as Angle, div as double):
        return Angle(ang1.GetAs(ang1.Mode) % div, ang1.Mode)
 
    static def op_Modulus(ang1 as double, div as Angle):
        return op_Modulus(Angle(ang1, div.Mode), div)
 
    # Equality (==)
    static def op_Equality(ang1 as Angle, ang2 as Angle):
        return ang1.Equals(ang2)
 
    static def op_Equality(ang1 as Angle, ang2 as double):
        return op_Equality(ang1,Angle(ang2,ang1.Mode))
 
    static def op_Equality(ang1 as double, ang2 as Angle):
        return op_Equality(Angle(ang1,ang2.Mode),ang2)
 
    # Inequality (!=)
    static def op_Inequality(ang1 as Angle, ang2 as Angle):
        return not ang1.Equals(ang2)
 
    static def op_Inequality(ang1 as Angle, ang2 as double):
        return op_Inequality(ang1,Angle(ang2,ang1.Mode))
 
    static def op_Inequality(ang1 as double, ang2 as Angle):
        return op_Inequality(Angle(ang1,ang2.Mode),ang2)
 
    # Less than (<)
    static def op_LessThan(small as Angle, big as Angle):
        return small.LessThan(big)
 
    static def op_LessThan(ang1 as Angle, ang2 as double):
        return op_LessThan(ang1,Angle(ang2,ang1.Mode))
 
    static def op_LessThan(ang1 as double, ang2 as Angle):
        return op_LessThan(Angle(ang1,ang2.Mode),ang2)
 
    # Less than or equal (<=)
    static def op_LessThanOrEqual(small as Angle, big as Angle):
        return small.LessThanOrEqual(big)
 
    static def op_LessThanOrEqual(ang1 as Angle, ang2 as double):
        return op_LessThanOrEqual(ang1,Angle(ang2,ang1.Mode))
 
    static def op_LessThanOrEqual(ang1 as double, ang2 as Angle):
        return op_LessThanOrEqual(Angle(ang1,ang2.Mode),ang2)
 
    # Greater than (>)
    static def op_GreaterThan(big as Angle, small as Angle):
        return big.GreaterThan(small)
 
    static def op_GreaterThan(ang1 as Angle, ang2 as double):
        return op_GreaterThan(ang1,Angle(ang2,ang1.Mode))
 
    static def op_GreaterThan(ang1 as double, ang2 as Angle):
        return op_GreaterThan(Angle(ang1,ang2.Mode),ang2)
 
    # Greater than or equal (>=)
    static def op_GreaterThanOrEqual(big as Angle, small as Angle):
        return big.GreaterThanOrEqual(small)
 
    static def op_GreaterThanOrEqual(ang1 as Angle, ang2 as double):
        return op_GreaterThanOrEqual(ang1,Angle(ang2,ang1.Mode))
 
    static def op_GreaterThanOrEqual(ang1 as double, ang2 as Angle):
        return op_GreaterThanOrEqual(Angle(ang1,ang2.Mode),ang2)
 
    # Complement (~)
    static def op_OnesComplement(ang as Angle):
        ang = Angle(ang)
        ang.Reverse()
        return ang
 
    # ---------------------------------------- #
}
