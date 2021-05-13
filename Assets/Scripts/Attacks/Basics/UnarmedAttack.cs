using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnarmedAttack : Attack
{
	protected override void Start()
	{
		base.Start();
		animator.SetInteger("Attack Step", attackStep);
		SkewAttack();
		hitID = Hurtbox.HitID.player;
		attackType = AttackType.melee;
		float dmg = 0;
		switch (attackStep)
		{
			case 1:
				ScaleAttack(3.5f);
				activeTime = 0.032f;
				dmg = weaponPower * 0.2f;
				knockback = 2f;
				weightClass = 1;
				SoundMaker.i.PlaySound("TinySwing", transform.position, 0.8f);
				break;
			case 2:
				ScaleAttack(3.5f);
				activeTime = 0.032f;
				dmg = weaponPower * 0.2f;
				knockback = 2f;
				weightClass = 1;
				SoundMaker.i.PlaySound("TinySwing", transform.position, 0.8f);
				break;
			case 3:
				ScaleAttack(4f);
				activeTime = 0.032f;
				dmg = weaponPower * 0.4f;
				knockback = 6f;
				weightClass = 1;
				SoundMaker.i.PlaySound("BigSwing", transform.position, 0.75f);
				break;
			case 4:
				ScaleAttack(3f);
				activeTime = 0.048f;
				dmg = weaponPower * 0.35f;
				knockback = 8f;
				weightClass = 1;
				SoundMaker.i.PlaySound("TinySwing", transform.position, 0.9f);
				SoundMaker.i.PlaySound("BigSwing", transform.position, 0.5f);
				break;
			case 5:
				ScaleAttack(1f);
				activeTime = 0.048f;
				dmg = weaponPower * 0.26f;
				knockback = 14f;
				weightClass = 2;
				SoundMaker.i.PlaySound("TinySwing", transform.position, 0.9f);
				SoundMaker.i.PlaySound("BigSwing", transform.position, 0.5f);
				break;
		}
		damage = Mathf.CeilToInt(dmg);
	}

	void Update()
	{
		if (attackStep == 0)
		{
			DestroySelf();
		}
		if (activeTime > 0)
		{
			HitCheck();
			activeTime -= Time.deltaTime;
		}
	}

	public override void MotionUpdate()
	{
		if (anchor != null)
			transform.position = Adjust() + (direction * offset);
	}

	public override void SetAnchor(Rigidbody anchor, Vector2 direction, float offset = 0, float elevation = 0)
	{
		base.SetAnchor(anchor, direction, offset, elevation);
		this.direction.z = this.direction.y;
		transform.position = Adjust() + (this.direction * offset);
	}

	public override Vector3 Adjust()
	{
		Vector3 adjustElevation = Vector3.zero;
		adjustElevation.y = elevation;
		adjustElevation.z = -elevation;
		adjustElevation = anchor.position + adjustElevation;
		return adjustElevation;
	}

	List<SphereHitbox> hitCircles = new List<SphereHitbox>();
	public override void HitCheck()
	{
		List<Collider> hurtboxes = new List<Collider>();
		LayerMask hurtboxMask = LayerMask.GetMask("Hurtbox");
		switch (attackStep)
		{
			case 1:
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.2f + offset)), 0.4f));
				break;
			case 2:
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.2f + offset)), 0.4f));
				break;
			case 3:
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.2f + offset)), 0.45f));
				Vector3 newDir = (Quaternion.AngleAxis(40, Vector3.forward) * direction);
				hitCircles.Add(new SphereHitbox(Adjust() + (newDir * (0.2f + offset)), 0.35f));
				newDir = (Quaternion.AngleAxis(-40, Vector3.forward) * direction);
				hitCircles.Add(new SphereHitbox(Adjust() + (newDir * (0.2f + offset)), 0.35f));
				break;
			case 4:
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.32f + offset)), 0.35f));
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.8f + offset)), 0.35f));
				break;
			case 5:
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.5f + offset)), 0.57f));
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.1f + offset)), 0.57f));
				break;
		}

		for (int h = 0; h < hitCircles.Count; h++)
			hurtboxes.AddRange(Physics.OverlapSphere(hitCircles[h].position, hitCircles[h].radius, hurtboxMask));

		for (int i = 0; i < hurtboxes.Count; i++)
		{
			int hurtboxID = hurtboxes[i].gameObject.GetInstanceID();
			if (BoxesHit.Contains(hurtboxID))
				continue;
			else
			{
				Hurtbox hb = hurtboxes[i].GetComponent<Hurtbox>();
				if (hitID != hb.hitID)
				{
					BoxesHit.Add(hurtboxID);
					Vector3 dir = direction;
					hb.Damage(attackType, damage, knockback, weightClass, dir, ownerStatus);
				}
			}
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		for (int h = 0; h < hitCircles.Count; h++)
			Gizmos.DrawWireSphere(hitCircles[h].position, hitCircles[h].radius);
	}
}
