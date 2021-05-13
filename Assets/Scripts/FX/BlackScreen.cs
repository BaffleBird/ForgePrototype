using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BlackScreen : MonoBehaviour
{
	public PlayerControl playerController;
	Image overlay;

	bool fade = false;
	float fadeCounterStart = 4;
	float fadeCounter = 0;

	private void OnEnable()
	{
		overlay = GetComponent<Image>();

		playerController.OnPlayerDeath += FadeScreenToBlack;
	}

	private void OnDisable()
	{
		playerController.OnPlayerDeath -= FadeScreenToBlack;
	}

	private void Update()
	{
		if (fade)
		{
			fadeCounter -= Time.deltaTime;
			overlay.color = new Color(0, 0, 0, 1 - (fadeCounter / fadeCounterStart));
			if(fadeCounter <= 0)
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
		}
	}

	void FadeScreenToBlack()
	{
		fadeCounter = fadeCounterStart;
		fade = true;
	}
}
