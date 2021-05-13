using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WraithIdleState : EntityState
{
	float restTime;

	public WraithIdleState(EntityController controller) : base(controller) { stateName = "Idle"; }

	public override string StartState()
	{
		myController.characterAnimator.Play("Idle");
		myController.entityStatus.currentHitstate = EntityStatus.HitState.normal;

		restTime = Random.Range(0.75f, 1.5f);
		if (myController.entityInput.target != null && myController.entityInput.GetPointerDistance() < 1.5)
			restTime -= 0.6f;
		return base.StartState();
	}

	public override void MainUpdate()
	{
		if (restTime > 0)
			restTime -= Time.deltaTime;
		else if (restTime <= 0)
		{
			float randomVal = Random.Range(0, 100);
			if (myController.entityInput.target != null)
			{
				if (randomVal > 25 && myController.entityInput.GetPointerDistance() < 1.2 && myController.GetState("Attack").Cooldown() <= 0)
					myController.SetState("Attack");
				else
					myController.SetState("Move");
			}
			else
				myController.SetState("Idle");
			return;
		}
	}

	public override string EndState()
	{
		return base.EndState();
	}
}
