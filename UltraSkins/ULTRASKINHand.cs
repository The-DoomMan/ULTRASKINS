using System.Collections.Generic;
using System.IO;
using BepInEx;
using UMM;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UltraSkins
{
	[UKPlugin("ULTRASKINS", "1.2.1", "This mod lets you replace the weapon and arm textures to your liking. \n please read the included readme file inside of the ULTRASKINS folder", true, true)]
	public class ULTRASKINHand : UKMod
	{

		public static string listPath = Path.Combine(Path.Combine(Path.Combine(Paths.BepInExRootPath, "UMM Mods"), "ULTRASKINS"), "TexturesToLoad.txt"); 
		public static string[] autoSwapTextures = File.ReadAllLines(listPath);

		public static Dictionary<string, Texture2D> autoSwapCache = new Dictionary<string, Texture2D>();

		public static bool swapped;

		private void Start()
		{
			SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
		}

		public static Texture2D ResolveTheTexture(Material mat)
		{
			if (mat && autoSwapCache.ContainsKey(mat.mainTexture.name))
			{
				return autoSwapCache[mat.mainTexture.name];
			}
			return null;
		}

		public static void PerformTheSwap(Material mat, bool forceswap = false)
		{
			if (mat && (!mat.name.StartsWith("Swapped_") || forceswap))
			{
				forceswap = false;
				if (!mat.name.StartsWith("Swapped_"))
				{
					mat.name = "Swapped_" + mat.name;
				}
				if (mat.HasProperty("_MainTex") && mat.mainTexture)
				{
					Texture2D y = ULTRASKINHand.ResolveTheTexture(mat);
					if (y != null && mat.mainTexture != y)
					{
						mat.mainTexture = ULTRASKINHand.ResolveTheTexture(mat);
					}
				}
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
				Transform button = Instantiate(shopGearChecker.transform.GetChild(3), shopGearChecker.transform);
				button.localPosition = new Vector3(-180f, -85f, -45f);
				button.localScale = new Vector3(1f, 1f, 1f);
				GameObject AGO = Instantiate(new GameObject(), button.transform);
				AGO.SetActive(false);
				SkinEventHandler skinEventHandler = button.gameObject.AddComponent<SkinEventHandler>();
				skinEventHandler.UKSH = transform.GetComponent<ULTRASKINHand>();
				skinEventHandler.Activator = AGO;
				button.GetComponent<ShopButton>().toActivate = new GameObject[] { AGO };
				button.GetComponent<ShopButton>().toDeactivate = new GameObject[0];
				button.GetComponentInChildren<Text>().text = "RELOAD SKINS";
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

		public string ReloadTextures(bool firsttime = false)
		{
			if (listPath == "")
			{
				listPath = Path.Combine(modFolder, "TexturesToLoad.txt");
			}
			InitOWGameObjects(firsttime);
			return LoadTextures();
		}

		public void InitOWGameObjects(bool firsttime = false)
		{
			GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
			foreach (Renderer renderer in cam.GetComponentsInChildren<Renderer>(true))
			{
				if (!renderer.gameObject.GetComponent<ParticleSystemRenderer>())
				{
					if (renderer.GetComponent<TextureOverWatch>() && !firsttime)
					{
						renderer.GetComponent<TextureOverWatch>().forceswap = true;
					}
					else if (!renderer.GetComponent<TextureOverWatch>())
					{
						renderer.gameObject.AddComponent<TextureOverWatch>().enabled = true;
					}
				}
			}
		}

		public string LoadTextures()
		{
			autoSwapTextures = File.ReadAllLines(listPath);
			autoSwapCache.Clear();
			bool failed = false;
			foreach (string text in autoSwapTextures)
			{
				if (text != autoSwapTextures[0] && text != "\n" && text != "")
				{
					Debug.Log("Loading " + text + " for auto swap");
					string path = Path.Combine(Path.Combine(modFolder, autoSwapTextures[0]), text + ".png");
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
						autoSwapCache.Add(text, texture2D);
					}
					else
					{
						failed = true;
					}
				}
			}
			if (!failed)
			{
				return "Successfully loaded all Textures from " + autoSwapTextures[0] + "!";
			}
			return "Failed to load all textures from " + autoSwapTextures[0] + ".\nPlease ensure all of the Texture Files names are Correct, refer to the README file for the correct names and more info.";
		}
	}
}
