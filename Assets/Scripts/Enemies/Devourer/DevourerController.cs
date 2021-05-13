using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevourerController : EntityController
{
	EntityState idle, move, attack, stun, death;

	void Start()
	{
		idle = new DevourerIdleState(this); states.Add("Idle", idle);
		move = new DevourerMoveState(this); states.Add("Move", move);
		attack = new DevourerAttackState(this); states.Add("Attack", attack);
		stun = new DevourerStunState(this); states.Add("Stun", stun);
		death = new DevourerDeathState(this); states.Add("Death", death);

		currentState = idle;
		currentState.StartState();
	}

	protected override void Update()
	{
		base.Update();
		if (flashTime > 0)
			IncrementFlash();
	}

	private void LateUpdate()
	{
		if (entityInput.target != null)
		{
			Vector3 flipScale = characterSprite.transform.localScale;
			if (entityInput.GetPointerDirection().x > 0)
				flipScale.x = Mathf.Abs(flipScale.x);
			else
				flipScale.x = Mathf.Abs(flipScale.x) * -1;
			characterSprite.transform.localScale = flipScale;
		}
	}

	public override void Damage(Attack.AttackType type, int damage, float knockback, int weight, Vector2 direction, EntityStatus ownerStatus)
	{
		Vector3 pos = transform.position + new Vector3(0, 1f, -1f);
		Flash(0.8f, 0.8f, Color.white);
		EffectSpawner.instance.SpawnHitEffect(pos, 4, Color.white);
		if (entityStatus.currentHitstate == EntityStatus.HitState.normal)
		{
			if (weight > 1)
				SoundMaker.i.PlaySound("Hit2", transform.position, 0.25f);
			else
				SoundMaker.i.PlaySound("Hit", transform.position, 0.2f);
			stun.PassMotion(knockback, 0.2f, direction);
			SetState("Stun");
		}
	}

	public override void Die()
	{
		SetState("Death");
	}
}
