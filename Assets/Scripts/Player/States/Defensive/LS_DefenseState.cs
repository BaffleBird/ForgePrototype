using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LS_DefenseState : PlayerState
{
	public float currentSpeed = 0f;
	float brakeSpeed = 0.3f;

	float parryLength = 0.22f;
	float parryTime = 0f;

	float effectLength = 0.2f;
	float effectTime = 0f;

	Vector2 movement;
	Vector3 attackDirection;
	public Vector3 slideDirection;

	public LS_DefenseState(PlayerControl playerController) : base(playerController) { stateName = "Defense"; }

	public override string StartState()
	{
		playerControl.animationAlert.ResetAlerts();
		attackDirection = playerControl.entityInput.GetPointerDirection();
		playerControl.characterAnimator.SetFloat("Horizontal", attackDirection.x);
		playerControl.characterAnimator.SetFloat("Vertical", attackDirection.y);

		playerControl.PlayAnimation("Defense Branch");
		playerControl.characterAnimator.SetInteger("Weapon Type", (int)playerControl.playerStats.currentWeapon.WeaponType);

		parryTime = parryLength;
		effectTime = effectLength + parryLength;

		playerControl.SetSpriteMat("Outline");
		playerControl.characterSprite.material.SetColor("_OutlineColor",new Color(0.2f,0.9f, 0.75f));
		playerControl.weaponSprite.material.SetColor("_OutlineColor", new Color(0.2f, 0.9f, 0.75f));
		playerControl.playerStats.currentHitstate = EntityStatus.HitState.parry;

		//EffectSpawner.instance.SpawnAnchoredGroundEffect(1, playerControl.rigidBody, Vector3.down, 0f, 0f).ScaleEffect(4.5f);
		return stateName;
	}

	public override void MainUpdate()
	{
		movement = playerControl.entityInput.movementInput;

		if (currentSpeed > 0)
			currentSpeed -= brakeSpeed;

		if (effectTime > 0) effectTime -= Time.deltaTime;
		if (effectTime > 0) parryTime -= Time.deltaTime;

		if (parryTime <= 0)
		{
			playerControl.SetSpriteMat("default");
			playerControl.playerStats.currentHitstate = EntityStatus.HitState.guard;
		}
		else
		{
			playerControl.characterSprite.material.SetColor("_OutlineColor", new Color(0.2f, 0.9f, 0.75f, parryTime / parryLength));
			playerControl.weaponSprite.material.SetColor("_OutlineColor", new Color(0.2f, 0.9f, 0.75f, parryTime / parryLength));
		}

		if (effectTime <= 0)
		{
			playerControl.SetState("Guard");
			return;
		}
	}

	public override Vector3 MotionUpdate()
	{
		return (slideDirection * currentSpeed);
	}

	public override string EndState()
	{
		currentSpeed = 0f;
		parryTime = 0f;
		effectTime = 0f;
		playerControl.animationAlert.ResetAlerts();
		playerControl.SetSpriteMat("default");
		return stateName;
	}
}
