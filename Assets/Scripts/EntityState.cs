using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityState
{
	public EntityController myController = null;
	public string stateName = "";

	public EntityState (EntityController myController = null)
	{
		this.myController = myController;
	}

	public virtual string StartState() { return stateName; }

	public virtual void ConstantUpdate() { }

	public virtual void MainUpdate() { }

	public virtual string EndState() { return stateName; }

	public virtual float Cooldown() { return 0f; }

	public virtual void ResetCooldown() { }

	public virtual void PassMotion(float speed, float brakeSpeed, Vector3 direction) { }

	public virtual void PassDamage(float damage, float knockback, int weight, Vector2 direction) { }

	public virtual Vector3 MotionUpdate()
	{
		return Vector3.zero;
	}
}