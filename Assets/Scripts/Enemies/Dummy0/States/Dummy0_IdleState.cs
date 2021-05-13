using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy0_IdleState : EntityState
{
	public Dummy0_IdleState(EntityController controller) : base(controller) { stateName = "Idle"; }
	public override string StartState()
	{
		myController.characterAnimator.Play("Idle", -1, 0f);
		return stateName;
	}
}
