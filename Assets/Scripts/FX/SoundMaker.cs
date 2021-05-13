using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMaker : MonoBehaviour
{
	public static SoundMaker i;
	private void Start()
	{
		i = this;
	}

	List<GameObject> soundPool = new List<GameObject>();
	public int amountToPool = 16;

	public void PlaySound(string keyName)
	{
		GameObject soundObject = GetSoundObject();
		AudioSource audioSource = soundObject.GetComponent<AudioSource>();
		soundObject.SetActive(true);
		audioSource.PlayOneShot(GetAudioClip(keyName));
		DestroySoundObject(soundObject, audioSource.clip.length);
	}

	public void PlaySound(string keyName, Vector3 position)
	{
		GameObject soundObject = GetSoundObject();
		AudioSource audioSource = soundObject.GetComponent<AudioSource>();
		soundObject.SetActive(true);
		soundObject.transform.position = position;
		audioSource.clip = GetAudioClip(keyName);
		audioSource.spatialBlend = 1;
		audioSource.maxDistance = 32;
		audioSource.Play();
		DestroySoundObject(soundObject, audioSource.clip.length);
	}

	public void PlaySound(string keyName, float audioLevel)
	{
		GameObject soundObject = GetSoundObject();
		AudioSource audioSource = soundObject.GetComponent<AudioSource>();
		soundObject.SetActive(true);
		audioSource.volume = audioLevel;
		audioSource.spatialBlend = 1;
		audioSource.maxDistance = 32;
		audioSource.PlayOneShot(GetAudioClip(keyName));
		DestroySoundObject(soundObject, audioSource.clip.length);
	}

	public void PlaySound(string keyName, Vector3 position, float audioLevel)
	{
		GameObject soundObject = GetSoundObject();
		AudioSource audioSource = soundObject.GetComponent<AudioSource>();
		soundObject.SetActive(true);
		soundObject.transform.position = position;
		audioSource.clip = GetAudioClip(keyName);
		audioSource.spatialBlend = 1;
		audioSource.maxDistance = 32;
		audioSource.rolloffMode = AudioRolloffMode.Linear;
		audioSource.volume = audioLevel;
		audioSource.Play();
		DestroySoundObject(soundObject, audioSource.clip.length);
	}
	public void PlaySound(string keyName, Vector3 position, float audioLevel, float distance)
	{
		GameObject soundObject = GetSoundObject();
		AudioSource audioSource = soundObject.GetComponent<AudioSource>();
		soundObject.SetActive(true);
		soundObject.transform.position = position;
		audioSource.clip = GetAudioClip(keyName);
		audioSource.spatialBlend = 1;
		audioSource.maxDistance = distance;
		audioSource.rolloffMode = AudioRolloffMode.Linear;
		audioSource.volume = audioLevel;
		audioSource.Play();
		DestroySoundObject(soundObject, audioSource.clip.length);
	}

	private AudioClip GetAudioClip(string keyName)
	{
		for (int i = 0; i < GameAssets.i.soundClips.Length; i++)
		{
			if (GameAssets.i.soundClips[i].key == keyName)
				return GameAssets.i.soundClips[i].soundClip;
		}
		Debug.LogError("No Sound Clip Found: " + keyName);
		return null;
	}

	private GameObject GetSoundObject()
	{
		if (soundPool.Count != 0)
		{
			for (int i = 0; i < soundPool.Count; i++)
			{
				if (!soundPool[i].activeInHierarchy)
					return soundPool[i];
			}
		}
		GameObject soundObject = new GameObject("Sound");
		soundObject.AddComponent<AudioSource>();
		soundPool.Add(soundObject);
		return soundObject;
	}

	private void DestroySoundObject(GameObject soundObj, float clipLength)
	{
		int index = soundPool.IndexOf(soundObj);
		if (soundPool.Count > amountToPool)
		{
			GameObject sendOff = soundPool[index];
			soundPool.RemoveAt(index);
			Destroy(sendOff);
		}
		StartCoroutine(Deactivate(soundObj, clipLength));
	}

	IEnumerator Deactivate(GameObject soundObj, float clipLength)
	{
		yield return new WaitForSeconds(clipLength);
		if (soundObj != null)
			soundObj.SetActive(false);
	}
}
