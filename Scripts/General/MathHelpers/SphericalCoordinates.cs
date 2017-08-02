/*************************
 * Original url: http://wiki.unity3d.com/index.php/SphericalCoordinates
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/MathHelpers/SphericalCoordinates.cs
 * File based on original modification date of: 9 November 2014, at 21:34. 
 *
 * Author: Bérenger. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.MathHelpers
{
    Description This a class to manipulate an object's position with spherical coordinates instead of cartesian coordinates (Vector3). This is heavily based upon this implementation : http://blog.nobel-joergensen.com/2010/10/22/spherical-coordinates-in-unity/. Here is the description given there : Spherical coordinate system is an alternative coordinate system, where two orthogonale coordinate axis define the world space in 3D. 
    The zenith axis points upwards and the azimuth axis points to the side. To define a point in this system the following is needed: 
    * Radius: the distance from the origin to the point
    * Elevation angle: the angle between the plane (with zenith axis as normal) and the line from the origin to the point
    * Polar angle: the rotation around the zenith axis
    Elevation angle and polar angles are basically the same as latitude and longitude. Note that a point specified in spherical coordinates may not be unique. The spherical coordinate system I’ll be looking at, is the one where the zenith axis equals the Y axis and the azimuth axis equals the X axis. 
    All angles are in radians. 
    Usage Once the instance is created, you can manipulate it through the Rotate functions or the Translate functions. Here is an example for a camera script : 
    using UnityEngine;
     
    public class CamRotate : MonoBehaviour 
    {	
    	public float rotateSpeed = 1f, scrollSpeed = 200f;
    	public Transform pivot;
     
    	public SphericalCoordinates sc;
     
    	private void Start()
    	{
    		sc = new SphericalCoordinates( transform.position, 3f, 10f, 0f, Mathf.PI*2f, 0f, Mathf.PI / 4f );
    		// Initialize position
    		transform.position = sc.toCartesian + pivot.position;
    	}
     
    	void Update () 
    	{
    		float kh, kv, mh, mv, h, v;
    		kh = Input.GetAxis( "Horizontal" );
    		kv = Input.GetAxis( "Vertical" );
     
    		bool anyMouseButton = Input.GetMouseButton(0) | Input.GetMouseButton(1) | Input.GetMouseButton(2);
    		mh = anyMouseButton ? Input.GetAxis( "Mouse X" ) : 0f;
    		mv = anyMouseButton ? Input.GetAxis( "Mouse Y" ) : 0f;
     
    		h = kh*kh > mh*mh ? kh : mh;
    		v = kv*kv > mv*mv ? kv : mv;
     
    		if( h*h > .1f || v*v > .1f )
    			transform.position = sc.Rotate( h * rotateSpeed * Time.deltaTime, v * rotateSpeed * Time.deltaTime ).toCartesian + pivot.position;
     
    		float sw = -Input.GetAxis("Mouse ScrollWheel");
    		if( sw*sw > Mathf.Epsilon )
    			transform.position = sc.TranslateRadius( sw * Time.deltaTime * scrollSpeed ).toCartesian + pivot.position;
     
    		transform.LookAt( pivot.position );
    	}
    }C# - SphericalCoordinates.cs using UnityEngine;
     
    //http://blog.nobel-joergensen.com/2010/10/22/spherical-coordinates-in-unity/
    //http://en.wikipedia.org/wiki/Spherical_coordinate_system
    public class SphericalCoordinates
    {
        public float radius
        { 
            get{ return _radius; }
            private set{ _radius = Mathf.Clamp( value, _minRadius, _maxRadius ); }
        }
        public float polar
        { 
            get{ return _polar; }
            private set
            { 
                _polar = loopPolar ? Mathf.Repeat( value, _maxPolar - _minPolar )
                                   : Mathf.Clamp( value, _minPolar, _maxPolar ); 
            }
        }
        public float elevation
        { 
            get{ return _elevation; }
            private set
            { 
                _elevation = loopElevation ? Mathf.Repeat( value, _maxElevation - _minElevation )
                                           : Mathf.Clamp( value, _minElevation, _maxElevation ); 
            }
        }
     
        // Determine what happen when a limit is reached, repeat or clamp.
        public bool loopPolar = true, loopElevation = false;
     
        private float _radius, _polar, _elevation;
        private float _minRadius, _maxRadius, _minPolar, _maxPolar, _minElevation, _maxElevation;
     
        public SphericalCoordinates(){}
        public SphericalCoordinates( float r, float p, float s,
            float minRadius = 1f, float maxRadius = 20f,
            float minPolar = 0f, float maxPolar = (Mathf.PI*2f),
            float minElevation = 0f, float maxElevation = (Mathf.PI / 3f) )
        {
            _minRadius = minRadius;
            _maxRadius = maxRadius;
            _minPolar = minPolar;
            _maxPolar = maxPolar;
            _minElevation = minElevation;
            _maxElevation = maxElevation;
     
            SetRadius(r);
            SetRotation(p, s);
        }
     
        public SphericalCoordinates(Transform T,
    		float minRadius = 1f, float maxRadius = 20f,
    		float minPolar = 0f, float maxPolar = (Mathf.PI*2f),
    		float minElevation = 0f, float maxElevation = (Mathf.PI / 3f)) :
    		this(T.position, minRadius, maxRadius, minPolar, maxPolar, minElevation, maxElevation) 
    	{ }
     
        public SphericalCoordinates(Vector3 cartesianCoordinate,
    		float minRadius = 1f, float maxRadius = 20f,
    		float minPolar = 0f, float maxPolar = (Mathf.PI*2f),
    		float minElevation = 0f, float maxElevation = (Mathf.PI / 3f))
        {
            _minRadius = minRadius;
            _maxRadius = maxRadius;
            _minPolar = minPolar;
            _maxPolar = maxPolar;
            _minElevation = minElevation;
            _maxElevation = maxElevation;
     
     
            FromCartesian( cartesianCoordinate );
        }
     
        public Vector3 toCartesian
        {
            get
            {
                float a = radius * Mathf.Cos(elevation);
                return new Vector3(a * Mathf.Cos(polar), radius * Mathf.Sin(elevation), a * Mathf.Sin(polar));
            }
        }
     
        public SphericalCoordinates FromCartesian(Vector3 cartesianCoordinate)
        {
            if( cartesianCoordinate.x == 0f )
                cartesianCoordinate.x = Mathf.Epsilon;
            radius = cartesianCoordinate.magnitude;
     
            polar = Mathf.Atan(cartesianCoordinate.z / cartesianCoordinate.x);
     
            if( cartesianCoordinate.x < 0f )
                polar += Mathf.PI;
            elevation = Mathf.Asin(cartesianCoordinate.y / radius);
     
            return this;
        }
     
        public SphericalCoordinates RotatePolarAngle(float x) { return Rotate(x, 0f); }
        public SphericalCoordinates RotateElevationAngle(float x) { return Rotate(0f, x); }
        public SphericalCoordinates Rotate(float newPolar, float newElevation){ return SetRotation( polar + newPolar, elevation + newElevation ); }
        public SphericalCoordinates SetPolarAngle(float x) { return SetRotation(x, elevation); }
        public SphericalCoordinates SetElevationAngle(float x) { return SetRotation(x, elevation); }
        public SphericalCoordinates SetRotation(float newPolar, float newElevation)
        {
            polar = newPolar;		
            elevation = newElevation;
     
            return this;
        }
     
        public SphericalCoordinates TranslateRadius(float x) { return SetRadius(radius + x); }
        public SphericalCoordinates SetRadius(float rad)
        {
            radius = rad;
            return this;
        }
     
        public override string ToString()
        {
            return "[Radius] " + radius + ". [Polar] " + polar + ". [Elevation] " + elevation + ".";
        }
}
}
