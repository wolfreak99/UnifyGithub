/*************************
 * Original url: http://wiki.unity3d.com/index.php/Flocking
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Effects/GeneralPurposeEffectScripts/Flocking.cs
 * File based on original modification date of: 21 October 2013, at 08:56. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Effects.GeneralPurposeEffectScripts
{
    Contents [hide] 
    1 Information 
    2 Download Package 
    3 JavaScript - BoidController.js 
    4 JavaScript - BoidFlocking.js 
    5 JavaScript - BoidWatcher.js 
    6 C# edition 
    7 C# - BoidController.cs 
    8 C# - BoidFlocking.cs 
    9 C# - BoidWatcher.cs 
    
    InformationHere's a set of flocking scripts, They will follow a target, have controls for min speed, max speed, randomness. 
    Uses the physics engine and oversized sphere colliders to keep the boids apart. 
    Use the example scene in the unityPackage to figure out how to hook it all together. 
    Download PackageDownload Flocking.unityPackage.zip 
    C# version by Benoit FOULETIER (basically just converted the original, cleaned up and optimized a bit... the logic is identical.) 
    JavaScript - BoidController.jsThis script creates and collects information on the boids. It Uses the surface of the controller's collider as spawn points. 
    #pragma strict
     
    var minVelocity : float = 5;
    var maxVelocity : float = 20;
    var randomness : float = 1; 
    var flockSize : int = 20;
    var prefab : GameObject; 
    var chasee : GameObject;
     
    var flockCenter : Vector3;
    var flockVelocity : Vector3;
     
    private var boids : Array;
     
    function Start() {
        boids = new Array(flockSize);
        for (var i=0; i<flockSize; i++) {
            var position = Vector3(
                Random.value * collider.bounds.size.x,
                Random.value * collider.bounds.size.y,
                Random.value * collider.bounds.size.z
            ) - collider.bounds.extents;
     
            var boid = Instantiate(prefab, transform.position, transform.rotation);
            boid.transform.parent = transform;
            boid.transform.localPosition = position;
            boid.GetComponent.<BoidFlocking>().setController(gameObject);
            boids[i] = boid;
        }
    }
     
    function Update () {
        var theCenter = Vector3.zero;
        var theVelocity = Vector3.zero;
     
        for (var boid : GameObject in boids) {
            theCenter = theCenter + boid.transform.localPosition;
            theVelocity = theVelocity + boid.rigidbody.velocity;
        }
     
        flockCenter = theCenter/(flockSize);
        flockVelocity = theVelocity/(flockSize);
    }JavaScript - BoidFlocking.jsEach boid runs this script. It handles randomness, clumping behavior, velocity matching behavior and target following behavior. 
    This could be updated to include weighting factors for velocity matching, target following and clumping, as has been done for randomness. 
    #pragma strict
     
    private var Controller : GameObject;
     
    private var inited = false;
    private var minVelocity : float;
    private var maxVelocity : float;
    private var randomness : float;
    private var chasee : GameObject;
     
    function Start () {
        StartCoroutine("boidSteering");
    }
     
    function boidSteering () {
        while(true) {
            if (inited) {
                rigidbody.velocity = rigidbody.velocity + calc() * Time.deltaTime;
     
                // enforce minimum and maximum speeds for the boids
                var speed = rigidbody.velocity.magnitude;
                if (speed > maxVelocity) {
                    rigidbody.velocity = rigidbody.velocity.normalized * maxVelocity;
                } else if (speed < minVelocity) {
                    rigidbody.velocity = rigidbody.velocity.normalized * minVelocity;
                }
            }
     
            var waitTime = Random.Range(0.3, 0.5);
            yield WaitForSeconds(waitTime);
        }
    }
     
    function calc () {
        var randomize = Vector3((Random.value *2) -1, (Random.value * 2) -1, (Random.value * 2) -1);
     
        randomize.Normalize();
        var boidController : BoidController = Controller.GetComponent.<BoidController>();
        var flockCenter : Vector3 = boidController.flockCenter;
        var flockVelocity : Vector3 = boidController.flockVelocity;
        var follow : Vector3 = chasee.transform.localPosition;
     
        flockCenter = flockCenter - transform.localPosition;
        flockVelocity = flockVelocity - rigidbody.velocity;
        follow = follow - transform.localPosition;
     
        return (flockCenter + flockVelocity + follow*2 + randomize*randomness);
    }
     
    function setController (theController : GameObject) {
        Controller = theController;
        var boidController : BoidController = Controller.GetComponent.<BoidController>();
        minVelocity = boidController.minVelocity;
        maxVelocity = boidController.maxVelocity;
        randomness = boidController.randomness;
        chasee = boidController.chasee;
        inited = true;
    }JavaScript - BoidWatcher.jsA camera script, watches the center of the flock. 
    #pragma strict
     
    var boidController : Transform;
     
    function LateUpdate () {
        if (boidController) {
            var watchPoint : Vector3 = boidController.GetComponent.<BoidController>().flockCenter;
            transform.LookAt(watchPoint+boidController.transform.position);    
        }
    }
    
    C# editionconverted by shinriyo 
    C# - BoidController.csusing UnityEngine;
    using System.Collections;
     
    public class BoidController : MonoBehaviour
    {
        public float minVelocity = 5;
        public float maxVelocity = 20;
        public float randomness = 1;
        public int flockSize = 20;
        public GameObject prefab;
        public GameObject chasee;
     
        public Vector3 flockCenter;
        public Vector3 flockVelocity;
     
        private GameObject[] boids;
     
        void Start()
        {
            boids = new GameObject[flockSize];
            for (var i=0; i<flockSize; i++)
            {
                Vector3 position = new Vector3 (
                    Random.value * collider.bounds.size.x,
                    Random.value * collider.bounds.size.y,
                    Random.value * collider.bounds.size.z
                ) - collider.bounds.extents;
     
                GameObject boid = Instantiate(prefab, transform.position, transform.rotation) as GameObject;
                boid.transform.parent = transform;
                boid.transform.localPosition = position;
                boid.GetComponent<BoidFlocking>().SetController (gameObject);
                boids[i] = boid;
            }
        }
     
        void Update ()
        {
            Vector3 theCenter = Vector3.zero;
            Vector3 theVelocity = Vector3.zero;
     
            foreach (GameObject boid in boids)
            {
                theCenter = theCenter + boid.transform.localPosition;
                theVelocity = theVelocity + boid.rigidbody.velocity;
            }
     
            flockCenter = theCenter/(flockSize);
            flockVelocity = theVelocity/(flockSize);
        }
    }C# - BoidFlocking.csusing UnityEngine;
    using System.Collections;
     
    public class BoidFlocking : MonoBehaviour
    {
        private GameObject Controller;
        private bool inited = false;
        private float minVelocity;
        private float maxVelocity;
        private float randomness;
        private GameObject chasee;
     
        void Start ()
        {
            StartCoroutine ("BoidSteering");
        }
     
        IEnumerator BoidSteering ()
        {
            while (true)
            {
                if (inited)
                {
                    rigidbody.velocity = rigidbody.velocity + Calc () * Time.deltaTime;
     
                    // enforce minimum and maximum speeds for the boids
                    float speed = rigidbody.velocity.magnitude;
                    if (speed > maxVelocity)
                    {
                        rigidbody.velocity = rigidbody.velocity.normalized * maxVelocity;
                    }
                    else if (speed < minVelocity)
                    {
                        rigidbody.velocity = rigidbody.velocity.normalized * minVelocity;
                    }
                }
     
                float waitTime = Random.Range(0.3f, 0.5f);
                yield return new WaitForSeconds (waitTime);
            }
        }
     
        private Vector3 Calc ()
        {
            Vector3 randomize = new Vector3 ((Random.value *2) -1, (Random.value * 2) -1, (Random.value * 2) -1);
     
            randomize.Normalize();
            BoidController boidController = Controller.GetComponent<BoidController>();
            Vector3 flockCenter = boidController.flockCenter;
            Vector3 flockVelocity = boidController.flockVelocity;
            Vector3 follow = chasee.transform.localPosition;
     
            flockCenter = flockCenter - transform.localPosition;
            flockVelocity = flockVelocity - rigidbody.velocity;
            follow = follow - transform.localPosition;
     
            return (flockCenter + flockVelocity + follow * 2 + randomize * randomness);
        }
     
        public void SetController (GameObject theController)
        {
            Controller = theController;
            BoidController boidController = Controller.GetComponent<BoidController>();
            minVelocity = boidController.minVelocity;
            maxVelocity = boidController.maxVelocity;
            randomness = boidController.randomness;
            chasee = boidController.chasee;
            inited = true;
        }
    }C# - BoidWatcher.csusing UnityEngine;
    using System.Collections;
     
    public class BoidWatcher : MonoBehaviour
    {
        public Transform boidController;
     
        void LateUpdate ()
        {
            if (boidController)
            {
                Vector3 watchPoint = boidController.GetComponent<BoidController>().flockCenter;
                transform.LookAt(watchPoint+boidController.transform.position);
            }
        }
    }
}
