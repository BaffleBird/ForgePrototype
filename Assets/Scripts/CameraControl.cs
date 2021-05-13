using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
	[SerializeField] Transform target = null;
	[SerializeField] float smoothTime = 0.1f;
	[SerializeField] float zDistance = 10f;
	[SerializeField] float maxDistance = 1f;
	Vector2 mousePos, targetPos, refvel;

	private void Awake()
	{
		Camera cam = GetComponent<Camera>();
		cam.depthTextureMode = DepthTextureMode.Depth;
	}

	private void FixedUpdate()
	{
		if (target != null)
		{
			mousePos = GetMousePosition();
			targetPos = GetTargetPosition();
			UpdateCameraPosition();
		}
	}

	Vector2 GetMousePosition()
	{
		Vector2 point = Camera.main.ScreenToViewportPoint(Input.mousePosition);
		point -= (Vector2.one / 2);
		float max = 0.9f;
		if(Mathf.Abs(point.x) > max || Mathf.Abs(point.y) > max)
			point = point.normalized;
		return point;
	}

	Vector2 GetTargetPosition()
	{
		Vector2 offset = mousePos * maxDistance;
		Vector2 point = (Vector2)target.position + offset;
		return point;
	}

	void UpdateCameraPosition()
	{
		Vector2 newPos = Vector2.SmoothDamp(transform.position, targetPos, ref refvel, smoothTime);
		transform.position = new Vector3(newPos.x, newPos.y, target.position.z - zDistance);
	}
}
