using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpriteSwapper : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;
	public string pathName = "";
	private string currentSheet = "";
	private string currentSprite;

	private Sprite[] sprites;
	private Dictionary<string, Sprite> spriteSheet;

    void Start()
    {
		spriteRenderer = GetComponent<SpriteRenderer>();
		if (pathName != "") LoadSpriteSheet();
    }

    void LateUpdate()
    {
		currentSprite = spriteRenderer.sprite.name;
		if (currentSprite.Split('_')[0] != currentSheet)
		{
			currentSheet = currentSprite.Split('_')[0];
			LoadSpriteSheet();
		}

		if (spriteSheet.ContainsKey(currentSprite))
		{
			spriteRenderer.sprite = spriteSheet[currentSprite];
		}
		else
			Debug.Log(currentSprite);
    }

	public void SwapSprite(string path)
	{
		pathName = path;
		LoadSpriteSheet();
	}

	public void LoadSpriteSheet()
	{
		sprites = Resources.LoadAll<Sprite>("Sprites/" + pathName + "/" + currentSheet);
		spriteSheet = sprites.ToDictionary(sprite => sprite.name, sprite => sprite);
	}
}
