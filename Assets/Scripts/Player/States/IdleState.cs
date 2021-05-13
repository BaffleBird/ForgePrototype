using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerState
{
	public IdleState(PlayerControl playerController) : base(playerController) { stateName = "Idle"; }

	public override string StartState()
	{
		playerControl.PlayAnimation("Idle");
		playerControl.shadowAnimator.SetInteger("Shadow Size", 1);
		playerControl.playerStats.currentHitstate = EntityStatus.HitState.normal;
		return stateName;
	}

	public override void MainUpdate()
	{
		//Get Inputs
		Vector3 movement = playerControl.entityInput.movementInput;

		//Move State Transition
		if (movement.x != 0 || movement.y != 0)
		{
			playerControl.SetState("Move");
			return;
		}

		//Dodge State Transition
		if (playerControl.entityInput.aDown && playerControl.GetState("Dodge").Cooldown() <= 0)
		{
			playerControl.SetState("Dodge");
			return;
		}

		//Attack Transition
		if (playerControl.entityInput.xDown)
		{
			if (playerControl.playerStats.GetNearestParry() != null)
				playerControl.SetState("Parry Strike");
			else
				playerControl.SetState("Strong Attack");
			return;
		}

		//Defense Transition
		if (playerControl.entityInput.bDown || playerControl.entityInput.bHold)
		{
			playerControl.SetState("Defense");
			return;
		}

		//Throw Weapon
		if (playerControl.entityInput.yDown)
		{
			playerControl.Throw_Pickup_Weapon();
		}
	}

	public override string EndState()
	{
		return stateName;
	}
}
