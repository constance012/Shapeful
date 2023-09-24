using System;
using UnityEngine;

public class AudioManager : PersistentSingleton<AudioManager>
{
	// An array of audios.
	public Audio[] audioArray;

	protected override void Awake()
	{
		base.Awake();

		foreach (var audio in audioArray)
		{
			GameObject audioSourceHolder = new GameObject(audio.name);
			audioSourceHolder.transform.parent = this.transform.Find(audio.audioType.ToString());

			// Add the Audio Source to the Manager's holder for each clip.
			audio.source = audioSourceHolder.AddComponent<AudioSource>();
			audio.source.outputAudioMixerGroup = audio.mixerGroup;
			audio.source.volume = audio.volume;
			audio.source.pitch = audio.pitch;
		}
	}

	private void Start()
	{
		Play("Main Theme");
	}

	/// <summary>
	/// Play the audio with a random clip and default pitch.
	/// </summary>
	/// <param name="audioName"></param>
	public void Play(string audioName)
	{
		if (!TryGetAudio(out Audio chosenAudio, audioName))
		{
			Debug.LogWarning($"Audio Clip: {audioName} could not be found!!");
			return;
		}

		chosenAudio.source.clip = GetRandomClip(chosenAudio);

		chosenAudio.source.Play();
	}

	/// <summary>
	/// Play the audio with the specified clip and pitch.
	/// </summary>
	/// <param name="audioName"></param>
	/// <param name="clipIndex"></param>
	/// <param name="pitch"></param>
	public void Play(string audioName, int clipIndex, float pitch)
	{
		if (!TryGetAudio(out Audio chosenAudio, audioName))
		{
			Debug.LogWarning($"Audio Clip: {audioName} could not be found!!");
			return;
		}

		chosenAudio.source.clip = chosenAudio[clipIndex];
		chosenAudio.source.pitch = pitch;

		chosenAudio.source.Play();
	}

	/// <summary>
	/// Play the audio with random clip and pitch;
	/// </summary>
	/// <param name="audioName"></param>
	/// <param name="min"></param>
	/// <param name="max"></param>
	public void PlayWithRandomPitch(string audioName, float min, float max)
	{
		if (!TryGetAudio(out Audio chosenAudio, audioName))
		{
			Debug.LogWarning($"Audio Clip: {audioName} could not be found!!");
			return;
		}

		chosenAudio.source.clip = GetRandomClip(chosenAudio);
		chosenAudio.source.pitch = UnityEngine.Random.Range(min, max);

		chosenAudio.source.Play();
	}

	public void SetVolume(string audioName, float newVolume, bool resetToDefault = false)
	{
		Audio chosenAudio = GetAudio(audioName);

		if (!resetToDefault)
			chosenAudio.source.volume = newVolume;
		else
			chosenAudio.source.volume = chosenAudio.volume;
	}

	public bool TryGetAudio(out Audio chosenAudio, string audioName)
	{
		chosenAudio = GetAudio(audioName);

		if (chosenAudio == null)
			return false;

		return true;
	}

	public Audio GetAudio(string audioName)
	{
		audioName = audioName.ToLower().Trim();
		return Array.Find(audioArray, sound => sound.name.ToLower().Equals(audioName));
	}

	private AudioClip GetRandomClip(Audio target)
	{
		int index = UnityEngine.Random.Range(0, target.clips.Length);
		return target[index];
	}
}
