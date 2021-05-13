using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
	[SerializeField] EntityStatus status = null;
	public enum HitID { player, enemy, neutral}
	public HitID hitID = HitID.neutral;

	//Return a bool to confirm that the entity has actually been hit
	public bool Damage(Attack.AttackType type, int damage, float knockback, int weight, Vector2 direction, EntityStatus ownerStatus)
	{
		return status.Damage(type, damage, knockback, weight, direction, ownerStatus);
	}
}
