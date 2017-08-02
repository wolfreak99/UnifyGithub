/*************************
 * Original url: http://wiki.unity3d.com/index.php/HSBColor
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/General/TextureColor/HSBColor.cs
 * File based on original modification date of: 3 September 2015, at 20:56. 
 *
 * Description 
 *   
 * Usage 
 *   
 * C# - HSBColor.cs 
 *   
 * JS - HSV2RGB2HSV.js 
 *   
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.General.TextureColor
{
    
    CS Version A: Jonathan Czeck (aarku) 
    JS Version B: Antony Stewart (ohh bertie) 
    
    
    DescriptionVersion A provides a Hue/Saturation/Brightness/Alpha color model in addition to Unity's built in Red/Green/Blue/Alpha colors. It is useful for interpolating between colors in a more pleasing fashion. 
    Version B in .JS, Is the same probably, one difference is it has backwards and forwards conversion to and from HSV. i haven't compared them, the maths is different. I have tested it though and it converts fine, the algorithm is better in 32 bit depth CPU than in GPU by a visible amount. doesnt have a lerp fct. 
    UsagePut this script in a file in /Plugins/HSBColor.cs so that it may be callable by JavaScript. Additionally, there is a helper class called Colorx that is useful to put in your Plugins folder. This provides a quick helper function to slerp between two Colors, which gives much more visually pleasing results than Color.Lerp. See Colorx for a simple importable Unity Package. 
    C# - HSBColor.csusing UnityEngine;
     
    [System.Serializable]
    public struct HSBColor
    {
        public float h;
        public float s;
        public float b;
        public float a;
     
        public HSBColor(float h, float s, float b, float a)
        {
            this.h = h;
            this.s = s;
            this.b = b;
            this.a = a;
        }
     
        public HSBColor(float h, float s, float b)
        {
            this.h = h;
            this.s = s;
            this.b = b;
            this.a = 1f;
        }
     
        public HSBColor(Color col)
        {
            HSBColor temp = FromColor(col);
            h = temp.h;
            s = temp.s;
            b = temp.b;
            a = temp.a;
        }
     
        public static HSBColor FromColor(Color color)
        {
            HSBColor ret = new HSBColor(0f, 0f, 0f, color.a);
     
            float r = color.r;
            float g = color.g;
            float b = color.b;
     
            float max = Mathf.Max(r, Mathf.Max(g, b));
     
            if (max <= 0)
            {
                return ret;
            }
     
            float min = Mathf.Min(r, Mathf.Min(g, b));
            float dif = max - min;
     
            if (max > min)
            {
                if (g == max)
                {
                    ret.h = (b - r) / dif * 60f + 120f;
                }
                else if (b == max)
                {
                    ret.h = (r - g) / dif * 60f + 240f;
                }
                else if (b > g)
                {
                    ret.h = (g - b) / dif * 60f + 360f;
                }
                else
                {
                    ret.h = (g - b) / dif * 60f;
                }
                if (ret.h < 0)
                {
                    ret.h = ret.h + 360f;
                }
            }
            else
            {
                ret.h = 0;
            }
     
            ret.h *= 1f / 360f;
            ret.s = (dif / max) * 1f;
            ret.b = max;
     
            return ret;
        }
     
        public static Color ToColor(HSBColor hsbColor)
        {
            float r = hsbColor.b;
            float g = hsbColor.b;
            float b = hsbColor.b;
            if (hsbColor.s != 0)
            {
                float max = hsbColor.b;
                float dif = hsbColor.b * hsbColor.s;
                float min = hsbColor.b - dif;
     
                float h = hsbColor.h * 360f;
     
                if (h < 60f)
                {
                    r = max;
                    g = h * dif / 60f + min;
                    b = min;
                }
                else if (h < 120f)
                {
                    r = -(h - 120f) * dif / 60f + min;
                    g = max;
                    b = min;
                }
                else if (h < 180f)
                {
                    r = min;
                    g = max;
                    b = (h - 120f) * dif / 60f + min;
                }
                else if (h < 240f)
                {
                    r = min;
                    g = -(h - 240f) * dif / 60f + min;
                    b = max;
                }
                else if (h < 300f)
                {
                    r = (h - 240f) * dif / 60f + min;
                    g = min;
                    b = max;
                }
                else if (h <= 360f)
                {
                    r = max;
                    g = min;
                    b = -(h - 360f) * dif / 60 + min;
                }
                else
                {
                    r = 0;
                    g = 0;
                    b = 0;
                }
            }
     
            return new Color(Mathf.Clamp01(r),Mathf.Clamp01(g),Mathf.Clamp01(b),hsbColor.a);
        }
     
        public Color ToColor()
        {
            return ToColor(this);
        }
     
        public override string ToString()
        {
            return "H:" + h + " S:" + s + " B:" + b;
        }
     
        public static HSBColor Lerp(HSBColor a, HSBColor b, float t)
        {
            float h,s;
     
            //check special case black (color.b==0): interpolate neither hue nor saturation!
            //check special case grey (color.s==0): don't interpolate hue!
            if(a.b==0){
                h=b.h;
                s=b.s;
            }else if(b.b==0){
                h=a.h;
                s=a.s;
            }else{
                if(a.s==0){
                    h=b.h;
                }else if(b.s==0){
                    h=a.h;
                }else{
                    // works around bug with LerpAngle
                    float angle = Mathf.LerpAngle(a.h * 360f, b.h * 360f, t);
                    while (angle < 0f)
                        angle += 360f;
                    while (angle > 360f)
                        angle -= 360f;
                    h=angle/360f;
                }
                s=Mathf.Lerp(a.s,b.s,t);
            }
            return new HSBColor(h, s, Mathf.Lerp(a.b, b.b, t), Mathf.Lerp(a.a, b.a, t));
        }
     
        public static void Test()
        {
            HSBColor color;
     
            color = new HSBColor(Color.red);
            Debug.Log("red: " + color);
     
            color = new HSBColor(Color.green);
            Debug.Log("green: " + color);
     
            color = new HSBColor(Color.blue);
            Debug.Log("blue: " + color);
     
            color = new HSBColor(Color.grey);
            Debug.Log("grey: " + color);
     
            color = new HSBColor(Color.white);
            Debug.Log("white: " + color);
     
            color = new HSBColor(new Color(0.4f, 1f, 0.84f, 1f));
            Debug.Log("0.4, 1f, 0.84: " + color);
     
            Debug.Log("164,82,84   .... 0.643137f, 0.321568f, 0.329411f  :" + ToColor(new HSBColor(new Color(0.643137f, 0.321568f, 0.329411f))));
        }
    }
    
    JS - HSV2RGB2HSV.jsusing UnityEngine;
     
    VERSION B in JS ... two HSV 2 RGB functions, to and from, dont put in a plugins folder, make it a static function some place if you need to access it in all scripts: 
     
     
    //CHANGE COLORS FROM RGB TO HSV:				
    	function Hue( H: float ): Vector3
    	{
    		var R : float= Mathf.Abs(H * 6 - 3) - 1;
    		var G : float= 2 - Mathf.Abs(H * 6 - 2);
    		var B : float= 2 - Mathf.Abs(H * 6 - 4);
    		return Vector3( Mathf.Clamp01(R),Mathf.Clamp01(G),Mathf.Clamp01(B) );
    	}
     
    	function HSVtoRGB( HSV: Vector3): Vector4
    	{
     
    		var H = Hue(HSV.x) ;
    		 H= Vector3 (H.x-1, H.y-1, H.z-1)* HSV.y ;
    		 H= Vector3 (H.x + 1, H.y + 1, H.z + 1)* HSV.z;
    		 return Vector4(H.x, H.y, H.z,1);
    	}
     
    	function RGBtoHSV(RGB: Vector3): Vector4
    	{
    		var HSV : Vector3 = Vector3.zero;
    		HSV.z = Mathf.Max(RGB.x, Mathf.Max(RGB.y, RGB.z));
    		var M :float = Mathf.Min(RGB.x, Mathf.Min(RGB.y, RGB.z));
    		var C :float = HSV.z - M;
    		if (C != 0)
    		{
    			HSV.y = C / HSV.z;
    			var Delta : Vector3 = Vector3( 
    			(HSV.z - RGB.x) / C ,  
    			(HSV.z - RGB.y) / C , 
    			(HSV.z - RGB.z) / C );
    			var del = Delta;
    			Delta.x -= del.z;
    			Delta.y -= del.x;
    			Delta.z -= del.y;
    			Delta.x += 2.0;
    			Delta.y += 4.0;
    			if (RGB.x >= HSV.z)
    				{HSV.x = Delta.z;}
    			else if (RGB.y >= HSV.z)
    				{HSV.x = Delta.x;}
    			else
    				{HSV.x = Delta.y;}
    			HSV.x = (HSV.x / 6.0)%1.0;
    		}
    		return Vector4(HSV.x,HSV.y,HSV.z,1);
    	}
}
