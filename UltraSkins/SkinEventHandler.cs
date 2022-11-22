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
				foreach (TextureOverWatch TOW in Resources.FindObjectsOfTypeAll<TextureOverWatch>())
				{
					TOW.enabled = true;
                    TOW.forceswap = true;
                    TOW.UpdateMaterials(null);
				}
                MonoSingleton<HudMessageReceiver>.Instance.SendHudMessage(message, "", "", 0, false);
			}
		}
	}
}