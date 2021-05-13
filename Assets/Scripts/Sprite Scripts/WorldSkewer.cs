using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldSkewer
{
	public static void SkewObject(GameObject ObjectToSkew, bool vertical)
	{
		SkewScale(ObjectToSkew);
		SkewAngle(ObjectToSkew, vertical);
	}

	public static void SkewScale(GameObject ObjectToSkew)
	{
		//Scale Object by 1.414
		Vector3 newScale = ObjectToSkew.transform.localScale;
		newScale.y *= 1.414f;
		ObjectToSkew.transform.localScale = newScale;
	}

	public static void SkewScaleWithDirection(GameObject ObjectToSkew, Vector3 direction)
	{
		Vector3 newScale = ObjectToSkew.transform.localScale;
		Vector3 dir = direction.normalized;
		newScale.y *= 1 + (0.414f * Mathf.Abs(dir.y));
		//newScale.x *= 1 + 0.414f * Mathf.Abs(dir.x);
		ObjectToSkew.transform.localScale = newScale;
	}

	public static void SkewAngle(GameObject ObjectToSkew, bool vertical)
	{
		//Rotate based on orientation
		Vector3 temp = ObjectToSkew.transform.eulerAngles;
		temp.x = vertical ? -45 : 45;
		ObjectToSkew.transform.rotation = Quaternion.Euler(temp);
	}

	public static float GetSkewAngle(bool vertical)
	{
		return vertical ? -45 : 45;
	}
}
