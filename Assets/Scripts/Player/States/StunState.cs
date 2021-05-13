using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : PlayerState
{
	public StunState(PlayerControl playerController) : base(playerController) { stateName = "Stun"; }

	public int stunLevel = 1;

	public float knockback;
	float brakeSpeed = 0.3f;
	Vector3 direction;
	float flightTime;

	Vector2 movement;

	public override string StartState()
	{
		
		playerControl.PlayAnimation("Stun");
		
		direction.z = direction.y;
		flightTime = knockback/16;

		playerControl.characterAnimator.SetBool("Stun Recovery", false);
		playerControl.characterAnimator.SetFloat("Horizontal", -direction.x);
		playerControl.characterAnimator.SetFloat("Vertical", -direction.y);

		playerControl.playerStats.currentHitstate = EntityStatus.HitState.i_frame;
		return stateName;
	}

	public override void MainUpdate()
	{
		movement = playerControl.entityInput.movementInput;
		if (stunLevel < 2)
		{
			if (knockback > 0)
				knockback -= brakeSpeed;
		}
		else if (stunLevel >= 2)
		{
			if (flightTime > 0)
				flightTime -= Time.deltaTime;
			else if (flightTime <= 0 && knockback > 0)
			{
				knockback -= brakeSpeed;
				playerControl.characterAnimator.SetBool("Stun Recovery", true);
				if (flightTime < 0)
					SoundMaker.i.PlaySound("Dash", playerControl.transform.position, 0.1f);
				flightTime = 0;
			}	
		}

		if (knockback < 0)
		{
			knockback = 0;
		}
		else if (knockback == 0)
		{
			if (playerControl.entityInput.bHold)
				playerControl.SetState("Defense");
			else if (movement != Vector2.zero)
				playerControl.SetState("Move");
			else
				playerControl.SetState("Idle");

			return;
		}	
	}

	public override Vector3 MotionUpdate()
	{
		return (direction * knockback);
	}

	public override string EndState()
	{
		stunLevel = 1;
		knockback = 0;
		flightTime = 0;
		playerControl.playerStats.currentHitstate = EntityStatus.HitState.normal;
		return base.EndState();
	}

	public override void PassDamage(float damage, float knockback, int weight, Vector2 direction)
	{
		this.knockback = knockback;
		this.direction = direction;
		stunLevel = weight - 1;
	}
}
