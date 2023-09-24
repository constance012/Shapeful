using UnityEngine;

public static class DeviceVibration
{
	#if UNITY_ANDROID && !UNITY_EDITOR
	public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
	public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
	public static AndroidJavaObject vibrateMotor = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
	
	#else
	public static AndroidJavaClass unityPlayer;
	public static AndroidJavaObject currentActivity;
	public static AndroidJavaObject vibrateMotor;
	#endif

	/// <summary>
	/// Vibrates the device with the default duration of 400ms.
	/// </summary>
	public static void Vibrate()
	{
		if (IsAndroid)
		{
			vibrateMotor.Call("vibrate");
		}
		else
		{
			Handheld.Vibrate();
		}
	}

	/// <summary>
	/// Vibrates the device with the duration of <c> milliseconds </c>
	/// </summary>
	/// <param name="milliseconds"> The vibrating duration in millisecond. </param>
	public static void Vibrate(long milliseconds)
	{
		if (IsAndroid)
		{
			vibrateMotor.Call("vibrate", milliseconds);
		}
		else
		{
			Handheld.Vibrate();
		}
	}

	/// <summary>
	/// Vibrates the device with the specified pattern of milliseconds array.
	/// <para />
	/// If enable repeat, the vibration pattern will be loop until <c> cancel </c> is called.
	/// </summary>
	/// <param name="pattern"> An array of numbers represent time in milliseconds. </param>
	/// <param name="repeat"> A boolean where the vibration should repeat. </param>
	public static void Vibrate(long[] pattern, bool repeat)
	{
		if (IsAndroid)
		{
			vibrateMotor.Call("vibrate", pattern, repeat);
		}
		else
		{
			Handheld.Vibrate();
		}
	}

	/// <summary>
	/// Call this to stop vibrating after having invoked <c> vibrate() </c> with repetition enabled.
	/// </summary>
	public static void Cancel()
	{
		if (IsAndroid)
		{
			vibrateMotor.Call("cancel");
		}
	}

	public static bool IsAndroid
	{
		get
		{
		#if UNITY_ANDROID && !UNITY_EDITOR
			return true;
		#else
			return false;
		#endif
		}
	}
}
