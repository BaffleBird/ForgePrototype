using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : EntityStatus
{
	public int equipIndex = 0;
	[SerializeField]int inventorySize = 4;
	[SerializeField] Weapon defaultWeapon = null;
	public List<Weapon> inventory = new List<Weapon>();

	[HideInInspector]public static List<EntityStatus> parryList = new List<EntityStatus>();

	public void AdjustInventory()
	{
		if (inventory.Count < inventorySize)
		{
			int emptySlots = inventorySize - inventory.Count;
			for (int i = 0; i < emptySlots; i++)
			{
				inventory.Add(defaultWeapon);
			}
		}
	}

	public Weapon currentWeapon
	{
		get{return inventory[equipIndex]; }
		set{ inventory[equipIndex] = value; }
	}

	public void DiscardWeapon()
	{
		if (defaultWeapon != null)
			currentWeapon = defaultWeapon;
	}

	public void SwitchWeapon(bool next)
	{
		if (next) equipIndex += 1;
		else equipIndex -= 1;

		if (equipIndex > inventorySize - 1) equipIndex = 0;
		else if (equipIndex < 0) equipIndex = inventorySize - 1;
	}

	public void SwitchWeapon(int index)
	{
		equipIndex = index;
	}

	public override bool Damage(Attack.AttackType type, int damage, float knockback, int weight, Vector2 direction, EntityStatus ownerStatus)
	{
		bool hit = false;
		if (type == Attack.AttackType.melee && currentHitstate == HitState.parry)
		{
			ownerStatus.Parry();
			Heal(5);
			EffectSpawner.instance.SpawnAnchoredGroundEffect(1, entityController.rigidBody, Vector3.down, 0f, 0f).ScaleEffect(4.5f);
		}
		else if (currentHitstate != HitState.i_frame && currentHitstate != HitState.special)
		{
			hit = true;
			TakeDamage(damage);
		}
		entityController.Damage(type, damage, knockback, weight, direction, ownerStatus);
		return hit;
	}

	public void TakeDamage(float damage)
	{
		if (currentHitstate == HitState.normal)
		{
			health -= damage;
		}
		if (currentHitstate == HitState.guard)
		{
			switch (currentWeapon.WeaponType)
			{
				case Weapon.Type.Longsword:
					health -= damage * 0.5f;
					break;
				case Weapon.Type.Spear:
					health -= damage * 0.35f;
					break;
				case Weapon.Type.Unarmed:
					health -= damage;
					break;
			}
			health = Mathf.CeilToInt(health);
		}

		if (health <= 0)
		{
			health = 0;
			entityController.Die();
		}

		UpdateHealth();
	}

	public override void Heal(float healAmount)
	{
		health += healAmount;
		if (health > maxHealth) health = maxHealth;
	}

	public EntityStatus GetNearestParry()
	{
		if (parryList.Count > 0)
		{
			Vector3 playerPos = entityController.transform.position;
			EntityStatus nearest = parryList[0];
			Vector3 nearestPos = nearest.entityController.transform.position;
			for (int i = 0; i < parryList.Count; i++)
			{
				Vector3 nextPos = parryList[i].entityController.transform.position;
				if (Vector3.Distance(nextPos,playerPos) < Vector3.Distance(nearestPos,playerPos))
				{
					nearest = parryList[i];
					nearestPos = nearest.entityController.transform.position;
				}
			}

			if (Vector3.Distance(nearestPos, playerPos) < 6f)
				return nearest;
		}
		return null;
	}
}
