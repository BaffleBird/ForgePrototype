using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WraithController : EntityController
{
	EntityState idle, move, attack, stun, parried, death;

	void Start()
	{
		idle = new WraithIdleState(this); states.Add("Idle", idle);
		move = new WraithMoveState(this); states.Add("Move", move);
		attack = new WraithAttackState(this); states.Add("Attack", attack);
		stun = new WraithStunState(this); states.Add("Stun", stun);
		parried = new WraithParryState(this); states.Add("Parried", parried);
		death = new WraithDeathState(this); states.Add("Death", death);

		currentState = idle;
		currentState.StartState();
	}

	protected override void Update()
	{
		base.Update();
		if (flashTime > 0)
			IncrementFlash();
		//Debug.Log(GetStateName());
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
			if (previousState == "Parried")
				SoundMaker.i.PlaySound("Stab", transform.position, 0.25f);
			else
			{
				if (weight > 1)
					SoundMaker.i.PlaySound("Hit2", transform.position, 0.25f);
				else
					SoundMaker.i.PlaySound("Hit", transform.position, 0.2f);
			}
			stun.PassMotion(knockback, 0.25f, direction);
			SetState("Stun");
		}
	}

	public override void Parry()
	{
		SetState("Parried");
	}

	public override void Die()
	{
		SetState("Death");
	}
}
