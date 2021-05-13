using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Contains inputs for Players (and AI)
/// </summary>
public class EntityInputs : MonoBehaviour
{
	public EntityController entityController;
	[HideInInspector] public Vector2 movementInput;
	[HideInInspector] public bool xDown, xHold, xRelease, xHeld, xTap;
	[HideInInspector] public bool aDown, aHold, aRelease;
	[HideInInspector] public bool bDown, bHold, bRelease;
	[HideInInspector] public bool yDown, yHold, yRelease;
	public GameObject target = null;

	void Update() { UpdateInput(); }

	public virtual void ConnectEntityController (EntityController entityController)
	{
		this.entityController.entityInput = null;
		this.entityController = entityController;
		this.entityController.entityInput = this;
	}

	public virtual void UpdateInput() { }
	public virtual Vector2 GetPointerDirection() { return Vector2.zero; }
	public virtual float GetPointerDistance() { return 0f; }
}

/// <summary>
/// State Machine Controller
/// </summary>
public abstract class EntityController : MonoBehaviour
{
	[Header("Entity Components")]
	public EntityInputs entityInput;
	public EntityStatus entityStatus;

	[Header("Animation Components")]
	public Animator characterAnimator;
	public AnimationAlert animationAlert;
	public Rigidbody rigidBody;

	[Header("Sprite Components")]
	public SpriteRenderer characterSprite;

	//States
	protected EntityState currentState;
	[HideInInspector]public string previousState = "";
	protected Dictionary<string, EntityState> states = new Dictionary<string, EntityState>();

	protected virtual void Update()
	{
		UpdateConstants();

		if (currentState != null)
			currentState.MainUpdate();
	}

	protected virtual void FixedUpdate()
	{
		if (currentState != null)
			rigidBody.MovePosition(rigidBody.position + (currentState.MotionUpdate() * Time.fixedDeltaTime));
	}

	protected virtual void UpdateConstants()
	{
		foreach (EntityState state in states.Values)
			state.ConstantUpdate();
	}

	public virtual void SetState(string stateName)
	{
		previousState = currentState.EndState();
		currentState = states[stateName];
		currentState.StartState();
	}

	public string GetStateName()
	{
		if (currentState != null)
			return currentState.stateName;
		return "N/A";
	}

	public virtual void PlayAnimation(string stateName) { characterAnimator.Play(stateName); }

	public virtual EntityState GetState(string stateName)
	{
		return states[stateName];
	}

	public virtual void Damage(Attack.AttackType type, int damage, float knockback, int weight, Vector2 direction, EntityStatus ownerStatus) { }
	public virtual void Parry() { }
	public virtual void Die() { }
	public virtual void DestroySelf()
	{
		Destroy(gameObject);
	}
	public virtual void SetSpriteMat(string materialAddress = "default")
	{
		if (materialAddress == "default")
		{
			Material newMat = Resources.Load("Materials/BaseSprite") as Material;
			characterSprite.material = newMat;
		}
		else
		{
			Material newMat = Resources.Load("Materials/" + materialAddress) as Material;
			characterSprite.material = newMat;
		}
	}

	/// <summary>
	/// Flash Module
	/// </summary>
	protected float flashTime;
	protected float flashAmount;
	protected Color flashColor;
	public virtual void Flash(float time, float amount, Color col)
	{
		flashTime = time;
		flashAmount = amount;
		flashColor = col;
		SetSpriteMat("Flash");
	}
	public virtual void IncrementFlash()
	{
		if (flashAmount > 0)
			flashAmount -= 0.1f;
		flashTime -= Time.deltaTime;
		characterSprite.material.SetColor("_FlashColor", flashColor);
		characterSprite.material.SetFloat("_FlashAmount", flashAmount);
		if (flashTime <= 0)
			SetSpriteMat("default");
	}
}

/// <summary>
/// Status for Health and other Stats
/// </summary>
public abstract class EntityStatus : MonoBehaviour
{
	public EntityController entityController;

	public enum HitState { normal, guard, parry, superarmor, i_frame, special }
	public HitState currentHitstate = HitState.normal;

	public float maxHealth;
	public float health;
	public event Action<float> HealthUpdate;
	public void UpdateHealth() { HealthUpdate(health); }

	protected virtual void Update() { }
	public virtual bool Damage(Attack.AttackType type, int damage, float knockback, int weight, Vector2 direction, EntityStatus ownerStatus) { return true; }
	public virtual void Heal(float healAmount) { }
	public virtual void Parry() { entityController.Parry(); }
	public virtual void Die() { entityController.Die(); }
}