using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongswordAttack : Attack
{
	protected override void Start()
	{
		base.Start();
		animator.SetInteger("Attack Step", attackStep);
		SkewAttack();
		hitID = Hurtbox.HitID.player;
		attackType = AttackType.melee;
		spriteRenderer.material.SetColor("_OutlineColor", new Color(0.8f, 0.9f, 1f));
		float dmg = 0;
		switch (attackStep)
		{
			case 1:
				ScaleAttack(3f);
				activeTime = 0.032f;
				dmg = weaponPower * 0.25f;
				knockback = 2f;
				weightClass = 1;
				SoundMaker.i.PlaySound("Slash", transform.position, .25f);
				break;
			case 2:
				ScaleAttack(3f);
				activeTime = 0.032f;
				dmg = weaponPower * 0.25f;
				knockback = 2f;
				weightClass = 1;
				SoundMaker.i.PlaySound("Slash", transform.position, .25f);
				break;
			case 4:
				ScaleAttack(3f);
				activeTime = 0.032f;
				dmg = weaponPower * 0.30f;
				knockback = 2f;
				weightClass = 1;
				SoundMaker.i.PlaySound("TinySwing", transform.position, 0.8f);
				SoundMaker.i.PlaySound("Slash", transform.position, 0.25f);
				break;
			case 5:
				ScaleAttack(4f);
				activeTime = 0.032f;
				dmg = weaponPower * 0.4f;
				knockback = 6f;
				weightClass = 2;
				SoundMaker.i.PlaySound("Strike2", transform.position, 0.25f);
				SoundMaker.i.PlaySound("BigSwing", transform.position, 0.6f);
				break;
			case 6:
				ScaleAttack(3.5f);
				activeTime = 0.048f;
				dmg = weaponPower * 0.3f;
				knockback = 1f;
				weightClass = 2;
				SoundMaker.i.PlaySound("Poof", transform.position, 0.7f);
				SoundMaker.i.PlaySound("Slash", transform.position, 0.25f);
				SoundMaker.i.PlaySound("BigSwing", transform.position, 0.5f);
				break;
			case 7:
				ScaleAttack(3.5f);
				activeTime = 0.048f;
				dmg = weaponPower * 0.3f;
				knockback = 5f;
				weightClass = 2;
				SoundMaker.i.PlaySound("Poof", transform.position, 0.7f);
				SoundMaker.i.PlaySound("Slash", transform.position, 0.25f);
				SoundMaker.i.PlaySound("BigSwing", transform.position, 0.5f);
				break;
			case 8:
				ScaleAttack(1f);
				activeTime = 0.064f;
				dmg = weaponPower * 0.2f;
				knockback = 2f;
				weightClass = 1;
				SoundMaker.i.PlaySound("Slash", transform.position, 0.25f);
				SoundMaker.i.PlaySound("BigSwing", transform.position, 0.3f);
				break;
			case 9:
				ScaleAttack(1f);
				activeTime = 0.064f;
				dmg = weaponPower * 0.28f;
				knockback = 6f;
				weightClass = 2;
				SoundMaker.i.PlaySound("Strike2", transform.position, 0.25f);
				SoundMaker.i.PlaySound("BigSwing", transform.position, 0.6f);
				break;
		}
		damage = Mathf.CeilToInt(dmg);
	}

	void Update()
    {
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
		//this.direction.y /= 1.414f;
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
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.32f + offset)), 0.7f));
				break;
			case 2:
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.42f + offset)), 0.6f));
				break;
			case 4:
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.32f + offset)), 0.5f));
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.62f + offset)), 0.5f));
				break;
			case 5:
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.42f + offset)), 0.6f));
				Vector3 newDir = (Quaternion.AngleAxis(40, Vector3.forward) * direction);
				hitCircles.Add(new SphereHitbox(Adjust() + (newDir * (0.45f + offset)), 0.5f));
				newDir = (Quaternion.AngleAxis(-40, Vector3.forward) * direction);
				hitCircles.Add(new SphereHitbox(Adjust() + (newDir * (0.45f + offset)), 0.5f));
				break;
			case 6:
				hitCircles.Add(new SphereHitbox(transform.position + (direction * (0.5f + offset)), 0.8f));
				break;
			case 7:
				hitCircles.Add(new SphereHitbox(transform.position + (direction * (0.5f + offset)), 0.8f));
				break;
			case 8:
				hitCircles.Add(new SphereHitbox(transform.position, 0.8f));
				break;
			case 9:
				hitCircles.Add(new SphereHitbox(transform.position, 1.1f));
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
					if (attackStep == 9)
						dir = hurtboxes[i].transform.position - transform.position;
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
