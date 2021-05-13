using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy1_ParriedState : EntityState
{
	float parryCount;

	public Dummy1_ParriedState(EntityController controller) : base(controller) { stateName = "Parried"; }

	public override string StartState()
	{
		parryCount = 2f;
		myController.characterAnimator.Play("Parried");
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
