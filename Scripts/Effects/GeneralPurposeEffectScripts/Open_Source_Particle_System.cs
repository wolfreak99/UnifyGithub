// Original url: http://wiki.unity3d.com/index.php/Open_Source_Particle_System
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Effects/GeneralPurposeEffectScripts/Open_Source_Particle_System.cs
// File based on original modification date of: 10 January 2012, at 20:52. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
Author: Nick Tziamihas, Michael Moiropoulos 
Contents [hide] 
1 Summary 
2 Features 
3 Integration 
4 Notes 
5 Disclaimer 
6 Javascript- OSParticleEmitter.js 
7 Javascript- OSParticleController.js 
8 C#- OSParticleEmitter.cs 
9 C#- OSParticleController.cs 

Summary The particle system integrated in Unity3D, although conclusive, does not expose every variable and method tied to particle generarion and animation. This makes particle system modifications difficult and some times impossible. In this project, we create our own particle system from scratch, and expose all of its aspects to the developer. 
 
 
Features This sample particle system creates an ellipsoid particle emitter that can be attached to any Game Object. The sample allows local velocity, random velocity, particle color animation, billboard setting, particle speed, lifetime, quantity and angular velocity. Additional features can be added manually, as needed following the examples from this template. 
Possible applications : 
Time Scale independent particle systems (for particle systems decorating pause screens). 
Particle systems with 3D geometry (Meteor Storms, Volcanic eruptions, debris from damaged spaceships, etc). 
Attaching scripts to each particle to achieve unique behaviours (dealing damage to enemies, triggering events, etc). 
Integration Create an empty object in your scene that will act as a particle emitter. Attach the OSParticleEmitter script to it and set its particle settings in the Inspector. Create a prefab for your particle. A 2D plane mesh with a Particle Shader will produce normal particle behaviour. Using different meshes and shaders can produce more unique results (see "Possible applications" above). Drag the prefab on the Particle Object variable in the Emitter's Inspector. 
Notes The OSParticleController script works directly from the Inspector. You don't need (and it's counter-advised unless you know what you're doing) to attach it anywhere in order for it to work. It's loaded during runtime in each particle by the OSParticleEmitter script. 
Disclaimer This script is by no means a substitute for Unity's built-in particle systems. It should be used in addition to that system to achieve behaviours that cannot normally be achieved. 


Javascript- OSParticleEmitter.jsprivate var _transform : Transform;
public var emit : boolean = true;
public var particleSettings : OSParticleSettings = new OSParticleSettings();
public var particleAmmount : int = 10;
public var emissionArea : float = 5;
public var particleObject : GameObject;
 
function Start() {
	_transform = transform;
 
	if ( particleObject == null ) {
		Debug.LogError("You must assign a GameObject as Particle");
		return;
	}
	StartCoroutine( Emit() );
}
 
private function Emit() : IEnumerator {
	var timeStep : float = (particleSettings.lifeMin / particleSettings.lifeMax + particleSettings.lifeMin) / particleAmmount;
	while (true){
		if ( emit ) {
			var myRotation : Quaternion = (particleSettings.billboard) ? Quaternion.LookRotation(Camera.main.transform.position, Camera.main.transform.up) : Quaternion.identity;
			(Instantiate(particleObject,_transform.position + Random.onUnitSphere * emissionArea, myRotation) as GameObject).AddComponent.<OSParticleController>().InitParticleSettings(particleSettings, _transform);
			yield WaitForSeconds( timeStep );
		} else yield;
	}
}
 
class OSParticleSettings {
	public var lifeMin : float = 2;
	public var lifeMax : float = 2;
	public var localVelocity : Vector3;
	public var rndVelocity : Vector3;
	public var particleMinSize : float = 1;
	public var particleMaxSize : float = 1;
	public var animateColor : boolean = false;
	public var animationColor : Color[] = new Color[5];
	public var billboard : boolean = true;
	public var angularVelocity : float;
}Javascript- OSParticleController.jsprivate var _transform : Transform;
private var mainCamera : Camera;
private var emitter : Transform;
private var particleSize : Vector3;
private var particleDir : Vector3;
private var particleLife : float;
private var animateColor : boolean;
private var animationColor : Color[];
private var billboard : boolean;
private var angularVelocity : float;
private var _material : Material;
private var myAngular : float;
 
function Start() {
	mainCamera = Camera.main;
	_transform.localScale = particleSize;
	if ( animateColor )
		StartCoroutine(InitiateAnimation());
	Destroy ( _transform.gameObject, particleLife );
}
 
function Update() {		
	_transform.Translate( particleDir * Time.deltaTime, emitter );
	myAngular += angularVelocity * Time.deltaTime;
	_transform.rotation = (billboard) ? mainCamera.transform.rotation * Quaternion.Euler(myAngular,-90,90) : Quaternion.identity * Quaternion.Euler(myAngular,0,0);
}
 
private function InitiateAnimation() : IEnumerator {
 
	var timeStep : float = particleLife / animationColor.Length;
	for ( var i = 0; i < animationColor.Length; i++ ) {
		if(i < animationColor.Length - 1) {
			var thisStep : float = 0;
			while(thisStep < timeStep) {
				_material.SetColor("_TintColor", Color.Lerp(animationColor[i], animationColor[i+1], thisStep));
				_material.color = Color.Lerp(animationColor[i], animationColor[i+1], thisStep);
				thisStep += Time.deltaTime;
				yield;
			}
		} else yield WaitForSeconds(timeStep);
	}
}
 
public function InitParticleSettings( particleSettings : OSParticleSettings, _emitter : Transform ) {
	emitter = _emitter;
	_material = renderer.material;
	_transform = transform;
	particleSize = _transform.localScale;
	var pSize : float = Random.Range(particleSettings.particleMinSize, particleSettings.particleMaxSize);
	particleSize *= pSize;
	var rndVelocity : Vector3 = particleSettings.rndVelocity;
	var localVelocity : Vector3 = particleSettings.localVelocity;
 
	particleDir.x = Random.Range(-rndVelocity.x + localVelocity.x, rndVelocity.x + localVelocity.x);
	particleDir.y = Random.Range(-rndVelocity.y + localVelocity.y, rndVelocity.y + localVelocity.y);
	particleDir.z = Random.Range(-rndVelocity.z + localVelocity.z, rndVelocity.z + localVelocity.z);
 
	particleLife = Random.Range(particleSettings.lifeMin, particleSettings.lifeMax);
	animateColor = particleSettings.animateColor;
	animationColor = particleSettings.animationColor;
	billboard = particleSettings.billboard;
	angularVelocity = particleSettings.angularVelocity;
	gameObject.hideFlags = HideFlags.HideInHierarchy;
 
}C#- OSParticleEmitter.csusing UnityEngine;
using System.Collections;
 
public class OSParticleEmitter : MonoBehaviour {
	private Transform _transform;
 
	public bool emit = true;
	public OSParticleSettings particleSettings = new OSParticleSettings();
 
	public int particleAmmount = 10;
	public float emissionArea = 5;
	public GameObject particleObject;
 
	void Start() {
		_transform = transform;
 
		if ( particleObject == null ) {
			Debug.LogError("You must assign a GameObject as Particle");
			return;
		}
 
		StartCoroutine( Emit() );
	}
 
	IEnumerator Emit() {
		float timeStep = (particleSettings.lifeMin / particleSettings.lifeMax + particleSettings.lifeMin) / particleAmmount;
 
		while (true){
			if ( emit ) {
				Quaternion myRotation;
				myRotation = (particleSettings.billboard) ? Quaternion.LookRotation(Camera.main.transform.position, Camera.main.transform.up) : Quaternion.identity;
				(Instantiate(particleObject,_transform.position + Random.onUnitSphere * emissionArea, myRotation) as GameObject)
				.AddComponent<OSParticleController>().InitParticleSettings(particleSettings, _transform);
 
				yield return new WaitForSeconds( timeStep );
			} else yield return null;
		}
	}
}
 
[System.Serializable]
public class OSParticleSettings {
	public float lifeMin = 2;
	public float lifeMax = 2;
	public Vector3 localVelocity;
	public Vector3 rndVelocity;
	public float particleMinSize = 1;
	public float particleMaxSize = 1;
	public bool animateColor = false;
	public Color[] animationColor = new Color[5];
	public bool billboard = true;
	public float angularVelocity;
}C#- OSParticleController.csusing UnityEngine;
using System.Collections;
 
public class OSParticleController : MonoBehaviour {
	private Transform _transform;
	private Camera mainCamera;
	private Transform emitter;
 
	private Vector3 particleSize;
	private Vector3 particleDir;
	private float particleLife;
	private bool animateColor = false;
	private Color[] animationColor;
	private bool billboard = true;
	private float angularVelocity;
	private Material _material;
 
	private float myAngular;
 
	void Start() {
		_transform.localScale = particleSize;
		if ( animateColor )
			StartCoroutine(InitiateAnimation());
 
		Destroy ( _transform.gameObject, particleLife );
	}
 
	void Update() {		
		_transform.Translate( particleDir * Time.deltaTime, emitter );
		myAngular += angularVelocity * Time.deltaTime;
		_transform.rotation = (billboard) ? mainCamera.transform.rotation * Quaternion.Euler(myAngular,-90,90) : Quaternion.identity * Quaternion.Euler(myAngular,0,0);
	}
 
	private IEnumerator InitiateAnimation() {
		float timeStep = particleLife / animationColor.Length;
 
		for ( int i = 0; i < animationColor.Length; i++ ) {
			if(i < animationColor.Length - 1) {
				float thisStep = 0;
				while(thisStep < timeStep) {
					_material.SetColor("_TintColor", Color.Lerp(animationColor[i], animationColor[i+1], thisStep));
					_material.color = Color.Lerp(animationColor[i], animationColor[i+1], thisStep);
					thisStep += Time.deltaTime;
					yield return null;
				}
			} else yield return new WaitForSeconds(timeStep);
		}
	}
 
	public void InitParticleSettings( OSParticleSettings particleSettings, Transform _emitter ) {
		emitter = _emitter;
		_material = renderer.material;
		_transform = transform;
		mainCamera = Camera.main;		
		particleSize = _transform.localScale;
		float pSize = Random.Range(particleSettings.particleMinSize, particleSettings.particleMaxSize);
		particleSize *= pSize;
 
		Vector3 rndVelocity = particleSettings.rndVelocity;
		Vector3 localVelocity = particleSettings.localVelocity;
 
		particleDir.x = Random.Range(-rndVelocity.x + localVelocity.x, rndVelocity.x + localVelocity.x);
		particleDir.y = Random.Range(-rndVelocity.y + localVelocity.y, rndVelocity.y + localVelocity.y);
		particleDir.z = Random.Range(-rndVelocity.z + localVelocity.z, rndVelocity.z + localVelocity.z);
 
		particleLife = Random.Range(particleSettings.lifeMin, particleSettings.lifeMax);
		animateColor = particleSettings.animateColor;
		animationColor = particleSettings.animationColor;
		billboard = particleSettings.billboard;
		angularVelocity = particleSettings.angularVelocity;
		gameObject.hideFlags = HideFlags.HideInHierarchy;
 
	}
}
}
