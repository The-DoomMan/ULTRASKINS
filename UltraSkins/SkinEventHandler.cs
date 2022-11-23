using UMM;
using UnityEngine;
using System.IO;
using GameConsole;

namespace UltraSkins
{
	public class SkinEventHandler : MonoBehaviour
	{
		public GameObject Activator;
		public string path;
		public string pname;
		public ULTRASKINHand UKSH;

		private void Update()
		{
			if (Activator != null && Activator.activeSelf)
			{
				Activator.SetActive(false);
				string message = UKSH.ReloadTextures(false, path);
				UKMod.SetPersistentModData("SkinsFolder", pname, "Tony.UltraSkins");
				TextureOverWatch[] TOWS = GameObject.FindWithTag("MainCamera").GetComponentsInChildren<TextureOverWatch>(true);
				foreach (TextureOverWatch TOW in TOWS)
				{
					if (TOW && TOW.gameObject)
					{
						TOW.enabled = true;
                    }
				}
                MonoSingleton<HudMessageReceiver>.Instance.SendHudMessage(message, "", "", 0, false);
			}
		}
	}
}