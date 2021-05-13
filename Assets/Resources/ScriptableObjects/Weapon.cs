using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Weapon : ScriptableObject
{
    public enum Type { Unarmed, Longsword, Spear, Greatsword, Shortsword}

	[SerializeField] Type weaponType;
	public Type WeaponType { get { return weaponType; } private set { weaponType = value; } }

	[SerializeField] string weaponName;
	public string Name { get { return weaponName; } private set { weaponName = value; } }

	[SerializeField] string inGameName;
	public string InGameName { get { return inGameName; } private set { inGameName = value; } }

	public GameObject weaponEffect;

	public float weaponPower;
	public int weaponWeight;

	public int MaxCombo()
	{
		switch (WeaponType)
		{
			case Type.Unarmed:
				return 3;
			case Type.Longsword:
				return 5;
			case Type.Spear:
				return 4;
			case Type.Greatsword:
				return 3;
			case Type.Shortsword:
				return 2;
		}
		return 0;
	}
}
