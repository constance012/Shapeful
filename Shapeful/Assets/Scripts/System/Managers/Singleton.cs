using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T Instance { get; protected set; }

	private bool _hasSingleton;

	protected virtual void Awake()
	{
		SetSingleton();
	}

	/// <summary>
	/// This method is automatically called in awake. You can manually call this method to set Instance forcefully.
	/// </summary>
	protected void SetSingleton()
	{
		if (_hasSingleton)
			return;

		if (Instance == null)
		{
			Instance = this as T;
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
