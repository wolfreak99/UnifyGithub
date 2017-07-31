// Original url: http://wiki.unity3d.com/index.php/ShootingWeaponScript
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Controllers/CharacterControllerScripts/ShootingWeaponScript.cs
// File based on original modification date of: 10 January 2012, at 20:47. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CharacterControllerScripts
{

Author: Markus Davey 
Contents [hide] 
1 Description 
2 Usage 
3 CSharp weaponScript.cs 
4 CSharp projectileScript.cs 

DescriptionThis script lets a weapon spawn bullets and propel them forward. It supports ballistic bullets and straight bullets. It supports random scatter of bullets. But have no off switch! Its a very early draft that was submitted to the forums. I felt that it would be a shame if it got lost in the swamp of forum posts. 
UsageAttach this script to a weapon. 
CSharp weaponScript.cs using UnityEngine;
using System.Collections;
 
/// <summary>
/// created by Markus Davey 22/11/2011
/// Basic weapon script
/// Skype: Markus.Davey
/// Unity forums: MarkusDavey
/// </summary>
 
 
public class weaponScript : MonoBehaviour 
{
    // public
    public float projMuzzleVelocity; // in metres per second
    public GameObject projPrefab;
    public float RateOfFire;
    public float Inaccuracy;
 
    // private
    private float fireTimer;
 
    // Use this for initialization
    void Start () 
    {
        fireTimer = Time.time + RateOfFire;
    }
 
    // Update is called once per frame
    void Update () 
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward, Color.red);
        if (Time.time > fireTimer)
        {
            GameObject projectile;
            Vector3 muzzlevelocity = transform.forward;
 
            if (Inaccuracy != 0)
            {
                Vector2 rand = Random.insideUnitCircle;
                muzzlevelocity += new Vector3(rand.x, rand.y, 0) * Inaccuracy;
            }
 
            muzzlevelocity = muzzlevelocity.normalized * projMuzzleVelocity;
 
            projectile = Instantiate(projPrefab, transform.position, transform.rotation) as GameObject;
            projectile.GetComponent<projectileScript>().muzzleVelocity = muzzlevelocity;
            fireTimer = Time.time + RateOfFire;
        }
        else
            return;
    }
}

CSharp projectileScript.cs using UnityEngine;
using System.Collections;
 
/// <summary>
/// created by Markus Davey 22/11/2011
/// Basic projectile script
/// Skype: Markus.Davey
/// Unity forums: MarkusDavey
/// </summary>
 
public class projectileScript : MonoBehaviour 
{
    public Vector3 muzzleVelocity;
 
    public float TTL;
 
    public bool isBallistic;
    public float Drag; // in metres/s lost per second.
 
    // Use this for initialization
    void Start () 
    {
        if (TTL == 0)
            TTL = 5;
        print(TTL);
        Invoke("projectileTimeout", TTL);
    }
 
    // Update is called once per frame
    void Update () 
    {
        if (Drag != 0)
            muzzleVelocity += muzzleVelocity * (-Drag * Time.deltaTime);
 
        if (isBallistic)
            muzzleVelocity += Physics.gravity * Time.deltaTime;
 
        if (muzzleVelocity == Vector3.zero)
            return;
        else
            transform.position += muzzleVelocity * Time.deltaTime;
        transform.LookAt(transform.position + muzzleVelocity.normalized);
        Debug.DrawLine(transform.position, transform.position + muzzleVelocity.normalized, Color.red);
    }
 
    void projectileTimeout()
    {
        DestroyObject(gameObject);
    }
 
}
}
