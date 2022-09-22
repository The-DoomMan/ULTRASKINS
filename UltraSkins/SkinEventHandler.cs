using UnityEngine;

namespace UltraSkins
{
	public class SkinEventHandler : MonoBehaviour
	{
		public GameObject Activator;
		public ULTRASKINHand UKSH;

		private void Update()
		{
			if (Activator.activeSelf)
			{
				Activator.SetActive(false);
				string message = UKSH.ReloadTextures(false);
				MonoSingleton<HudMessageReceiver>.Instance.SendHudMessage(message, "", "", 0, false);
			}
		}
	}
}