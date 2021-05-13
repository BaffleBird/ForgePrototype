using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow_ReloadState : EntityState
{
	public Crossbow_ReloadState(EntityController controller) : base(controller) { stateName = "Reload"; }

	public override string StartState()
	{
		myController.characterAnimator.Play("Reload");
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
