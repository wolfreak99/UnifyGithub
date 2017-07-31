// Original url: http://wiki.unity3d.com/index.php/Floating_Origin
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CameraControls/Floating_Origin.cs
// File based on original modification date of: 8 September 2016, at 16:19. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Controllers.CameraControls
{
Contents [hide] 
1 Description 
2 Usage 
3 Notes 
4 C# - FloatingOrigin.cs 

Description This script translates all objects in the world to keep the camera near the origin in order to prevent spatial jittering due to limited floating-point precision. The script detects when the camera is further than 'threshold' units from the origin, at which point it moves everything so that the camera is back at the origin. There is also an option to disable physics beyond a certain point. 
For background on spatial jitter and how/why floating origin works refer to the PhD by Chris Thorne. 
Usage Attach this script to the GameObject that has your main camera. 
'threshold' determines the distance at which mass object translation occurs, this is not a lightweight operation so less frequent movement is better. 'physicsThreshold' determines the distance from the camera at which physics is disabled (set to zero to prevent this behaviour). 
You might wish to customize this script to optimize it to your code. One ideas might be to NOT use the scene's root as the root space for your game objects other than (say) your skydome, but to use an otherwise empty GameObject as the "scene root". In this way, rather than having to find and bump the top-level GameObjects around, you can merely move this "scene root" and they will all go along for the ride. 
Notes Make sure your other camera controls are in Update() and not LateUpdate() otherwise they could clash. If this cannot be done, you might wish to alter the script processing order to have this script run after any other camera control scripts. 
The physics disabling is a little gnarly, it would be good to have a better way than fudging the velocity & angle sleep values. It's worth noting that if you are using the physicsThreshold it will reset the velocitySleep and angularVelocitySleep values. So if you had previously set velocitySleep or angularVelocitySleep to something different for specific objects, those values will be overwritten. If this is intolerable, you could perhaps add a script to the GameObjects whose physics had to be disabled, and to store the values within it. 
C# - FloatingOrigin.cs// FloatingOrigin.cs
// Written by Peter Stirling
// 11 November 2010
// Uploaded to Unify Community Wiki on 11 November 2010
// Updated to Unity 5.x particle system by Tony Lovell 14 January, 2016
// fix to ensure ALL particles get moved by Tony Lovell 8 September, 2016
// URL: http://wiki.unity3d.com/index.php/Floating_Origin
using UnityEngine;
using System.Collections;
 
[RequireComponent(typeof(Camera))]
public class FloatingOrigin : MonoBehaviour
{
    public float threshold = 100.0f;
    public float physicsThreshold = 1000.0f; // Set to zero to disable
 
    #if OLD_PHYSICS
    public float defaultSleepVelocity = 0.14f;
    public float defaultAngularVelocity = 0.14f;
    #else
    public float defaultSleepThreshold = 0.14f;
    #endif
 
    ParticleSystem.Particle[] parts = null;
 
    void LateUpdate()
    {
        Vector3 cameraPosition = gameObject.transform.position;
        cameraPosition.y = 0f;
        if (cameraPosition.magnitude > threshold)
        {
            Object[] objects = FindObjectsOfType(typeof(Transform));
            foreach(Object o in objects)
            {
                Transform t = (Transform)o;
                if (t.parent == null)
                {
                    t.position -= cameraPosition;
                }
            }
 
            #if SUPPORT_OLD_PARTICLE_SYSTEM
            // move active particles from old Unity particle system that are active in world space
            objects = FindObjectsOfType(typeof(ParticleEmitter));
            foreach (Object o in objects)
            {
                ParticleEmitter pe = (ParticleEmitter)o;
 
                // if the particle is not in world space, the logic above should have moved them already
		if (!pe.useWorldSpace)
		    continue;
 
                Particle[] emitterParticles = pe.particles;
                for(int i = 0; i < emitterParticles.Length; ++i)
                {
                    emitterParticles[i].position -= cameraPosition;
                }
                pe.particles = emitterParticles;
            }
            #endif
 
            // new particles... very similar to old version above
            objects = FindObjectsOfType(typeof(ParticleSystem));		
            foreach (UnityEngine.Object o in objects)
            {
                ParticleSystem sys = (ParticleSystem)o;
 
		if (sys.simulationSpace != ParticleSystemSimulationSpace.World)
			continue;
 
		int particlesNeeded = sys.maxParticles;
 
		if (particlesNeeded <= 0) 
			continue;
 
		bool wasPaused = sys.isPaused;
		bool wasPlaying = sys.isPlaying;
 
		if (!wasPaused)
			sys.Pause ();
 
		// ensure a sufficiently large array in which to store the particles
		if (parts == null || parts.Length < particlesNeeded) {		
			parts = new ParticleSystem.Particle[particlesNeeded];
		}
 
		// now get the particles
		int num = sys.GetParticles(parts);
 
		for (int i = 0; i < num; i++) {
			parts[i].position -= cameraPosition;
		}
 
		sys.SetParticles(parts, num);
 
		if (wasPlaying)
			sys.Play ();
            }
 
            if (physicsThreshold > 0f)
            {
                float physicsThreshold2 = physicsThreshold * physicsThreshold; // simplify check on threshold
                objects = FindObjectsOfType(typeof(Rigidbody));
                foreach (UnityEngine.Object o in objects)
                {
                    Rigidbody r = (Rigidbody)o;
                    if (r.gameObject.transform.position.sqrMagnitude > physicsThreshold2) 
                    {
                        #if OLD_PHYSICS
                        r.sleepAngularVelocity = float.MaxValue;
                        r.sleepVelocity = float.MaxValue;
                        #else
                        r.sleepThreshold = float.MaxValue;
                        #endif
                    } 
                    else 
                    {
                        #if OLD_PHYSICS
                        r.sleepAngularVelocity = defaultSleepVelocity;
                        r.sleepVelocity = defaultAngularVelocity;
                        #else
                        r.sleepThreshold = defaultSleepThreshold;
                        #endif
                    }
                }
            }
        }
    }
}
}
