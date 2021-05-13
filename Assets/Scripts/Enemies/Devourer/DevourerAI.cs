using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevourerAI : EntityInputs
{
	public override void UpdateInput()
	{
		target = GameObject.FindWithTag("Player");
		RaycastHit hit;
		if (Physics.Linecast(transform.position, target.transform.position, out hit) && GetPointerDistance() < 12)
		{
			if (hit.transform.tag != "Player")
				target = null;
		}
	}

	public override Vector2 GetPointerDirection()
	{
		if (target != null)
		{
			Vector2 targetDirection = target.transform.position - entityController.transform.position;
			return targetDirection.normalized;
		}
		else
		{
			float x = Random.Range(-5, 5);
			float y = Random.Range(-5, 5);
			return new Vector2(x, y).normalized;
		}
	}

	public override float GetPointerDistance()
	{
		if (target != null)
			return Vector2.Distance(target.transform.position, entityController.transform.position);
		return Random.Range(0, 5);
	}
}
