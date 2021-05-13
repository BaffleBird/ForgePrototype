using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationAlert : MonoBehaviour
{
	public bool[] flag = new bool[8];
	/* CURRENT TYPICAL FLAG INDEX
	 * 0	:	End of Animation
	 * 1	:	Autostep Flag
	 */
	public int animationPing = 0;

	public void TriggerFlag(int flagNumber)
	{
		flag[flagNumber] = true;
	}

	public void Ping(int pingAmount)
	{
		animationPing += pingAmount;
	}

	public void ResetAlerts()
	{
		for (int i = 0; i < flag.Length; i++) { flag[i] = false; }
		animationPing = 0;
	}
}
