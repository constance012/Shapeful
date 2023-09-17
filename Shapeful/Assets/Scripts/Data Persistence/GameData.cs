using System;

namespace CSTGames.DataPersistence
{
	[Serializable]
	public struct GameData
	{
		public int highscore;

		public GameData(int highscore)
		{
			this.highscore = highscore;
		}
	}
}
