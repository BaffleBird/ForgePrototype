using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashRedirectState : PlayerState
{
	PlayerState weaponDashAttack;

	public DashRedirectState(PlayerControl playerController) : base(playerController) { stateName = "Dash Attack"; }

	public override string StartState()
	{
		playerControl.animationAlert.ResetAlerts();

		//playerControl.PlayAnimation("Dash Branch");

		switch (playerControl.playerStats.currentWeapon.WeaponType)
		{
			case Weapon.Type.Longsword:
				weaponDashAttack = new LS_DashAttackState(playerControl);
				break;
			case Weapon.Type.Spear:
				weaponDashAttack = new SP_DashAttackState(playerControl);
				break;
			case Weapon.Type.Greatsword:
				break;
			case Weapon.Type.Shortsword:
				break;
			case Weapon.Type.Unarmed:
				weaponDashAttack = new UA_DashAttackState(playerControl);
				break;
		}
		weaponDashAttack.StartState();
		return stateName;
	}

	public override void MainUpdate()
	{
		weaponDashAttack.MainUpdate();
	}

	public override Vector3 MotionUpdate()
	{
		return weaponDashAttack.MotionUpdate();
	}

	public override string EndState()
	{
		weaponDashAttack.EndState();
		return stateName;
	}
}
