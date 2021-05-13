using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WraithAttackState : EntityState
{
	GameObject slashPrefab;
	float cooldown = 0;
	Vector2 direction;

	public WraithAttackState(EntityController controller) : base(controller)
	{
		stateName = "Attack";
		slashPrefab = Resources.Load("Prefabs/Wraith_Slash") as GameObject;;
	}

	public override string StartState()
	{
		direction = myController.entityInput.GetPointerDirection();
		myController.animationAlert.ResetAlerts();
		myController.characterAnimator.Play("Attack");
		myController.entityStatus.currentHitstate = EntityStatus.HitState.normal;

		return base.StartState();
	}

	public override void MainUpdate()
	{
		if (myController.animationAlert.flag[1])
		{
			myController.animationAlert.flag[1] = false;
			SpawnAttack();
		}

		if (myController.animationAlert.flag[0])
		{
			float r = Random.Range(0, 100);
			if (r > 50)
				myController.SetState("Idle");
			else
				myController.SetState("Move");
			return;
		}
	}

	public override string EndState()
	{
		cooldown = 0.75f;
		myController.SetSpriteMat("default");
		return base.EndState();
	}

	public override void ConstantUpdate()
	{
		if (cooldown > 0)
			cooldown -= Time.deltaTime;
	}

	public override float Cooldown()
	{
		return cooldown;
	}

	void SpawnAttack()
	{
		GameObject attackPrefab = Object.Instantiate(slashPrefab, myController.transform.position, Quaternion.Euler(Vector3.zero));
		WraithSlash attack = attackPrefab.GetComponent<WraithSlash>();
		WorldSkewer.SkewObject(attackPrefab, attack.orientVertically);
		attack.ownerStatus = myController.entityStatus;
		attack.SetAnchor(myController.rigidBody, direction, 0.5f, 0.5f);
	}
}
