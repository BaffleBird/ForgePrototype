using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy1Status : EntityStatus
{
	public override bool Damage(Attack.AttackType type, int damage, float knockback, int weight, Vector2 direction, EntityStatus ownerStatus)
	{
		SoundMaker.i.PlaySound("Hit", entityController.transform.position, 0.25f);
		EffectSpawner.instance.PopDamage(transform.position + new Vector3(0, 1f, -5f), damage);
		entityController.Damage(type, damage, knockback, weight, direction, ownerStatus);
		return false;
	}
}
