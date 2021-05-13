using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPunch : Attack
{
	protected override void Start()
	{
		base.Start();
		SkewAttack();
		hitID = Hurtbox.HitID.enemy;
		attackType = AttackType.melee;

		activeTime = 0.036f;
		damage = 0;
		knockback = 6f;
		weightClass = 3;

		SoundMaker.i.PlaySound("BigSwing", transform.position, 0.8f);
	}

	void Update()
	{
		activeTime -= Time.deltaTime;
		if (activeTime < 0.03f && activeTime > 0)
			HitCheck();
	}

	protected override void FixedUpdate()
	{
		
	}

	public override void SetAnchor(Rigidbody anchor, Vector2 direction, float offset = 0, float elevation = 0)
	{
		base.SetAnchor(anchor, direction, offset, elevation);
		this.direction.z = this.direction.y;
		transform.position = Adjust() + (this.direction * offset);
	}

	List<SphereHitbox> hitCircles = new List<SphereHitbox>();
	public override void HitCheck()
	{
		List<Collider> hurtboxes = new List<Collider>();
		LayerMask hurtboxMask = LayerMask.GetMask("Hurtbox");

		Vector3 newDir = (Quaternion.AngleAxis(-60, Vector3.forward) * direction);
		hitCircles.Add(new SphereHitbox(Adjust() + (newDir * (0.7f)), 0.5f));
		newDir = (Quaternion.AngleAxis(-20, Vector3.forward) * direction);
		hitCircles.Add(new SphereHitbox(Adjust() + (newDir * (1f)), 0.7f));

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
