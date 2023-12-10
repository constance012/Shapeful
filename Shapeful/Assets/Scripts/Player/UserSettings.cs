using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

/// <summary>
/// A static wrapper class for easily manipulating PlayerPref keys.
/// </summary>
public static class UserSettings
{
	public enum SettingSection { Audio, Graphics, Controls, All }
	
	#region Audio Settings
	public static float MasterVolume
	{
		get { return PlayerPrefs.GetFloat("MasterVolume", 0f); }
		set { PlayerPrefs.SetFloat("MasterVolume", value); }
	}

	public static float MusicVolume
	{
		get { return PlayerPrefs.GetFloat("MusicVolume", 0f); }
		set { PlayerPrefs.SetFloat("MusicVolume", value); }
	}

	public static float SoundsVolume
	{
		get { return PlayerPrefs.GetFloat("SoundsVolume", 0f); }
		set { PlayerPrefs.SetFloat("SoundsVolume", value); }
	}
	#endregion


	#region Graphics Settings
	public static int QualityLevel
	{
		get { return PlayerPrefs.GetInt("QualityLevel", 1); }
		set { PlayerPrefs.SetInt("QualityLevel", value); }
	}

	public static int ResolutionIndex
	{
		get { return PlayerPrefs.GetInt("ResolutionIndex", 7); }
		set { PlayerPrefs.SetInt("ResolutionIndex", value); }
	}

	public static bool IsFullscreen
	{
		get { return PlayerPrefs.GetInt("IsFullscreen", 0) == 1; }
		set { PlayerPrefs.SetInt("IsFullscreen", value ? 1 : 0); }
	}

	public static float TargetFramerate
	{
		get { return PlayerPrefs.GetFloat("TargetFramerate", 120f); }
		set { PlayerPrefs.SetFloat("TargetFramerate", value); }
	}

	public static bool UseVsync
	{
		get { return PlayerPrefs.GetInt("UseVsync", 0) == 1; }
		set { PlayerPrefs.SetInt("UseVsync", value ? 1 : 0); }
	}

	public static bool EnableBackgroundParticles
	{
		get { return PlayerPrefs.GetInt("EnableBackgroundParticles", 1) == 1; }
		set { PlayerPrefs.SetInt("EnableBackgroundParticles", value ? 1 : 0); }
	}

	public static bool SecondaryColorSameAsPrimary
	{
		get { return PlayerPrefs.GetInt("SecondaryColorSameAsPrimary", 0) == 1; }
		set { PlayerPrefs.SetInt("SecondaryColorSameAsPrimary", value ? 1 : 0); }
	}
	#endregion

	#region Controls Settings
	public static string SelectedKeyset
	{
		get { return PlayerPrefs.GetString("SelectedKeyset", "Default"); }
		set { PlayerPrefs.SetString("SelectedKeyset", value); }
	}
	#endregion

	/// <summary>
	/// Resets all the settings in the specified section to their default values.
	/// </summary>
	/// <param name="section"></param>
	public static void ResetToDefault(SettingSection section)
	{
		switch (section)
		{
			case SettingSection.Audio:
				MasterVolume = 0f;
				MusicVolume = 0f;
				SoundsVolume = 0f;
				break;

			case SettingSection.Graphics:
				QualityLevel = 1;
				ResolutionIndex = 7;
				IsFullscreen = false;
				TargetFramerate = 120f;
				UseVsync = false;
				EnableBackgroundParticles = true;
				SecondaryColorSameAsPrimary = false;
				break;

			case SettingSection.Controls:
				SelectedKeyset = "Default";
				break;

			case SettingSection.All:
				MasterVolume = 0f;
				MusicVolume = 0f;
				SoundsVolume = 0f;

				QualityLevel = 1;
				ResolutionIndex = 7;
				IsFullscreen = false;
				TargetFramerate = 120f;
				UseVsync = false;
				EnableBackgroundParticles = true;
				SecondaryColorSameAsPrimary = false;

				SelectedKeyset = "Default";
				break;
		}
	}

	/// <summary>
	///	Deletes the specified key from the player's preferences by name.
	///	<para />
	///	WARNING: The method causes <b> irreversible changes </b>, use with your own risk.
	///	</summary>
	///	<param name="keyName">The name of the key to be deleted.</param>
	///
	public static void DeleteKey(string keyName)
	{
		PlayerPrefs.DeleteKey(keyName);
	}

	/// <summary>
	///	Deletes all keys and values from the player's preferences. Use with caution.
	///	<para />
	///	<b>WARNING: The method causes <b> irreversible changes </b>, use with your own risk.</b>
	/// </summary>
	public static void DeleteAllKeys()
	{
		PlayerPrefs.DeleteAll();
	}
}