using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintState : PlayerState
{
	public float sprintSpeed = 7f;
	Vector3 movement;
	Vector3 lastMove;

	public SprintState(PlayerControl playerController) : base(playerController)
	{
		lastMove = Vector3.right;
		stateName = "Sprint";
	}

	public override string StartState()
	{
		playerControl.PlayAnimation("Sprint");
		playerControl.shadowAnimator.SetInteger("Shadow Size", 1);
		playerControl.playerStats.currentHitstate = EntityStatus.HitState.normal;
		return stateName;
	}

	public override void MainUpdate()
	{
		lastMove = movement;
		movement = playerControl.entityInput.movementInput;

		if (playerControl.animationAlert.flag[4])
		{
			SoundMaker.i.PlaySound("FootStep", playerControl.transform.position, 0.1f);
			playerControl.animationAlert.flag[4] = false;
		}

		if (playerControl.entityInput.xDown)
		{
			//Check for parriables
			playerControl.SetState("Dash Attack");
			return;
		}

		if (movement.x != 0 || movement.y != 0)
		{
			
			if (lastMove.normalized == -movement.normalized)
			{
				playerControl.GetState("Slide").PassMotion(4.5f, 0.25f, lastMove);
				playerControl.SetState("Slide");
				return;
			}
			else
			{
				playerControl.characterAnimator.SetFloat("Horizontal", movement.x);
				playerControl.characterAnimator.SetFloat("Vertical", movement.y);
			}
		}
		else
		{
			Vector3 currentDirection = new Vector3(playerControl.characterAnimator.GetFloat("Horizontal"), playerControl.characterAnimator.GetFloat("Vertical"), 0);
			playerControl.GetState("Slide").PassMotion(4.5f, 0.25f, currentDirection);
			playerControl.SetState("Slide");
			return;
		}

		if (playerControl.entityInput.aDown && playerControl.GetState("Dodge").Cooldown() <= 0)
		{
			playerControl.SetState("Dodge");
			return;
		}

		//Defense Transition
		if (playerControl.entityInput.bHold)
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
		return (movement * sprintSpeed);
	}

	public override string EndState()
	{
		return stateName;
	}
}
