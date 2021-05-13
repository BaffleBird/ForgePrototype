using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputs : EntityInputs
{
	[SerializeField]
	PlayerControl playerControl = null;

	//*Inputs to Update
	//Vector2 movementInput;
	//bool xDown, xHold, xRelease;
	//bool yDown, yHold, yRelease;
	//bool aDown, aHold, aRelease;
	//bool bDown, bHold, bRelease;

	public float xHoldCount = 0;
	public bool xHolding;

	void Update()
	{
		UpdateInput();
	}

	public override void UpdateInput()
	{
		xTap = false;
		xHeld = false;

		movementInput.x = Input.GetAxisRaw("Horizontal");
		movementInput.y = Input.GetAxisRaw("Vertical");

		//a Input
		aDown = Input.GetButtonDown("Jump");

		//b Input;
		bDown = Input.GetButtonDown("Fire2");
		bHold = Input.GetButton("Fire2");
		bRelease = Input.GetButtonUp("Fire2");

		//x Input
		xDown = Input.GetButtonDown("Fire1");
		xHold = Input.GetButton("Fire1");
		xRelease = Input.GetButtonUp("Fire1");

		if (xDown)
			xHolding = true;

		if (xHolding)
			xHoldCount += Time.deltaTime;

		if (xHolding && xRelease)
		{
			if (xHoldCount < 0.18)
				xTap = true;
			xHoldCount = 0;
			xHolding = false;
		}
		else if (xHoldCount >= 0.18 && xHold)
		{
			xHeld = true;
			xHoldCount = 0;
			xHolding = false;
		}

		//y Input;
		yDown = Input.GetButtonDown("Fire3");
		yHold = Input.GetButton("Fire3");
		yRelease = Input.GetButtonUp("Fire3");

		if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
		if (Input.GetKeyDown(KeyCode.Backspace)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public override Vector2 GetPointerDirection()
	{
		Vector2 direction = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		direction = Camera.main.ScreenToWorldPoint(new Vector3(direction.x, direction.y, Camera.main.nearClipPlane));
		direction = direction - (Vector2)playerControl.gameObject.transform.position;
		return direction.normalized;
	}

	public override float GetPointerDistance()
	{
		Vector2 mouseDist = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		return Vector2.Distance(playerControl.rigidBody.position, mouseDist);
	}

	public override void ConnectEntityController(EntityController entityController)
	{
		this.entityController.entityInput = null;
		this.entityController = entityController;
		this.entityController.entityInput = this;
	}
}
