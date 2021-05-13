using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardState : PlayerState
{
	public float currentSpeed = 0f;
	float brakeSpeed = 0.25f;
	Vector3 slideDirection;

	Vector2 movement;
	Vector2 attackDirection;
	

	public GuardState(PlayerControl playerController) : base(playerController) { stateName = "Guard"; }

	public override string StartState()
	{

		playerControl.animationAlert.ResetAlerts();
		attackDirection = playerControl.entityInput.GetPointerDirection();
		playerControl.characterAnimator.SetFloat("Horizontal", attackDirection.x);
		playerControl.characterAnimator.SetFloat("Vertical", attackDirection.y);
		playerControl.PlayAnimation("Guard");
		playerControl.shadowAnimator.SetInteger("Shadow Size", 1);

		playerControl.playerStats.currentHitstate = EntityStatus.HitState.guard;
		return stateName;
	}

	public override void MainUpdate()
	{
		movement = playerControl.entityInput.movementInput;
		attackDirection = playerControl.entityInput.GetPointerDirection();
		playerControl.characterAnimator.SetFloat("Horizontal", attackDirection.x);
		playerControl.characterAnimator.SetFloat("Vertical", attackDirection.y);

		if (currentSpeed > 0)
		{
			currentSpeed -= brakeSpeed;
			playerControl.characterAnimator.SetFloat("Horizontal", -slideDirection.x);
			playerControl.characterAnimator.SetFloat("Vertical", -slideDirection.y);
		}
		else if (currentSpeed < 0)
			currentSpeed = 0;

		if (playerControl.playerStats.GetNearestParry() != null && playerControl.entityInput.xDown)
		{
			playerControl.SetState("Parry Strike");
			return;
		}

		if ((!playerControl.entityInput.bHold || playerControl.entityInput.bRelease) && currentSpeed <= 0)
		{
			if (movement != Vector2.zero)
			{
				playerControl.SetState("Move");
				return;
			}
			else
			{
				playerControl.SetState("Idle");
				return;
			}
		}
	}

	public override Vector3 MotionUpdate()
	{
		return currentSpeed * slideDirection;
	}

	public override string EndState()
	{
		currentSpeed = 0f;
		playerControl.animationAlert.ResetAlerts();
		return stateName;
	}

	public override void PassDamage(float damage, float knockback, int weight, Vector2 direction)
	{
		slideDirection = direction;
		slideDirection.z = slideDirection.y;
		currentSpeed = knockback;
	}

}
