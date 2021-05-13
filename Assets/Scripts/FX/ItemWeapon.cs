using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWeapon : MonoBehaviour, IDroppedWeapon
{
	[SerializeField] Weapon weapon = null;
	public bool Pickable()
	{
		return true;
	}

	public Weapon GetWeapon()
	{
		return weapon;
	}

	public void DestroySelf()
	{
		Destroy(gameObject);
	}

	public void SetSpriteMat(string materialAddress)
	{
		SpriteRenderer image = GetComponent<SpriteRenderer>();
		if (materialAddress == "default")
			image.material = Resources.Load("Materials/BaseSprite") as Material;
		else
			image.material = Resources.Load("Materials/" + materialAddress) as Material;
	}
}
