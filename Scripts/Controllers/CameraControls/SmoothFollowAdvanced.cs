/*************************
 * Original url: http://wiki.unity3d.com/index.php/SmoothFollowAdvanced
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Controllers/CameraControls/SmoothFollowAdvanced.cs
 * File based on original modification date of: 10 January 2012, at 20:47. 
 *
 * Author: Daniel P. Rossi (DR9885) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Controllers.CameraControls
{
    Contents [hide] 
    1 Download 
    2 Description 
    3 Usage 
    4 TODO: 
    5 C# Scripts 
    5.1 C# - SmoothFollowAdvanced.cs 
    5.2 C# - CameraBumper.cs 
    5.3 C# - CameraControl.cs 
    5.4 C# - LimitedFloat.cs 
    5.5 C# - DetectionTrigger.cs 
    5.6 C# - UnityObject 
    
    Download Media:SmoothFollowAdvanced.zip 
    DescriptionThis is a Camera script designed in a singleton pattern, to allow programmers have easy access to camera control. It is also built to make implementation easy without scripting. 
    This scrip includes ideas of a bumper from the SmoothFollowWithCameraBumper script. It also has the flow of SmoothFollow2 script. Built off these two ideas, I have included a control set. I have also limits to among many other cool features. 
    NOTE: Currently Collider Functionality is Broken, so Please use Raycasting. 
    Usage Attach This script to the Main Camera, then set the target. 
    To use in scripting call "SmoothCameraAdvanced.FocusOn(TRANSFORM)" or "SmoothCameraAdvanced.ResetPosition()". Also you may also access the static properties as this is a Singleton class. 
    TODO: Fix Collision Using Colliders Instead of Raycasting 
    C# Scripts C# - SmoothFollowAdvanced.cs using UnityEngine;
    using System;
    using System.Collections.Generic;
     
     
    [AddComponentMenu("Camera-Control/SmoothCameraAdvanced")]
    class SmoothCameraAdvanced : MonoBehaviour
    {
        #region Private Properties
     
        private static SmoothCameraAdvanced instance = null;
        public static SmoothCameraAdvanced Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType(typeof(SmoothCameraAdvanced)) as SmoothCameraAdvanced;
                if (instance == null && Camera.main != null)
                    instance = Camera.main.gameObject.AddComponent(typeof(SmoothCameraAdvanced)) as SmoothCameraAdvanced;
                return instance;
            }
        }
     
        // Unity property "transform" calls GetComponent(Transform), so we store the value
        private Transform ourCameraTransform;
        private static Transform CameraTransform
        {
            get
            {
                if (Instance.ourCameraTransform == null)
                {
                    Instance.ourCameraTransform = Instance.transform;
                    Bumper.Ignores.Add(Instance.ourCameraTransform);
                }
                return Instance.ourCameraTransform;
            }
        }
     
        #endregion
     
        #region Public Properties
     
        [SerializeField] private Transform target = null;
        public static Transform Target
        {
            set
            {
                // Remove Prior Target
                Bumper.Ignores.Remove(Instance.target);
     
                Instance.target = value;
     
                // Add New Target
                Bumper.Ignores.Add(Instance.target);
            }
            get { return Instance.target; }
        }
     
        [SerializeField] public CameraBumper bumper;
        public static CameraBumper Bumper
        {
            get { return Instance.bumper; }
            set { Instance.bumper = value; }
        }
     
        [SerializeField] List<CameraControl> controls = new List<CameraControl>();
        public static List<CameraControl> Controls
        {
            get { return Instance.controls; }
            set { Instance.controls = value; }
        }
     
     
        #region Offset Properties
     
        [SerializeField] private Vector3 lookAtOffset; // allows offsetting of camera lookAt, very useful for low bumper heights
        public static Vector3 LookAtOffset
        {
            get { return Instance.lookAtOffset; }
            set { Instance.lookAtOffset = value; }
        }
     
        private Vector3 runtimeOffset = Vector3.zero;
        private static Vector3 RuntimeOffset
        {
            set { PanX.Current = value.x; }
            get { return new Vector3(PanX.Current, PanY.Current,0); }
        }
     
        #endregion
     
        #region MovementType Properties
     
        public enum MovementType { Instant, LinearInterpolation, SphericalLinearInterpolation }
     
        [SerializeField] private MovementType rotationType = MovementType.SphericalLinearInterpolation;
        private static MovementType RotationType
        {
            get { return Instance.rotationType; }
            set { Instance.rotationType = value; }
        }
     
        [SerializeField] private MovementType translationType = MovementType.SphericalLinearInterpolation;
        public static MovementType TranslationType
        {
            get { return Instance.translationType; }
            set { Instance.translationType = value; }
        }
     
        #endregion
     
        #region Position Properties
     
        [SerializeField] private LimitedFloat distance = new LimitedFloat(3.0f, 1, 10);
        public static LimitedFloat Distance
        {
            get { return Instance.distance; }
            set { Instance.distance = value; }
        }
     
        [SerializeField] private LimitedFloat height = new LimitedFloat(1.0f, 1, 5);
        public static LimitedFloat Height
        {
            get { return Instance.height; }
            set { Instance.height = value; }
        }
     
        [SerializeField] private LimitedFloat panX = new LimitedFloat(0.0f, -1, 1);
        public static LimitedFloat PanX
        {
            get { return Instance.panX; }
            set { Instance.panX = value; }
        }
     
        [SerializeField] private LimitedFloat panY = new LimitedFloat(0.0f, 0, 2);
        public static LimitedFloat PanY
        {
            get { return Instance.panY; }
            set { Instance.panY = value; }
        }
     
        #endregion
     
        #region Damping Properties
     
        [SerializeField] private float damping = 5.0f;
        public static float Damping
        {
            get { return Instance.damping; }
            set { Instance.damping = value; }
        }
     
        [SerializeField] private float rotationDamping = 10.0f;
        public static float RotationDamping
        {
            get { return Instance.rotationDamping; }
            set { Instance.rotationDamping = value; }
        }
     
        #endregion
     
        #endregion
     
        #region Unity Methods
     
        void Reset()
        {
            Controls.Add(new CameraControl(TargetEnum.Distance, MouseCodeEnum.ScrollWheel, 2));
            Controls.Add(new CameraControl(TargetEnum.Height, MouseCodeEnum.ScrollWheel, 1));
            Controls.Add(new CameraControl(TargetEnum.PanY, MouseCodeEnum.ScrollWheel, 0.5f));
            Controls.Add(new CameraControl(TargetEnum.PanX, KeyCode.LeftArrow, -1));
            Controls.Add(new CameraControl(TargetEnum.PanX, KeyCode.RightArrow, 1));
        }
     
        private void Awake()
        {
            if (target)
                FocusOn(target);
        }
     
        public void Update()
        {
            UpdateControls();
     
            UpdateCameraPosition();
        }
     
        #endregion
     
        #region Private Methods
     
        private void UpdateControls()
        {
            // Update Controls
     
            foreach (CameraControl control in controls)
            {
                // Handle Buttons
                if(control.Value != 0)
                    switch (control.Target)
                    {
                        case TargetEnum.Distance: Distance += control.Value; break;
                        case TargetEnum.Height: Height += control.Value; break;
                        case TargetEnum.PanX: PanX += control.Value; break;
                        case TargetEnum.PanY: PanY += control.Value; break;
                        //case TargetEnum.Pivot: Pivot += control.Value; break;
                    }
            }
        }
     
        private void UpdateCameraPosition()
        {
            Vector3 wantedPosition = target.TransformPoint(PanX.Current, height.Current, -distance.Current);
     
            // Only Update When Changes are Present
            if (wantedPosition != CameraTransform.position)
            {
                wantedPosition = Bumper.UpdatePosition(target, CameraTransform, wantedPosition, Time.deltaTime * damping);
     
                switch(translationType)
                {
                    case MovementType.Instant: CameraTransform.position = wantedPosition; break;
                    case MovementType.LinearInterpolation:
                        CameraTransform.position = Vector3.Lerp(CameraTransform.position, wantedPosition, Time.deltaTime * damping);
                        break;
                    case MovementType.SphericalLinearInterpolation:
                        CameraTransform.position = Vector3.Slerp(CameraTransform.position, wantedPosition, Time.deltaTime * damping);
                        break;
                }
     
                /// Rotate Camera
                Vector3 lookPosition = target.TransformPoint(lookAtOffset + RuntimeOffset);
                Quaternion wantedRotation = Quaternion.LookRotation(lookPosition - CameraTransform.position, target.up);
     
                switch (rotationType)
                {
                    case MovementType.Instant:
                        CameraTransform.rotation = Quaternion.LookRotation(lookPosition - CameraTransform.position, target.up);
                        break;
                    case MovementType.LinearInterpolation:
                        CameraTransform.rotation = Quaternion.Lerp(CameraTransform.rotation, wantedRotation, Time.deltaTime * rotationDamping);
                        break;
                    case MovementType.SphericalLinearInterpolation:
                        CameraTransform.rotation = Quaternion.Slerp(CameraTransform.rotation, wantedRotation, Time.deltaTime * rotationDamping);
                        break;
                }
            }
        }
     
        #endregion
     
        #region Public Methods
     
        public static void FocusOn(Transform argTarget)
        {
            Target = argTarget;
     
            CameraTransform.parent = Target;
     
            UnityObject Object = Target.GetComponent(typeof(UnityObject)) as UnityObject;
            if (Object)
            {
                float myYOffSet = Object.Renderer.bounds.size.y;
                LookAtOffset = Vector3.up * myYOffSet;
                Height.Current = myYOffSet + .5f;
                Distance.Current = myYOffSet + 1;
                PanY.Current = 0;
                PanX.Current = 0;
            }
        }
     
        public static void ResetPosition()
        {
            UnityObject Object = Target.GetComponent(typeof(UnityObject)) as UnityObject;
            float myYOffSet = Object.Renderer.bounds.size.y;
            LookAtOffset = Vector3.up * myYOffSet;
            PanX.Current = 0;
        }
     
        #endregion
    }
    
    
    
    C# - CameraBumper.cs using System;
    using UnityEngine;
    using System.Collections.Generic;
     
    [Serializable]
    class CameraBumper
    {
        #region Fields
     
        private RaycastHit hit;
        private bool isColliderHit;
     
        #endregion
     
        #region Properties
     
        #region Private Properties
     
        private GameObject ourBumper;
        private GameObject Bumper
        {
            get
            {
                if (ourBumper == null)
                    ourBumper = new GameObject("Bumper");
                return ourBumper;
            }
            set { ourBumper = value; }
        }
     
        private DetectionTrigger ourDetectionTrigger;
        private DetectionTrigger DetectionTrigger
        {
            get
            {
                if (ourDetectionTrigger == null)
                    ourDetectionTrigger = Bumper.AddComponent(typeof(DetectionTrigger)) as DetectionTrigger;
                return ourDetectionTrigger;
            }
        }
     
     
        #endregion
     
        #region Public Properties
     
        public enum CollisionType { None, Raycast, Collider }
        [SerializeField] private CollisionType collisionType = CollisionType.Raycast; // length of bumper ray
        public CollisionType Collision
        {
            get { return collisionType; }
            set { collisionType = value; }
        }
     
        [SerializeField] private float distanceCheck = 2.5f; // length of bumper
        public float DistanceCheck
        {
            get { return distanceCheck; }
            set { distanceCheck = value; }
        }
     
        [SerializeField] private float newCameraHeight = 1.0f; // adjust camera height while bumping
        public float NewCameraHeight
        {
            get { return newCameraHeight; }
            set { newCameraHeight = value; }
        }
     
        [SerializeField] private Vector3 offset = Vector3.zero; // allows offset of the bumper ray from target origin
        public Vector3 Offset
        {
            get { return offset; }
            set { offset = value; }
        }
     
        private List<Transform> ourIgnores = new List<Transform>();
        public List<Transform> Ignores
        {
            get { return ourIgnores; }
            set { ourIgnores = value; }
        }
     
        private List<Type> ourIgnoreTypes = new List<Type>();
        public List<Type> IgnoreTypes
        {
            get { return ourIgnoreTypes; }
            set { ourIgnoreTypes = value; }
        }
        #endregion
     
        #endregion
     
        #region Methods
     
        private bool IsBumperHit(Transform argTarget, Transform argCamera)
        {
            switch (collisionType)
            {
                case CollisionType.Collider:
                    Bumper.transform.position = argTarget.position + offset + (-1 * argTarget.forward);
                    DetectionTrigger.Ignores = Ignores;
                    DetectionTrigger.IgnoreTypes = IgnoreTypes;
                    return DetectionTrigger.IsTripped;
                case CollisionType.Raycast:
                    // check to see if there is anything behind the target
                    Vector3 back = argTarget.transform.TransformDirection(-1 * Vector3.forward);
     
                    // cast the bumper ray out from rear and check to see if there is anything behind
                    return Physics.Raycast(argTarget.TransformPoint(offset), back, out hit, distanceCheck)
                        && hit.transform != argTarget;
                default: return false;
            }
        }
     
        public Vector3 UpdatePosition(Transform argTarget, Transform argCamera, Vector3 argWantedPosition, float argT)
        {
            if (IsBumperHit(argTarget, argCamera))
            {
                argWantedPosition.x = hit.point.x;
                argWantedPosition.z = hit.point.z;
                argWantedPosition.y = Mathf.Lerp(hit.point.y + newCameraHeight, argWantedPosition.y, argT);
            }
            return argWantedPosition;
        }
     
        #endregion
    }
    
    
    
    
    
    
    
    C# - CameraControl.cs using UnityEngine;
    using System;
     
    public enum TargetEnum { Height, Distance, PanX, PanY, Pivot }
    public enum MouseCodeEnum { None, ScrollWheel, X, Y }
     
    [Serializable]
    public class CameraControl
    {
        #region Properties 
     
        [SerializeField] private TargetEnum target = TargetEnum.Distance;
        public TargetEnum Target
        {
            get { return target; }
            set { target = value; }
        }
     
        [SerializeField] private float stepSize = 1.0f;
        public float StepSize
        {
            get { return this.stepSize; }
            set { this.stepSize = value; }
        }
     
        #region Input Controls
     
        [SerializeField] private MouseCodeEnum mouseCode = MouseCodeEnum.None;
        public MouseCodeEnum MouseCode
        {
            get { return mouseCode; }
            set { mouseCode = value; }
        }
     
        [SerializeField] private KeyCode keyCode = KeyCode.None;
        public KeyCode KeyCode
        {
            get { return keyCode; }
            set { keyCode = value; }
        }
     
        #endregion
     
        #region Input Checks
     
        public bool IsPressed
        {
            get { return Input.GetKey(keyCode); }
        }
     
        public float Value
        {
            get
            {
                // Check Axis
                float value = 0;
                switch (mouseCode)
                {
                    case MouseCodeEnum.ScrollWheel: value = -Input.GetAxis("Mouse ScrollWheel") * StepSize * 100 * Time.deltaTime; break;
                    case MouseCodeEnum.X: value = Input.GetAxis("Mouse X") * StepSize * 100 * Time.deltaTime; break;
                    case MouseCodeEnum.Y: value = Input.GetAxis("Mouse Y") * StepSize * 100 * Time.deltaTime; break;
                }
     
                // Check Button Buttons
                if( value == 0 && IsPressed)
                    value = StepSize * Time.deltaTime;
     
                return value;
            }
        }
     
        #endregion
     
        #endregion
     
        #region Constructor
     
        public CameraControl()
        {
            stepSize = 1;
        }
     
        public CameraControl(TargetEnum argTarget, KeyCode argKeyCode, float argStepSize)
        {
            target = argTarget;
            keyCode = argKeyCode;
            stepSize = argStepSize;
        }
     
        public CameraControl(TargetEnum argTarget, MouseCodeEnum argMouseCode, float argStepSize)
        {
            target = argTarget;
            mouseCode = argMouseCode;
            stepSize = argStepSize;
        }
     
        public CameraControl(TargetEnum argTarget, KeyCode argKeyCode, MouseCodeEnum argMouseCode, float argStepSize)
        {
            target = argTarget;
            keyCode = argKeyCode;
            mouseCode = argMouseCode;
            stepSize = argStepSize;
        }
     
        #endregion
    }
    
    C# - LimitedFloat.cs [Serializable]
    public class LimitedFloat
    {
        #region Properties
     
        [SerializeField] private float min;
        public float Min
        {
            get { return min; }
            set { min = value; }
        }
     
        [SerializeField] private float max;
        public float Max
        {
            get { return max; }
            set { max = Mathf.Clamp(value, min, float.MaxValue); }
        }
     
        [SerializeField] private float current;
        public float Current
        {
            get { return current; }
            set { current = Mathf.Clamp(value, min, max); ; }
        }
     
        #endregion
     
        #region Constructors
     
        public LimitedFloat(float argStartValue)
        {
            current = argStartValue;
            min = float.MinValue;
            max = float.MaxValue;
        }
     
        public LimitedFloat(float argStartValue, float argMin, float argMax)
        {
            current = argStartValue;
            min = argMin;
            max = argMax;
        }
     
        public LimitedFloat(LimitedFloat argCopy)
        {
            min = argCopy.Min;
            max = argCopy.Max;
            current = argCopy.Current;
        }
     
        #endregion
     
        #region Operator Overloads
     
        public static LimitedFloat operator +(LimitedFloat argLHS, float argRHS)
        {
            LimitedFloat myLimitedFloat = new LimitedFloat(argLHS);
            myLimitedFloat.Current += argRHS;
            return myLimitedFloat;
        }
     
        public static LimitedFloat operator -(LimitedFloat argLHS, float argRHS)
        {
            LimitedFloat myLimitedFloat = new LimitedFloat(argLHS);
            myLimitedFloat.Current -= argRHS;
            return myLimitedFloat;
        }
     
        #endregion
    }C# - DetectionTrigger.cs using UnityEngine;
    using System;
    using System.Collections.Generic;
     
    [AddComponentMenu("Triggers/DetectionTrigger")]
    public class DetectionTrigger : MonoBehaviour
    {
        #region Properties
     
        #region Private Properties
     
        private GameObject ourGameObject;
        private GameObject GameObject
        {
            get
            {
                if (ourGameObject == null)
                    ourGameObject = gameObject;
                return ourGameObject;
            }
        }
     
        protected Collider ourCollider;
        protected Collider Collider
        {
            get
            {
                if (ourCollider == null)
                {
                    ourCollider = GetCollider();
                    ourCollider.isTrigger = true;
                }
                return ourCollider;
            }
        }
     
        #endregion
     
        #region Public Properties
     
        public enum ColliderEnumType { Box, Capsule, Sphere, Wheel, Mesh }
        [SerializeField] private ColliderEnumType colliderType = ColliderEnumType.Sphere;
        public ColliderEnumType ColliderType
        {
            get { return colliderType; }
            set { colliderType = value; }
        }
     
        private Dictionary<int, Transform> ourColliders = new Dictionary<int, Transform>();
        public Dictionary<int, Transform> Colliders
        {
            get { return ourColliders; }
            set { ourColliders = value; }
        }
     
        private List<Transform> ourIgnores = new List<Transform>();
        public List<Transform> Ignores
        {
            get { return ourIgnores; }
            set { ourIgnores = value; }
        }
     
        private List<Type> ourIgnoreTypes = new List<Type>();
        public List<Type> IgnoreTypes
        {
            get { return ourIgnoreTypes; }
            set { ourIgnoreTypes = value; }
        }
     
        #endregion
     
        #endregion
     
        #region Unity Methods
     
        public void Awake()
        {
            if (Collider) ;
        }
     
        void OnTriggerEnter(Collider argCollider)
        {
            Debug.Log(argCollider.transform.GetInstanceID() + " " + argCollider.name);
            ourColliders.Add(argCollider.transform.GetInstanceID(), argCollider.transform);
        }
     
        void OnTriggerExit(Collider argCollider)
        {
            ourColliders.Remove(argCollider.transform.GetInstanceID());
        }
     
        void OnColliderEnter(Collision argCollider)
        {
            Debug.Log(argCollider.transform.GetInstanceID() + " " + argCollider.transform.name);
            ourColliders.Add(argCollider.transform.GetInstanceID(), argCollider.transform);
        }
     
        void OnColliderExit(Collision argCollider)
        {
            ourColliders.Remove(argCollider.transform.GetInstanceID());
        }
     
        #endregion
     
        #region Private Methods
     
        private Collider GetCollider()
        {
            Collider myCollider = null;
            switch (colliderType)
            {
                case ColliderEnumType.Box:
                    myCollider = GetComponent(typeof(BoxCollider)) as BoxCollider;
                    if (myCollider == null)
                        myCollider = GameObject.AddComponent(typeof(BoxCollider)) as BoxCollider;
                    break;
                case ColliderEnumType.Capsule:
                    myCollider = GetComponent(typeof(CapsuleCollider)) as CapsuleCollider;
                    if (myCollider == null)
                        myCollider = GameObject.AddComponent(typeof(CapsuleCollider)) as CapsuleCollider;
                    break;
                case ColliderEnumType.Sphere:
                    myCollider = GetComponent(typeof(SphereCollider)) as SphereCollider;
                    if (myCollider == null)
                        myCollider = GameObject.AddComponent(typeof(SphereCollider)) as SphereCollider;
                    break;
                case ColliderEnumType.Wheel:
                    myCollider = GetComponent(typeof(WheelCollider)) as WheelCollider;
                    if (myCollider == null)
                        myCollider = GameObject.AddComponent(typeof(WheelCollider)) as WheelCollider;
                    break;
                case ColliderEnumType.Mesh:
                    myCollider = GetComponent(typeof(MeshCollider)) as MeshCollider;
                    if (myCollider == null)
                        myCollider = GameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
                    break;
            }
            if (myCollider == null)
                throw new Exception("Trigger Item Has No Collider");
     
            return myCollider;
        }
     
        #endregion
     
     
        public bool IsTripped
        {
            get
            {
                if (ourColliders.Count == 0)
                    return false;
                else
                {
                    bool isTripped = false;
                    foreach (Transform t in ourColliders.Values)
                        if (!Ignores.Contains(t))
                            isTripped = true;
                    return isTripped;
                }
            }
        }
     
    }C# - UnityObject using UnityEngine;
    using System.Collections;
     
    /// <summary>
    /// This Class's Main Purpose is to Store All Unity Properties
    /// 
    /// Normal Unity Properties call the "GetComponent" function each use.
    /// </summary>
    public abstract class UnityObject : MonoBehaviour 
    {
        #region Feilds
     
        // General
        private GameObject ourGameObject = null;
        private Transform ourTransform = null;
     
        // Physics
        private Rigidbody ourRigidbody = null;
        private Collider ourCollider = null;
     
        // Visual
        private Animation ourAnimation = null;
        private Renderer ourRenderer = null;
     
        // Bool
        private bool isMoving = false;
     
        #endregion
     
        #region Properties
     
        #region General Properties
     
        public GameObject GameObject
        {
            get
            {
                /// if not stored, find object
                if (ourGameObject == null)
                    ourGameObject = gameObject;
                return ourGameObject;
            }
        }
     
        public Transform Transform
        {
            get
            {
                /// if not stored, find object
                if (ourTransform == null)
                    ourTransform = transform;
                return ourTransform;
            }
        }
     
        public Vector3 Position
        {
            set { Transform.position = value; }
            get { return Transform.position; }
        }
     
    	public bool IsMoving
        {
            get { return isMoving; }
            set { isMoving = value; }
        }
     
        #endregion
     
        #region Physics Properties
     
        public Rigidbody Rigidbody
        {
            get
            {
                /// if not stored, find object
                if (ourRigidbody == null)
                    ourRigidbody = rigidbody;
                return ourRigidbody;
            }
        }
     
        public Collider Collider
        {
            get
            {
                /// if not stored, find object
                if (ourCollider == null)
                    ourCollider = collider;
                return ourCollider;
            }
        }
     
        #endregion
     
        #region Visual Properties
     
        public Animation Animation
        {
            get
            {
                /// if not stored, find object
                if (ourAnimation == null)
                    ourAnimation = animation;
                return ourAnimation;
            }
        }
     
        public Renderer Renderer
        {
            get
            {
    //            /// if not stored, find object
    //            if (ourRenderer == null)
    //                ourRenderer = renderer;
     
                /// if still not stored, use childs renderer
                if (ourRenderer == null)
                {
                    Transform myRootChild = Transform.FindChild("Root");
                    if(myRootChild != null)
                        ourRenderer = myRootChild.renderer;
                }
                return ourRenderer;
            }
        }
     
        #endregion
     
            #region Methods
     
        private Transform FindChild(Transform currentTransform, string argName)
        {
            //Debug.Log(currentTransform.name + " " + currentTransform.childCount);
            //Debug.Log(currentTransform.name + " " + argName);
            if (currentTransform.name == argName)
                return currentTransform;
            else if (currentTransform.childCount != 0)
                foreach (Transform t in currentTransform)
                {
                    Transform result = FindChild(t, argName);
                    if(result != null)
                        return result;
                }
            return null;
        }
     
    	#endregion
}
}
