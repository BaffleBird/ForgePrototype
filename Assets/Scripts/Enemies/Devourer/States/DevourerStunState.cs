using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevourerStunState : EntityState
{
	float stunTime = 0.3f;

	float speed;
	float brakeSpeed;
	Vector3 direction;

	public DevourerStunState(EntityController controller) : base(controller) { stateName = "Stun"; }

	public override string StartState()
	{
		stunTime = 0.5f;
		myController.animationAlert.ResetAlerts();
		myController.characterAnimator.Play("Stun");
		myController.entityStatus.currentHitstate = EntityStatus.HitState.normal;
		return base.StartState();
	}

	public override void MainUpdate()
	{
		if (stunTime > 0)
			stunTime -= Time.deltaTime;
		if (speed > 0)
			speed -= brakeSpeed;
		else if (speed != 0)
			speed = 0;

		if (stunTime <= 0 && speed <= 0)
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
		return base.EndState();
	}
}
