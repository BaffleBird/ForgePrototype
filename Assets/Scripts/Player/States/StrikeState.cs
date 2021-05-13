using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeState : PlayerState
{
	EntityStatus target;
	Vector3 direction;
	float dashTime;
	float speed;
	float brakeSpeed = 2f;
	float flash;
	

	public StrikeState(PlayerControl playerController) : base(playerController) { stateName = "Parry Strike"; }

	public override string StartState()
	{
		playerControl.animationAlert.ResetAlerts();
		playerControl.PlayAnimation("Parry Strike");
		playerControl.characterAnimator.SetInteger("Weapon Type", (int)playerControl.playerStats.currentWeapon.WeaponType);

		playerControl.playerStats.currentHitstate = EntityStatus.HitState.i_frame;
		playerControl.gameObject.layer = LayerMask.NameToLayer("Phase");

		target = playerControl.playerStats.GetNearestParry();
		direction = (Vector2)playerControl.transform.position - (Vector2)target.entityController.transform.position;
		direction = direction.normalized; direction.z = direction.y;
		direction = -direction;
		playerControl.characterAnimator.SetFloat("Horizontal", direction.x);
		playerControl.characterAnimator.SetFloat("Vertical", direction.y);

		dashTime = 0.1f;
		speed = (Vector3.Distance(target.entityController.transform.position, playerControl.transform.position) + 2.5f) * 4.5f;
		brakeSpeed = speed * 0.075f;
		flash = 1f;

		playerControl.SetSpriteMat("Flash");
		EffectSpawner.instance.SpawnGroundEffectDirected(5, 4f, playerControl.transform.position, direction);
		float damage = playerControl.playerStats.currentWeapon.weaponPower * .8f;
		target.Damage(Attack.AttackType.melee, Mathf.CeilToInt(damage), 0f, 3, direction, playerControl.playerStats);
		Vector3 newDir = (Quaternion.AngleAxis(45, Vector3.forward) * direction);
		EffectSpawner.instance.SpawnFGEffect(4, target.entityController.transform.position + new Vector3(0, 0.5f, -0.3f), 8f, newDir, Color.white, new Color(0,0,0,0));

		PlayerStatus.parryList.Remove(target);

		SoundMaker.i.PlaySound("Strike", playerControl.transform.position, 0.5f);
		SoundMaker.i.PlaySound("Strike2", playerControl.transform.position, 0.5f);
		return stateName;
	}

	public override void MainUpdate()
	{
		//Motion timing
		if (dashTime > 0)
		{
			dashTime -= Time.deltaTime;
		}
		else if (speed > 0)
		{
			speed -= brakeSpeed;
			if (speed < 0) speed = 0;

			//Flash
			if (flash > 0)
			{
				flash -= 0.2f;
				playerControl.characterSprite.material.SetFloat("_FlashAmount", flash);
				playerControl.weaponSprite.material.SetFloat("_FlashAmount", flash);
			}
			else if (flash <= 0)
				playerControl.SetSpriteMat("default");
		}

		//Strike Chain
		if (playerControl.playerStats.GetNearestParry() != null && playerControl.entityInput.xDown && dashTime <= 0)
		{
			playerControl.SetState("Parry Strike");
			return;
		}

		//Return to Neutral
		if (speed <= 0)
		{
			if (playerControl.entityInput.movementInput != Vector2.zero)
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
		return direction * speed;
	}

	public override string EndState()
	{
		playerControl.gameObject.layer = LayerMask.NameToLayer("Entity");
		playerControl.SetSpriteMat("default");
		return stateName;
	}
}
