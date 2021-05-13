using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevourerDeathState : EntityState
{
	float deathTime = .75f;
	float deathTimer;
	public DevourerDeathState(EntityController controller) : base(controller) { stateName = "Death"; }

	public override string StartState()
	{
		deathTimer = deathTime;
		myController.characterAnimator.Play("Stun");
		myController.entityStatus.currentHitstate = EntityStatus.HitState.i_frame;

		SoundMaker.i.PlaySound("Alien1", myController.transform.position, 0.25f);

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
