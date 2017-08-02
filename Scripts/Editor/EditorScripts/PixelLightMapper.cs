/*************************
 * Original url: http://wiki.unity3d.com/index.php/PixelLightMapper
 * Github url: https://github.com/wolfreak99/UnifyGithub/blob/master/Scripts/Editor/EditorScripts/PixelLightMapper.cs
 * File based on original modification date of: 10 January 2012, at 20:45. 
 *
 * Author: Alex Vendelbo Ringgaard (Talzor) 
 *
 * This file has not yet been properly formatted, feel free to contribute!
 *
 *************************/

namespace UnifyGithub.Editor.EditorScripts
{
    Thanks to: Yoggy 
    Description A pixel based light mapper for Unity. I.e. instead of casting rays from lightsources into the world, it will cast rays from all texels in each light map to the light source. 
    Usage Place the script in the Editor folder and run the wizard from GameObject/Lightmap Wizard (Pixel). 
    The light mapper supports both point, spot and directional lights and while it doesn't match them completely it's usually a fairly good approximation. 
    The objects 2nd UV set should be uniquely laid out (or the 1st UV set if the 2nd isn't used) (Note that most of Unity's primitives aren't uniquely UV mapped) 
    Options: 
    lightmapMode (default ALL): Determines what objects are lightmapped. 
    ALL: All objects in the scene with a lightmap shader is lightmapped 
    SELECTION: Only directly selected objects are lightmapped 
    SELECTION_OR_CHILD: Only selected objects and thier children are lightmapped 
    Smoothing (default 1): Determines how much the final lightmaps should be smoothed. When smoothing a texel tcenter the lightmapper looks at all texels that are at within smoothing distance from tcenter (including tcenter) and averages the color (texels that have recieved no light is ignored). 
    Save Light Maps (default false): Should the lightmaps be saved after being generated. If false all lightmaps will revert on reimport which is good if your testing out difference light setups and doesn't want to loose the old lightmaps just yet. 
    Alpha Lookup (default false): If true the light mapper whill look at the alpha of all the surfaces from a texel and to the light source and modify the light intensity accordingly. (E.g. if the light passes through a surface with an alpha of 0.4 only 60% of the light will get through to the surface behind). Also surfaces with an alpha value will only receive the light that doesn't pass through. (E.g. a surface with an alpha of 0.25 will only recieve 25% of the light hitting it); 
    No Directional Shadow (default false): If this is true directional light will always be applied regardless of colliders. Use this for indoor scene where the "roof" whould block all directional light. 
    Layer Mask (default everything): The layers that light rays should be blocked by/test alpha against. 
    Ambient Color (defaut black): A color that is added to all texels. 
    C# - PixelLightMapper.cs using UnityEngine;
    using UnityEditor;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
     
    ///Lightmapper based on texel to light raycast @see http://www.unifycommunity.com/wiki/index.php?title=PixelLightMapper#Usage for more info
    public class PixelLightMapper : ScriptableWizard {	
    	public enum LightmapMode {
    		ALL,				//Lightmap all objects in the scene with a lightmap
    		SELECTION,			//Lightmap all objects in the scene with a lightmap which is selected
    		SELECTION_OR_CHILD,	//Lightmap all objects in the scene with a lightmap which is selected or a child of a selected transform
    	}
     
    	public LightmapMode lightmapMode = LightmapMode.ALL; ///<What objects should be lightmapped
    	public int smoothing = 1;				///<How much should the final lightmaps be smoothed
    	public bool saveLightMaps = false;		///<Should the lightmaps be saved after being generated. If false all lightmaps will revert on reimport
    	public bool alphaLookup = false;		///<Should the lightmapper try to look at the alpha of surfaces
    	public bool noDirectionalShadow = false;///<Always apply directional light regardsless of colliders (Good for indoor rooms)
    	public LayerMask layerMask = -1;		///<What layers to raycast the light agains
    	public Color ambientColor = Color.black;///<Color added to all texels
     
    	[MenuItem("GameObject/Lightmap Wizard (Pixel)")]
    	public static void CreateWizard() {
    		DisplayWizard("Generate Lightmaps (Pixel based)", typeof(PixelLightMapper), "Run", "Reset");	
    	}
     
    	private const int falloffStart = 60; ///<Inside this angle (-ray.direction/hit.normal) full light will be used
    	private const int falloffEnd = 90; ///<At this angle (-ray.direction/hit.normal) the surface recieves no light (Between it will use lerp)
    	private const float spotFalloff = 2.5f; ///<The outer range where spot light begins to falloff
     
    	void OnWizardCreate() {
    		//Find all lights
    		Light[] lights = Object.FindObjectsOfType(typeof (Light)) as Light[];
     
    		//Find all renderers with a lightmap that should be lightmapped and all lightmaps that will be used (This is primarily done to show the progress bar)
    		List<Renderer> models = new List<Renderer>(); //List of all renderers with a lightmap
    		List<Texture2D> lightmaps = new List<Texture2D>(); //List of all lightmaps that will be used
    		foreach (Renderer renderer in Object.FindObjectsOfType(typeof (Renderer))) {
    			if (renderer.sharedMaterial.HasProperty("_LightMap")) {
    				if ((lightmapMode == LightmapMode.ALL) ||
    					(lightmapMode == LightmapMode.SELECTION && Selected(renderer.transform)) ||
    					(lightmapMode == LightmapMode.SELECTION_OR_CHILD && SelectedOrChild(renderer.transform))
    				) {
    					Texture2D lightmap = renderer.sharedMaterial.GetTexture("_LightMap") as Texture2D;
     
    					if (lightmap) {
    						models.Add(renderer);
     
    						if (!lightmaps.Contains(lightmap)) {
    							lightmaps.Add(lightmap);
    						}
    					}
    				}
    			}
    		}
     
    		//This is just for the progressbar
    		int totalSteps = lightmaps.Count + models.Count + lightmaps.Count; //Clear + Lightmap + Smoothing
    		int currentStep = 0;
     
    		//Clear all lightmaps
    		 for (int lightmapIndex = 0; lightmapIndex < lightmaps.Count; lightmapIndex++) { 
    			Texture2D lightmap = lightmaps[lightmapIndex]; 
     
    			EditorUtility.DisplayProgressBar( 
    			   "Lightmapping", 
    			   string.Format("Clearing lightmaps: {0} ({1} of {2})", lightmap.name, lightmapIndex+1, lightmaps.Count), 
    			   currentStep++ / (float) totalSteps 
    			); 
     
    			// force the lightmap to be readable so that we can access its pixels 
    			string lightmapPath = AssetDatabase.GetAssetPath (lightmap); 
    			TextureImporter texImport = (TextureImporter) AssetImporter.GetAtPath (lightmapPath); 
    			if (texImport != null  &&  !texImport.isReadable) 
    			{ 
    			   texImport.isReadable = true; 
    			   AssetDatabase.ImportAsset (lightmapPath, ImportAssetOptions.ForceUpdate); 
    			} 
     
    			lightmap.SetPixels (new Color [lightmap.width*lightmap.height]); //Set the lightmap to all black 
    		 } 
     
    		for (int rendererIndex = 0; rendererIndex < models.Count; rendererIndex++) {
    			Renderer renderer = models[rendererIndex];
     
    			EditorUtility.DisplayProgressBar(
    				"Lightmapping", 
    				string.Format("Calculating Light: {0} ({1} of {2})", renderer.name, rendererIndex+1, models.Count), 
    				currentStep++ / (float) totalSteps
    			);
     
    			Mesh mesh = ((MeshFilter) renderer.GetComponent(typeof (MeshFilter))).sharedMesh;
    			Texture2D lightmap = renderer.sharedMaterial.GetTexture("_LightMap") as Texture2D;
    			Texture2D mainTex = renderer.sharedMaterial.mainTexture as Texture2D; //For alpha lookup
     
    			if (lightmap.format == TextureFormat.Alpha8 || lightmap.format == TextureFormat.ARGB32 || lightmap.format == TextureFormat.RGB24) {
    				int[] triangles = mesh.triangles;
    				Vector3[] vertices = mesh.vertices;
    				Vector3[] normals = mesh.normals;
    				Vector2[] mainUV = mesh.uv; //Used for alphaLookup
    				Vector2[] lightUV = mesh.uv2.Length > 0 ? mesh.uv2 : mesh.uv;
     
    				//Transform vertices and normals to global coords
    				Transform t = renderer.transform;
    				for (int vert = 0; vert < vertices.Length; vert++) {
    					vertices[vert] = t.TransformPoint(vertices[vert]);	
    				}
    				for (int normal = 0; normal < normals.Length; normal++) {
    					normals[normal] = t.TransformDirection(normals[normal]);	
    				}
     
    				for (int i = 0; i < triangles.Length; i += 3) {
    					//Vertice indexs
    					int vi0 = triangles[i];
    					int vi1 = triangles[i+1];
    					int vi2 = triangles[i+2];
     
    					//Lightmap UV coords [pixel]
    					Vector2 pUV0 = new Vector2(lightUV[vi0].x * lightmap.width, lightUV[vi0].y * lightmap.height);
    					Vector2 pUV1 = new Vector2(lightUV[vi1].x * lightmap.width, lightUV[vi1].y * lightmap.height);
    					Vector2 pUV2 = new Vector2(lightUV[vi2].x * lightmap.width, lightUV[vi2].y * lightmap.height);
     
    					Triangle2D pUVTriangle = new Triangle2D(pUV0, pUV1, pUV2);
     
    					//mainTex UV coords [pixel]. These are only used (and computed) if alphaLookup is true (AND there is a mainTex)
    					Vector2 pMainUV0 = alphaLookup && mainTex ? new Vector2(mainUV[vi0].x * mainTex.width, mainUV[vi0].y * mainTex.height) : Vector2.zero;
    					Vector2 pMainUV1 = alphaLookup && mainTex ? new Vector2(mainUV[vi1].x * mainTex.width, mainUV[vi1].y * mainTex.height) : Vector2.zero;
    					Vector2 pMainUV2 = alphaLookup && mainTex ? new Vector2(mainUV[vi2].x * mainTex.width, mainUV[vi2].y * mainTex.height) : Vector2.zero;
     
    					//Square of pixels around triangle
    					Vector4 bounds = new Vector4(
    						Mathf.Floor(Mathf.Min(Mathf.Min(pUV0.x, pUV1.x), pUV2.x)),
    						Mathf.Ceil(Mathf.Max(Mathf.Max(pUV0.x, pUV1.x), pUV2.x)),
    						Mathf.Floor(Mathf.Min(Mathf.Min(pUV0.y, pUV1.y), pUV2.y)),
    						Mathf.Ceil(Mathf.Max(Mathf.Max(pUV0.y, pUV1.y), pUV2.y))
    					);
     
    					for (float x = bounds.x; x < bounds.y; x++) {
    						for (float y = bounds.z; y < bounds.w; y++) {
    							Vector2 texel = new Vector2(x + 0.5f, y + 0.5f); //Use the middel of the texel
     
    							//Check if the texel is inside the triangle
    							if (pUVTriangle.IsPointInside(texel)) {
    								Vector3 bCoords = pUVTriangle.PointToBaryCentric(texel);
     
    								Vector3 worldPosition = vertices[vi0] * bCoords.x + vertices[vi1] * bCoords.y + vertices[vi2] * bCoords.z;
    								Vector3 worldNormal = normals[vi0] * bCoords.x + normals[vi1] * bCoords.y + normals[vi2] * bCoords.z;
    								Vector3 raycastOffset = worldNormal.normalized * 0.0001f; ///Vector added to worldposition when raycasting so that objects hit themself more consistently
     
    								Color texelColor = lightmap.GetPixel((int) x, (int) y);
     
    								//The modifier based on the surfaces alpha
    								float alpha = 1;
    								if (alphaLookup) {
    									if (mainTex && (mainTex.format == TextureFormat.Alpha8 || mainTex.format == TextureFormat.ARGB32)) {
    										Vector2 mainTexel = bCoords.x * pMainUV0 + bCoords.y * pMainUV1 + bCoords.z * pMainUV2;
     
    										alpha = mainTex.GetPixel((int) mainTexel.x, (int) mainTexel.y).a;
    									}
    								}
     
    								foreach (Light light in lights) {
    									Transform lightTransform = light.transform;
    									float alphaMod = alpha;
     
    									//POINT LIGHT
    									if (light.type == LightType.Point) {
    										Vector3 toLight = lightTransform.position-worldPosition;
    										float distanceToLight = toLight.magnitude;
     
    										if (light.range > distanceToLight && !Obstructed(worldPosition+raycastOffset, toLight, distanceToLight, ref alphaMod)) {
    											float angleMod = AngleMod(toLight, worldNormal);
    											float attenuationMod = light.attenuate ? 2*Mathf.Pow(Mathf.Exp(-distanceToLight/light.range), 5) : 1;
     
    											texelColor += light.color * light.intensity * angleMod * attenuationMod * alphaMod;
    										}
    									}
    									//SPOT LIGHT
    									else if (light.type == LightType.Spot) {
    										Vector3 toLight = lightTransform.position-worldPosition;
    										float distanceToLight = toLight.magnitude;
     
    										float angleToSpot = Vector3.Angle(-toLight, lightTransform.forward);
     
    										if (angleToSpot < light.spotAngle/2) {
    											float projectedDistance = Mathf.Cos(angleToSpot*Mathf.Deg2Rad) * distanceToLight; //Distance to light (projected onto light.forward)
     
    											if (light.range > projectedDistance && !Obstructed(worldPosition+raycastOffset, toLight, distanceToLight, ref alphaMod)) {
    												//Edge falloff
    												float edgeMod = (spotFalloff - Mathf.Max(angleToSpot - ( (light.spotAngle/2) - spotFalloff ), 0)) / spotFalloff;
     
    												float angleMod = AngleMod(toLight, worldNormal);
    												float attenuationMod = light.attenuate ? Mathf.Max(1-Mathf.Pow(projectedDistance/light.range, 3), 0) : 1;
     
    												texelColor += light.color * light.intensity * edgeMod * angleMod * attenuationMod * alphaMod;
    											}
    										}
    									}
    									//DIRECTIONAL
    									else if (light.type == LightType.Directional) {
    										if (Vector3.Angle(lightTransform.forward, worldNormal) > 90) {
    											if (noDirectionalShadow || !Obstructed(worldPosition+raycastOffset, -lightTransform.forward, Mathf.Infinity, ref alphaMod)) {
    												float angleMod = AngleMod(-lightTransform.forward, worldNormal);
     
    												texelColor += light.color * light.intensity * angleMod * alphaMod;
    											}
    										}												
    									}
    								} //END foreach light
     
    								lightmap.SetPixel((int) x, (int) y, texelColor);
    							}						
    						}	
    					}
    				}
    			} //END if (format)
    			else {
    				Debug.LogError(string.Format("Unsupported format of lightmap in object {0}", renderer.name));	
    			}
    		} //END foreach (renderer)
     
    		//Smooth and save lightmaps
    		for (int lightmapIndex = 0; lightmapIndex < lightmaps.Count; lightmapIndex++) {
    			Texture2D lightmap = lightmaps[lightmapIndex];
     
    			EditorUtility.DisplayProgressBar(
    				"Lightmapping", 
    				string.Format("Smoothing: {0} ({1} of {2})", lightmap.name, lightmapIndex+1, lightmaps.Count), 
    				currentStep++ / (float) totalSteps
    			);
     
    			SmoothLightMaps(lightmap);
     
    			lightmap.Apply();
     
    			//Save to disk
    			if (saveLightMaps) {
    				File.WriteAllBytes(EditorUtility.GetAssetPath(lightmap), lightmap.EncodeToPNG());
    			}
    		}
     
    		EditorUtility.ClearProgressBar();			
    	}
     
    	void OnWizardOtherButton() {
    		lightmapMode = LightmapMode.ALL;
    		smoothing = 1;
    		saveLightMaps = false;
    		alphaLookup = false;
    		noDirectionalShadow = false;
    		layerMask = -1;
    		ambientColor = Color.black;
    	}
     
    	///Compute the intensity modifier from the angle of the surface
    	private float AngleMod(Vector3 forward, Vector3 normal) {
    		float angle = Vector3.Angle(forward, normal);
     
    		return Mathf.Max(((falloffEnd-falloffStart) - Mathf.Max(angle-falloffStart, 0)), 0) / (falloffEnd-falloffStart);
    	}
     
    	private bool Obstructed(Vector3 from, Vector3 direction, float distance, ref float alphaMod) {
    		RaycastHit[] hits = Physics.RaycastAll(from, direction, distance, layerMask);
     
    		foreach (RaycastHit hit in hits) {
    			Collider collider = hit.collider;
    			Renderer renderer = collider.renderer;
     
    			if (!collider.isTrigger && renderer.enabled) {
    				if (alphaLookup) {
    					Texture2D mainTex = renderer.sharedMaterial.mainTexture as Texture2D; //For alpha lookup
    					if (mainTex && (mainTex.format == TextureFormat.Alpha8 || mainTex.format == TextureFormat.ARGB32)) {
    						alphaMod *= 1f - mainTex.GetPixel((int) (hit.textureCoord.x*mainTex.width), (int) (hit.textureCoord.y*mainTex.height)).a;						
    					}
    					else {
    						return true; //The texture isn't an alpha texture (or no texture isn't set)
    					}
    				}
    				else {
    					return true;
    				}	
    			}
    		}
     
    		return false;
    	}
     
    	public void SmoothLightMaps(Texture2D lightmap) {
    		Color[] pixels = lightmap.GetPixels(0);
    		Color[] newPixels = new Color[pixels.Length];
     
    		for (int width = 0; width < lightmap.width; width++) {
    			for (int height = 0; height < lightmap.height; height++) {
    				int pixelsBlended = 0;
    				Color blendedColor = new Color(0, 0, 0, 1);
     
    				for (int offsetW = -smoothing; offsetW <= smoothing; offsetW++) {
    					for (int offsetH = -smoothing; offsetH <= smoothing; offsetH++) {
    						//If inside the texture
    						if (width + offsetW >= 0 && width + offsetW < lightmap.width &&
    							height + offsetH >= 0 && height + offsetH < lightmap.height) {
     
    							Color pixelColor = pixels[(height+offsetH)*lightmap.width + (width+offsetW)];
    							if (pixelColor.r > 0 || pixelColor.g > 0 || pixelColor.b > 0) { //Ignore texels that are black (this all but kill edge artifacts)
    								blendedColor += pixelColor;
    								pixelsBlended++;
    							}
    						}
    					}	
    				}
     
    				newPixels[height*lightmap.width + width] = (blendedColor/(pixelsBlended > 0 ? pixelsBlended : 1)) + ambientColor;				
    			}						
    		}
     
    		lightmap.SetPixels(newPixels, 0);		
    	}
     
    	///@return True if t is selected
    	private bool Selected(Transform t) {
    		foreach (Transform selectedTransform in Selection.transforms) {
    			if (t == selectedTransform) {
    				return true;	
    			}
    		}
    		return false;
    	}
     
    	///@return True if t is selected or a child of a selected transform	
    	private bool SelectedOrChild(Transform t) {
    		foreach (Transform selectedTransform in Selection.transforms) {
    			if (ChildOf(t, selectedTransform)) {
    				return true;	
    			}
    		}
    		return false;
    	}
    	///@return True if child if a child of parent
    	private bool ChildOf(Transform child, Transform parent) {
    		if (child == parent) {
    			return true;	
    		}
    		else if (child.parent != null) {
    			return ChildOf(child.parent, parent);
    		}
    		else {
    			return false;	
    		}
    	}
     
    	public struct Triangle2D {
    		Vector2 vert0;
    		Vector2 vert1;
    		Vector2 vert2;
     
    		public Triangle2D(Vector2 vert0, Vector2 vert1, Vector2 vert2) {
    			this.vert0 = vert0;
    			this.vert1 = vert1;
    			this.vert2 = vert2;
    		}
     
    		public Vector3 PointToBaryCentric(Vector2 point) {
    			float A, B, C, G, H, I;
     
    			A = vert0.x - vert2.x;
    			B = vert1.x - vert2.x;
    			C = vert2.x - point.x;
    			G = vert0.y - vert2.y;
    			H = vert1.y - vert2.y;
    			I = vert2.y - point.y;
     
    			float w1, w2, w3;
    			w1 = (B*I - C*H) / (A*H - B*G);
    			w2 = (A*I - C*G) / (B*G - A*H);
    			w3 = 1 - w1 - w2;
     
    			return new Vector3(w1, w2, w3);
    		}
     
    		public bool IsPointInside(Vector2 point) {
    			Vector3 weights = PointToBaryCentric(point);
    			return (weights.x >= 0 && weights.y >= 0 && weights.z >= 0);
    		}	
    	}
}
}
