using UnityEngine;

public static class SpriteExtensions
{
	/// <summary>
	/// Get the texture of a specific sprite sliced from the parent sprite sheet by the Unity Sprite Editor.
	/// </summary>
	/// <param name="slicedSprite"></param>
	/// <returns> The texture the provided sliced sprite. </returns>
	public static Texture2D GetSlicedTexture(this Sprite slicedSprite)
	{
		Rect rect = slicedSprite.rect;
		Color[] colors = slicedSprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);

		Texture2D slicedTex = new Texture2D((int)rect.width, (int)rect.height);
		
		slicedTex.filterMode = FilterMode.Point;
		slicedTex.SetPixels(0, 0, slicedTex.width, slicedTex.height, colors);
		slicedTex.Apply();

		return slicedTex;
	}
}
