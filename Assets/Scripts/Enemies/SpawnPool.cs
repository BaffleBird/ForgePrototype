using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPool : MonoBehaviour
{
	[SerializeField] float width = 6f;
	[SerializeField] float height = 5f;
	[SerializeField] float triggerRange = 5f;
	[SerializeField] float spawnRate = 0.45f;
	float spawnCounter = 0f;
	int enemyCount = 0;
	int waveCount = 0;
	[SerializeField] int waveMin = 1;
	[SerializeField] int waveMax = 5;

	public GameObject spawnBlob;
	public GameObject wraithPrefab;
	public GameObject devourerPrefab;

	private void Start()
	{
		spawnCounter = spawnRate;
	}

	private void Update()
	{
		if (spawnCounter > 0)
			spawnCounter -= Time.deltaTime;

		enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
		Vector3 playerPos = GameObject.FindWithTag("Player").transform.position;
		float dist = Vector3.Distance(transform.position, playerPos);

		if (enemyCount <= 0 && spawnCounter <= 0 && waveCount <= 0 && dist < triggerRange)
		{
			waveCount = Random.Range(waveMin, waveMax);
			spawnCounter = spawnRate * 2;
		}

		if (waveCount > 0 && spawnCounter <= 0)
		{
			
			SpawnEnemy(wraithPrefab);
			int r = Random.Range(1, 100);
			if (r > 70)
				SpawnEnemy(devourerPrefab);
			spawnCounter = spawnRate;
			waveCount--;
		}
	}

	void SpawnEnemy(GameObject enemy)
	{
		Vector3 pos = transform.position + new Vector3(Random.Range(-width, width), Random.Range(-height, height), 0);
		pos.z = pos.y;
		GameObject newSpawn = Instantiate(spawnBlob, pos, spawnBlob.transform.rotation);
		SpawnBlob blob = newSpawn.GetComponent<SpawnBlob>();
		blob.spawnPrefab = enemy;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, triggerRange);
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, new Vector3(width * 2, height * 2, height * 2));
	}
}
