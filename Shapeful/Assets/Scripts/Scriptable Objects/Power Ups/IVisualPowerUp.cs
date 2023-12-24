using UnityEngine;

public interface IVisualPowerUp
{
	public Color VisualColor { get; }
	public Sprite GetSpriteAtCurrentState();
}
