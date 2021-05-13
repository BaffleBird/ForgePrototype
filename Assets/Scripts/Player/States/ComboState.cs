using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboState : PlayerState
{
	float fullMove = 3.5f;
	float currentSpeed = 0f;
	float brakeSpeed = 0.25f;

	float recoveryTime = 0.25f;
	float recovery = 0f;

	int attackStep = 1;
	int maxCombo = 0;
	int miniCombo = 1;

	float offset = 0;

	Vector2 movement;
	Vector3 attackDirection;
	bool attackInput = false;
	bool attackHeld = false;

	public ComboState(PlayerControl playerController) : base(playerController){ stateName = "Combo"; }

	public override string StartState()
	{
		if (playerControl.previousState != "Combo")
			ResetCombo();

		maxCombo = playerControl.playerStats.currentWeapon.MaxCombo();

		if (attackStep == 1)
			currentSpeed = movement != Vector2.zero || playerControl.entityInput.GetPointerDistance() > 2f ? fullMove : 0;

		playerControl.animationAlert.ResetAlerts();
		attackDirection = playerControl.entityInput.GetPointerDirection();
		playerControl.characterAnimator.SetFloat("Horizontal", attackDirection.x);
		playerControl.characterAnimator.SetFloat("Vertical", attackDirection.y);
		playerControl.characterAnimator.SetInteger("Attack Step", attackStep);
		playerControl.characterAnimator.SetInteger("Weapon Type", (int)playerControl.playerStats.currentWeapon.WeaponType);
		playerControl.PlayAnimation("Combo Branch");
		playerControl.shadowAnimator.SetInteger("Shadow Size", 2);

		switch (playerControl.playerStats.currentWeapon.WeaponType)
		{
			case Weapon.Type.Longsword:
				offset = 0.5f;
				break;
			case Weapon.Type.Unarmed:
				offset = 0.55f;
				break;
			case Weapon.Type.Spear:
				offset = 0.55f;
				break;
		}

		playerControl.playerStats.currentHitstate = EntityStatus.HitState.normal;
		return stateName;
	}

	
	public override void MainUpdate()
	{
		if (currentSpeed > 0)
			currentSpeed -= brakeSpeed;

		if ((playerControl.entityInput.xDown || playerControl.animationAlert.flag[1]) && attackStep < maxCombo)
			attackInput = true;

		if (playerControl.entityInput.xHeld)
			attackHeld = true;

			movement = playerControl.entityInput.movementInput;

		//Attack Spawn
		Vector3 newDir = Vector3.zero;
		if (playerControl.animationAlert.flag[2])
		{
			if (playerControl.playerStats.currentWeapon.WeaponType == Weapon.Type.Spear && attackStep == 1)
			{
				switch (miniCombo)
				{
					case 1:
						newDir = (Quaternion.AngleAxis(12, Vector3.forward) * attackDirection);
						SpawnComboAttack(newDir);
						break;
					case 2:
						SpawnComboAttack();
						break;
					case 3:
						newDir = (Quaternion.AngleAxis(-12, Vector3.forward) * attackDirection);
						SpawnComboAttack(newDir);
						break;
				}
			}
			else
				SpawnComboAttack();
				
			playerControl.animationAlert.flag[2] = false;
			miniCombo++;
		}
		
		
		//Attack Transition
		if (playerControl.animationAlert.flag[0])
		{
			if (attackHeld && !playerControl.animationAlert.flag[1])
			{
				ResetCombo();
				playerControl.SetState("Strong Attack");
				return;
			}

			if (attackInput && attackStep < maxCombo)
			{
				if (!playerControl.animationAlert.flag[1])
					currentSpeed = movement != Vector2.zero || playerControl.entityInput.GetPointerDistance() > 2.4f ? fullMove : 0;

				attackStep++;
				playerControl.SetState("Combo");
				return;
			}
			//Start Recovery if not started
			if (recovery <= 0)
				recovery = recoveryTime;
		}

		//Recovery to Dodge/Move/Idle Transition
		if (recovery > 0)
		{
			recovery -= Time.deltaTime;

			//Defense Transition
			if (playerControl.entityInput.bHold)
			{
				ResetCombo();
				playerControl.SetState("Defense");
				return;
			}
			//Dodge Transition
			if (playerControl.entityInput.aDown && playerControl.GetState("Dodge").Cooldown() <= 0)
			{
				ResetCombo();
				playerControl.SetState("Dodge");
				return;
			}
			//Throw Weapon
			if (playerControl.entityInput.yHold)
			{
				playerControl.Throw_Pickup_Weapon();
				ResetCombo();
				playerControl.SetState("Idle");
				return;
			}

			if (recovery <= 0)
			{
				//Move Transition
				if (movement != Vector2.zero)
				{
					ResetCombo();
					playerControl.SetState("Move");
					return;
				}
				//Idle Transition
				if (recovery <= 0)
				{
					ResetCombo();
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
		recovery = 0f;
		attackInput = false;
		if (!playerControl.animationAlert.flag[1]) attackHeld = false;
		return stateName;
	}

	void ResetCombo()
	{
		recovery = 0f;
		attackInput = false;
		if (!playerControl.animationAlert.flag[1]) attackHeld = false;
		currentSpeed = 0f;
		attackStep = 1;
		miniCombo = 1;
	}

	void SpawnComboAttack()
	{
		GameObject attackPrefab = Object.Instantiate(playerControl.playerStats.currentWeapon.weaponEffect, playerControl.transform.position, Quaternion.Euler(Vector3.zero));
		Attack attack = attackPrefab.GetComponent<Attack>();
		WorldSkewer.SkewScaleWithDirection(attackPrefab, attackDirection);
		attack.weaponPower = playerControl.playerStats.currentWeapon.weaponPower;
		attack.SetAnchor(playerControl.rigidBody, attackDirection, offset, 0.4f);
		attack.attackStep = attackStep;
	}

	void SpawnComboAttack(Vector2 directionAdjust)
	{
		GameObject attackPrefab = Object.Instantiate(playerControl.playerStats.currentWeapon.weaponEffect, playerControl.transform.position, Quaternion.Euler(Vector3.zero));
		Attack attack = attackPrefab.GetComponent<Attack>();
		WorldSkewer.SkewScaleWithDirection(attackPrefab, directionAdjust);
		attack.weaponPower = playerControl.playerStats.currentWeapon.weaponPower;
		attack.SetAnchor(playerControl.rigidBody, directionAdjust, offset, 0.4f);
		attack.attackStep = attackStep;
	}
}
