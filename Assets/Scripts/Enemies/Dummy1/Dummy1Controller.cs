using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy1Controller : EntityController
{
	EntityState idle, punch, stun, parried;

	void Start()
	{
		idle = new Dummy1_IdleState(this); states.Add("Idle", idle);
		punch = new Dummy1_PunchState(this); states.Add("Punch", punch);
		stun = new Dummy1_StunState(this); states.Add("Stun", stun);
		parried = new Dummy1_ParriedState(this); states.Add("Parried", parried);

		currentState = idle;
		currentState.StartState();
	}

	protected override void Update()
	{
		base.Update();
		if (flashTime > 0)
			IncrementFlash();
	}

	public override void Damage(Attack.AttackType type, int damage, float knockback, int weight, Vector2 direction, EntityStatus ownerStatus)
	{
		Vector3 pos = transform.position + new Vector3(0, 1f, -1f);
		Flash(0.8f, 0.8f, Color.white);
		EffectSpawner.instance.SpawnHitEffect(pos, 4, Color.white);
		if (entityStatus.currentHitstate == EntityStatus.HitState.normal)
		{
			if (previousState == "Parried")
				SoundMaker.i.PlaySound("Stab", transform.position, 0.25f);
			else
			{
				if (weight > 1)
					SoundMaker.i.PlaySound("Hit2", transform.position, 0.25f);
				else
					SoundMaker.i.PlaySound("Hit", transform.position, 0.2f);
			}
				
			stun.PassDamage(0f, knockback, weight, direction);
			SetState("Stun");
		}
	}

	public override void Parry()
	{
		SetState("Parried");
	}
}
