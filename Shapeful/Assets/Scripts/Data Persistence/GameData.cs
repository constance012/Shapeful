using UnityEngine;
using System;

namespace CSTGames.DataPersistence
{
	[Serializable]
	public class GameData
	{
		public long lastUpdated;

		public uint continueAttempts;
		
		/// <summary>
		/// Stores the remaining cooldown time of the next continue attempt (totalHours, minutes, seconds). 
		/// </summary>
		private Vector3Int continueAttemptRemainingCD;
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
		public Color primaryColor;
		public Color secondaryColor;

		/// <summary>
		/// Initialize with default values of the data.
		/// </summary>
		public GameData()
		{
			this.lastUpdated = DateTime.Now.ToBinary();

			this.continueAttempts = GameManager.MAX_CONTINUE_ATTEMPT;
			this.continueAttemptRemainingCD = Vector3Int.zero;

			this.highscore = 0;
			this.primaryColor = Color.white;
			this.secondaryColor = Color.black;
		}
	}
}
