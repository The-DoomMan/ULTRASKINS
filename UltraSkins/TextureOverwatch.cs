using UnityEngine;

namespace UltraSkins
{

	public class TextureOverWatch : MonoBehaviour
	{
		public Material[] cachedMaterials;
		public Renderer renderer;
		public bool forceswap;

		private void Update()
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
			}
		}

		private void OnDisable()
        {
			transform.GetComponent<TextureOverWatch>().enabled = true;
		}
	}
}
