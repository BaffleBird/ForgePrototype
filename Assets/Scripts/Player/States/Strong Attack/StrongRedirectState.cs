using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongRedirectState : PlayerState
{
	PlayerState weaponStrongAttack;

	public StrongRedirectState(PlayerControl playerController) : base(playerController) { stateName = "Strong Attack"; }

	public override string StartState()
	{
		playerControl.animationAlert.ResetAlerts();

		switch (playerControl.playerStats.currentWeapon.WeaponType)
		{
			case Weapon.Type.Longsword:
				weaponStrongAttack = new LS_StrongAttackState(playerControl);
				break;
			case Weapon.Type.Spear:
				weaponStrongAttack = new SP_StrongAttackState(playerControl);
				break;
			case Weapon.Type.Greatsword:
				break;
			case Weapon.Type.Shortsword:
				break;
			case Weapon.Type.Unarmed:
				weaponStrongAttack = new UA_StrongAttackState(playerControl);
				break;
		}

		weaponStrongAttack.StartState();
		return stateName;
	}

	public override void MainUpdate()
	{
		weaponStrongAttack.MainUpdate();
	}

	public override Vector3 MotionUpdate()
	{
		return weaponStrongAttack.MotionUpdate();
	}

	public override string EndState()
	{
		weaponStrongAttack.EndState();
		return stateName;
	}
}
