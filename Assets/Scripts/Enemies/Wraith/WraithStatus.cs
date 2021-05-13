using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WraithStatus : EntityStatus
{
	public override bool Damage(Attack.AttackType type, int damage, float knockback, int weight, Vector2 direction, EntityStatus ownerStatus)
	{
		if (currentHitstate != HitState.i_frame)
		{
			health -= damage;
			EffectSpawner.instance.PopDamage(transform.position + new Vector3(0, 1f, -5f), damage);
			entityController.Damage(type, damage, knockback, weight, direction, ownerStatus);
			if (health <= 0)
			{
				Die();
				SoundMaker.i.PlaySound("WraithCry", transform.position, 0.1f);
			}
				
		}
		return (currentHitstate == HitState.normal || currentHitstate == HitState.superarmor);
	}
}
