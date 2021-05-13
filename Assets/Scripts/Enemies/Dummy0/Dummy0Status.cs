using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy0Status : EntityStatus
{
	public override bool Damage(Attack.AttackType type, int damage, float knockback, int weight, Vector2 direction, EntityStatus ownerStatus)
	{
		entityController.Damage(type, damage, knockback, weight, direction, ownerStatus);
		EffectSpawner.instance.PopDamage(transform.position + new Vector3(0,1f,-5f), damage);
		return true;
	}
}
