using HarmonyLib;
using UnityEngine;

namespace UltraSkins
{
	[HarmonyPatch(typeof(Material), "mainTexture", MethodType.Getter)]
	internal class RendererPatchGetter
	{
		/*private static bool Prefix(Material __instance, ref Texture __result)
		{
			ULTRASKINHand.PerformTheSwap(__instance);
			return true;
		}*/
	}
}