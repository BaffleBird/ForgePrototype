﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UA_DashAttackState : PlayerState
{
	float dashSpeed = 16.5f;
	float currentSpeed = 0f;
	float brakeSpeed = 5f;
	float attackTime = 0f;

	float recoveryTime = 0.3f;
	float recovery = 0f;

	Vector2 movement;
	Vector3 attackDirection;

	public UA_DashAttackState(PlayerControl playerController) : base(playerController) { stateName = "Dash Attack"; }

	public override string StartState()
	{
		playerControl.animationAlert.ResetAlerts();
		attackDirection = new Vector2(playerControl.characterAnimator.GetFloat("Horizontal"), playerControl.characterAnimator.GetFloat("Vertical"));
		attackDirection = attackDirection.normalized;
		
		playerControl.characterAnimator.SetFloat("Horizontal", attackDirection.x);
		playerControl.characterAnimator.SetFloat("Vertical", attackDirection.y);

		playerControl.PlayAnimation("Dash Branch");
		playerControl.characterAnimator.SetInteger("Weapon Type", (int)playerControl.playerStats.currentWeapon.WeaponType);

		currentSpeed = dashSpeed;
		brakeSpeed = dashSpeed * 0.12f;
		attackTime = 0.1f;
		playerControl.playerStats.currentHitstate = EntityStatus.HitState.superarmor;

		EffectSpawner.instance.SpawnGroundEffectDirected(5, 4f, playerControl.transform.position, attackDirection);
		SpawnAttack();
		return stateName;
	}

	public override void MainUpdate()
	{
		movement = playerControl.entityInput.movementInput;
		if (attackTime > 0) attackTime -= Time.deltaTime;


		//Recovery Transition
		if (attackTime <= 0)
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
			else if (currentSpeed < 0)
				currentSpeed = 0;

			//Attack Transition
			if (playerControl.entityInput.xDown && currentSpeed == 0)
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
			if (playerControl.entityInput.aDown && playerControl.GetState("Dodge").Cooldown() <= 0 && recovery <= .05f)
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

	void SpawnAttack()
	{
		GameObject attackPrefab = Object.Instantiate(playerControl.playerStats.currentWeapon.weaponEffect, playerControl.transform.position, Quaternion.Euler(Vector3.zero));
		Attack attack = attackPrefab.GetComponent<Attack>();
		attack.weaponPower = playerControl.playerStats.currentWeapon.weaponPower;
		attack.attackStep = 5;
		attack.ScaleAttack(4f);
		attack.SetAnchor(playerControl.rigidBody, attackDirection, 0.3f, 0.45f);
	}
}
