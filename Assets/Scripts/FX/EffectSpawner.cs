using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawner : MonoBehaviour
{
	public static EffectSpawner instance;
	GameObject effectPrefab = null;
	GameObject damagePrefab = null;

    // Start is called before the first frame update
    void Start()
    {
		instance = this;
		effectPrefab = Resources.Load("Prefabs/GeneralEffect") as GameObject;
		damagePrefab = Resources.Load("Prefabs/DamagePop") as GameObject;
	}

	public void PopDamage(Vector3 position, int damage)
	{
		GameObject damagePop = Instantiate(damagePrefab, position, Quaternion.identity);
		DamagePop dmp = damagePop.GetComponent<DamagePop>();
		dmp.Setup(damage);
	}

	public PhysicalEffect SpawnFGEffect(int id, Vector3 position, float scale, Vector3 direction, Color fillColor = new Color())
	{
		GameObject effectObject = Instantiate(effectPrefab, position, Quaternion.Euler(Vector3.zero));
		PhysicalEffect effect = effectObject.GetComponent<PhysicalEffect>();
		effect.ScaleEffect(scale);
		effect.SetID(id);
		effect.direction = direction;

		SpriteRenderer effectSprite = effectObject.GetComponent<SpriteRenderer>();
		effectSprite.color = fillColor;
		return effect;
	}

	public PhysicalEffect SpawnFGEffect(int id, Vector3 position, float scale, Vector3 direction, Color fillColor = new Color(), Color outlineColor = new Color())
	{
		GameObject effectObject = Instantiate(effectPrefab, position, Quaternion.Euler(Vector3.zero));
		PhysicalEffect effect = effectObject.GetComponent<PhysicalEffect>();
		effect.ScaleEffect(scale);
		effect.SetID(id);
		effect.direction = direction;

		SpriteRenderer effectSprite = effectObject.GetComponent<SpriteRenderer>();
		effectSprite.material = Resources.Load("Materials/OutlineAlways") as Material;
		effectSprite.material.SetColor("_OutlineColor", outlineColor);
		effectSprite.color = fillColor;
		return effect;
	}

	public PhysicalEffect SpawnAnchoredGroundEffect(int id, Rigidbody anchor, Vector3 direction = new Vector3(), float offset = 0f, float elevation = 0f)
	{
		GameObject effectObject = Instantiate(effectPrefab, anchor.position, Quaternion.Euler(Vector3.zero));
		PhysicalEffect effect = effectObject.GetComponent<PhysicalEffect>();
		effect.SetAnchor(anchor, direction, offset, elevation);
		effect.SetID(id);
		return effect;
	}

	public PhysicalEffect SpawnGroundEffectDirected(int id, float scale = 4, Vector3 position = new Vector3(), Vector3 direction = new Vector3())
	{
		GameObject effectObject = Instantiate(effectPrefab, position, Quaternion.Euler(Vector3.zero));
		PhysicalEffect effect = effectObject.GetComponent<PhysicalEffect>();
		effect.SetID(id);
		effect.ScaleEffect(scale);
		Animator animator = effectObject.GetComponent<Animator>();
		animator.SetFloat("Horizontal", direction.x);
		animator.SetFloat("Vertical", direction.y);
		return effect;
	}

	public PhysicalEffect SpawnHitEffectOutlined(Vector3 position, float scale ,Color fillColor = new Color(), Color outlineColor = new Color())
	{
		GameObject effectObject = Instantiate(effectPrefab, position, Quaternion.Euler(Vector3.zero));
		PhysicalEffect effect = effectObject.GetComponent<PhysicalEffect>();
		effect.ScaleEffect(scale);
		effect.orientVertically = true;
		effect.direction = Random.insideUnitCircle.normalized;
		effect.SkewEffect();
		effect.SetID(4);

		SpriteRenderer effectSprite = effectObject.GetComponent<SpriteRenderer>();
		effectSprite.sortingLayerName = "FG Effects";
		effectSprite.sortingOrder = 0;
		effectSprite.color = fillColor;
		effectSprite.material = Resources.Load("Materials/OutlineAlways") as Material;
		effectSprite.material.SetColor("_OutlineColor", outlineColor);
		return effect;
	}

	public PhysicalEffect SpawnHitEffect(Vector3 position, float scale, Color fillColor = new Color())
	{
		GameObject effectObject = Instantiate(effectPrefab, position, Quaternion.Euler(Vector3.zero));
		PhysicalEffect effect = effectObject.GetComponent<PhysicalEffect>();
		effect.ScaleEffect(scale);
		effect.orientVertically = true;
		effect.direction = Random.insideUnitCircle.normalized;
		effect.SkewEffect();
		effect.SetID(4);

		SpriteRenderer effectSprite = effectObject.GetComponent<SpriteRenderer>();
		effectSprite.sortingLayerName = "FG Effects";
		effectSprite.sortingOrder = 0;
		effectSprite.color = fillColor;
		return effect;
	}

	
}
