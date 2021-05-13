using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowBolt : Attack
{
	[HideInInspector]float lifeTime;

	protected override void Start()
	{
		base.Start();
		attackType = AttackType.projectile;
		breakable = true;
		activeBox = true;
		SkewAttack();

		hitID = Hurtbox.HitID.enemy;
		lifeTime = 1.5f;
		damage = 0;
		knockback = 3;
		weightClass = 2;
	}

	private void Update()
	{
		if (lifeTime > 0)
			lifeTime -= Time.deltaTime;
		else
			DestroySelf();
	}

	public override void MotionUpdate()
	{
		rigidBody.MovePosition(rigidBody.position + (direction * speed * Time.fixedDeltaTime));
	}

	public override void SetTrajectory(Vector2 direction, float speed, float elevation)
	{
		base.SetTrajectory(direction, speed, elevation);
		this.direction.z = this.direction.y;
		Adjust();
	}

	private void OnTriggerEnter(Collider other)
	{
		//Wall Collision
		if (other.gameObject.layer == 8)
		{
			SoundMaker.i.PlaySound("Stab", transform.position, 0.4f, 24);
			DestroySelf();
		}

		//Entity Collision
		bool hit = false;
		if (other.gameObject.tag == "Hurtbox" && activeBox)
		{
			int hurtboxID = other.gameObject.GetInstanceID();
			if (!BoxesHit.Contains(hurtboxID))
			{
				Hurtbox hb = other.GetComponent<Hurtbox>();
				if (hitID != hb.hitID)
				{
					BoxesHit.Add(hurtboxID);
					hit = hb.Damage(attackType, damage, knockback, weightClass, direction, ownerStatus);
				}
			}
			if (hit)
				DestroySelf();
		}

		//Breakable Collision
		if (other.gameObject.tag == "Neutral")
		{
			//Damage Entity (Break the thing)
		}
	}
}
