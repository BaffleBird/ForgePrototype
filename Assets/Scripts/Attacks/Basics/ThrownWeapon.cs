using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownWeapon : Attack, IDroppedWeapon
{
	public float airTime;
	float airStartTime;

	float dropSpeed = -0.025f;
	float dropAcceleration = 0.01f;
	float dropRate = 1;

	public float rotation;
	[SerializeField] GameObject shadowSprite = null;
	[SerializeField] Weapon weapon = null;
	public Weapon GetWeapon() { return weapon; }
	Vector3 anchorPos;

	protected override void Start()
	{
		base.Start();
		attackType = AttackType.projectile;
		activeBox = true;
		SkewAttack();
		rotation = Random.Range(-12f, 12f) * 2;

		hitID = Hurtbox.HitID.player;
		switch (weapon.WeaponType)
		{
			case Weapon.Type.Longsword:
				damage = (int)(weapon.weaponPower * 0.35f);
				knockback = weapon.weaponWeight * 4f;
				break;
			case Weapon.Type.Spear:
				damage = (int)(weapon.weaponPower * 0.5f);
				knockback = weapon.weaponWeight * 6f;
				break;
		}
		weightClass = weapon.weaponWeight;

		SoundMaker.i.PlaySound("TinySwing", transform.position, 0.5f);
	}

	private void Update()
	{
		//Count down air time, adjust elevation based on airtime
		dropSpeed += dropAcceleration;
		if (elevation > 0)
			elevation -= dropSpeed * dropRate;
		if (elevation < 0)
			elevation = 0;

		//If airTime is down, start spinning and deactivate hitbox
		if (elevation <= 0 && activeBox)
		{
			activeBox = false;
			speed = speed * 0.4f;
		}

		//Decrement speed and rotation if not zero, stop both if either are stopped
		if (Mathf.Abs(rotation) > 0 && !activeBox)
		{
			rotation = (Mathf.Abs(rotation) - 0.2f) * Mathf.Sign(rotation);
		}
			
		if (speed > 0 && !activeBox)
			speed -= 0.25f;
		if (speed < 0 || Mathf.Abs(rotation) < 0)
		{
			rotation = 0;
			speed = 0;
		}
	}

	public override void MotionUpdate()
	{
		Vector3 adjustElevation = Vector3.zero;
		adjustElevation.y = elevation;
		adjustElevation.z = -elevation;
		direction.z = direction.y;
		anchorPos = anchorPos + (direction * speed * Time.fixedDeltaTime);
		rigidBody.MovePosition(anchorPos + adjustElevation);
		shadowSprite.transform.position = anchorPos + new Vector3(0,0,0.001f);
		Quaternion spin = Quaternion.Euler(new Vector3(0, 0, rotation));
		if (!activeBox && speed > 0)
			rigidBody.MoveRotation(rigidBody.rotation * spin);
		Vector3 reverse = transform.rotation.eulerAngles;
		reverse.z = 0;
		shadowSprite.transform.rotation = Quaternion.Euler(reverse);

	}

	private void OnTriggerEnter(Collider other)
	{
		//Wall Collision
		if (other.gameObject.layer == 8)
		{
			Vector3 adjustElevation = Vector3.zero;
			adjustElevation.y = elevation;
			adjustElevation.z = -elevation;
			direction.z = direction.y;
			anchorPos = anchorPos + (direction * speed * Time.fixedDeltaTime * .35f);
			rigidBody.MovePosition(anchorPos + adjustElevation);

			if (activeBox)
				SoundMaker.i.PlaySound("Stab", transform.position, 0.5f, 32);

			activeBox = false;
			speed = 0;
			dropRate = 0;
			dropAcceleration = 0;
			rotation = 0;

			
		}

		//Enemy Collision
		if (other.gameObject.tag == "Hurtbox" && activeBox)
		{
			int hurtboxID = other.gameObject.GetInstanceID();
			if (!BoxesHit.Contains(hurtboxID))
			{
				Hurtbox hb = other.GetComponent<Hurtbox>();
				if (hitID != hb.hitID)
				{
					BoxesHit.Add(hurtboxID);
					hb.Damage(attackType, damage, knockback, weightClass, direction, ownerStatus);
				}
			}
		}

		//Breakable Collision
		if (other.gameObject.tag == "Neutral")
		{
			//Damage Entity (Break the thing)
		}
	}

	public void SetTrajectory(Vector2 direction, float speed, float elevation, float dropRate)
	{
		this.direction = direction;
		this.speed = speed;
		this.elevation = elevation;
		this.dropRate = dropRate;
		anchorPos = transform.position;
		transform.position = Adjust();
	}

	public override Vector3 Adjust()
	{
		Vector3 adjustElevation = Vector3.zero;
		adjustElevation.y = elevation;
		adjustElevation.z = -elevation;
		adjustElevation = anchorPos + adjustElevation;
		return adjustElevation;
	}

	public void SetWeapon(Weapon weapon)
	{
		this.weapon = weapon;

		SpriteRenderer image = GetComponent<SpriteRenderer>();
		Vector2 oldSize = new Vector2(image.sprite.rect.width, image.sprite.rect.height);
		string path = "Sprites/WeaponItems/" + weapon.WeaponType.ToString() + "_" + weapon.Name;
		Sprite weaponSprite = Resources.Load<Sprite>("Sprites/WeaponItems/" + weapon.WeaponType.ToString() + "_" + weapon.Name);
		image.sprite = weaponSprite;
		Vector2 newSize = new Vector2(image.sprite.rect.width, image.sprite.rect.height);

		//Adjust Scale of Collider to fit weapon
		BoxCollider box = GetComponent<BoxCollider>();
		box.size = new Vector3(0.1f * (newSize.x / oldSize.x), 0.35f * (newSize.y / oldSize.y), 0.1f * (newSize.x / oldSize.x));
	}

	public new void SetSpriteMat(string materialAddress)
	{
		SpriteRenderer image = GetComponent<SpriteRenderer>();
		if (materialAddress == "default")
			image.material = Resources.Load("Materials/BaseSprite") as Material;
		else
			image.material = Resources.Load("Materials/" + materialAddress) as Material;
	}

	public bool Pickable() { return !activeBox; }
}
