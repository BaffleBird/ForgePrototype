using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
	public static GameAssets i;
	private void Start()
	{
		i = this;
	}

	[Header("Audio Clips")]
	public SoundClip[] soundClips;

	[System.Serializable]
	public class SoundClip
	{
		public string key;
		public AudioClip soundClip;
	}
}
