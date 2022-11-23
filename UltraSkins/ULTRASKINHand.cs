﻿using System;
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
	[UKPlugin("ULTRASKINS", "1.4.0", "This mod lets you replace the weapon and arm textures to your liking. \n please read the included readme file inside of the ULTRASKINS folder", true, true)]
	public class ULTRASKINHand : UKMod
	{

		public static Dictionary<string, Texture> autoSwapCache = new Dictionary<string, Texture>();
		public static List<string> failedloads = new List<string>();
		public string[] directories;
		public string serializedSet = "";
        public bool swapped = false;
        Harmony UKSHarmony;

		public override void OnModLoaded()
		{
			UKSHarmony = new Harmony("Tony.UltraSkins");
            UKSHarmony.PatchAll(typeof(HarmonyGunPatcher));
            UKSHarmony.PatchAll();
		}

		public override void OnModUnload()
		{
			UKSHarmony.UnpatchSelf();
			UKSHarmony = null;
		}

        [HarmonyPatch]
        public class HarmonyGunPatcher
		{
            [HarmonyPatch(typeof(GunControl), "SwitchWeapon", new Type[] { typeof(int), typeof(List<GameObject>), typeof(bool), typeof(bool) })]
            [HarmonyPostfix]
            public static void SwitchWeaponPost(GunControl __instance, int target, List<GameObject> slot, bool lastUsed = false, bool scrolled = false)
            {

                TextureOverWatch[] TOWS = __instance.currentWeapon.GetComponentsInChildren<TextureOverWatch>(true);
                ReloadTextureOverWatch(TOWS);
            }

            [HarmonyPatch(typeof(GunControl), "YesWeapon")]
            [HarmonyPostfix]
            public static void WeaponYesPost(GunControl __instance)
            {
                TextureOverWatch[] TOWS = __instance.currentWeapon.GetComponentsInChildren<TextureOverWatch>(true);
                ReloadTextureOverWatch(TOWS);
            }

            [HarmonyPatch(typeof(FistControl), "YesFist")]
            [HarmonyPostfix]
            public static void YesFistPost(FistControl __instance)
            {

                TextureOverWatch[] TOWS = __instance.currentArmObject.GetComponentsInChildren<TextureOverWatch>(true);
                ReloadTextureOverWatch(TOWS);
            }

            [HarmonyPatch(typeof(FistControl), "ArmChange", new Type[] { typeof(int) })]
            [HarmonyPostfix]
            public static void SwitchFistPost(FistControl __instance, int orderNum)
            {
                TextureOverWatch[] TOWS = __instance.currentArmObject.GetComponentsInChildren<TextureOverWatch>(true);
				ReloadTextureOverWatch(TOWS);
            }

            [HarmonyPatch(typeof(PlayerTracker), "ChangeToFPS")]
            [HarmonyPostfix]
            public static void ChangeToFPSPost(PlayerTracker __instance)
            {
                TextureOverWatch[] WTOWS = GameObject.FindGameObjectWithTag("GunControl").GetComponent<GunControl>().currentWeapon.GetComponentsInChildren<TextureOverWatch>(true);
                ReloadTextureOverWatch(WTOWS);
                TextureOverWatch[] FTOWS = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<FistControl>().currentArmObject.GetComponentsInChildren<TextureOverWatch>(true);
                ReloadTextureOverWatch(FTOWS);
            }


            public static void ReloadTextureOverWatch(TextureOverWatch[] TOWS)
			{
                foreach (TextureOverWatch TOW in TOWS)
                {
                    TOW.enabled = true;
                }
            }
        }

        private void Start()
        {
            SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
        }

		private void SceneManagerOnsceneLoaded(Scene scene, LoadSceneMode mode)
		{
			swapped = false;
			CreateSkinGUI();
		}

		private void Update()
		{
			if (!swapped)
			{
				if (UKMod.PersistentModDataExists("SkinsFolder", "Tony.UltraSkins"))
				{
					serializedSet = RetrieveStringPersistentModData("SkinsFolder", "Tony.UltraSkins");
				}
				ReloadTextures(true);
				swapped = true;
			}
        }


        public static Texture ResolveTheTextureProperty(Material mat, string property)
        {
			string textureToResolve = "";
			if (mat)
			{
				//Debug.Log("Switching:" + mat.name + " " + property);
				switch (property)
				{
					case "_MainTex":
						textureToResolve = mat.mainTexture.name;
                       		break;
					case "_EmissiveTex":
						switch (mat.mainTexture.name)
                        {
                            case "T_NailgunNew_NoGlow":
                                textureToResolve = "T_Nailgun_New_Glow";
                                break;
                            default:
                                textureToResolve = mat.mainTexture.name + "_Emissive";
                                break;
                        }
						break;
					case "_IDTex":
						switch(mat.mainTexture.name)
						{
							case "T_RocketLauncher_Desaturated":
								textureToResolve = "T_RocketLauncher_ID";
								break;
							case "T_NailgunNew_NoGlow":
								textureToResolve = "T_NailgunNew_ID";
								break;
							default:
								textureToResolve = mat.mainTexture.name + "_ID";
								break;
						}
							break;
						default:
                            textureToResolve = "";
							break;
				}
				if (textureToResolve != "" && autoSwapCache.ContainsKey(textureToResolve))
                    //Debug.Log("Switched:" + mat.name + "." + property + ", tex name: " + textureToResolve);
                    return autoSwapCache[textureToResolve];
            }
            //Debug.Log("failed Switching:" + mat.name + "." + property + ", attempted tex name: " + attemptedresolve + " , Proper texture: " + mat.GetTexture(property).name);
            return mat.GetTexture(property);
        }

		public static void PerformTheSwap(Material mat, bool forceswap = false)
		{
			if (mat && (!mat.name.StartsWith("Swapped_") || forceswap))
			{
				if (!mat.name.StartsWith("Swapped_"))
				{
						mat.name = "Swapped_" + mat.name;
				}
				forceswap = false;
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
					}
                }
            }
        }

		public void CreateSkinGUI()
		{
			foreach (ShopGearChecker shopGearChecker in Resources.FindObjectsOfTypeAll<ShopGearChecker>())
			{
                string[] dirs = Directory.GetDirectories(modFolder);
				directories = dirs;
                ShopCategory[] SCs = shopGearChecker.GetComponentsInChildren<ShopCategory>(true);
				GameObject PresetsMenu = Instantiate(shopGearChecker.transform.GetChild(3).GetComponent<ShopButton>().toActivate[0], shopGearChecker.transform);
				PresetsMenu.name = "ultraskins window";
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
				button.name = "ultraskins button";
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
				int numberofpages = (dirs.Length/3);
                GameObject pageHandler = Instantiate(new GameObject(), PresetsMenu.transform);
                pageHandler.name = "Page Handler";
                pageHandler.transform.localPosition = new Vector3(0, 0, 0);
                PageEventHandler PGEH = pageHandler.AddComponent<PageEventHandler>();
                PGEH.UKSH = transform.GetComponent<ULTRASKINHand>();
                PGEH.pagesamount = numberofpages;
                for(int e = 0; e < numberofpages + 1; e++)
				{
                    GameObject Page = Instantiate(new GameObject(), pageHandler.transform);
                    Page.name = "Page" + e;
                    Page.transform.localPosition = new Vector3(0, 0, 0);
					for (int d = 0; d < ((e == numberofpages) ? dirs.Length % 3 : 3); d++)
					{
						int pagebuttonnumber = Mathf.Clamp((e * 3) + d, 0, dirs.Length - 1);
						GameObject FoldBut = Instantiate(FolderButton, Page.transform);
						FoldBut.name = "button" + pagebuttonnumber;
						Destroy(FoldBut.transform.GetChild(2).gameObject);
						Destroy(FoldBut.transform.GetChild(4).gameObject);
						Destroy(FoldBut.transform.GetChild(5).gameObject);
						FoldBut.GetComponentInChildren<Text>().text = Path.GetFileName(dirs[pagebuttonnumber]);
						FoldBut.GetComponentInChildren<Text>().transform.localPosition = new Vector3(-325, 15, -15);
						FoldBut.transform.localPosition = new Vector3(0, 300 - (85 * d), -15);
						GameObject AGO = Instantiate(FoldBut, button.transform);
						AGO.SetActive(false);
						SkinEventHandler skinEventHandler = FoldBut.gameObject.AddComponent<SkinEventHandler>();
						skinEventHandler.UKSH = transform.GetComponent<ULTRASKINHand>();
						skinEventHandler.Activator = AGO;
						skinEventHandler.path = dirs[pagebuttonnumber];
						skinEventHandler.pname = Path.GetFileName(dirs[pagebuttonnumber]);
						FoldBut.GetComponent<ShopButton>().toActivate = new GameObject[] { AGO };
						FoldBut.GetComponent<ShopButton>().toDeactivate = new GameObject[0];
					}
					if(e != 0)
					Page.gameObject.SetActive(false);
                }
                for (int r = 0; r < 2; r++)
                {
                    GameObject FoldBut = Instantiate(FolderButton, PresetsMenu.transform);
                    Destroy(FoldBut.transform.GetChild(2).gameObject);
                    Destroy(FoldBut.transform.GetChild(4).gameObject);
                    Destroy(FoldBut.transform.GetChild(5).gameObject);
                    FoldBut.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 80);
                    FoldBut.gameObject.name = (r == 1) ? "<<" : ">>";
                    FoldBut.GetComponentInChildren<Text>().text = (r == 1) ? "<<" : ">>";
					FoldBut.GetComponentInChildren<Text>().fontSize = 34;
                    FoldBut.GetComponentInChildren<Text>().transform.localPosition = new Vector3(-95, 15, -15);
                    FoldBut.transform.localPosition = new Vector3((r == 1) ? -180 : 0, 45, -15);
                    GameObject AGO = Instantiate(FoldBut, button.transform);
                    AGO.SetActive(false);
                    PageButton PEH = FoldBut.gameObject.AddComponent<PageButton>();
                    PEH.UKSH = transform.GetComponent<ULTRASKINHand>();
					PEH.pageEventHandler = PGEH;
					PEH.Activator = AGO;
					PEH.moveamount = (r == 1) ? -1 : 1;
                    FoldBut.GetComponent<ShopButton>().toActivate = new GameObject[] { AGO };
                    FoldBut.GetComponent<ShopButton>().toDeactivate = new GameObject[0];
                }
                FolderButton.SetActive(false);
            }
        }

		public string ReloadTextures(bool firsttime = false, string path = "")
		{
			if(firsttime && serializedSet != "")
			{
				path = Path.Combine(modFolder, serializedSet);
            }
			else if (firsttime && serializedSet == "")
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
				if (renderer.gameObject.layer == 13 && !renderer.gameObject.GetComponent<ParticleSystemRenderer>() && !renderer.gameObject.GetComponent<TrailRenderer>() && !renderer.gameObject.GetComponent<LineRenderer>())
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
            autoSwapCache.Clear();
			bool failed = false;
            DirectoryInfo dir = new DirectoryInfo(fpath);
            FileInfo[] Files = dir.GetFiles("*.png");
            foreach (FileInfo file in Files) 
			{
				if(file.Exists)
				{
					byte[] data = File.ReadAllBytes(file.FullName);
					string name = Path.GetFileNameWithoutExtension(file.FullName);
                    Texture2D texture2D = new Texture2D(2, 2);
                    texture2D.name = name;
                    texture2D.filterMode = FilterMode.Point;
                    texture2D.LoadImage(data);
                    texture2D.Apply();
					if (file.Name == "Railgun_Main_AlphaGlow.png")
                    {
                        Texture2D texture2D2 = new Texture2D(2, 2);
                        byte[] data2 = File.ReadAllBytes(Path.Combine(file.DirectoryName, "Railgun_Main_Emission.png"));
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
                    autoSwapCache.Add(name, texture);
                }
                else
                {
                    failed = true;
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