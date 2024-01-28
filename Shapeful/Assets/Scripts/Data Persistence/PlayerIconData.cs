using System;
using UnityEngine;

namespace CSTGames.DataPersistence
{
	public enum PlayerSpriteLayer { Primary, Secondary, Static }

	[Serializable]
	public class PlayerIconData
	{
		[HideInInspector] public int index;
		public string iconName;
		public byte[] staticTextureData;
		public byte[] primaryTextureData;
		public byte[] secondaryTextureData;

		// Constants and readonly fields.
		[NonSerialized] public const int PIXEL_PER_UNIT = 17;
		[NonSerialized] public readonly Vector2 pivot = Vector2.one * .5f;

		public bool IsDefault => this.index == -1;

		public PlayerIconData()
		{
			this.index = -1;
			this.iconName = "Icon_Default";

			this.staticTextureData = null;
			this.primaryTextureData = null;
			this.secondaryTextureData = null;
		}

		public PlayerIconData(IconCustomizeMenu.PlayerIcon playerIcon)
		{
			this.index = playerIcon.index;
			this.iconName = playerIcon.iconName;

			this.staticTextureData = playerIcon.staticSprite.GetSlicedTexture().EncodeToPNG();
			this.primaryTextureData = playerIcon.primarySprite.GetSlicedTexture().EncodeToPNG();
			this.secondaryTextureData = playerIcon.secondarySprite.GetSlicedTexture().EncodeToPNG();
		}

		public Sprite ReconstructSprite(PlayerSpriteLayer layer)
		{
			Texture2D tex = new Texture2D(1, 1);

			tex.filterMode = FilterMode.Point;

			switch (layer)
			{
				case PlayerSpriteLayer.Static:
					tex.LoadImage(staticTextureData);
					break;

				case PlayerSpriteLayer.Secondary:
					tex.LoadImage(secondaryTextureData);
					break;
				
				case PlayerSpriteLayer.Primary:
					tex.LoadImage(primaryTextureData);
					break;
			}

			return Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), pivot, PIXEL_PER_UNIT);
		}
	}
}
