using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevourerAttackState : EntityState
{
	GameObject ballPrefab;
	float cooldown = 0;
	Vector2 direction;

	public DevourerAttackState(EntityController controller) : base(controller)
	{
		stateName = "Attack";
		ballPrefab = Resources.Load("Prefabs/DevourBall") as GameObject; ;
	}

	public override string StartState()
	{
		direction = myController.entityInput.GetPointerDirection();
		myController.animationAlert.ResetAlerts();
		myController.characterAnimator.Play("Attack");
		myController.entityStatus.currentHitstate = EntityStatus.HitState.normal;

		SoundMaker.i.PlaySound("Alien2", myController.transform.position, 0.25f);

		return base.StartState();
	}

	public override void MainUpdate()
	{
		if (myController.animationAlert.flag[1])
		{
			SoundMaker.i.PlaySound("Poof", myController.transform.position, 0.25f);
			myController.animationAlert.flag[1] = false;
			SpawnAttack();
		}

		if (myController.animationAlert.flag[0])
		{
			float r = Random.Range(0, 100);
			if (r > 70)
				myController.SetState("Idle");
			else
				myController.SetState("Move");
			return;
		}
	}

	public override string EndState()
	{
		cooldown = Random.Range(2.5f,5f);
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
		SoundMaker.i.PlaySound("Poof", myController.transform.position, 0.6f);

		GameObject attackPrefab = Object.Instantiate(ballPrefab, myController.transform.position, Quaternion.Euler(Vector3.zero));
		DevourBall attack = attackPrefab.GetComponent<DevourBall>();
		//WorldSkewer.SkewScaleWithDirection(attackPrefab, direction);
		attack.ScaleAttack(3.5f);
		attack.SetTrajectory(direction, 6f, 0.6f);
	}
}
