using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class PlayerControl : EntityController
{
	public PlayerStatus playerStats;

	[Header("Animator Components")]
	public Animator weaponAnimator;
	public SpriteSwapper weaponSwapper;
	public Animator shadowAnimator;

	[Header("Sprite Components")]
	public SpriteRenderer weaponSprite;

	//Other
	GameObject throwablePrefab = null;

	[Header("Debug")]
	public TextMeshProUGUI stateDisplay;
	public bool checkState;

	//States
	[HideInInspector]
	PlayerState idle, move, dodge, sprint, slide, combo,
		strong_attack, dash_attack, defense, guard, stun,
		strike, death;

	void Start()
	{
		playerStats.AdjustInventory();
		SpriteEquip();
		throwablePrefab = Resources.Load("Prefabs/ThrownWeapon") as GameObject;

		idle = new IdleState(this); states.Add(idle.stateName, idle);
		move = new MoveState(this); states.Add(move.stateName, move);
		dodge = new DodgeState(this); states.Add(dodge.stateName, dodge);
		sprint = new SprintState(this); states.Add(sprint.stateName, sprint);
		slide = new SlideState(this); states.Add(slide.stateName, slide);
		combo = new ComboState(this); states.Add(combo.stateName, combo);
		strong_attack = new StrongRedirectState(this); states.Add(strong_attack.stateName, strong_attack);
		dash_attack = new DashRedirectState(this); states.Add(dash_attack.stateName, dash_attack);
		defense = new DefensiveRedirectState(this); states.Add(defense.stateName, defense);
		guard = new GuardState(this); states.Add(guard.stateName, guard);
		stun = new StunState(this); states.Add(stun.stateName, stun);
		strike = new StrikeState(this); states.Add(strike.stateName, strike);
		death = new DeathState(this); states.Add(death.stateName, death);

		currentState = idle;
		currentState.StartState();
	}

	protected override void Update()
	{
		UpdateConstants();

		if (currentState != null)
			currentState.MainUpdate();

		if (checkState)
			stateDisplay.text = GetStateName();

		if (flashTime > 0)
			IncrementFlash();
	}

	protected override void FixedUpdate()
	{
		if (currentState != null)
			rigidBody.MovePosition(rigidBody.position + (currentState.MotionUpdate() * Time.fixedDeltaTime));
	}

	GameObject lastNearest = null;
	private void LateUpdate()
	{
		UpdateWeaponAnimator();
		shadowAnimator.SetInteger("Shadow Size", 2); // for dodging, attacks, and defense manuevering

		GameObject nextNearest = GetNearestWeapon();
		if (nextNearest != null)
		{
			if (lastNearest != null)
			{
				lastNearest.GetComponent<IDroppedWeapon>().SetSpriteMat("default");
				if (nextNearest.GetComponent<IDroppedWeapon>().Pickable())
					nextNearest.GetComponent<IDroppedWeapon>().SetSpriteMat("Outline");
			}
			lastNearest = nextNearest;
		}
		else if (lastNearest != null)
		{
			lastNearest.GetComponent<IDroppedWeapon>().SetSpriteMat("default");
		}
	}

	protected override void UpdateConstants()
	{
		dodge.ConstantUpdate();
	}

	public void UpdateWeaponAnimator()
	{
		if (playerStats.currentWeapon.WeaponType != Weapon.Type.Unarmed)
		{
			weaponAnimator.SetFloat("Horizontal", characterAnimator.GetFloat("Horizontal"));
			weaponAnimator.SetFloat("Vertical", characterAnimator.GetFloat("Vertical"));
			weaponAnimator.SetInteger("State", characterAnimator.GetInteger("State"));
			weaponAnimator.SetInteger("Attack Step", characterAnimator.GetInteger("Attack Step"));
			weaponAnimator.SetBool("Stun Recovery", characterAnimator.GetBool("Stun Recovery"));
			weaponAnimator.SetInteger("Weapon Type", (int)playerStats.currentWeapon.WeaponType);
		}
	}

	void SpriteEquip()
	{
		if (playerStats.currentWeapon.WeaponType != Weapon.Type.Unarmed)
		{
			weaponAnimator.gameObject.SetActive(true);
			weaponSwapper.SwapSprite(playerStats.currentWeapon.WeaponType.ToString() + "/" + playerStats.currentWeapon.Name);
		}
		else
		{
			weaponAnimator.gameObject.SetActive(false);
		}
		characterAnimator.SetInteger("Weapon Type", (int)playerStats.currentWeapon.WeaponType);
		weaponAnimator.SetInteger("Weapon Type", (int)playerStats.currentWeapon.WeaponType);
	}

	public override void SetSpriteMat(string materialAddress)
	{
		
		if (materialAddress == "default")
		{
			Material newMat = Resources.Load("Materials/BaseSprite") as Material;
			characterSprite.material = newMat;
			weaponSprite.material = newMat;
		}
		else
		{
			Material newMat = Resources.Load("Materials/" + materialAddress) as Material;
			characterSprite.material = newMat;
			weaponSprite.material = newMat;
		}
	}

	public override void PlayAnimation(string stateName)
	{
		characterAnimator.Play(stateName);
		if (playerStats.currentWeapon.WeaponType != Weapon.Type.Unarmed)
		{
			weaponAnimator.Play(stateName);
		}
			
	}

	public void Throw_Pickup_Weapon()
	{
		if (playerStats.currentWeapon.WeaponType != Weapon.Type.Unarmed) // Throw weapon
		{
			EffectSpawner.instance.SpawnAnchoredGroundEffect(3, rigidBody, entityInput.GetPointerDirection(), 0.5f, 0.5f).ScaleEffect(3f);
			SoundMaker.i.PlaySound("TinySwing", transform.position, 0.5f);

			GameObject weaponPrefab = Instantiate(throwablePrefab, transform.position, Quaternion.Euler(Vector3.zero));
			//WorldSkewer.SkewObject(weaponPrefab, false);
			ThrownWeapon thrownWeapon = weaponPrefab.GetComponent<ThrownWeapon>();
			thrownWeapon.SetWeapon(playerStats.currentWeapon);
			thrownWeapon.ScaleAttack(2.25f);
			thrownWeapon.SetTrajectory(entityInput.GetPointerDirection(), 18f, 0.6f, 0.5f);

			//Discard Weapon
			playerStats.DiscardWeapon();
		}
		else // Pick up weapon
		{
			GameObject nearestWeapon = GetNearestWeapon();
			if (nearestWeapon != null)
			{
				SoundMaker.i.PlaySound("TinySwing", transform.position, 0.5f);
				playerStats.currentWeapon = nearestWeapon.GetComponent<IDroppedWeapon>().GetWeapon();
				nearestWeapon.GetComponent<IDroppedWeapon>().DestroySelf();
			}	
		}
		SpriteEquip();
		if (weaponAnimator.gameObject.activeSelf)
			weaponAnimator.Play(GetStateName());
		UpdateWeaponAnimator();
	}

	public void DropWeapon()
	{
		if (playerStats.currentWeapon.WeaponType != Weapon.Type.Unarmed) // Throw weapon
		{
			//EffectSpawner.instance.SpawnAnchoredGroundEffect(3, rigidBody, entityInput.GetPointerDirection(), 0.5f, 0.5f).ScaleEffect(3f);
			SoundMaker.i.PlaySound("TinySwing", transform.position, 0.5f);

			GameObject weaponPrefab = Instantiate(throwablePrefab, transform.position, Quaternion.Euler(Vector3.zero));
			//WorldSkewer.SkewObject(weaponPrefab, false);
			ThrownWeapon thrownWeapon = weaponPrefab.GetComponent<ThrownWeapon>();
			thrownWeapon.SetWeapon(playerStats.currentWeapon);
			thrownWeapon.ScaleAttack(2.25f);
			thrownWeapon.SetTrajectory(entityInput.GetPointerDirection(), 4f, 0.6f, 0.5f);

			//Discard Weapon
			playerStats.DiscardWeapon();
		}
		SpriteEquip();
		UpdateWeaponAnimator();
	}

	public GameObject GetNearestWeapon()
	{
		GameObject[] weapons = GameObject.FindGameObjectsWithTag("Weapon");
		
		if (playerStats.currentWeapon.WeaponType == Weapon.Type.Unarmed && weapons.Length > 0)
		{
			GameObject nearest = weapons[0];
			float distance = Vector3.Distance(transform.position, weapons[0].transform.position);
			foreach (GameObject weapon in weapons)
			{
				float newDistance = Vector3.Distance(transform.position, weapon.transform.position);
				if (newDistance < distance && weapon.GetComponent<IDroppedWeapon>().GetWeapon())
				{
					nearest = weapon;
					distance = newDistance;
				}
					
			}
			if (distance < 1.5f)
					return nearest;
		}
		return null;
	}

	public override void Damage(Attack.AttackType type, int damage, float knockback, int weight, Vector2 direction, EntityStatus ownerStatus)
	{
		Vector3 pos = transform.position + new Vector3(0, 0.5f, 0.5f);
		switch ( playerStats.currentHitstate )
		{
			case EntityStatus.HitState.parry:
				SoundMaker.i.PlaySound("Parry", transform.position, 0.5f);
				SoundMaker.i.PlaySound("Parry2", transform.position, 0.36f);
				defense.PassDamage(damage, knockback, weight, direction);
				break;
			case EntityStatus.HitState.normal:
				SoundMaker.i.PlaySound("Stab", transform.position, 0.5f);
				SoundMaker.i.PlaySound("Hit", transform.position, 0.3f);
				stun.PassDamage(0f, knockback, weight, direction);
				Flash(0.9f, 0.9f, Color.red);
				SetState("Stun");
				EffectSpawner.instance.SpawnHitEffect(pos, 3, Color.red);
				break;
			case EntityStatus.HitState.guard:
				if (playerStats.currentWeapon.WeaponType == Weapon.Type.Unarmed)
				{
					SoundMaker.i.PlaySound("Stab", transform.position, 0.5f);
					SoundMaker.i.PlaySound("Hit", transform.position, 0.36f);
					stun.PassDamage(0f, knockback, weight, direction);
					Flash(0.8f, 0.8f, Color.red);
					SetState("Stun");
					EffectSpawner.instance.SpawnHitEffect(pos, 3, Color.red);
				}
				else
				{
					SoundMaker.i.PlaySound("Clank", transform.position, 0.3f);
					SoundMaker.i.PlaySound("Stab", transform.position, 0.5f);
					SetState("Guard");
					guard.PassDamage(0f, knockback, weight, direction);
					EffectSpawner.instance.SpawnHitEffect(pos, 3f, new Color(1f, 0.9f, 0.4f));
				}
				break;
			case EntityStatus.HitState.superarmor:
				SoundMaker.i.PlaySound("Stab", transform.position, 0.3f);
				Flash(0.8f, 0.8f, new Color(0.85f, 0, 0, 1));
				EffectSpawner.instance.SpawnHitEffect(pos, 2, Color.red);
				break;
			case EntityStatus.HitState.special:
				break;
		}
	}

	public override void Die()
	{
		SetState("Death");
	}
	public event Action OnPlayerDeath;
	public void DieDie()
	{
		OnPlayerDeath();
	}

	public override void Flash(float time, float amount, Color col)
	{
		flashTime = time;
		flashAmount = amount;
		flashColor = col;
		SetSpriteMat("Flash");
	}
	public override void IncrementFlash()
	{
		SetSpriteMat("Flash");
		if (flashAmount > 0)
			flashAmount -= 0.1f;
		flashTime -= Time.deltaTime;
		characterSprite.material.SetColor("_FlashColor", flashColor);
		weaponSprite.material.SetColor("_FlashColor", flashColor);
		characterSprite.material.SetFloat("_FlashAmount", flashAmount);
		weaponSprite.material.SetFloat("_FlashAmount", flashAmount);
		if (flashTime <= 0)
			SetSpriteMat("default");
	}
}

public class PlayerState : EntityState
{
	public PlayerControl playerControl;
	public PlayerState(PlayerControl playerController) { playerControl = playerController; }
}