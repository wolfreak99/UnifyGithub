/*************************
 * Original url: http://wiki.unity3d.com/index.php/SunLight
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Physics/SimulationScripts/SunLight.cs
 * File based on original modification date of: 10 January 2012, at 20:47. 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Physics.SimulationScripts
{
    SunLightTODO: - A dynamic sky. - test the sun colors - improve code 
    Version change : i added a algorithm that changes the color of the sunlight. but i have the feeling this is not totaly correct. 
    Codeusing UnityEngine;
    using System;
    using System.Collections;
     
     
    ///////////////////////////////////////
    /// Sunlight script.                ///
    /// Version 0.2 by Martijn Dekker   ///
    /// martijn.pixelstudio@gmail.com   ///
    ///////////////////////////////////////
     
    public class sunlight: MonoBehaviour {
     
        public float DayOfTheYear = 250;
        public float latitude = 0f;
        public float longitude = 0f;
        public float domeRadius = 0f;
        public float updateTime = 5.0f;
        public float timeWarp = 180.0f;
        public float timeOfTheDay = 12.0f;
     
        private float stdMeridian = -45.0f;        
        private Vector3 sunDirection = new Vector3();
        private Vector3 sunPosition = new Vector3();
        private float latitudeInRadian, solarDeclination, sunnyTime, phiSun, thetaSun;
     
    	void Update () {
     
           setLatitude();
           setDay();
           maingui MG = (maingui)GameObject.Find("mainManager").GetComponent(typeof(maingui));
           timeOfTheDay = MG.hourValue;
           DayOfTheYear = MG.dayOftheyear;
           setTimeOfDay();
     
           solarDeclination = calculateSolarDiclination(DayOfTheYear);
           sunnyTime = calculateSunnyTime(latitude, solarDeclination);
           setSunPosition(timeOfTheDay);
           updateSunLightColor(timeOfTheDay);
    	}
     
        void setSunPosition(float time)
        {
            float solarTime, solarAltitude, opp, adj, solarAzimuth, cosSolarDeclination, sinSolarDeclination, cosLatitude, sinLatitude;
     
            sinLatitude = Mathf.Sin(latitudeInRadian);
            cosLatitude = Mathf.Cos(latitudeInRadian);
            sinSolarDeclination = Mathf.Sin(solarDeclination);
            cosSolarDeclination = Mathf.Cos(solarDeclination);
     
            solarTime = time + (0.170f * Mathf.Sin(4f * Mathf.PI * (DayOfTheYear - 80f) / 373f) -
                0.129f * Mathf.Sin(2 * Mathf.PI * (DayOfTheYear - 8f) / 355f)) + (stdMeridian - longitude) / 15;
     
     
            solarAltitude = Mathf.Asin(sinLatitude * sinSolarDeclination - cosLatitude * cosSolarDeclination * Mathf.Cos(Mathf.PI * solarTime / sunnyTime));
     
            opp = -cosSolarDeclination * Mathf.Sin(Mathf.PI * solarTime / sunnyTime);
            adj = (cosLatitude * sinSolarDeclination + sinLatitude * cosSolarDeclination * Mathf.Cos(Mathf.PI * solarTime / sunnyTime));
            solarAzimuth = Mathf.Atan2(opp, adj);
     
            if (solarAltitude > 0.0f)
            {
                if ((opp < 0.0f && solarAzimuth < 0.0f) || (opp > 0.0f && solarAzimuth > 0.0f))
                {
                    solarAzimuth = (0.5f * Mathf.PI ) + solarAzimuth;
                }
                else
                {
                    solarAzimuth = (0.5f * Mathf.PI) - solarAzimuth;
                }
                phiSun = (Mathf.PI * 2.0f) - solarAzimuth;
                this.thetaSun = (0.5f * Mathf.PI ) - solarAltitude;
     
                sunDirection.x = domeRadius;
                sunDirection.y = phiSun;
                sunDirection.z = solarAltitude;
     
                Vector3 sunDirection2 = new Vector3();
                sunDirection2 = calcDirection();
     
                sunPosition = sphericalToCartesian(sunDirection);
     
                transform.localPosition = sunPosition; //position
                transform.LookAt(sunDirection2); // rotation
     
            }
     
     
        }
     
        Vector3 calcDirection()
        {
            Vector3 dir = new Vector3();
            dir.x = Mathf.Cos(0.5f * Mathf.PI - thetaSun) * Mathf.Cos(phiSun);
            dir.y = Mathf.Sin(0.5f * Mathf.PI - thetaSun);
            dir.z = Mathf.Cos(0.5f * Mathf.PI - thetaSun) * Mathf.Sin(phiSun);
            return dir.normalized;
     
        }
        Vector3 sphericalToCartesian(Vector3 sunDir)
        {
            Vector3 res = new Vector3();
            res.y = sunDirection.x * Mathf.Sin(sunDir.z);
            float tmp = sunDirection.x * Mathf.Cos(sunDir.z);
            res.x = tmp * Mathf.Cos(sunDir.y);
            res.z = tmp * Mathf.Sin(sunDir.y);
     
            return res;
        }
     
        void setLatitude()
        {
            latitude = Mathf.Clamp(latitude, -90.0f, 90.0f);
            latitudeInRadian = Mathf.Deg2Rad * latitude;
        }
        void setDay()
        {
            this.DayOfTheYear = Mathf.Clamp(DayOfTheYear, 0.0f, 365f);
        }
        void setTimeOfDay()
        {
            this.timeOfTheDay = Mathf.Clamp(timeOfTheDay, 8f, 23f);
        }
     
        float calculateSolarDiclination(float jDay)
        {
            return (0.4093f * Mathf.Sin(2 * Mathf.PI * (284f + jDay) / 365f));
        }
        float calculateSunnyTime(float lat, float solarDeclin)
        {
            float st = (2.0f * Mathf.Acos(-Mathf.Tan(lat) * Mathf.Tan(solarDeclin)));
            return (st*Mathf.Rad2Deg)/15;
     
        }
     
        private void updateSunLightColor(float currentTime)
        {
     
            float[] dColor = new float[4];
            dColor[0] = 0.9843f;
            dColor[1] = 0.5098f;
            dColor[2] = 0;
            dColor[3] = 1;
            // Create an array defines the new sunlight color.
            float[] newColor = new float[4];
            // If the current time is between 6:00 and 12:00, change the color towards white.
            if (currentTime >= 6 && currentTime < 12)
            {
                for (int i = 0; i < newColor.Length; i++)
                {
                    newColor[i] = dColor[i] + ((1 - dColor[i]) / 6) * (currentTime - 6);
                }
            }
            // Else if the current time is between 12:00 and 18:00, change the color towards dColor.
            else if (currentTime >= 12 && currentTime < 18)
            {
                for (int i = 0; i < newColor.Length; i++)
                {
                    newColor[i] = 1 - ((1 - dColor[i]) / 6) * (currentTime - 12);
                }
            }
            // Else if the current time is between 18:00 and 24:00, change the color towards black.
            else if (currentTime >= 18 && currentTime < 24)
            {
                for (int i = 0; i < newColor.Length; i++)
                {
                    newColor[i] = dColor[i] - (dColor[i] / 6) * (currentTime - 18);
                }
            }
            // Else if the current time is between 0:00 and 6:00, change the color towards dColor.
            else if (currentTime >= 0 && currentTime < 6)
            {
                for (int i = 0; i < newColor.Length; i++)
                {
                    newColor[i] = (dColor[i] / 6) * (currentTime - 0);
                }
            }
            // RenderSettings.fogColor = new Color(newColor[0], newColor[1], newColor[2], newColor[3]);
            light.color = new Color(newColor[0], newColor[1], newColor[2], newColor[3]);
        }
     
    }
}
