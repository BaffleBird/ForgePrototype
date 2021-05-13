using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy1_PunchState : EntityState
{
	GameObject punchPrefab;

	public Dummy1_PunchState(EntityController controller) : base(controller)
	{
		stateName = "Punch";
		punchPrefab = Resources.Load("Prefabs/Dummy1_Punch") as GameObject;
	}

	public override string StartState()
	{
		myController.animationAlert.ResetAlerts();
		myController.characterAnimator.Play("Punch");
		myController.entityStatus.currentHitstate = EntityStatus.HitState.superarmor;

		return base.StartState();
	}

	public override void MainUpdate()
	{
		if (myController.animationAlert.flag[1])
		{
			myController.animationAlert.flag[1] = false;
			SpawnPunch();
		}

		if (myController.animationAlert.flag[0])
		{
			myController.SetState("Idle");
			return;
		}
	}

	public override string EndState()
	{

		return base.EndState();
	}

	void SpawnPunch()
	{
		GameObject attackPrefab = Object.Instantiate(punchPrefab, myController.transform.position, Quaternion.Euler(Vector3.zero));
		DummyPunch attack = attackPrefab.GetComponent<DummyPunch>();
		WorldSkewer.SkewObject(attackPrefab, attack.orientVertically);
		attack.ownerStatus = myController.entityStatus;
		attack.ScaleAttack(5);
		attack.SetAnchor(myController.rigidBody, Vector2.down, -0.5f, 0.5f);
	}
}
