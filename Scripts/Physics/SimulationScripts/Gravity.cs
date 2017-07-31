// Original url: http://wiki.unity3d.com/index.php/Gravity
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Physics/SimulationScripts/Gravity.cs
// File based on original modification date of: 4 December 2015, at 19:07. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Physics.SimulationScripts
{
Provides a relatively accurate simulation of body-to-body gravity (i.e. plantary gravity). 

You'll want to set Range to a different value depending on the size of your level, etc. 
Gravity.jsvar range : float = 30.0;
 
function FixedUpdate () {
	var cols : Collider[] = Physics.OverlapSphere(transform.position, range);
	var rbs : Array = new Array();
	for (c=0;c<cols.length;c++) {
		if (cols[c].attachedRigidbody && cols[c].attachedRigidbody != rigidbody) {
			var breaking :boolean = false;
			for (r=0;r<rbs.length;r++) {
				if (cols[c].attachedRigidbody == rbs[r]) {
					breaking=true;
					break;
				}
			}
			if (breaking) continue;
			rbs.Add(cols[c].attachedRigidbody);
			var offset : Vector3 = (transform.position - cols[c].transform.position);
			var mag: float = offset.magnitude;
			cols[c].attachedRigidbody.AddForce(offset/mag/mag * rigidbody.mass);
		}
	}
}Gravity.cs[oPless] 
using UnityEngine;
using System.Collections.Generic;
 
public class Gravity : MonoBehaviour 
{	
	public static float range = 1000;
 
	void FixedUpdate () 
	{
		Collider[] cols  = Physics.OverlapSphere(transform.position, range); 
		List<Rigidbody> rbs = new List<Rigidbody>();
 
		foreach(Collider c in cols)
		{
			Rigidbody rb = c.attachedRigidbody;
			if(rb != null && rb != rigidbody && !rbs.Contains(rb))
			{
				rbs.Add(rb);
				Vector3 offset = transform.position - c.transform.position;
				rb.AddForce( offset / offset.sqrMagnitude * rigidbody.mass);
			}
		}
	}
}

Update for Unity 5 and editor gizmo that shows gravity range. 
using UnityEngine;
using System.Collections.Generic;
 
public class Gravity : MonoBehaviour
{
    public float range = 10f;
 
    Rigidbody ownRb;
 
    void Start()
    {
        ownRb = GetComponent<Rigidbody>();
    }
 
    void FixedUpdate()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, range);
        List<Rigidbody> rbs = new List<Rigidbody>();
 
        foreach (Collider c in cols)
        {
            Rigidbody rb = c.attachedRigidbody;
            if (rb != null && rb != ownRb && !rbs.Contains(rb))
            {
                rbs.Add(rb);
                Vector3 offset = transform.position - c.transform.position;
                rb.AddForce(offset / offset.sqrMagnitude * ownRb.mass);
            }
        }
    }
 
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
}
