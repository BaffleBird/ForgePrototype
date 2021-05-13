using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WraithMoveState : EntityState
{
	float moveSpeed = 3f;
	Vector3 direction;
	float moveTime = 0.5f;

	public WraithMoveState(EntityController controller) : base(controller) { stateName = "Move"; }

	public override string StartState()
	{
		myController.entityStatus.currentHitstate = EntityStatus.HitState.normal;

		moveTime = 0.5f;
		float randomVal = Random.Range(0, 100);
		if (randomVal > 40 && myController.GetState("Attack").Cooldown() <= 0)
		{
			myController.characterAnimator.Play("Move Forward");
			moveSpeed = 3.5f;
			moveTime += Random.Range(0f, 1.2f);
		}
		else
		{
			myController.characterAnimator.Play("Move Backward");
			moveSpeed = -3f;
		}

		return base.StartState();
	}

	public override void MainUpdate()
	{
		direction = myController.entityInput.GetPointerDirection();
		float targetDistance = myController.entityInput.GetPointerDistance();

		if (moveTime > 0)
			moveTime -= Time.deltaTime;

		if (myController.entityInput.target != null && targetDistance < 1.2 && myController.GetState("Attack").Cooldown() <= 0)
		{
			myController.SetState("Attack");
			return;
		}

		if (moveTime <= 0)
		{
			myController.SetState("Idle");
			return;
		}
	}

	public override Vector3 MotionUpdate()
	{
		direction = direction.normalized;
		direction.z = direction.y;
		return (direction * moveSpeed);
	}

	public override string EndState()
	{
		return base.EndState();
	}

}
