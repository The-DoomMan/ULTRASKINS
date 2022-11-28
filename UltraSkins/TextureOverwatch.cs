using UnityEngine;

namespace UltraSkins
{

	public class TextureOverWatch : MonoBehaviour
	{
		public Material[] cachedMaterials;
		public Renderer renderer;
		public bool forceswap;

		void OnEnable()
		{
            if (!renderer)
            {
                renderer = GetComponent<Renderer>();
            }
            if (renderer.materials != cachedMaterials)
			UpdateMaterials();
		}

		public void UpdateMaterials()
		{
			if (renderer && renderer.materials != cachedMaterials)
			{
				Material[] materials = renderer.materials;
				for (int i = 0; i < materials.Length; i++)
				{
					ULTRASKINHand.PerformTheSwap(materials[i], forceswap, transform.GetComponent<TextureOverWatch>());
				}
				cachedMaterials = renderer.materials;
            }
			transform.GetComponent<TextureOverWatch>().enabled = false;
		}
    }
}
