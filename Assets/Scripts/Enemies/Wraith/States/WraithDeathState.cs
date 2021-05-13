using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WraithDeathState : EntityState
{
	float deathTime = .5f;
	float deathTimer;
	public WraithDeathState(EntityController controller) : base(controller) { stateName = "Death"; }

	public override string StartState()
	{
		deathTimer = deathTime;
		myController.characterAnimator.Play("Stun");
		myController.entityStatus.currentHitstate = EntityStatus.HitState.i_frame;
		return base.StartState();
	}

	public override void MainUpdate()
	{
		if (deathTimer > 0)
			deathTimer -= Time.deltaTime;
		else if (deathTimer <= 0)
		{
			myController.DestroySelf();
		}

		float d = deathTimer / deathTime;
		myController.characterSprite.color = new Color(d, d, d, d);
	}

	public override string EndState()
	{
		return base.EndState();
	}
}
