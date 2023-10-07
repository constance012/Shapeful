using UnityEngine;
using System;

namespace CSTGames.DataPersistence
{
	[Serializable]
	public class GameData
	{
		public long lastUpdated;

		public const uint MAX_CONTINUE_ATTEMPT = 3;
		public uint continueAttempts;
		
		/// <summary>
		/// Stores the remaining cooldown time of the next continue attempt (totalHours, minutes, seconds). 
		/// </summary>
		private Vector3Int continueAttemptRemainingCD = Vector3Int.zero;
		public Vector3Int ContinueAttemptRemainingCD
		{
			get { return continueAttemptRemainingCD; }
			set
			{
				value.y += value.z / 60;
				value.z -= 60 * (value.z / 60);

				value.x += value.y / 60;
				value.y -= 60 * (value.y / 60);

				continueAttemptRemainingCD = value;
			}
		}

		public int highscore;
		public Color playerColor;

		/// <summary>
		/// Initialize with default values of the data.
		/// </summary>
		public GameData()
		{
			this.lastUpdated = DateTime.Now.ToBinary();
			this.continueAttempts = 2;

			this.highscore = 0;
			this.playerColor = Color.white;
		}
	}
}
