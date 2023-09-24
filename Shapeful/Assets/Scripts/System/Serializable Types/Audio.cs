using System;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// A custom class contains information about Audio Clips.
/// Used by the Audio Manager.
/// </summary>
[Serializable]
public class Audio
{
	public enum AudioType { Sound, Music }

	public string name;
	public AudioType audioType;

	[Space]
	public AudioClip[] clips;

	[Space]
	public AudioMixerGroup mixerGroup;

	[Space]
	[Range(0f, 1f)]
	public float volume = 1f;

	[Range(-3f, 3f)]
	public float pitch = 1f;

	public bool loop;
	[HideInInspector] public AudioSource source;

	public AudioClip this[int index]
	{
		get { return clips[index]; }
	}

	public int ClipsCount => clips.Length;
}
