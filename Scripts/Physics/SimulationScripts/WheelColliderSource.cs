/*************************
 * Original url: http://wiki.unity3d.com/index.php/WheelColliderSource
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Physics/SimulationScripts/WheelColliderSource.cs
 * File based on original modification date of: 28 October 2010, at 02:02. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Physics.SimulationScripts
{
    Wheel Collider Source is a project intended to provide a free open source alternative to the official Unity Wheel Collider and associated classes. It's intention is to match as closely as possible the interface, implementation and dynamics of the original Unity files to serve as a learning tool and basis for improvement. Note that these files are not intended to improve on the original files, but are intended to recreate them as closely as possible. 
    
    Files include: 
    WheelColliderSource.cs 
    WheelFrictionCurveSource.cs 
    JointSpringSource.cs 
    WheelHitSource.cs 
    Additionally the file CarController.cs has been provided to facilitate initial testing and usage. 
    All the source files can be conveniently downloaded here: Media:WheelColliderSource.zip 
    
    Listed bellow are several known differences in behavior between the original and source files. If you find any additional differences please add them to the list, email me at rotatingcube@gmail.com or submit a fix with a description of what was changed and why. 
    Friction curves need to be compared to make sure the curve functions are providing the same Force values for a given Slip value. 
    Suspension needs to be compared to make sure the behavior is the same. 
    Wheel inertia implementation needs to be checked. 
    Slip calculation needs to be compared and maybe reimplemented (probably using the percentage slip calculation). 
    All the OnCollision and OnTrigger functions need to be implemented. 
    
    Additionally over time I'd like to work on improving these aspects of the project: 
    Improve formatting to improve readability. 
    Better comments explaining in detail the implementation and reasoning behind each major feature. 
    Profile and improve code efficiency (NEVER to the detriment of readability). 
}
