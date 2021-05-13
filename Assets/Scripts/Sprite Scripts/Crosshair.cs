using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
	void Start()
	{
		Material mat = Resources.Load("Materials/OutlineAlways") as Material;
		Image crosshair = GetComponent<Image>();
		crosshair.material = mat;
		crosshair.material.SetColor("_OutlineColor", new Color(0,0,0,0.5f));
	}
	void LateUpdate()
    {
		if (Cursor.visible)
			Cursor.visible = false;
		transform.position = Input.mousePosition;
	}
}
