using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow_IdleState : EntityState
{
	public bool loaded = false;
	public float timer;

	public Crossbow_IdleState(EntityController controller) : base(controller) { stateName = "Idle"; }

	public override string StartState()
	{
		if (loaded)
			timer = 0.65f;
		else
			timer = 0.5f;
		return stateName;
	}

	public override void MainUpdate()
	{
		if (timer > 0)
		{
			timer -= Time.deltaTime;
		}
		else if (timer <= 0)
		{
			if (loaded)
			{
				loaded = false;
				myController.SetState("Fire");
				return;
			}
			else
			{
				loaded = true;
				myController.SetState("Reload");
				return;
			}
		}
	}
}
