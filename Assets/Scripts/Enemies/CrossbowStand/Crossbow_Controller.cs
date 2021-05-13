using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow_Controller : EntityController
{
	//State List
	public EntityState idle, fire, reload;

	void Start()
	{
		idle = new Crossbow_IdleState(this); states.Add("Idle", idle);
		fire = new Crossbow_FireState(this); states.Add("Fire", fire);
		reload = new Crossbow_ReloadState(this); states.Add("Reload", reload);

		currentState = idle;
		currentState.StartState();
	}

	protected override void FixedUpdate()
	{
		
	}
}
