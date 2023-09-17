using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public void StartGame()
	{
		SceneManager.LoadSceneAsync("Scenes/Game Scene");
	}

	public void QuitGame()
	{
		Debug.Log("Quiting player...");
		Application.Quit();
	}
}
