using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WraithSlash : Attack
{
	protected override void Start()
	{
		base.Start();
		SkewAttack();
		hitID = Hurtbox.HitID.enemy;
		attackType = AttackType.melee;

		ScaleAttack(3.5f);
		activeTime = 0.036f;
		damage = 4;
		knockback = 8f;
		weightClass = 2;

		SoundMaker.i.PlaySound("TinySwing", transform.position, 0.7f);
		SoundMaker.i.PlaySound("BigSwing", transform.position, 0.4f);
	}

	void Update()
	{
		activeTime -= Time.deltaTime;
		if (activeTime < 0.03f && activeTime > 0)
			HitCheck();
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

		hitCircles.Add(new SphereHitbox(Adjust() + (direction * (0.2f + offset)), 0.4f));

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
