using System.Collections;
using System.Collections.Generic;

public interface IDroppedWeapon
{
	bool Pickable();
	Weapon GetWeapon();
	void DestroySelf();
	void SetSpriteMat(string materialAddress);
}

public interface IBreakable
{
	void BreakObject();
}