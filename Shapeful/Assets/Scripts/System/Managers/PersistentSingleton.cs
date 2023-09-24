using UnityEngine;

/// <summary>
/// Makes a permanent singleton reference for the entire game session, which persists between different scenes.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
	protected override void Awake()
	{
		SetPersistentSingleton();
	}

	protected void SetPersistentSingleton()
	{
		if (_hasSingleton)
			return;

		if (Instance == null)
		{
			Instance = this as T;
			DontDestroyOnLoad(this.gameObject);
			_hasSingleton = true;
		}
		else
		{
			string typeName = typeof(T).Name;
			Debug.LogWarning($"More than one Instance of {typeName} found!! Destroy the newest one.");

			Destroy(this.gameObject);

			return;
		}
	}
}
