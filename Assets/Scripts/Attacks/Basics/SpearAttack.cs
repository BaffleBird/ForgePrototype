using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearAttack : Attack
{
	float attackRate = 0f;
	float attackRateProc = 0f;

	protected override void Start()
	{
		base.Start();
		animator.SetInteger("Attack Step", attackStep);
		SkewAttack();
		hitID = Hurtbox.HitID.player;
		attackType = AttackType.melee;
		spriteRenderer.material.SetColor("_OutlineColor", new Color(.7f, .7f, .9f));
		float dmg = 0;
		switch (attackStep)
		{
			case 1:
				ScaleAttack(4f);
				activeTime = 0.032f;
				dmg = weaponPower * 0.1f;
				knockback = 3f;
				weightClass = 1;
				SoundMaker.i.PlaySound("Slash", transform.position, .25f);
				break;
			case 2:
			case 3:
				ScaleAttack(4f);
				activeTime = 0.032f;
				dmg = weaponPower * 0.2f;
				knockback = 4f;
				weightClass = 1;
				SoundMaker.i.PlaySound("TinySwing", transform.position, 0.7f);
				SoundMaker.i.PlaySound("Strike2", transform.position, 0.25f);
				break;
			case 4:
				ScaleAttack(5f);
				activeTime = 0.032f;
				dmg = weaponPower * 0.3f;
				knockback = 10f;
				weightClass = 2;
				SoundMaker.i.PlaySound("Strike2", transform.position, 0.2f);
				SoundMaker.i.PlaySound("BigSwing", transform.position, 0.6f);
				SoundMaker.i.PlaySound("Poof", transform.position, 0.6f);
				break;
			case 5:
				ScaleAttack(4f);
				activeTime = (1f / 60f) * 24f;
				dmg = weaponPower * 0.15f;
				knockback = 6f;
				weightClass = 2;
				SoundMaker.i.PlaySound("Slash", transform.position, 0.25f);
				SoundMaker.i.PlaySound("BigSwing", transform.position, 0.5f);
				attackRate = attackRateProc = (activeTime * .2f);
				break;
			case 6:
				ScaleAttack(4.5f);
				activeTime = (1f / 60f) * 30f;
				dmg = weaponPower * 0.1f;
				knockback = 7.5f;
				weightClass = 2;
				SoundMaker.i.PlaySound("Poof", transform.position, 0.7f);
				SoundMaker.i.PlaySound("Slash", transform.position, 0.25f);
				SoundMaker.i.PlaySound("BigSwing", transform.position, 0.6f);
				attackRate = attackRateProc = (1f / 60f) * 5f;
				break;
		}
		damage = Mathf.CeilToInt(dmg);
	}

	void Update()
	{
		if (activeTime > 0)
		{
			if (attackRateProc <= 0)
			{
				HitCheck();
				attackRateProc = attackRate;
				if (attackRate > 0)
				{
					BoxesHit.Clear();
					switch (attackStep)
					{
						case 5:
							SoundMaker.i.PlaySound("Slash", transform.position, 0.25f);
							break;
						case 6:
							SoundMaker.i.PlaySound("BigSwing", transform.position, 0.3f);
							break;
					}
				}
			}
		}
		else if (attackStep > 4)
		{
			DestroySelf();
		}

		attackRateProc -= Time.deltaTime;
		activeTime -= Time.deltaTime;
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
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.42f + offset)), 0.4f));
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (1.1f + offset)), 0.4f));
				break;
			case 2:
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.42f + offset)), 0.4f));
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.85f + offset)), 0.55f));
				break;
			case 3:
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.42f + offset)), 0.4f));
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.85f + offset)), 0.55f));
				break;
			case 4:
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.42f + offset)), 0.4f));
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (1.3f + offset)), 0.42f));
				break;
			case 5:
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.42f + offset)), 0.4f));
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.85f + offset)), 0.6f));
				break;
			case 6:
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.42f + offset)), 0.7f));
				hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.9f + offset)), 0.6f));
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
