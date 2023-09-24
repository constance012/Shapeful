namespace CSTGames.DataPersistence
{
	public interface ISaveDataTransceiver
	{
		public void LoadData(GameData data);
		public void SaveData(GameData data);
	}
}
