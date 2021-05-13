using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy0_StunState : EntityState
{
	float startFlashTime = 0.1f;
	float flashTime;

	float speed;
	float brakeSpeed;
	Vector3 direction;

	public Dummy0_StunState(EntityController controller) : base(controller) { stateName = "Stun"; }

	public override string StartState()
	{
		myController.characterAnimator.Play("Stun", -1, 0f);
		myController.SetSpriteMat("Flash");
		flashTime = startFlashTime;
		PhysicalEffect effect = EffectSpawner.instance.SpawnHitEffect(myController.rigidBody.position, 4f, Color.white);
		effect.SetAnchor(myController.rigidBody, effect.direction, 0f, 0.5f);
		return stateName;
	}

	public override void MainUpdate()
	{
		if (flashTime > 0)
		{
			flashTime -= Time.deltaTime;
			myController.characterSprite.material.SetFloat("_FlashAmount", (flashTime / startFlashTime) + 0.3f);
		}
		else if (flashTime != 0)
		{
			flashTime = 0;
			myController.SetSpriteMat("default");
		}

		if (speed > 0)
			speed -= brakeSpeed;
		else if (speed != 0)
			speed = 0;
		
		if (myController.animationAlert.flag[0])
		{
			myController.SetState("Idle");
			return;
		}
	}

	public override void PassMotion(float speed, float brakeSpeed, Vector3 direction)
	{
		this.speed = speed;
		this.brakeSpeed = brakeSpeed;
		this.direction = direction;
	}

	public override Vector3 MotionUpdate()
	{
		direction.z = direction.y;
		return (speed * direction);
	}

	public override string EndState()
	{
		myController.SetSpriteMat("default");
		myController.animationAlert.ResetAlerts();
		return base.EndState();
	}
}
