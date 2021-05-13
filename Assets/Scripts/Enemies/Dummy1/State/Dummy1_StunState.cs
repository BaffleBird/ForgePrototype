using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy1_StunState : EntityState
{
	float stunCount = 0;

	public Dummy1_StunState(EntityController controller) : base(controller) { stateName = "Stun"; }

	public override string StartState()
	{
		stunCount++;
		myController.animationAlert.ResetAlerts();
		myController.characterAnimator.Play("Stun");
		myController.entityStatus.currentHitstate = EntityStatus.HitState.normal;
		return base.StartState();
	}

	public override void MainUpdate()
	{
		if (stunCount > 4)
		{
			myController.entityStatus.currentHitstate = EntityStatus.HitState.superarmor;
		}

		if (myController.animationAlert.flag[0])
		{
			stunCount = 0;
			myController.SetState("Idle");
			return;
		}
	}

	public override string EndState()
	{
		return base.EndState();
	}
}
