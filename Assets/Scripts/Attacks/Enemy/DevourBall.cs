using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevourBall : Attack
{
	[HideInInspector] public float lifeTime;

	protected override void Start()
	{
		base.Start();
		attackType = AttackType.projectile;
		breakable = true;
		activeBox = true;
		//SkewAttack();

		hitID = Hurtbox.HitID.enemy;
		lifeTime = 1.5f;
		damage = 8;
		knockback = 7;
		weightClass = 3;
	}

	private void Update()
	{
		if (lifeTime > 0)
			lifeTime -= Time.deltaTime;
		else if (activeBox)
			Explode();
	}

	public override void MotionUpdate()
	{
		rigidBody.MovePosition(rigidBody.position + (direction * speed * Time.fixedDeltaTime));
	}

	public override void SetTrajectory(Vector2 direction, float speed, float elevation)
	{
		base.SetTrajectory(direction, speed, elevation);
		this.direction.z = this.direction.y;
		transform.position = Adjust();
	}

	public void Explode()
	{
		speed = 0;
		activeBox = false;
		animator.Play("Impact");
		SoundMaker.i.PlaySound("Poof", transform.position, 0.7f, 24);
	}

	private void OnTriggerEnter(Collider other)
	{
		//Wall Collision
		if (other.gameObject.layer == 8 && activeBox)
		{
			Explode();
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
			{
				Explode();
			}
		}

		//Breakable Collision
		if (other.gameObject.tag == "Neutral")
		{
			//Damage Entity (Break the thing)
		}
	}
}
