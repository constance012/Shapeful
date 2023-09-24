using UnityEngine;
using System;

namespace CSTGames.DataPersistence
{
	[Serializable]
	public class GameData
	{
		public int highscore;
		public Color playerColor;

		/// <summary>
		/// Initialize with default values of the data.
		/// </summary>
		public GameData()
		{
			this.highscore = 0;
			this.playerColor = Color.white;
		}
	}
}
