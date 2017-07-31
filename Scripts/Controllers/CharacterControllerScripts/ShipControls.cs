// Original url: http://wiki.unity3d.com/index.php/ShipControls
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CharacterControllerScripts/ShipControls.cs
// File based on original modification date of: 10 January 2012, at 20:57. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CharacterControllerScripts
{
Author: Jonathan Czeck (aarku) 
Contents [hide] 
1 Description 
2 Usage 
3 C# - ShipControls.cs 
4 Boo - ShipControls.boo 
5 JavaScript - ShipControls.js 

Description This script performs 2D spaceship controls like OverWhelmed Arena that you can tweak to your heart's content. The input to the script, player or bot, is abstracted. This means you can have a Bot script in conjunction with this one that will drive the ship. 
Usage Place this script on a GameObject with a RigidBody component on it. 
C# - ShipControls.cs // Removed two depreciated functions.  Used Deg2Rad and back again because I don't know what I'm doing.  Anyone is welcome to clean this up properly.  -WarpZone
 
using UnityEngine;
using System.Collections;
 
// Put this on a rigidbody object and instantly
// have 2D spaceship controls like OverWhelmed Arena
// that you can tweak to your heart's content.
 
[RequireComponent (typeof (Rigidbody))]
public class ShipControls : MonoBehaviour
{
    public float hoverHeight = 3F;
    public float hoverHeightStrictness = 1F;
    public float forwardThrust = 5000F;
    public float backwardThrust = 2500F;
    public float bankAmount = 0.1F;
    public float bankSpeed = 0.2F;
    public Vector3 bankAxis = new Vector3(-1F, 0F, 0F);
    public float turnSpeed = 8000F;
 
    public Vector3 forwardDirection = new Vector3(1F, 0F, 0F);
 
    public float mass = 5F;
 
    // positional drag
    public float sqrdSpeedThresholdForDrag = 25F;
    public float superDrag = 2F;
    public float fastDrag = 0.5F;
    public float slowDrag = 0.01F;
 
    // angular drag
    public float sqrdAngularSpeedThresholdForDrag = 5F;
    public float superADrag = 32F;
    public float fastADrag = 16F;
    public float slowADrag = 0.1F;
 
    public bool playerControl = true;
 
    float bank = 0F;
 
    void SetPlayerControl(bool control)
    {
        playerControl = control;
    }
 
    void Start()
    {
        rigidbody.mass = mass;
    }
 
    void FixedUpdate()
    {
        if (Mathf.Abs(thrust) > 0.01F)
        {
            if (rigidbody.velocity.sqrMagnitude > sqrdSpeedThresholdForDrag)
                rigidbody.drag = fastDrag;
            else
                rigidbody.drag = slowDrag;
        }
        else
            rigidbody.drag = superDrag;
 
        if (Mathf.Abs(turn) > 0.01F)
        {
            if (rigidbody.angularVelocity.sqrMagnitude > sqrdAngularSpeedThresholdForDrag)
                rigidbody.angularDrag = fastADrag;
            else
                rigidbody.angularDrag = slowADrag;
        }
        else
            rigidbody.angularDrag = superADrag;
 
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, hoverHeight, transform.position.z), hoverHeightStrictness);
 
        float amountToBank = rigidbody.angularVelocity.y * bankAmount;
 
        bank = Mathf.Lerp(bank, amountToBank, bankSpeed);
 
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation *= Mathf.Deg2Rad;
        rotation.x = 0F;
        rotation.z = 0F;
        rotation += bankAxis * bank;
        //transform.rotation = Quaternion.EulerAngles(rotation);
         rotation *= Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(rotation);
    }
 
    float thrust = 0F;
    float turn = 0F;
 
    void Thrust(float t)
    {
        thrust = Mathf.Clamp(t, -1F, 1F);
    }
 
    void Turn(float t)
    {
        turn = Mathf.Clamp(t, -1F, 1F) * turnSpeed;
    }
 
    bool thrustGlowOn = false;
 
    void Update ()
    {
        float theThrust = thrust;
 
        if (playerControl)
        {
            thrust = Input.GetAxis("Vertical");
            turn = Input.GetAxis("Horizontal") * turnSpeed;
        }
 
        if (thrust > 0F)
        {
            theThrust *= forwardThrust;
            if (!thrustGlowOn)
            {
                thrustGlowOn = !thrustGlowOn;
                BroadcastMessage("SetThrustGlow", thrustGlowOn, SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            theThrust *= backwardThrust;
            if (thrustGlowOn)
            {
                thrustGlowOn = !thrustGlowOn;
                BroadcastMessage("SetThrustGlow", thrustGlowOn, SendMessageOptions.DontRequireReceiver);
            }
        }
 
        rigidbody.AddRelativeTorque(Vector3.up * turn * Time.deltaTime);
        rigidbody.AddRelativeForce(forwardDirection * theThrust * Time.deltaTime);
    }
}Boo - ShipControls.boo This is a direct, verbatim conversion from the C# version. No attempt has been made to use Boo features. 
import UnityEngine
import System.Collections
 
[RequireComponent (Rigidbody)]
class ShipControls (MonoBehaviour):
    public hoverHeight = 3F
    public hoverHeightStrictness = 1F
    public forwardThrust = 5000F
    public backwardThrust = 2500F
    public bankAmount = 0.1F
    public bankSpeed = 0.2F
    public bankAxis = Vector3(-1F, 0F, 0F)
    public turnSpeed = 8000F
 
    public forwardDirection = Vector3(1F, 0F, 0F)
 
    public mass = 5F
 
    // positional drag
    public sqrdSpeedThresholdForDrag = 25F
    public superDrag = 2F
    public fastDrag = 0.5F
    public slowDrag = 0.01F
 
    // angular drag
    public sqrdAngularSpeedThresholdForDrag = 5F
    public superADrag = 32F
    public fastADrag = 16F
    public slowADrag = 0.1F
 
    public playerControl = true
 
    public bank = 0F
 
    def SetPlayerControl(control as bool):
        playerControl = control
 
    def Start():
        rigidbody.mass = mass
 
    def FixedUpdate():
        if Mathf.Abs(thrust) > 0.01:
            if rigidbody.velocity.sqrMagnitude > sqrdSpeedThresholdForDrag:
                rigidbody.drag = fastDrag
            else:
                rigidbody.drag = slowDrag
        else:
            rigidbody.drag = superDrag
 
        if Mathf.Abs(turn) > 0.01:
            if rigidbody.angularVelocity.sqrMagnitude > sqrdAngularSpeedThresholdForDrag:
                rigidbody.angularDrag = fastADrag
            else:
                rigidbody.angularDrag = slowADrag
        else:
            rigidbody.angularDrag = superADrag
 
        transform.position = Vector3.Lerp(transform.position, Vector3(transform.position.x, hoverHeight, transform.position.z), hoverHeightStrictness)
 
        amountToBank as single = rigidbody.angularVelocity.y * bankAmount
 
        bank = Mathf.Lerp(bank, amountToBank, bankSpeed)
 
        rotation = transform.rotation.ToEulerAngles()
        rotation.x = 0F
        rotation.z = 0F
        rotation += bankAxis * bank
        transform.rotation = Quaternion.EulerAngles(rotation)
 
    thrust = 0F
    turn = 0F
 
    def Thrust(t as single):
        turn = Mathf.Clamp(t, -1F, 1F) * turnSpeed
 
    def Turn(t as single):
        turn = Mathf.Clamp(t, -1F, 1F) * turnSpeed
 
    thrustGlowOn = false
 
    def Update ():
        theThrust = thrust
 
        if (playerControl):
            thrust = Input.GetAxis("Vertical")
            turn = Input.GetAxis("Horizontal") * turnSpeed
 
        if (thrust > 0.0):
            theThrust *= forwardThrust
            if (not thrustGlowOn):
                thrustGlowOn = not thrustGlowOn
                BroadcastMessage("SetThrustGlow", thrustGlowOn, SendMessageOptions.DontRequireReceiver)
        else:
            theThrust *= backwardThrust
            if (thrustGlowOn):
                thrustGlowOn = not thrustGlowOn
                BroadcastMessage("SetThrustGlow", thrustGlowOn, SendMessageOptions.DontRequireReceiver)
 
        rigidbody.AddRelativeTorque(Vector3.up * turn * Time.deltaTime)
        rigidbody.AddRelativeForce(forwardDirection * theThrust * Time.deltaTime)JavaScript - ShipControls.js And here's a JavaScript version. Just because. 
// js adaptation by WarpZone
 
// Put this on a rigidbody object and instantly
// have 2D spaceship controls like OverWhelmed Arena
// that you can tweak to your heart's content.
 
@script RequireComponent (Rigidbody)
 
   	var hoverHeight : float = 3;
    var hoverHeightStrictness : float = 1.0;
    var  forwardThrust : float = 5000.0;
    var  backwardThrust : float = 2500.0;
    var  bankAmount : float = 0.1;
    var  bankSpeed : float = 0.2;
    var  bankAxis : Vector3 = new Vector3(-1.0, 0.0, 0.0);
    var  turnSpeed : float = 8000.0;
 
    var  forwardDirection : Vector3 = new Vector3(0.0, 0.0, 1.0);
 
    var  mass : float = 5.0;
 
    // positional drag
    var sqrdSpeedThresholdForDrag : float = 25.0;
    var superDrag : float = 2.0;
    var fastDrag : float = 0.5;
    var slowDrag : float = 0.01;
 
    // angular drag
    var sqrdAngularSpeedThresholdForDrag : float = 5.0;
    var superADrag : float = 32.0;
    var fastADrag : float = 16.0;
    var slowADrag : float = 0.1;
 
 
 
    var playerControl : boolean = true;
 
    private var bank : float = 0.0;
 
	function SetPlayerControl(control : boolean)
    {
        playerControl = control;
    }
 
 
    function Start()
    {
        gameObject.rigidbody.mass = mass;
    }
 
    function FixedUpdate()
    {
        if (Mathf.Abs(thrust) > 0.01)
        {
            if (rigidbody.velocity.sqrMagnitude > sqrdSpeedThresholdForDrag)
                rigidbody.drag = fastDrag;
            else
                rigidbody.drag = slowDrag;
        }
        else
            rigidbody.drag = superDrag;
 
        if (Mathf.Abs(turn) > 0.01)
        {
            if (rigidbody.angularVelocity.sqrMagnitude > sqrdAngularSpeedThresholdForDrag)
                rigidbody.angularDrag = fastADrag;
            else
                rigidbody.angularDrag = slowADrag;
        }
        else
            rigidbody.angularDrag = superADrag;
 
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, hoverHeight, transform.position.z), hoverHeightStrictness);
 
        var amountToBank : float = rigidbody.angularVelocity.y * bankAmount;
 
        bank = Mathf.Lerp(bank, amountToBank, bankSpeed);
 
        var rotation : Vector3 = transform.rotation.eulerAngles;
        rotation *= Mathf.Deg2Rad;
        rotation.x = 0;
        rotation.z = 0;
        rotation += bankAxis * bank;
        rotation *= Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(rotation);
    }
 
    // thrust
    private var thrust : float = 0;
    private var turn : float = 0;
 
 	function Thrust( t : float )
    {
        thrust = Mathf.Clamp( t, -1.0, 1.0 );
    }
 
    function Turn(t : float)
    {
        turn = Mathf.Clamp( t, -1.0, 1.0 ) * turnSpeed;
    }
 
    private var thrustGlowOn : boolean = false;
 
    function Update ()
    {
        var theThrust : float = thrust;
 
        if (playerControl)
        {
            thrust = Input.GetAxis("Vertical");
            turn = Input.GetAxis("Horizontal") * turnSpeed;
        }
 
        if (thrust > 0.0)
        {
            theThrust *= forwardThrust;
            if (!thrustGlowOn)
            {
                thrustGlowOn = !thrustGlowOn;
                BroadcastMessage("SetThrustGlow", thrustGlowOn, SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            theThrust *= backwardThrust;
            if (thrustGlowOn)
            {
                thrustGlowOn = !thrustGlowOn;
                BroadcastMessage("SetThrustGlow", thrustGlowOn, SendMessageOptions.DontRequireReceiver);
            }
        }
 
        rigidbody.AddRelativeTorque(Vector3.up * turn * Time.deltaTime);
        rigidbody.AddRelativeForce(forwardDirection * theThrust * Time.deltaTime);
}
</javascript >
}
