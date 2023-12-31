using System;
using UnityEngine;

namespace CSTGames.DataPersistence
{
	[Serializable]
	public class PlayerIconData
	{
		[HideInInspector] public int index;
		public string iconName;
		public byte[] primaryTextureData;
		public byte[] secondaryTextureData;

		// Constants and readonly fields.
		[NonSerialized] public const int PIXEL_PER_UNIT = 17;
		[NonSerialized] public readonly Vector2 pivot = Vector2.one * .5f;

		public PlayerIconData()
		{
			this.index = -1;
			this.iconName = "default_icon";
			this.primaryTextureData = null;
			this.secondaryTextureData = null;
		}

		public PlayerIconData(IconCustomizeMenu.PlayerIcon playerIcon)
		{
			this.index = playerIcon.index;
			this.iconName = playerIcon.iconName;
			this.primaryTextureData = playerIcon.primary.GetSlicedTexture().EncodeToPNG();
			this.secondaryTextureData = playerIcon.secondary.GetSlicedTexture().EncodeToPNG();
		}

		public Sprite ReconstructPrimarySprite()
		{
			Texture2D tex = new Texture2D(1, 1);

			tex.filterMode = FilterMode.Point;
			tex.LoadImage(primaryTextureData);

			return Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), pivot, PIXEL_PER_UNIT);
		}

		public Sprite ReconstructSecondarySprite()
		{
			Texture2D tex = new Texture2D(1, 1);

			tex.filterMode = FilterMode.Point;
			tex.LoadImage(secondaryTextureData);

			return Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), pivot, PIXEL_PER_UNIT);
		}
	}
}
