using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveRedirectState : PlayerState
{
	PlayerState weaponDefMove;
	bool reParry = false;

	public DefensiveRedirectState(PlayerControl playerController) : base(playerController) { stateName = "Defense"; }

	public override string StartState()
	{
		playerControl.animationAlert.ResetAlerts();

		switch (playerControl.playerStats.currentWeapon.WeaponType)
		{
			case Weapon.Type.Longsword:
				weaponDefMove = new LS_DefenseState(playerControl);
				break;
			case Weapon.Type.Spear:
				weaponDefMove = new SP_DefenseState(playerControl);
				break;
			case Weapon.Type.Greatsword:
				break;
			case Weapon.Type.Shortsword:
				break;
			case Weapon.Type.Unarmed:
				weaponDefMove = new UA_DefenseState(playerControl);
				break;
		}
		weaponDefMove.StartState();

		SoundMaker.i.PlaySound("Clink", playerControl.transform.position, 0.7f);
		reParry = false;

		return stateName;
	}

	public override void MainUpdate()
	{
		weaponDefMove.MainUpdate();

		//Defense Transition
		if (playerControl.entityInput.bDown && reParry)
		{
			playerControl.SetState("Defense");
			return;
		}
	}

	public override Vector3 MotionUpdate()
	{
		return weaponDefMove.MotionUpdate();
	}

	public override string EndState()
	{
		weaponDefMove.EndState();
		return stateName;
	}

	public override void PassDamage(float damage, float knockback, int weight, Vector2 direction)
	{
		reParry = true;
	}
}
