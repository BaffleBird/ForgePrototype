using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UA_StrongAttackState : PlayerState
{
	float fullMove = 5.5f;
	float currentSpeed = 0f;
	float brakeSpeed = 0.25f;

	float recoveryTime = 0.35f;
	float recovery = 0f;

	Vector2 movement;
	Vector3 attackDirection;

	bool pulse;

	public UA_StrongAttackState(PlayerControl playerController) : base(playerController) { stateName = "Strong Attack"; }

	public override string StartState()
	{
		playerControl.animationAlert.ResetAlerts();
		attackDirection = playerControl.entityInput.GetPointerDirection();
		playerControl.characterAnimator.SetFloat("Horizontal", attackDirection.x);
		playerControl.characterAnimator.SetFloat("Vertical", attackDirection.y);

		playerControl.PlayAnimation("Strong Branch");
		playerControl.characterAnimator.SetInteger("Weapon Type", (int)playerControl.playerStats.currentWeapon.WeaponType);

		playerControl.playerStats.currentHitstate = EntityStatus.HitState.superarmor;

		return stateName;
	}

	public override void MainUpdate()
	{
		movement = playerControl.entityInput.movementInput;
		if (currentSpeed > 0)
			currentSpeed -= brakeSpeed;

		//Combo Transition
		if (playerControl.entityInput.xTap && !playerControl.animationAlert.flag[3])
		{
			playerControl.SetState("Combo");
			return;
		}
		else if (playerControl.animationAlert.flag[3] && !pulse)
		{
			EffectSpawner.instance.SpawnAnchoredGroundEffect(2, playerControl.rigidBody, Vector3.down, 0, 0f).ScaleEffect(4f);
			pulse = true;
		}

		//Attack Spawn
		if (playerControl.animationAlert.flag[2])
		{
			currentSpeed = movement != Vector2.zero || playerControl.entityInput.GetPointerDistance() > 2f ? fullMove : 0;
			SpawnStrongAttack();
			playerControl.animationAlert.flag[2] = false;
		}

		//Recovery Transition
		if (playerControl.animationAlert.flag[0] && recovery <= 0)
		{
			recovery = recoveryTime;
			playerControl.playerStats.currentHitstate = EntityStatus.HitState.normal;
		}

		//Recovery to Dodge/Move/Idle Transition
		if (recovery > 0)
		{
			recovery -= Time.deltaTime;
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
		pulse = false;
		playerControl.animationAlert.ResetAlerts();
		return stateName;
	}

	void SpawnStrongAttack()
	{
		GameObject attackPrefab = Object.Instantiate(playerControl.playerStats.currentWeapon.weaponEffect, playerControl.transform.position, Quaternion.Euler(Vector3.zero));
		WorldSkewer.SkewScale(attackPrefab);
		Attack attack = attackPrefab.GetComponent<Attack>();
		attack.weaponPower = playerControl.playerStats.currentWeapon.weaponPower;
		attack.SetAnchor(playerControl.rigidBody, attackDirection, 0.32f, 0.5f);
		attack.attackStep = 4;
	}
}
