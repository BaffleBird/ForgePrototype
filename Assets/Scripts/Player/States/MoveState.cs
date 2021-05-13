using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : PlayerState
{
	public float maxSpeed = 5f;
	public float acceleration = 1f;

	float moveSpeed = 0f;
	Vector3 movement;

	public MoveState(PlayerControl playerController) : base(playerController) { stateName = "Move"; }

	public override string StartState()
	{
		playerControl.PlayAnimation("Move");
		playerControl.shadowAnimator.SetInteger("Shadow Size", 1);
		playerControl.playerStats.currentHitstate = EntityStatus.HitState.normal;
		return stateName;
	}

	public override void MainUpdate()
	{
		movement = playerControl.entityInput.movementInput;

		if (playerControl.animationAlert.flag[4])
		{
			SoundMaker.i.PlaySound("FootStep", playerControl.transform.position, 0.1f);
			playerControl.animationAlert.flag[4] = false;
		}

		if (playerControl.entityInput.xDown)
		{
			if (playerControl.playerStats.GetNearestParry() != null)
				playerControl.SetState("Parry Strike");
			else
				playerControl.SetState("Strong Attack");
			return;
		}

		if (playerControl.entityInput.aDown && playerControl.GetState("Dodge").Cooldown() <= 0)
		{
			playerControl.SetState("Dodge");
			return;
		}

		if (movement.x != 0 || movement.y != 0)
		{
			moveSpeed = moveSpeed < maxSpeed ? moveSpeed + acceleration : maxSpeed;
			playerControl.characterAnimator.SetFloat("Horizontal", movement.x);
			playerControl.characterAnimator.SetFloat("Vertical", movement.y);
		}
		else
		{
			playerControl.SetState("Idle");
			return;
		}

		//Defense Transition
		if (playerControl.entityInput.bHold || playerControl.entityInput.bDown)
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

	public override Vector3 MotionUpdate()
	{
		movement = movement.normalized;
		movement.z = movement.y;
		return (movement * moveSpeed);
	}

	public override string EndState()
	{
		moveSpeed = 0;
		return stateName;
	}

	public void adjustMove(float speed)
	{
		moveSpeed = speed;
	}
}
