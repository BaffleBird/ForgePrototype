using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour
{
	//Get Components
	[Header("Components")]
	public SpriteRenderer spriteRenderer;
	public Animator animator;
	public Rigidbody rigidBody;
	public bool orientVertically;
	[HideInInspector]public int attackStep;

	// Attack Type
	public enum AttackType { melee, projectile, aoe };
	[Header("Attack Typing")]
	public AttackType attackType = AttackType.melee;
	public bool piercing;
	public bool breakable = false;

	//Physical Manipulation
	protected Rigidbody anchor;
	protected float offset;
	protected float elevation;
	protected Vector3 direction;
	protected float speed;

	//Numbers and Typing
	[Header("Hit Settings")]
	protected float activeTime;
	public float ActiveTime { get { return activeTime; } }

	protected bool activeBox;
	public bool ActiveBox { get { return activeBox; } }

	public Hurtbox.HitID hitID = Hurtbox.HitID.neutral;
	protected List<int> BoxesHit = new List<int>();
	public EntityStatus ownerStatus;

	[HideInInspector]public float weaponPower;
	protected int damage;
	protected float knockback;
	protected int weightClass;

	//Update Methods
	protected virtual void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		rigidBody = GetComponent<Rigidbody>();
	}

	protected virtual void FixedUpdate()
	{
		MotionUpdate();
	}

	public virtual void MotionUpdate() { }

	public virtual void HitCheck() { }

	//Physical Manipulation
	public virtual void SetAnchor(Rigidbody anchor, Vector2 direction, float offset, float elevation)
	{
		this.anchor = anchor;
		this.direction = direction;
		this.offset = offset;
		this.elevation = elevation;
	}

	public virtual void SetTrajectory(Vector2 direction, float speed, float elevation)
	{
		this.direction = direction;
		this.speed = speed;
		this.elevation = elevation;
	}

	public virtual void SkewAttack()
	{
		float skew = WorldSkewer.GetSkewAngle(orientVertically);
		float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + 90;
		transform.rotation = Quaternion.Euler(new Vector3(skew, 0, angle));
	}

	public virtual void ScaleAttack(float scale)
	{
		Vector3 orignalScale = transform.localScale;
		orignalScale.x *= scale;
		orignalScale.y *= scale;
		orignalScale.z = 1;
		transform.localScale = orignalScale;
	}

	public virtual Vector3 Adjust()
	{
		Vector3 adjustElevation = Vector3.zero;
		adjustElevation.y = elevation;
		adjustElevation.z = -elevation;
		adjustElevation = transform.position + adjustElevation;
		return adjustElevation;
	}

	public virtual void DestroySelf()
	{
		Destroy(gameObject);
	}

	public void SetSpriteMat(string materialAddress)
	{
		SpriteRenderer image = GetComponent<SpriteRenderer>();
		if (materialAddress == "default")
			image.material = Resources.Load("Materials/BaseSprite") as Material;
		else
			image.material = Resources.Load("Materials/" + materialAddress) as Material;
	}

	//////////////////////////////////////////////
	public class SphereHitbox
	{
		public Vector3 position;
		public float radius;

		public SphereHitbox(Vector3 pos, float rad)
		{
			position = pos;
			radius = rad;
		}
	}
}


