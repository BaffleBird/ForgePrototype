using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow_FireState : EntityState
{
	public GameObject ammoPrefab;
	public Crossbow_FireState(EntityController controller) : base(controller)
	{
		stateName = "Fire";
		ammoPrefab = Resources.Load<GameObject>("Prefabs/Bolt");
	}

	public override string StartState()
	{
		myController.characterAnimator.Play("Fire");

		GameObject boltObject = Object.Instantiate(ammoPrefab, myController.transform.position, Quaternion.Euler(Vector3.zero));
		WorldSkewer.SkewObject(boltObject, false);
		CrossbowBolt bolt = boltObject.GetComponent<CrossbowBolt>();
		bolt.ScaleAttack(2f);
		bolt.SetTrajectory(Vector2.down, 9f, 0.6f);

		SoundMaker.i.PlaySound("TinySwing", myController.transform.position, 0.6f);
		return stateName;
	}

	public override void MainUpdate()
	{
		if (myController.animationAlert.flag[0])
		{
			myController.SetState("Idle");
			return;
		}
	}

	public override string EndState()
	{
		myController.animationAlert.ResetAlerts();
		return base.EndState();
	}
}
