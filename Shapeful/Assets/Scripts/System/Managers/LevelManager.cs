using CSTGames.DataPersistence;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : PersistentSingleton<LevelManager>
{
	[Header("Reference"), Space]
	[SerializeField] private Animator transitionPanel;

	private void Start()
	{
		transitionPanel.Play("Menu First Loaded");
	}

	public void LoadScene(string sceneName)
	{
		StartCoroutine(LoadSceneCoroutine(sceneName));
	}

	public void ReloadScene(string sceneName)
	{
		SceneManager.LoadSceneAsync(sceneName);
	}

	public void ReloadScene(int buildIndex)
	{
		SceneManager.LoadSceneAsync(buildIndex);
	}

	private IEnumerator LoadSceneCoroutine(string sceneName)
	{
		AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
		op.allowSceneActivation = false;

		transitionPanel.Play("Start");

		yield return new WaitForSecondsRealtime(1f);

		op.allowSceneActivation = true;

		yield return new WaitUntil(() => op.isDone);

		transitionPanel.Play("End");
	}
}
