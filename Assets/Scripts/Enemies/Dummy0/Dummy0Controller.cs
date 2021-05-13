using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy0Controller : EntityController
{

	//State List
	public EntityState idle;
	public EntityState stun;

	void Start()
	{
		idle = new Dummy0_IdleState(this); states.Add("Idle", idle);
		stun = new Dummy0_StunState(this); states.Add("Stun", stun);

		currentState = idle;
		currentState.StartState();
	}

	public override void Damage(Attack.AttackType type, int damage, float knockback, int weight, Vector2 direction, EntityStatus ownerStatus)
	{
		if (weight > 1)
			SoundMaker.i.PlaySound("Hit2", transform.position, 0.25f);
		else
			SoundMaker.i.PlaySound("Hit", transform.position, 0.2f);
		SetState("Stun");
		stun.PassMotion(knockback, 0.25f, direction);
	}
}
