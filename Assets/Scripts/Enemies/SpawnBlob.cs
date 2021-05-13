using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlob : MonoBehaviour
{
	public GameObject spawnPrefab = null;
	[SerializeField] AnimationAlert animationAlert = null;

	private void Start()
	{
		SoundMaker.i.PlaySound("Poof", transform.position, 0.6f);
	}

	// Update is called once per frame
	void Update()
    {
        if (animationAlert.flag[1])
		{
			GameObject spawn = Instantiate(spawnPrefab, transform.position, transform.rotation);
			animationAlert.flag[1] = false;
			
		}

		if (animationAlert.flag[0])
			Destroy(gameObject);
    }
}
