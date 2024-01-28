namespace CSTGames.DataPersistence
{
	public interface ISaveDataTransceiver
	{
		public bool Ready { get; }
		public void LoadData(GameData data);
		public void SaveData(GameData data);
	}
}
