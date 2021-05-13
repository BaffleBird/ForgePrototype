using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePop : MonoBehaviour
{
	TextMeshPro textMesh;
	float speed = 0f;
	float drift = 0f;
	float maxTime = 0.5f;
	float timer = 0f;

    void Awake()
    {
		textMesh = GetComponent<TextMeshPro>();
		speed = Random.Range(4f, 8f);
		timer = maxTime;
		drift = Random.Range(-1f, 1f);
		drift += Mathf.Sign(drift) * 0.1f;
	}

    // Update is called once per frame
    public void Setup(int damage)
    {
		textMesh.text = damage.ToString();
    }

	private void Update()
	{
		Vector3 move = new Vector3(drift, speed, 0);
		if (timer <= 0)
			Destroy(gameObject);
		transform.position += move * Time.deltaTime;

		timer -= Time.deltaTime;
		speed *= .75f;
		drift *= .75f;
	}
}
