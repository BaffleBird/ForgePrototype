using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy1_IdleState : EntityState
{
	float restTime;

	public Dummy1_IdleState(EntityController controller) : base(controller) { stateName = "Idle"; }

	public override string StartState()
	{
		restTime = 1.5f;
		myController.characterAnimator.Play("Idle");
		myController.entityStatus.currentHitstate = EntityStatus.HitState.superarmor;
		return base.StartState();
	}

	public override void MainUpdate()
	{
		if (restTime > 0)
			restTime -= Time.deltaTime;
		else if (restTime <= 0)
		{
			myController.SetState("Punch");
			return;
		}
	}

	public override string EndState()
	{
		return base.EndState();
	}
}
