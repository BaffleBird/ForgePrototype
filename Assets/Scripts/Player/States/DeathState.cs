using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : PlayerState
{
	public DeathState(PlayerControl controller) : base(controller) { stateName = "Death"; }

	public override string StartState()
	{
		playerControl.DropWeapon();
		playerControl.PlayAnimation("Death");
		playerControl.playerStats.currentHitstate = EntityStatus.HitState.i_frame;

		return base.StartState();
	}

	public override void MainUpdate()
	{
		if (playerControl.animationAlert.flag[1])
		{
			EffectSpawner.instance.SpawnAnchoredGroundEffect(2, playerControl.rigidBody, Vector3.down, 0, 0f).ScaleEffect(4f);
			playerControl.animationAlert.flag[1] = false;
		}

		if(playerControl.animationAlert.flag[0])
		{
			playerControl.DieDie();
			playerControl.animationAlert.flag[0] = false;
		}
	}

	public override string EndState()
	{
		return base.EndState();
	}
}
