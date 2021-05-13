using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalEffect : MonoBehaviour
{
	//Get Components
	[Header("Components")]
	public Animator animator = null;
	public Rigidbody rigidBody = null;
	public bool orientVertically;

	//Anchor
	protected Rigidbody anchor = null;
	protected float offset = 0f;
	protected float elevation = 0f;

	//Physicality
	public Vector3 direction = Vector3.down;
	protected float speed = 0f;

	protected virtual void Start()
	{
		animator = GetComponent<Animator>();
		rigidBody = GetComponent<Rigidbody>();
		SkewEffect();
	}

	protected virtual void FixedUpdate()
	{
		MotionUpdate();
	}

	public virtual void HitCheck() { }

	public virtual void MotionUpdate()
	{
		Vector3 adjustElevation = Vector3.zero;
		adjustElevation.y = elevation;
		adjustElevation.z = -elevation;

		direction.z = direction.y;
		if (anchor != null)
		{
			transform.position = anchor.position + adjustElevation + (direction * offset);
		}
	}

	public virtual void SetAnchor(Rigidbody anchor, Vector2 direction, float offset, float elevation)
	{
		this.anchor = anchor;
		this.direction = direction;
		this.offset = offset;
		this.elevation = elevation;
		MotionUpdate();
	}

	public virtual void SkewEffect()
	{
		float skew = WorldSkewer.GetSkewAngle(orientVertically);
		float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) + 90;
		transform.rotation = Quaternion.Euler(new Vector3(skew, 0, angle));
	}

	public virtual void ScaleEffect(float scale)
	{
		transform.localScale = new Vector3(scale, scale, 1);
	}

	public void DestroySelf()
	{
		Destroy(gameObject);
	}

	public void SetID(int id)
	{
		animator.SetInteger("Effect ID", id);
	}
}
