using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideState : PlayerState
{
	float residualSpeed = 0f;
	float brakeSpeed = 0f;
	Vector3 residualDirection = Vector3.zero;

	public SlideState(PlayerControl playerController) : base(playerController) { stateName = "Slide"; }

	public override string StartState()
	{
		playerControl.characterAnimator.SetFloat("Horizontal", residualDirection.x);
		playerControl.characterAnimator.SetFloat("Vertical", residualDirection.y);
		playerControl.PlayAnimation("Slide");
		playerControl.shadowAnimator.SetInteger("Shadow Size", 2);

		playerControl.playerStats.currentHitstate = EntityStatus.HitState.normal;

		SoundMaker.i.PlaySound("BigSwing", playerControl.transform.position, 0.2f);
		SoundMaker.i.PlaySound("Dash", playerControl.transform.position, 0.05f);
		return stateName;
	}

	public override void MainUpdate()
	{
		Vector3 movement = playerControl.entityInput.movementInput;

		if (residualSpeed > 0)
			residualSpeed = residualSpeed > 0 ? residualSpeed - brakeSpeed : 0;

		if (residualSpeed <= 0)
		{
			playerControl.SetState("Idle");
		}
		else if (movement.x != 0 || movement.y != 0)
		{
			if (movement.normalized == -residualDirection.normalized && residualSpeed < 0.75f && residualSpeed > 0.35f && playerControl.previousState == "Sprint")
			{
				playerControl.SetState("Sprint");
				return;
			}
		}

		if (playerControl.entityInput.aDown && playerControl.GetState("Dodge").Cooldown() <= 0)
		{
			playerControl.SetState("Dodge");
			return;
		}

		if (playerControl.entityInput.xDown)
		{
			if (playerControl.playerStats.GetNearestParry() != null)
				playerControl.SetState("Parry Strike");
			else
				playerControl.SetState("Strong Attack");
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
		if (residualSpeed > 0)
		{
			residualDirection = residualDirection.normalized;
			residualDirection.z = residualDirection.y;
			return (residualDirection * residualSpeed);
		}
		return Vector3.zero;
	}

	public override string EndState()
	{
		ResetSlide();
		return stateName;
	}

	public override void PassMotion(float speed, float brakeSpeed, Vector3 direction)
	{
		residualSpeed = speed;
		this.brakeSpeed = brakeSpeed;
		residualDirection = direction;
	}

	public void ResetSlide()
	{
		PassMotion(0, 0, Vector3.zero);
	}
}
