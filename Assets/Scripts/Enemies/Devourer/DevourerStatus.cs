using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevourerStatus : EntityStatus
{
	public override bool Damage(Attack.AttackType type, int damage, float knockback, int weight, Vector2 direction, EntityStatus ownerStatus)
	{
		if (currentHitstate != HitState.i_frame)
		{
			health -= damage;
			EffectSpawner.instance.PopDamage(transform.position + new Vector3(0, 1f, -5f), damage);
			if (health <= 0)
				Die();
			else
				entityController.Damage(type, damage, knockback, weight, direction, ownerStatus);
		}
		return (currentHitstate == HitState.normal || currentHitstate == HitState.superarmor);
	}
}
