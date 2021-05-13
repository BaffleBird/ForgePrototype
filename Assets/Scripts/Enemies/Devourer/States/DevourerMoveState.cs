using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevourerMoveState : EntityState
{
	float moveSpeed = 3f;
	Vector3 direction;
	float moveTime = 0.5f;
	int sideTrack = 0;

	public DevourerMoveState(EntityController controller) : base(controller) { stateName = "Move"; }

	public override string StartState()
	{
		myController.entityStatus.currentHitstate = EntityStatus.HitState.normal;
		myController.characterAnimator.Play("Idle");

		moveTime = 0.75f;
		float dist = myController.entityInput.GetPointerDistance();
		sideTrack = 0;

		if (dist > 7)
		{
			moveSpeed = Mathf.Abs(moveSpeed);
		}
		else if (dist < 4)
		{
			moveSpeed = -Mathf.Abs(moveSpeed);
		}
		else
		{
			float randomVal = Random.Range(0, 100);
			if (randomVal <= 10)
			{
				moveTime += Random.Range(0f, 0.75f);
			}
			else if (randomVal > 10)
			{
				if (randomVal < 55)
					sideTrack = 1;
				else
					sideTrack = -1;
			}
		}
		
		return base.StartState();
	}

	public override void MainUpdate()
	{
		direction = myController.entityInput.GetPointerDirection();
		if (sideTrack != 0)
			direction = (Quaternion.AngleAxis(90 * sideTrack, Vector3.forward) * direction);

		float targetDistance = myController.entityInput.GetPointerDistance();

		if (moveTime > 0)
			moveTime -= Time.deltaTime;

		if (myController.entityInput.target != null && myController.GetState("Attack").Cooldown() <= 0)
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
