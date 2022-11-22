using UnityEngine;

namespace UltraSkins
{

	public class TextureOverWatch : MonoBehaviour
	{
		public Material[] cachedMaterials;
		public Renderer renderer;
		public bool forceswap;

		public void UpdateMaterials(GameObject weaponsource = null)
		{
			if (!renderer)
			{
				renderer = GetComponent<Renderer>();
			}
			if (renderer && renderer.materials != cachedMaterials)
			{
				Material[] materials = renderer.materials;
				for (int i = 0; i < materials.Length; i++)
				{
					ULTRASKINHand.PerformTheSwap(materials[i], forceswap);
				}
				cachedMaterials = renderer.materials;
                transform.GetComponent<TextureOverWatch>().enabled = false;
            }
		}
	}
}
