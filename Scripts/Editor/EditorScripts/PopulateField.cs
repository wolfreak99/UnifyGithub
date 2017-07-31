// Original url: http://wiki.unity3d.com/index.php/PopulateField
// Github url: https://github.com/wolfreak99/UnifyGithub/blob/Master/Scripts/Editor/EditorScripts/PopulateField.cs
// File based on original modification date of: 1 February 2014, at 16:23. 
//
// This file has not yet been properly formatted, feel free to contribute!
//

namespace UnifyGithub.Editor.EditorScripts
{
Author: Erdin Kacan (Nidre) 
Description Fill an area by Instantiating selected GameObject with specific parameters. 
Beaware : I have created this code piece for my Project and i am not planning to Support it.(I might update it as i develop it further if my project requires so.) 
Beaware 2 : I haven't really optimized it so this is not the most optimized way of doing that(But worked well for my needs).It might contain bugs but nothing major. 
Usage You must place the script in a folder named Editor in your project's Assets folder for it to work properly. 
Navigate to Custom/Nidre/PopulateField from the Unity Menu Bar. 
In the opened window; 
Select the GameObject you want to populate the field with. Under Transform section select the appropiate options for rotation and scale. Under Spawn section select the appropiate parameters of field type. Under Advanced section you can disable/enable/change limitations to speed up the operation. 
C# - PopulateField.cs //Author Erdin Kacan (Nidre)
//Webpage : http://erdinkacan.tumblr.com/
 
//PopulateField by Erdin Kacan (Nidre) is licensed under the Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
//To view a copy of this license, visit http://creativecommons.org/licenses/by-nc-sa/3.0/.
 
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
 
public class PopulateField : EditorWindow
{
    static List<Vector3> _occupiedPositions = new List<Vector3>();
    static int _currentRetry;
    static float _maximumRetry;
    static Vector3 _intendedPosition;
    static GameObject _prefab;
    static GameObject _tempPrefab;
    static float _radius = 2;
    static float _minScale;
    static float _maxScale;
    static float _scale;
    static Density _density = Density.Medium;
    static Vector3 _cubeArea;
    static Shape _shape = Shape.Sphere;
    static float _castRadius = 1;
    static GameObject _container;
    static bool _limitIterations = true;
    static int _iterationLimit = 500000;
    static bool _limitObjectCount = true;
    static int _objectLimit = 1000;
    static bool _is3D;
    static bool _randomizeRotations;
    static bool _randomizeRotationX;
    static bool _randomizeRotationY;
    static bool _randomizeRotationZ;
    static bool _randomizeScale;
    static bool _randomizeScaleX;
    static bool _randomizeScaleY;
    static bool _randomizeScaleZ;
    static bool _allowOverdraw;
    static int _currentObjectCount;
 
    static System.DateTime _lastCheck;
    static float _retryPerSecond;
    static int _previousRetryCount;
 
    static float _updateEvery = 1;
 
 
    #region Visual
    static bool _transformToggle;
    static bool _spawnAreaToggle;
    static bool _advancedToggle;
    static Vector2 _scrollPos;
    #endregion
 
    enum Shape
    {
        Cube,
        Sphere
    }
 
    enum Density
    {
        Low = 4,
        Medium = 3,
        High = 2,
        Extreme = 1,
    }
 
    [MenuItem("Custom/Nidre/Misc/Populate Field")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PopulateField));
        //Defaults();
    }
 
    private static void Defaults()
    {
        _prefab = Resources.Load("Prefabs/Asteroids/Asteroid.Rock", typeof(GameObject)) as GameObject;
        _minScale = 0.1f;
        _maxScale = 1f;
        _radius = 1;
        _density = Density.Medium;
        _is3D = false;
        _limitObjectCount = true;
        _objectLimit = 100;
    }
 
    public void OnGUI()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Restore Defaults"))
        {
            Defaults();
        }
        _prefab = EditorGUILayout.ObjectField(_prefab, typeof(GameObject), false, null) as GameObject;
        _transformToggle = EditorGUILayout.Foldout(_transformToggle, "Transform");
        if (_transformToggle)
        {
            _randomizeScale = EditorGUILayout.Toggle("Randomize Scale", _randomizeScale);
            if (_randomizeScale)
            {
                EditorGUILayout.BeginHorizontal();
                _randomizeScaleX = EditorGUILayout.Toggle("Randomize X", _randomizeScaleX);
                _randomizeScaleY = EditorGUILayout.Toggle("Randomize Y", _randomizeScaleY);
                _randomizeScaleZ = EditorGUILayout.Toggle("Randomize Z", _randomizeScaleZ);
                EditorGUILayout.EndHorizontal();
 
                EditorGUILayout.MinMaxSlider(new GUIContent("Scale"), ref _minScale, ref _maxScale, 0.001f, 100f, null);
                EditorGUILayout.BeginHorizontal();
                _minScale = EditorGUILayout.FloatField("Min", Mathf.Clamp(_minScale, 0.1f, _maxScale - 0.1f));
                _maxScale = EditorGUILayout.FloatField("Max", Mathf.Clamp(_maxScale, _minScale + 0.1f, 100f));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                _maxScale = _minScale = EditorGUILayout.FloatField("Scale", Mathf.Clamp(_minScale, 0.1f, 100f));
            }
 
            _randomizeRotations = EditorGUILayout.Toggle("Randomize Rotations", _randomizeRotations);
            if (_randomizeRotations)
            {
                EditorGUILayout.BeginHorizontal();
                _randomizeRotationX = EditorGUILayout.Toggle("Randomize X", _randomizeRotationX);
                _randomizeRotationY = EditorGUILayout.Toggle("Randomize Y", _randomizeRotationY);
                _randomizeRotationZ = EditorGUILayout.Toggle("Randomize Z", _randomizeRotationZ);
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.Separator();
        _spawnAreaToggle = EditorGUILayout.Foldout(_spawnAreaToggle, "Spawn");
        if (_spawnAreaToggle)
        {
            _density = (Density)EditorGUILayout.EnumPopup("Density", _density);
            _shape = (Shape)EditorGUILayout.EnumPopup("Shape", _shape);
            switch (_density)
            {
                case Density.Low:
                    _maximumRetry = 100;
                    break;
                case Density.Medium:
                    _maximumRetry = 1000;
                    break;
                case Density.High:
                    _maximumRetry = 10000;
                    break;
                case Density.Extreme:
                    _maximumRetry = 100000;
                    break;
            }
            _castRadius = (int)_density;
            switch (_shape)
            {
                case Shape.Cube:
                    _cubeArea = EditorGUILayout.Vector3Field("Area", _cubeArea);
                    _maximumRetry *= (_cubeArea.x + _cubeArea.y + _cubeArea.z) / 3;
                    break;
                case Shape.Sphere:
                    _radius = EditorGUILayout.FloatField("Radius", _radius);
                    _is3D = EditorGUILayout.Toggle("Is 3D Field(Extremely Slow)", _is3D);
                    _maximumRetry *= _radius;
                    break;
            }
        }
        EditorGUILayout.Separator();
        _advancedToggle = EditorGUILayout.Foldout(_advancedToggle, "Advanced");
        if (_advancedToggle)
        {
            _allowOverdraw = EditorGUILayout.Toggle("Allow Overdraw", _allowOverdraw);
            _limitObjectCount = EditorGUILayout.Toggle("Limit Object Count", _limitObjectCount);
            if (_limitObjectCount)
            {
                _objectLimit = EditorGUILayout.IntField("Object Limit", _objectLimit);
            }
            _limitIterations = EditorGUILayout.Toggle("Limit Iterations", _limitIterations);
            if (_limitIterations)
            {
                _iterationLimit = EditorGUILayout.IntField("Iteration Limit", _iterationLimit);
                _maximumRetry = _iterationLimit;
            }
            _updateEvery = EditorGUILayout.FloatField(string.Format("Update every {0} seconds.", _updateEvery), Mathf.Clamp(_updateEvery, 0.1f, 60f));
        }
        if (_maximumRetry > 25000 || _is3D)
        {
            EditorGUILayout.HelpBox(string.Format("OPERATION WILL BE VERY SLOW.\nIterations : {0}\nObject Limit : {1}\nConsider Lowering Density,Disabling 3D or Limiting Object Count.", _maximumRetry, (_limitObjectCount ? _objectLimit.ToString() : "Unlimited")), MessageType.Warning);
            //EditorGUILayout.LabelField("OPERATION WILL BE VERY SLOW.");
            //EditorGUILayout.LabelField("Iterations : " + _maximumRetry.ToString());
            //EditorGUILayout.LabelField("Object Limit : " + (_limitObjectCount ? _objectLimit.ToString() : "Unlimited"));
            //EditorGUILayout.LabelField("Consider Lowering Density,Disabling 3D or Limiting Object Count.");
        }
        EditorGUILayout.EndVertical();
 
        if (GUILayout.Button("Create Field"))
        {
            _occupiedPositions.Clear();
            _container = new GameObject("Container");
            _currentObjectCount = 0;
            _currentRetry = 0;
 
            Spawn();
        }
        EditorGUILayout.EndScrollView();
    }
 
    static void Spawn()
    {
        while (_currentRetry < _maximumRetry)
        {
            _currentRetry++;
            switch (_shape)
            {
                case Shape.Cube:
                    _intendedPosition = new Vector3(Random.Range(-_cubeArea.x / 2, _cubeArea.x / 2), Random.Range(-_cubeArea.y / 2, _cubeArea.y / 2), Random.Range(-_cubeArea.z / 2, _cubeArea.z / 2));
                    break;
                case Shape.Sphere:
                    _intendedPosition = Random.insideUnitSphere * _radius;
                    if (!_is3D) _intendedPosition = new Vector3(_intendedPosition.x, 0, _intendedPosition.z);
                    break;
            }
            if (IsPoisitionAvailable(_intendedPosition))
            {
                SpawnAsteroid();
                _currentObjectCount++;
                if (_limitObjectCount)
                {
                    if (_currentObjectCount == _objectLimit)
                    {
                        Debug.Log("Maximum Object Limit Reached");
                        break;
                    }
                }
            }
            if (System.DateTime.Now.Subtract(_lastCheck).TotalSeconds > _updateEvery)
            {
                string message;
                if (_previousRetryCount == 0)
                {
                    message = string.Format("Iterations : {0} of {1}.", _currentRetry, _maximumRetry);
                }
                else
                {
                    _retryPerSecond = (_currentRetry - _previousRetryCount) / (float)System.DateTime.Now.Subtract(_lastCheck).TotalSeconds;
                    message = string.Format("Iterations : {0} of {1}.{2} left.", _currentRetry, _maximumRetry, new System.TimeSpan(0, 0, (int)(_maximumRetry / _retryPerSecond)));
                }
                _previousRetryCount = _currentRetry;
                _lastCheck = System.DateTime.Now;
                string title = string.Format("Spawned : {0}", _currentObjectCount);
                if (EditorUtility.DisplayCancelableProgressBar(title, message, _currentRetry / _maximumRetry))
                {
                    EditorUtility.ClearProgressBar();
                    Debug.Log("Cancelled.");
                    return;
                }
            }
        }
        EditorUtility.ClearProgressBar();
        Debug.Log("Maximum Retry Limit Reached");
    }
 
    static bool IsPoisitionAvailable(Vector3 pos)
    {
        if (_occupiedPositions.Contains(pos)) { return false; }
        if (!_allowOverdraw)
        {
            switch (_shape)
            {
                case Shape.Cube:
                    if (Physics.CapsuleCast(new Vector3(pos.x, -_cubeArea.y / 2, pos.z), new Vector3(pos.x, _cubeArea.y / 2, pos.z), _castRadius * _scale, Vector3.down)) { return false; }
                    break;
                case Shape.Sphere:
                    if (Physics.CapsuleCast(new Vector3(pos.x, -_radius / 2, pos.z), new Vector3(pos.x, _radius / 2, pos.z), _castRadius * _scale, Vector3.down)) { return false; }
                    break;
            }
        }
        else if (Physics.OverlapSphere(pos, _castRadius * _scale).Length != 0) { return false; }
        return true;
    }
 
    static void SpawnAsteroid()
    {
        GameObject temp = Instantiate(_prefab) as GameObject;
        Transform tempTransform = temp.transform;
        temp.name = string.Format("{0} {1}", _prefab.name, _occupiedPositions.Count);
        tempTransform.parent = _container.transform;
        tempTransform.position = _intendedPosition;
 
        if (_randomizeRotations)
        {
            tempTransform.rotation = Quaternion.Euler(_randomizeRotationX ? Random.Range(-180, 180) : 0, _randomizeRotationY ? Random.Range(-180, 180) : 0, _randomizeRotationZ ? Random.Range(-180, 180) : 0);
        }
        else tempTransform.rotation = Quaternion.identity;
        if (_randomizeScale)
        {
            float _scale = Random.Range(_minScale, _maxScale);
            tempTransform.localScale = new Vector3(_randomizeScaleX ? _scale : _minScale, _randomizeScaleY ? _scale : _minScale, _randomizeScaleZ ? _scale : _minScale);
        }
        else tempTransform.localScale = new Vector3(_minScale, _minScale, _minScale);
 
        _occupiedPositions.Add(tempTransform.position);
    }
 
}
}
