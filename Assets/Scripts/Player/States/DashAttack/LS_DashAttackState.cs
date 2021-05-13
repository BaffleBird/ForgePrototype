using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LS_DashAttackState : PlayerState
{
	float dashSpeed = 8.5f;
	float currentSpeed = 0f;
	float brakeSpeed = 1f;

	float recoveryTime = 0.25f;
	float recovery = 0f;

	Vector2 movement;
	Vector3 attackDirection;

	public LS_DashAttackState(PlayerControl playerController) : base(playerController) { stateName = "Dash Attack"; }

	public override string StartState()
	{
		playerControl.animationAlert.ResetAlerts();
		attackDirection = new Vector2(playerControl.characterAnimator.GetFloat("Horizontal"), playerControl.characterAnimator.GetFloat("Vertical"));
		attackDirection = attackDirection.normalized;

		playerControl.PlayAnimation("Dash Branch");
		playerControl.characterAnimator.SetInteger("Weapon Type", (int)playerControl.playerStats.currentWeapon.WeaponType);

		currentSpeed = dashSpeed;
		playerControl.playerStats.currentHitstate = EntityStatus.HitState.superarmor;
		return stateName;
	}

	public override void MainUpdate()
	{
		movement = playerControl.entityInput.movementInput;

		//Attack Spawn
		if (playerControl.animationAlert.flag[1])
		{
			SpawnAttack(1);
			playerControl.animationAlert.flag[1] = false;
		}
		if (playerControl.animationAlert.flag[2])
		{
			SpawnAttack(2);
			playerControl.animationAlert.flag[2] = false;
		}

		//Recovery Transition
		if (playerControl.animationAlert.flag[0])
		{
			if (recovery <= 0)
			{
				recovery = recoveryTime;
				playerControl.GetState("Dodge").ResetCooldown();
			}
		}

		//Recovery to Dodge/Move/Idle Transition
		if (recovery > 0)
		{
			recovery -= Time.deltaTime;
			if (currentSpeed > 0)
				currentSpeed -= brakeSpeed;

			//Attack Transition
			if (playerControl.entityInput.xDown)
			{
				playerControl.SetState("Combo");
				return;
			}
			//Defense Transition
			if (playerControl.entityInput.bHold)
			{
				playerControl.SetState("Defense");
				return;
			}
			//Dodge Transition
			if (playerControl.entityInput.aDown && playerControl.GetState("Dodge").Cooldown() <= 0)
			{
				playerControl.SetState("Dodge");
				return;
			}
			//Throw Weapon
			if (playerControl.entityInput.yHold)
			{
				playerControl.Throw_Pickup_Weapon();
				playerControl.SetState("Idle");
				return;
			}

			if (recovery <= 0)
			{
				//Move Transition
				if (movement != Vector2.zero)
				{
					playerControl.SetState("Move");
					return;
				}
				//Idle Transition
				if (recovery <= 0)
				{
					playerControl.SetState("Idle");
					return;
				}
			}
		}
	}

	public override Vector3 MotionUpdate()
	{
		attackDirection.z = attackDirection.y;
		return (attackDirection * currentSpeed);
	}

	public override string EndState()
	{
		currentSpeed = 0f;
		recovery = 0f;
		playerControl.animationAlert.ResetAlerts();
		return stateName;
	}

	void SpawnAttack(int i)
	{
		GameObject attackPrefab = Object.Instantiate(playerControl.playerStats.currentWeapon.weaponEffect, playerControl.transform.position, Quaternion.Euler(Vector3.zero));
		Attack attack = attackPrefab.GetComponent<Attack>();
		if (i == 1)
		{
			attack.attackStep = 8;
			attack.ScaleAttack(4f);
		}
		else if (i == 2)
		{
			attack.attackStep = 9;
			attack.ScaleAttack(3.5f);
		}
		attack.weaponPower = playerControl.playerStats.currentWeapon.weaponPower;
		attack.SetAnchor(playerControl.rigidBody, attackDirection, 0.1f, 0.4f);
	}
}
