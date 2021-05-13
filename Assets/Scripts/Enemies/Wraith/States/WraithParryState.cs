using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WraithParryState : EntityState
{
	float parryCount;

	public WraithParryState(EntityController controller) : base(controller) { stateName = "Parry"; }

	public override string StartState()
	{
		parryCount = 1.2f;
		myController.characterAnimator.Play("Stun");
		myController.entityStatus.currentHitstate = EntityStatus.HitState.normal;

		myController.SetSpriteMat("Outline");
		myController.characterSprite.material.SetColor("_OutlineColor", new Color(0f, 1f, 1f, 1f));

		PlayerStatus.parryList.Add(myController.entityStatus);

		return base.StartState();
	}

	public override void MainUpdate()
	{
		if (parryCount > 0)
			parryCount -= Time.deltaTime;
		else if (parryCount <= 0)
		{
			myController.SetState("Stun");
			return;
		}
	}

	public override string EndState()
	{
		myController.SetSpriteMat("default");
		if (PlayerStatus.parryList.Contains(myController.entityStatus))
			PlayerStatus.parryList.Remove(myController.entityStatus);
		return base.EndState();
	}
}
