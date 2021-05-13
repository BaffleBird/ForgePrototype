using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeState : PlayerState
{
	float dodgeCooldown = 0.3f;
	float dodgeSpeed = 17f;
	float dodgeLength = 0.2f;
	float dodgeTime = 0f;

	public float dodgeCD = 0f;
	Vector3 dodgeDirection;

	public DodgeState(PlayerControl playerController) : base(playerController) { stateName = "Dodge"; }

	public override string StartState()
	{
		SoundMaker.i.PlaySound("TinySwing", playerControl.transform.position, 0.5f);
		SoundMaker.i.PlaySound("Dash", playerControl.transform.position, 0.15f);

		dodgeTime = dodgeLength;
		dodgeDirection = playerControl.entityInput.movementInput;

		if (dodgeDirection.x == 0 && dodgeDirection.y == 0) //If not moving Dodge in the direction the player is facing
		{
			dodgeDirection.x = playerControl.characterAnimator.GetFloat("Horizontal");
			dodgeDirection.y = playerControl.characterAnimator.GetFloat("Vertical");
		}

		EffectSpawner.instance.SpawnGroundEffectDirected(5, 4f, playerControl.transform.position, dodgeDirection);

		playerControl.characterAnimator.SetFloat("Horizontal", dodgeDirection.x);
		playerControl.characterAnimator.SetFloat("Vertical", dodgeDirection.y);
		playerControl.PlayAnimation("Dodge");
		playerControl.shadowAnimator.SetInteger("Shadow Size", 2);

		playerControl.playerStats.currentHitstate = EntityStatus.HitState.i_frame;
		playerControl.gameObject.layer = LayerMask.NameToLayer("Phase");

		
		return stateName;
	}

	public override void ConstantUpdate()
	{
		if (dodgeCD > 0)
			dodgeCD -= Time.deltaTime;
		else
			dodgeCD = 0;
	}

	public override void MainUpdate()
	{
		dodgeTime -= Time.deltaTime;
		if (dodgeTime < dodgeLength * 0.5)
		{
			playerControl.gameObject.layer = LayerMask.NameToLayer("Entity");
			playerControl.playerStats.currentHitstate = EntityStatus.HitState.normal;
		}

		if (playerControl.entityInput.xDown)
		{
			playerControl.SetState("Dash Attack");
			return;
		}

		if (dodgeTime <= 0)
		{
			ResetCooldown();
			Vector3 movement = playerControl.entityInput.movementInput;

			if (movement.x != 0 || movement.y != 0)
			{
				playerControl.SetState("Sprint");
				return;
			}
			else
			{
				playerControl.GetState("Slide").PassMotion(6f, 0.3f, dodgeDirection);
				playerControl.SetState("Slide");
				return;
			}	
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
		dodgeDirection = dodgeDirection.normalized;
		dodgeDirection.z = dodgeDirection.y;
		return (dodgeDirection * dodgeSpeed);
	}

	public override float Cooldown()
	{
		return dodgeCD;
	}

	public override void ResetCooldown()
	{
		dodgeCD = dodgeCooldown;
	}
	public override string EndState()
	{
		playerControl.gameObject.layer = LayerMask.NameToLayer("Entity");
		return stateName;
	}
}
