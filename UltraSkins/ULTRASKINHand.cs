using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UMM;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace UltraSkins
{
	[UKPlugin("ULTRASKINS", "1.2.1", "This mod lets you replace the weapon and arm textures to your liking. \n please read the included readme file inside of the ULTRASKINS folder", true, true)]
	[HarmonyPatch]
	public class ULTRASKINHand : UKMod
	{

		public static string listPath = Path.Combine(Path.Combine(Path.Combine(Paths.BepInExRootPath, "UMM Mods"), "ULTRASKINS"), "TexturesToLoad.txt");
		public static string[] autoSwapTextures = File.ReadAllLines(listPath);

		public static Dictionary<string, Texture> autoSwapCache = new Dictionary<string, Texture>();
		public string[] directories;

		public static bool swapped;

		Harmony UKSHarmony;

		public override void OnModLoaded()
		{
			UKSHarmony = new Harmony("Tony.UltraSkins");
			UKSHarmony.PatchAll();
		}

		public override void OnModUnload()
		{
			UKSHarmony.UnpatchSelf();
			UKSHarmony = null;
		}

		[HarmonyPatch(typeof(GunControl), "SwitchWeapon", new Type[] {typeof(int), typeof(List<GameObject>), typeof(bool), typeof(bool)})]
		[HarmonyPostfix]
		public static void SwitchWeaponPost(GunControl __instance, int target, List<GameObject> slot, bool lastUsed = false, bool scrolled = false)
        {
	
			TextureOverWatch[] TOWS = __instance.currentWeapon.GetComponentsInChildren<TextureOverWatch>(true);
			foreach (TextureOverWatch TOW in TOWS)
			{
				TOW.UpdateMaterials(__instance.currentWeapon);
			}
		}


       /* [HarmonyPatch(typeof(GunControl), "ResetWeapons", new Type[] { typeof(bool) })]
        [HarmonyPostfix]
        public static void UpdateWPPost(GunControl __instance, bool firstTime = false)
        {

            TextureOverWatch[] TOWS = __instance.currentWeapon.GetComponentsInChildren<TextureOverWatch>(true);
            foreach (TextureOverWatch TOW in TOWS)
            {
                TOW.UpdateMaterials(__instance.currentWeapon);
            }
        }*/


        [HarmonyPatch(typeof(FistControl), "ArmChange", new Type[] { typeof(int) })]
        [HarmonyPostfix]
        public static void SwitchFistPost(FistControl __instance, int orderNum)
        {

            TextureOverWatch[] TOWS = __instance.currentArmObject.GetComponentsInChildren<TextureOverWatch>(true);
            foreach (TextureOverWatch TOW in TOWS)
            {
                TOW.UpdateMaterials(__instance.currentArmObject);
            }
        }

        private void Start()
		{
			SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
		}

		/*public static Texture2D ResolveTheTexture(Material mat)
		{
			if (mat && autoSwapCache.ContainsKey(mat.mainTexture.name))
			{
				return autoSwapCache[mat.mainTexture.name];
			}
			return null;
		}*/

        public static Texture ResolveTheTextureProperty(Material mat, string property)
        {
			string textureToResolve = "";
			if (mat)
			{
				/*if (weapon.GetComponent<Nailgun>() && !weapon.GetComponent<Nailgun>().altVersion)
				{
                    switch (property)
                    {
                        case "_MainTex":
                            textureToResolve = "T_NailgunNew_NoGlow";
                            break;
                        case "_EmissiveTex":
                            textureToResolve = "T_Nailgun_New_Glow";
                            break;
                        case "_IDTex":
                            textureToResolve = "T_NailgunNew_ID";
                            break;
                        default:
                            break;
                    }
                }
				else*/
				{
					switch (property)
					{
						case "_MainTex":
							textureToResolve = mat.mainTexture.name;
							break;
						case "_EmissiveTex":
							textureToResolve = mat.mainTexture.name + "_Emissve";
							break;
						case "_IDTex":
							textureToResolve = mat.mainTexture.name + "_ID";
							break;
						default:
                            textureToResolve = "";
                            break;
					}
				}
				if(textureToResolve != "" && autoSwapCache.ContainsKey(textureToResolve))
                return autoSwapCache[textureToResolve];
            }
            return mat.GetTexture(property);
        }

		public static void PerformTheSwap(Material mat, bool forceswap = false)
		{
			if (mat && (!mat.name.StartsWith("Swapped_") || forceswap))
			{
				forceswap = false;
				/* List of all important/possible properties
				_MainTex
				_AlphaTex
				_MetallicGlossMap
				_BumpMap
				_EmissionMap
				_EmissiveTex
				_IDTex
				*/
				/*Debug.Log("============Start============");
				int[] TextProperties = mat.GetTexturePropertyNameIDs();
				for(int s = 0; s < TextProperties.Length; s++)
				{
					Debug.Log("Material: " + mat.name + ", Porperty: " + mat.GetTexturePropertyNames()[s] + ", ID :" + TextProperties[s]);
				}
                Debug.Log("=============End=============");*/
				string[] textureProperties = mat.GetTexturePropertyNames();
				foreach (string property in textureProperties)
				{
						Texture resolvedTexture = ULTRASKINHand.ResolveTheTextureProperty(mat, property);
						if (resolvedTexture && resolvedTexture != null && mat.GetTexture(property) != resolvedTexture)
						{
							mat.SetTexture(property, resolvedTexture);
							if (!mat.name.StartsWith("Swapped_"))
							{
								mat.name = "Swapped_" + mat.name;
							}
						}
                }
                /*if (mat.HasProperty("_MainTex") && mat.mainTexture)
				{
					Texture2D y = ULTRASKINHand.ResolveTheTexture(mat);
					if (y != null && mat.mainTexture != y)
					{
						mat.mainTexture = ULTRASKINHand.ResolveTheTexture(mat);
					}
				}*/
            }
        }

		private void SceneManagerOnsceneLoaded(Scene scene, LoadSceneMode mode)
		{
			swapped = false;
			CreateSkinGUI();
		}

		public void CreateSkinGUI()
		{
			foreach (ShopGearChecker shopGearChecker in Resources.FindObjectsOfTypeAll<ShopGearChecker>())
			{
                string[] dirs = Directory.GetDirectories(modFolder);
				directories = dirs;
                ShopCategory[] SCs = shopGearChecker.GetComponentsInChildren<ShopCategory>(true);
				GameObject PresetsMenu = Instantiate(shopGearChecker.transform.GetChild(3).GetComponent<ShopButton>().toActivate[0], shopGearChecker.transform);
				PresetsMenu.SetActive(false);
				foreach (ShopCategory SC in SCs)
				{
					List<GameObject> deactivateobjects = new List<GameObject>();
					for (int s = 0; s < SC.GetComponent<ShopButton>().toDeactivate.Length; s++)
					{
						deactivateobjects.Add(SC.GetComponent<ShopButton>().toDeactivate[s]);
					}
					deactivateobjects.Add(PresetsMenu);
					SC.GetComponent<ShopButton>().toDeactivate = deactivateobjects.ToArray();
				}
				Transform button = Instantiate(shopGearChecker.transform.GetChild(3), shopGearChecker.transform);
				button.localPosition = new Vector3(-180f, -85f, -45f);
				button.localScale = new Vector3(1f, 1f, 1f);
				button.GetComponent<ShopButton>().toActivate = new GameObject[] { PresetsMenu };
				button.GetComponent<ShopButton>().toDeactivate = new GameObject[] 
				{ 
					shopGearChecker.transform.GetChild(9).gameObject, 
					shopGearChecker.transform.GetChild(10).gameObject,
					shopGearChecker.transform.GetChild(11).gameObject,
					shopGearChecker.transform.GetChild(12).gameObject,
					shopGearChecker.transform.GetChild(13).gameObject,
                    shopGearChecker.transform.GetChild(14).gameObject
                };
                button.GetComponentInChildren<Text>().text = "ULTRASKINS";
				for(int p = 2; p < PresetsMenu.transform.childCount; p++)
				{
					Destroy(PresetsMenu.transform.GetChild(p).gameObject);
				}
                GameObject FolderButton = PresetsMenu.transform.GetChild(1).gameObject;
                FolderButton.SetActive(true);
				for(int e = 0; e < dirs.Length; e++)
				{
					GameObject FoldBut = Instantiate(FolderButton, PresetsMenu.transform);
                    Destroy(FoldBut.transform.GetChild(2).gameObject);
                    Destroy(FoldBut.transform.GetChild(4).gameObject);
                    Destroy(FoldBut.transform.GetChild(5).gameObject);
                    FoldBut.GetComponentInChildren<Text>().text = Path.GetFileName(dirs[e]);
                    FoldBut.GetComponentInChildren<Text>().transform.localPosition = new Vector3(-325, 15, -15);
                    FoldBut.transform.localPosition = new Vector3(0, 300 - (85 * e), -15);
                    GameObject AGO = Instantiate(FoldBut, button.transform);
                    AGO.SetActive(false);
                    SkinEventHandler skinEventHandler = button.gameObject.AddComponent<SkinEventHandler>();
                    skinEventHandler.UKSH = transform.GetComponent<ULTRASKINHand>();
                    skinEventHandler.Activator = AGO;
					skinEventHandler.path = dirs[e];
					skinEventHandler.pname = Path.GetFileName(dirs[e]);
					FoldBut.GetComponent<ShopButton>().toActivate = new GameObject[] { AGO };
                    FoldBut.GetComponent<ShopButton>().toDeactivate = new GameObject[0];
                }
				FolderButton.SetActive(false);
            }
        }

		private void Update()
		{
			if (!swapped)
			{
                swapped = true;
				ReloadTextures(true);
			}
		}

		public string ReloadTextures(bool firsttime = false, string path = "")
		{
			if (listPath == "")
			{
				listPath = Path.Combine(modFolder, "TexturesToLoad.txt");
			}
			if(path == "")
			{
				Path.Combine(modFolder, "OG Textures");
            }
			InitOWGameObjects(firsttime);
			return LoadTextures(path);
		}

		public void InitOWGameObjects(bool firsttime = false)
		{
			GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
			foreach (Renderer renderer in cam.GetComponentsInChildren<Renderer>(true))
			{
				if (renderer.gameObject.layer != 2 && !renderer.gameObject.GetComponent<ParticleSystemRenderer>() && !renderer.gameObject.GetComponent<TrailRenderer>() && !renderer.gameObject.GetComponent<LineRenderer>())
				{
					if (renderer.GetComponent<TextureOverWatch>() && !firsttime)
					{
						TextureOverWatch TOW = renderer.GetComponent<TextureOverWatch>();
                        TOW.enabled = true;
						TOW.forceswap = true;
                    }
					else if (!renderer.GetComponent<TextureOverWatch>())
					{
						renderer.gameObject.AddComponent<TextureOverWatch>().enabled = true;
					}
				}
			}
		}

		public string LoadTextures(string fpath = "")
		{
			autoSwapTextures = File.ReadAllLines(listPath);
			autoSwapCache.Clear();
			bool failed = false;
			foreach (string text in autoSwapTextures)
			{
				if (text != autoSwapTextures[0] && text != "\n" && text != "")
				{
					//Debug.Log("Loading " + text + " for auto swap");
					string path = Path.Combine(fpath, text + ".png");
                    if (File.Exists(path))
					{
						byte[] data = File.ReadAllBytes(path);
						Texture2D texture2D = new Texture2D(2, 2);
						texture2D.name = text;
						texture2D.filterMode = FilterMode.Point;
						texture2D.LoadImage(data);
						texture2D.Apply();
						if (text == "Railgun_Main_AlphaGlow")
						{
							Texture2D texture2D2 = new Texture2D(2, 2);
							byte[] data2 = File.ReadAllBytes(Path.Combine(Path.Combine(modFolder, autoSwapTextures[0]), "Railgun_Main_Emission.png"));
							texture2D2.filterMode = FilterMode.Point;
							texture2D2.LoadImage(data2);
							texture2D2.Apply();
							Color[] pixels = texture2D.GetPixels();
							Color[] pixels2 = texture2D2.GetPixels();
							for (int k = 0; k < pixels.Length; k++)
							{
								pixels[k].a = pixels2[k].r;
							}
							texture2D.SetPixels(pixels);
							texture2D.Apply();
						}
						Texture texture = new Texture();
						texture = texture2D;
						autoSwapCache.Add(text, texture);
					}
					else
					{
						failed = true;
					}
				}
			}
			if (!failed)
			{
				return "Successfully loaded all Textures from " + Path.GetFileName(fpath) + "!";
			}
			return "Failed to load all textures from " + Path.GetFileName(fpath) + ".\nPlease ensure all of the Texture Files names are Correct, refer to the README file for the correct names and more info.";
		}
	}
}
