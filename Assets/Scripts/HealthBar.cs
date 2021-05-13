using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	public Slider healthSlider;
	public EntityStatus status;

	private void OnEnable()
	{
		status.HealthUpdate += SetHealth;
	}

	private void OnDisable()
	{
		status.HealthUpdate -= SetHealth;
	}

	public void Start()
	{
		SetMaxHealth(status.maxHealth);
		SetHealth(status.health);
	}

	public void SetMaxHealth(float maxHP)
	{
		healthSlider.maxValue = maxHP;
	}

	public void SetHealth(float hp)
	{
		healthSlider.value = hp;
	}
}
